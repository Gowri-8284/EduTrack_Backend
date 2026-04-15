using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository
{
	public interface IInstructorAttendanceRepository
	{
		// ATTENDANCE
		Task<bool> EnrollmentExistsAsync(string enrollmentId);
		Task<bool> BatchExistsAsync(string batchId);
		Task<bool> AttendanceExistsAsync(string enrollmentId, DateTime date);

		Task<CourseBatch?> GetBatchByIdAsync(string batchId);
		Task<string> GenerateAttendanceIdAsync();
		Task AddAttendanceAsync(Attendance attendance);
		Task AddBatchAttendanceAsync(List<Attendance> attendances);
		Task<List<Attendance>> GetAllAttendanceAsync();
		Task<List<Attendance>> GetAttendanceByBatchAndDateAsync(string batchId, DateTime date);
		Task<List<string>> GetEnrollmentsByBatchAsync(string batchId);

		Task<List<Enrollment>> GetEnrollmentsByBatchIdAsync(string batchId);
		Task<List<string>> GetAllBatchIdsAsync();
		Task<Attendance?> GetLastAttendanceAsync();


		Task<List<Attendance>> GetAllAttendancesAsync();
		Task<List<Attendance>> GetAttendanceByDateAsync(DateTime date);
		Task<List<Attendance>> GetAttendanceByBatchAsync(string batchId);
		Task<List<Attendance>> GetAttendanceByEnrollmentAsync(string enrollmentId);
		Task<Attendance> GetAttendanceByIdAsync(string attendanceId);
		Task UpdateAttendanceAsync(Attendance attendance);
		Task UpdateAttendanceStatusAsync(Attendance attendance);
		Task SoftDeleteAttendanceAsync(Attendance attendance);

		Task<List<Attendance>> GetDeletedAttendanceByBatchAndDateAsync(string batchId, DateTime date);
		Task RestoreBatchAttendanceAsync(List<Attendance> records);
		Task<List<Attendance>> GetAttendanceByCourseAndDateAsync(string courseId, DateTime date);
		Task DeleteBatchAttendanceAsync(List<Attendance> records, string reason);
		Task<List<Attendance>> GetDeletedAttendancesAsync();
		IEnumerable<GetCourseResponseDTO> GetAllCoursesByInstructorId(string instructorId);
		CourseBatchDTO GetBatchByCourse(string? courseId, string courseName);
	}
}
