using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class AssessmentItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public double MaxPoints { get; set; }

        public Guid AssessmentId { get; set; }
        public Assessment? Assessment { get; set; }

        public Guid CLOId { get; set; }
        public CourseLearningOutcome? CLO { get; set; }

        [JsonIgnore]
        public ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
    }
}
