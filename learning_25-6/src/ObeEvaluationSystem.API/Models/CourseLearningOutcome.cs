using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class CourseLearningOutcome
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid CourseId { get; set; }
        public Course? Course { get; set; }

        public Guid PLOId { get; set; }
        public ProgramLearningOutcome? PLO { get; set; }

        // The contribution of this CLO to the PLO (e.g. 0.5 = 50%)
        public double WeightInPlo { get; set; } = 1.0;

        [JsonIgnore]
        public ICollection<AssessmentItem> AssessmentItems { get; set; } = new List<AssessmentItem>();
    }
}
