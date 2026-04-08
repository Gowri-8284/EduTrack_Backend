using System.Threading.Tasks;
using EduTrackAcademics.DTO;
namespace EduTrackAcademics.Services
{
	public interface IStudentProfileService
	{
		Task<List<StudentDTO>> GetAllStudentsAsync();
		Task<StudentDTO> GetPersonalInfoAsync(string studentId);
		Task<StudentDTO> GetProgramDetails(string studentId);
		Task UpdateAdditionalInfo(string studentId, StudentAdditionalDetailsDTO dto);
		Task<int> GetCreditPointsAsync(string studentId);
		Task<(DateTime DueDate, string Type, string CourseName)> GetAssignmentDetailsForStudentAsync(string studentId, string courseId);

	}
}