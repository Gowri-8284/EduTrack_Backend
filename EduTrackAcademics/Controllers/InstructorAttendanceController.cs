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

		[Authorize(Roles = "Instructor")]
		[HttpPost("batchAttendance")]
		public async Task<IActionResult> MarkBatchAttendance([FromBody] MarkBatchAttendanceDTO dto)
		{
			var result = await _service.MarkBatchAttendanceAsync(dto);
			if (result.Contains("inactive") || result.Contains("Invalid") || result.Contains("already"))
				return BadRequest(result);
			return Ok(result);
		}

		[Authorize(Roles = "Coordinator, Instructor")]
		[HttpGet("attendanceSummary")]
		public async Task<IActionResult> GetAttendanceSummary()
		{
			var result = await _service.GetAttendanceSummaryAsync();
			return Ok(result);
		}

		//  GET DETAILS (Batch + Date)
		[Authorize(Roles = "Instructor")]
		[HttpGet("attendance/{batchId}/{date}")]
		public async Task<IActionResult> GetAttendanceDetails(string batchId, DateTime date)
		{
			var result = await _service.GetAttendanceDetailsAsync(batchId, date);
			return Ok(result);
		}


		[HttpGet("batch/{batchId}/enrollments")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> GetEnrollmentsByBatch(string batchId)
		{
			var result = await _service.GetEnrollmentIdsByBatchAsync(batchId);
			return Ok(result);
		}
		[HttpGet("batches")]
		[Authorize(Roles = "Coordinator, Instructor")]
		public async Task<IActionResult> GetBatches()
		{
			var result = await _service.GetAllBatchesAsync();
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("attendance")]
		public async Task<IActionResult> GetAllAttendance()
		{
			var result = await _service.GetAllAttendanceAsync();
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]

		[HttpGet("attendance/date/{date}")]
		public async Task<IActionResult> GetAttendanceByDate(DateTime date)
		{
			var result = await _service.GetAttendanceByDateAsync(date);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("attendance/batch/{batchId}")]
		public async Task<IActionResult> GetAttendanceByBatch(string batchId)
		{
			var result = await _service.GetAttendanceByBatchAsync(batchId);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("attendance/enrollment/{enrollmentId}")]
		public async Task<IActionResult> GetAttendanceByEnrollment(string enrollmentId)
		{
			var result = await _service.GetAttendanceByEnrollmentAsync(enrollmentId);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpPut("attendance/{attendanceId}")]
		public async Task<IActionResult> UpdateAttendance(string attendanceId, [FromBody] AttendanceDTO dto)
		{
			var result = await _service.UpdateAttendanceAsync(attendanceId, dto);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpPatch("status/{attendanceId}")]
		public async Task<IActionResult> PatchAttendanceStatus(string attendanceId, [FromBody] AttendanceStatusDTO dto)
		{
			if (string.IsNullOrWhiteSpace(attendanceId))
				return BadRequest("AttendanceId is required");

			if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
				return BadRequest("Status is required");

			var result = await _service.PatchAttendanceStatusAsync(attendanceId, dto.Status);

			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpDelete("attendance/{attendanceId}")]
		public async Task<IActionResult> DeleteAttendance(string attendanceId, [FromQuery] string reason)
		{
			var result = await _service.DeleteAttendanceAsync(attendanceId, reason);
			if (result.Contains("not found"))
				return NotFound(result);

			return Ok(result);
		}

		// DELETE BY BATCH
		[Authorize(Roles = "Instructor")]
		[HttpDelete("attendance/batch/{batchId}/{date}")]
		public async Task<IActionResult> DeleteByBatch(string batchId, DateTime date, string reason)
		{
			var result = await _service.DeleteAttendanceByBatchAsync(batchId, date, reason);
			return Ok(result);
		}

		// DELETE BY COURSE
		[Authorize(Roles = "Instructor")]
		[HttpDelete("attendance/course/{courseId}/{date}")]
		public async Task<IActionResult> DeleteByCourse(string courseId, DateTime date, string reason)
		{
			var result = await _service.DeleteAttendanceByCourseAsync(courseId, date, reason);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpPut("attendance/restore/{batchId}/{date}")]
		public async Task<IActionResult> RestoreAttendance(string batchId, DateTime date)
		{
			var result = await _service.RestoreAttendanceByBatchAsync(batchId, date);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("attendance/deleted")]
		public async Task<IActionResult> GetDeletedAttendance()
		{
			var result = await _service.GetDeletedAttendanceSummaryAsync();
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("attendance/students/{batchId}")]
		public async Task<IActionResult> GetStudentsForAttendance(string batchId)
		{
			try
			{
				var result = await _service.GetStudentsForAttendanceAsync(batchId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize(Roles = "Coordinator, Instructor")]
		[HttpGet("instructor/{instructorId}/courses")]
		public IActionResult GetCoursesWithBatch(string instructorId)
		{
			var result = _service.GetAllCoursesByInstructorId(instructorId);
			return Ok(result);
		}

		[Authorize(Roles = "Instructor")]
		[HttpGet("get-batch")]
		public IActionResult GetBatchByCourse(string? courseId, string courseName)
		{
			var result = _service.GetBatchByCourse(courseId, courseName);

			if (result == null)
				return NotFound("Course or Batch not found");

			return Ok(result);
		}
	}
}
