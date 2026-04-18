using EduTrackAcademics.Data;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Services
{
	public interface IDeadlineReminderService
	{
		Task CheckAndSendDeadlineRemindersAsync();
	}

	public class DeadlineReminderService : IDeadlineReminderService
	{
		private readonly EduTrackAcademicsContext _context;

		public DeadlineReminderService(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		public async Task CheckAndSendDeadlineRemindersAsync()
		{
			var tomorrow = DateTime.Today.AddDays(1);

			// 1. Get Assessments due tomorrow
			var upcomingAssessments = await _context.Assessments
				.Where(a => a.DueDate.Date == tomorrow)
				.ToListAsync();

			foreach (var assessment in upcomingAssessments)
			{
				// 2. Multi-table join to get the numeric UserId
				// Path: Assessment -> CourseBatches -> StudentBatchAssignments -> Students
				var userIdsToNotify = await (from courseBatch in _context.CourseBatches
											 join batchAssign in _context.StudentBatchAssignments
												  on courseBatch.BatchId equals batchAssign.BatchId
											 join student in _context.Student // Join Student table to get UserId
												  on batchAssign.StudentId equals student.StudentId
											 join enroll in _context.Enrollment
												  on student.StudentId equals enroll.StudentId
											 where courseBatch.CourseId == assessment.CourseId
												&& enroll.CourseId == assessment.CourseId
											 select student.UserId) // Now selecting the int UserId
											 .Distinct()
											 .ToListAsync();

				if (userIdsToNotify.Any())
				{
					// 3. Create the master Notification
					var notification = new Notification
					{
						NotificationId = Guid.NewGuid().ToString(),
						Title = "Deadline Reminder",
						Message = $"The assessment for {assessment.CourseId} is due tomorrow.",
						CreatedByRole = "System",
						TargetRole = "Batch",
						CreatedOn = DateTime.Now,
						IsRead = false
					};

					_context.Notification.Add(notification);

					// 4. Create status entries for each numeric UserId
					foreach (var id in userIdsToNotify)
					{
						var userStatus = new NotificationUserStatus
						{
							NotificationId = notification.NotificationId,
							UserId = id.Value, // Successfully passing the integer ID
							IsRead = false
						};
						_context.NotificationUserStatus.Add(userStatus);
					}
				}
			}

			await _context.SaveChangesAsync();
		}
	}
}