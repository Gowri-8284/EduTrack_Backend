using System;
using System.ComponentModel.DataAnnotations;

namespace EduTrackAcademics.Model
{
	public class Notification
	{
		[Key]
		public string NotificationId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string Title { get; set; }

		[Required]
		public string Message { get; set; }

		[Required]
		public string CreatedByRole { get; set; }  // Admin, Coordinator, Instructor

		[Required]
		public string TargetRole { get; set; }     // Student, Instructor, All

		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public bool IsRead { get; set; } = false; // default unread
	}
}