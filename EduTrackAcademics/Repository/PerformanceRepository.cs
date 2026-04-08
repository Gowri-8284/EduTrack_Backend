using EduTrackAcademics.Data;

using EduTrackAcademics.DTO;

using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace EduTrackAcademics.Repository

{

	public class PerformanceRepository : IPerformanceRepository

	{

		private readonly EduTrackAcademicsContext _context;


		public PerformanceRepository(EduTrackAcademicsContext context)

		{

			_context = context;
		}

		// ✅ COUNT

		public async Task<int> GetPerformanceCountAsync()

		{

			return await _context.Performances.CountAsync();

		}

		// ✅ ADD

		public async Task AddPerformanceAsync(Performance performance)

		{

			await _context.Performances.AddAsync(performance);

			await _context.SaveChangesAsync();

		}

		// ✅ GET LAST

		public async Task<Performance?> GetLastPerformanceAsync()

		{

			return await _context.Performances

				.OrderByDescending(p => p.LastUpdated)

				.FirstOrDefaultAsync();

		}

		// ✅ CONTENT %

		public async Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId)

		{

			var moduleIds = await _context.Modules

				.Where(m => m.CourseId == courseId)

				.Select(m => m.ModuleID)

				.ToListAsync();

			if (moduleIds.Count == 0)

				return 0;

			var contentIds = await _context.Contents

				.Where(c => moduleIds.Contains(c.ModuleID))

				.Select(c => c.ContentID)

				.ToListAsync();

			int total = contentIds.Count;

			if (total == 0)

				return 0;

			int completed = await _context.StudentProgress

				.CountAsync(p =>

					p.StudentId == studentId &&

					contentIds.Contains(p.ContentId) &&

					p.IsCompleted);

			double percentage = ((double)completed / total) * 100;

			return Math.Round(percentage, 2);

		}



		// ✅ MAIN (SAVE + UPDATE)

		public async Task<EnrollmentAverageScoreDTO> GetAverageScoreAsync(string enrollmentId)

		{

			var enrollment = await _context.Enrollment

				.Include(e => e.Student)

				.Include(e => e.Course)

				.FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
			if (enrollment == null)
			{
				throw new Exception("Enrollment not found");
			}

			if (enrollment == null)

				throw new Exception("Enrollment not found");

			var assessmentIds = await _context.Assessments

				.Where(a => a.CourseId == enrollment.CourseId)

				.Select(a => a.AssessmentID)

				.ToListAsync();

			var submissions = await _context.Submission
   .Where(s => s.StudentID == enrollment.StudentId)
   .ToListAsync();
			// ASSESSMENT COUNT

			int totalAssessments = assessmentIds.Count;

			int completedAssessments = submissions.Count;

			double assessmentPercentage = totalAssessments == 0

				? 0

				: (completedAssessments * 100.0 / totalAssessments);


			decimal totalScore = submissions.Any() ? submissions.Sum(s => (decimal)s.Score) : 0;

			decimal avgScore = submissions.Any() ? submissions.Average(s => (decimal)s.Score) : 0;

			double contentPercentage = await GetCourseProgressPercentageAsync(

				enrollment.StudentId,

				enrollment.CourseId);

			decimal finalPercentage = Math.Round((decimal)contentPercentage, 2);

			var existing = await _context.Performances

				.FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId);

			if (existing != null)

			{

				existing.AvgScore = avgScore;

				existing.CompletionPercentage = finalPercentage;

				existing.LastUpdated = DateTime.UtcNow;

				_context.Performances.Update(existing);

			}

			else

			{

				var batch = await _context.CourseBatches

					.FirstOrDefaultAsync(cb => cb.CourseId == enrollment.CourseId);

				await _context.Performances.AddAsync(new Performance

				{

					ProgressID = Guid.NewGuid().ToString(),

					EnrollmentId = enrollmentId,

					StudentId = enrollment.StudentId,

					AvgScore = avgScore,

					CompletionPercentage = finalPercentage,

					LastUpdated = DateTime.UtcNow,

					BatchId = batch?.BatchId,

					InstructorId = batch?.InstructorId

				});

			}

			await _context.SaveChangesAsync();

			return new EnrollmentAverageScoreDTO

			{

				EnrollmentId = enrollmentId,

				StudentName = enrollment.Student?.StudentName ?? "N/A",

				CourseName = enrollment.Course?.CourseName ?? "N/A",

				TotalScore = totalScore,

				AverageScore = avgScore,
				CompletionPercentage = finalPercentage,
				TotalAssessments = totalAssessments,
				CompletedAssessments = completedAssessments,
				AssessmentPercentage = assessmentPercentage

			};

		}

		// ✅ LAST UPDATED (IST)

		public async Task<LastUpdatedDTO> GetLastModifiedDateAsync(string enrollmentId)

		{

			var data = await _context.Performances

				.Include(p => p.Student)

				.Include(p => p.courseBatch)

					.ThenInclude(cb => cb.Course)

				.Include(p => p.courseBatch)

					.ThenInclude(cb => cb.Instructor)

				.Where(p => p.EnrollmentId == enrollmentId)

				.OrderByDescending(p => p.LastUpdated)

				.FirstOrDefaultAsync();

			if (data == null)

			{

				return new LastUpdatedDTO

				{

					EnrollmentId = enrollmentId,

					StudentName = "No Data Found"

				};

			}

			// ✅ IST conversion

			var ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

			var time = TimeZoneInfo.ConvertTimeFromUtc(data.LastUpdated, ist);

			return new LastUpdatedDTO

			{

				EnrollmentId = enrollmentId,

				StudentId = data.StudentId,

				StudentName = data.Student?.StudentName,

				CourseName = data.courseBatch?.Course?.CourseName,

				BatchId = data.BatchId,

				InstructorId = data.courseBatch?.InstructorId,

				LastUpdated = time

			};

		}


		// ✅ INSTRUCTOR BATCHES

		public async Task<List<InstructorBatchDTO>> GetInstructorBatchesAsync(string instructorId)

		{

			return await _context.CourseBatches

				.Include(cb => cb.Course)

				.Include(cb => cb.Instructor) // 🔥 IMPORTANT

				.Where(cb => cb.InstructorId == instructorId)
.Select(cb => new InstructorBatchDTO

{

	BatchId = cb.BatchId,

	CourseName = cb.Course.CourseName,

	StudentCount = _context.StudentBatchAssignments

		.Count(s => s.BatchId == cb.BatchId),

	InstructorId = cb.Instructor.InstructorId,

	InstructorName = cb.Instructor.InstructorName,

	InstructorEmail = cb.Instructor.InstructorEmail,

	InstructorPhone = cb.Instructor.InstructorPhone,

	// 🔥 NEW FIELDS

	IsActive = cb.IsActive,

	StartDate = cb.LastFilledDate,

	EndDate = cb.LastFilledDate != null

		? cb.LastFilledDate.Value.AddDays(cb.Course.DurationInWeeks * 7)

		: (DateTime?)null

})


				.ToListAsync();

		}


		//// ✅ BATCH REPORT (FULL 🔥)



		public async Task<GetBatchReportDTO> GetBatchPerformanceAsync(string batchId)

		{

			// 🔹 Get Batch

			var batch = await _context.CourseBatches

				.Include(cb => cb.Course)

				.Include(cb => cb.Instructor)

				.FirstOrDefaultAsync(cb => cb.BatchId == batchId);

			if (batch == null)

				return null;

			// 🔹 Get Enrollments (by CourseId)

			var enrollments = await _context.Enrollment

				.Include(e => e.Student)

				.Where(e => e.CourseId == batch.CourseId)

				.ToListAsync();

			// 🔹 Remove duplicates

			var uniqueEnrollments = enrollments

				.GroupBy(e => e.EnrollmentId)

				.Select(g => g.First())

				.ToList();

			var students = new List<StudentPerformanceDTO>();

			foreach (var e in uniqueEnrollments)

			{

				// 🔹 Submissions

				var submissions = await _context.Submission

					.Where(s => s.StudentID == e.StudentId)

					.ToListAsync();

				var totalScore = submissions.Sum(s => s.Score);

				var lastUpdated = submissions

					.OrderByDescending(s => s.SubmissionDate)

					.Select(s => s.SubmissionDate)

					.FirstOrDefault();

				// 🔥 IMPORTANT FIX → completion calculation
				var completion = await GetCourseProgressPercentageAsync(e.StudentId, batch.CourseId);





				// 🔹 Attendance

				var totalClasses = await _context.Attendances

					.Where(a => a.BatchId == batchId && !a.IsDeleted)

					.Select(a => a.SessionDate.Date)

					.Distinct()

					.CountAsync();

				var attended = await _context.Attendances

					.Where(a => a.BatchId == batchId &&

								a.EnrollmentID == e.EnrollmentId &&

								a.Status == "Present" &&

								!a.IsDeleted)

					.CountAsync();

				double attendancePercentage = totalClasses == 0

					? 0

					: (double)attended / totalClasses * 100;

				students.Add(new StudentPerformanceDTO

				{

					StudentId = e.StudentId,

					StudentName = e.Student.StudentName,

					CourseName = batch.Course.CourseName,

					AvgScore = submissions.Count == 0

						? 0

						: (decimal)submissions.Average(s => s.Score),

					CompletionPercentage = completion, // ✅ FIXED

					AttendancePercentage = attendancePercentage,

					TotalScore = totalScore,

					EnrollmentId = e.EnrollmentId,

					LastUpdated = lastUpdated

				});

			}

			// 🔹 Batch Calculations

			decimal batchAvgScore = students.Count == 0

				? 0

				: students.Average(s => s.AvgScore);

			double batchAvgAttendance = students.Count == 0

				? 0

				: students.Average(s => s.AttendancePercentage);

			double batchAvgCompletion = students.Count == 0

				? 0

				: students.Average(s => s.CompletionPercentage);

			var topPerformer = students

				.OrderByDescending(s => s.AvgScore)

				.FirstOrDefault();

			int completedStudents = students.Count(s => s.CompletionPercentage == 100);

			var batchLastUpdated = students

				.Where(s => s.LastUpdated != null)

				.OrderByDescending(s => s.LastUpdated)

				.Select(s => s.LastUpdated)

				.FirstOrDefault();

			// 🔹 Final DTO

			return new GetBatchReportDTO

			{

				BatchId = batchId,

				CourseName = batch.Course.CourseName,

				InstructorId = batch.InstructorId,

				InstructorName = batch.Instructor.InstructorName,

				Students = students,

				TotalStudents = students.Count,

				BatchAverageScore = Math.Round(batchAvgScore, 2),

				BatchAverageAttendance = Math.Round((decimal)batchAvgAttendance, 2),

				BatchAverageCompletionPercentage = Math.Round((decimal)batchAvgCompletion, 2),

				TopPerformer = topPerformer?.StudentName,

				CompletedStudents = completedStudents,

				LastUpdated = batchLastUpdated

			};

		}


		public async Task<double> GetInstructorCompletionRate(string instructorId)

		{

			var batches = await _context.CourseBatches

				.Where(cb => cb.InstructorId == instructorId)

				.ToListAsync();

			double totalPercentage = 0;

			int studentCount = 0;

			foreach (var batch in batches)

			{

				var students = await _context.StudentBatchAssignments

					.Where(s => s.BatchId == batch.BatchId)

					.Select(s => s.StudentId)

					.ToListAsync();

				foreach (var studentId in students)

				{

					double progress = await GetCourseProgressPercentageAsync(studentId, batch.CourseId);

					totalPercentage += progress;

					studentCount++;

				}

			}

			if (studentCount == 0) return 0;

			return Math.Round(totalPercentage / studentCount, 2);

		}
		public async Task<List<CourseBatch>> GetBatchesByInstructor(string instructorId)
		{
			return await _context.CourseBatches
				.Where(cb => cb.InstructorId == instructorId)
				.ToListAsync();

		}
		public async Task<List<InstructorBatchDTO>> GetAllBatchesAsync()

		{

			return await _context.CourseBatches

				.Include(cb => cb.Course)

				.Include(cb => cb.Instructor)

				.Select(cb => new InstructorBatchDTO

				{

					BatchId = cb.BatchId,

					CourseName = cb.Course.CourseName,

					StudentCount = _context.StudentBatchAssignments

						.Count(s => s.BatchId == cb.BatchId),

					InstructorId = cb.Instructor.InstructorId,

					InstructorName = cb.Instructor.InstructorName,

					InstructorEmail = cb.Instructor.InstructorEmail,

					InstructorPhone = cb.Instructor.InstructorPhone,

					IsActive = cb.IsActive,

					StartDate = cb.LastFilledDate,

					EndDate = cb.LastFilledDate != null

						? cb.LastFilledDate.Value.AddDays(cb.Course.DurationInWeeks * 7)

						: (DateTime?)null

				})

				.ToListAsync();

		}

	}
}