using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<CourseLearningOutcome> CourseLearningOutcomes { get; set; } = new List<CourseLearningOutcome>();
        
        [JsonIgnore]
        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    }
}
