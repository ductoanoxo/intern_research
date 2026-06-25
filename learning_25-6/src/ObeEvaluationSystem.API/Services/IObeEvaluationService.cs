using ObeEvaluationSystem.API.Models;

namespace ObeEvaluationSystem.API.Services
{
    public interface IObeEvaluationService
    {
        Task SeedObeDataAsync();
        Task<StudentObeReportDto?> GetStudentObeReportAsync(Guid studentId);
    }

    public class StudentObeReportDto
    {
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<CloAttainmentDto> CloAttainments { get; set; } = new();
        public List<PloAttainmentDto> PloAttainments { get; set; } = new();
        public List<CompetencyProfileDto> CompetencyProfiles { get; set; } = new();
        public List<RemediationDto> RemediationRecommendations { get; set; } = new();
    }

    public class CloAttainmentDto
    {
        public Guid CloId { get; set; }
        public string CloCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public double MaxPoints { get; set; }
        public double PointsEarned { get; set; }
        public double Percentage { get; set; }
        public bool IsAttained { get; set; }
    }

    public class PloAttainmentDto
    {
        public Guid PloId { get; set; }
        public string PloCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Percentage { get; set; }
        public bool IsAttained { get; set; }
    }

    public class CompetencyProfileDto
    {
        public Guid CompetencyId { get; set; }
        public string CompetencyCode { get; set; } = string.Empty;
        public string CompetencyName { get; set; } = string.Empty;
        public double AttainmentScore { get; set; }
        public string Level { get; set; } = string.Empty; // Mastery, Developing, Beginning
    }

    public class RemediationDto
    {
        public string CloCode { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public double CurrentPercentage { get; set; }
        public string ActionRequired { get; set; } = string.Empty;
    }
}
