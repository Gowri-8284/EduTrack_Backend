namespace EduTrackAcademics.DTO
{
    public class StudentAssessmentStatsDTO
    {
    public int TotalAssessments { get; set; }
    public int SubmittedAssessments { get; set; }
    public int PendingAssessments { get; set; }
    public string CourseId{ get; set; }
    }
}
