# Tài liệu Nghiên cứu Lý thuyết: Student Academic Data Modeling, Skill Mapping, Competency Profile Generation & Skill Inference

Tài liệu này nghiên cứu chuyên sâu về các phương pháp mô hình hóa dữ liệu học tập của sinh viên, bản đồ kỹ năng, tạo hồ sơ năng lực và thuật toán suy luận kỹ năng ẩn trong các hệ thống Giáo dục Thích ứng (Adaptive Learning) và Quản lý Giáo dục theo Kết quả Đầu ra (OBE).

---

## 1. Student Academic Data Modeling (Mô hình hóa Dữ liệu Học tập của Sinh viên)

### 1.1. Khái niệm và Vai trò
**Student Academic Data Modeling** là quá trình thiết kế cấu trúc dữ liệu để lưu trữ, biểu diễn và phản ánh toàn bộ tiến trình học tập, hành vi, và trạng thái tri thức của người học. Một mô hình dữ liệu học tập tốt không chỉ lưu trữ điểm số tĩnh (Static Grades) mà phải ghi lại chuỗi thời gian (Time-series) tương tác của sinh viên nhằm mục đích phân tích và dự báo.

### 1.2. Các thành phần cốt lõi của Mô hình Dữ liệu Học tập
Mô hình dữ liệu học tập chuẩn thường bao gồm ba nhóm thực thể chính:
1.  **Thông tin Định danh & Bối cảnh (Student Context):**
    *   `Student`: ID, Tên, Mã sinh viên, Lớp học, Chương trình đào tạo.
    *   `Enrollment`: Lịch sử đăng ký khóa học, Lớp học phần.
2.  **Dữ liệu Tiến trình & Hành vi (Process & Activity Logs):**
    *   `StudentActivityLog`: Ghi nhận hoạt động trên hệ thống LMS (Time-on-task, thời gian xem bài giảng, số lần tải tài liệu, tương tác diễn đàn).
    *   `Submission`: Lịch sử nộp bài tập, thời gian hoàn thành so với deadline.
3.  **Dữ liệu Kết quả & Đánh giá (Assessment Attempt Logs):**
    *   `AssessmentAttempt`: Chi tiết kết quả các lần làm bài kiểm tra, thi cử. Ghi nhận điểm số, đáp án lựa chọn, thời gian làm từng câu hỏi.

---

## 2. Skill Mapping (Bản đồ Kỹ năng)

### 2.1. Định nghĩa Skill Mapping
**Skill Mapping** là quá trình xây dựng cấu trúc phân cấp (Taxonomy/Ontology) của các kỹ năng học thuật hoặc nghề nghiệp, sau đó ánh xạ (map) các tài nguyên học tập (bài giảng, video) và công cụ đánh giá (câu hỏi quiz, bài tập lớn, tiêu chí rubric) vào các kỹ năng tương ứng.

```
       [Lĩnh vực Tri thức / Khóa học]
                    │
         ┌──────────┴──────────┐
         ▼                     ▼
     [Kỹ năng A]           [Kỹ năng B]
         │                     │
   ┌─────┴─────┐               ▼
   ▼           ▼         [Câu hỏi Q3] (Weight: 1.0)
[Câu hỏi Q1] [Câu hỏi Q2]
(Weight: 0.7) (Weight: 0.3)
```

### 2.2. Các thành phần của Hệ thống Skill Map
*   **Skill Nodes (Nút kỹ năng):** Đại diện cho một đơn vị kiến thức cụ thể (ví dụ: *SQL Join*, *Sorting Algorithms*, *Git Branching*). Mỗi nút thường có cấp độ Bloom (Remember, Understand, Apply, Analyze, Evaluate, Create).
*   **Assessment-Skill Mapping Matrix (Ma trận ánh xạ Đánh giá - Kỹ năng):** Một câu hỏi trắc nghiệm hoặc một tiêu chí Rubric có thể đo lường một hoặc nhiều kỹ năng khác nhau với các trọng số (Weights) khác nhau (tổng trọng số đối với một câu hỏi thường bằng 1.0).
*   **Prerequisite Relations (Quan hệ tiên quyết):** Định nghĩa mối quan hệ phụ thuộc giữa các kỹ năng dưới dạng một Đồ thị có hướng không chu trình (DAG - Directed Acyclic Graph). Ví dụ: Kỹ năng *Basic Programming* là điều kiện tiên quyết để học *Data Structures*.

---

## 3. Competency Profile Generation (Tạo Hồ sơ Năng lực)

### 3.1. Phân biệt Skill (Kỹ năng) và Competency (Năng lực)
*   **Skill (Kỹ năng):** Khả năng thực hiện một hành động cụ thể, kỹ thuật (ví dụ: viết hàm Python, cấu hình router). Thường mang tính vi mô (micro).
*   **Competency (Năng lực):** Khả năng ứng dụng tổng hòa kiến thức, kỹ năng và thái độ để hoàn thành xuất sắc một vai trò hoặc nhiệm vụ phức tạp trong bối cảnh thực tế (ví dụ: *Năng lực Thiết kế Hệ thống*, *Năng lực Giải quyết Vấn đề*). Mang tính vĩ mô (macro).

### 3.2. Quy trình sinh Hồ sơ Năng lực (Competency Profile)
Hồ sơ Năng lực của sinh viên được biểu diễn dưới dạng biểu đồ radar hoặc tập hợp điểm số trên các trục năng lực chính (Domains):
1.  **Thu thập dữ liệu thô:** Lấy điểm các bài kiểm tra, quiz và hành vi học tập của sinh viên.
2.  **Tính toán điểm kỹ năng trực tiếp (Direct Skill Score):**
    $$S_{direct}(Skill_j) = \frac{\sum_{i} AttemptScore_i \times Weight_{ij}}{\sum_{i} MaxScore_i \times Weight_{ij}}$$
3.  **Hợp nhất với Suy luận Kỹ năng (Inferred Skill Score):** Kết hợp điểm trực tiếp và điểm suy luận từ các kỹ năng liên quan trong đồ thị.
4.  **Ánh xạ lên Khung Năng lực (Competency Mapping):** Gom nhóm các kỹ năng thành các trục năng lực lớn (ví dụ: *Cognitive/Nhận thức*, *Practical/Thực hành*, *Social/Giao tiếp*).
    $$Competency(Domain_k) = \frac{\sum_{j \in Domain_k} SkillScore_j \times Significance_j}{\sum_{j \in Domain_k} Significance_j}$$

---

## 4. Skill Inference (Suy luận Kỹ năng)

### 4.1. Tại sao cần Suy luận Kỹ năng?
Trong thực tế, một bài kiểm tra không thể bao phủ toàn bộ hàng trăm kỹ năng nhỏ trong chương trình học. **Skill Inference** cho phép hệ thống tự động suy luận trạng thái thấu hiểu của sinh viên đối với các kỹ năng **chưa được đánh giá trực tiếp** thông qua các kỹ năng đã được đánh giá và quan hệ tiên quyết (Prerequisite Graph).

### 4.2. Cơ chế suy luận hai chiều trên Đồ thị Tiên quyết (DAG)
Giả sử có quan hệ tiên quyết: $Skill_{Prereq} \rightarrow Skill_{Advanced}$

#### 4.2.1. Suy luận tiến (Forward Inference - Từ Tiền quyết lên Nâng cao):
Nếu sinh viên chưa làm bài kiểm tra nào của kỹ năng nâng cao ($Skill_{Advanced}$), mức độ sẵn sàng học hoặc làm chủ kỹ năng đó được ước lượng dựa trên mức độ hoàn thành các kỹ năng tiền quyết của nó.
*   **Logic:** Nếu chưa thành thạo kỹ năng cơ bản, chắc chắn sẽ gặp khó khăn hoặc không thể làm chủ kỹ năng nâng cao.
*   **Công thức mô phỏng:**
    $$M_{fwd}(Skill_{Advanced}) = \left( \prod_{c \in Prereqs} M_{final}(c) \right) \times Confidence_{fwd}$$
    Trong đó $M_{final}(c)$ là mức độ làm chủ của kỹ năng tiền quyết $c$.

#### 4.2.2. Suy luận lùi (Backward Inference - Từ Nâng cao xuống Tiền quyết):
Nếu sinh viên làm bài kiểm tra và đạt điểm cao ở kỹ năng nâng cao ($Skill_{Advanced}$), hệ thống có thể suy luận rằng sinh viên đó **đã làm chủ** các kỹ năng tiền quyết ($Skill_{Prereq}$), ngay cả khi họ chưa từng làm bài tập về kỹ năng tiền quyết đó.
*   **Logic:** Bạn không thể viết một chương trình hướng đối tượng phức tạp (nâng cao) thành công nếu bạn không biết cách khai báo biến và viết vòng lặp (cơ bản).
*   **Công thức mô phỏng:**
    $$M_{bwd}(Skill_{Prereq}) = \max \left( M_{final}(Skill_{Prereq}), M_{final}(Skill_{Advanced}) \times Confidence_{bwd} \right)$$

#### 4.2.3. Hợp nhất điểm làm chủ cuối cùng (Mastery Fusion):
Điểm làm chủ cuối cùng của một kỹ năng được tính bằng cách dung hòa điểm đánh giá trực tiếp và điểm suy luận (tiến/lùi) dựa trên mức độ tin cậy:
$$M_{final}(Skill) = \alpha \cdot M_{direct}(Skill) + (1 - \alpha) \cdot M_{inferred}(Skill)$$
Trong đó:
*   $\alpha$ là trọng số tin cậy trực tiếp. Nếu số lượng bài tập trực tiếp $N \ge N_{threshold}$, $\alpha \approx 1.0$ (tin tưởng hoàn toàn vào bài kiểm tra thực tế).
*   Nếu $N = 0$, $\alpha = 0.0$ (phụ thuộc hoàn toàn vào suy luận từ các nút lân cận).
*   Nếu $0 < N < N_{threshold}$, $\alpha = \frac{N}{N_{threshold}}$ (kết hợp cả hai).
