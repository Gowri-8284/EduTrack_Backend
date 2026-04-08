namespace EduTrackAcademics.DTO

{

	public class CourseDTO
	{
		public string CourseName { get; set; }

		public int Credits { get; set; }

		public int DurationInWeeks { get; set; }

		public string AcademicYearId { get; set; }

		// ✅ ADD THIS
		public int BatchSize { get; set; }
	}

}
