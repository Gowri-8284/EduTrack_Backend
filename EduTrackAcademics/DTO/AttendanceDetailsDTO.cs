namespace EduTrackAcademics.DTO
{
	public class AttendanceDetailsDTO
	{
		public string AttendanceId { get; set; }
		public string StudentId { get; set; } = string.Empty;
		public string StudentName { get; set; } = string.Empty;
		public string EnrollmentID { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string BatchId { get; set; } = string.Empty;
	}
}
