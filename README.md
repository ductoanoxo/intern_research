# Nghiên cứu .NET 8 & Demo Minimal API

Thư mục này chứa tài liệu nghiên cứu chi tiết về **.NET 8 & C# 12** cùng ứng dụng **Demo Web API** chạy trên Docker để phục vụ giai đoạn Training/Research của Intern.

---

## 📂 Cấu trúc thư mục

```text
dotnet-learning_18-6/
├── DemoApi/                    # Dự án Demo thực hành
│   ├── DemoApi.csproj          # File dự án .NET 8, quản lý dependency (Swagger)
│   ├── Program.cs              # Code API sử dụng C# 12 & .NET 8 (TimeProvider, Primary Constructor...)
│   ├── appsettings.json        # Cấu hình log và host
│   ├── Dockerfile              # Cấu hình Docker build tối ưu (Multi-stage)
│   └── start.sh                # Shell script tự động build và chạy container
├── lythuyet_dotnet8.md         # Báo cáo lý thuyết nghiên cứu chi tiết (Markdown)
├── lythuyet_dotnet8.docx       # Báo cáo lý thuyết (File Word gốc)
└── README.md                   # Tài liệu hướng dẫn này
```

---

## 📝 Nội dung nghiên cứu chính (`lythuyet_dotnet8.md`)
Tài liệu nghiên cứu lý thuyết bao gồm 13 phần chi tiết về:
* **Hệ sinh thái .NET 8** và phân biệt vai trò của các framework con (ASP.NET Core, EF Core...).
* **Ứng dụng của .NET 8** trong các hệ thống doanh nghiệp (ERP, CRM, Core Banking...).
* **Kiến trúc hoạt động** của CLR, BCL và cơ chế biên dịch JIT vs Native AOT.
* **Các tính năng mới** nổi bật: C# 12 (Primary Constructors, Collection Expressions), TimeProvider, Keyed DI, Frozen Collections.
* **Đánh giá ưu & nhược điểm** cùng các khó khăn thực tế khi tiếp cận công nghệ.

---

## 🚀 Hướng dẫn cài đặt & Chạy ứng dụng Demo

Dự án Demo được đóng gói hoàn toàn qua **Docker**, bạn không cần cài đặt .NET SDK trực tiếp trên máy chủ của mình để chạy thử.

### 1. Khởi chạy nhanh bằng Script
Di chuyển vào thư mục dự án và cấp quyền thực thi cho file `start.sh`, sau đó chạy script:

```bash
cd DemoApi/
chmod +x start.sh
./start.sh
```

*Script này sẽ tự động thực hiện việc biên dịch Docker image (`dotnet-api:latest`) và khởi chạy container map ra cổng `8080` của máy thật.*

---

### 2. Kiểm tra & Test API

Ứng dụng demo tích hợp sẵn **Swagger** để tiện cho việc test trực quan trên browser.

* **Cách 1: Test qua Swagger UI (Trình duyệt Web)**
  Truy cập đường link: 👉 [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
  
* **Cách 2: Test nhanh qua Command Line (`curl`)**
  ```bash
  # Test Health check endpoint (Sử dụng TimeProvider của .NET 8)
  curl -i http://localhost:8080/health

  # Test Demo Collection Expression & Spread Operator (C# 12)
  curl -i http://localhost:8080/demo/collections

  # Test Demo Primary Constructor (C# 12)
  curl -i http://localhost:8080/demo/user/Toan
  ```

---

### 3. Dừng ứng dụng
Khi đã test xong, để dừng và xóa container nhằm giải phóng tài nguyên:

```bash
docker rm -f dotnet-api
```
