using Microsoft.EntityFrameworkCore;
using ObeEvaluationSystem.API.Models;

namespace ObeEvaluationSystem.API.Data
{
    public class ObeDbContext : DbContext
    {
        public ObeDbContext(DbContextOptions<ObeDbContext> options) : base(options)
        {
        }

        public DbSet<Competency> Competencies => Set<Competency>();
        public DbSet<ProgramLearningOutcome> ProgramLearningOutcomes => Set<ProgramLearningOutcome>();
        public DbSet<CourseLearningOutcome> CourseLearningOutcomes => Set<CourseLearningOutcome>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Assessment> Assessments => Set<Assessment>();
        public DbSet<AssessmentItem> AssessmentItems => Set<AssessmentItem>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentGrade> StudentGrades => Set<StudentGrade>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Competency -> PLO relationship
            modelBuilder.Entity<ProgramLearningOutcome>()
                .HasOne(p => p.Competency)
                .WithMany(c => c.ProgramLearningOutcomes)
                .HasForeignKey(p => p.CompetencyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure PLO -> CLO relationship
            modelBuilder.Entity<CourseLearningOutcome>()
                .HasOne(c => c.PLO)
                .WithMany(p => p.CourseLearningOutcomes)
                .HasForeignKey(c => c.PLOId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Course -> CLO relationship
            modelBuilder.Entity<CourseLearningOutcome>()
                .HasOne(c => c.Course)
                .WithMany(co => co.CourseLearningOutcomes)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Course -> Assessment relationship
            modelBuilder.Entity<Assessment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assessments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Assessment -> AssessmentItem relationship
            modelBuilder.Entity<AssessmentItem>()
                .HasOne(ai => ai.Assessment)
                .WithMany(a => a.AssessmentItems)
                .HasForeignKey(ai => ai.AssessmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure CLO -> AssessmentItem relationship
            modelBuilder.Entity<AssessmentItem>()
                .HasOne(ai => ai.CLO)
                .WithMany(clo => clo.AssessmentItems)
                .HasForeignKey(ai => ai.CLOId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Student -> StudentGrade relationship
            modelBuilder.Entity<StudentGrade>()
                .HasOne(sg => sg.Student)
                .WithMany(s => s.StudentGrades)
                .HasForeignKey(sg => sg.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure AssessmentItem -> StudentGrade relationship
            modelBuilder.Entity<StudentGrade>()
                .HasOne(sg => sg.AssessmentItem)
                .WithMany(ai => ai.StudentGrades)
                .HasForeignKey(sg => sg.AssessmentItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
