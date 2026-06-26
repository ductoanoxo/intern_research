using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartLearningAnalytics.API.Data;
using SmartLearningAnalytics.API.Models;

namespace SmartLearningAnalytics.API.Services
{
    public interface IAnalyticsService
    {
        Task SeedDemoDataAsync();
        Task<StudentReportDto?> GetStudentReportAsync(Guid studentId);
        Task<List<StudentClusterInput>> GetCohortClusterInputsAsync();
        Task<List<Student>> GetStudentsAsync();
        Task<QuestionAttemptResultDto> RecordQuestionAttemptAsync(Guid studentId, Guid questionItemId, bool isCorrect);
    }

    public class StudentReportDto
    {
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // 1. Learning Analytics (LA) Metrics
        public double TotalStudyTimeHours { get; set; }
        public double EngagementScore { get; set; } // Out of 100
        public double ConsistencyScore { get; set; } // 0.0 to 1.0 (based on frequency of activity days)
        public double AverageSubmissionLatencyHours { get; set; } // Delay in hours relative to deadline
        public List<ActivitySummaryDto> ActivityLogs { get; set; } = new List<ActivitySummaryDto>();

        // 2. Student Modeling (SM)
        public double EstimatedAbilityTheta { get; set; } // IRT Theta (-3.0 to +3.0)
        public List<SkillMasteryDto> SkillMasteries { get; set; } = new List<SkillMasteryDto>();

        // 3. Competency Assessment (CA)
        public List<CompetencyProfileDto> Competencies { get; set; } = new List<CompetencyProfileDto>();

        // 4. EDM Risk Prediction & Explainable Path
        public string PredictedRisk { get; set; } = string.Empty; // Safe, Borderline, AtRisk
        public List<string> RiskDecisionPath { get; set; } = new List<string>();
        public string RiskExplanation { get; set; } = string.Empty;

        // Remediation Recommendations
        public List<string> PedagogicalRecommendations { get; set; } = new List<string>();
    }

    public class ActivitySummaryDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double TotalTimeSpentMinutes { get; set; }
    }

    public class SkillMasteryDto
    {
        public string SkillCode { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public double MasteryProbability { get; set; } // BKT P(L)
        public string Status => MasteryProbability >= 0.95 ? "Mastered" : MasteryProbability >= 0.70 ? "Proficient" : "Learning";
    }

    public class CompetencyProfileDto
    {
        public string RubricTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public double OverallScorePercent { get; set; }
        public List<CriterionScoreDto> CriterionScores { get; set; } = new List<CriterionScoreDto>();
    }

    public class CriterionScoreDto
    {
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double ScorePercent { get; set; }
        public string Comments { get; set; } = string.Empty;
    }

    public class QuestionAttemptResultDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public string QuestionCode { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public double PriorMasteryProbability { get; set; }
        public double UpdatedMasteryProbability { get; set; }
        public bool MasteryAchieved => UpdatedMasteryProbability >= 0.95;
    }

    public class AnalyticsService : IAnalyticsService
    {
        private readonly AnalyticsDbContext _context;
        private readonly IBktEngine _bktEngine;
        private readonly IIrtEngine _irtEngine;
        private readonly IEdmEngine _edmEngine;

        public AnalyticsService(
            AnalyticsDbContext context,
            IBktEngine bktEngine,
            IIrtEngine irtEngine,
            IEdmEngine edmEngine)
        {
            _context = context;
            _bktEngine = bktEngine;
            _irtEngine = irtEngine;
            _edmEngine = edmEngine;
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<QuestionAttemptResultDto> RecordQuestionAttemptAsync(Guid studentId, Guid questionItemId, bool isCorrect)
        {
            var student = await _context.Students.FindAsync(studentId)
                ?? throw new KeyNotFoundException("Student not found");
            var question = await _context.QuestionItems.Include(q => q.Skill).FirstOrDefaultAsync(q => q.Id == questionItemId)
                ?? throw new KeyNotFoundException("Question item not found");
            var skill = question.Skill ?? throw new InvalidOperationException("Skill is null");

            // Record Attempt
            var attempt = new QuestionAttempt
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                QuestionItemId = questionItemId,
                IsCorrect = isCorrect,
                PointsEarned = isCorrect ? 10.0 : 0.0,
                MaxPoints = 10.0,
                AttemptedAt = DateTime.UtcNow
            };
            _context.QuestionAttempts.Add(attempt);

            // Fetch current BKT state or initialize
            var skillState = await _context.StudentSkillStates
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.SkillId == skill.Id);

            double priorPl = skill.P_L0;
            if (skillState == null)
            {
                double updatedPl = _bktEngine.UpdateMastery(skill, priorPl, isCorrect);
                skillState = new StudentSkillState
                {
                    Id = Guid.NewGuid(),
                    StudentId = studentId,
                    SkillId = skill.Id,
                    P_L = updatedPl,
                    LastUpdated = DateTime.UtcNow
                };
                _context.StudentSkillStates.Add(skillState);
            }
            else
            {
                priorPl = skillState.P_L;
                double updatedPl = _bktEngine.UpdateMastery(skill, priorPl, isCorrect);
                skillState.P_L = updatedPl;
                skillState.LastUpdated = DateTime.UtcNow;
            }

            // Log activity
            _context.StudentActivityLogs.Add(new StudentActivityLog
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                ActivityType = "QuizAttempt",
                TimeSpentSeconds = 120, // 2 minutes average
                Timestamp = DateTime.UtcNow,
                Detail = $"Answered {question.Code} ({(isCorrect ? "Correct" : "Incorrect")})"
            });

            await _context.SaveChangesAsync();

            return new QuestionAttemptResultDto
            {
                StudentName = student.FullName,
                SkillName = skill.Name,
                QuestionCode = question.Code,
                IsCorrect = isCorrect,
                PriorMasteryProbability = Math.Round(priorPl, 4),
                UpdatedMasteryProbability = Math.Round(skillState.P_L, 4)
            };
        }

        public async Task<StudentReportDto?> GetStudentReportAsync(Guid studentId)
        {
            var student = await _context.Students
                .Include(s => s.ActivityLogs)
                .Include(s => s.QuestionAttempts)
                    .ThenInclude(qa => qa.QuestionItem)
                        .ThenInclude(qi => qi.Skill)
                .Include(s => s.SkillStates)
                    .ThenInclude(ss => ss.Skill)
                .Include(s => s.RubricScores)
                    .ThenInclude(rs => rs.Criterion)
                        .ThenInclude(rc => rc.Rubric)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return null;

            // 1. Calculate Learning Analytics Metrics
            double totalSeconds = student.ActivityLogs.Sum(al => al.TimeSpentSeconds);
            double studyTimeHours = totalSeconds / 3600.0;

            // Engagement Score out of 100 based on counts
            double rawEng = 0;
            foreach (var log in student.ActivityLogs)
            {
                rawEng += log.ActivityType switch
                {
                    "VideoView" => 15,
                    "DocRead" => 10,
                    "ForumPost" => 25,
                    "QuizAttempt" => 10,
                    _ => 5
                };
            }
            double engagementScore = Math.Clamp((rawEng / 350.0) * 100.0, 10.0, 100.0);

            // Consistency Score: Ratio of unique active days over the last 14 days
            int activeDaysCount = student.ActivityLogs
                .Select(al => al.Timestamp.Date)
                .Distinct()
                .Count();
            double consistencyScore = Math.Clamp(activeDaysCount / 10.0, 0.1, 1.0);

            // Latency hours (simulated average: active students nộp sớm -24h, at risk nộp trễ +18h)
            // We use simulated log profiles for latency
            double avgLatency = student.StudentCode switch
            {
                "SV001" => -24.0, // Submits 24 hours early
                "SV002" => 2.0,   // Submits just in time / slightly late
                "SV003" => 18.5,  // Submits very late / past deadline
                _ => 0.0
            };

            var activitySummary = student.ActivityLogs
                .GroupBy(al => al.ActivityType)
                .Select(g => new ActivitySummaryDto
                {
                    ActivityType = g.Key,
                    Count = g.Count(),
                    TotalTimeSpentMinutes = Math.Round(g.Sum(al => al.TimeSpentSeconds) / 60.0, 1)
                }).ToList();

            // 2. Student Modeling (SM)
            // Calculate IRT Theta based on attempts
            var irtResponses = student.QuestionAttempts
                .Where(qa => qa.QuestionItem != null)
                .Select(qa => new IrtResponse
                {
                    Difficulty = qa.QuestionItem!.Difficulty,
                    Discrimination = qa.QuestionItem.Discrimination,
                    IsCorrect = qa.IsCorrect
                }).ToList();

            double estimatedTheta = _irtEngine.EstimateAbility(irtResponses);

            // BKT Skill masteries
            var skillMasteries = student.SkillStates.Select(ss => new SkillMasteryDto
            {
                SkillCode = ss.Skill!.Code,
                SkillName = ss.Skill.Name,
                MasteryProbability = ss.P_L
            }).ToList();

            // Handle skills that haven't been practiced yet: show initial P_L0
            var allSkills = await _context.Skills.ToListAsync();
            foreach (var skill in allSkills)
            {
                if (!skillMasteries.Any(sm => sm.SkillCode == skill.Code))
                {
                    skillMasteries.Add(new SkillMasteryDto
                    {
                        SkillCode = skill.Code,
                        SkillName = skill.Name,
                        MasteryProbability = skill.P_L0
                    });
                }
            }

            // 3. Competency Assessment (CA)
            var competencies = student.RubricScores
                .GroupBy(rs => rs.Criterion!.Rubric!)
                .Select(g => {
                    double totalWeightedScore = 0.0;
                    double totalWeight = 0.0;
                    var criterionScores = new List<CriterionScoreDto>();

                    foreach (var rs in g)
                    {
                        totalWeightedScore += rs.Score * rs.Criterion!.Weight;
                        totalWeight += rs.Criterion.Weight;
                        
                        criterionScores.Add(new CriterionScoreDto
                        {
                            Name = rs.Criterion.Name,
                            Weight = rs.Criterion.Weight,
                            ScorePercent = rs.Score,
                            Comments = rs.GraderComments
                        });
                    }

                    double overallScore = totalWeight > 0 ? totalWeightedScore / totalWeight : 0;

                    return new CompetencyProfileDto
                    {
                        RubricTitle = g.Key.Title,
                        CourseCode = g.Key.CourseCode,
                        OverallScorePercent = Math.Round(overallScore, 1),
                        CriterionScores = criterionScores
                    };
                }).ToList();

            // 4. EDM Risk Prediction
            // We use the student's metrics to feed the decision tree
            double averageQuizScore = student.QuestionAttempts.Count > 0
                ? (student.QuestionAttempts.Count(qa => qa.IsCorrect) / (double)student.QuestionAttempts.Count) * 100.0
                : 0.0;

            // Adjust quiz score if student has no attempts but has rubric scores
            if (student.QuestionAttempts.Count == 0 && competencies.Count > 0)
            {
                averageQuizScore = competencies.Average(c => c.OverallScorePercent);
            }

            var treeResult = _edmEngine.PredictStudentRisk(studyTimeHours, averageQuizScore, avgLatency);

            // Recommendations
            var recs = new List<string>();
            if (treeResult.RiskStatus == "AtRisk")
            {
                recs.Add("Critical: Schedule a 1-on-1 pedagogical counseling session to review basic syntax and environment setup.");
                recs.Add("Required: Review 'Module 1: Introduction to Web APIs' and complete the recovery lab assignment.");
            }
            if (treeResult.RiskStatus == "Borderline")
            {
                recs.Add("Advisory: Formulate a daily study plan. Aim for at least 30 minutes of study every second day to improve consistency.");
                recs.Add("Exercise: Practice more intermediate-level exercises of 'SQL Joins' to strengthen query design skills.");
            }
            if (estimatedTheta < -0.5)
            {
                recs.Add($"Subject-Matter Weakness: IRT Engine reports low capability state (Theta: {estimatedTheta:F2}). Practice simpler sub-tasks with low difficulty.");
            }
            
            // Add BKT specific recommendation
            foreach (var sm in skillMasteries.Where(sm => sm.MasteryProbability < 0.70))
            {
                recs.Add($"Skill reinforcement: Your mastery of '{sm.SkillName}' is currently low ({sm.MasteryProbability*100:F0}%). We recommend attempting at least 3 basic practice questions in this specific skill area.");
            }

            if (recs.Count == 0)
            {
                recs.Add("Maintain Momentum: You have mastered current requirements. Explore advanced topics like 'Clean Architecture Microservices'.");
            }

            return new StudentReportDto
            {
                StudentId = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                TotalStudyTimeHours = Math.Round(studyTimeHours, 2),
                EngagementScore = Math.Round(engagementScore, 1),
                ConsistencyScore = Math.Round(consistencyScore, 2),
                AverageSubmissionLatencyHours = avgLatency,
                ActivityLogs = activitySummary,
                EstimatedAbilityTheta = estimatedTheta,
                SkillMasteries = skillMasteries.OrderByDescending(sm => sm.MasteryProbability).ToList(),
                Competencies = competencies,
                PredictedRisk = treeResult.RiskStatus,
                RiskDecisionPath = treeResult.DecisionPath,
                RiskExplanation = treeResult.Explanation,
                PedagogicalRecommendations = recs
            };
        }

        public async Task<List<StudentClusterInput>> GetCohortClusterInputsAsync()
        {
            var students = await _context.Students
                .Include(s => s.ActivityLogs)
                .Include(s => s.QuestionAttempts)
                .Include(s => s.RubricScores)
                    .ThenInclude(rs => rs.Criterion)
                .ToListAsync();

            var inputs = new List<StudentClusterInput>();

            foreach (var student in students)
            {
                double totalSeconds = student.ActivityLogs.Sum(al => al.TimeSpentSeconds);
                double studyTimeHours = totalSeconds / 3600.0;

                // Calculate average academic score (average of quizzes and rubrics)
                double avgQuizScore = student.QuestionAttempts.Count > 0
                    ? (student.QuestionAttempts.Count(qa => qa.IsCorrect) / (double)student.QuestionAttempts.Count) * 100.0
                    : 0.0;

                double avgRubricScore = student.RubricScores.Count > 0
                    ? student.RubricScores.Average(rs => rs.Score)
                    : 0.0;

                double finalScore = 0.0;
                if (student.QuestionAttempts.Count > 0 && student.RubricScores.Count > 0)
                {
                    finalScore = (avgQuizScore + avgRubricScore) / 2.0;
                }
                else if (student.QuestionAttempts.Count > 0)
                {
                    finalScore = avgQuizScore;
                }
                else
                {
                    finalScore = avgRubricScore;
                }

                inputs.Add(new StudentClusterInput
                {
                    StudentId = student.Id,
                    StudentCode = student.StudentCode,
                    StudentName = student.FullName,
                    StudyTimeHours = Math.Round(studyTimeHours, 2),
                    AverageQuizScorePercent = Math.Round(finalScore, 1)
                });
            }

            return inputs;
        }

        public async Task SeedDemoDataAsync()
        {
            // Clear existing tables in correct order
            _context.QuestionAttempts.RemoveRange(_context.QuestionAttempts);
            _context.QuestionItems.RemoveRange(_context.QuestionItems);
            _context.RubricScores.RemoveRange(_context.RubricScores);
            _context.RubricCriteria.RemoveRange(_context.RubricCriteria);
            _context.AssessmentRubrics.RemoveRange(_context.AssessmentRubrics);
            _context.StudentActivityLogs.RemoveRange(_context.StudentActivityLogs);
            _context.StudentSkillStates.RemoveRange(_context.StudentSkillStates);
            _context.Skills.RemoveRange(_context.Skills);
            _context.Students.RemoveRange(_context.Students);
            await _context.SaveChangesAsync();

            // 1. Seed Skills (with BKT parameters)
            var skillSql = new Skill
            {
                Id = Guid.NewGuid(),
                Code = "SQL-JOIN",
                Name = "SQL Querying & Joins",
                Description = "Ability to write optimized SQL queries involving multiple tables using Inner, Left, and Full Outer joins.",
                P_L0 = 0.20, // Initial Mastery
                P_T = 0.25,  // Learning transition
                P_G = 0.15,  // Guessing
                P_S = 0.10   // Slipping
            };

            var skillCleanArch = new Skill
            {
                Id = Guid.NewGuid(),
                Code = "CLEAN-ARCH",
                Name = "Clean Architecture Layering",
                Description = "Ability to design software following SOLID principles and Clean Architecture (Core, Infrastructure, Web API layers).",
                P_L0 = 0.10,
                P_T = 0.18,
                P_G = 0.20,
                P_S = 0.12
            };

            var skillJwtAuth = new Skill
            {
                Id = Guid.NewGuid(),
                Code = "JWT-AUTH",
                Name = "JWT & Secure Authorization",
                Description = "Ability to configure JWT token issuance, verification middleware, and Claim-based authorization policies.",
                P_L0 = 0.15,
                P_T = 0.20,
                P_G = 0.10,
                P_S = 0.08
            };

            _context.Skills.AddRange(skillSql, skillCleanArch, skillJwtAuth);

            // 2. Seed Question Items (with IRT parameters: b = Difficulty, a = Discrimination)
            // SQL-JOIN questions (Easy, Medium, Hard)
            var qSql1 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillSql.Id,
                Code = "Q-SQL-01",
                QuestionText = "Select all fields from Students and join with Grades using INNER JOIN.",
                Difficulty = -1.2, // Easy
                Discrimination = 1.1
            };
            var qSql2 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillSql.Id,
                Code = "Q-SQL-02",
                QuestionText = "Write a query utilizing a LEFT JOIN to find students who haven't submitted any assignments.",
                Difficulty = 0.2, // Medium
                Discrimination = 1.3
            };
            var qSql3 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillSql.Id,
                Code = "Q-SQL-03",
                QuestionText = "Perform a three-way join between Students, RubricScores and Criteria, grouping by Student ID and calculating weighted averages.",
                Difficulty = 1.8, // Hard
                Discrimination = 1.6
            };

            // CLEAN-ARCH questions
            var qArch1 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillCleanArch.Id,
                Code = "Q-ARCH-01",
                QuestionText = "Explain the dependency rule in Clean Architecture: which layer can reference which?",
                Difficulty = -0.5, // Easy-Medium
                Discrimination = 1.0
            };
            var qArch2 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillCleanArch.Id,
                Code = "Q-ARCH-02",
                QuestionText = "Design a MediatR Pipeline Behavior to handle fluent validation exceptions globally.",
                Difficulty = 1.2, // Hard
                Discrimination = 1.5
            };

            // JWT-AUTH questions
            var qJwt1 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillJwtAuth.Id,
                Code = "Q-JWT-01",
                QuestionText = "Where should claims like role or email be inserted in a JWT token structure?",
                Difficulty = -0.8, // Easy
                Discrimination = 0.9
            };
            var qJwt2 = new QuestionItem
            {
                Id = Guid.NewGuid(),
                SkillId = skillJwtAuth.Id,
                Code = "Q-JWT-02",
                QuestionText = "Implement custom Authorization Policies in ASP.NET Core that validate both Role and Department claims.",
                Difficulty = 1.4, // Hard
                Discrimination = 1.4
            };

            _context.QuestionItems.AddRange(qSql1, qSql2, qSql3, qArch1, qArch2, qJwt1, qJwt2);

            // 3. Seed Assessment Rubrics (Competency Evaluation)
            var rubricBackend = new AssessmentRubric
            {
                Id = Guid.NewGuid(),
                Title = "E-Commerce Backend API Implementation",
                CourseCode = "CS-202"
            };
            _context.AssessmentRubrics.Add(rubricBackend);

            var cCodeQuality = new RubricCriterion
            {
                Id = Guid.NewGuid(),
                RubricId = rubricBackend.Id,
                Name = "Code Quality & Clean Coding Standards",
                Description = "Measures naming consistency, small methods, proper file structuring, and separation of concerns.",
                Weight = 0.40 // 40%
            };
            var cArchDesign = new RubricCriterion
            {
                Id = Guid.NewGuid(),
                RubricId = rubricBackend.Id,
                Name = "Architectural Layers Alignment",
                Description = "Evaluates whether dependencies flow inward to the Domain layer, with no Presentation dependencies in Core.",
                Weight = 0.30 // 30%
            };
            var cErrorHandling = new RubricCriterion
            {
                Id = Guid.NewGuid(),
                RubricId = rubricBackend.Id,
                Name = "Robust Error Handling & Logging",
                Description = "Assess implementation of global exception middleware and logs tracking structured events.",
                Weight = 0.30 // 30%
            };
            _context.RubricCriteria.AddRange(cCodeQuality, cArchDesign, cErrorHandling);

            // 4. Seed 3 Student Profiles representing different clusters:
            // - SV001: Nguyen Van A (Active High-Achiever: High Study Time, High Scores, Active BKT, High IRT Ability)
            // - SV002: Tran Thi B (Inconsistent/Moderate: Moderate study time, passing scores, average BKT/IRT)
            // - SV003: Le Van C (At-Risk/Passive: Very low study time, low/failing scores, poor BKT/IRT)

            var stuA = new Student { Id = Guid.NewGuid(), StudentCode = "SV001", FullName = "Nguyen Van A" };
            var stuB = new Student { Id = Guid.NewGuid(), StudentCode = "SV002", FullName = "Tran Thi B" };
            var stuC = new Student { Id = Guid.NewGuid(), StudentCode = "SV003", FullName = "Le Van C" };
            _context.Students.AddRange(stuA, stuB, stuC);

            // Seed additional students to have a bigger cohort for clustering (e.g. 10 students total)
            var additionalStudents = new List<Student>
            {
                new Student { Id = Guid.NewGuid(), StudentCode = "SV004", FullName = "Pham Quoc D" },
                new Student { Id = Guid.NewGuid(), StudentCode = "SV005", FullName = "Hoang Thu E" },
                new Student { Id = Guid.NewGuid(), StudentCode = "SV006", FullName = "Vu Minh F" },
                new Student { Id = Guid.NewGuid(), StudentCode = "SV007", FullName = "Do Thanh G" },
                new Student { Id = Guid.NewGuid(), StudentCode = "SV008", FullName = "Ngo Quang H" },
                new Student { Id = Guid.NewGuid(), StudentCode = "SV009", FullName = "Bui Mai I" },
                new Student { Id = Guid.NewGuid(), StudentCode = "SV010", FullName = "Dang Duc J" }
            };
            _context.Students.AddRange(additionalStudents);

            // Save to establish student identities for child records
            await _context.SaveChangesAsync();

            // 5. Seed Activity Logs
            var logs = new List<StudentActivityLog>();
            var now = DateTime.UtcNow;

            // Student A logs: Active, steady studying, early mornings and late nights
            for (int day = 0; day < 10; day++)
            {
                logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuA.Id, ActivityType = "DocRead", TimeSpentSeconds = 1800, Timestamp = now.AddDays(-day).AddHours(2) });
                logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuA.Id, ActivityType = "VideoView", TimeSpentSeconds = 2400, Timestamp = now.AddDays(-day).AddHours(3) });
                if (day % 3 == 0)
                    logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuA.Id, ActivityType = "ForumPost", TimeSpentSeconds = 600, Timestamp = now.AddDays(-day).AddHours(4) });
            }

            // Student B logs: Inconsistent, study sessions clustered right before deadlines (e.g. active on only 4 days out of 10)
            logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuB.Id, ActivityType = "DocRead", TimeSpentSeconds = 3600, Timestamp = now.AddDays(-1) });
            logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuB.Id, ActivityType = "VideoView", TimeSpentSeconds = 5400, Timestamp = now.AddDays(-1).AddHours(1) });
            logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuB.Id, ActivityType = "DocRead", TimeSpentSeconds = 3000, Timestamp = now.AddDays(-4) });
            logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuB.Id, ActivityType = "VideoView", TimeSpentSeconds = 1800, Timestamp = now.AddDays(-4).AddHours(2) });

            // Student C logs: Passive, barely logged in
            logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = stuC.Id, ActivityType = "DocRead", TimeSpentSeconds = 900, Timestamp = now.AddDays(-8) });

            // Add logs for additional students
            // SV004-SV006: High Achievers
            foreach (var s in additionalStudents.Take(3))
            {
                for (int day = 0; day < 8; day++)
                {
                    logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = s.Id, ActivityType = "DocRead", TimeSpentSeconds = 1500, Timestamp = now.AddDays(-day) });
                    logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = s.Id, ActivityType = "VideoView", TimeSpentSeconds = 2000, Timestamp = now.AddDays(-day).AddHours(1) });
                }
            }
            // SV007-SV008: Moderate
            foreach (var s in additionalStudents.Skip(3).Take(2))
            {
                logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = s.Id, ActivityType = "DocRead", TimeSpentSeconds = 2000, Timestamp = now.AddDays(-2) });
                logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = s.Id, ActivityType = "VideoView", TimeSpentSeconds = 3000, Timestamp = now.AddDays(-5) });
            }
            // SV009-SV010: At-Risk
            foreach (var s in additionalStudents.Skip(5))
            {
                logs.Add(new StudentActivityLog { Id = Guid.NewGuid(), StudentId = s.Id, ActivityType = "VideoView", TimeSpentSeconds = 1200, Timestamp = now.AddDays(-6) });
            }

            _context.StudentActivityLogs.AddRange(logs);

            // 6. Seed Rubric Scores (Competency)
            // Student A: High scores
            _context.RubricScores.AddRange(
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuA.Id, CriterionId = cCodeQuality.Id, Score = 90.0, GraderComments = "Excellent code quality. Strictly follows clean code rules." },
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuA.Id, CriterionId = cArchDesign.Id, Score = 95.0, GraderComments = "Flawless dependency flows. Layers are completely decoupled." },
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuA.Id, CriterionId = cErrorHandling.Id, Score = 85.0, GraderComments = "Good global exception handling. Logging could contain more context." }
            );

            // Student B: Moderate scores
            _context.RubricScores.AddRange(
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuB.Id, CriterionId = cCodeQuality.Id, Score = 70.0, GraderComments = "Code is clean but some methods are too long. Variable naming can be improved." },
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuB.Id, CriterionId = cArchDesign.Id, Score = 75.0, GraderComments = "Layer isolation is fine, but noticed direct DB referencing in Web API controller." },
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuB.Id, CriterionId = cErrorHandling.Id, Score = 60.0, GraderComments = "Exceptions are caught but swallowed. Needs structured error logs." }
            );

            // Student C: Low/failing scores
            _context.RubricScores.AddRange(
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuC.Id, CriterionId = cCodeQuality.Id, Score = 45.0, GraderComments = "Messy indentation, generic variable names (e.g. a, b, c)." },
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuC.Id, CriterionId = cArchDesign.Id, Score = 40.0, GraderComments = "No clear division of layers. Domain layers are mixed with Controllers." },
                new RubricScore { Id = Guid.NewGuid(), StudentId = stuC.Id, CriterionId = cErrorHandling.Id, Score = 35.0, GraderComments = "Application crashes frequently on boundary inputs. No exception handling." }
            );

            // Seed rubric scores for additional students
            foreach (var s in additionalStudents.Take(3)) // High achievers
            {
                _context.RubricScores.AddRange(
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cCodeQuality.Id, Score = 88.0, GraderComments = "Great coding standard." },
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cArchDesign.Id, Score = 85.0, GraderComments = "Solid layers implementation." },
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cErrorHandling.Id, Score = 90.0, GraderComments = "Exceptional error management." }
                );
            }
            foreach (var s in additionalStudents.Skip(3).Take(2)) // Moderate
            {
                _context.RubricScores.AddRange(
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cCodeQuality.Id, Score = 68.0, GraderComments = "Fair quality." },
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cArchDesign.Id, Score = 70.0, GraderComments = "Basic layering architecture." },
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cErrorHandling.Id, Score = 65.0, GraderComments = "Standard try-catch blocks." }
                );
            }
            foreach (var s in additionalStudents.Skip(5)) // At-risk
            {
                _context.RubricScores.AddRange(
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cCodeQuality.Id, Score = 40.0, GraderComments = "Poor standards." },
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cArchDesign.Id, Score = 45.0, GraderComments = "Layers are heavily coupled." },
                    new RubricScore { Id = Guid.NewGuid(), StudentId = s.Id, CriterionId = cErrorHandling.Id, Score = 30.0, GraderComments = "No exception protection." }
                );
            }

            // 7. Seed Question Attempts (Student Modeling)
            // SQL-JOIN: Student A answers all correctly (Easy, Medium, Hard)
            // Student B: Correct, Correct, Incorrect (stumbled on Hard)
            // Student C: Incorrect, Incorrect, Correct (guessed the Easy one or stumbled)
            var attempts = new List<QuestionAttempt>
            {
                // Student A - SQL JOIN
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qSql1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qSql2.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qSql3.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                // Student A - CLEAN ARCH
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qArch1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qArch2.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                // Student A - JWT AUTH
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qJwt1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuA.Id, QuestionItemId = qJwt2.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },

                // Student B - SQL JOIN
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuB.Id, QuestionItemId = qSql1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuB.Id, QuestionItemId = qSql2.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuB.Id, QuestionItemId = qSql3.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 },
                // Student B - CLEAN ARCH
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuB.Id, QuestionItemId = qArch1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuB.Id, QuestionItemId = qArch2.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 },

                // Student C - SQL JOIN
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuC.Id, QuestionItemId = qSql1.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuC.Id, QuestionItemId = qSql2.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 },
                new QuestionAttempt { Id = Guid.NewGuid(), StudentId = stuC.Id, QuestionItemId = qSql3.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 } // Guessed/copied
            };

            // Seed attempts for additional students
            foreach (var s in additionalStudents.Take(3)) // High achievers: mostly correct
            {
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qSql1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 });
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qSql2.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 });
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qArch1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 });
            }
            foreach (var s in additionalStudents.Skip(3).Take(2)) // Moderate: 50/50
            {
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qSql1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 });
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qSql2.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 });
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qArch1.Id, IsCorrect = true, PointsEarned = 10, MaxPoints = 10 });
            }
            foreach (var s in additionalStudents.Skip(5)) // At-risk: incorrect
            {
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qSql1.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 });
                attempts.Add(new QuestionAttempt { Id = Guid.NewGuid(), StudentId = s.Id, QuestionItemId = qSql2.Id, IsCorrect = false, PointsEarned = 0, MaxPoints = 10 });
            }

            _context.QuestionAttempts.AddRange(attempts);
            await _context.SaveChangesAsync();

            // 8. Compute and Seed Initial BKT Skill States based on attempts
            // We simulate BKT progression step by step using BktEngine
            var seededStudents = new List<Student> { stuA, stuB, stuC };
            seededStudents.AddRange(additionalStudents);

            foreach (var student in seededStudents)
            {
                var stuAttempts = attempts.Where(a => a.StudentId == student.Id).ToList();
                var groupedBySkill = stuAttempts.GroupBy(a => a.QuestionItem!.SkillId);

                foreach (var group in groupedBySkill)
                {
                    var skill = await _context.Skills.FindAsync(group.Key);
                    if (skill != null)
                    {
                        var listAnswers = group.OrderBy(a => a.AttemptedAt).Select(a => a.IsCorrect).ToList();
                        var bktResult = _bktEngine.RunSimulation(skill, listAnswers);

                        _context.StudentSkillStates.Add(new StudentSkillState
                        {
                            Id = Guid.NewGuid(),
                            StudentId = student.Id,
                            SkillId = skill.Id,
                            P_L = bktResult.FinalMasteryProbability,
                            LastUpdated = DateTime.UtcNow
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
