using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using System.Collections.Generic;
namespace EduTrackAcademics.Services
{


    public interface IPerformanceService

    {


        Task<int> GetPerformanceCountAsync();

        //   Task<EnrollmentAverageScoreDTO> GetAverageScoreAsync(string enrollmentId);

        Task<LastUpdatedDTO> GetLastUpdatedAsync(string enrollmentId);

        Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId);

        Task<GetBatchReportDTO> GetBatchReportAsync(string batchId);
        Task<object> GetOngoingBatches(string instrcutorId);
        Task<List<InstructorBatchDTO>> GetAllBatchesAsync();
        Task<List<BatchCompletionDTO>> GetInstructorCompletionRate(string instructorId);
        Task<List<BatchClassCountDTO>> GetBatchClassCountsByInstructor(string instructorId);
        Task<List<BatchStartDateDTO>> GetBatchStartDatesAsync();
        Task DeleteStudentAsync(string enrollmentId);

        Task UpdateStudentAsync(UpdateStudentDTO dto);
        Task GeneratePerformanceForBatch(string batchId);
        Task<double> GetCourseDropoutRateAsync(string courseId);






    }

}




