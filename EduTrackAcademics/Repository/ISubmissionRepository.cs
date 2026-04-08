using EduTrackAcademics.Model;
using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Repository
{
	public interface ISubmissionRepository
	{

		Task<bool> IsStudentEnrolledAsync(string studentId);

		Task<List<ViewAssessmentDto>> GetAssessmentsByStudentIdAsync(string studentId);

		Task<ViewAssessmentDto> GetAssessmentByIdAsync(string assessmentId);
		Task<int> GetSubmissionCountAsync();

		Task<List<string>> GetStudentCourseIdsAsync(string studentId);

		Task<List<StartAssessmentDto>> GetQuestionsByAssessmentIdAsync(string assessmentId);

		Task InsertOrUpdateAnswerAsync(StudentAnswerDto dto);

		Task<string> SubmitAssessmentAsync(Submission submission);

		Task<UpdateSubmissionDto> CalculateScoreAsync(string studentId, string assessmentId);

		Task<UpdateSubmissionDto> UpdateSubmissionAsync(UpdateSubmissionDto dto);

		Task<bool> IsAssessmentSubmittedAsync(string studentId, string assessmentId);
		Task AddFeedbackAsync(SubmitFeedbackDto dto);

		Task<List<Submission>> GetSubmissionsByAssessmentIdAsync(string assessmentId);

		Task<List<Submission>> GetAllSubmissionsAsync();
	}
}
