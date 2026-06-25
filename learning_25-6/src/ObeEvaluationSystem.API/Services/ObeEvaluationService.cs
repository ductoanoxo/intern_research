using Microsoft.EntityFrameworkCore;
using ObeEvaluationSystem.API.Data;
using ObeEvaluationSystem.API.Models;

namespace ObeEvaluationSystem.API.Services
{
    public class ObeEvaluationService : IObeEvaluationService
    {
        private readonly ObeDbContext _context;

        public ObeEvaluationService(ObeDbContext context)
        {
            _context = context;
        }

        public async Task SeedObeDataAsync()
        {
            // Clear existing data to allow clean re-seeding
            _context.StudentGrades.RemoveRange(_context.StudentGrades);
            _context.Students.RemoveRange(_context.Students);
            _context.AssessmentItems.RemoveRange(_context.AssessmentItems);
            _context.Assessments.RemoveRange(_context.Assessments);
            _context.CourseLearningOutcomes.RemoveRange(_context.CourseLearningOutcomes);
            _context.Courses.RemoveRange(_context.Courses);
            _context.ProgramLearningOutcomes.RemoveRange(_context.ProgramLearningOutcomes);
            _context.Competencies.RemoveRange(_context.Competencies);
            await _context.SaveChangesAsync();

            // 1. Seed Competencies (KSA Framework)
            var comp1 = new Competency
            {
                Code = "COMP-1",
                Name = "Web Application Design and Implementation",
                Description = "Ability to design, build, and deploy secure and scalable web APIs using modern backend technologies."
            };
            var comp2 = new Competency
            {
                Code = "COMP-2",
                Name = "Data Management and Database Engineering",
                Description = "Ability to design database schemas, manage transactional consistency, and write high-performance queries."
            };
            await _context.Competencies.AddRangeAsync(comp1, comp2);

            // 2. Seed PLOs
            var plo1 = new ProgramLearningOutcome
            {
                Code = "PLO-1",
                Description = "Design and implement responsive, secure, and performant web services using modern backend stacks.",
                Competency = comp1
            };
            var plo2 = new ProgramLearningOutcome
            {
                Code = "PLO-2",
                Description = "Design, implement, and maintain secure relational database schemas with transactional integrity.",
                Competency = comp2
            };
            await _context.ProgramLearningOutcomes.AddRangeAsync(plo1, plo2);

            // 3. Seed Courses
            var course1 = new Course { Code = "CS-201", Name = "ASP.NET Core Backend Development" };
            var course2 = new Course { Code = "CS-202", Name = "Relational Databases and EF Core" };
            await _context.Courses.AddRangeAsync(course1, course2);

            // 4. Seed CLOs
            var clo11 = new CourseLearningOutcome
            {
                Code = "CLO-1.1",
                Description = "Implement secure JWT-based authentication and claim-based authorization.",
                Course = course1,
                PLO = plo1,
                WeightInPlo = 0.6 // 60% contribution to PLO-1
            };
            var clo12 = new CourseLearningOutcome
            {
                Code = "CLO-1.2",
                Description = "Design web APIs adhering to Clean Architecture principles.",
                Course = course1,
                PLO = plo1,
                WeightInPlo = 0.4 // 40% contribution to PLO-1
            };
            var clo21 = new CourseLearningOutcome
            {
                Code = "CLO-2.1",
                Description = "Construct relational schemas and execute transactional database queries.",
                Course = course2,
                PLO = plo2,
                WeightInPlo = 1.0 // 100% contribution to PLO-2
            };
            await _context.CourseLearningOutcomes.AddRangeAsync(clo11, clo12, clo21);

            // 5. Seed Assessments & AssessmentItems
            var midtermCs201 = new Assessment { Name = "Midterm Exam", Course = course1 };
            var finalProjectCs201 = new Assessment { Name = "Final Project", Course = course1 };
            var assignmentCs202 = new Assessment { Name = "Database Assignment", Course = course2 };
            await _context.Assessments.AddRangeAsync(midtermCs201, finalProjectCs201, assignmentCs202);

            // Items for Midterm (CS-201)
            var item1 = new AssessmentItem { Name = "Question 1: JWT Signature verification flow", MaxPoints = 10, Assessment = midtermCs201, CLO = clo11 };
            var item2 = new AssessmentItem { Name = "Question 2: Explain dependency inversion in Clean Architecture", MaxPoints = 10, Assessment = midtermCs201, CLO = clo12 };

            // Items for Final Project (CS-201)
            var item3 = new AssessmentItem { Name = "Part 1: Implement custom JWT middleware", MaxPoints = 20, Assessment = finalProjectCs201, CLO = clo11 };
            var item4 = new AssessmentItem { Name = "Part 2: Split domain entity and database configuration", MaxPoints = 20, Assessment = finalProjectCs201, CLO = clo12 };

            // Items for Database Assignment (CS-202)
            var item5 = new AssessmentItem { Name = "Task 1: Configure SQLite entity mappings via Fluent API", MaxPoints = 15, Assessment = assignmentCs202, CLO = clo21 };
            var item6 = new AssessmentItem { Name = "Task 2: Write Linq query with Deferred Execution", MaxPoints = 15, Assessment = assignmentCs202, CLO = clo21 };

            await _context.AssessmentItems.AddRangeAsync(item1, item2, item3, item4, item5, item6);

            // 6. Seed Students
            var studentA = new Student { StudentCode = "SV001", FullName = "Nguyen Van A" };
            var studentB = new Student { StudentCode = "SV002", FullName = "Tran Thi B" }; // Struggling with JWT Auth
            await _context.Students.AddRangeAsync(studentA, studentB);

            // 7. Seed StudentGrades
            // Student A (SV001) - High scores
            var gradesA = new List<StudentGrade>
            {
                new() { Student = studentA, AssessmentItem = item1, PointsEarned = 9.0 },  // CLO-1.1
                new() { Student = studentA, AssessmentItem = item2, PointsEarned = 8.5 },  // CLO-1.2
                new() { Student = studentA, AssessmentItem = item3, PointsEarned = 18.0 }, // CLO-1.1
                new() { Student = studentA, AssessmentItem = item4, PointsEarned = 17.5 }, // CLO-1.2
                new() { Student = studentA, AssessmentItem = item5, PointsEarned = 14.0 }, // CLO-2.1
                new() { Student = studentA, AssessmentItem = item6, PointsEarned = 13.5 }  // CLO-2.1
            };

            // Student B (SV002) - Failing JWT Auth (CLO-1.1), passing others
            var gradesB = new List<StudentGrade>
            {
                new() { Student = studentB, AssessmentItem = item1, PointsEarned = 4.0 },  // CLO-1.1 (failing)
                new() { Student = studentB, AssessmentItem = item2, PointsEarned = 8.0 },  // CLO-1.2 (passing)
                new() { Student = studentB, AssessmentItem = item3, PointsEarned = 8.0 },  // CLO-1.1 (failing)
                new() { Student = studentB, AssessmentItem = item4, PointsEarned = 16.0 }, // CLO-1.2 (passing)
                new() { Student = studentB, AssessmentItem = item5, PointsEarned = 12.0 }, // CLO-2.1 (passing)
                new() { Student = studentB, AssessmentItem = item6, PointsEarned = 11.5 }  // CLO-2.1 (passing)
            };

            await _context.StudentGrades.AddRangeAsync(gradesA);
            await _context.StudentGrades.AddRangeAsync(gradesB);

            await _context.SaveChangesAsync();
        }

        public async Task<StudentObeReportDto?> GetStudentObeReportAsync(Guid studentId)
        {
            var student = await _context.Students
                .Include(s => s.StudentGrades)
                    .ThenInclude(sg => sg.AssessmentItem)
                        .ThenInclude(ai => ai.CLO)
                            .ThenInclude(clo => clo.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return null;

            // Load PLOs and Competencies for traversal
            var plos = await _context.ProgramLearningOutcomes
                .Include(p => p.Competency)
                .ToListAsync();

            var clos = await _context.CourseLearningOutcomes.ToListAsync();

            var report = new StudentObeReportDto
            {
                StudentId = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName
            };

            // 1. Calculate CLO Attainments
            // For each CLO, sum the student's earned points and max points.
            var cloGroupedGrades = student.StudentGrades
                .Where(sg => sg.AssessmentItem?.CLO != null)
                .GroupBy(sg => sg.AssessmentItem!.CLOId)
                .ToList();

            foreach (var cloObj in clos)
            {
                var gradesForClo = cloGroupedGrades.FirstOrDefault(g => g.Key == cloObj.Id);
                double pointsEarned = gradesForClo?.Sum(g => g.PointsEarned) ?? 0.0;
                double maxPoints = gradesForClo?.Sum(g => g.AssessmentItem!.MaxPoints) ?? 0.0;

                double percentage = maxPoints > 0 ? (pointsEarned / maxPoints) * 100 : 0.0;
                bool isAttained = percentage >= 50.0; // 50% threshold for CLO passing

                // Retrieve course code and name
                var cloDetails = await _context.CourseLearningOutcomes
                    .Include(c => c.Course)
                    .FirstOrDefaultAsync(c => c.Id == cloObj.Id);

                report.CloAttainments.Add(new CloAttainmentDto
                {
                    CloId = cloObj.Id,
                    CloCode = cloObj.Code,
                    Description = cloObj.Description,
                    CourseCode = cloDetails?.Course?.Code ?? "N/A",
                    CourseName = cloDetails?.Course?.Name ?? "N/A",
                    MaxPoints = maxPoints,
                    PointsEarned = pointsEarned,
                    Percentage = Math.Round(percentage, 2),
                    IsAttained = isAttained
                });
            }

            // 2. Calculate PLO Attainments
            // Formula: PLO_k = Sum(CLO_i * Weight_ik) / Sum(Weight_ik) for all CLO_i mapped to PLO_k
            foreach (var plo in plos)
            {
                var mappedClos = clos.Where(c => c.PLOId == plo.Id).ToList();
                double weightedSum = 0.0;
                double totalWeight = 0.0;

                foreach (var mappedClo in mappedClos)
                {
                    var cloReport = report.CloAttainments.FirstOrDefault(c => c.CloId == mappedClo.Id);
                    if (cloReport != null)
                    {
                        weightedSum += cloReport.Percentage * mappedClo.WeightInPlo;
                        totalWeight += mappedClo.WeightInPlo;
                    }
                }

                double ploPercentage = totalWeight > 0 ? weightedSum / totalWeight : 0.0;
                bool isPloAttained = ploPercentage >= 60.0; // 60% threshold for PLO passing

                report.PloAttainments.Add(new PloAttainmentDto
                {
                    PloId = plo.Id,
                    PloCode = plo.Code,
                    Description = plo.Description,
                    Percentage = Math.Round(ploPercentage, 2),
                    IsAttained = isPloAttained
                });
            }

            // 3. Calculate Competency Profiles
            // Competency Score is the average score of all PLOs mapped to it
            var competencies = await _context.Competencies.ToListAsync();
            foreach (var comp in competencies)
            {
                var mappedPlos = plos.Where(p => p.CompetencyId == comp.Id).ToList();
                double scoreSum = 0.0;
                int count = 0;

                foreach (var mappedPlo in mappedPlos)
                {
                    var ploReport = report.PloAttainments.FirstOrDefault(p => p.PloId == mappedPlo.Id);
                    if (ploReport != null)
                    {
                        scoreSum += ploReport.Percentage;
                        count++;
                    }
                }

                double compScore = count > 0 ? scoreSum / count : 0.0;
                
                // Classify competency level
                string level;
                if (compScore >= 75.0)
                    level = "Mastery";
                else if (compScore >= 50.0)
                    level = "Developing";
                else
                    level = "Beginning";

                report.CompetencyProfiles.Add(new CompetencyProfileDto
                {
                    CompetencyId = comp.Id,
                    CompetencyCode = comp.Code,
                    CompetencyName = comp.Name,
                    AttainmentScore = Math.Round(compScore, 2),
                    Level = level
                });
            }

            // 4. Generate Remediation Recommendations
            // If CLO attainment < 50%, student needs remediation
            foreach (var cloAtt in report.CloAttainments.Where(c => !c.IsAttained))
            {
                report.RemediationRecommendations.Add(new RemediationDto
                {
                    CloCode = cloAtt.CloCode,
                    CourseCode = cloAtt.CourseCode,
                    CourseName = cloAtt.CourseName,
                    CurrentPercentage = cloAtt.Percentage,
                    ActionRequired = $"The student scored {cloAtt.Percentage}% (below 50% threshold) on {cloAtt.CloCode}. " +
                                     $"Remediation Plan: 1. Attend a dedicated tutorial session on '{cloAtt.Description}'. " +
                                     $"2. Review reference documentation. 3. Complete a recovery assignment covering the weak topics."
                });
            }

            return report;
        }
    }
}
