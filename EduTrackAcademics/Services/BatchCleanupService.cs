using EduTrackAcademics.Data;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Services
{
	public class BatchCleanupService
	{
		private readonly EduTrackAcademicsContext _context;

		public BatchCleanupService(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		public void ProcessExpiredBatches()
		{
			// 1. Get batches that have actually started (LastFilledDate is not null)
			var activeBatches = _context.CourseBatches
				.Where(b => b.LastFilledDate != null)
				.ToList();

			foreach (var batch in activeBatches)
			{
				var course = _context.Course.FirstOrDefault(c => c.CourseId == batch.CourseId);
				if (course == null) continue;

				// --- Calculation Logic ---
				DateTime startDate = batch.LastFilledDate.Value;

				// Calculate Duration in Days
				int durationDays = course.DurationInWeeks * 7;

				// Calculate the Last Day
				DateTime lastDay = startDate.AddDays(durationDays);

				// 2. Check if the current date has crossed the Last Day
				if (DateTime.Now.Date > lastDay.Date)
				{
					// Remove related assignments first to avoid Foreign Key errors
					var assignments = _context.StudentBatchAssignments.Where(a => a.BatchId == batch.BatchId);
					_context.StudentBatchAssignments.RemoveRange(assignments);

					// Delete the batch
					_context.CourseBatches.Remove(batch);
				}
			}

			_context.SaveChanges();

		}
	}
}
	
