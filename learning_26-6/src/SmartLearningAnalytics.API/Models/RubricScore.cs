using System;

namespace SmartLearningAnalytics.API.Models
{
    public class RubricScore
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid CriterionId { get; set; }
        public RubricCriterion? Criterion { get; set; }
        public double Score { get; set; } // Scale 0-100 (percentage)
        public string GraderComments { get; set; } = string.Empty;
        public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
    }
}
