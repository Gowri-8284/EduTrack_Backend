using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Services
{
	public interface IInstructorAssessmentService
	{
		// ASSESSMENT
		Task<string> CreateAssessmentAsync(AssessmentDTO dto);
		Task<List<AssessmentResponseDTO>> GetAllAssessmentsAsync();
		Task<object> GetAssessmentByIdAsync(string id);
		Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date);
		Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId);
		Task<string> UpdateAssessmentAsync(string assessmentId, AssessmentDTO dto);
		Task<string> DeleteAssessmentAsync(string assessmentId);

		// QUESTIONS
		Task<string> AddQuestionAsync(QuestionDTO dto);
		Task<Question> GetQuestionByIdAsync(string questionId);
		Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId);
		Task<string> UpdateQuestionAsync(string questionId, QuestionDTO dto);
		Task<string> DeleteQuestionAsync(string questionId);

		// fetching data
		Task<string> GetSubmissionStatus(string studentId, string assessmentId);
		Task<List<object>> GetStudentDashboard(string studentId);
		Task<SubmissionResultDTO> GetResult(string studentId, string assessmentId);
		Task<Submission> GetSubmission(string studentId, string assessmentId);

		IEnumerable<AcademicYearCourseResponseDTO> GetCoursesByAcademicYear(string academicYearId);
		Task<List<InstructorCourseBatchDTO>> GetCoursesByInstructorAsync(string instructorId);

		Task<AssessmentResponsesCountDTO> GetAssessmentStatusAsync(string assessmentId);
		Task<List<SubmissionDetailsDTO>> GetSubmissionDetailsAsync(string assessmentId);
	}
}
