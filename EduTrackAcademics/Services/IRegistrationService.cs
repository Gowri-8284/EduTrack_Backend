using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using System.Threading.Tasks;
namespace EduTrackAcademics.Services
{
	public interface IRegistrationService
	{
			Task RegisterStudentAsync(StudentDTO dto);
			Task RegisterInstructorAsync(InstructorDTO dto);
			Task RegisterCoordinatorAsync(CoordinatorDTO dto);
		}
}

