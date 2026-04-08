using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Services

{

	public interface IInstructorAttendanceService
	{


		// ATTENDANCE
		Task<string> MarkAttendanceAsync(AttendanceDTO dto);

		Task<string> MarkBatchAttendanceAsync(MarkBatchAttendanceDTO dto);

		Task<List<string>> GetEnrollmentIdsByBatchAsync(string batchId);

		Task<List<BatchDropdownDTO>> GetAllBatchesAsync();

		Task<List<object>> GetAllAttendanceAsync();

		Task<List<object>> GetAttendanceByDateAsync(DateTime date);

		Task<List<object>> GetAttendanceByBatchAsync(string batchId);

		Task<List<object>> GetAttendanceByEnrollmentAsync(string enrollmentId);

		Task<string> UpdateAttendanceAsync(string attendanceId, AttendanceDTO dto);

		Task<string> PatchAttendanceStatusAsync(string attendanceId, string EnrollmentId, string status);

		Task<string> DeleteAttendanceAsync(string attendanceId, string reason);

	}

}