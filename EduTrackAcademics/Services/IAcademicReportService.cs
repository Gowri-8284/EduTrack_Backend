using EduTrackAcademics.Model;

using EduTrackAcademics.DTO;

using System.Threading.Tasks;

namespace EduTrackAcademics.Services


{


	public interface IAcademicReportService

	{

		Task<GetBatchReportDTO> GetBatchReportAsync(string batchId);

		Task<AcademicReportDTO> GetFullAcademicReportAsync();

		Task SaveOrUpdateAcademicReportAsync(AcademicReportDTO dto);


	}


}