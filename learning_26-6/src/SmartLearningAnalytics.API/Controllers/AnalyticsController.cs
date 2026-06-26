using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLearningAnalytics.API.Data;
using SmartLearningAnalytics.API.Models;
using SmartLearningAnalytics.API.Services;

namespace SmartLearningAnalytics.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IBktEngine _bktEngine;
        private readonly IIrtEngine _irtEngine;
        private readonly IEdmEngine _edmEngine;
        private readonly AnalyticsDbContext _context;

        public AnalyticsController(
            IAnalyticsService analyticsService,
            IBktEngine bktEngine,
            IIrtEngine irtEngine,
            IEdmEngine edmEngine,
            AnalyticsDbContext context)
        {
            _analyticsService = analyticsService;
            _bktEngine = bktEngine;
            _irtEngine = irtEngine;
            _edmEngine = edmEngine;
            _context = context;
        }

        /// <summary>
        /// Seed all default skills, question items with IRT properties, rubrics, students, activity logs and scores.
        /// </summary>
        [HttpPost("seed")]
        public async Task<IActionResult> SeedDatabase()
        {
            try
            {
                await _analyticsService.SeedDemoDataAsync();
                return Ok(new { Message = "Smart Learning Analytics database successfully seeded with 3 main student profiles and 7 cohort students." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// List all students.
        /// </summary>
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _analyticsService.GetStudentsAsync();
            return Ok(students);
        }

        /// <summary>
        /// Get a comprehensive report for a student combining BKT, IRT, Rubrics, and Decision Tree risk analysis.
        /// </summary>
        [HttpGet("students/{studentId:guid}/report")]
        public async Task<IActionResult> GetStudentReport(Guid studentId)
        {
            var report = await _analyticsService.GetStudentReportAsync(studentId);
            if (report == null)
            {
                return NotFound(new { Message = $"Student with ID {studentId} not found." });
            }
            return Ok(report);
        }

        /// <summary>
        /// Perform K-Means clustering on the student cohort.
        /// </summary>
        [HttpGet("cohort/clusters")]
        public async Task<IActionResult> GetCohortClusters()
        {
            var inputs = await _analyticsService.GetCohortClusterInputsAsync();
            var clusters = _edmEngine.ClusterStudents(inputs);
            return Ok(new
            {
                TotalStudentsClustered = inputs.Count,
                AlgorithmUsed = "K-Means (K=3)",
                FeaturesUsed = new[] { "TotalStudyTimeHours", "AverageQuizScorePercent" },
                Clusters = clusters
            });
        }

        /// <summary>
        /// Record a new question attempt and compute BKT probability change in real-time.
        /// </summary>
        [HttpPost("attempts")]
        public async Task<IActionResult> RecordAttempt([FromBody] RecordAttemptDto dto)
        {
            try
            {
                var result = await _analyticsService.RecordQuestionAttemptAsync(dto.StudentId, dto.QuestionItemId, dto.IsCorrect);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Sandbox simulation endpoint to see Bayesian Knowledge Tracing updates step-by-step.
        /// </summary>
        [HttpPost("simulation/bkt")]
        public IActionResult SimulateBkt([FromBody] BktSimRequest request)
        {
            var skill = new Skill
            {
                P_L0 = request.P_L0,
                P_T = request.P_T,
                P_G = request.P_G,
                P_S = request.P_S
            };

            var result = _bktEngine.RunSimulation(skill, request.Answers);
            return Ok(new
            {
                Message = "BKT Simulation run successfully.",
                Parameters = new { InitialMastery = request.P_L0, LearningTransition = request.P_T, Guess = request.P_G, Slip = request.P_S },
                SimulationResult = result
            });
        }

        /// <summary>
        /// Sandbox simulation endpoint to estimate IRT Theta ability.
        /// </summary>
        [HttpPost("simulation/irt")]
        public IActionResult SimulateIrt([FromBody] List<IrtResponse> responses)
        {
            double theta = _irtEngine.EstimateAbility(responses);
            return Ok(new
            {
                Message = "IRT Theta estimation run successfully.",
                TotalQuestions = responses.Count,
                EstimatedTheta = theta,
                Interpretation = theta >= 1.5 ? "Very High Ability" : theta >= 0.5 ? "High Ability" : theta >= -0.5 ? "Average Ability" : theta >= -1.5 ? "Low Ability" : "Very Low Ability"
            });
        }

        /// <summary>
        /// List all question items with their IRT settings.
        /// </summary>
        [HttpGet("questions")]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _context.QuestionItems.Include(q => q.Skill).ToListAsync();
            return Ok(questions);
        }
    }

    public class RecordAttemptDto
    {
        public Guid StudentId { get; set; }
        public Guid QuestionItemId { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class BktSimRequest
    {
        public double P_L0 { get; set; } = 0.15;
        public double P_T { get; set; } = 0.20;
        public double P_G { get; set; } = 0.20;
        public double P_S { get; set; } = 0.10;
        public List<bool> Answers { get; set; } = new List<bool>();
    }
}
