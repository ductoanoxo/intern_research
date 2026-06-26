using Microsoft.EntityFrameworkCore;
using SmartLearningAnalytics.API.Models;

namespace SmartLearningAnalytics.API.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<StudentSkillState> StudentSkillStates => Set<StudentSkillState>();
        public DbSet<StudentActivityLog> StudentActivityLogs => Set<StudentActivityLog>();
        public DbSet<AssessmentRubric> AssessmentRubrics => Set<AssessmentRubric>();
        public DbSet<RubricCriterion> RubricCriteria => Set<RubricCriterion>();
        public DbSet<RubricScore> RubricScores => Set<RubricScore>();
        public DbSet<QuestionItem> QuestionItems => Set<QuestionItem>();
        public DbSet<QuestionAttempt> QuestionAttempts => Set<QuestionAttempt>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships
            
            // StudentSkillState
            modelBuilder.Entity<StudentSkillState>()
                .HasOne(s => s.Student)
                .WithMany(st => st.SkillStates)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentSkillState>()
                .HasOne(s => s.Skill)
                .WithMany()
                .HasForeignKey(s => s.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentActivityLog
            modelBuilder.Entity<StudentActivityLog>()
                .HasOne(s => s.Student)
                .WithMany(st => st.ActivityLogs)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // RubricCriterion
            modelBuilder.Entity<RubricCriterion>()
                .HasOne(r => r.Rubric)
                .WithMany(ru => ru.Criteria)
                .HasForeignKey(r => r.RubricId)
                .OnDelete(DeleteBehavior.Cascade);

            // RubricScore
            modelBuilder.Entity<RubricScore>()
                .HasOne(r => r.Student)
                .WithMany(st => st.RubricScores)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RubricScore>()
                .HasOne(r => r.Criterion)
                .WithMany()
                .HasForeignKey(r => r.CriterionId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuestionItem
            modelBuilder.Entity<QuestionItem>()
                .HasOne(q => q.Skill)
                .WithMany()
                .HasForeignKey(q => q.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuestionAttempt
            modelBuilder.Entity<QuestionAttempt>()
                .HasOne(q => q.Student)
                .WithMany(s => s.QuestionAttempts)
                .HasForeignKey(q => q.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionAttempt>()
                .HasOne(q => q.QuestionItem)
                .WithMany()
                .HasForeignKey(q => q.QuestionItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
