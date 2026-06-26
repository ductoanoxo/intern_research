using System;
using System.Collections.Generic;
using SmartLearningAnalytics.API.Models;

namespace SmartLearningAnalytics.API.Services
{
    public interface IBktEngine
    {
        BktSimulationResult RunSimulation(Skill skill, List<bool> answers);
        double UpdateMastery(Skill skill, double currentPl, bool isCorrect);
    }

    public class BktSimulationResult
    {
        public List<BktStep> Steps { get; set; } = new List<BktStep>();
        public double FinalMasteryProbability => Steps.Count > 0 ? Steps[^1].UpdatedPl : 0;
        public bool IsMastered => FinalMasteryProbability >= 0.95;
    }

    public class BktStep
    {
        public int AttemptNumber { get; set; }
        public bool Answer { get; set; }
        public double PriorPl { get; set; }
        public double PosteriorPl { get; set; } // P(L|Action)
        public double UpdatedPl { get; set; }   // P(L_t) after transition P(T)
    }

    public class BktEngine : IBktEngine
    {
        public BktSimulationResult RunSimulation(Skill skill, List<bool> answers)
        {
            var result = new BktSimulationResult();
            double currentPl = skill.P_L0;

            for (int i = 0; i < answers.Count; i++)
            {
                bool ans = answers[i];
                double priorPl = currentPl;
                
                // 1. Calculate Posterior: P(L|Action)
                double posteriorPl;
                if (ans)
                {
                    // Correct: P(L | Correct)
                    double numerator = priorPl * (1 - skill.P_S);
                    double denominator = priorPl * (1 - skill.P_S) + (1 - priorPl) * skill.P_G;
                    posteriorPl = denominator > 0 ? numerator / denominator : 0;
                }
                else
                {
                    // Incorrect: P(L | Incorrect)
                    double numerator = priorPl * skill.P_S;
                    double denominator = priorPl * skill.P_S + (1 - priorPl) * (1 - skill.P_G);
                    posteriorPl = denominator > 0 ? numerator / denominator : 0;
                }

                // 2. Account for learning transition: P(L_t)
                currentPl = posteriorPl + (1 - posteriorPl) * skill.P_T;

                result.Steps.Add(new BktStep
                {
                    AttemptNumber = i + 1,
                    Answer = ans,
                    PriorPl = Math.Round(priorPl, 4),
                    PosteriorPl = Math.Round(posteriorPl, 4),
                    UpdatedPl = Math.Round(currentPl, 4)
                });
            }

            return result;
        }

        public double UpdateMastery(Skill skill, double currentPl, bool isCorrect)
        {
            double posteriorPl;
            if (isCorrect)
            {
                double numerator = currentPl * (1 - skill.P_S);
                double denominator = currentPl * (1 - skill.P_S) + (1 - currentPl) * skill.P_G;
                posteriorPl = denominator > 0 ? numerator / denominator : 0;
            }
            else
            {
                double numerator = currentPl * skill.P_S;
                double denominator = currentPl * skill.P_S + (1 - currentPl) * (1 - skill.P_G);
                posteriorPl = denominator > 0 ? numerator / denominator : 0;
            }

            double updatedPl = posteriorPl + (1 - posteriorPl) * skill.P_T;
            return Math.Clamp(Math.Round(updatedPl, 4), 0.0, 1.0);
        }
    }
}
