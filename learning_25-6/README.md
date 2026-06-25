# [Daily report - 25/06/2026]

*   **Họ tên:** Trà Đức Toàn
*   **Nội dung nghiên cứu hôm nay:**
    1.  **Outcome-Based Education (OBE)**: Tìm hiểu triết lý và 4 nguyên lý cốt lõi của giáo dục dựa trên chuẩn đầu ra (Clarity of focus, Design backwards, High expectations, Expanded opportunities).
    2.  **Learning Outcomes (CLO, PLO)**: Nghiên cứu phân cấp chuẩn đầu ra học phần (Course Learning Outcomes - CLO) theo thang đo Bloom và chuẩn đầu ra chương trình (Program Learning Outcomes - PLO), cùng với cách thức xây dựng ma trận ánh xạ.
    3.  **Competency Framework (Khung năng lực)**: Cấu trúc mô hình KSA (Knowledge, Skills, Attitude) để đánh giá năng lực nghề nghiệp thực tế và mối tương quan giữa Competency với PLO/CLO.
    4.  **OBE Attainment Engine**: Nghiên cứu công thức toán học đo lường độ đạt chuẩn của sinh viên từ các đầu điểm chi tiết lên CLO, PLO và phân loại cấp độ năng lực (Mastery, Developing, Beginning) nhằm đề xuất lộ trình cải thiện (Remediation).
*   **Tiến độ tự đánh giá:** 100%
*   **Issue:** Không có.
*   **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 3 keyword tại thư mục [docs/lythuyet_obe_clo_plo_competency.md](./docs/lythuyet_obe_clo_plo_competency.md).

---

# BÁO CÁO CHI TIẾT DỰ ÁN DEMO: OBE EVALUATION SYSTEM

Dự án này triển khai một **OBE Evaluation Engine** thu nhỏ bằng **.NET 8** sử dụng SQLite. Hệ thống tự động tính toán tiến độ đạt chuẩn của người học (CLO, PLO), ánh xạ chúng sang Khung năng lực (Competency) và đưa ra gợi ý kế hoạch học tập cải thiện (Remediation).

## 1. Mô hình Dữ liệu & Luồng xử lý Tính toán (Data Flow)

```
[Student Grades] (Scores per Question)
       │
       ▼ (Sum points earned vs max points per CLO)
[CLO Attainment] ───(Threshold < 50%)───► [Remediation Recommendations]
       │
       ▼ (Weighted average using WeightInPlo)
[PLO Attainment]
       │
       ▼ (Average score of mapped PLOs)
[Competency Profile] ───► [Competency Level: Mastery / Developing / Beginning]
```

### Quy tắc tính toán:
1.  **CLO Attainment:** $\frac{\sum \text{Điểm đạt được}}{\sum \text{Điểm tối đa}} \times 100$. Chuẩn đạt được khi đạt $\ge 50\%$.
2.  **PLO Attainment:** Trung bình có trọng số của các CLO ánh xạ vào PLO đó. Chuẩn đạt được khi đạt $\ge 60\%$.
3.  **Competency Level:** Trung bình cộng các PLO liên quan:
    *   **Mastery (Thành thục):** $\ge 75\%$
    *   **Developing (Đang phát triển):** $50\% - 75\%$
    *   **Beginning (Mới bắt đầu):** $< 50\%$ (Hệ thống tự động đưa vào danh sách Remediation).

---

## 2. Cấu trúc mã nguồn dự án

*   **`src/ObeEvaluationSystem.API/Models/`**: Các thực thể miền (Domain Entities).
    *   `Competency.cs`: Khung năng lực nghề nghiệp (ví dụ: Phát triển Web, Cơ sở dữ liệu).
    *   `ProgramLearningOutcome.cs` (PLO): Chuẩn đầu ra cấp chương trình.
    *   `CourseLearningOutcome.cs` (CLO): Chuẩn đầu ra cấp môn học, ánh xạ lên PLO kèm trọng số.
    *   `Course.cs`: Thông tin môn học.
    *   `Assessment.cs` & `AssessmentItem.cs`: Các bài kiểm tra và câu hỏi chi tiết gắn với CLO.
    *   `Student.cs` & `StudentGrade.cs`: Thông tin sinh viên và điểm số đạt được cho từng câu hỏi.
*   **`src/ObeEvaluationSystem.API/Data/ObeDbContext.cs`**: Quản lý kết nối SQLite và cấu hình quan hệ thực thể bằng Fluent API.
*   **`src/ObeEvaluationSystem.API/Services/`**:
    *   `IObeEvaluationService.cs` & `ObeEvaluationService.cs`: Chứa logic nghiệp vụ tính toán phân tích và seeding dữ liệu mẫu.
*   **`src/ObeEvaluationSystem.API/Controllers/ObeController.cs`**: Các endpoint REST API cung cấp dữ liệu cho Client và kích hoạt Seeding.

---

## 💻 HƯỚNG DẪN KHỞI CHẠY & KIỂM THỬ DỰ ÁN

### 1. Khởi động hệ thống bằng Docker Compose

Mở terminal tại thư mục `learning_25-6/src/` và chạy lệnh:
```bash
docker compose up --build -d
```
Lệnh này sẽ build ứng dụng API và khởi chạy trên môi trường Docker.

### 2. Các cổng dịch vụ mở trên localhost
*   **OBE API Swagger Portal:** [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

### 3. Kịch bản kiểm thử (Step-by-Step Test)

#### Bước 3.1: Seed dữ liệu OBE ban đầu
Gọi API seed dữ liệu mẫu để tạo cấu trúc Competencies, PLOs, CLOs, các bài test và điểm của 2 sinh viên (`SV001` - Nguyen Van A, `SV002` - Tran Thi B):
*   **Method:** `POST`
*   **URL:** `http://localhost:8080/api/obe/seed`
*   **Response nhận được:** `{"message": "OBE Education Data successfully seeded into SQLite database."}`

#### Bước 3.2: Lấy danh sách Sinh viên để lấy ID kiểm tra
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/obe/students`
*   **Kết quả:** Nhận về danh sách ID dạng GUID của sinh viên `SV001` và `SV002`.

#### Bước 3.3: Xem báo cáo OBE chi tiết của Sinh viên xuất sắc (SV001)
Gọi API xem báo cáo bằng ID của sinh viên `SV001`:
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/obe/students/{SV001_GUID_HERE}/report`
*   **Kết quả:** 
    *   Các CLO (1.1, 1.2, 2.1) đạt tỉ lệ cao ($>85\%$), trạng thái `isAttained: true`.
    *   Các PLO (1, 2) đạt trạng thái đạt chuẩn.
    *   Khung năng lực `COMP-1` và `COMP-2` đều xếp loại **Mastery**.
    *   Danh sách `remediationRecommendations` trống vì sinh viên đạt tất cả chuẩn đầu ra.

#### Bước 3.4: Xem báo cáo OBE của Sinh viên gặp khó khăn (SV002)
Gọi API xem báo cáo bằng ID của sinh viên `SV002` (sinh viên bị hổng kiến thức JWT):
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/obe/students/{SV002_GUID_HERE}/report`
*   **Kết quả:**
    *   `CLO-1.1` (JWT Auth) chỉ đạt **40%** $\rightarrow$ `isAttained: false`.
    *   `CLO-1.2` (Clean Arch) đạt **80%** $\rightarrow$ `isAttained: true`.
    *   Do `CLO-1.1` có trọng số lớn ($60\%$), `PLO-1` chỉ đạt **56%** $\rightarrow$ `isAttained: false`.
    *   Khung năng lực `COMP-1` (Web Dev) bị hạ xuống mức **Developing** ($56\%$).
    *   Hệ thống tự động đưa ra phương án cải thiện tại danh sách `remediationRecommendations`:
        ```json
        {
          "cloCode": "CLO-1.1",
          "courseCode": "CS-201",
          "courseName": "ASP.NET Core Backend Development",
          "currentPercentage": 40,
          "actionRequired": "The student scored 40% (below 50% threshold) on CLO-1.1. Remediation Plan: 1. Attend a dedicated tutorial session on 'Implement secure JWT-based authentication and claim-based authorization.'. 2. Review reference documentation. 3. Complete a recovery assignment covering the weak topics."
        }
        ```

---

### 4. Dọn dẹp tài nguyên
Để tắt container:
```bash
docker compose down
```
