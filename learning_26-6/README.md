# [Daily report - 26/06/2026]

*   **Họ tên:** Trà Đức Toàn
*   **Nội dung nghiên cứu hôm nay:**
    1.  **Competency Assessment (Đánh giá năng lực):** Nghiên cứu phương pháp đo lường năng lực tích hợp qua bộ Rubric đa chiều (Criteria, Weight, Descriptors) và Đánh giá thích ứng (Adaptive Assessment) dựa trên trình độ người học.
    2.  **Learning Analytics (Phân tích học tập):** Tìm hiểu 4 cấp độ phân tích (Descriptive, Diagnostic, Predictive, Prescriptive) và các chỉ số hành vi học tập (Time-on-task, Consistency Score, Submission Latency).
    3.  **Student Modeling (Mô hình hóa người học):** Nghiên cứu mô hình toán học Bayesian Knowledge Tracing (BKT) (Hidden Markov Model để theo dõi trạng thái tri thức) và Item Response Theory (IRT) (2PL logistic model đo năng lực tiềm ẩn $\theta$ của người học và đặc tính câu hỏi).
    4.  **Educational Data Mining (EDM):** Phân biệt với Learning Analytics; áp dụng thuật toán K-Means Clustering để phân cụm sinh viên và Decision Tree Classifier để dự báo nguy cơ học tập giải thích được (Explainable AI).
*   **Tiến độ tự đánh giá:** 100%
*   **Issue:** Không có.
*   **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 4 keyword tại thư mục [docs/lythuyet_analytics_modeling_edm.md](./docs/lythuyet_analytics_modeling_edm.md).

---

# BÁO CÁO CHI TIẾT DỰ ÁN DEMO: SMART LEARNING ANALYTICS ENGINE

Dự án này triển khai một **Smart Learning Analytics Engine** bằng **.NET 8** sử dụng SQLite. Hệ thống áp dụng các mô hình toán học và logic phân tích thực tế để phân tích hành vi, đo lường năng lực đa chiều qua Rubrics, mô hình hóa tri thức người học bằng BKT/IRT và dự báo nguy cơ học tập bằng Decision Tree và Phân cụm sinh viên.

## 1. Kiến trúc Hệ thống & Luồng Dữ liệu (Data Flow)

```
[Student Logs & Quiz Answers]
       │
       ├─────────────────────────────────┐
       ▼ (BKT & IRT Engines)             ▼ (Activity Log Analytics)
[Student Tri thức & Năng lực θ]    [LA Metrics: Time, Consistency, Latency]
       │                                 │
       └──────────────┬──────────────────┘
                      ▼
        [EDM Risk Prediction Engine] (Decision Tree & K-Means)
                      │
                      ▼
     [Pedagogical Remediation Plan] (Gợi ý lộ trình cá nhân hóa)
```

### Các Module cốt lõi:
1.  **Bayesian Knowledge Tracing (BKT) Engine:** Theo dõi xác suất làm chủ kỹ năng $P(L_t)$ thời gian thực sau mỗi lượt làm bài.
2.  **Item Response Theory (IRT) Engine:** Sử dụng mô hình 2PL logistic để tính xác suất và ước lượng năng lực tiềm ẩn ($\theta$) dựa trên độ khó trung bình của các câu hỏi làm được.
3.  **Competency Rubrics Evaluation:** Đánh giá năng lực đa chiều bằng thang điểm Rubric có trọng số.
4.  **EDM Predictor:**
    *   **Decision Tree:** Phân loại rủi ro học tập (`Safe`, `Borderline`, `AtRisk`) có kèm theo giải thích chi tiết quy luật (Explainable Path).
    *   **Rule-based Clustering (Mô phỏng K-Means):** Phân cụm sinh viên trong lớp thành 3 nhóm hành vi: *Active High-Achievers*, *Inconsistent Performers*, và *At-Risk Learners*.

---

## 2. Cấu trúc mã nguồn dự án

*   **`src/SmartLearningAnalytics.API/Models/`**: Các thực thể miền (Domain Entities).
    *   `Student.cs`: Thông tin sinh viên.
    *   `Skill.cs`: Kỹ năng học tập kèm tham số BKT ($P(L_0), P(T), P(G), P(S)$).
    *   `StudentSkillState.cs`: Trạng thái xác suất mastery hiện tại của sinh viên đối với skill.
    *   `StudentActivityLog.cs`: Log chi tiết hành vi học tập (Time-on-task, Activity Type).
    *   `AssessmentRubric.cs` & `RubricCriterion.cs`: Khung Rubric đánh giá năng lực đa chiều.
    *   `RubricScore.cs`: Điểm số đánh giá chi tiết theo từng tiêu chí Rubric.
    *   `QuestionItem.cs`: Câu hỏi thi trắc nghiệm kèm tham số IRT ($Difficulty\ b, Discrimination\ a$).
    *   `QuestionAttempt.cs`: Lịch sử trả lời câu hỏi của sinh viên.
*   **`src/SmartLearningAnalytics.API/Data/AnalyticsDbContext.cs`**: Quản lý SQLite DB và cấu hình Fluent API.
*   **`src/SmartLearningAnalytics.API/Services/`**:
    *   `BktEngine.cs`: Thuật toán BKT cập nhật xác suất tri thức.
    *   `IrtEngine.cs`: Thuật toán IRT ước lượng năng lực $\theta$ (Heuristic Average).
    *   `EdmEngine.cs`: Phân cụm Cohort và Decision Tree Risk Classifier.
    *   `AnalyticsService.cs`: Điều phối dữ liệu, sinh báo cáo tổng hợp và seeding dữ liệu mẫu.
*   **`src/SmartLearningAnalytics.API/Controllers/AnalyticsController.cs`**: Cung cấp các REST API endpoints và các cổng sandbox mô phỏng.

---

## 💻 HƯỚNG DẪN KHỞI CHẠY & KIỂM THỬ DỰ ÁN

### 1. Khởi động hệ thống bằng Docker Compose

Mở terminal tại thư mục `learning_26-6/src/` và chạy lệnh:
```bash
docker compose up --build -d
```

### 2. Các cổng dịch vụ mở trên localhost
*   **Smart Learning Analytics Swagger Portal:** [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

### 3. Kịch bản kiểm thử (Step-by-Step Test)

#### Bước 3.1: Seed dữ liệu ban đầu
Gọi API seed dữ liệu để tạo cấu trúc Skills, IRT Questions, Competency Rubrics, và 3 hồ sơ sinh viên tiêu biểu đại diện cho 3 nhóm học tập:
*   **Method:** `POST`
*   **URL:** `http://localhost:8080/api/analytics/seed`
*   **Response:** `{"message": "Smart Learning Analytics database successfully seeded with 3 main student profiles and 7 cohort students."}`

#### Bước 3.2: Lấy danh sách Sinh viên để lấy GUID kiểm tra
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/analytics/students`
*   **Kết quả:** Nhận về danh sách sinh viên gồm `Nguyen Van A` (SV001 - Giỏi), `Tran Thi B` (SV002 - Khá/Chưa ổn định), và `Le Van C` (SV003 - Nguy cơ cao).

#### Bước 3.3: Xem báo cáo phân tích chi tiết của học viên giỏi (SV001)
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/analytics/students/{SV001_GUID_HERE}/report`
*   **Kết quả:**
    *   **LA Metrics:** `totalStudyTimeHours` cao (~12.2 giờ), `engagementScore` đạt 100%, `consistencyScore` cao (1.0).
    *   **Student Modeling:** Năng lực ước lượng IRT $\theta$ cao (2.5), xác suất làm chủ kỹ năng BKT $P(L)$ đối với `SQL-JOIN` đạt mức $0.96$ (`Mastered`).
    *   **Competency Profile:** Đạt điểm Rubric trung bình $91\%$.
    *   **EDM Prediction:** Trạng thái nguy cơ là `Safe` với đường dẫn cây quyết định (Decision Path) chi tiết.

#### Bước 3.4: Xem báo cáo của học viên thụ động nguy cơ cao (SV003)
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/analytics/students/{SV003_GUID_HERE}/report`
*   **Kết quả:**
    *   **LA Metrics:** Học tập cực ít (~0.25 giờ), `consistencyScore` thấp (0.1).
    *   **Student Modeling:** Ước lượng năng lực IRT $\theta$ cực thấp (-2.5), xác suất BKT $P(L)$ chỉ đạt $0.11$.
    *   **Competency Profile:** Điểm Rubric kém (~40%).
    *   **EDM Prediction:** Trạng thái nguy cơ là `AtRisk`. Hệ thống tự động đề xuất lộ trình cải thiện (Pedagogical Recommendations) như ôn tập Module 1, xếp lịch dạy phụ đạo 1-on-1.

#### Bước 3.5: Nộp câu trả lời mới và kiểm tra cập nhật BKT thời gian thực
Học viên `SV002` thực hiện trả lời đúng một câu hỏi. Gọi API nộp bài:
*   **Method:** `POST`
*   **URL:** `http://localhost:8080/api/analytics/attempts`
*   **Body:**
    ```json
    {
      "studentId": "{SV002_GUID_HERE}",
      "questionItemId": "{Q_SQL_03_GUID_HERE}",
      "isCorrect": true
    }
    ```
*   **Kết quả:** Trả về xác suất tri thức trước (`PriorMasteryProbability`) và sau (`UpdatedMasteryProbability`) được cập nhật theo công thức BKT một cách trực quan.

#### Bước 3.6: Xem gom cụm tự động cohort (Rule-based Clustering)
Gọi API phân cụm để xem hệ thống phân chia 10 học viên trong lớp thành các nhóm hành vi:
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/analytics/cohort/clusters`
*   **Kết quả:** Nhận về 3 nhóm:
    *   `Active High-Achievers` (Nguyen Van A và các bạn học chăm chỉ).
    *   `Inconsistent / Moderate Performers` (Tran Thi B và nhóm học bình thường).
    *   `At-Risk / Passive Learners` (Le Van C và các bạn ít hoạt động).
    *   Các toạ độ Centroid tương ứng đại diện cho trung bình (Mean) hành vi của nhóm.

#### Bước 3.7: Trải nghiệm Sandbox mô phỏng BKT và IRT
Hệ thống cung cấp 2 endpoint sandbox để người dùng tự do thử nghiệm thuật toán mà không cần lưu cơ sở dữ liệu:
*   **Simulate BKT:** `POST http://localhost:8080/api/analytics/simulation/bkt`
    *   *Body:* Truyền vào các tham số $P(L_0), P(T), P(G), P(S)$ và chuỗi đúng/sai `[true, true, false, true]` để nhận về chi tiết từng bước tính toán.
*   **Simulate IRT:** `POST http://localhost:8080/api/analytics/simulation/irt`
    *   *Body:* Truyền vào một danh sách các câu hỏi có độ khó, độ phân biệt khác nhau kèm kết quả đúng/sai của sinh viên để ước lượng năng lực $\theta$.

---

## 4. Dọn dẹp tài nguyên
Để tắt container:
```bash
docker compose down
```
