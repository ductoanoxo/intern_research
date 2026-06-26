using System;

namespace SmartLearningAnalytics.API.Models
{
    public class QuestionAttempt
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid QuestionItemId { get; set; }
        public QuestionItem? QuestionItem { get; set; }
        public bool IsCorrect { get; set; }
        public double PointsEarned { get; set; }
        public double MaxPoints { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }
}
