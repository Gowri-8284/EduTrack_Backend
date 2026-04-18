using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace EduTrackAcademics.Services
{
	public class InstructorAssessmentService : IInstructorAssessmentService
	{
		private readonly IInstructorAssessmentRepository _repo;

		public InstructorAssessmentService(IInstructorAssessmentRepository repo)
		{
			_repo = repo;
		}


		// ASSESSMENT

		public async Task<string> CreateAssessmentAsync(AssessmentDTO dto)
		{
			//  Get batch using courseId
			var batch = await _repo.GetBatchByCourseIdAsync(dto.CourseId);

			if (batch == null)
				throw new Exception("No batch found for this course");

			// Check if batch is active
			if (!batch.IsActive)
				throw new Exception("This course batch is inactive. Try later.");

			// Generate ID
			var id = await _repo.GenerateAssessmentIdAsync();

			// Set due date & status
			DateTime finalDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Local).AddHours(-5).AddMinutes(-30);

			var status = finalDate > DateTime.Now ? "Open" : "Closed";

			// Create assessment
			var assessment = new Assessment
			{
				AssessmentID = id,
				CourseId = dto.CourseId,
				Type = dto.Type,
				MaxMarks = dto.MaxMarks,
				DueDate = finalDate,
				Status = status
			};

			// Save
			await _repo.AddAssessmentAsync(assessment);

			return $"Assessment created with ID {id}";
		}

		public async Task<List<AssessmentResponseDTO>> GetAllAssessmentsAsync()
		{
			return await _repo.GetAllAssessmentsAsync();
		}

		public async Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date)
		{
			return await _repo.GetAssessmentsByDateAsync(date);
		}

		public async Task<object> GetAssessmentByIdAsync(string id)
		{
			var assessment = await _repo.GetAssessmentByIdAsync(id);
			if (assessment == null)
				return null;
			return new
			{
				id = assessment.AssessmentID,
				courseId = assessment.CourseId,
				type = assessment.Type,
				maxMarks = assessment.MaxMarks,
				date = assessment.DueDate.ToLocalTime(),

				status = assessment.DueDate > DateTime.Now ? "Open" : "Closed"
			};
		}

		public async Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId)
			=> await _repo.GetAssessmentsByCourseAsync(courseId);

		public async Task<string> UpdateAssessmentAsync(string id, AssessmentDTO dto)
		{
			var assessment = await _repo.GetAssessmentByIdAsync(id);
			if (assessment == null)
				throw new ApplicationException("Assessment not found");

			assessment.Type = dto.Type;
			assessment.MaxMarks = dto.MaxMarks;
			assessment.DueDate = dto.DueDate.ToLocalTime();

			await _repo.UpdateAssessmentAsync(assessment);
			return "Assessment updated successfully";
		}

		public async Task<string> DeleteAssessmentAsync(string id)
		{
			var assessment = await _repo.GetAssessmentByIdAsync(id);
			if (assessment == null)
				throw new ApplicationException("Assessment not found");

			await _repo.DeleteAssessmentAsync(assessment);
			return "Assessment deleted successfully";
		}

		// QUESTIONS

		public async Task<string> AddQuestionAsync(QuestionDTO dto)
		{
			//  VALIDATE FIRST
			await ValidateQuestionMarks(dto.AssessmentId, dto.Marks);

			var id = await _repo.GenerateQuestionIdAsync();

			var question = new Question
			{
				QuestionId = id,
				AssessmentId = dto.AssessmentId,
				QuestionType = dto.QuestionType,
				QuestionText = dto.QuestionText,
				OptionA = dto.OptionA,
				OptionB = dto.OptionB,
				OptionC = dto.OptionC,
				OptionD = dto.OptionD,
				CorrectOption = dto.CorrectOption,
				Marks = dto.Marks
			};

			await _repo.AddQuestionAsync(question);

			return $"Question added with ID {id}";
		}

		public async Task<Question> GetQuestionByIdAsync(string id)
		{
			var question = await _repo.GetQuestionByIdAsync(id);
			if (question == null)
				throw new QuestionNotFoundException(id);
			return question;
		}

		public async Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId)
			=> await _repo.GetQuestionsByAssessmentAsync(assessmentId);

		public async Task<string> UpdateQuestionAsync(string id, QuestionDTO dto)
		{
			var question = await _repo.GetQuestionByIdAsync(id);
			if (question == null)
				throw new QuestionNotFoundException(id);

			await ValidateQuestionMarksForUpdate(dto.AssessmentId, id, dto.Marks);

			question.QuestionText = dto.QuestionText;
			question.OptionA = dto.OptionA;
			question.OptionB = dto.OptionB;
			question.OptionC = dto.OptionC;
			question.OptionD = dto.OptionD;
			question.CorrectOption = dto.CorrectOption;
			question.Marks = dto.Marks;

			await _repo.UpdateQuestionAsync(question);
			return "Question updated successfully";
		}

		public async Task<string> DeleteQuestionAsync(string QuestionId)
		{
			var question = await _repo.GetQuestionByIdAsync(QuestionId);
			if (question == null)
				throw new QuestionNotFoundException(QuestionId);

			await _repo.DeleteQuestionAsync(question);
			return "Question deleted successfully";
		}

		// fetching data

		//  STATUS LOGIC
		public async Task<string> GetSubmissionStatus(string studentId, string assessmentId)
		{
			var exists = await _repo.ExistsSubmission(studentId, assessmentId);

			return exists ? "Submitted" : "Pending";
		}



		//  DASHBOARD
		public async Task<List<object>> GetStudentDashboard(string studentId)
		{
			var submissions = await _repo.GetStudentSubmissions(studentId);

			return submissions.Select(s => new
			{
				s.AssessmentId,
				s.SubmissionId,
				Status = !string.IsNullOrEmpty(s.SubmissionId) ? "Submitted" : "Pending",
				s.Score
			}).ToList<object>();
		}

		//  GET RESULT
		public async Task<SubmissionResultDTO> GetResult(string studentId, string assessmentId)
		{
			var submission = await _repo.GetSubmission(studentId, assessmentId);

			if (submission == null)
				return null;

			var totalMarks = await _repo.GetTotalMarks(assessmentId);

			double percentage = totalMarks == 0
				? 0
				: (submission.Score * 100.0) / totalMarks;

			return new SubmissionResultDTO
			{
				IsSubmitted = submission.Score != null,
				Score = submission.Score,
				Percentage = percentage
			};
		}

		// Get single submission (for resume UI)
		public async Task<Submission> GetSubmission(string studentId, string assessmentId)
		{
			return await _repo.GetSubmission(studentId, assessmentId);
		}

		public IEnumerable<AcademicYearCourseResponseDTO> GetCoursesByAcademicYear(string academicYearId)
		{
			return _repo.GetCoursesByAcademicYear(academicYearId);
		}

		private async Task ValidateQuestionMarks(string assessmentId, int newMarks)
		{
			// Get assessment
			var assessment = await _repo.GetAssessmentByIdAsync(assessmentId);

			if (assessment == null)
				throw new Exception("Assessment not found");

			// Get current total marks
			var currentTotal = await _repo.GetTotalMarksByAssessmentIdAsync(assessmentId);

			if (currentTotal + newMarks > assessment.MaxMarks)
			{
				throw new Exception(
					$"Marks limit exceeded. Current: {currentTotal}, Adding: {newMarks}, Max: {assessment.MaxMarks}"
				);
			}
		}

		private async Task ValidateQuestionMarksForUpdate(string assessmentId, string questionId, int newMarks)
		{
			// Get assessment
			var assessment = await _repo.GetAssessmentByIdAsync(assessmentId);
			if (assessment == null)
				throw new Exception("Assessment not found");

			// Get current total marks
			var currentTotal = await _repo.GetTotalMarksByAssessmentIdAsync(assessmentId);

			// Get existing question
			var existingQuestion = await _repo.GetQuestionByIdAsync(questionId);
			if (existingQuestion == null)
				throw new Exception("Question not found");

			// Adjust total (remove old marks, add new)
			var adjustedTotal = currentTotal - existingQuestion.Marks + newMarks;

			if (adjustedTotal > assessment.MaxMarks)
			{
				throw new Exception(
					$"Marks limit exceeded. Current: {currentTotal}, " +
					$"Removing: {existingQuestion.Marks}, Adding: {newMarks}, " +
					$"Max: {assessment.MaxMarks}"
				);
			}
		}


		public async Task<List<InstructorCourseBatchDTO>> GetCoursesByInstructorAsync(string instructorId)
		{
			return await _repo.GetCoursesByInstructorAsync(instructorId);
		}

		public async Task<AssessmentResponsesCountDTO> GetAssessmentStatusAsync(string assessmentId)
		{
			return await _repo.GetAssessmentStatusAsync(assessmentId);
		}

		public async Task<List<SubmissionDetailsDTO>> GetSubmissionDetailsAsync(string assessmentId)
		{
			return await _repo.GetSubmissionDetailsAsync(assessmentId);
		}
	}
}
