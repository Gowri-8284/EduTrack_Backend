using EduTrackAcademics.Model;

namespace EduTrackAcademics.Repository
{
	public interface IRegistrationRepository
	{
		Task RegisterStudentAsync(Student student, Users user);
		Task RegisterInstructorAsync(Instructor instructor, Users user);
		Task RegisterCoordinatorAsync(Coordinator coordinator, Users user);
	}
}
