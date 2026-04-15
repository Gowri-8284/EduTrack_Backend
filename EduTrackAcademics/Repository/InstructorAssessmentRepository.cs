using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository
{
	public class InstructorAssessmentRepository : IInstructorAssessmentRepository
	{
		private readonly EduTrackAcademicsContext _context;

		public InstructorAssessmentRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		// ASSESSMENT

		public async Task<string> GenerateAssessmentIdAsync()
		{
			var last = await _context.Assessments
				.OrderByDescending(a => a.AssessmentID)
				.Select(a => a.AssessmentID)
				.FirstOrDefaultAsync();

			int next = last == null ? 1 : int.Parse(last.Substring(1)) + 1;
			return $"A{next:000}";

		}

		public async Task AddAssessmentAsync(Assessment assessment)
		{
			await _context.Assessments.AddAsync(assessment);
			await _context.SaveChangesAsync();
		}

		public async Task<CourseBatch?> GetBatchByCourseIdAsync(string courseId)
		{
			return await _context.CourseBatches.FirstOrDefaultAsync(b => b.CourseId == courseId);
		}

		public async Task<List<AssessmentResponseDTO>> GetAllAssessmentsAsync()
		{
			return await _context.Assessments
				.Include(a => a.Course) // JOIN Course
				.Select(a => new AssessmentResponseDTO
				{
					AssessmentId = a.AssessmentID,
					CourseId = a.CourseId,
					CourseName = a.Course.CourseName,
					Type = a.Type,
					MaxMarks = a.MaxMarks,
					DueDate = a.DueDate.ToLocalTime(),
					Status = a.DueDate > DateTime.Now ? "Open" : "Closed"
				})
				.ToListAsync();
		}


		public async Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date)
		{
			return await _context.Assessments
				.Where(a => a.DueDate.Date == date.Date)
				.ToListAsync();
		}

		public async Task<Assessment?> GetAssessmentByIdAsync(string id)
		{
			return await _context.Assessments.FirstOrDefaultAsync(a => a.AssessmentID == id);
		}

		public async Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId)
			=> await _context.Assessments
				.Where(a => a.CourseId == courseId)
				.ToListAsync();

		public async Task UpdateAssessmentAsync(Assessment assessment)
		{
			_context.Assessments.Update(assessment);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAssessmentAsync(Assessment assessment)
		{
			_context.Assessments.Remove(assessment);
			await _context.SaveChangesAsync();

		}

		// QUESTIONS

		public async Task<string> GenerateQuestionIdAsync()
		{
			var last = await _context.Questions
				.OrderByDescending(q => q.QuestionId)
				.Select(q => q.QuestionId)
				.FirstOrDefaultAsync();

			int next = last == null ? 1 : int.Parse(last.Substring(1)) + 1;
			return $"Q{next:000}";
		}

		public async Task<bool> QuestionExistsAsync(string id)
			=> await _context.Questions.AnyAsync(q => q.QuestionId == id);

		public async Task AddQuestionAsync(Question question)
		{
			await _context.Questions.AddAsync(question);
			await _context.SaveChangesAsync();
		}

		public async Task<Question?> GetQuestionByIdAsync(string id)
			=> await _context.Questions.FindAsync(id);

		public async Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId)
			=> await _context.Questions
				.Where(q => q.AssessmentId == assessmentId)
				.ToListAsync();

		public async Task UpdateQuestionAsync(Question question)
		{
			_context.Questions.Update(question);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteQuestionAsync(Question question)
		{
			_context.Questions.Remove(question);
			await _context.SaveChangesAsync();
		}

		// fetching data

		public async Task<Submission> GetSubmission(string studentId, string assessmentId)
		{
			return await _context.Submission
				.FirstOrDefaultAsync(s => s.StudentID == studentId && s.AssessmentId == assessmentId);
		}

		public async Task<bool> ExistsSubmission(string studentId, string assessmentId)
		{
			return await _context.Submission
				.AnyAsync(s => s.StudentID == studentId
							&& s.AssessmentId == assessmentId);
		}

		public async Task AddSubmission(Submission submission)
		{
			await _context.Submission.AddAsync(submission);

			// Save internally (not exposed outside)
			await _context.SaveChangesAsync();
		}

		public async Task<int> GetTotalMarks(string assessmentId)
		{
			return await _context.Questions
				.Where(q => q.AssessmentId == assessmentId)
				.SumAsync(q => q.Marks);
		}

		public async Task<List<Submission>> GetStudentSubmissions(string studentId)
		{
			return await _context.Submission
				.Where(s => s.StudentID == studentId)
				.ToListAsync();
		}

		public IEnumerable<AcademicYearCourseResponseDTO> GetCoursesByAcademicYear(string academicYearId)
		{
			var courses = _context.Course
				.Where(c => c.AcademicYearId == academicYearId)
				.Select(c => new AcademicYearCourseResponseDTO
				{
					CourseId = c.CourseId,
					CourseName = c.CourseName,
					AcademicYearId = c.AcademicYearId
				})
				.ToList();

			if (!courses.Any())
				throw new ApplicationException("No courses found for this academic year");

			return courses;
		}

		public async Task<int> GetTotalMarksByAssessmentIdAsync(string assessmentId)
		{
			return await _context.Questions
				.Where(q => q.AssessmentId == assessmentId)
				.SumAsync(q => (int?)q.Marks) ?? 0;
		}

		public async Task<List<InstructorCourseBatchDTO>> GetCoursesByInstructorAsync(string instructorId)
		{
			return await (
				from b in _context.CourseBatches
				join c in _context.Course on b.CourseId equals c.CourseId
				where b.InstructorId == instructorId
				select new InstructorCourseBatchDTO
				{
					BatchId = b.BatchId,
					CourseId = c.CourseId,
					CourseName = c.CourseName,
					IsActive = b.IsActive
				}
			).ToListAsync();
		}

		public async Task<AssessmentResponsesCountDTO> GetAssessmentStatusAsync(string assessmentId)
		{
			// Get assessment
			var assessment = await _context.Assessments
				.FirstOrDefaultAsync(a => a.AssessmentID == assessmentId);

			if (assessment == null)
				throw new Exception("Assessment not found");

			// Get batch using CourseId
			var batch = await _context.CourseBatches
				.FirstOrDefaultAsync(b => b.CourseId == assessment.CourseId);

			if (batch == null)
				throw new Exception("Batch not found");

			// Total students
			int totalStudents = batch.CurrentStudents;

			// Submitted count (SubmissionId exists)
			int submittedCount = await _context.Submission
				.CountAsync(s => s.AssessmentId == assessmentId);

			// Pending count
			int pendingCount = totalStudents - submittedCount;

			return new AssessmentResponsesCountDTO
			{
				AssessmentId = assessmentId,
				BatchId = batch.BatchId,
				TotalStudents = totalStudents,
				SubmittedCount = submittedCount,
				PendingCount = pendingCount
			};
		}

		public async Task<List<SubmissionDetailsDTO>> GetSubmissionDetailsAsync(string assessmentId)
		{
			// Get all submissions for assessment
			var submissions = await _context.Submission
				.Where(s => s.AssessmentId == assessmentId)
				.ToListAsync();

			// Get all questions for assessment
			var questions = await _context.Questions
				.Where(q => q.AssessmentId == assessmentId)
				.ToListAsync();

			int totalMarks = questions.Sum(q => q.Marks);

			var result = new List<SubmissionDetailsDTO>();

			foreach (var sub in submissions)
			{
				// Get student
				var student = await _context.Student
					.FirstOrDefaultAsync(s => s.StudentId == sub.StudentID);

				int score = 0;

				foreach (var q in questions)
				{
					var answer = await _context.StudentAnswer
						.FirstOrDefaultAsync(a =>
							a.AssessmentId == sub.AssessmentId &&
							a.StudentId == sub.StudentID &&
							a.QuestionId == q.QuestionId);

					// Case-insensitive comparison
					if (answer != null &&
						!string.IsNullOrEmpty(answer.Answer) &&
						answer.Answer.Trim().ToLower() ==
						q.CorrectOption.Trim().ToLower())
					{
						score += q.Marks;
					}
				}

				double percentage = totalMarks == 0
					? 0
					: (double)score / totalMarks * 100;

				result.Add(new SubmissionDetailsDTO
				{
					SubmissionId = sub.SubmissionId,
					AssessmentId = sub.AssessmentId,
					StudentId = sub.StudentID,
					StudentName = student?.StudentName ?? "Unknown",
					Score = score,
					Percentage = percentage,
					Feedback = percentage >= 50 ? "Good" : "Needs Improvement",
					SubmissionDateTime = sub.SubmissionDate.ToLocalTime() 
				});
			}

			return result;
		}
	}
}
