using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Mvc;
using EduTrackAcademics.DTO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors;

namespace EduTrackAcademics.Controllers
{
	[ApiController]
	[Route("api/profile")]
	[EnableCors("AllowReact")]
	public class StudentController : ControllerBase
	{
		private readonly IStudentProfileService _service;

		public StudentController(IStudentProfileService service)
		{
			_service = service;
		}

		//[Authorize(Roles ="Admin")]
		[HttpGet("GetAll-Students")]
		public async Task<IActionResult> GetAllStudentsAsync()
		{

			var students = await _service.GetAllStudentsAsync();
			return Ok(students);
		}

		//[Authorize(Roles ="Student")]
		[HttpGet("Personal-Information/{studentId}")]
		public async Task<IActionResult> GetPersonalInfo(string studentId)
		{
			var result = await _service.GetPersonalInfoAsync(studentId);
			return Ok(result);
		}

		//[Authorize(Roles = "Student")]
		[HttpGet("Program-Details/{studentId}")]
		public async Task<IActionResult> GetProgramDetails(string studentId)
		{
			var result = await _service.GetProgramDetails(studentId);
			return Ok(new
			{
				Details = result

			});

		}

		//[Authorize(Roles = "Student")]
		[HttpPut("Additional-Information/{studentId}")]
		public async Task<IActionResult> UpdateAdditionalInfo(string studentId, [FromBody] StudentAdditionalDetailsDTO dto)
		{
			await _service.UpdateAdditionalInfo(studentId, dto);
			return Ok(new { Message = "Additional information updated successfully." });
		}

		//[Authorize(Roles = "Student")]
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


		//[Authorize(Roles = "Student")]
		[HttpGet("Assignment-Due")]
		public async Task<IActionResult> GetAssignmentDue(string studentId, string courseId)
		{
			var assignment = await _service.GetAssignmentDetailsForStudentAsync(studentId, courseId);
			return Ok(new
			{
				AssignmentDue = assignment.DueDate,
				CoureName = assignment.CourseName,
				Message = "Assignment details retrieved successfully."
			});

		}

	}
}
