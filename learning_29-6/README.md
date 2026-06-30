# [Daily report - 29/06/2026]

*   **Họ tên:** Trà Đức Toàn
*   **Nội dung nghiên cứu hôm nay:**
    1.  **Student Academic Data Modeling:** Mô hình hóa các thực thể học tập cơ bản: Học viên (`Student`), Kỹ năng (`Skill`), Quan hệ tiên quyết (`Prerequisite`), Điểm số (`Attempt`) và Trạng thái tri thức (`SkillState`).
    2.  **Skill Mapping:** Thiết lập bản đồ kỹ năng dạng đồ thị có hướng (DAG) phụ thuộc: *Basics* $\rightarrow$ *OOP* $\rightarrow$ *API Dev*, và ánh xạ điểm từ bài thi/lab vào kỹ năng đích tương ứng.
    3.  **Competency Profile Generation:** Tổng hợp các kỹ năng chi tiết thành các nhóm năng lực vĩ mô (Practical, Cognitive) biểu diễn dưới dạng phần trăm thấu hiểu trung bình của học viên.
    4.  **Skill Inference:** Suy luận kỹ năng ẩn hai chiều tự động:
        *   *Forward (Tiến):* Ước lượng độ sẵn sàng học môn nâng cao dựa trên kết quả môn cơ bản.
        *   *Backward (Lùi):* Suy luận ngầm định học viên đã thấu hiểu môn nền tảng khi họ làm bài thi nâng cao đạt điểm cao (ngay cả khi chưa làm bài kiểm tra trực tiếp của môn nền tảng đó).
*   **Tiến độ tự đánh giá:** 100%
*   **Mini project:** Simple Console Competency Inference Demo (Mã nguồn gọn nhẹ trong 1 file duy nhất `Program.cs`).
*   **Issue:** Không có.
*   **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 4 keyword tại thư mục [docs/theory_student_competency.md](./docs/theory_student_competency.md).

---

# BÁO CÁO DỰ ÁN DEMO ĐƠN GIẢN: SIMPLE COMPETENCY ANALYTICS

Để phục vụ nghiên cứu nhanh trong 1 ngày, dự án này đã tối giản hóa tối đa cấu trúc mã nguồn thành một chương trình **Console C# duy nhất (Single-file `Program.cs`)** chạy trong môi trường Docker độc lập.

## Cấu trúc thư mục Demo đơn giản
```
learning_29-6/
├── README.md                          <- Báo cáo này
├── docs/
│   └── theory_student_competency.md   <- Tài liệu lý thuyết chi tiết
└── src/
    └── SimpleConsoleDemo/
        ├── Program.cs                 <- Toàn bộ logic demo nằm tại đây (dưới 200 dòng code)
        ├── SimpleConsoleDemo.csproj   <- File cấu hình project .NET 8
        └── Dockerfile                 <- Docker đóng gói để chạy không cần cài .NET SDK
```

---

## 💻 HƯỚNG DẪN KHỞI CHẠY (Chỉ 1 câu lệnh)

Mở terminal tại thư mục `learning_29-6/src/` và chạy lệnh sau để build và chạy riêng Demo Console:

```bash
docker compose up --build simple-console-demo
```

Hệ thống sẽ biên dịch và in trực tiếp toàn bộ kết quả phân tích & suy luận thuật toán ra màn hình Terminal của bạn.

---

## 🔍 Giải thích Logic dòng Code trong `Program.cs`

Mã nguồn [Program.cs](./src/SimpleConsoleDemo/Program.cs) chia làm 4 bước rõ ràng:

1.  **Academic Data Modeling (Mô hình dữ liệu):**
    Định nghĩa các Class đơn giản để lưu trữ thông tin:
    ```csharp
    public class Student { string Code, string Name }
    public class Skill { string Code, string Name, string Domain }
    public class Prerequisite { string ParentCode, string ChildCode, double Weight }
    public class Attempt { string StudentCode, string TargetSkillCode, double Score }
    ```
2.  **Skill Mapping (Bản đồ tiên quyết):**
    Thiết lập cấu trúc tiên quyết tuyến tính:
    *   **Lập trình Cơ bản (SK-001)** $\rightarrow$ **OOP (SK-002)** $\rightarrow$ **API Dev (SK-003)**
3.  **Kịch bản dữ liệu kiểm thử (Mô phỏng):**
    *   Học viên **Trần Thị B** chưa từng làm bài thi nào về Lập trình Cơ bản (`SK-001`) và OOP (`SK-002`).
    *   Nhưng học viên B làm bài thực hành **Lab Web API (`SK-003`) đạt 90% (0.90)**.
4.  **Skill Inference (Suy luận lan truyền):**
    *   **Tính toán trực tiếp:** `SK-003` nhận điểm trực tiếp 90% (Confidence = 1.0).
    *   **Lan truyền lùi (Backward):** Vì học viên làm tốt API (`SK-003`), hệ thống suy luận ngược về `SK-002` (OOP) và `SK-001` (Basics) đạt $90\% \times \text{Trọng số} \approx 72\%$, dù học viên chưa từng thi 2 môn đó.

---

## 📊 KẾT QUẢ ĐẦU RA MONG ĐỢI TRÊN TERMINAL

Khi chạy thành công, output sẽ hiển thị như sau:

```text
==========================================================================
 DEMO NGHIÊN CỨU: STUDENT COMPETENCY MODELING & INFERENCE (SINGLE-FILE)
==========================================================================

[Đầu vào] Học viên: Trần Thị B (SV002)
 Nhật ký làm bài thực tế:
   - Bài LAB-API đánh giá trực tiếp kỹ năng SK-003 đạt: 90%

 -> Chú ý: Học viên chưa có bất kỳ điểm đánh giá trực tiếp nào cho SK-001 & SK-002.

--- Đang thực hiện tính toán & suy luận kỹ năng ẩn... ---

 KẾT QUẢ SUY LUẬN KỸ NĂNG ẨN:
--------------------------------------------------------------------------
 Mã Kỹ Năng | Tên Kỹ Năng               | Trực tiếp | Suy luận | Hợp nhất (Cuối)
--------------------------------------------------------------------------
 SK-001     | Lập trình Cơ bản (Basics) |      0.0% |    72.0% |         72.0%
 SK-002     | Lập trình Hướng đối tượng (OOP) |      0.0% |    72.0% |         72.0%
 SK-003     | Phát triển Web API (API Dev) |     90.0% |    61.2% |         90.0%
--------------------------------------------------------------------------
 GIẢI THÍCH LOGIC:
 * Học viên đạt 90% ở môn API Dev (SK-003). Hệ thống chạy suy luận lùi (Backward Inference)
   và xác định học viên đạt 72% ở môn OOP (SK-002) và 72% ở môn Basics (SK-001)
   dù học viên này chưa làm bài đánh giá trực tiếp nào cho 2 kỹ năng đó.

 HỒ SƠ NĂNG LỰC ĐẦU RA (COMPETENCY PROFILE):
--------------------------------------------------------------------------
 - Trục Năng lực Practical   :  81.0% [Thành thạo (Mastered)]
 - Trục Năng lực Cognitive   :  72.0% [Đang phát triển (Developing)]
--------------------------------------------------------------------------
 KHUYẾN NGHỊ HỌC TẬP (Pedagogical Guidance):
 -> [Đề xuất]: Kỹ năng nền tảng 'Lập trình Cơ bản' chưa đạt mức giỏi (hiện tại 72%). Học viên nên bổ sung thêm bài tập để làm chủ hoàn toàn.
==========================================================================
```
