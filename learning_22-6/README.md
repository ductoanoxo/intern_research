# [Daily report - 22/06/2026]

* **Họ tên:** Trà Đức Toàn
* **Nội dung nghiên cứu hôm nay:**
  1. **SOLID Principles** (SRP, OCP, LSP, ISP, DIP áp dụng trong phát triển phần mềm).
  2. **Clean Architecture** (Cấu trúc phân lớp đồng tâm: Domain, Application, Infrastructure, Presentation/Api và Dependency Rule).
  3. **MediatR / Mediator Pattern** (Commands, Queries, Handlers, Pipeline Behaviors để xử lý Cross-cutting Concerns).
* **Tiến độ tự đánh giá:** 100% 
* **Issue:** none
* **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 3 keyword tại thư mục [docs/lythuyet_solid_cleanarch_mediatr.md](./docs/lythuyet_solid_cleanarch_mediatr.md).

---

# BÁO CÁO CHI TIẾT DỰ ÁN DEMO: BOOK STORE API (CLEAN ARCHITECTURE & MEDIATR)

Để áp dụng các chủ đề nghiên cứu hôm nay, một dự án **Book Store API** đã được xây dựng bằng **.NET 8** sử dụng cấu trúc **Clean Architecture** kết hợp mẫu thiết kế **Mediator (MediatR)** và tuân thủ chặt chẽ các nguyên lý **SOLID**.

## 1. Cấu trúc mã nguồn dự án (Clean Architecture Layers)

Dự án được phân tách thành 4 lớp rõ rệt trong thư mục `src/`:

* **`CleanArchitectureDemo.Domain` (Lớp lõi - Core):**
  * Không phụ thuộc vào bất kỳ thư viện hay framework bên ngoài nào.
  * Chứa thực thể `Book.cs`, enum `BookCategory.cs`, và interface `IBookDiscountStrategy.cs` kèm các class chiến lược giảm giá cụ thể.
* **`CleanArchitectureDemo.Application` (Lớp nghiệp vụ - Application Business Rules):**
  * Phụ thuộc duy nhất vào lớp Domain.
  * Chứa định nghĩa interface repository `IBookRepository.cs`.
  * Chứa các Use Case dưới dạng CQRS: Commands (`CreateBookCommand`, `ApplyDiscountCommand`), Queries (`GetBooksQuery`, `GetBookByIdQuery`), Handlers và các Validators sử dụng FluentValidation.
  * Chứa các Pipeline Behaviors xử lý tự động: `LoggingBehavior` và `ValidationBehavior` (Middleware của MediatR).
* **`CleanArchitectureDemo.Infrastructure` (Lớp hạ tầng - Infrastructure):**
  * Thực hiện kết nối SQLite Database thông qua EF Core.
  * Triển khai cụ thể repo `BookRepository.cs` và DbContext `BookStoreDbContext.cs`.
  * Chứa `DbInitializer.cs` tự động khởi tạo cơ sở dữ liệu và chèn seed data khi API khởi động.
* **`CleanArchitectureDemo.Api` (Lớp hiển thị - Presentation):**
  * Định nghĩa HTTP Endpoints thông qua `BooksController.cs`.
  * Chứa `ExceptionHandlingMiddleware.cs` để bắt các lỗi xác thực và format JSON trả về HTTP 400 Bad Request.
  * File `Program.cs` cấu hình kết nối DI giữa các lớp, bật Swagger UI và kích hoạt Middleware.

---

## 2. Minh họa các Nguyên lý SOLID trong Project

| Nguyên lý | Vị trí áp dụng trong mã nguồn | Giải thích chi tiết |
|---|---|---|
| **SRP** (Single Responsibility) | `CreateBookCommand` / `CreateBookCommandHandler` / `CreateBookCommandValidator` | Tách biệt hoàn toàn dữ liệu đầu vào, logic xử lý nghiệp vụ lưu sách, và luật xác thực dữ liệu thành các file độc lập. Mỗi class chỉ gánh vác 1 nhiệm vụ duy nhất. |
| **OCP** (Open/Closed) | `IBookDiscountStrategy` (Percentage, Fixed, NoDiscount) & MediatR behaviors | Thêm mới cách thức giảm giá sách chỉ cần tạo thêm class implements `IBookDiscountStrategy` mà không cần sửa code cũ trong `Book`. Thêm các middleware phụ trợ cho request chỉ cần viết thêm `IPipelineBehavior` mà không sửa Handler. |
| **LSP** (Liskov Substitution) | `book.ApplyDiscount(discountStrategy)` | Mọi lớp con triển khai `IBookDiscountStrategy` đều có thể truyền vào hàm tính giảm giá của sách mà không làm sai lệch hay sập luồng nghiệp vụ của hệ thống. |
| **ISP** (Interface Segregation)| `IBookRepository` | Repository chỉ định nghĩa đúng 5 phương thức CRUD cụ thể cho thực thể `Book`. Không sử dụng Generic Repository cồng kềnh chứa các hàm không cần thiết. |
| **DIP** (Dependency Inversion)| `IBookRepository` & `IBookDiscountStrategy` | Lớp Application (Handler) chỉ phụ thuộc vào interface `IBookRepository`. Lớp Domain (Book) chỉ phụ thuộc vào `IBookDiscountStrategy` (abstraction). Mọi chi tiết cụ thể như SQLite, EF Core hay công thức giảm giá phần trăm được cấu hình động từ ngoài vào qua DI container. |

---

## 3. Danh sách các API Endpoints và Kiểm thử thực tế

Dự án đã được tích hợp giao diện **Swagger UI** trực quan tại địa chỉ gốc của API:

### 3.1. Lấy danh sách sách kèm Seed Data mặc định
* **API:** `GET /api/books`
* **Mô tả:** Trả về danh sách sách hiện có. Có thể truyền query param `category` để lọc (ví dụ: `Technology`, `Fiction`, `Science`, `Biography`).
* **Kết quả trả về (JSON):**
  ```json
  [
    {
      "id": "3f6933a3-35f1-4e75-bacb-05e3c59ec312",
      "title": "Clean Architecture",
      "author": "Robert C. Martin",
      "isbn": "978-0134494166",
      "price": 35.5,
      "stock": 15,
      "category": "Technology"
    },
    {
      "id": "b4c4ad32-1185-4c35-8952-bd3934988921",
      "title": "Design Patterns",
      "author": "Erich Gamma",
      "isbn": "978-0201633610",
      "price": 49.99,
      "stock": 8,
      "category": "Technology"
    }
  ]
  ```

### 3.2. Kiểm thử Cơ chế Tự động Xác thực (FluentValidation Pipeline Behavior)
* **API:** `POST /api/books`
* **Mô tả:** Gửi yêu cầu tạo sách mới. Khi gửi dữ liệu sai quy định (ví dụ: tiêu đề trống, giá âm, ISBN sai định dạng), MediatR `ValidationBehavior` sẽ chặn request lại trước khi vào tới Handler, ném exception và được `ExceptionHandlingMiddleware` bắt lại để trả về mã lỗi 400 Bad Request sạch đẹp.
* **Request Body (Không hợp lệ):**
  ```json
  {
    "title": "",
    "author": "",
    "isbn": "123",
    "price": -10,
    "stock": -5,
    "category": 999
  }
  ```
* **Response Body (HTTP 400):**
  ```json
  {
    "Title": "Validation Failed",
    "Status": 400,
    "Errors": {
      "Title": ["Title is required."],
      "Author": ["Author is required."],
      "Isbn": ["Invalid ISBN format. Example: 978-3-16-148410-0 or 0-12-345678-9."],
      "Price": ["Price cannot be negative."],
      "Stock": ["Stock cannot be negative."],
      "Category": ["Invalid book category."]
    }
  }
  ```

### 3.3. Áp dụng Chiến lược Giảm giá (Strategy Pattern - OCP/LSP)
* **API:** `POST /api/books/{id}/apply-discount`
* **Mô tả:** Áp dụng giảm giá cho một cuốn sách cụ thể bằng chiến lược giảm giá theo phần trăm (`Percentage`) hoặc số tiền cố định (`Fixed`).
* **Request Body (Giảm giá 20%):**
  ```json
  {
    "discountType": "Percentage",
    "value": 20
  }
  ```
* **Kết quả:** Giá cuốn sách được cập nhật trong Database SQLite thông qua Domain Entity logic.

---

## 💻 HƯỚNG DẪN CHẠY DỰ ÁN TRÊN MÁY LOCAL

### Khởi động dự án bằng Docker Compose
Bạn chỉ cần mở Terminal tại thư mục `learning_22-6/` và chạy lệnh:
```bash
docker compose up -d
```
Sau đó truy cập **[http://localhost:8080](http://localhost:8080)** để mở giao diện Swagger UI và bắt đầu test trực quan các API.

Để tắt dự án và giải phóng tài nguyên:
```bash
docker compose down
```
