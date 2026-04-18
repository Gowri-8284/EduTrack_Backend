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

			var notification = new Notification
			{
				Title = dto.Title,
				Message = dto.Message,
				CreatedByRole = senderRole,
				TargetRole = dto.TargetRole
			};

			_context.Notification.Add(notification);
			_context.SaveChanges();

			List<int> targetUserIds = new List<int>();

			if (dto.TargetRole == "Student")
			{
				targetUserIds = _context.Users.Where(u => u.Role == "Student").Select(u => u.UserId).ToList();
			}
			else if (dto.TargetRole == "Instructor")
			{
				if (dto.TargetId.HasValue && dto.TargetId > 0)
					targetUserIds.Add(dto.TargetId.Value);
				else
					targetUserIds = _context.Users.Where(u => u.Role == "Instructor").Select(u => u.UserId).ToList();
			}
			else if (dto.TargetRole == "Batch")
			{
				if (!string.IsNullOrEmpty(dto.BatchIdString))
				{
					// We go from Batch -> Student (by ID) -> User (by Email)
					targetUserIds = (from sba in _context.StudentBatchAssignments
									 join std in _context.Student on sba.StudentId equals std.StudentId
									 join usr in _context.Users on std.StudentEmail equals usr.Email
									 where sba.BatchId == dto.BatchIdString
									 select usr.UserId)
									 .Distinct()
									 .ToList();
				}
			}
			foreach (var userId in targetUserIds)
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
			var userIdString = User.FindFirst("id")?.Value;
			if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
			var userId = int.Parse(userIdString);

			// Use a simpler join to ensure we aren't filtering out Batch roles
			var results = _context.NotificationUserStatus
				.Where(s => s.UserId == userId)
				.Join(_context.Notification,
					status => status.NotificationId,
					notif => notif.NotificationId,
					(status, notif) => new {
						notif.NotificationId,
						notif.Title,
						notif.Message,
						notif.CreatedOn,
						notif.TargetRole,
						notif.CreatedByRole,
						status.IsRead
					})
				.OrderByDescending(x => x.CreatedOn)
				.ToList();

			return Ok(results);
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
