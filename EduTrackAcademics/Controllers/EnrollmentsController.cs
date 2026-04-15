using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableCors("MyCorsPolicy")]
	public class EnrollmentController : ControllerBase
	{
		private readonly IEnrollmentService _service;

		public EnrollmentController(IEnrollmentService service)
		{
			_service = service;
		}

		//Insert into the enrollment table When student enrolled to a course
		[Authorize(Roles = "Student")]
		[HttpPost]
		public async Task<IActionResult> AddEnrollment([FromBody] EnrollmentDto dto)
		{
			try
			{
				var result = await _service.AddEnrollmentAsync(dto);

				return Ok(new
				{
					status = 200,
					data = $"Inserted Into Enrollment Table with enrollment id {result}"
				});
			}
			catch (ApplicationException ex)
			{
				return StatusCode(501,
				new { status = 501, message = ex.Message });
			}

		}

		// Display the content of the enrolled course
		[Authorize(Roles = "Student")]
		[HttpGet("content")]
		public async Task<IActionResult> ViewCourseContent([FromQuery] EnrollmentDto dto)
		{
			try
			{
				var modules = await _service
					.GetContentForStudentAsync(dto.StudentId, dto.CourseId);

				return Ok(new { status = 200, data = modules });
			}
			catch (ApplicationException ex)
			{
				return StatusCode(500,
					new { status = 500, message = ex.Message });
			}

		}


		[Authorize(Roles = "Student")]
		[HttpPost("mark-completed")]
		public async Task<IActionResult> MarkContentCompleted([FromBody] MarkCompletedDto dto)
		{
			try
			{
				var result = await _service.MarkAsCompletedAndSyncStatusAsync(dto.StudentId, dto.CourseId, dto.ContentId);

				return Ok(new
				{
					status = 200,
					progress = result.ProgressPercentage,
					enrollmentStatus = result.Status,
					message = result.Message
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { status = 500, message = ex.Message });
			}
		}

		// course progress(percentage) is calculated and displayed 
		[Authorize(Roles = "Student")]
		[HttpGet("progress")]
		public async Task<IActionResult> GetCourseProgress([FromQuery] EnrollmentDto dto)
		{
			try
			{
				var progress = await _service
					.GetCourseProgressPercentageAsync(dto.StudentId, dto.CourseId);

				return Ok(new { status = 200, progress });
			}
			catch (ApplicationException ex)
			{
				return StatusCode(500,
					new { status = 500, message = ex.Message });
			}

		}

		//Update the status in enrollment table Active->completed
		[Authorize(Roles = "Student")]
		[HttpGet("status")]
		public async Task<IActionResult> GetCourseStatus([FromQuery] EnrollmentDto dto)
		{
			try
			{
				var result = await _service.GetCourseStatusAsync(dto.StudentId, dto.CourseId);

				return Ok(new
				{
					status = 200,
					CurrentStatus = result
				});
			}
			catch (ApplicationException ex)
			{
				return StatusCode(500,
					new { status = 500, message = ex.Message });
			}

		}

		//Individual student attendance for the courses that student enrolled
		[Authorize(Roles = "Student,Instructor")]
		[HttpGet("student-attendance/{studentId}")]
		public async Task<IActionResult> GetStudentAttendance(string studentId)
		{
			var data = await _service.CalculateStudentAttendanceByStudentIdAsync(studentId);

			return Ok(new
			{
				status = 200,
				studentId,
				attendance = data
			});
		}

		// Returns the avarage attendace percentage of batches that are in particular course
		[Authorize(Roles = "Instructor,Coordinator,Admin")]
		[HttpGet("course-batch-attendance/{courseId}")]
		public async Task<IActionResult> GetBatchWiseAttendance(string courseId)
		{
			try
			{
				var result = await _service.GetBatchWiseAttendanceAsync(courseId);

				if (result == null || result.Count == 0)
				{
					return NotFound(new
					{
						status = 404,
						message = "No batches found for this course"
					});
				}

				return Ok(new
				{
					status = 200,
					courseId,
					batchWiseAttendance = result
				});
			}
			catch (ApplicationException ex)
			{
				return StatusCode(500, new
				{
					status = 500,
					message = "Error fetching batch attendance",
					error = ex.Message
				});
			}
		}


		// Get all courses that match the student's Program and Qualification
		[Authorize(Roles = "Student")]
		[HttpGet("available-courses/{studentId}")]
		public async Task<IActionResult> GetAvailableCourses(string studentId)
		{
			try
			{
				var courses = await _service.GetAvailableCoursesForStudentAsync(studentId);

				if (courses == null || !courses.Any())
				{
					return NotFound(new
					{
						status = 404,
						message = "No courses found matching your program and qualification."
					});
				}

				return Ok(new { status = 200, data = courses });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { status = 500, message = ex.Message });
			}
		}

		// Search courses by name within the student's assigned Program/Qualification
		[Authorize(Roles = "Student")]
		[HttpGet("search-by-name")]
		public async Task<IActionResult> SearchCoursesByName([FromQuery] string studentId, [FromQuery] string courseName)
		{
			try
			{
				if (string.IsNullOrEmpty(courseName))
				{
					return BadRequest(new { status = 400, message = "Course name is required for search." });
				}

				var results = await _service.SearchCoursesForStudentAsync(studentId, courseName);

				if (results == null || results.Count == 0)
				{
					return NotFound(new { status = 404, message = "No courses found for your program." });
				}

				return Ok(new { status = 200, data = results });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { status = 500, message = ex.Message });
			}
		}


		// Get all courses that the student is currently enrolled in
		[Authorize(Roles = "Student")]
		[HttpGet("my-courses/{studentId}")]
		public async Task<IActionResult> GetMyEnrolledCourses(string studentId)
		{
			try
			{
				var enrolledCourses = await _service.GetStudentEnrolledCoursesAsync(studentId);

				if (enrolledCourses == null || !enrolledCourses.Any())
				{
					return Ok(new
					{
						status = 200,
						message = "You are not enrolled in any courses yet.",
					});
				}

				return Ok(new { status = 200, data = enrolledCourses });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { status = 500, message = ex.Message });
			}
		}

		// Search specifically within the student's current active enrollments
		[Authorize(Roles = "Student")]
		[HttpGet("search-my-courses")]
		public async Task<IActionResult> SearchMyCourses([FromQuery] string studentId, [FromQuery] string courseName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(courseName))
				{
					return BadRequest(new { status = 400, message = "Search term is required." });
				}

				var results = await _service.SearchStudentEnrolledCoursesAsync(studentId, courseName);

				return Ok(new
				{
					status = 200,
					count = results.Count,
					data = results
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { status = 500, message = ex.Message });
			}
		}

		//[Authorize(Roles = "Student")]
		[HttpPatch("sync-status")]
		public async Task<IActionResult> SyncEnrollmentStatus([FromQuery] string studentId)
		{
			try
			{
				// We now check the parameter instead of the User Claims
				if (string.IsNullOrEmpty(studentId))
				{
					return BadRequest(new { status = 400, message = "Student ID is required to sync status." });
				}

				// Silently check and update statuses
				await _service.CheckAndUpdateDropoutStatusAsync(studentId);

				return Ok(new { status = 200, message = "Status synchronized successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { status = 500, message = ex.Message });
			}
		}
	}
}