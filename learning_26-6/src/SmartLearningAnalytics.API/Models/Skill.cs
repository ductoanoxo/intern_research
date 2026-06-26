using System;

namespace SmartLearningAnalytics.API.Models
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty; // e.g., "SQL-JOIN", "JWT-AUTH"
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // BKT parameters
        public double P_L0 { get; set; } = 0.15; // Initial probability of mastery
        public double P_T { get; set; } = 0.20;  // Probability of learning on next attempt
        public double P_G { get; set; } = 0.20;  // Guess probability (answering correctly without mastery)
        public double P_S { get; set; } = 0.10;  // Slip probability (answering incorrectly with mastery)
    }
}
