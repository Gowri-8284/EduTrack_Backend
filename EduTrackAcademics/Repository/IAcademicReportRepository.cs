using EduTrackAcademics.Model;
using EduTrackAcademics.DTO;

using System.Threading.Tasks;
namespace EduTrackAcademics.Repository

{

	public interface IAcademicReportRepository
	{

		Task SaveOrUpdateAcademicReport(AcademicReportDTO dto);


		Task<GetBatchReportDTO> GetBatchReport(string batchId);

		Task<AcademicReportDTO> GetFullAcademicReport();

	}


}