using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Services
{
	public interface ISubmissionService
	{
		Task<List<ViewAssessmentDto>> GetStudentAssessmentsAsync(string studentId);
		Task<List<StartAssessmentDto>> StartAssessmentAsync(string studentId, string assessmentId);

		Task InsertOrUpdateAnswerAsync(StudentAnswerDto dto);

		Task<string> SubmitAssessmentAsync(SubmitAssessmentDto dto);

		//Task<(int score, double percentage)> CalculateScoreAsync(SubmitAssessmentDto dto);

		Task AddFeedbackAsync(SubmitFeedbackDto dto);
		Task<UpdateSubmissionDto> UpdateSubmissionAsync(string studendId, string assesmentId);

		Task<List<Submission>> GetAllSubmissionsByAssessmentAsync(string assessmentId);

		Task<List<Submission>> GetAllSubmissionsAsync();
	}
}
