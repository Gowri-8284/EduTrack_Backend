namespace EduTrackAcademics.DTO

{

	public class InstructorBatchDTO
	{
		public string BatchId { get; set; }


		public string InstructorId { get; set; }


		public string CourseName { get; set; }

		public int StudentCount { get; set; }

		public string InstructorName { get; set; }

		public string InstructorEmail { get; set; }

		public long InstructorPhone { get; set; }

		public bool IsActive { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }


	}



}