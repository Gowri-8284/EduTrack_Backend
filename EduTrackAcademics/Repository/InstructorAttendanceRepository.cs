using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Repository
{
	public class InstructorAttendanceRepository : IInstructorAttendanceRepository
	{
		private readonly EduTrackAcademicsContext _context;

		public InstructorAttendanceRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}

		// ATTENDANCE

		public async Task<bool> EnrollmentExistsAsync(string enrollmentId)
		{
			return await _context.Enrollment
				.AnyAsync(e => e.EnrollmentId == enrollmentId);
		}

		public async Task<bool> BatchExistsAsync(string batchId)
		{
			return await _context.CourseBatches
				.AnyAsync(b => b.BatchId == batchId);
		}

		public async Task<bool> AttendanceExistsAsync(string enrollmentId, DateTime date)
		{
			return await _context.Attendances
				.AnyAsync(a =>
					a.EnrollmentID == enrollmentId &&
					a.SessionDate.Date == date.Date &&
					!a.IsDeleted);
		}

		public async Task<CourseBatch?> GetBatchByIdAsync(string batchId)
		{
			return await _context.CourseBatches.FirstOrDefaultAsync(b => b.BatchId == batchId);
		}

		public async Task<Attendance?> GetLastAttendanceAsync()
		{
			return await _context.Attendances
				.OrderByDescending(a => a.AttendanceID)
				.FirstOrDefaultAsync();
		}

		public async Task<string> GenerateAttendanceIdAsync()
		{
			var last = await _context.Attendances
				.OrderByDescending(a => a.AttendanceID)
				.FirstOrDefaultAsync();

			if (last == null)
				return "AT001";

			int number = int.Parse(last.AttendanceID.Substring(2));
			return $"AT{(number + 1).ToString("D3")}";
		}

		public async Task AddAttendanceAsync(Attendance attendance)
		{
			_context.Attendances.Add(attendance);
			await _context.SaveChangesAsync();
		}

		public async Task AddBatchAttendanceAsync(List<Attendance> attendances)
		{
			await _context.Attendances.AddRangeAsync(attendances);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Attendance>> GetAllAttendancesAsync()
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Student)
				.ToListAsync();
		}

		public async Task<List<string>> GetEnrollmentsByBatchAsync(string batchId)
		{
			var studentIds = await _context.StudentBatchAssignments
				.Where(x => x.BatchId == batchId)
				.Select(x => x.StudentId)
				.ToListAsync();

			var enrollmentIds = await _context.Enrollment
				.Where(e => studentIds.Contains(e.StudentId))
				.Select(e => e.EnrollmentId)
				.ToListAsync();

			return enrollmentIds;
		}

		public async Task<List<Enrollment>> GetEnrollmentsByBatchIdAsync(string batchId)
		{
			return await _context.Enrollment
					.Include(e => e.Student)
					.Where(e => _context.StudentBatchAssignments
					.Any(s => s.StudentId == e.StudentId && s.BatchId == batchId))
					.GroupBy(e => e.StudentId)
					.Select(g => g.OrderByDescending(x => x.EnrollmentDate).First())
					.ToListAsync();
		}

		public async Task<List<string>> GetAllBatchIdsAsync()
		{
			return await _context.StudentBatchAssignments
				.Select(x => x.BatchId)
				.Distinct()
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetAllAttendanceAsync()
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Student)
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Course)
				.Where(a => !a.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetAttendanceByDateAsync(DateTime date)
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Student)
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Course)
				.Where(a => a.SessionDate.Date == date.Date && !a.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetAttendanceByBatchAsync(string batchId)
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Student)
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Course)
				.Where(a => a.BatchId == batchId && !a.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetAttendanceByEnrollmentAsync(string enrollmentId)
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Student)
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Course)
				.Where(a => a.EnrollmentID == enrollmentId && !a.IsDeleted)
				.ToListAsync();
		}

		public async Task<Attendance> GetAttendanceByIdAsync(string attendanceId)
		{
			return await _context.Attendances
				.FirstOrDefaultAsync(a => a.AttendanceID == attendanceId && !a.IsDeleted);
		}

		public async Task UpdateAttendanceAsync(Attendance attendance)
		{
			_context.Attendances.Update(attendance);
			await _context.SaveChangesAsync();
		}



		public async Task UpdateAttendanceStatusAsync(Attendance attendance)
		{
			_context.Attendances.Update(attendance);
			await _context.SaveChangesAsync();
		}

		public async Task SoftDeleteAttendanceAsync(Attendance attendance)
		{
			_context.Attendances.Update(attendance);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Attendance>> GetAttendanceByBatchAndDateAsync(string batchId, DateTime date)
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
				.ThenInclude(e => e.Student)
				.Where(a => a.BatchId == batchId &&
							a.SessionDate.Date == date.Date &&
							!a.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetAttendanceByCourseAndDateAsync(string courseId, DateTime date)
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
				.Where(a => a.Enrollment.CourseId == courseId &&
							a.SessionDate.Date == date.Date &&
							!a.IsDeleted)
				.ToListAsync();
		}

		public async Task DeleteBatchAttendanceAsync(List<Attendance> records, string reason)
		{
			foreach (var record in records)
			{
				record.IsDeleted = true;
				record.DeletionDate = DateTime.Now;
				record.DeletionReason = reason;
			}

			await _context.SaveChangesAsync();
		}

		public async Task<List<Attendance>> GetDeletedAttendanceByBatchAndDateAsync(string batchId, DateTime date)
		{
			return await _context.Attendances
				.Where(a => a.BatchId == batchId &&
							a.SessionDate.Date == date.Date &&
							a.IsDeleted)
				.ToListAsync();
		}

		public async Task RestoreBatchAttendanceAsync(List<Attendance> records)
		{
			foreach (var record in records)
			{
				record.IsDeleted = false;
				record.DeletionDate = null;
			}

			await _context.SaveChangesAsync();
		}

		public async Task<List<Attendance>> GetDeletedAttendancesAsync()
		{
			return await _context.Attendances
				.Include(a => a.Enrollment)
					.ThenInclude(e => e.Student)
				.Where(a => a.IsDeleted)   
				.ToListAsync();
		}

		public IEnumerable<GetCourseResponseDTO> GetAllCoursesByInstructorId(string instructorId)
		{
			var data = (from b in _context.CourseBatches
						join c in _context.Course on b.CourseId equals c.CourseId
						join bc in _context.BatchConfigs on c.CourseId equals bc.CourseId
						where b.InstructorId == instructorId
						select new GetCourseResponseDTO
						{
							CourseId = c.CourseId,
							CourseName = c.CourseName,
							Credits = c.Credits,
							DurationInWeeks = c.DurationInWeeks,
							AcademicYearId = c.AcademicYearId,
							BatchSize = bc.BatchSize,
							CurrentStudents = b.CurrentStudents,
							MaxStudents = b.MaxStudents,
							IsActive = b.IsActive
						}).ToList();

			return data;
		}

		public CourseBatchDTO GetBatchByCourse(string? courseId, string courseName)
		{
			var course = _context.Course
				.FirstOrDefault(c =>
					(courseId != null && c.CourseId.ToLower() == courseId.ToLower()) ||
					(!string.IsNullOrEmpty(courseName) && c.CourseName.ToLower().Trim() == courseName.ToLower().Trim()));

			if (course == null) return null; 

			var batch = _context.CourseBatches
				.FirstOrDefault(b => b.CourseId == course.CourseId);

			return new CourseBatchDTO
			{
				CourseId = course.CourseId,
				CourseName = course.CourseName,
				BatchId = batch?.BatchId 
			};
		}


	}
}
