namespace EduTrackAcademics.DTO
{
	public class AssessmentResponseDTO
	{
		public string AssessmentId { get; set; } = string.Empty;
		public string CourseId { get; set; } = string.Empty;
		public string CourseName { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
		public int MaxMarks { get; set; }
		public DateTime DueDate { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
