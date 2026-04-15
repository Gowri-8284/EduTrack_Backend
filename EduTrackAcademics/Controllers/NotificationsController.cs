using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EduTrackAcademics.Controllers
{
	[ApiController]
	[Route("api/notifications")]
	public class NotificationController : ControllerBase
	{
		private readonly EduTrackAcademicsContext _context;

		public NotificationController(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		[Authorize(Roles = "Coordinator,Admin,Instructor")]
		[HttpPost("create")]
		public IActionResult CreateNotification([FromBody] NotificationDTO dto)
		{
			var senderRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

			// 1. We save the notification with a descriptive TargetRole (for history/logs)
			var notification = new Notification
			{
				Title = dto.Title,
				Message = dto.Message,
				CreatedByRole = senderRole,
				// Even if it's not in the User table, we store "Batch" in the Notification 
				// table so we know this was a batch-specific message later.
				TargetRole = dto.TargetRole
			};

			_context.Notification.Add(notification);
			_context.SaveChanges();

			List<int> targetUserIds = new List<int>();

			// 2. Logic to find the actual UserIds
			if (dto.TargetRole == "Student")
			{
				// Simple filter: all students
				targetUserIds = _context.Users.Where(u => u.Role == "Student").Select(u => u.UserId).ToList();
			}
			else if (dto.TargetRole == "Instructor")
			{
				if (dto.TargetId.HasValue && dto.TargetId > 0)
					targetUserIds.Add(dto.TargetId.Value); // One specific instructor
				else
					targetUserIds = _context.Users.Where(u => u.Role == "Instructor").Select(u => u.UserId).ToList();
			}
			else if (dto.TargetRole == "Batch")
			{
				targetUserIds = _context.StudentBatchAssignments
					.Where(sba => sba.BatchId == dto.BatchIdString)
					.Select(sba => sba.Student.UserId)
					// This part removes nulls and converts int? to int
					.Where(id => id.HasValue)
					.Select(id => id.Value)
					.ToList();
			}
			// Save status for each targeted user
			foreach (var userId in targetUserIds.Distinct())
			{
				_context.NotificationUserStatus.Add(new NotificationUserStatus
				{
					NotificationId = notification.NotificationId,
					UserId = userId,
					IsRead = false
				});
			}

			_context.SaveChanges();
			return Ok(new { message = "Notification Broadcasted" });
		}

		[Authorize]
		[HttpGet("my-notifications")]
		public IActionResult GetMyNotifications()
		{
			var userId = int.Parse(User.FindFirst("id")?.Value);

			var data = (from n in _context.Notification
						join s in _context.NotificationUserStatus
						on n.NotificationId equals s.NotificationId
						where s.UserId == userId
						orderby n.CreatedOn descending
						select new
						{
							n.NotificationId,
							n.Title,
							n.Message,
							n.CreatedOn,
							n.TargetRole,       // ✅ include this
							n.CreatedByRole,    // ✅ include this
							s.IsRead
						}).ToList();

			return Ok(data);
		}
		[Authorize]
		[HttpPut("{id}/mark-read")]
		public IActionResult MarkAsRead(string id)
		{
			var userId = int.Parse(User.FindFirst("id")?.Value);

			var record = _context.NotificationUserStatus
				.FirstOrDefault(x => x.NotificationId == id && x.UserId == userId);

			if (record == null) return NotFound();

			record.IsRead = true;
			_context.SaveChanges();

			return Ok();
		}

		[Authorize(Roles = "Coordinator")]
		[HttpDelete("{id}")]
		public IActionResult DeleteNotification(string id)
		{
			var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
			var notification = _context.Notification.FirstOrDefault(n => n.NotificationId == id);

			if (notification == null)
				return NotFound("Notification not found");

			if (role != "Admin" && notification.CreatedByRole != role)
				return Unauthorized("You are not allowed to delete this notification");

			_context.Notification.Remove(notification);
			_context.SaveChanges();

			return Ok(new { Message = "Notification deleted successfully" });
		}
	}
}
