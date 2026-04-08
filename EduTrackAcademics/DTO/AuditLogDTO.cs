
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.DTO
{
	public class AuditLogDTO
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Student")]
		public string StudentId { get; set; }
		public DateTime LoginTime { get; set; }
		public DateTime LogoutTime { get; set; }
		public TimeSpan TimeSpent { get; set; }
		public DateTime Date { get; set; }
		
	}
}
