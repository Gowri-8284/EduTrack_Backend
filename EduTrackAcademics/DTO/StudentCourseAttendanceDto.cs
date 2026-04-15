namespace EduTrackAcademics.DTO
{
	public class StudentCourseAttendanceDto
	{
		public string CourseId { get; set; }
		public string CourseName { get; set; }
		public double AttendancePercentage { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Status { get; set; }
	}
}
