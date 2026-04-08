namespace EduTrackAcademics.DTO

{

	public class EnrollmentAverageScoreDTO
	{

		public string EnrollmentId { get; set; }

		public string StudentName { get; set; }

		public string CourseName { get; set; }

		public decimal TotalScore { get; set; }

		public decimal AverageScore { get; set; }

		public decimal CompletionPercentage { get; set; }

		public int TotalAssessments { get; set; }

		public int CompletedAssessments { get; set; }

		public double AssessmentPercentage { get; set; }



	}

}