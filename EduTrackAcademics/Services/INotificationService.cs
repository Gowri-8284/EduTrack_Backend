
using EduTrackAcademics.Model;


namespace EduTrackAcademics.Services
{

	public interface INotificationService
	{
		// For targeting a specific student (Enrollment)
		Task SendNotificationToUserAsync(int userId, string title, string message, string targetRole);

		// For targeting everyone in a role (Admin Announcements)
		Task SendNotificationToRoleAsync(string title, string message, string targetRole, string createdByRole);

		// To fetch notifications for the UI
		Task<List<Notification>> GetUserNotificationsAsync(int userId);
		void SendNotificationToUser(int userId, string title, string message, string targetRole);
	}
}

