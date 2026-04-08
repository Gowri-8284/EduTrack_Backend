using System;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Services
{
	public interface IEnrollmentService
	{
		Task<string> AddEnrollmentAsync(EnrollmentDto dto);
		Task<List<ModuleWithContentDto>> GetContentForStudentAsync(string studentId, string courseId);
		//Task<int> MarkContentCompletedAsync(string studentId, string courseId, string contentId);
		Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId);
		Task<string> GetCourseStatusAsync(string studentId, string courseId);

		Task<ProgressResponseDto> MarkAsCompletedAndSyncStatusAsync(string studentId, string courseId, string contentId);
		Task<List<BatchAttendanceDto>> GetBatchWiseAttendanceAsync(string courseId);
		Task<List<StudentCourseAttendanceDto>> CalculateStudentAttendanceByStudentIdAsync(string studentId);
		//Task<List<Enrollment>> GetAllEnrollmentAsync();

		Task<List<EnrollCourseDto>> GetAvailableCoursesForStudentAsync(string studentId);
		Task<List<EnrollCourseDto>> SearchCoursesForStudentAsync(string studentId, string courseName);
		Task<List<EnrollCourseDto>> GetStudentEnrolledCoursesAsync(string studentId);
		Task<List<EnrollCourseDto>> SearchStudentEnrolledCoursesAsync(string studentId, string courseName);
		Task CheckAndUpdateDropoutStatusAsync(string studentId);
	}
}
