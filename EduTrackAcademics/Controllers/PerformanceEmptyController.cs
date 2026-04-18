using EduTrackAcademics.DTO;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduTrackAcademics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _performanceService;

        public PerformanceController(IPerformanceService performanceService)
        {
            _performanceService = performanceService;
        }

        // 1. Get total performance count
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetPerformanceCount()
        {
            var count = await _performanceService.GetPerformanceCountAsync();
            return Ok(count);
        }

        // 2. Get last updated details for a specific enrollment
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("last-updated/{enrollmentId}")]
        public async Task<ActionResult<LastUpdatedDTO>> GetLastUpdated(string enrollmentId)
        {
            try
            {
                var result = await _performanceService.GetLastUpdatedAsync(enrollmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 3. Get all batches assigned to a specific instructor
        [Authorize(Roles = "Instructor")]
        [HttpGet("instructor-batches/{instructorId}")]
        public async Task<ActionResult<List<InstructorBatchDTO>>> GetInstructorBatches(string instructorId)
        {
            try
            {
                var result = await _performanceService.GetInstructorBatchesAsync(instructorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 4. Get a full performance report for a batch
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("batch-report/{batchId}")]
        public async Task<ActionResult<GetBatchReportDTO>> GetBatchReport(string batchId)
        {
            var result = await _performanceService.GetBatchReportAsync(batchId);
            if (result == null)
            {
                return NotFound(new { message = $"Batch with ID {batchId} not found." });
            }
            return Ok(result);
        }

        // 5. Get completion rates for all batches under an instructor
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("instructor-completion/{instructorId}")]
        public async Task<ActionResult<List<BatchCompletionDTO>>> GetInstructorCompletionRate(string instructorId)
        {
            try
            {
                var result = await _performanceService.GetInstructorCompletionRate(instructorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 6. Get count and IDs of active (ongoing) batches
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("ongoing-batches/{instructorId}")]
        public async Task<ActionResult<object>> GetOngoingBatches(string instructorId)
        {
            try
            {
                var result = await _performanceService.GetOngoingBatches(instructorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 7. Get all batches in the system (Admin View)
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("all-batches")]
        public async Task<ActionResult<List<InstructorBatchDTO>>> GetAllBatches()
        {
            try
            {
                var result = await _performanceService.GetAllBatchesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 8. Get number of classes conducted per batch
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("class-counts/{instructorId}")]
        public async Task<ActionResult<List<BatchClassCountDTO>>> GetBatchClassCounts(string instructorId)
        {
            try
            {
                var result = await _performanceService.GetBatchClassCountsByInstructor(instructorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 9. Get start dates of all batches
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpGet("batch-start-dates")]
        public async Task<ActionResult<List<BatchStartDateDTO>>> GetBatchStartDates()
        {
            try
            {
                var result = await _performanceService.GetBatchStartDatesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 10. Trigger performance calculation
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpPost("generate-batch-performance/{batchId}")]
        public async Task<ActionResult<object>> GeneratePerformanceForBatch(string batchId)
        {
            try
            {
                await _performanceService.GeneratePerformanceForBatch(batchId);
                return Ok(new { message = $"Performance data generated for batch {batchId}" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 11. Update student enrollment details
        [Authorize(Roles = "Instructor,Admin,Coordinator")]
        [HttpPut("update-student")]
        public async Task<ActionResult<object>> UpdateStudent([FromBody] UpdateStudentDTO dto)
        {
            try
            {
                await _performanceService.UpdateStudentAsync(dto);
                return Ok(new { message = "Student updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 12. Delete a student enrollment
        [Authorize(Roles = "Instructor,admin,coordinator")]
        [HttpDelete("delete-student/{enrollmentId}")]
        public async Task<ActionResult<object>> DeleteStudent(string enrollmentId)
        {
            try
            {
                await _performanceService.DeleteStudentAsync(enrollmentId);
                return Ok(new { message = "Student deleted successfully" });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Instructor,admin,coordinator")]
        [HttpGet("course-dropout/{courseId}")] 
        public async Task<IActionResult> GetCourseDropoutRate(string courseId)
        {
            try
            {
                var result = await _performanceService.GetCourseDropoutRateAsync(courseId);

                // DIRECT 'result' badulu, oka Anonymous Object return cheyi
                // Deenivalla Frontend lo 'response.data.dropoutRate' ani access cheyochu
                return Ok(new { dropoutRate = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Internal server error vachina, కనీసం zero pampithe safe
                return StatusCode(500, new { dropoutRate = 0, message = ex.Message });
            }
        }

        [Authorize(Roles = "Instructor,admin,coordinator")]
        [HttpGet("student-assessment-stats/{studentId}")]

        public async Task<ActionResult> GetStudentAssessmentStats(string studentId)

        {

            var result = await _performanceService.GetStudentAssessmentStatsAsync(studentId);

            return Ok(result);

        }


    }
}