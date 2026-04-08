namespace EduTrackAcademics.DTO
{
	public class UpdateSubmissionDto
	{
		public string StudentId { get; set; }
		public string AssessmentId { get; set; }
		public string submissionId { get; set; }
		public string Feedback { get; set; }
		public int Score { get; set; }
		public double Percentage { get; set; }
	}
}
