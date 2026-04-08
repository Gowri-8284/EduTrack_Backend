using System.Diagnostics;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Repository;
using EduTrackAcademics.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace EduTrackAcademics.Service
{
	public class CoordinatorDashboardService : ICoordinatorDashboardService
	{
		private readonly ICoordinatorDashboardRepo _repo;
		private readonly EduTrackAcademicsContext _context;

		public CoordinatorDashboardService(ICoordinatorDashboardRepo repo,EduTrackAcademicsContext context)
		{
			_repo = repo;
			_context = context;
		}
		public async Task<(bool Success, string Message)> AddEnrollmentAsync(EnrollmentDto dto)
		{
			using var transaction = _context.Database.BeginTransaction();

			try
			{
				// ✅ 1. Validate course
				var course = await _context.Course.FindAsync(dto.CourseId);
				if (course == null)
					return (false, "Course not found");


				// ✅ 2. Prevent duplicate enrollment
				bool alreadyExists = await _context.Enrollment.AnyAsync(e =>
					e.StudentId == dto.StudentId &&
					e.CourseId == dto.CourseId &&
					e.Status == "Active");

				if (alreadyExists)
					return (false, "Student already enrolled");


				// ✅ 3. Create Enrollment
				int count = await _context.Enrollment.CountAsync();
				string enrollmentId = $"E{(count + 1):D3}";

				var enrollment = new Enrollment
				{
					EnrollmentId = enrollmentId,
					StudentId = dto.StudentId,
					CourseId = dto.CourseId,
					EnrollmentDate = DateTime.Now,
					Status = "Active",
					Credits = course.Credits
				};

				_context.Enrollment.Add(enrollment);
				await _context.SaveChangesAsync();

				// 🔥 4. AUTO ASSIGN (SAFE VERSION)
				AutoAssignStudentToBatch(dto.StudentId, dto.CourseId);

				transaction.Commit();

				return (true, "Enrollment + Auto Assignment successful");

			}
			catch (Exception ex)
			{
			

				await transaction.RollbackAsync();
				return (false, ex.InnerException?.Message ?? ex.Message);

			}
		}

		public void AutoAssignPendingStudents()
		{
			// 1️⃣ Free up instructors whose batches are completed
			var activeBatches = _context.CourseBatches
				.Where(b => b.IsActive)
				.ToList();

			foreach (var batch in activeBatches)
			{
				var course = _context.Course.FirstOrDefault(c => c.CourseId == batch.CourseId);

				if (course == null) continue;

				// Calculate dynamic end date based on when batch reached max students
				if (batch.CurrentStudents == batch.MaxStudents && batch.LastFilledDate == null)
				{
					batch.LastFilledDate = DateTime.Now;
				}

				var batchStartDate = batch.LastFilledDate ?? DateTime.Now;

				var batchEndDate = batchStartDate.AddDays(course.DurationInWeeks * 7);

				if (DateTime.Now >= batchEndDate)
				{
					batch.IsActive = false;
				}
			}
			_context.SaveChanges();

			// 2️⃣ Get students in queue (Active but not assigned)
			var pendingEnrollments = _context.Enrollment
		.Where(e => e.Status == "Active" &&
					!_context.StudentBatchAssignments
						.Join(_context.CourseBatches,
							  sba => sba.BatchId,
							  cb => cb.BatchId,
							  (sba, cb) => new { sba.StudentId, cb.CourseId })
						.Any(x => x.StudentId == e.StudentId && x.CourseId == e.CourseId))
		.ToList();

			// 2️⃣ Assign them
			foreach (var enrollment in pendingEnrollments)
			{
				AutoAssignStudentToBatch(enrollment.StudentId, enrollment.CourseId);
			}
		}

		public void AutoAssignStudentToBatch(string studentId, string courseId)
		{
			int batchSize = _repo.GetBatchSize(courseId);

			// 1️⃣ Try to find a batch with free slots
			var batch = _context.CourseBatches
				.FirstOrDefault(b => b.CourseId == courseId &&
									 b.CurrentStudents < b.MaxStudents 
									 );

			// 2️⃣ If batch exists, assign
			if (batch != null)
			{
				bool alreadyAssigned = _context.StudentBatchAssignments
					.Any(a => a.StudentId == studentId && a.BatchId == batch.BatchId);

				if (!alreadyAssigned)
				{
					_context.StudentBatchAssignments.Add(new StudentBatchAssignment
					{
						StudentId = studentId,
						BatchId = batch.BatchId
					});

					batch.CurrentStudents++;

					// 🔥 MAIN FIX
					if (batch.CurrentStudents == batch.MaxStudents)
					{
						batch.LastFilledDate = DateTime.Now;
						batch.IsActive = true;
						Console.WriteLine("🔥 FILLED BATCH HIT 🔥");
						Debug.WriteLine("🔥 FILLED BATCH HIT 🔥");

						Console.WriteLine($"Batch {batch.BatchId} filled at {batch.LastFilledDate}");

						_context.Entry(batch).Property(b => b.LastFilledDate).IsModified = true;
						_context.Entry(batch).Property(b => b.IsActive).IsModified = true;
					}



					var enrollment = _context.Enrollment
						.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

					if (enrollment != null)
						enrollment.Status = "Assigned";

					_context.SaveChanges();
				}
				return;
			}

			// 3️⃣ If no batch exists, create a new batch if instructor is free
			var instructor = _context.Instructor
	.FirstOrDefault(i => !_context.CourseBatches
		.Any(b => b.InstructorId == i.InstructorId && b.IsActive));

			if (instructor == null) return; // no free instructor
			string batchId = $"B{(_context.CourseBatches.Count() + 1):D3}";


			// Create new batch
			var newBatch = new CourseBatch
			{
				BatchId = batchId,  // ✅ FIX
				CourseId = courseId,
				InstructorId = instructor.InstructorId,
				CurrentStudents = 1,
				MaxStudents = batchSize,
				IsActive = false,
				LastFilledDate = null
			};

		

			_context.CourseBatches.Add(newBatch);
			_context.StudentBatchAssignments.Add(new StudentBatchAssignment
			{
				StudentId = studentId,
				BatchId = newBatch.BatchId
			});

			var enrollment2 = _context.Enrollment
				.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);
			if (enrollment2 != null) enrollment2.Status = "Assigned";

			_context.SaveChanges();
		}

		// ================== PROGRAMS ==================

		//[Authorize(Roles = "Coordinator")]
		public IEnumerable<object> GetPrograms()
		{
			var programs = _repo.GetPrograms();

			if (programs == null || !programs.Any())
				throw new CourseNotFoundException("Programs not found");

			return programs;
		}
		public IEnumerable<object> GetCourse()
		{
			var course = _repo.GetCourse();

			if (course == null || !course.Any())
				throw new CourseNotFoundException("Programs not found");

			return course;
		}

		// ================== ACADEMIC YEARS ==================

		//[Authorize(Roles = "Coordinator")]
		public IEnumerable<object> GetAcademicYears(string programId)
		{
			if (string.IsNullOrWhiteSpace(programId))
				throw new InvalidDataException("Program ID is required");

			var years = _repo.GetAcademicYears(programId);

			if (years == null || !years.Any())
				throw new CourseNotFoundException($"Academic years not found for program: {programId}");

			return years;
		}

		// ================== COURSE ==================

		public object AddCourse(CourseDTO dto)
		{
			if (dto == null)
				throw new InvalidDataException("Course data is required");

			return _repo.AddCourse(dto);
		}
		public IEnumerable<object> GetAllCourse()

		{

			var course = _repo.GetAllCourse();


			if (course == null || !course.Any())

				throw new CourseNotFoundException("Courses not found");


			return course;

		}

		public IEnumerable<object> GetCourses(string yearId)
		{
			if (string.IsNullOrWhiteSpace(yearId))
				throw new InvalidDataException("Year ID is required");

			var courses = _repo.GetCourses(yearId);

			if (courses == null || !courses.Any())
				throw new CourseNotFoundException($"Courses not found for year: {yearId}");

			return courses;
		}

		public object UpdateCourse(string courseId, CourseDTO dto)
		{
			if (string.IsNullOrWhiteSpace(courseId))
				throw new InvalidDataException("Course ID is required");

			if (dto == null)
				throw new InvalidDataException("Course data is required");

			return _repo.UpdateCourse(courseId, dto);
		}

		public bool DeleteCourse(string courseId)
		{
			if (string.IsNullOrWhiteSpace(courseId))
				throw new InvalidDataException("Course ID is required");

			var result = _repo.DeleteCourse(courseId);

			if (!result)
				throw new CourseNotFoundException($"Course not found: {courseId}");

			return result;
		}

		// ================== STUDENTS ==================

		public IEnumerable<object> GetStudents(string qualification, string program, int year)
		{
			if (string.IsNullOrWhiteSpace(qualification) ||
				string.IsNullOrWhiteSpace(program))
				throw new InvalidDataException("Qualification and Program are required");

			var students = _repo.GetStudents(qualification, program, year);

			if (students == null || !students.Any())
				throw new StudentNotFoundException($"{qualification}-{program}-{year}");

			return students;
		}
		public IEnumerable<object> GetStudentList()
		{
		

			var students = _repo.GetStudentList();

			//if (students == null || !students.Any())
			//	throw new StudentNotFoundException($"students are not there");

			return students;
		}


		public IEnumerable<object> GetStudentsInBatch(string batchId)
		{
			if (string.IsNullOrWhiteSpace(batchId))
				throw new InvalidDataException("Batch ID is required");

			var students = _repo.GetStudentsInBatch(batchId);

			if (students == null || !students.Any())
				throw new BatchNotFoundException(batchId);

			return students;
		}

		// ================== INSTRUCTORS ==================

		public IEnumerable<object> GetInstructors(string skill)
		{
			if (string.IsNullOrWhiteSpace(skill))
				throw new InvalidDataException("Skill is required");

			var instructors = _repo.GetInstructors(skill);

			if (instructors == null || !instructors.Any())
				throw new InstructorNotFoundException(skill);

			return instructors;
		}

		public IEnumerable<object> GetInstructorBatches(string instructorId)
		{
			if (string.IsNullOrWhiteSpace(instructorId))
				throw new InvalidDataException("Instructor ID is required");

			var batches = _repo.GetInstructorBatches(instructorId);

			if (batches == null || !batches.Any())
				throw new InstructorBatchesNotFoundException(instructorId);

			return batches;
		}

		public IEnumerable<object> InstructorDashboard(string instructorId)
		{
			if (string.IsNullOrWhiteSpace(instructorId))
				throw new InvalidDataException("Instructor ID is required");

			var dashboard = _repo.InstructorDashboard(instructorId);

			if (dashboard == null || !dashboard.Any())
				throw new InstructorNotFoundException(instructorId);

			return dashboard;
		}

		// ================== BATCH ==================

		public IEnumerable<object> GetBatches(string program, int year)
		{
			if (string.IsNullOrWhiteSpace(program))
				throw new InvalidDataException("Program is required");

			var batches = _repo.GetBatches(program, year);

			if (batches == null || !batches.Any())
				throw new BatchNotFoundException($"{program}-{year}");

			return batches;
		}

		public object GetBatchCount(string program, int year)
		{
			if (string.IsNullOrWhiteSpace(program))
				throw new InvalidDataException("Program is required");

			return _repo.GetBatchCount(program, year);
		}

		public object AssignBatches(AutoAssignBatchDTO dto)
		{
			if (dto == null)
				throw new InvalidDataException("Batch assignment data is required");

			return _repo.AssignBatches(dto);
		}

		public object AssignSingleBatch(AutoAssignBatchDTO dto)
		{
			if (dto == null)
				throw new InvalidDataException("Batch assignment data is required");

			return _repo.AssignSingleBatch(dto);
		}
	}
}