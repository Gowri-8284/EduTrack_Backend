using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

public interface IPerformanceRepository

{


	Task<EnrollmentAverageScoreDTO> GetAverageScoreAsync(string enrollmentId);


	Task<LastUpdatedDTO> GetLastModifiedDateAsync(string enrollmentId);


	Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId);


	Task<GetBatchReportDTO> GetBatchPerformanceAsync(string batchId);


	Task<int> GetPerformanceCountAsync();


	Task AddPerformanceAsync(Performance performance);


	Task<Performance?> GetLastPerformanceAsync();

	Task<double> GetInstructorCompletionRate(string instrcutorId);

	Task<List<CourseBatch>> GetBatchesByInstructor(string instructorId);

	Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId);

	Task<List<InstructorBatchDTO>> GetAllBatchesAsync();



}