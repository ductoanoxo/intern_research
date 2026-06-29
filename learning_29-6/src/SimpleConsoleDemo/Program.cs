using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleConsoleDemo
{
    // ==========================================
    // 1. STUDENT ACADEMIC DATA MODELING
    // ==========================================
    public class Student
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class Skill
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Domain { get; set; } = ""; // Cognitive, Practical, SoftSkills
    }

    public class Prerequisite
    {
        public string ParentCode { get; set; } = "";
        public string ChildCode { get; set; } = "";
        public double Weight { get; set; } = 1.0;
    }

    public class Attempt
    {
        public string StudentCode { get; set; } = "";
        public string AssessmentCode { get; set; } = "";
        public string TargetSkillCode { get; set; } = "";
        public double Score { get; set; } // Điểm số chuẩn hóa từ 0.0 đến 1.0
    }

    public class SkillState
    {
        public double DirectMastery { get; set; }
        public double InferredMastery { get; set; }
        public double CombinedMastery { get; set; }
        public double Confidence { get; set; } // Trọng số tin cậy dựa trên số lần kiểm tra trực tiếp
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("==========================================================================");
            Console.WriteLine("🚀 DEMO NGHIÊN CỨU: STUDENT COMPETENCY MODELING & INFERENCE (SINGLE-FILE)");
            Console.WriteLine("==========================================================================\n");

            // ==========================================
            // 2. SKILL MAPPING (Định nghĩa Bản đồ Kỹ năng & Tiên quyết)
            // ==========================================
            var skills = new List<Skill>
            {
                new Skill { Code = "SK-001", Name = "Lập trình Cơ bản (Basics)", Domain = "Practical" },
                new Skill { Code = "SK-002", Name = "Lập trình Hướng đối tượng (OOP)", Domain = "Cognitive" },
                new Skill { Code = "SK-003", Name = "Phát triển Web API (API Dev)", Domain = "Practical" }
            };

            // Quan hệ tiên quyết: Basics (SK-001) -> OOP (SK-002) -> API Dev (SK-003)
            var prerequisites = new List<Prerequisite>
            {
                new Prerequisite { ParentCode = "SK-001", ChildCode = "SK-002", Weight = 1.0 },
                new Prerequisite { ParentCode = "SK-002", ChildCode = "SK-003", Weight = 0.8 }
            };

            // Dữ liệu học viên Trần Thị B (Mã: SV002)
            var student = new Student { Code = "SV002", Name = "Trần Thị B" };

            // Nhật ký điểm thi (Academic Logs):
            // KỊCH BẢN ĐẶC BIỆT: Học viên B CHƯA TỪNG thi bài nào về Lập trình Cơ bản (SK-001) hay OOP (SK-002).
            // Nhưng đã làm bài Lab Web API (SK-003) đạt điểm xuất sắc: 90% (0.90)
            var attempts = new List<Attempt>
            {
                new Attempt { StudentCode = "SV002", AssessmentCode = "LAB-API", TargetSkillCode = "SK-003", Score = 0.90 }
            };

            Console.WriteLine($"[Đầu vào] Học viên: {student.Name} ({student.Code})");
            Console.WriteLine(" Nhật ký làm bài thực tế:");
            foreach (var att in attempts)
            {
                Console.WriteLine($"   - Bài {att.AssessmentCode} đánh giá trực tiếp kỹ năng {att.TargetSkillCode} đạt: {att.Score * 100}%");
            }
            Console.WriteLine("\n -> Chú ý: Học viên chưa có bất kỳ điểm đánh giá trực tiếp nào cho SK-001 & SK-002.");
            Console.WriteLine("\n--- Đang thực hiện tính toán & suy luận kỹ năng ẩn... ---\n");

            // ==========================================
            // 4. SKILL INFERENCE ENGINE (Bộ suy luận kỹ năng)
            // ==========================================
            var skillStates = skills.ToDictionary(s => s.Code, s => new SkillState());

            // Bước 1: Tính toán Direct Mastery và Confidence ban đầu
            foreach (var sk in skills)
            {
                var skAttempts = attempts.Where(a => a.TargetSkillCode == sk.Code).ToList();
                if (skAttempts.Any())
                {
                    skillStates[sk.Code].DirectMastery = skAttempts.Average(a => a.Score);
                    // Có bài đánh giá trực tiếp -> Độ tin cậy = 1.0, ngược lại = 0.0
                    skillStates[sk.Code].Confidence = 1.0;
                    skillStates[sk.Code].CombinedMastery = skillStates[sk.Code].DirectMastery;
                }
                else
                {
                    skillStates[sk.Code].DirectMastery = 0.0;
                    skillStates[sk.Code].Confidence = 0.0;
                    skillStates[sk.Code].CombinedMastery = 0.0;
                }
            }

            // Bước 2: Chạy thuật toán Lan truyền suy luận 2 chiều (3 vòng lặp để hội tụ DAG)
            for (int iter = 0; iter < 3; iter++)
            {
                foreach (var sk in skills)
                {
                    double fwdInferred = 0.0;
                    double bwdInferred = 0.0;

                    // A. Forward Propagation (Suy luận tiến: Tiền quyết -> Nâng cao)
                    var parents = prerequisites.Where(p => p.ChildCode == sk.Code).ToList();
                    if (parents.Any())
                    {
                        // Mức sẵn sàng học kỹ năng nâng cao bằng trung bình các kỹ năng tiền quyết x 0.85
                        fwdInferred = parents.Average(p => skillStates[p.ParentCode].CombinedMastery) * 0.85;
                    }

                    // B. Backward Propagation (Suy luận lùi: Nâng cao -> Tiền quyết)
                    var children = prerequisites.Where(p => p.ParentCode == sk.Code).ToList();
                    if (children.Any())
                    {
                        // Nếu đã làm chủ kỹ năng nâng cao, chắc chắn phải biết các kỹ năng tiền quyết
                        bwdInferred = children.Max(c => skillStates[c.ChildCode].CombinedMastery * c.Weight);
                    }

                    // C. Hợp nhất điểm suy luận ẩn
                    double inferred = Math.Max(fwdInferred, bwdInferred);
                    skillStates[sk.Code].InferredMastery = inferred;

                    // D. Tính Combined Mastery dựa trên Confidence
                    double alpha = skillStates[sk.Code].Confidence;
                    skillStates[sk.Code].CombinedMastery = (alpha * skillStates[sk.Code].DirectMastery) + ((1.0 - alpha) * inferred);
                }
            }

            // In kết quả suy luận kỹ năng
            Console.WriteLine("📊 KẾT QUẢ SUY LUẬN KỸ NĂNG ẨN:");
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine(" Mã Kỹ Năng | Tên Kỹ Năng               | Trực tiếp | Suy luận | Hợp nhất (Cuối)");
            Console.WriteLine("--------------------------------------------------------------------------");
            foreach (var sk in skills)
            {
                var state = skillStates[sk.Code];
                Console.WriteLine($" {sk.Code,-10} | {sk.Name,-25} | {state.DirectMastery * 100,8:0.0}% | {state.InferredMastery * 100,7:0.0}% | {state.CombinedMastery * 100,12:0.0}%");
            }
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine("💡 GIẢI THÍCH LOGIC:");
            Console.WriteLine(" * Học viên đạt 90% ở môn API Dev (SK-003). Hệ thống chạy suy luận lùi (Backward Inference)");
            Console.WriteLine("   và xác định học viên đạt 72% ở môn OOP (SK-002) và 72% ở môn Basics (SK-001)");
            Console.WriteLine("   dù học viên này chưa làm bài đánh giá trực tiếp nào cho 2 kỹ năng đó.");
            Console.WriteLine();

            // ==========================================
            // 3. COMPETENCY PROFILE GENERATION (Tạo hồ sơ năng lực)
            // ==========================================
            Console.WriteLine("👤 HỒ SƠ NĂNG LỰC ĐẦU RA (COMPETENCY PROFILE):");
            Console.WriteLine("--------------------------------------------------------------------------");
            
            // Gom nhóm kỹ năng theo Domain để tính điểm năng lực vĩ mô
            var domains = skills.Select(s => s.Domain).Distinct().ToList();
            foreach (var dom in domains)
            {
                var domSkills = skills.Where(s => s.Domain == dom).ToList();
                double domainCompetency = domSkills.Average(s => skillStates[s.Code].CombinedMastery) * 100.0;
                
                Console.WriteLine($" - Trục Năng lực {dom,-12}: {domainCompetency,5:0.0}% [{GetCompetencyLabel(domainCompetency)}]");
            }
            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine("📝 KHUYẾN NGHỊ HỌC TẬP (Pedagogical Guidance):");
            if (skillStates["SK-001"].CombinedMastery < 0.80)
            {
                Console.WriteLine($" -> [Đề xuất]: Kỹ năng nền tảng 'Lập trình Cơ bản' chưa đạt mức giỏi (hiện tại {skillStates["SK-001"].CombinedMastery*100:0}%). Học viên nên bổ sung thêm bài tập để làm chủ hoàn toàn.");
            }
            else
            {
                Console.WriteLine(" -> Học viên có lộ trình học tập tốt. Sẵn sàng nhận dự án thực tế.");
            }
            Console.WriteLine("==========================================================================\n");
        }

        static string GetCompetencyLabel(double score)
        {
            if (score >= 80.0) return "Thành thạo (Mastered)";
            if (score >= 50.0) return "Đang phát triển (Developing)";
            return "Mới bắt đầu (Beginning)";
        }
    }
}
