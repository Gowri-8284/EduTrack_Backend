namespace EduTrackAcademics.DTO

{

	public class StudentPerformanceDTO
	{



		public string StudentId { get; set; }

		public string StudentName { get; set; }

		public string CourseName { get; set; }

		public decimal AvgScore { get; set; }

		public double CompletionPercentage { get; set; }

		public double AttendancePercentage { get; set; }

		public decimal TotalScore { get; set; }

		public string EnrollmentId { get; set; }

		public DateTime? LastUpdated { get; set; }

	}

}
