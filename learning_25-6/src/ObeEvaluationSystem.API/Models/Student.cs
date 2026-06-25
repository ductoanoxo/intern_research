using System.Text.Json.Serialization;

namespace ObeEvaluationSystem.API.Models
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
    }
}
