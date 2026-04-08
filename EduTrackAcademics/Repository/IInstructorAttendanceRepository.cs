using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository

{

	public interface IInstructorAttendanceRepository
	{

		// ATTENDANCE
		Task<bool> EnrollmentExistsAsync(string enrollmentId);

		Task<bool> BatchExistsAsync(string batchId);

		Task<bool> AttendanceExistsAsync(string enrollmentId, DateTime date);

		Task<string> GenerateAttendanceIdAsync();

		Task AddAttendanceAsync(Attendance attendance);

		Task AddBatchAttendanceAsync(List<Attendance> attendances);

		Task<List<string>> GetEnrollmentsByBatchAsync(string batchId);

		Task<List<string>> GetAllBatchIdsAsync();

		Task<Attendance?> GetLastAttendanceAsync();




		Task<List<Attendance>> GetAllAttendanceAsync();

		Task<List<Attendance>> GetAttendanceByDateAsync(DateTime date);

		Task<List<Attendance>> GetAttendanceByBatchAsync(string batchId);

		Task<List<Attendance>> GetAttendanceByEnrollmentAsync(string enrollmentId);

		Task<Attendance> GetAttendanceByIdAsync(string attendanceId);

		Task UpdateAttendanceAsync(Attendance attendance);

		Task UpdateAttendanceStatusAsync(Attendance attendance);

		Task SoftDeleteAttendanceAsync(Attendance attendance);

	}

}