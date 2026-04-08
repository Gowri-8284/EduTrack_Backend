using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.DTO
{
	public class StudentDashboardDTO
	{
		//add properties relevant to the student dashboard
		[ForeignKey("StudentId")]
		public string StudentId { get; set; }
		public string Name { get; set; }
		public string Qualification { get; set; }
		public StudentDTO StudentDetails { get; internal set; }
	}
}
