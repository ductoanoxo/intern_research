using System;

namespace SmartLearningAnalytics.API.Models
{
    public class StudentSkillState
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid SkillId { get; set; }
        public Skill? Skill { get; set; }
        public double P_L { get; set; } // Current probability of mastery
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
