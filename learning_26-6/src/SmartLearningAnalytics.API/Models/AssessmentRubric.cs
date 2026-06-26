using System;
using System.Collections.Generic;

namespace SmartLearningAnalytics.API.Models
{
    public class AssessmentRubric
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty; // e.g. "DDD and CQRS Practical Assignment"
        public string CourseCode { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<RubricCriterion> Criteria { get; set; } = new List<RubricCriterion>();
    }
}
