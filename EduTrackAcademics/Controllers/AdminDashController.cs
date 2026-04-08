using EduTrackAcademics.DTO;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackAcademics.Controllers

{

	[ApiController]

	[Route("api/admin")]

	[EnableCors("MyCorsPolicy")]



	public class AdminDashboardController : ControllerBase
	{

		private readonly IAdminService _service;


		public AdminDashboardController(IAdminService service)

		{

			_service = service;

		}


		[HttpPost("qualification")]

		public IActionResult AddQualification([FromBody] QualificationDTO dto)

		{

			var result = _service.AddQualification(dto);

			return Ok(result);

		}

		[HttpGet("qualifications")]

		public IActionResult GetAllQualification()

		{
			return Ok(_service.GetAllQualification());

		}

		[HttpPut("qualification/{name}")]

		public IActionResult EditQualification(string name, [FromBody] QualificationDTO dto)

		{
			var result = _service.UpdateQualificationByName(name, dto);

			return Ok(result);

		}

		[HttpDelete("qualification/{name}")]

		public IActionResult DeleteQualification(string name)

		{
			var result = _service.DeleteQualificationByName(name);

			return Ok(result);

		}

		[HttpPost("program")]

		public IActionResult AddProgram([FromBody] ProgramDTO dto)

		{

			var result = _service.AddProgram(dto);

			return Ok(result);

		}

		[HttpGet("programs")]

		public IActionResult GetAllPrograms()

		{
			return Ok(_service.GetAllPrograms());

		}

		[HttpPut("program/{name}")]

		public IActionResult EditProgram(string name, [FromBody] ProgramDTO dto)

		{
			var result = _service.UpdateProgramByName(name, dto);

			return Ok(result);

		}

		[HttpDelete("program/{name}")]

		public IActionResult DeleteProgram(string name)

		{
			var result = _service.DeleteProgramByName(name);

			return Ok(result);

		}

		[HttpPost("academic-year")]

		public IActionResult AddAcademicYear([FromBody] AcademicYearDTO dto)

		{

			var result = _service.AddAcademicYear(dto);

			return Ok(result);

		}

		[HttpGet("academic-years")]

		public IActionResult GetAllAcademicYears()

		{
			return Ok(_service.GetAllAcademicYears());

		}

		[HttpPut("academic-year/{yearName}")]

		public IActionResult EditAcademicYear(string yearName, [FromBody] AcademicYearDTO dto)

		{
			var result = _service.UpdateAcademicYearByName(yearName, dto);

			return Ok(result);

		}

		[HttpDelete("academic-year/{yearName}")]

		public IActionResult DeleteAcademicYear(string yearName)

		{
			var result = _service.DeleteAcademicYearByName(yearName);

			return Ok(result);

		}
		[HttpPost("rules")]

		public IActionResult AddRule([FromBody] AcademicRuleDTO dto)

		{

			var result = _service.AddRule(dto);

			return Ok(result);

		}


		[HttpGet("rules")]

		public IActionResult GetAllRules()

		{

			return Ok(_service.GetAllRules());

		}

		// Note: GetAllRules is already in your code, adding Edit and Delete

		[HttpPut("rule/{ruleName}")]

		public IActionResult EditRule(string ruleName, [FromBody] AcademicRuleDTO dto)

		{

			var result = _service.UpdateRuleByName(ruleName, dto);

			return Ok(result);

		}


		[HttpDelete("rule/{ruleName}")]

		public IActionResult DeleteRule(string ruleName)

		{

			var result = _service.DeleteRuleByName(ruleName);

			return Ok(result);

		}

	}

}