namespace EduTrackAcademics.DTO
{
	public class GetCourseResponseDTO
	{
		public string CourseId { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public int DurationInWeeks { get; set; }
		public string AcademicYearId { get; set; }

		public int BatchSize { get; set; }
		public int CurrentStudents { get; set; }
		public int MaxStudents { get; set; }
		public bool IsActive { get; set; }
	}
}
