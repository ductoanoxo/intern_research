using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class Competency
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<ProgramLearningOutcome> ProgramLearningOutcomes { get; set; } = new List<ProgramLearningOutcome>();
    }
}
