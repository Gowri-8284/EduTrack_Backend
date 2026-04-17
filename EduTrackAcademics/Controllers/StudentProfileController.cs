using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Mvc;
using EduTrackAcademics.DTO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors;
using EduTrackAcademics.Data;

namespace EduTrackAcademics.Controllers
{
	[ApiController]
	[Route("api/profile")]
	[EnableCors("MyCorsPolicy")]
	public class StudentController : ControllerBase
	{
		private readonly IStudentProfileService _service;
		private readonly EduTrackAcademicsContext _context;

		public StudentController(IStudentProfileService service,EduTrackAcademicsContext context)
		{
			_service = service;
			_context = context;
		}

		[Authorize(Roles ="Admin")]
		[HttpGet("GetAll-Students")]
		public async Task<IActionResult> GetAllStudentsAsync()
		{

			var students = await _service.GetAllStudentsAsync();
			return Ok(students);
		}

		[Authorize(Roles ="Student,Instructor")]
		[HttpGet("Personal-Information/{studentId}")]
		public async Task<IActionResult> GetPersonalInfo(string studentId)
		{
			var result = await _service.GetPersonalInfoAsync(studentId);
			return Ok(result);
		}

		[Authorize(Roles = "Student,Instructor")]
		[HttpGet("Program-Details/{studentId}")]
		public async Task<IActionResult> GetProgramDetails(string studentId)
		{
			var result = await _service.GetProgramDetails(studentId);
			return Ok(new
			{
				Details = result

			});

		}

	[Authorize(Roles = "Student")]
		[HttpPut("Additional-Information/{studentId}")]
		public async Task<IActionResult> UpdateAdditionalInfo(string studentId, [FromBody] StudentAdditionalDetailsDTO dto)
		{
			await _service.UpdateAdditionalInfo(studentId, dto);
			return Ok(new { Message = "Additional information updated successfully." });
		}

		[Authorize(Roles = "Student")]
		[HttpGet("GetAdditional-Information/{studentId}")]
		public async Task<IActionResult> GetAdditionalInfo(string studentId)
		{
			var result = await _service.GetAdditionalInfoAsync(studentId);
			return Ok(result);
		}

		[Authorize(Roles = "Student")]
		[HttpGet("Credit-points/{studentId}")]
		public async Task<IActionResult> GetCreditPoints(string studentId)
		{
			var credits = await _service.GetCreditPointsAsync(studentId);

			return Ok(new
			{
				StudentId = studentId,
				TotalCredits = credits
			});
		}


		[Authorize(Roles = "Student")]
		[HttpGet("Assignments-Due/{studentId}")]
		public async Task<IActionResult> GetAssignmentsDue(string studentId)
		{
			var assignments = await _service.GetAssignmentsForStudentAsync(studentId);

			var response = assignments.Select(a => new
			{
				AssignmentDue = a.DueDate,
				CourseName = a.CourseName
			});

			return Ok(new
			{
				Data = response,
				Count = response.Count(),
				Message = "Student assignment list retrieved successfully."
			});
		}


		[HttpGet("domain-id/{userId}")]
		public IActionResult GetDomainIdByUserId(int userId)
		{
			// 1️⃣ Get user & role
			var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
			if (user == null)
				return NotFound("User not found");

			// 2️⃣ Role-based ID mapping
			if (user.Role == "Student")
			{
				var studentId = _context.Student
					.Where(s => s.UserId == userId)
					.Select(s => s.StudentId)
					.FirstOrDefault();

				return Ok(new
				{
					role = "Student",
					studentId
				});
			}

			if (user.Role == "Instructor")
			{
				var instructorId = _context.Instructor
					.Where(i => i.UserId == userId)
					.Select(i => i.InstructorId)
					.FirstOrDefault();

				return Ok(new
				{
					role = "Instructor",
					instructorId
				});
			}

			if (user.Role == "Coordinator")
			{
				var coordinatorId = _context.Coordinator
					.Where(c => c.UserId == userId)
					.Select(c => c.CoordinatorId)
					.FirstOrDefault();

				return Ok(new
				{
					role = "Coordinator",
					coordinatorId
				});
			}

			// Admin or others
			return Ok(new
			{
				role = user.Role,
				userId
			});
		}

	}
}
