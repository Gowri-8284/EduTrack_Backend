using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Services

{

	public interface IInstructorAssessmentService
	{

		// ASSESSMENT
		Task<string> CreateAssessmentAsync(AssessmentDTO dto);

		Task<List<Assessment>> GetAllAssessmentsAsync();

		Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date);

		Task<Assessment> GetAssessmentByIdAsync(string assessmentId);

		Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId);

		Task<string> UpdateAssessmentAsync(string assessmentId, AssessmentDTO dto);

		Task<string> DeleteAssessmentAsync(string assessmentId);

		//Task<SubmissionResultDTO> AddFeedbackAsync(UpdateSubmissionDto dto);// QUESTIONS
		Task<string> AddQuestionAsync(QuestionDTO dto);

		Task<Question> GetQuestionByIdAsync(string questionId);

		Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId);

		Task<string> UpdateQuestionAsync(string questionId, QuestionDTO dto);

		Task<string> DeleteQuestionAsync(string questionId);


		// fetching data
		Task<string> GetStatus(string studentId, string assessmentId);

		Task<SubmissionResultDTO> GetResult(string studentId, string assessmentId);

		Task<Submission> GetSubmission(string studentId, string assessmentId);


		Task<List<Submission>> GetStudentSubmissions(string studentId);

	}

}