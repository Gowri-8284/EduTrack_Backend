using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Humanizer;
using NuGet.Protocol.Core.Types;

namespace EduTrackAcademics.Services
{
	public class SubmissionService : ISubmissionService
	{
		private readonly ISubmissionRepository _repo;

		public SubmissionService(ISubmissionRepository repo)
		{
			_repo = repo;
		}

		public async Task<List<ViewAssessmentDto>> GetStudentAssessmentsAsync(string studentId)
		{
			bool isEnrolled = await _repo.IsStudentEnrolledAsync(studentId);

			if (!isEnrolled)
			{
				throw new InvalidOperationException("Student is not enrolled in any courses");
			}
			return await _repo.GetAssessmentsByStudentIdAsync(studentId);
		}

		public async Task<List<StartAssessmentDto>> StartAssessmentAsync(string studentId, string assessmentId)
		{
			// Get enrolled courses
			var courseIds = await
				_repo.GetStudentCourseIdsAsync(studentId);

			if (courseIds == null || !courseIds.Any())
				throw new ApplicationException("Student not enrolled in any courses");

			// Get assessment details
			var assessment = await
				_repo.GetAssessmentByIdAsync(assessmentId);

			if (assessment == null)
				throw new ApplicationException("Assessment not found");

			//Check if assessment belongs to enrolled course
			if (!courseIds.Contains(assessment.CourseID))
				throw new ApplicationException("Student not authorized for this assessment");

			// Validate status and due date
			if (assessment.Status != "Open" && assessment.DueDate < DateTime.Now)
				throw new AssessmentInactiveException("Assessment is not active");

			if (await _repo.IsAssessmentSubmittedAsync(studentId, assessmentId))
			{
				throw new AssessmentAttemptedException("Assessment already submitted");
			}

			// Load questions
			var questions = await
				_repo.GetQuestionsByAssessmentIdAsync(assessmentId);

			if (!questions.Any())
				throw new ApplicationException("No questions available");

			return questions;
		}

		public async Task InsertOrUpdateAnswerAsync(StudentAnswerDto dto)
		{

			if (await _repo.IsAssessmentSubmittedAsync(dto.StudentId, dto.AssessmentId))
			{
				throw new AssessmentAttemptedException("Assessment already submitted");
			}

			await _repo.InsertOrUpdateAnswerAsync(dto);
		}

		// Submit Assessment + Calculate Score
		public async Task<string> SubmitAssessmentAsync(SubmitAssessmentDto dto)
		{
			if (await _repo.IsAssessmentSubmittedAsync(dto.StudentId, dto.AssessmentId))
			{
				throw new AssessmentAttemptedException("Assessment already submitted");
			}

			// Create Submission entry
			int count = await _repo.GetSubmissionCountAsync();
			string submissionid = $"SB{(count + 1):D3}";
			var submission = new Submission
			{
				SubmissionId = submissionid,
				StudentID = dto.StudentId,
				AssessmentId = dto.AssessmentId,
				SubmissionDate = DateTime.Now,
				Score = 0,
				Feedback = ""
			};

			return await _repo.SubmitAssessmentAsync(submission);

		}

		public async Task AddFeedbackAsync(SubmitFeedbackDto dto)
		{

			await _repo.AddFeedbackAsync(dto);
			//await _repo.UpdateSubmissionAsync(UpdateSubmissionDto dto);
			//return res;
		}

		public async Task<UpdateSubmissionDto> UpdateSubmissionAsync(string studentId, string assessmentId)
		{
			var res = await _repo.CalculateScoreAsync(studentId, assessmentId);
			return await _repo.UpdateSubmissionAsync(res);
		}

		public async Task<List<Submission>> GetAllSubmissionsByAssessmentAsync(string assessmentId)
		{
			// Optional: Check if assessment exists in the Assessments table first
			var assessment = await _repo.GetAssessmentByIdAsync(assessmentId);
			if (assessment == null)
			{
				throw new ApplicationException("Assessment not found.");
			}

			return await _repo.GetSubmissionsByAssessmentIdAsync(assessmentId);
		}


		public async Task<List<Submission>> GetAllSubmissionsAsync()
		{
			var submissions = await _repo.GetAllSubmissionsAsync();
			if (submissions == null || !submissions.Any())
			{
				throw new Exception("No submission records found in the database.");
			}
			return submissions;
		}
	}
}
