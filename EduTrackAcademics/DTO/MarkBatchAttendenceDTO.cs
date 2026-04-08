namespace EduTrackAcademics.DTO

{

	public class MarkBatchAttendanceDTO
	{

		public string BatchId { get; set; } = string.Empty;

		public DateTime SessionDate { get; set; }

		public string Mode { get; set; } = string.Empty;


		public List<MarkStudentAttendanceDTO> Students { get; set; } = new();

	}

}