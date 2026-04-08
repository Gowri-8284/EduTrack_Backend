namespace EduTrackAcademics.DTO
{
	public class AssignBatchDTO
	{
		public string CourseId { get; set; }
		public string InstructorId { get; set; }
		public int BatchSize { get; set; }
		public List<string> StudentIds { get; set; }
	}
}
