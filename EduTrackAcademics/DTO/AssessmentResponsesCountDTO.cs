namespace EduTrackAcademics.DTO
{
	public class AssessmentResponsesCountDTO
	{
		public string AssessmentId { get; set; }
		public string BatchId { get; set; }
		public int TotalStudents { get; set; }
		public int SubmittedCount { get; set; }
		public int PendingCount { get; set; }
	}
}
