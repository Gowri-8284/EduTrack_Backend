namespace EduTrackAcademics.DTO
{
	public class SubmissionDetailsDTO
	{
		public string SubmissionId { get; set; }
		public string AssessmentId { get; set; }
		public string StudentId { get; set; }
		public string StudentName { get; set; }
		public int Score { get; set; }
		public double Percentage { get; set; }
		public string Feedback { get; set; }
		public DateTime SubmissionDateTime { get; set; }
	}
}
