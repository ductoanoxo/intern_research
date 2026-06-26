using System;

namespace SmartLearningAnalytics.API.Models
{
    public class RubricCriterion
    {
        public Guid Id { get; set; }
        public Guid RubricId { get; set; }
        public AssessmentRubric? Rubric { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. "Code Quality", "Architecture"
        public string Description { get; set; } = string.Empty;
        public double Weight { get; set; } // Weight from 0.0 to 1.0
    }
}
