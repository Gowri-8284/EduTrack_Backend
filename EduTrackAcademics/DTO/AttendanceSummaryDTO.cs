namespace EduTrackAcademics.DTO
{
	public class AttendanceSummaryDTO
	{
		public string BatchId { get; set; } = string.Empty;
		public DateTime SessionDate { get; set; }
		public string CourseId { get; set; } = string.Empty;
		public string CourseName { get; set; } = string.Empty;

		public int TotalStudents { get; set; }
		public int PresentCount { get; set; }
		public int AbsentCount { get; set; }
	}
}
