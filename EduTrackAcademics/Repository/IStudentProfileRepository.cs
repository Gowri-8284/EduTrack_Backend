using EduTrackAcademics.DTO;

namespace EduTrackAcademics.Repository
{
	public interface IStudentProfileRepository
	{
			Task<StudentDTO?> GetPersonalInfoAsync(string studentId);
		    Task<List<StudentDTO>> GetAllStudentsAsync();
			Task<StudentDTO?> GetProgramDetailsAsync(string studentId);
			Task<bool> StudentExistsAsync(string studentId);
			Task UpdateAdditionalInfoAsync(string studentId, StudentAdditionalDetailsDTO dto);
		    Task<StudentAdditionalDetailsDTO?> GetAdditionalInfoAsync(string studentId);
		Task<int> GetCreditPointsAsync(string studentId);
			Task<IEnumerable<(DateTime DueDate, string CourseName, string Status)>> GetStudentAssignmentsAsync(string studentId);

			Task<bool> IsStudentEnrolledInCourseAsync(string studentId, string courseId);
		   

	}
}

