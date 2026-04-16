using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using EduTrackAcademics.Service;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EduTrackAcademics.Controllers
{
	[ApiController]
	[Route("api/coordinator")]
	[AllowAnonymous]

	[EnableCors("MyCorsPolicy")]
	public class CoordinatorDashboardController : ControllerBase
	{
		private readonly ICoordinatorDashboardService _service;
		private readonly EduTrackAcademicsContext _context;
		private readonly INotificationService _notificationService;


		public CoordinatorDashboardController(EduTrackAcademicsContext context, ICoordinatorDashboardService service, INotificationService notificationService)
		{
			_service = service;
			_context = context;
			_notificationService = notificationService;
		}


		[Authorize(Roles = "Coordinator,Admin")]
		[HttpGet("programs")]
		public IActionResult GetPrograms()
		{
			return Ok(_service.GetPrograms());
		}

		[HttpGet("COURSES")]
		public IActionResult GetCourse()
		{
			return Ok(_service.GetCourse());
		}



	    [Authorize(Roles = "Coordinator,Admin")]		

		[HttpGet("program/{programId}/years")]
		public IActionResult GetAcademicYears(string programId)
		{
			return Ok(_service.GetAcademicYears(programId));


		}

		//[Authorize(Roles = "Coordinator")]		

		[HttpPost("course")]

		public IActionResult AddCourse([FromBody] CourseDTO dto)

		{

			return Ok(_service.AddCourse(dto));


		}

		
		[Authorize(Roles = "Coordinator")]
		[HttpPut("course/{id}")]
		public IActionResult UpdateCourse(string id, [FromBody] CourseDTO dto)
		{
			var updatedCourse = _service.UpdateCourse(id, dto);
			return Ok(updatedCourse);
		}

		[Authorize(Roles = "Coordinator")]
		[HttpDelete("course/{id}")]
		public IActionResult DeleteCourse(string id)
		{
			_service.DeleteCourse(id);
			return NoContent();
		}

		[Authorize(Roles = "Coordinator")]
		[HttpGet("academic-year/{yearId}/courses")]
		public IActionResult GetCourses(string yearId)
		{
			return Ok(_service.GetCourses(yearId));
		}

		[Authorize(Roles = "Coordinator,Admin")]
		[HttpGet("students")]
		public IActionResult GetStudents(string qualification, string program, int year)
		{
			return Ok(_service.GetStudents(qualification, program, year));
		}
		[HttpGet("details")]
		public IActionResult GetAllStudents()
		{
			var studentDetails = (from s in _context.Student
								  join e in _context.Enrollment
									  on s.StudentId equals e.StudentId into se
								  from e in se.DefaultIfEmpty()
								  join b in _context.CourseBatches
									  on e.CourseId equals b.CourseId into eb
								  from b in eb.DefaultIfEmpty()
								  select new
								  {
									  StudentId = s.StudentId,
									  StudentName = s.StudentName,
									  StudentEmail = s.StudentEmail,
									  CourseId = e.CourseId ?? "N/A",
									  BatchName = b.BatchId ?? "Unassigned"
									  // Grade omitted for now
								  }).ToList();

			return Ok(studentDetails);
		}

		// GET: api/coordinator/students/details/{courseId}
		[HttpGet("details/{courseId}")]
		public IActionResult GetStudentsByCourse(string courseId)
		{
			var studentDetails = (from s in _context.Student
								  join e in _context.Enrollment
									  on s.StudentId equals e.StudentId
								  join b in _context.CourseBatches
									  on e.CourseId equals b.CourseId into eb
								  from b in eb.DefaultIfEmpty()
								  where e.CourseId == courseId
								  select new
								  {
									  StudentId = s.StudentId,
									  StudentName = s.StudentName,
									  StudentEmail = s.StudentEmail,
									  CourseId = e.CourseId,
									  BatchName = b.BatchId ?? "Unassigned"
									  // Grade omitted for now
								  }).ToList();

			return Ok(studentDetails);
		}
		[HttpGet("students/all")]
		public IActionResult GetStudentList()
		{
			return Ok(_service.GetStudentList());
		}

		[Authorize(Roles = "Coordinator,Admin")]
		[HttpGet("instructors")]
		public IActionResult GetInstructors(string skill)
		{
			// Start with all instructors
			var query = _context.Instructor.AsQueryable();

			// If skill is provided, filter
			if (!string.IsNullOrEmpty(skill))
			{
				query = query.Where(i => i.InstructorSkills.Contains(skill));
			}

			// Build instructor DTOs using joins
			var instructors = query
				.Select(i => new
				{
					i.InstructorId,
					InstructorName = i.InstructorName,
					Expertise = i.InstructorSkills, // skill column
					Courses = _context.CourseBatches
						.Where(cb => cb.InstructorId == i.InstructorId)
						.Select(cb => cb.Course.CourseName)
						.Distinct()
						.ToList(),
					Batches = _context.CourseBatches
						.Where(cb => cb.InstructorId == i.InstructorId)
						.Select(cb => cb.BatchId)
						.ToList()
					// BatchEndDate omitted for now
				})
				.ToList();

			return Ok(instructors);
		}
		// Always return all instructors
		[HttpGet("instructors/all")]
		public IActionResult GetAllInstructors()
		{
			var instructors = _context.Instructor
				.Select(i => new
				{
					i.InstructorId,
					InstructorName = i.InstructorName,
					Expertise = i.InstructorSkills, // skill column
					Courses = _context.CourseBatches
						.Where(cb => cb.InstructorId == i.InstructorId)
						.Select(cb => cb.Course.CourseName)
						.Distinct()
						.ToList(),
					Batches = _context.CourseBatches
						.Where(cb => cb.InstructorId == i.InstructorId)
						.Select(cb => cb.BatchId)
						.ToList()
					// BatchEndDate omitted for now
				})
				.ToList();

			return Ok(instructors);
		}

		//[Authorize(Roles = "Coordinator,Admin")]        
		[HttpGet("batches")]

		public IActionResult GetBatches(string program, int year)
		{
			return Ok(_service.GetBatches(program, year));
		}


		//[HttpGet("batch-count")]//public IActionResult GetBatchCount(string program, int year)//{//  return Ok(_service.GetBatchCount(program, year));//}//[Authorize(Roles = "Coordinator,Admin")]   
		[HttpGet("batch/{batchId}/students")]
		public IActionResult GetStudentsInBatch(string batchId)
		{
			return Ok(_service.GetStudentsInBatch(batchId));
		}


		//[HttpPost("assign-batches")]//public IActionResult AssignBatches([FromBody] AutoAssignBatchDTO dto)//{//  return Ok(_service.AssignBatches(dto));//}       
		[HttpGet("eligible-students")]

		public IActionResult GetEligibleStudents(string courseId, string qualification, string program, int year)
		{
			var students = FetchEligibleStudents(courseId, qualification, program, year);

			if (!students.Any())
				return Ok(new { Message = "No eligible students found" });

			return Ok(students);
		}

		private List<object> FetchEligibleStudents(string courseId, string qualification, string program, int year)
		{
			return _context.Enrollment
			.Include(e => e.Student)
			.Where(e =>
			e.CourseId == courseId &&
			e.Status == "Active" &&
			e.Student.StudentQualification == qualification &&
			e.Student.StudentProgram == program &&
			e.Student.Year == year &&
			!_context.StudentBatchAssignments.Any(a => a.StudentId == e.StudentId)
			)
			.Select(e => new
			{
				e.StudentId,
				e.Student.StudentName
			})
			.ToList<object>();
		}



		[HttpPost("add-enrollment")]
		public async Task<IActionResult> AddEnrollment([FromBody] EnrollmentDto dto)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				// ✅ 1. Validate course
				var course = await _context.Course.FindAsync(dto.CourseId);
				if (course == null)
					return BadRequest("Course not found");

				// ✅ 2. Prevent duplicate enrollment
				bool alreadyExists = await _context.Enrollment.AnyAsync(e =>
					e.StudentId == dto.StudentId &&
					e.CourseId == dto.CourseId &&
					e.Status == "Active");

				if (alreadyExists)
					return BadRequest("Student already enrolled");

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

				var student = await _context.Student
					.FirstOrDefaultAsync(s => s.StudentId == dto.StudentId);

				if (student != null && student.UserId.HasValue)
				{
					await _notificationService.SendNotificationToUserAsync(
						student.UserId.Value,
						"Enrollment Successful",
						$"You have been successfully enrolled in {course.CourseName}.",
						"Student"
					);
				}
				else
				{
					Console.WriteLine("❌ Notification skipped: Student/UserId missing");
				}

				// 🔥 4. AUTO ASSIGN (SAFE VERSION)
				await AutoAssignStudentToBatch(dto.StudentId, dto.CourseId);

				var assignedBatch = _context.StudentBatchAssignments
					.Where(a => a.StudentId == dto.StudentId)
					.OrderByDescending(a => a.BatchId)
					.FirstOrDefault();

				if (assignedBatch != null && student != null && student.UserId.HasValue)
				{
					await _notificationService.SendNotificationToUserAsync(
						student.UserId.Value,
						"Batch Assigned",
						$"You have been assigned to Batch {assignedBatch.BatchId}.",
						"Student"
					);
				}

				await transaction.CommitAsync();

				return Ok(new
				{
					message = "Enrollment + Auto Assignment successful"
				});
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				return StatusCode(500, new
				{
					error = ex.InnerException?.Message ?? ex.Message
				});
			}
		}

		private async Task AutoAssignStudentToBatch(string studentId, string courseId)
		{
			int batchSize = GetBatchSize(courseId);

			// ✅ 1. Find existing batch (NOT FULL)
			var batch = _context.CourseBatches
				.FirstOrDefault(b =>
					b.CourseId == courseId &&
					b.CurrentStudents < b.MaxStudents &&
					!b.IsActive);

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

					if (batch.CurrentStudents == batch.MaxStudents)
						batch.IsActive = true;

					var enrollment = _context.Enrollment
						.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

					if (enrollment != null)
						enrollment.Status = "Assigned";

					_context.SaveChanges();

					// ✅ Instructor Notification (awaited)
					var instructor = _context.Instructor
						.FirstOrDefault(i => i.InstructorId == batch.InstructorId);

					if (instructor != null && instructor.UserId.HasValue)
					{
						await _notificationService.SendNotificationToUserAsync(
							instructor.UserId.Value,
							"New Student Assigned",
							$"A new student (ID: {studentId}) has been added to your batch {batch.BatchId}.",
							"Instructor"
						);
					}
				}

				return;
			}

			// ✅ 2. Find available instructor
			var instructorAvailable = _context.Instructor
				.FirstOrDefault(i =>
					!_context.CourseBatches.Any(b =>
						b.InstructorId == i.InstructorId && b.IsActive));

			if (instructorAvailable == null)
			{
				Console.WriteLine("No instructor available");
				return;
			}

			// ✅ 3. Create new batch
			string batchId = $"B{_context.CourseBatches.Count() + 1:D3}";

			var newBatch = new CourseBatch
			{
				BatchId = batchId,
				CourseId = courseId,
				InstructorId = instructorAvailable.InstructorId,
				MaxStudents = batchSize,
				CurrentStudents = 1,
				IsActive = false
			};

			_context.CourseBatches.Add(newBatch);

			_context.StudentBatchAssignments.Add(new StudentBatchAssignment
			{
				StudentId = studentId,
				BatchId = batchId
			});

			var newEnrollment = _context.Enrollment
				.FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

			if (newEnrollment != null)
				newEnrollment.Status = "Assigned";

			_context.SaveChanges();

			// ✅ Instructor Notification for new batch (awaited)
			if (instructorAvailable.UserId.HasValue)
			{
				await _notificationService.SendNotificationToUserAsync(
					instructorAvailable.UserId.Value,
					"New Batch Created",
					$"A new batch {batchId} has been created and student (ID: {studentId}) has been assigned to it.",
					"Instructor"
				);
			}
		}


		private int GetBatchSize(string courseId)

		{

			var config = _context.BatchConfigs

			.FirstOrDefault(c => c.CourseId == courseId);


			return config?.BatchSize ?? 2; // default = 2 (your test case)   
		}

		[Authorize(Roles = "Coordinator,Admin")]		
		[HttpGet("instructor/{instructorId}/batches")]

		public IActionResult GetInstructorBatches(string instructorId)

		{


			return Ok(_service.GetInstructorBatches(instructorId));

		}
		[HttpGet("course/{courseId}/batches")]
		public IActionResult GetCourseBatches(string courseId)
		{
			var batches = _context.CourseBatches
				.Include(b => b.Instructor)
				.Where(b => b.CourseId == courseId)
				.Select(b => new
				{
					b.BatchId,

					b.MaxStudents,
					b.CurrentStudents,
					b.IsActive,
					Instructor = b.Instructor != null ? b.Instructor.InstructorName : "Unassigned"
				})
				.ToList();

			return Ok(batches);
		}

		[HttpGet("course/{courseId}/students")]
		public IActionResult GetCourseStudents(string courseId)
		{
			var students = _context.Enrollment
				.Include(e => e.Student)
				.Where(e => e.CourseId == courseId && e.Status == "Active")
				.Select(e => new
				{
					e.Student.StudentId,
					Name = e.Student.StudentName,
					BatchName = _context.StudentBatchAssignments
						.Where(a => a.StudentId == e.StudentId)
						.Select(a => a.CourseBatch.BatchId)
						.FirstOrDefault(),

				})
				.ToList();

			return Ok(students);
		}
		[HttpGet("dashboard/stats")]
		public IActionResult GetDashboardStats()
		{
			var stats = new[]
			{
			new { title = "Programs", value = _context.Programs.Count(), icon = "bi-journal-bookmark" },
			new { title = "Courses", value = _context.Course.Count(), icon = "bi-book" },
			new { title = "Batches", value = _context.CourseBatches.Count(), icon = "bi-people" },
			new { title = "Enrolled Students", value = _context.Enrollment.Count(e => e.Status == "Assigned"||e.Status == "Active"), icon = "bi-person-check" }
		};

			return Ok(stats);
		}
		[HttpGet("dashboard/enrollment-trends")]
		public IActionResult GetEnrollmentTrends()
		{
			var trends = _context.Enrollment
				.GroupBy(e => e.EnrollmentDate.Month)
				.Select(g => new { month = g.Key, count = g.Count() })
				.OrderBy(x => x.month)
				.ToList();

			return Ok(trends);
		}
		// ✅ Batch Performance
		[HttpGet("dashboard/performance")]
		public IActionResult GetBatchPerformance()
		{
			var performance = _context.CourseBatches
				.Select(cb => new
				{
					batchId = cb.BatchId,
					courseId = cb.CourseId,
					instructorId = cb.InstructorId,
					maxStudents = cb.MaxStudents,
					currentStudents = cb.CurrentStudents,
					progress = cb.MaxStudents > 0
						? (double)cb.CurrentStudents / cb.MaxStudents * 100
						: 0
				})
				.ToList();

			return Ok(performance);
		}
		[HttpGet("dashboard/students-by-program")]
		public IActionResult GetStudentsByProgram()
		{
			var data = _context.Student
				.GroupBy(s => s.StudentProgram)
				.Select(g => new { program = g.Key, count = g.Count() })
				.ToList();

			return Ok(data);
		}

		// ✅ Gender Distribution
		[HttpGet("dashboard/gender-distribution")]
		public IActionResult GetGenderDistribution()
		{
			var data = _context.Student
				.GroupBy(s => s.StudentGender)
				.Select(g => new { gender = g.Key, count = g.Count() })
				.ToList();

			return Ok(data);
		}
		[HttpGet("dashboard/notifications")]
		public IActionResult GetNotifications()
		{
			var notes = _context.Enrollment
				.OrderByDescending(e => e.EnrollmentDate)
				.Take(5)
				.Select(e => new
				{
					title = "New Enrollment",
					message = $"{e.Student.StudentName} enrolled in {e.Course.CourseName}",
					time = e.EnrollmentDate.ToString("dd MMM yyyy")
				})
				.ToList();

			return Ok(notes);
		}
		[HttpGet("instructor/{id}/details")]
		public async Task<IActionResult> GetInstructorDetails(string id)
		{
			var instructor = await _context.Instructor
				.FirstOrDefaultAsync(i => i.InstructorId == id);

			if (instructor == null)
			{
				return NotFound(new { message = "Instructor not found" });
			}

			// Returning the specific fields from your model
			return Ok(new
			{
				instructor.InstructorId,
				instructor.InstructorName,
				instructor.InstructorEmail,
				instructor.InstructorPhone,
				instructor.InstructorQualification,
				instructor.InstructorSkills,
				instructor.InstructorExperience,
				instructor.InstructorJoinDate,
				instructor.InstructorGender,
				instructor.ResumePath,

			});
		}
		[HttpGet("instructorsList")]
		[Authorize]
		public async Task<IActionResult> GetInstructors()
		{
			try
			{
				var instructors = await _context.Users
					.Include(u => u.Instructor) // Load the related Instructor data
					.Where(u => u.Role == "Instructor")
					.Select(u => new
					{
						// Matches 'i.userId' in your React code
						UserId = u.UserId,
						// Uses Instructor Name if available, otherwise fallback to Email
						FullName = u.Instructor != null ? u.Instructor.InstructorName : u.Email,
						Email = u.Email
					})
					.ToListAsync();

				return Ok(instructors);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error fetching instructors");
			}
		}
		[HttpGet("GetAllBatchess")]
		[Authorize]
		public async Task<IActionResult> GetAllBatches()
		{
			try
			{
				var batches = await _context.CourseBatches
					.Select(b => new
					{
						// Matches 'b.batchId' in your React map function
						BatchId = b.BatchId
					})
					.ToListAsync();

				return Ok(batches);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Error fetching batches");
			}
		}




		//[HttpGet("instructor/{instructorId}/dashboard")]//public IActionResult InstructorDashboard(string instructorId)//{//  return Ok(_service.InstructorDashboard(instructorId));//}  }
	}
}