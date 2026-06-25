using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObeEvaluationSystem.API.Data;
using ObeEvaluationSystem.API.Services;

namespace ObeEvaluationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObeController : ControllerBase
    {
        private readonly IObeEvaluationService _obeService;
        private readonly ObeDbContext _context;

        public ObeController(IObeEvaluationService obeService, ObeDbContext context)
        {
            _obeService = obeService;
            _context = context;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                await _obeService.SeedObeDataAsync();
                return Ok(new { message = "OBE Education Data successfully seeded into SQLite database." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred during database seeding.", details = ex.Message });
            }
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students
                .Select(s => new { s.Id, s.StudentCode, s.FullName })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("students/{id:guid}/report")]
        public async Task<IActionResult> GetStudentObeReport(Guid id)
        {
            var report = await _obeService.GetStudentObeReportAsync(id);
            if (report == null)
            {
                return NotFound(new { error = $"Student with ID {id} not found." });
            }

            return Ok(report);
        }
    }
}
