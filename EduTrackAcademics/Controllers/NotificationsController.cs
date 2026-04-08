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
			if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Message) || string.IsNullOrEmpty(dto.TargetRole))
				return BadRequest("Title, Message, and TargetRole are required.");

			var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

			var notification = new Notification
			{
				Title = dto.Title,
				Message = dto.Message,
				CreatedByRole = role,
				TargetRole = dto.TargetRole
			};

			_context.Notification.Add(notification);
			_context.SaveChanges();

			return Ok(new
			{
				Message = "Notification created successfully",
				notification.NotificationId
			});
		}

		
		[Authorize]
		[HttpGet("my-notifications")]
		public IActionResult GetMyNotifications()
		{
			var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

			var notifications = _context.Notification
				.Where(n => n.TargetRole == role || n.TargetRole == "All")
				.OrderByDescending(n => n.CreatedOn)
				.ToList();

			return Ok(notifications);
		}


		[Authorize]
		[HttpPut("{id}/mark-read")]
		public IActionResult MarkAsRead(string id)
		{
			var notification = _context.Notification.FirstOrDefault(n => n.NotificationId == id);
			if (notification == null)
				return NotFound("Notification not found");

			notification.IsRead = true;
			_context.SaveChanges();

			return Ok(new { Message = "Notification marked as read" });
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
