using EduTrackAcademics.DTO;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackAcademics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[EnableCors("MyCorsPolicy")]
	public class RegistrationController : ControllerBase
	{
		private readonly IRegistrationService _registrationService;

		public RegistrationController(IRegistrationService registrationService)
		{
			_registrationService = registrationService;
		}
		// Student Registration
		[AllowAnonymous]
		[HttpPost("Student")]
		public async Task<IActionResult> RegisterStudent([FromForm] StudentDTO dto)
		{

			await _registrationService.RegisterStudentAsync(dto);
			return Ok(new { message = "Student registered successfully" });
		}

		//  Instructor Registration
<<<<<<< HEAD
		//[Authorize(Roles ="Coordinator")]
=======

	[Authorize(Roles ="Coordinator")]

>>>>>>> 4b84423fc6e32082a4f6a3234fd1ebf7a8fcc697
		[HttpPost("Instructor")]
		public async Task<IActionResult> RegisterInstructor([FromForm] InstructorDTO dto)
		{
			await _registrationService.RegisterInstructorAsync(dto);
			return Ok(new { message = "Instructor registered successfully" });
		}

		//  Coordinator Registration
<<<<<<< HEAD
		//[Authorize(Roles = "Admin")]
=======
	[Authorize(Roles = "Admin")]
>>>>>>> 4b84423fc6e32082a4f6a3234fd1ebf7a8fcc697
		[HttpPost("Coordinator")]
		public async Task<IActionResult> RegisterCoordinator([FromForm] CoordinatorDTO dto)
		{

			await _registrationService.RegisterCoordinatorAsync(dto);
			return Ok(new { message = "Coordinator registered successfully" });
		}
	}
}

