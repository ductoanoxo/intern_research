# [Daily report - 30/06/2026]

*   **Họ tên:** Trà Đức Toàn
*   **Nội dung nghiên cứu hôm nay:**
    1.  **AI Resume Generation**: Tối ưu hóa mô tả kinh nghiệm thô của sinh viên theo mô hình STAR và công thức Google XYZ (Action + Context + Metric).
    2.  **Resume Template Generation**: Tạo mã nguồn xuất CV sang định dạng HTML chuẩn cấu trúc A4 (hỗ trợ in/tải PDF trực quan) với các Template chuyên nghiệp (Modern Slate, Minimalist).
    3.  **ATS Resume Optimization**: Chạy thuật toán TF-IDF tùy chỉnh từ đầu để trích xuất từ khóa quan trọng từ Job Description (JD), tính toán điểm tương thích ATS (0-100%) và tìm ra các từ khóa kỹ năng bị thiếu.
    4.  **Personalized Resume Generation**: Tự động tùy biến kỹ năng và định dạng CV dựa trên nghề nghiệp mục tiêu phù hợp nhất.
    5.  **Academic-to-Career Mapping**: Chạy ma trận trọng số đồ thị để ánh xạ điểm học tập (CS101, CS102, v.v.) sang các kỹ năng và năng lực công việc thực tế.
    6.  **Career Recommendation System**: Tính toán Cosine Similarity để so sánh Vector năng lực sinh viên với các Profile vị trí công việc, xếp hạng định hướng và chỉ ra khoảng cách kỹ năng (Skill Gaps).
*   **Tiến độ tự đánh giá:** 100%
*   **Mini project:** Console Resume & Career Engine 
*   **Issue:** Không có.    
*   **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 6 keyword tại [docs/theory_resume_career.md](./docs/theory_resume_career.md).

---

# BÁO CÁO DỰ ÁN DEMO: RESUME & CAREER ENGINE

Để tối giản cấu trúc mã nguồn theo hướng nghiên cứu nhanh và trực quan, dự án này đã gộp toàn bộ logic của cả 6 modules vào **một file Python duy nhất (`main.py`)** chạy độc lập trong môi trường container Docker. Chương trình tự động tính toán, hiển thị kết quả trực quan trên Terminal và ghi ra file CV HTML (`resume_output.html`) cá nhân hóa trong thư mục `src`.

## 📂 Cấu trúc thư mục tối giản
```
learning_30-6/
├── docs/
│   └── theory_resume_career.md      <- Tài liệu lý thuyết chi tiết của 6 keyword
├── src/
│   ├── main.py                      <- Toàn bộ logic chạy thử nghiệm trong 1 file duy nhất
│   ├── resume_output.html           <- Bản CV HTML cá nhân hóa xuất ra (Được sinh tự động sau khi chạy)
│   ├── Dockerfile                   <- Đóng gói container Python
│   └── requirements.txt             <- Không cần thư viện ngoài (sử dụng thư viện chuẩn của Python)
├── docker-compose.yml               <- Quản lý container dịch vụ
└── README.md                        <- Báo cáo này
```

---

## 💻 HƯỚNG DẪN KHỞI CHẠY (Chỉ 1 câu lệnh)

Mở terminal tại thư mục `learning_30-6/` và chạy lệnh sau để build và chạy Demo:

```bash
docker compose up --build simple-resume-demo
```

---

## 📊 KẾT QUẢ ĐẦU RA MONG ĐỢI TRÊN TERMINAL

Khi chạy thành công, output sẽ hiển thị như sau:

```text
============================================================================
      PATHFINDER AI: RESUME GENERATOR & CAREER PORTAL (SIMPLIFIED)
============================================================================

[Bước 1] Phân tích điểm & Gợi ý nghề nghiệp (Academic & Career System)
----------------------------------------------------------------------------
  * Điểm số học tập đầu vào (Thang điểm 10):
    - CS101 (Data Structures & Algorithms): 9.2/10
    - CS102 (Object-Oriented Programming (OOP)): 8.8/10
    - CS201 (Database Management Systems (DBMS)): 9.0/10
    - CS202 (Web Application Development): 7.0/10
    - CS301 (Operating Systems & Networking): 6.5/10
    - CS302 (Software Engineering & Devops Lab): 8.5/10
    - CS401 (Machine Learning & Data Mining): 5.0/10

  * Vector năng lực chuyên môn suy luận (Competency Profile):
    - Algorithmic Optimization       : 7.2/10 | ███████░░░
    - OOP & Design Patterns          : 8.0/10 | ████████░░
    - Database Tuning & SQL          : 7.8/10 | ████████░░
    - Frontend Tech (React/HTML/CSS) : 5.6/10 | █████░░░░░
    - Backend REST APIs (FastAPI/Node) : 7.6/10 | ███████░░░
    - System Concurrency & Network   : 5.2/10 | █████░░░░░
    - CI/CD Pipelines & Docker       : 7.7/10 | ███████░░░
    - Machine Learning & Data Math   : 4.5/10 | ████░░░░░░

  * Đề xuất nghề nghiệp phù hợp nhất (Cosine Similarity):
    1. Backend Developer            : 97.4% tương thích
       -> Lỗ hổng kỹ năng (Skill Gaps): Backend REST APIs (FastAPI/Node) (thiếu -1.9), Database Tuning & SQL (thiếu -1.2)
    2. DevOps Engineer              : 90.0% tương thích
       -> Lỗ hổng kỹ năng (Skill Gaps): CI/CD Pipelines & Docker (thiếu -1.8), System Concurrency & Network (thiếu -3.3)
    3. Frontend Developer           : 85.5% tương thích
       -> Lỗ hổng kỹ năng (Skill Gaps): Frontend Tech (React/HTML/CSS) (thiếu -3.9), OOP & Design Patterns (thiếu -0.5)

[Bước 2] Tối ưu hóa mô tả kinh nghiệm chuẩn STAR (Mục tiêu: Backend Developer)
----------------------------------------------------------------------------
  * Mô tả thô: I was responsible for writing backend endpoints and speeding up query search times.
  * STAR/XYZ:  Optimized and deployed backend endpoints and speeding up query search times utilizing FastAPI endpoints and SQL indexing, achieving a 35% faster query response time.

[Bước 3] Đánh giá độ tương thích ATS (ATS Optimization Audit)
----------------------------------------------------------------------------
  * Job Description (Yêu cầu tuyển dụng):
    "...Backend Developer... database tuning, SQL, Python, FastAPI, Docker..."

  * Kết quả Audit ATS:
    - Điểm tương thích ATS: 90/100
    - Từ khóa quan trọng trích xuất từ JD: fastapi, docker, backend, developer, database, tuning, sql, python
    - Từ khóa khớp (Matched): docker, backend, developer, database, tuning, sql, python
    - Từ khóa thiếu (Missing): fastapi

[Bước 4] Tạo bản CV Cá nhân hóa xuất ra File (Personalized Resume Template)
----------------------------------------------------------------------------
  * Thành công: Bản CV mẫu Modern Slate đã được lưu thành file:
    👉 /app/resume_output.html
    (Mở file này bằng trình duyệt của bạn để xem và in bản PDF A4 chuẩn!)
============================================================================
```
