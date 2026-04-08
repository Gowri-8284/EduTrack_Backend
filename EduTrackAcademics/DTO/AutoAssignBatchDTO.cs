namespace EduTrackAcademics.DTO
{
	public class AutoAssignBatchDTO
	{
		public string CourseId { get; set; }
		public int Year { get; set; }
		public string Qualification { get; set; }
		public string Program { get; set; }
		public string InstructorId { get; set; }
		public int BatchSize { get; set; }
	}
}
