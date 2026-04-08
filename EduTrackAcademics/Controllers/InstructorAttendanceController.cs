using EduTrackAcademics.Data;
using EduTrackAcademics.DTO;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EduTrackAcademics.Controllers

{

	[Route("api/instructorAttendance")]

	[ApiController]

	public class InstructorAttendanceController : ControllerBase
	{

		private readonly IInstructorAttendanceService _service;

		private readonly EduTrackAcademicsContext _context;


		public InstructorAttendanceController(EduTrackAcademicsContext context, IInstructorAttendanceService service)

		{

			_service = service;

			_context = context;

		}


		// ATTENDANCE
		[Authorize(Roles = "Instructor")]    
		[HttpPost("attendance")]

		public async Task<IActionResult> MarkAttendance([FromBody] AttendanceDTO dto)

		{

			var result = await _service.MarkAttendanceAsync(dto);

			return Ok(result);

		}


		[HttpPost("batchAttendance")]

		public async Task<IActionResult> MarkBatchAttendance([FromBody] MarkBatchAttendanceDTO dto)

		{

			var result = await _service.MarkBatchAttendanceAsync(dto);

			return Ok(result);

		}


		[HttpGet("batch/{batchId}/enrollments")]

		public async Task<IActionResult> GetEnrollmentsByBatch(string batchId)

		{

			var result = await _service.GetEnrollmentIdsByBatchAsync(batchId);

			return Ok(result);

		}

		[HttpGet("batches")]

		public async Task<IActionResult> GetBatches()

		{

			var result = await _service.GetAllBatchesAsync();

			return Ok(result);

		}


		[Authorize(Roles = "Admin, Coordinator, Instructor")]    
		[HttpGet("attendance")]

		public async Task<IActionResult> GetAllAttendance()

		{

			var result = await _service.GetAllAttendanceAsync();

			return Ok(result);

		}




		[Authorize(Roles = "Admin, Coordinator, Instructor")]    
		[HttpGet("attendance/date/{date}")]

		public async Task<IActionResult> GetAttendanceByDate(DateTime date)

		{

			var result = await _service.GetAttendanceByDateAsync(date);

			return Ok(result);

		}


		[Authorize(Roles = "Admin, Coordinator, Instructor")]    
		[HttpGet("attendance/batch/{batchId}")]

		public async Task<IActionResult> GetAttendanceByBatch(string batchId)

		{

			var result = await _service.GetAttendanceByBatchAsync(batchId);

			return Ok(result);

		}


		[Authorize(Roles = "Admin, Coordinator, Instructor, Student")]    
		[HttpGet("attendance/enrollment/{enrollmentId}")]

		public async Task<IActionResult> GetAttendanceByEnrollment(string enrollmentId)

		{

			var result = await _service.GetAttendanceByEnrollmentAsync(enrollmentId);

			return Ok(result);

		}


		[Authorize(Roles = "Coordinator, Instructor")]    
		[HttpPut("attendance/{attendanceId}")]

		public async Task<IActionResult> UpdateAttendance(string attendanceId, [FromBody] AttendanceDTO dto)

		{

			var result = await _service.UpdateAttendanceAsync(attendanceId, dto);

			return Ok(result);

		}



		[HttpPatch("attendance/{attendanceId}")]

		public async Task<IActionResult> PatchAttendanceStatus(string attendanceId, [FromBody] AttendanceStatusDTO dto)

		{


			Console.WriteLine($"EnrollmentID = {dto.EnrollmentID}");

			Console.WriteLine($"Status = {dto.Status}");


			if (string.IsNullOrWhiteSpace(attendanceId))

				return BadRequest("AttendanceId is required");


			if (string.IsNullOrWhiteSpace(dto.EnrollmentID))

				return BadRequest("EnrollmentID is required");


			if (string.IsNullOrWhiteSpace(dto.Status))

				return BadRequest("Status is required");


			var result = await _service

			.PatchAttendanceStatusAsync(attendanceId, dto.EnrollmentID, dto.Status);


			return Ok(result);

		}



		[Authorize(Roles = "Admin, Coordinator")]    
		[HttpDelete("attendance/{attendanceId}")]

		public async Task<IActionResult> DeleteAttendance(string attendanceId, [FromQuery] string reason)

		{

			var result = await _service.DeleteAttendanceAsync(attendanceId, reason);

			if (result.Contains("not found"))

				return NotFound(result);


			return Ok(result);

		}

	}

}