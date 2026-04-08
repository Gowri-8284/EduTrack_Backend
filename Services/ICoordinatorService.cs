namespace EduTrackAcademics.Services
{
    public interface ICoordinatorService
    {
         List<String> GetInstructorDetails();
        string AddInstructorDetails(string InsName);

    }
}
