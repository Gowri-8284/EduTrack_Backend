using EduTrackAcademics.Data;
using Microsoft.AspNetCore.Mvc;
using EduTrackAcademics.Services;

namespace EduTrackAcademics.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CoordinatorDashController : ControllerBase
	{
		//private readonly EduTrackAcademicsContext _context;
		private readonly ICoordinatorService _coordinatorService;

		public CoordinatorDashController(ICoordinatorService coordinatorService)
		{
			//_context = context;
			_coordinatorService = coordinatorService;
		}

		[HttpGet ("GetInstuctorDetails")]
		public ActionResult<List<string>> GetInstructorDetails()
		{
			var result = _coordinatorService.GetInstructorDetails();
			return Ok(result);
		}
		[HttpPost("AddInstuctorDetails")]
		public  ActionResult <string>  AddInstructorDetails(string name)
		{
			return Ok( _coordinatorService.AddInstructorDetails(name));
			
		}

	}
}
