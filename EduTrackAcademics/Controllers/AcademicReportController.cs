using EduTrackAcademics.Data;
using EduTrackAcademics.Exceptions;
using EduTrackAcademics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AcademicReportController : ControllerBase
{
    private readonly IAcademicReportService _service;
    public AcademicReportController(IAcademicReportService service)
    {
        _service = service;
    }


    

      [Authorize(Roles = "Admin")]

    [HttpGet("get-batch-report")]

    public async Task<IActionResult> GetBatchReport(string batchId)

    {

        var result = await _service.GetBatchReportAsync(batchId);

        return Ok(result);
    }



   [Authorize(Roles = "Admin")]

    [HttpGet("full-report")]
    public async Task<IActionResult> GetFullReport()
    {
        var result = await _service.GetFullAcademicReportAsync();
        return Ok(result);
    }
    //  Generate + Save
    [Authorize(Roles = "Admin")]
    [HttpPost("generate-and-save")]
    public async Task<IActionResult> GenerateAndSaveReport()
    {
        var report = await _service.GetFullAcademicReportAsync();
        await _service.SaveOrUpdateAcademicReportAsync(report);
        return Ok(report);
    }
}

