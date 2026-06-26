using System;

namespace SmartLearningAnalytics.API.Models
{
    public class QuestionItem
    {
        public Guid Id { get; set; }
        public Guid SkillId { get; set; }
        public Skill? Skill { get; set; }
        public string Code { get; set; } = string.Empty; // e.g. "Q-SQL-JOIN-01"
        public string QuestionText { get; set; } = string.Empty;

        // IRT 2PL parameters
        public double Difficulty { get; set; } // b parameter (typically -3.0 to +3.0)
        public double Discrimination { get; set; } = 1.0; // a parameter (typically 0.5 to 2.5)
    }
}
