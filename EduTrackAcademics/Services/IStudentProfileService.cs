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
		Task<StudentAdditionalDetailsDTO> GetAdditionalInfoAsync(string studentId);
		Task<int> GetCreditPointsAsync(string studentId);
		Task<IEnumerable<(DateTime DueDate, string CourseName, string Status)>> GetAssignmentsForStudentAsync(string studentId);

	}
}