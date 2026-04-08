using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq.Expressions;

namespace EduTrackAcademics.Controllers

{

	[ApiController]


	[Route("api/[controller]")]


	public class PerformanceController : ControllerBase

	{

		private readonly IPerformanceService _service;


		public PerformanceController(IPerformanceService service)


		{

			_service = service;


		}



		[HttpGet("count")]


		public async Task<IActionResult> GetCount()


		{

			var result = await _service.GetPerformanceCountAsync();


			return Ok(result);


		}

		[HttpGet("average-score/{enrollmentId}")]


		public async Task<IActionResult> GetAverageScore(string enrollmentId)


		{

			var result = await _service.GetAverageScoreAsync(enrollmentId);


			return Ok(result);


		}

		[HttpGet("lastupdated/{enrollmentId}")]


		public async Task<IActionResult> GetLastUpdated(string enrollmentId)


		{

			var result = await _service.GetLastUpdatedAsync(enrollmentId);


			return Ok(result);


		}

		[HttpGet("instructor-batches/{instructorId}")]


		public async Task<IActionResult> GetInstructorBatches(string instructorId)


		{

			var result = await _service.GetInstructorBatchesAsync(instructorId);


			return Ok(result);


		}

		[HttpGet("batch/{batchId}")]


		public async Task<IActionResult> GetBatchReport(string batchId)


		{

			var result = await _service.GetBatchReportAsync(batchId);


			return Ok(result);


		}
		[HttpGet("completion-rate/{instructorId}")]


		public async Task<IActionResult> GetCompletionRate(string instructorId)


		{

			var result = await _service.GetInstructorCompletionRate(instructorId);


			return Ok(result);


		}

		[HttpGet("ongoing-batches/{instructorId}")]


		public async Task<IActionResult> GetOngoingBatches(string instructorId)


		{


			var data = await _service.GetOngoingBatches(instructorId);


			return Ok(data);


		}

		// ✅ GET ALL BATCHES (NEW API)


		[HttpGet("instructor-batches")]


		public async Task<IActionResult> GetAllBatches()


		{


			var result = await _service.GetAllBatchesAsync();


			return Ok(result);


		}





	}


}