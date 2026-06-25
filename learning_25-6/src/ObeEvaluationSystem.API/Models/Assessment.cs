using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class Assessment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public Guid CourseId { get; set; }
        public Course? Course { get; set; }

        [JsonIgnore]
        public ICollection<AssessmentItem> AssessmentItems { get; set; } = new List<AssessmentItem>();
    }
}
