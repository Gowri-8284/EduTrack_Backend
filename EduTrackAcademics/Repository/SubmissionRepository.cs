using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository
{
	public class SubmissionRepository : ISubmissionRepository
	{
		private readonly EduTrackAcademicsContext _context;

		public SubmissionRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		public async Task<bool> IsStudentEnrolledAsync(string studentId)
		{
			return await _context.Enrollment
				.AnyAsync(e => e.StudentId == studentId);
		}

		public async Task<List<ViewAssessmentDto>> GetAssessmentsByStudentIdAsync(string studentId)
		{

			var enrolledCourseIds = await _context.Enrollment
			.Where(e => e.StudentId == studentId)
			.Select(e => e.CourseId)
			.ToListAsync();

			var assessments = await _context.Assessments
				.Where(a => enrolledCourseIds.Contains(a.CourseId))
				.Include(a => a.Course)
				.Select(a => new ViewAssessmentDto
				{
					AssessmentID = a.AssessmentID,
					CourseID = a.CourseId,
					CourseName = a.Course.CourseName,
					Type = a.Type,
					MaxMarks = a.MaxMarks,
					DueDate = a.DueDate,
					Status = a.Status
				})
				.ToListAsync();

			return assessments;
		}

		public async Task<bool> IsAssessmentSubmittedAsync(String studentId, string assessmentId)
		{
			var exists = await _context.Submission.AnyAsync(s => s.StudentID == studentId
					&& s.AssessmentId == assessmentId);

			if (exists)
			{
				return true;
			}
			return false;
		}

		public async Task<ViewAssessmentDto> GetAssessmentByIdAsync(string assessmentId)
		{
			return await _context.Assessments
				.Where(a => a.AssessmentID == assessmentId)
				.Select(a => new ViewAssessmentDto
				{
					AssessmentID = a.AssessmentID,
					CourseID = a.CourseId,
					Type = a.Type,
					MaxMarks = a.MaxMarks,
					DueDate = a.DueDate,
					Status = a.Status
				})
				.FirstOrDefaultAsync();
		}

		public async Task<List<string>> GetStudentCourseIdsAsync(string studentId)
		{
			return await _context.Enrollment
				.Where(e => e.StudentId == studentId)
				.Select(e => e.CourseId)
				.ToListAsync();
		}

		public async Task<List<StartAssessmentDto>> GetQuestionsByAssessmentIdAsync(string assessmentId)
		{
			return await _context.Questions
				.Where(q => q.AssessmentId == assessmentId)
				.OrderBy(q => q.OrderNo)
				.Select(q => new StartAssessmentDto
				{
					AssessmentId = q.AssessmentId,
					QuestionId = q.QuestionId,
					QuestionType = q.QuestionType,
					QuestionText = q.QuestionText,
					OptionA = q.OptionA,
					OptionB = q.OptionB,
					OptionC = q.OptionC,
					OptionD = q.OptionD,
					Marks = q.Marks,
					OrderNo = q.OrderNo
				})
				.ToListAsync();
		}

		public async Task InsertOrUpdateAnswerAsync(StudentAnswerDto dto)
		{
			var existingAnswer = await _context.StudentAnswer
				.FirstOrDefaultAsync(x =>
					x.StudentId == dto.StudentId &&
					x.AssessmentId == dto.AssessmentId &&
					x.QuestionId == dto.QuestionId);

			if (existingAnswer != null)
			{
				// Update existing answer
				existingAnswer.Answer = dto.Answer;
				existingAnswer.createdDate = DateTime.Now;
			}
			else
			{
				// Insert new answer
				int count = await _context.StudentAnswer.CountAsync();
				string newId = $"SA{(count + 1):D3}";
				var answer = new StudentAnswer
				{
					StudentAnswerId = newId,
					StudentId = dto.StudentId,
					AssessmentId = dto.AssessmentId,
					QuestionId = dto.QuestionId,
					Answer = dto.Answer,
					createdDate = DateTime.Now
				};

				await _context.StudentAnswer.AddAsync(answer);
			}

			await _context.SaveChangesAsync();
		}

		public async Task<int> GetSubmissionCountAsync()
		{
			return await _context.Submission.CountAsync();
		}

		// SUBMIT ASSESSMENT
		public async Task<string> SubmitAssessmentAsync(Submission submission)
		{

			await _context.Submission.AddAsync(submission);
			await _context.SaveChangesAsync();

			return submission.SubmissionId;
		}

		public async Task AddFeedbackAsync(SubmitFeedbackDto dto)
		{
			var submission = await _context.Submission
				.FirstOrDefaultAsync(s => s.SubmissionId == dto.submissionId);
			if (submission != null)
			{
				submission.Feedback = dto.Feedback;
				await _context.SaveChangesAsync();
			}
		}

		// CALCULATE SCORE & PERCENTAGE
		public async Task<UpdateSubmissionDto> CalculateScoreAsync(string studentId, string assessmentId)
		{
			// Load questions for the assessment
			var questions = await _context.Questions
				.Where(q => q.AssessmentId == assessmentId)
				.ToListAsync();

			// Load student's answers for the assessment
			var studentAnswers = await _context.StudentAnswer
				.Where(a => a.StudentId == studentId && a.AssessmentId == assessmentId)
				.ToListAsync();

			var submission_id = await _context.Submission
				.FirstOrDefaultAsync(s => s.StudentID == studentId && s.AssessmentId == assessmentId);

			int score = 0;

			// Calculate score based on correct options
			foreach (var question in questions)
			{
				var answer = studentAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
				if (answer != null && answer.Answer == question.CorrectOption)
				{
					score += question.Marks;
				}
			}

			// Retrieve assessment's MaxMarks safely from the Assessments DbSet
			var assessmentMaxMarks = await _context.Assessments
				.Where(a => a.AssessmentID == assessmentId)
				.Select(a => a.MaxMarks)
				.FirstOrDefaultAsync();

			// Compute percentage, avoid division by zero
			double percentage = (assessmentMaxMarks > 0) ? ((double)score / assessmentMaxMarks) * 100.0 : 0.0;

			var result = new UpdateSubmissionDto
			{
				StudentId = studentId,
				AssessmentId = assessmentId,
				submissionId = submission_id.SubmissionId,
				Score = score,
				Percentage = percentage
			};

			return result;
		}

		// UPDATE SCORE 
		public async Task<UpdateSubmissionDto> UpdateSubmissionAsync(UpdateSubmissionDto dto)
		{
			var submission = await _context.Submission
				.FirstOrDefaultAsync(s => s.SubmissionId == dto.submissionId);

			if (submission != null)
			{
				submission.Score = dto.Score;

				await _context.SaveChangesAsync();
			}

			return dto;
		}


		public async Task<List<Submission>> GetSubmissionsByAssessmentIdAsync(string assessmentId)
		{
			return await _context.Submission
				.Where(s => s.AssessmentId == assessmentId)
				.ToListAsync();
		}

		public async Task<List<Submission>> GetAllSubmissionsAsync()
		{
			// Returns all rows and all columns (SubmissionId, Date, Score, Feedback, AssessmentId, StudentID)
			return await _context.Submission.ToListAsync();
		}
	}
}
