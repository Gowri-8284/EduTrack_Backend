using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using System.Collections.Generic;
namespace EduTrackAcademics.Services

{

	public interface IPerformanceService

	{

		Task<int> GetPerformanceCountAsync();


		Task<EnrollmentAverageScoreDTO> GetAverageScoreAsync(string enrollmentId);


		Task<LastUpdatedDTO> GetLastUpdatedAsync(string enrollmentId);


		Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId);


		Task<GetBatchReportDTO> GetBatchReportAsync(string batchId);

		Task<double> GetInstructorCompletionRate(string instrcutorId);

		Task<object> GetOngoingBatches(string instrcutorId);

		Task<List<InstructorBatchDTO>> GetAllBatchesAsync();


	}


}