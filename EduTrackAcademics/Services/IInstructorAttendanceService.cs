using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Services
{
	public interface IInstructorAttendanceService
	{

		// ATTENDANCE
		Task<string> MarkAttendanceAsync(AttendanceDTO dto);
		Task<string> MarkBatchAttendanceAsync(MarkBatchAttendanceDTO dto);
		Task<List<AttendanceSummaryDTO>> GetAttendanceSummaryAsync();
		Task<List<AttendanceDetailsDTO>> GetAttendanceDetailsAsync(string batchId, DateTime date);
		Task<List<string>> GetEnrollmentIdsByBatchAsync(string batchId);
		Task<List<BatchDropdownDTO>> GetAllBatchesAsync();
		Task<List<BatchStudentsDTO>> GetStudentsForAttendanceAsync(string batchId);
		Task<List<object>> GetAllAttendanceAsync();
		Task<List<object>> GetAttendanceByDateAsync(DateTime date);
		Task<List<object>> GetAttendanceByBatchAsync(string batchId);
		Task<List<object>> GetAttendanceByEnrollmentAsync(string enrollmentId);
		Task<string> UpdateAttendanceAsync(string attendanceId, AttendanceDTO dto);
		Task<string> PatchAttendanceStatusAsync(string attendanceId, string status);
		Task<string> DeleteAttendanceAsync(string attendanceId, string reason);

		Task<string> DeleteAttendanceByBatchAsync(string batchId, DateTime date, string reason);

		Task<string> DeleteAttendanceByCourseAsync(string courseId, DateTime date, string reason);
		Task<string> RestoreAttendanceByBatchAsync(string batchId, DateTime date);
		Task<List<AttendanceSummaryDTO>> GetDeletedAttendanceSummaryAsync();
		IEnumerable<GetCourseResponseDTO> GetAllCoursesByInstructorId(string instructorId);
		CourseBatchDTO GetBatchByCourse(string? courseId, string courseName);
	}
}
