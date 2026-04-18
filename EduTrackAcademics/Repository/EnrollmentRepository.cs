using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EduTrackAcademics.Repository
{
	public class EnrollmentRepository : IEnrollmentRepository
	{
		private readonly EduTrackAcademicsContext _context;

		public EnrollmentRepository(EduTrackAcademicsContext context)
		{
			_context = context;
		}
		public async Task<int> GetEnrollmentCountAsync()
		{
			return await _context.Enrollment.CountAsync();
		}

		//Checks if duplicates are present or not
		public async Task<bool> CheckIdExistsAsync(string enrollmentId)
		{
			return await Task.FromResult(
				_context.Enrollment.Any(e => e.EnrollmentId == enrollmentId)
			);
		}

		// check whether the student enrolled for particular course or not
		public async Task<bool> IsEnrolledAsync(string studentId, string courseId)
		{
			return await Task.FromResult(
				_context.Enrollment.Any(e =>
					e.StudentId == studentId && e.CourseId == courseId)
			);
		}

		public async Task AddEnrollmentAsync(Enrollment enrollment)
		{
			_context.Enrollment.Add(enrollment);
			await _context.SaveChangesAsync();
			
		}


		//return the module along with the contents 
		public async Task<List<ModuleWithContentDto>> GetModulesByCourseAsync(string courseId)
		{

			var modules = await _context.Modules
			.Where(m => m.CourseId == courseId)
			.OrderBy(m => m.SequenceOrder)
			.Select(m => new ModuleWithContentDto
			{
				ModuleID = m.ModuleID,
				ModuleName = m.Name,
				SequenceOrder = m.SequenceOrder,

				Contents = _context.Contents
				.Where(c => c.ModuleID == m.ModuleID && c.Status == "Published")
				.Select(c => new ModuleContentDto
				{
					ContentID = c.ContentID,
					Title = c.Title,
					ContentType = c.ContentType,
					ContentURI = c.ContentURI,
					Duration = c.Duration
				})
				.ToList()
			})
			.ToListAsync();

			return modules;
		}

		public async Task<bool> CheckIfProgressExistsAsync(string studentId, string contentId)
		{
			return await Task.FromResult(
				_context.StudentProgress.Any(p =>
					p.StudentId == studentId &&
					p.ContentId == contentId)
			);
		}

		public async Task<int> GetProgressCountAsync()
		{
			return await Task.FromResult<int>(_context.StudentProgress.Count());
		}

		public async Task<int> MarkContentCompletedAsync(StudentProgress progress)
		{
			_context.StudentProgress.Add(progress);
			_context.SaveChanges();
			return await Task.FromResult(1);
		}
		public async Task<double> GetCourseProgressPercentageAsync(string studentId, string courseId)
		{
			var moduleIds = _context.Modules
				.Where(m => m.CourseId == courseId)
				.Select(m => m.ModuleID)
				.ToList();

			var contentIds = _context.Contents
				.Where(c => moduleIds.Contains(c.ModuleID))
				.Select(c => c.ContentID)
				.ToList();

			int totalContentCount = contentIds.Count();

			if (totalContentCount == 0)
				return 0;

			int completedItems = _context.StudentProgress
				.Count(p =>
					p.StudentId == studentId &&
					contentIds.Contains(p.ContentId) &&
					p.IsCompleted);

			double percentage = ((double)completedItems / (double)totalContentCount) * 100;

			return await Task.FromResult(Math.Round(percentage, 2));
		}

		public async Task<string> GetCourseStatusAsync(string studentId, string courseId)
		{
			var enrollment = await _context.Enrollment
			.Where(e => e.StudentId == studentId && e.CourseId == courseId)
			.Select(e => e.Status)
			.FirstOrDefaultAsync();

			// Default to "Active" if no status is found
			return enrollment ?? "Active";
		}

		public async Task UpdateEnrollmentStatusAsync(string studentId, string courseId, string status)
		{
			var enrollment = _context.Enrollment
				.FirstOrDefault(e =>
					e.StudentId == studentId &&
					e.CourseId == courseId);

			enrollment?.Status = status;
			_context.SaveChanges();

			await Task.CompletedTask;
		}

		public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId)
		{
			return await _context.Enrollment
				.Include(e => e.Course)
				.Where(e => e.StudentId == studentId)
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetStudentAttendanceAsync(string enrollmentId)
		{
			return await _context.Attendances
				.Where(a => a.EnrollmentID == enrollmentId && !a.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<CourseBatch>> GetBatchesByCourseAsync(string courseId)
		{
			return await _context.CourseBatches
				.Where(b => b.CourseId == courseId&b.IsActive)
				.ToListAsync();
		}

		public async Task<List<Attendance>> GetBatchAttendanceAsync(string batchId)
		{
			return await _context.Attendances
				.Where(a => a.BatchId == batchId && !a.IsDeleted)
				.ToListAsync();
		}

		public async Task<List<Course>> GetCoursesByStudentProgramAsync(string studentId)
		{
			var student = await _context.Student
				.FirstOrDefaultAsync(s => s.StudentId == studentId);

			if (student == null) return new List<Course>();

			// Join through the hierarchy to find matching courses
			return await _context.Course
				.Include(c => c.AcademicYear)
					.ThenInclude(ay => ay.Program)
						.ThenInclude(p => p.Qualification)
				.Where(c => c.AcademicYear.Program.ProgramName == student.StudentProgram
					   && c.AcademicYear.Program.Qualification.Qualificationsh == student.StudentQualification)
				.ToListAsync();
		}


		public async Task<List<Course>> GetCoursesByNameAndStudentProgramAsync(string studentId, string courseName)
		{
			var student = await _context.Student
				.FirstOrDefaultAsync(s => s.StudentId == studentId);

			if (student == null) return new List<Course>();

			return await _context.Course
				.Include(c => c.AcademicYear)
					.ThenInclude(ay => ay.Program)
						.ThenInclude(p => p.Qualification)
				.Where(c => c.AcademicYear.Program.ProgramName == student.StudentProgram
					   && c.AcademicYear.Program.Qualification.Qualificationsh == student.StudentQualification
					   && c.CourseName.Contains(courseName)) // Partial match search
				.ToListAsync();
		}

		public async Task<List<Enrollment>> GetEnrolledCoursesByStudentIdAsync(string studentId)
		{
			return await _context.Enrollment
				.Include(e => e.Course)
				.Where(e => e.StudentId == studentId)
				.ToListAsync();
		}


		public async Task<List<Enrollment>> SearchEnrolledCoursesByNameAsync(string studentId, string courseName)
		{
			return await _context.Enrollment
				.Include(e => e.Course)
				.Where(e => e.StudentId == studentId &&
							e.Course.CourseName.Contains(courseName))
				.ToListAsync();
		}

		public async Task<List<Enrollment>> GetActiveEnrollmentsWithCourseAsync(string studentId)
		{
			return await _context.Enrollment
				.Include(e => e.Course)
				.Where(e => e.StudentId == studentId && e.Status == "Active")
				.ToListAsync();
		}

		public async Task<CourseBatch> GetStudentSpecificBatchAsync(string studentId, string courseId)
		{
			// We look through StudentBatchAssignments to find the one batch 
			// where this student is assigned for this specific course.
			var assignment = await _context.StudentBatchAssignments
				.Include(sba => sba.CourseBatch)
				.FirstOrDefaultAsync(sba => sba.StudentId == studentId && sba.CourseBatch.CourseId == courseId);

			return assignment?.CourseBatch;
		}
	}
}
