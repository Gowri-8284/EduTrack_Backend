namespace EduTrackAcademics.DTO
{
	public class ViewAssessmentDto
	{
		public string AssessmentID { get; set; }
		public string CourseID { get; set; }
		public string Type { get; set; }
		public int MaxMarks { get; set; }
		public DateTime DueDate { get; set; }
		public string Status { get; set; }
		public string CourseName { get; internal set; }
	}
}
