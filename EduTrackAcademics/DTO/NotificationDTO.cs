namespace EduTrackAcademics.DTO
{
	public class NotificationDTO
	{
		public string Title { get; set; }
		public string Message { get; set; }
		public string TargetRole { get; set; } // "Student", "Instructor", "Batch", "All"

		// Use this for the Instructor's UserId
		public int? TargetId { get; set; }

		// Add this for the Batch string ID
		public string? BatchIdString { get; set; }
	}
}
