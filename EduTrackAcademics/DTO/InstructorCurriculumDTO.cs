namespace EduTrackAcademics.DTO
{
	public class InstructorCurriculumDTO
	{
		public string CourseId { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public int DurationInWeeks { get; set; }
		public string AcademicYearId { get; set; }

		public string BatchId { get; set; }
		public int BatchSize { get; set; }
		public int CurrentStudents { get; set; }

		public int TotalModules { get; set; }
		public int TotalAssessments { get; set; }
	}
}
