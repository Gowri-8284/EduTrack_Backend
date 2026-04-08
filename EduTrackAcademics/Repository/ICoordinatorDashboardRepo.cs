using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository

{

	public interface ICoordinatorDashboardRepo
	{

		IEnumerable<object> GetPrograms();
		IEnumerable<object> GetCourse();
		IEnumerable<object> GetAcademicYears(string programId);

		Course AddCourse(CourseDTO dto);

		IEnumerable<object> GetCourses(string yearId);
		IEnumerable<object> GetAllCourse();
		IEnumerable<object> GetStudents(string qualification, string program, int year);
		IEnumerable<object> GetStudentList();
		IEnumerable<object> GetInstructors(string skill);

		IEnumerable<object> GetBatches(string program, int year);

		int GetBatchCount(string program, int year);

		IEnumerable<object> GetStudentsInBatch(string batchId);

		object AssignBatches(AutoAssignBatchDTO dto);

		object AssignSingleBatch(AutoAssignBatchDTO dto);

		IEnumerable<object> GetInstructorBatches(string instructorId);

		IEnumerable<object> InstructorDashboard(string instructorId);

		object UpdateCourse(string CourseId, CourseDTO dto);
		int GetBatchSize(string courseId);
		bool DeleteCourse(string courseId);

	}

}