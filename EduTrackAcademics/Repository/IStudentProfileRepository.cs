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
		    Task<int> GetCreditPointsAsync(string studentId);
		    Task<(DateTime DueDate, string Type, string CourseName)?> GetAssignmentDetailsAsync(string courseId);
		    Task<bool> IsStudentEnrolledInCourseAsync(string studentId, string courseId);
		   

	}
}

