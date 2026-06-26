using System;
using System.Collections.Generic;

namespace SmartLearningAnalytics.API.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<StudentActivityLog> ActivityLogs { get; set; } = new List<StudentActivityLog>();
        public ICollection<StudentSkillState> SkillStates { get; set; } = new List<StudentSkillState>();
        public ICollection<RubricScore> RubricScores { get; set; } = new List<RubricScore>();
        public ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
    }
}
