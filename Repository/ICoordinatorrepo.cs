namespace EduTrackAcademics.Repository
{
    public interface ICoordinatorrepo
    {
        List<String> GetInstructorData();
        string AddInstructorData(string n);
    }
}
