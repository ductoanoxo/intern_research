# .NET Backend Stack - Intern Research & Training Repository

Chào mừng bạn đến với kho lưu trữ tài liệu nghiên cứu và thực hành của Intern về **.NET Backend Stack**. Kho lưu trữ này được tổ chức theo từng ngày học tập và nghiên cứu các công nghệ cốt lõi của Microsoft .NET 8.

---

## 📂 Danh mục các nội dung nghiên cứu (Table of Contents)

### [Ngày 1: 18/06/2026] Tìm hiểu về .NET 8 & C# 12 (Minimal API)
* **Chủ đề nghiên cứu:** Hệ sinh thái .NET 8, CLR, BCL, cơ chế biên dịch JIT/AOT, và các tính năng mới trong C# 12 (Primary Constructors, Collection Expressions...).
* **Demo Thực hành:** Xây dựng một **Minimal API** đơn giản minh họa các tính năng mới của C# 12, đóng gói Docker hoàn chỉnh.
* **Chi tiết tài liệu & hướng dẫn chạy:** [Xem chi tiết tại thư mục dotnet-learning_18-6](./dotnet-learning_18-6/README.md)

---

### [Ngày 2: 19/06/2026] ASP.NET Core Web API, LINQ & EF Core (Database First)
* **Chủ đề nghiên cứu:** Thiết kế Controller-based Web API, Dependency Injection, lập trình truy vấn dữ liệu với LINQ (Deferred/Immediate Execution, toán tử gom nhóm, liên kết nâng cao) và Entity Framework Core Database First (Scaffolding).
* **Demo Thực hành (Mini-project):** Xây dựng hệ thống quản lý danh mục, sản phẩm, và đặt hàng **E-Commerce API**. Tích hợp đầy đủ xử lý Transaction, kiểm tra tồn kho bằng Change Tracking, và xuất báo cáo thống kê nâng cao. Triển khai đồng bộ dữ liệu an toàn trên Docker qua Bind Mount.
* **Chi tiết tài liệu & hướng dẫn chạy:** [Xem chi tiết tại thư mục aspdotnet-learning_19-6](./aspdotnet-learning_19-6/README.md)

---

### [Ngày 3: 22/06/2026] SOLID Principles, Clean Architecture & MediatR / Mediator Pattern
* **Chủ đề nghiên cứu:** 5 nguyên lý thiết kế hướng đối tượng (SOLID), cấu trúc phân lớp Clean Architecture (Domain, Application, Infrastructure, Presentation) và mẫu thiết kế Mediator (MediatR) với Pipeline Behaviors trong .NET 8.
* **Demo Thực hành:** Xây dựng **Book Store API** theo kiến trúc phân lớp chuẩn chỉnh. Áp dụng Strategy Pattern cho tính năng giảm giá (minh họa OCP/LSP), thiết kế CQRS và xử lý Validation tự động qua MediatR Pipeline Behavior (FluentValidation) kết hợp Middleware xử lý ngoại lệ toàn cục.
* **Chi tiết tài liệu & hướng dẫn chạy:** [Xem chi tiết tại thư mục learning_22-6](./learning_22-6/README.md)

### [Ngày 4: 23/06/2026] Domain-Driven Design (DDD), CQRS Pattern & JWT Authentication
* **Chủ đề nghiên cứu:** Thiết kế hướng miền DDD (Aggregate Roots, Entities, Value Objects, Domain Events và cơ chế tự động dispatch sự kiện), mẫu thiết kế CQRS (tách biệt luồng đọc/ghi với MediatR, tối ưu hóa truy vấn Read-side qua `.AsNoTracking()`), và xác thực bảo mật không trạng thái bằng JWT Authentication (Authentication/Authorization Middleware, Claim-based Access Control, custom CurrentUser context).
* **Demo Thực hành:** Xây dựng **Advanced Book Store API** áp dụng đầy đủ các nguyên lý DDD và CQRS. Thực hiện cơ chế tự động bốc tách sự kiện miền trong EF Core `SaveChangesAsync` để trừ kho sách khi đặt đơn hàng trong cùng một giao dịch. Hệ thống tích hợp phân quyền chặt chẽ theo Roles (`Admin` và `Customer`) và hỗ trợ giao diện Swagger cho phép test trực quan token JWT.
* **Chi tiết tài liệu & hướng dẫn chạy:** [Xem chi tiết tại thư mục learning_23-6](./learning_23-6/README.md)

---

### [Ngày 5: 24/06/2026] RabbitMQ, gRPC, API Gateway & Microservices Architecture
* **Chủ đề nghiên cứu:** Khái niệm kiến trúc hệ thống vi dịch vụ (Microservices), cổng API Gateway định tuyến bằng YARP (Yet Another Reverse Proxy), giao tiếp đồng bộ hiệu năng cao qua gRPC HTTP/2, và giao tiếp bất đồng bộ định hướng sự kiện (Event-Driven) qua RabbitMQ sử dụng MassTransit.
* **Demo Thực hành:** Xây dựng hệ thống **Microservices Demo System** bao gồm 3 dịch vụ: `ApiGateway` (YARP), `OrderService` (gRPC Client, RabbitMQ Publisher) và `InventoryService` (gRPC Server, RabbitMQ Consumer). Thực hiện đặt đơn hàng, kiểm kho đồng bộ qua gRPC và khấu trừ kho bất đồng bộ qua RabbitMQ.
* **Chi tiết tài liệu & hướng dẫn chạy:** [Xem chi tiết tại thư mục learning_24-6](./learning_24-6/README.md)

---

## Cách chạy toàn bộ dự án trên máy Local

Bạn có thể chạy thử các dự án demo của từng ngày theo hướng dẫn cụ thể trong README.md của thư mục tương ứng. Tất cả dự án đều được đóng gói bằng Docker nên bạn chỉ cần cài đặt Docker/Docker Compose trên máy mà không cần cài đặt trực tiếp .NET SDK.

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
* **Chạy Demo Ngày 3 (Cổng 8080 - Nhớ tắt các demo khác để tránh trùng cổng):**
  ```bash
  cd learning_22-6/
  docker compose up -d
  ```
* **Chạy Demo Ngày 4 (Cổng 8080 - Nhớ tắt các demo khác để tránh trùng cổng):**
  ```bash
  cd learning_23-6/
  docker compose up --build -d
  ```
* **Chạy Demo Ngày 5 (Cổng 8080 - Nhớ tắt các demo khác để tránh trùng cổng):**
  ```bash
  cd learning_24-6/
  docker compose up --build -d
  ```

---

##  Thông tin Intern thực hiện
* **Họ tên:** Trà Đức Toàn

