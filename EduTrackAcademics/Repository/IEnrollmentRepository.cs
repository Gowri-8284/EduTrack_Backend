using EduTrackAcademics.Model;
using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Repository
{
	public interface IEnrollmentRepository
	{
		Task AddEnrollmentAsync(Enrollment enrollment);
		Task<bool> CheckIdExistsAsync(string enrollmentId);
		Task<int> GetEnrollmentCountAsync();
		Task<bool> IsEnrolledAsync(string studentId, string courseId);
		Task<List<ModuleWithContentDto>> GetModulesByCourseAsync(string courseId);
		Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId);
		Task<string> GetCourseStatusAsync(string studentId, string courseId);
		Task UpdateEnrollmentStatusAsync(string studentId, string courseId, string status);
		Task<List<CourseBatch>> GetBatchesByCourseAsync(string courseId);
		Task<List<Attendance>> GetBatchAttendanceAsync(string batchId);
		Task<int> MarkContentCompletedAsync(StudentProgress progress);
		Task<bool> CheckIfProgressExistsAsync(string studentId, string contentId);
		Task<int> GetProgressCountAsync();
		Task<List<Attendance>> GetStudentAttendanceAsync(string enrollmentId);
		Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId);
		//Task<List<Enrollment>> GetAllEnrollmentAsync();

		Task<List<Course>> GetCoursesByStudentProgramAsync(string studentId);
		Task<List<Course>> GetCoursesByNameAndStudentProgramAsync(string studentId, string courseName);
		Task<List<Enrollment>> GetEnrolledCoursesByStudentIdAsync(string studentId);
		Task<List<Enrollment>> SearchEnrolledCoursesByNameAsync(string studentId, string courseName);
		Task<List<Enrollment>> GetActiveEnrollmentsWithCourseAsync(string studentId);
		Task<CourseBatch> GetStudentSpecificBatchAsync(string studentId, string courseId);


	}
}
