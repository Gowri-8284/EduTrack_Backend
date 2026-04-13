using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace EduTrackAcademics.Services
{
	public class EnrollmentService : IEnrollmentService
	{
		private readonly IEnrollmentRepository _repo;
		private readonly EduTrackAcademicsContext _context;

		public EnrollmentService(IEnrollmentRepository repo, EduTrackAcademicsContext context)
		{
			_repo = repo;
			_context = context;
		}

		public async Task<string> AddEnrollmentAsync(EnrollmentDto dto)
		{
			// Validate course (course already mapped to year)
			var course = await _context.Course.FindAsync(dto.CourseId);
			if (course == null)
				throw new ApplicationException("Course not found");

			// 1. Check if the boolean is actually TRUE
			bool alreadyEnrolled = await _context.Enrollment.AnyAsync(e =>
				e.StudentId == dto.StudentId &&
				e.CourseId == dto.CourseId );

			if (alreadyEnrolled) // Fixed logic
				throw new EnrollmentAlreadyExistsException("Student already enrolled for this course");

			// 2. Caution: This ID generation method is risky under high load
			int count = await _repo.GetEnrollmentCountAsync();
			var Enrollment_Id = $"E{(count + 1):D3}";

			var enrollment = new Enrollment
			{
				EnrollmentId = Enrollment_Id,
				StudentId = dto.StudentId,
				CourseId = dto.CourseId,
				EnrollmentDate = DateTime.Now,
				Status = "Active",
				Credits = course.Credits
			};

			await _repo.AddEnrollmentAsync(enrollment);
			return Enrollment_Id;
		}

		public async Task<List<ModuleWithContentDto>> GetContentForStudentAsync(string studentId, string courseId)
		{
			if (!await _repo.IsEnrolledAsync(studentId, courseId))
				throw new EnrollmentNotExistsException("You must enroll first", 403);

			return await _repo.GetModulesByCourseAsync(courseId);
		}


		public async Task<ProgressResponseDto> MarkAsCompletedAndSyncStatusAsync(string studentId, string courseId, string contentId)
		{
			// 1. Save the progress record
			if (await _repo.CheckIfProgressExistsAsync(studentId, contentId))
				throw new ApplicationException("This content is already marked as completed.");

			var progress = new StudentProgress
			{
				ProgressID = Guid.NewGuid().ToString(),
				StudentId = studentId,
				CourseId = courseId,
				ContentId = contentId,
				IsCompleted = true,
				CompletionDate = DateTime.Now
			};
			await _repo.MarkContentCompletedAsync(progress);

			// 2. Calculate the NEW percentage immediately
			double currentProgress = await _repo.GetCourseProgressPercentageAsync(studentId, courseId);

			// 3. If 100%, update the Enrollment Status to "Completed"
			string enrollmentStatus = "Active";
			if (currentProgress >= 100)
			{
				await _repo.UpdateEnrollmentStatusAsync(studentId, courseId, "Completed");
				enrollmentStatus = "Completed";
			}

			// Return everything the frontend needs to update the UI without another GET call
			return new ProgressResponseDto
			{
				ProgressPercentage = currentProgress,
				Status = enrollmentStatus,
				Message = currentProgress >= 100 ? "Course Completed!" : "Progress Saved"
			};
		}

		public async Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId)
		{
			return await _repo.GetCourseProgressPercentageAsync(studentId, courseId);
		}

		public async Task<string> GetCourseStatusAsync(string studentId, string courseId)
		{
			return await _repo.GetCourseStatusAsync(studentId, courseId);
		}

		public async Task<List<StudentCourseAttendanceDto>> CalculateStudentAttendanceByStudentIdAsync(string studentId)
		{
			var enrollments = await _repo.GetEnrollmentsByStudentIdAsync(studentId);
			if (enrollments == null || !enrollments.Any()) return new List<StudentCourseAttendanceDto>();

			var result = new List<StudentCourseAttendanceDto>();

			foreach (var enrollment in enrollments)
			{
				// CHANGE: Get the specific batch the student is assigned to
				var studentBatch = await _repo.GetStudentSpecificBatchAsync(studentId, enrollment.CourseId);

				// If the student isn't assigned to a batch yet or it has no start date, skip
				if (studentBatch == null || !studentBatch.LastFilledDate.HasValue)
					continue;

				// 3. Define the Window based on the specific batch's LastFilledDate
				DateTime startDate = studentBatch.LastFilledDate.Value;
				DateTime endDate = startDate.AddDays(enrollment.Course.DurationInWeeks * 7);

				// 4. Get attendance records (linked via EnrollmentID)
				var attendanceRecords = await _repo.GetStudentAttendanceAsync(enrollment.EnrollmentId);

				int totalSessions = 0;
				int totalPresent = 0;

				if (attendanceRecords != null && attendanceRecords.Any())
				{
					// Filter attendance within the course timeframe
					var validSessions = attendanceRecords
						.Where(a => a.SessionDate.Date >= startDate.Date && a.SessionDate.Date <= endDate.Date)
						.ToList();

					totalSessions = validSessions.Select(a => a.SessionDate.Date).Distinct().Count();
					totalPresent = validSessions.Count(a => a.Status == "Present");
				}

				// 5. Calculate Percentage
				double percentage = totalSessions > 0 ? Math.Round(((double)totalPresent / totalSessions) * 100, 2) : 0;

				// 6. Dropped Logic: 75% rule after at least 4 sessions
				if (totalSessions >= 4 && percentage < 75 && DateTime.Now >= endDate.Date)
				{
					await _repo.UpdateEnrollmentStatusAsync(studentId, enrollment.CourseId, "Dropped");
					enrollment.Status = "Dropped";
				}

				result.Add(new StudentCourseAttendanceDto
				{
					CourseId = enrollment.CourseId,
					CourseName = enrollment.Course.CourseName,
					AttendancePercentage = percentage,
					StartDate = startDate,
					EndDate = endDate,
					Status = enrollment.Status
				});
			}

			return result;
		}

		public async Task<List<BatchAttendanceDto>> GetBatchWiseAttendanceAsync(string courseId)
		{
			// Get all active batches for the course
			var batches = await _repo.GetBatchesByCourseAsync(courseId);

			if (batches == null || batches.Count == 0)
				return new List<BatchAttendanceDto>();

			var batchAttendanceList = new List<BatchAttendanceDto>();

			// Loop through each batch
			foreach (var batch in batches)
			{
				var attendanceRecords = await _repo.GetBatchAttendanceAsync(batch.BatchId);

				double percentage = 0;

				if (attendanceRecords != null && attendanceRecords.Count > 0)
				{
					int totalRecords = attendanceRecords.Count;
					int totalPresent = attendanceRecords.Count(a => a.Status == "Present");

					percentage = ((double)totalPresent / totalRecords) * 100;
					percentage = Math.Round(percentage, 2);
				}

				batchAttendanceList.Add(new BatchAttendanceDto
				{
					BatchId = batch.BatchId,
					CourseId = batch.CourseId,
					AttendancePercentage = percentage
				});
			}

			return batchAttendanceList;
		}


		public async Task<List<EnrollCourseDto>> GetAvailableCoursesForStudentAsync(string studentId)
		{
			var courses = await _repo.GetCoursesByStudentProgramAsync(studentId);

			return courses.Select(c => new EnrollCourseDto
			{
				CourseId = c.CourseId,
				CourseName = c.CourseName,
				Credits = c.Credits,
				DurationInWeeks = c.DurationInWeeks
			}).ToList();
		}

		public async Task<List<EnrollCourseDto>> SearchCoursesForStudentAsync(string studentId, string courseName)
		{
			var courses = await _repo.GetCoursesByNameAndStudentProgramAsync(studentId, courseName);

			return courses.Select(c => new EnrollCourseDto
			{
				CourseId = c.CourseId,
				CourseName = c.CourseName,
				Credits = c.Credits,
				DurationInWeeks = c.DurationInWeeks,
			}).ToList();
		}


		public async Task<List<EnrollCourseDto>> GetStudentEnrolledCoursesAsync(string studentId)
		{
			var enrollments = await _repo.GetEnrolledCoursesByStudentIdAsync(studentId);

			return enrollments.Select(e => new EnrollCourseDto
			{
				CourseId=e.CourseId,
				CourseName = e.Course.CourseName,
				Credits = e.Course.Credits,
				DurationInWeeks = e.Course.DurationInWeeks
				// You could also add e.EnrollmentDate if you update the Dto
			}).ToList();
		}

		public async Task<List<EnrollCourseDto>> SearchStudentEnrolledCoursesAsync(string studentId, string courseName)
		{
			var enrollments = await _repo.SearchEnrolledCoursesByNameAsync(studentId, courseName);

			return enrollments.Select(e => new EnrollCourseDto
			{
				CourseId = e.CourseId,
				CourseName = e.Course.CourseName,
				Credits = e.Course.Credits,
				DurationInWeeks = e.Course.DurationInWeeks
			}).ToList();
		}

		public async Task CheckAndUpdateDropoutStatusAsync(string studentId)
		{
			var enrollments = await _repo.GetActiveEnrollmentsWithCourseAsync(studentId);
			bool changed = false;

			foreach (var enrollment in enrollments)
			{
				// 1. Find the specific batch assignment for this student/course combination
				var assignment = await _context.StudentBatchAssignments
					.Include(sba => sba.CourseBatch)
					.FirstOrDefaultAsync(sba => sba.StudentId == studentId &&
											   sba.CourseBatch.CourseId == enrollment.CourseId);

				// 2. Extract batch details
				var batch = assignment?.CourseBatch;

				// 3. Logic: Only calculate deadline if batch exists, is active, and has a start date
				if (batch != null && batch.IsActive && batch.LastFilledDate.HasValue)
				{
					var deadline = batch.LastFilledDate.Value.AddDays(enrollment.Course.DurationInWeeks * 7);

					// 4. Check if current time has surpassed the batch-calculated deadline
					if (DateTime.Now > deadline)
					{
						double progress = await GetCourseProgressPercentageAsync(studentId, enrollment.CourseId);

						if (progress < 100)
						{
							enrollment.Status = "Dropped";
							changed = true;
						}
					}
				}
				else
				{
					// ELSE BLOCK:
					// If batch is null, LastFilledDate is null, or IsActive is false:
					// The student's "clock" hasn't started yet. 
					// We do nothing (status remains "Active").
				}
			}

			if (changed)
			{
				await _context.SaveChangesAsync();
			}
		}
	}
}
