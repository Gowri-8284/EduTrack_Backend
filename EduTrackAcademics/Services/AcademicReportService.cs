using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Repository;
using System;
using System.Threading.Tasks;
namespace EduTrackAcademics.Services

{

    public class AcademicReportService : IAcademicReportService

    {

        private readonly IAcademicReportRepository _repo;

        public AcademicReportService(IAcademicReportRepository repo)

        {

            _repo = repo;

        }


        // 🔥 SINGLE BATCH

        public async Task<GetBatchReportDTO> GetBatchReportAsync(string batchId)

        {

            if (string.IsNullOrEmpty(batchId))

                throw new InvalidInputException("BatchId is required");

            var result = await _repo.GetBatchReport(batchId);

            if (result == null)

                throw new DataNotFoundException($"No data found for BatchId: {batchId}");

            return result;

        }

        // 🔥 FULL REPORT

        public async Task<AcademicReportDTO> GetFullAcademicReportAsync()

        {

            var result = await _repo.GetFullAcademicReport();

            if (result == null || result.Batches == null || !result.Batches.Any())

                throw new DataNotFoundException("No batches found in the system");

            return result;

        }

        // 💾 SAVE / UPDATE

        public async Task SaveOrUpdateAcademicReportAsync(AcademicReportDTO dto)

        {

            if (dto == null || dto.Batches == null || !dto.Batches.Any())

                throw new InvalidInputException("Invalid report data");

            await _repo.SaveOrUpdateAcademicReport(dto);

        }

    }

}


