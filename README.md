# .NET Backend Stack - Intern Research & Training Repository

Chào mừng bạn đến với kho lưu trữ tài liệu nghiên cứu và thực hành của Intern về **.NET Backend Stack**. Kho lưu trữ này được tổ chức theo từng ngày học tập và nghiên cứu các công nghệ cốt lõi của Microsoft .NET 8.

---

## 📂 Danh mục các nội dung nghiên cứu (Table of Contents)

### 📅 [Ngày 1: 18/06/2026] Tìm hiểu về .NET 8 & C# 12 (Minimal API)
* **Chủ đề nghiên cứu:** Hệ sinh thái .NET 8, CLR, BCL, cơ chế biên dịch JIT/AOT, và các tính năng mới trong C# 12 (Primary Constructors, Collection Expressions...).
* **Demo Thực hành:** Xây dựng một **Minimal API** đơn giản minh họa các tính năng mới của C# 12, đóng gói Docker hoàn chỉnh.
* **Chi tiết tài liệu & hướng dẫn chạy:** 👉 [Xem chi tiết tại thư mục dotnet-learning_18-6](./dotnet-learning_18-6/README.md)

---

### 📅 [Ngày 2: 19/06/2026] ASP.NET Core Web API, LINQ & EF Core (Database First)
* **Chủ đề nghiên cứu:** Thiết kế Controller-based Web API, Dependency Injection, lập trình truy vấn dữ liệu với LINQ (Deferred/Immediate Execution, toán tử gom nhóm, liên kết nâng cao) và Entity Framework Core Database First (Scaffolding).
* **Demo Thực hành (Mini-project):** Xây dựng hệ thống quản lý danh mục, sản phẩm, và đặt hàng **E-Commerce API**. Tích hợp đầy đủ xử lý Transaction, kiểm tra tồn kho bằng Change Tracking, và xuất báo cáo thống kê nâng cao. Triển khai đồng bộ dữ liệu an toàn trên Docker qua Bind Mount.
* **Chi tiết tài liệu & hướng dẫn chạy:** 👉 [Xem chi tiết tại thư mục aspdotnet-learning_19-6](./aspdotnet-learning_19-6/README.md)

---

## 🚀 Cách chạy toàn bộ dự án trên máy Local

Bạn có thể chạy thử các dự án demo của từng ngày theo hướng dẫn cụ thể trong README.md của thư mục tương ứng. Cả hai dự án đều được đóng gói bằng Docker nên bạn chỉ cần cài đặt Docker/Docker Compose trên máy mà không cần cài đặt trực tiếp .NET SDK.

* **Chạy Demo Ngày 1 (Cổng 8080):**
  ```bash
  cd dotnet-learning_18-6/DemoApi/
  chmod +x start.sh
  ./start.sh
  ```
* **Chạy Demo Ngày 2 (Cổng 8080 - Nhớ tắt demo Ngày 1 trước để tránh trùng cổng):**
  ```bash
  cd aspdotnet-learning_19-6/
  docker compose up --build -d
  ```

---

## 👤 Thông tin Intern thực hiện
* **Họ tên:** Trà Đức Toàn

