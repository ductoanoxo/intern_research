using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartLearningAnalytics.API.Services
{
    public interface IEdmEngine
    {
        DecisionTreeResult PredictStudentRisk(double studyTimeHours, double quizScorePercent, double avgLatencyHours);
        List<ClusterResult> ClusterStudents(List<StudentClusterInput> students);
    }

    public class DecisionTreeResult
    {
        public string RiskStatus { get; set; } = string.Empty; // AtRisk, Borderline, Safe
        public List<string> DecisionPath { get; set; } = new List<string>();
        public string Explanation { get; set; } = string.Empty;
    }

    public class StudentClusterInput
    {
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public double StudyTimeHours { get; set; }
        public double AverageQuizScorePercent { get; set; }
    }

    public class ClusterResult
    {
        public int ClusterId { get; set; }
        public string ClusterName { get; set; } = string.Empty;
        public string ClusterDescription { get; set; } = string.Empty;
        public double CentroidStudyTimeHours { get; set; }
        public double CentroidAverageScorePercent { get; set; }
        public List<StudentClusterInput> Members { get; set; } = new List<StudentClusterInput>();
    }

    public class EdmEngine : IEdmEngine
    {
        // Simple nested if-else to represent Decision Tree logic
        public DecisionTreeResult PredictStudentRisk(double studyTimeHours, double quizScorePercent, double avgLatencyHours)
        {
            var result = new DecisionTreeResult();

            if (quizScorePercent < 50.0)
            {
                result.DecisionPath.Add($"Average Quiz Score ({quizScorePercent:F1}%) < 50.0%");
                if (studyTimeHours < 3.0)
                {
                    result.DecisionPath.Add($"Total Study Time ({studyTimeHours:F1} hours) < 3.0 hours");
                    result.RiskStatus = "AtRisk";
                    result.Explanation = "Học viên có kết quả kiểm tra thấp và thời gian tự học rất ít. Nguy cơ trượt môn cao.";
                }
                else
                {
                    result.DecisionPath.Add($"Total Study Time ({studyTimeHours:F1} hours) >= 3.0 hours");
                    result.RiskStatus = "Borderline";
                    result.Explanation = "Học viên có học bài nhưng điểm kiểm tra vẫn thấp. Cần hỗ trợ phụ đạo thêm.";
                }
            }
            else
            {
                result.DecisionPath.Add($"Average Quiz Score ({quizScorePercent:F1}%) >= 50.0%");
                if (studyTimeHours < 2.0 && avgLatencyHours > 12.0)
                {
                    result.DecisionPath.Add($"Total Study Time ({studyTimeHours:F1} hours) < 2.0 hours");
                    result.DecisionPath.Add($"Average Submission Latency ({avgLatencyHours:F1} hours) > 12.0 hours");
                    result.RiskStatus = "Borderline";
                    result.Explanation = "Kết quả đạt yêu cầu nhưng thời gian tự học ít và nộp bài sát/trễ hạn. Cần nhắc nhở.";
                }
                else
                {
                    result.DecisionPath.Add("Study Engagement & Submissions are satisfactory");
                    result.RiskStatus = "Safe";
                    result.Explanation = "Học tập ổn định, tự học tốt và điểm kiểm tra đạt yêu cầu.";
                }
            }

            return result;
        }

        // Simplify clustering from iterative mathematical K-Means to Rule-based Cohort Segmentation
        // (This simulates clustering behavior in a deterministic and clear way)
        public List<ClusterResult> ClusterStudents(List<StudentClusterInput> students)
        {
            var clusters = new List<ClusterResult>
            {
                new ClusterResult 
                { 
                    ClusterId = 0, 
                    ClusterName = "At-Risk / Passive Learners", 
                    ClusterDescription = "Học viên có điểm số thấp hoặc thời gian tự học quá ít. Cần giáo viên quan tâm trực tiếp." 
                },
                new ClusterResult 
                { 
                    ClusterId = 1, 
                    ClusterName = "Inconsistent / Moderate Performers", 
                    ClusterDescription = "Học viên học tập ở mức trung bình, nỗ lực và kết quả tương đối ổn định." 
                },
                new ClusterResult 
                { 
                    ClusterId = 2, 
                    ClusterName = "Active High-Achievers", 
                    ClusterDescription = "Học viên xuất sắc, tích cực tự học và đạt điểm kiểm tra rất cao." 
                }
            };

            if (students == null || students.Count == 0)
            {
                return clusters;
            }

            // Group students into clusters using simple, logical threshold rules
            foreach (var student in students)
            {
                if (student.AverageQuizScorePercent < 50.0 || student.StudyTimeHours < 1.5)
                {
                    clusters[0].Members.Add(student);
                }
                else if (student.AverageQuizScorePercent >= 75.0 && student.StudyTimeHours >= 4.0)
                {
                    clusters[2].Members.Add(student);
                }
                else
                {
                    clusters[1].Members.Add(student);
                }
            }

            // Calculate centroids as the average of study hours and test scores within each group
            foreach (var cluster in clusters)
            {
                if (cluster.Members.Count > 0)
                {
                    cluster.CentroidStudyTimeHours = Math.Round(cluster.Members.Average(m => m.StudyTimeHours), 2);
                    cluster.CentroidAverageScorePercent = Math.Round(cluster.Members.Average(m => m.AverageQuizScorePercent), 2);
                }
                else
                {
                    cluster.CentroidStudyTimeHours = 0.0;
                    cluster.CentroidAverageScorePercent = 0.0;
                }
            }

            return clusters;
        }
    }
}
