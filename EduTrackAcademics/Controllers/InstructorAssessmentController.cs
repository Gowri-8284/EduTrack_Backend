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
		//[Authorize(Roles = "Instructor")]    
		[HttpPost("assessment")]

		public async Task<IActionResult> CreateAssessment([FromBody] AssessmentDTO dto)

		{

			if (dto == null)

			{

				return BadRequest("Invalid data");

			}

			var result = await _service.CreateAssessmentAsync(dto);

			return Ok(new
			{

				Message = result
			});

		}


		[HttpGet("assessments")]

		public async Task<IActionResult> GetAll()

		{

			return Ok(await _service.GetAllAssessmentsAsync());

		}


		[HttpGet("assessments/date/{date}")]

		public async Task<IActionResult> GetByDate(DateTime date)

		{

			return Ok(await _service.GetAssessmentsByDateAsync(date));

		}


		//[Authorize(Roles = "Admin, Coordinator, Instructor, Student")]    
		[HttpGet("assessmentDetails/{id}")]

		public async Task<IActionResult> GetAssessment(string id)

	=> Ok(await _service.GetAssessmentByIdAsync(id));


	//	[Authorize(Roles = "Admin, Coordinator, Instructor, Student")]    
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


	//	[Authorize(Roles = "Admin, Coordinator, Instructor, Student")]    
		[HttpGet("assessments/course/{courseId}")]

		public async Task<IActionResult> GetAssessmentsByCourse(string courseId)

	=> Ok(await _service.GetAssessmentsByCourseAsync(courseId));


		//[Authorize(Roles = "Coordinator, Instructor")]    
		[HttpPut("assessment/{id}")]

		public async Task<IActionResult> UpdateAssessment(string id, AssessmentDTO dto)

	=> Ok(await _service.UpdateAssessmentAsync(id, dto));


		//[Authorize(Roles = "Admin, Coordinator")]    
		[HttpDelete("assessment/{id}")]

		public async Task<IActionResult> DeleteAssessment(string id)

	=> Ok(await _service.DeleteAssessmentAsync(id));



		
		//[Authorize(Roles = "Instructor")]    
		[HttpPost("question")]

		public async Task<IActionResult> AddQuestion(QuestionDTO dto)

	   => Ok(await _service.AddQuestionAsync(dto));


		//[Authorize(Roles = "Admin, Coordinator, Instructor")]    
		[HttpGet("question/{QuestionId}")]

		public async Task<IActionResult> GetQuestion(string QuestionId)

	=> Ok(await _service.GetQuestionByIdAsync(QuestionId));


	//	[Authorize(Roles = "Admin, Coordinator, Instructor, Student")]    
		[HttpGet("questions/assessment/{assessmentId}")]

		public async Task<IActionResult> GetQuestionsByAssessment(string assessmentId)

	=> Ok(await _service.GetQuestionsByAssessmentAsync(assessmentId));


		//[Authorize(Roles = "Coordinator, Instructor")]    
		[HttpPut("question/{QuestionId}")]

		public async Task<IActionResult> UpdateQuestion(string QuestionId, QuestionDTO dto)

	=> Ok(await _service.UpdateQuestionAsync(QuestionId, dto));


		//[Authorize(Roles = "Admin, Coordinator")]    
		[HttpDelete("question/{QuestionId}")]

		public async Task<IActionResult> DeleteQuestion(string QuestionId)

		{

			var result = await _service.DeleteQuestionAsync(QuestionId);

			return Ok(result);

		}


		// Fetching Data// ✅ GET STATUS    
		[HttpGet("status")]

		public async Task<IActionResult> GetStatus(string studentId, string assessmentId)

		{

			var status = await _service.GetStatus(studentId, assessmentId);


			return Ok(new
			{

				studentId,

				assessmentId,

				status
			});

		}


		// ✅ GET RESULT    
		[HttpGet("result")]

		public async Task<IActionResult> GetResult(string studentId, string assessmentId)

		{

			var result = await _service.GetResult(studentId, assessmentId);


			if (result == null)

				return NotFound("Submission not found");


			return Ok(result);

		}


		// 🔹 GET submission (for resume exam)    
		[HttpGet("submission")]

		public async Task<IActionResult> GetSubmission(string studentId, string assessmentId)

		{

			var submission = await _service.GetSubmission(studentId, assessmentId);


			if (submission == null)

				return NotFound("No submission found");


			return Ok(submission);

		}


		// 🔹 GET all submissions (dashboard)    
		[HttpGet("student/{studentId}")]

		public async Task<IActionResult> GetStudentSubmissions(string studentId)

		{

			var submissions = await _service.GetStudentSubmissions(studentId);


			return Ok(submissions);

		}

	}

}