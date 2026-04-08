using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository

{

	public interface IInstructorAssessmentRepository
	{

		// ASSESSMENT
		Task<string> GenerateAssessmentIdAsync();

		Task AddAssessmentAsync(Assessment assessment);

		Task<List<Assessment>> GetAllAssessmentsAsync();

		Task<List<Assessment>> GetAssessmentsByDateAsync(DateTime date);

		Task<Assessment?> GetAssessmentByIdAsync(string assessmentId);

		Task<List<Assessment>> GetAssessmentsByCourseAsync(string courseId);

		Task UpdateAssessmentAsync(Assessment assessment);

		Task DeleteAssessmentAsync(Assessment assessment);


		//Task<Submission> GetSubmissionAsync(string studentId, string assessmentId);//Task<int> GetTotalMarksAsync(string assessmentId);//Task UpdateSubmissionAsync(Submission submission);// QUESTIONS
		Task<string> GenerateQuestionIdAsync();

		Task<bool> QuestionExistsAsync(string questionId);

		Task AddQuestionAsync(Question question);

		Task<Question?> GetQuestionByIdAsync(string questionId);

		Task<List<Question>> GetQuestionsByAssessmentAsync(string assessmentId);

		Task UpdateQuestionAsync(Question question);

		Task DeleteQuestionAsync(Question question);


		// fetching data
		Task<Submission> GetSubmission(string studentId, string assessmentId);

		Task<int> GetTotalMarks(string assessmentId);

		Task<List<Submission>> GetStudentSubmissions(string studentId);

	}

}