using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Service

{

	public interface ICoordinatorDashboardService
	{

		IEnumerable<object> GetPrograms();
		IEnumerable<object> GetCourse();
		Task<(bool Success, string Message)> AddEnrollmentAsync(EnrollmentDto dto);
		void AutoAssignPendingStudents();
		void AutoAssignStudentToBatch(string studentId, string courseId);

		IEnumerable<object> GetAcademicYears(string programId);

		object AddCourse(CourseDTO dto);

		IEnumerable<object> GetCourses(string yearId);
		IEnumerable<object> GetAllCourse();
		IEnumerable<object> GetStudents(string qualification, string program, int year);
		IEnumerable<object> GetStudentList();
		IEnumerable<object> GetInstructors(string skill);

		IEnumerable<object> GetBatches(string program, int year);

		object GetBatchCount(string program, int year);

		IEnumerable<object> GetStudentsInBatch(string batchId);

		object AssignBatches(AutoAssignBatchDTO dto);

		object AssignSingleBatch(AutoAssignBatchDTO dto);

		IEnumerable<object> GetInstructorBatches(string instructorId);

		IEnumerable<object> InstructorDashboard(string instructorId);

		object UpdateCourse(string CourseId, CourseDTO dto);

		bool DeleteCourse(string courseId);

	}

}