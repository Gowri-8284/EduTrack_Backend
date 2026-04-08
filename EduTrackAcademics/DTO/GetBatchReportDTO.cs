using EduTrackAcademics.DTO;

public class GetBatchReportDTO

{

	public string BatchId { get; set; }

	public string CourseName { get; set; }

	// public decimal BatchAveragePercentage { get; set; }
	public decimal BatchAverageAttendance { get; set; }

	public decimal StudentAverageAttendence { get; set; }

	public List<StudentPerformanceDTO> Students { get; set; }

	public int TotalStudents { get; set; }

	public decimal BatchAverageScore { get; set; }

	public string TopPerformer { get; set; }

	public int CompletedStudents { get; set; }

	public DateTime? LastUpdated { get; set; }

	public string InstructorId { get; set; }

	public string InstructorName { get; set; }

	public decimal BatchAverageCompletionPercentage { get; set; }

}