using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository
{
	public interface IInstructorAssessmentRepository
	{
		// ASSESSMENT
		Task<string> GenerateAssessmentIdAsync();
		Task AddAssessmentAsync(Assessment assessment);
		Task<CourseBatch?> GetBatchByCourseIdAsync(string courseId);
		Task<List<AssessmentResponseDTO>> GetAllAssessmentsAsync();
		Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date);
		Task<Assessment?> GetAssessmentByIdAsync(string assessmentId);
		Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId);
		Task UpdateAssessmentAsync(Assessment assessment);
		Task DeleteAssessmentAsync(Assessment assessment);

		// QUESTIONS
		Task<string> GenerateQuestionIdAsync();
		Task<bool> QuestionExistsAsync(string questionId);
		Task AddQuestionAsync(Question question);
		Task<Question?> GetQuestionByIdAsync(string questionId);
		Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId);
		Task UpdateQuestionAsync(Question question);
		Task DeleteQuestionAsync(Question question);

		// fetching data
		Task<Submission> GetSubmission(string studentId, string assessmentId);
		Task<bool> ExistsSubmission(string studentId, string assessmentId);
		Task AddSubmission(Submission submission);
		Task<int> GetTotalMarks(string assessmentId);
		Task<List<Submission>> GetStudentSubmissions(string studentId);
		IEnumerable<AcademicYearCourseResponseDTO> GetCoursesByAcademicYear(string academicYearId);
		Task<int> GetTotalMarksByAssessmentIdAsync(string assessmentId);
		Task<List<InstructorCourseBatchDTO>> GetCoursesByInstructorAsync(string instructorId);
		Task<AssessmentResponsesCountDTO> GetAssessmentStatusAsync(string assessmentId);
		Task<List<SubmissionDetailsDTO>> GetSubmissionDetailsAsync(string assessmentId);
	}
}
