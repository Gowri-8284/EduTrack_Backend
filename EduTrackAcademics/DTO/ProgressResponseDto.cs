namespace EduTrackAcademics.DTO
{
	public class ProgressResponseDto
	{
		// The updated percentage (e.g., 85.5)
		public double ProgressPercentage { get; set; }

		// The current status from the Enrollment table ("Active" or "Completed")
		public string Status { get; set; }

		// A custom message to display in the frontend alert
		public string Message { get; set; }
	}
}