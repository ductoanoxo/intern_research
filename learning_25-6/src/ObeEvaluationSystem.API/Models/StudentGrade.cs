namespace ObeEvaluationSystem.API.Models
{
    public class StudentGrade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }

        public Guid AssessmentItemId { get; set; }
        public AssessmentItem? AssessmentItem { get; set; }

        public double PointsEarned { get; set; }
    }
}
