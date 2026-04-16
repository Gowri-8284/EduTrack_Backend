using EduTrackAcademics.Data;
using EduTrackAcademics.Model;



using Microsoft.EntityFrameworkCore;


namespace EduTrackAcademics.Services
{

	public class NotificationService : INotificationService
	{
		private readonly EduTrackAcademicsContext _context;

		public NotificationService(EduTrackAcademicsContext context)
		{
			_context = context;
		}
		public void SendNotificationToUser(int userId, string title, string message, string targetRole)
		{
			Console.WriteLine("🔥 Notification method called");  // ADD THIS

			var notification = new Notification
			{
				NotificationId = Guid.NewGuid().ToString(),
				Title = title,
				Message = message,
				CreatedByRole = "System",
				TargetRole = targetRole,
				CreatedOn = DateTime.UtcNow
			};

			_context.Notification.Add(notification);

			var status = new NotificationUserStatus
			{
				NotificationId = notification.NotificationId,
				UserId = userId,
				IsRead = false
			};

			_context.NotificationUserStatus.Add(status);
		}

		// METHOD A: Target one specific person (Used in AddEnrollment)
		public async Task SendNotificationToUserAsync(int userId, string title, string message, string targetRole)
		{
			// 1. Create the notification message
			var notification = new Notification
			{
				NotificationId = Guid.NewGuid().ToString(),
				Title = title,
				Message = message,
				CreatedByRole = "System",
				TargetRole = targetRole,
				CreatedOn = DateTime.UtcNow
			};

			_context.Notification.Add(notification);

			// 2. Link it to the specific user (This is what makes it appear in their inbox)
			var status = new NotificationUserStatus
			{
				NotificationId = notification.NotificationId,
				UserId = userId,
				IsRead = false
			};

			_context.NotificationUserStatus.Add(status);

			// We leave this WITHOUT SaveChanges so the Coordinator Service 
			// can save it together with the Enrollment.
		}
		// METHOD B: Target a whole Role (Replaces your Controller logic)
		public async Task SendNotificationToRoleAsync(string title, string message, string targetRole, string createdByRole)
		{
			var notification = new Notification
			{
				NotificationId = Guid.NewGuid().ToString(),
				Title = title,
				Message = message,
				CreatedByRole = createdByRole,
				TargetRole = targetRole,
				CreatedOn = DateTime.UtcNow
			};

			_context.Notification.Add(notification);
			await _context.SaveChangesAsync(); // Save first to ensure the ID exists

			var users = await _context.Users
				.Where(u => targetRole == "All" || u.Role == targetRole)
				.ToListAsync();

			foreach (var user in users)
			{
				_context.NotificationUserStatus.Add(new NotificationUserStatus
				{
					NotificationId = notification.NotificationId,
					UserId = user.UserId,
					IsRead = false
				});
			}

			await _context.SaveChangesAsync();
		}

		public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
		{
			return await (from n in _context.Notification
						  join s in _context.NotificationUserStatus on n.NotificationId equals s.NotificationId
						  where s.UserId == userId
						  orderby n.CreatedOn descending
						  select n).ToListAsync();
		}
	}
}

