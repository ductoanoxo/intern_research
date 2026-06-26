using System;

namespace SmartLearningAnalytics.API.Models
{
    public class StudentActivityLog
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public string ActivityType { get; set; } = string.Empty; // e.g. "VideoView", "DocRead", "QuizAttempt", "ForumPost"
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int TimeSpentSeconds { get; set; }
        public string Detail { get; set; } = string.Empty; // e.g. "Lesson 1: Intro to ASP.NET"
    }
}
