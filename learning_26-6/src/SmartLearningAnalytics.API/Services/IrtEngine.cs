using System;
using System.Collections.Generic;
using System.Linq;
using SmartLearningAnalytics.API.Models;

namespace SmartLearningAnalytics.API.Services
{
    public interface IIrtEngine
    {
        double CalculateProbability(double theta, double difficulty, double discrimination);
        double EstimateAbility(List<IrtResponse> responses);
    }

    public class IrtResponse
    {
        public double Difficulty { get; set; }     // b parameter
        public double Discrimination { get; set; } // a parameter
        public bool IsCorrect { get; set; }        // response (true = 1, false = 0)
    }

    public class IrtEngine : IIrtEngine
    {
        // Keep the standard 2PL probability formula
        public double CalculateProbability(double theta, double difficulty, double discrimination)
        {
            double exponent = -discrimination * (theta - difficulty);
            return 1.0 / (1.0 + Math.Exp(exponent));
        }

        // Simplify estimation to a heuristic scoring instead of complex MLE Grid Search optimization
        public double EstimateAbility(List<IrtResponse> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                return 0.0; // Default ability (average)
            }

            int correctCount = responses.Count(r => r.IsCorrect);
            int incorrectCount = responses.Count(r => !r.IsCorrect);

            // Simple heuristic calculation:
            // Correct answers push ability up, incorrect ones push it down.
            // We adjust the weight based on the question's difficulty.
            double baseScore = (correctCount - incorrectCount) * 0.5;

            // Average difficulty of questions answered correctly
            double avgDifficultyOfCorrect = correctCount > 0 
                ? responses.Where(r => r.IsCorrect).Average(r => r.Difficulty) 
                : 0.0;

            // Average difficulty of questions answered incorrectly
            double avgDifficultyOfIncorrect = incorrectCount > 0 
                ? responses.Where(r => !r.IsCorrect).Average(r => r.Difficulty) 
                : 0.0;

            // Final estimate: combine base score with difficulty adjustments
            double theta = baseScore + (avgDifficultyOfCorrect * 0.3) + (avgDifficultyOfIncorrect * 0.2);

            // Clamp between -2.5 (very weak) and +2.5 (very strong)
            return Math.Clamp(Math.Round(theta, 2), -2.5, 2.5);
        }
    }
}
