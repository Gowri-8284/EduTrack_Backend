namespace EduTrackAcademics.DTO

{

	public class LastUpdatedDTO
	{
		public string StudentId { get; set; }

		public string StudentName { get; set; }

		public string CourseName { get; set; }

		public string BatchId { get; set; }

		public string InstructorId { get; set; }

		public DateTimeOffset LastUpdated { get; set; }

		public string EnrollmentId { get; set; }


	}

}
