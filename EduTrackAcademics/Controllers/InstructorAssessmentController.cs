using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Model;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduTrackAcademics.Controllers
{
	[Route("api/instructorAssessmentQuestion")]
	[ApiController]
	public class InstructorAssessmentController : ControllerBase
	{
		private readonly IInstructorAssessmentService _service;
		private readonly EduTrackAcademicsContext _context;

		public InstructorAssessmentController(IInstructorAssessmentService service, EduTrackAcademicsContext context)
		{
			_service = service;
			_context = context;
		}
		// ASSESSMENT

		[Authorize(Roles = "Instructor")]

		[HttpPost("assessment")]
		public async Task<IActionResult> CreateAssessment([FromBody] AssessmentDTO dto)
		{
			if (dto == null)
				return BadRequest("Invalid data");

			try
			{
				var result = await _service.CreateAssessmentAsync(dto);
				return Ok(new { message = result });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize(Roles = "Coordinator, Instructor")]
		[HttpGet("assessments")]
		public async Task<IActionResult> GetAllAssessments()
		{
			try
			{
				var data = await _service.GetAllAssessmentsAsync();
				return Ok(data);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("assessments/date/{date}")]
		public async Task<IActionResult> GetByDate(DateTime date)
		{
			return Ok(await _service.GetAssessmentsByDateAsync(date));
		}

		[Authorize(Roles = "Instructor, Student")]
		[HttpGet("assessmentDetails/{id}")]
		public async Task<IActionResult> GetAssessment(string id)
		{
			var result = await _service.GetAssessmentByIdAsync(id);
			if (result == null)
				return NotFound("Assessment not found");
			return Ok(result);
		}

		[Authorize(Roles = "Instructor, Student")]
		[HttpGet("assessmentQuestions/{id}")]
		public async Task<IActionResult> GetAllQuestions(string id)
		{
			var questions = await _context.Questions
				.Where(q => q.AssessmentId == id)
				.ToListAsync();

			if (questions == null || !questions.Any())
				return NotFound($"No questions found for assessment {id}");

			return Ok(questions);
		}



	[Authorize(Roles = "Admin, Coordinator, Instructor, Student")]    

		[HttpGet("assessments/course/{courseId}")]
		public async Task<IActionResult> GetAssessmentsByCourse(string courseId)
			=> Ok(await _service.GetAssessmentsByCourseAsync(courseId));

		[Authorize(Roles = "Instructor")]
		[HttpPut("assessment/{id}")]
		public async Task<IActionResult> UpdateAssessment(string id, AssessmentDTO dto)
			=> Ok(await _service.UpdateAssessmentAsync(id, dto));

		[Authorize(Roles = "Instructor")]
		[HttpDelete("assessment/{id}")]
		public async Task<IActionResult> DeleteAssessment(string id)
			=> Ok(await _service.DeleteAssessmentAsync(id));

		// QUESTIONS

		[Authorize(Roles = "Instructor")]
		[HttpPost("question")]
		public async Task<IActionResult> AddQuestion(QuestionDTO dto)
		{
			try
			{
				var result = await _service.AddQuestionAsync(dto);
				return Ok(new { message = result });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.InnerException?.Message ?? ex.Message);
			}
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("question/{QuestionId}")]
		public async Task<IActionResult> GetQuestion(string QuestionId)
			=> Ok(await _service.GetQuestionByIdAsync(QuestionId));

		[Authorize(Roles = "Instructor")]
		[HttpGet("questions/assessment/{assessmentId}")]
		public async Task<IActionResult> GetQuestionsByAssessment(string assessmentId)
			=> Ok(await _service.GetQuestionsByAssessmentAsync(assessmentId));

		[Authorize(Roles = "Instructor")]
		[HttpPut("question/{QuestionId}")]
		public async Task<IActionResult> UpdateQuestion(string QuestionId, QuestionDTO dto)
			=> Ok(await _service.UpdateQuestionAsync(QuestionId, dto));

		[Authorize(Roles = "Instructor")]
		[HttpDelete("question/{QuestionId}")]
		public async Task<IActionResult> DeleteQuestion(string QuestionId)
		{
			var result = await _service.DeleteQuestionAsync(QuestionId);
			return Ok(result);
		}

		// Fetching Data

		// Get Status
		[Authorize(Roles = "Instructor")]
		[HttpGet("status")]
		public async Task<IActionResult> GetStatus(string studentId, string assessmentId)
		{
			var status = await _service.GetSubmissionStatus(studentId, assessmentId);
			return Ok(status);
		}

		// Submit Assessment
		//[Authorize(Roles = "Instructor")]
		//[HttpPost("submit")]
		//public async Task<IActionResult> Submit(string studentId, string assessmentId, int score, string feedback)
		//{
		//	try
		//	{
		//		var result = await _service.SubmitAssessment(studentId, assessmentId, score, feedback);
		//		return Ok(result);
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest(ex.Message);
		//	}
		//}

		// Dashboard
		[Authorize(Roles = "Instructor")]
		[HttpGet("student/{studentId}")]
		public async Task<IActionResult> GetDashboard(string studentId)
		{
			var data = await _service.GetStudentDashboard(studentId);
			return Ok(data);
		}

		// GET RESULT
		[Authorize(Roles = "Instructor")]
		[HttpGet("result")]
		public async Task<IActionResult> GetResult(string studentId, string assessmentId)
		{
			var result = await _service.GetResult(studentId, assessmentId);

			if (result == null)
				return NotFound("Submission not found");

			return Ok(result);
		}

		// GET submission (for resume exam)
		[Authorize(Roles = "Instructor")]
		[HttpGet("submission")]
		public async Task<IActionResult> GetSubmission(string studentId, string assessmentId)
		{
			var submission = await _service.GetSubmission(studentId, assessmentId);

			if (submission == null)
				return NotFound("No submission found");

			return Ok(submission);
		}

		[Authorize(Roles = "Coordinator, Instructor")]
		[HttpGet("courses/academic-year/{academicYearId}")]
		public IActionResult GetCoursesByAcademicYear(string academicYearId)
		{
			var result = _service.GetCoursesByAcademicYear(academicYearId);
			return Ok(result);
		}

		[Authorize(Roles = "Coordinator, Instructor")]
		[HttpGet("instructor/{instructorId}/courses")]
		public async Task<IActionResult> GetCourses(string instructorId)
		{
			var data = await _service.GetCoursesByInstructorAsync(instructorId);
			return Ok(data);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("assessment/{assessmentId}/status")]
		public async Task<IActionResult> GetAssessmentStatus(string assessmentId)
		{
			try
			{
				var result = await _service.GetAssessmentStatusAsync(assessmentId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("assessment/{assessmentId}/details")]
		public async Task<IActionResult> GetSubmissionDetails(string assessmentId)
		{
			try
			{
				var data = await _service.GetSubmissionDetailsAsync(assessmentId);

				return Ok(new
				{
					status = 200,
					data = data
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}
