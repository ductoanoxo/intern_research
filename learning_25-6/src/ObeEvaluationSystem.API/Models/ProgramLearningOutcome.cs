using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class ProgramLearningOutcome
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid CompetencyId { get; set; }
        public Competency? Competency { get; set; }

        [JsonIgnore]
        public ICollection<CourseLearningOutcome> CourseLearningOutcomes { get; set; } = new List<CourseLearningOutcome>();
    }
}
