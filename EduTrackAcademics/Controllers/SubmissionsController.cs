using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Humanizer;
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
	public class SubmissionController : ControllerBase
	{
		private readonly ISubmissionService _service;

		public SubmissionController(ISubmissionService service)
		{
			_service = service;
		}

		//Student can view the assessments that are belong to that particular studnet id
		[Authorize(Roles ="Student")]
		[HttpGet("view-assessments")]
		public async Task<IActionResult> ViewAssessments([FromQuery] FetchAssessmentDto dto)
		{
			try
			{
				var data = await _service.GetStudentAssessmentsAsync(dto.StudentId);

				if (data == null || !data.Any())
				{
					return NotFound(new
					{
						status = 404,
						message = "No assessments found"
					});
				}

				return Ok(new
				{
					status = 200,
					data = data
				});
			}
			catch (ApplicationException ex)
			{
				return StatusCode(500, new
				{
					status = 500,
					message = ex.Message
				});
			}
		}

		//Student can able to start the assesment when it is active and questions will be displayed
		[Authorize(Roles = "Student")]
		[HttpGet("start-assessment")]
		public async Task<IActionResult> StartAssessment(string studentId, string assessmentId)
		{
			try
			{
				var questions = await
					_service.StartAssessmentAsync(
						studentId, assessmentId);

				return Ok(new
				{
					status = 200,
					data = questions
				});
			}
			catch (ApplicationException ex)
			{
				return BadRequest(new
				{
					status = 400,
					message = ex.Message
				});
			}
		}

		//Student can answer the questions the answers are stored in the Student answer table
		[Authorize(Roles = "Student")]
		[HttpPost("answer")]
		public async Task<IActionResult> InsertOrUpdateAnswer(
		[FromBody] StudentAnswerDto dto)
		{
			await _service.InsertOrUpdateAnswerAsync(dto);

			return Ok(new
			{
				status = 200,
				Message = "Answer saved successfully"
			});
		}

		//Submit option is to sumbit the assessment for evaluation 
		[Authorize(Roles = "Student")]
		[HttpPost("submit")]
		public async Task<IActionResult> SubmitAssessment([FromBody] SubmitAssessmentDto dto)
		{
			var result = await _service.SubmitAssessmentAsync(dto);

			return Ok(new
			{
				SubmissionId = result,
				Message = "Assessment submitted successfully"
			});
		}

		//After submission Feedback column is updated and scores are evaluated
		[Authorize(Roles = "Student")]
		[HttpPut("UpdateFeedback")]
		public async Task<IActionResult> UpdateSubmission(SubmitFeedbackDto dto)
		{

			await _service.AddFeedbackAsync(dto);

			return Ok(new
			{
				Message = "Feedback Subimitted successfully"
			});
		}

		[Authorize(Roles ="Student")]
		[HttpGet("Score")]
		public async Task<IActionResult> ViewScoreandUpdateSubmission([FromQuery] string studentId, string assessmentId)
		{
			var result = await _service.UpdateSubmissionAsync(studentId, assessmentId);
			return Ok(new
			{
				Score = result.Score,
				Percentage = result.Percentage,
				Message = "Score updated successfully"
			});
		}


	[Authorize(Roles = "Instructor,Admin")]
		[HttpGet("assessment/{assessmentId}")]
		public async Task<IActionResult> GetAllSubmissions(string assessmentId)
		{
			try
			{
				var data = await _service.GetAllSubmissionsByAssessmentAsync(assessmentId);

				if (data == null || !data.Any())
				{
					return NotFound(new { status = 404, message = "No submissions found for this assessment" });
				}

				return Ok(new { status = 200, data = data });
			}
			catch (Exception ex)
			{
				return BadRequest(new { status = 400, message = ex.Message });
			}
		}

		// Fetch EVERY submission record in the system
		[Authorize(Roles = "Admin,Instructor")]
		[HttpGet("all-records")]
		public async Task<IActionResult> GetAllSubmissions()
		{
			try
			{
				var data = await _service.GetAllSubmissionsAsync();
				return Ok(new
				{
					status = 200,
					totalCount = data.Count,
					data = data
				});
			}
			catch (Exception ex)
			{
				return NotFound(new
				{
					status = 404,
					message = ex.Message
				});
			}
		}
	}
}
