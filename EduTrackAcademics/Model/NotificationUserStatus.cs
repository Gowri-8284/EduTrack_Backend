using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduTrackAcademics.Model
{
	public class NotificationUserStatus
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string NotificationId { get; set; }

		[ForeignKey("NotificationId")]
		public Notification Notification { get; set; }  // navigation property

		[Required]
		public int UserId { get; set; }

		public bool IsRead { get; set; } = false;

	}
}
