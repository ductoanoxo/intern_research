# [Daily report - 23/06/2026]

* **Họ tên:** Trà Đức Toàn
* **Nội dung nghiên cứu hôm nay:**
  1. **DDD (Domain-Driven Design)**: Triển khai các khái niệm Aggregate Roots (`User`, `Book`, `Order`), Entities (`OrderItem`), Value Objects (`Email`, `Money`, `Address`), Domain Events (`OrderPlacedEvent`), và cơ chế dispatch sự kiện tự động thông qua SaveChanges EF Core.
  2. **CQRS Pattern**: Tách biệt hoàn toàn luồng xử lý ghi trạng thái (Commands) và đọc trạng thái (Queries) sử dụng MediatR. Tối ưu hóa truy vấn Read-side với `.AsNoTracking()`.
  3. **JWT Authentication**: Cấu hình cơ chế xác thực không trạng thái (stateless) bằng JSON Web Token, phân quyền người dùng dựa trên Roles (`Admin`, `Customer`), và xây dựng lớp ngữ cảnh `CurrentUser` độc lập trong lớp Application.
* **Tiến độ tự đánh giá:** 100%
* **Issue:** none
* **Mini project:** Xây dựng **Advanced Book Store API** (.NET 8 Web API, SQLite, DDD, CQRS, JWT Authentication, FluentValidation, Exception Handling Middleware).
* **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 3 keyword tại thư mục [docs/lythuyet_ddd_cqrs_jwt.md](./docs/lythuyet_ddd_cqrs_jwt.md).

---

# BÁO CÁO CHI TIẾT DỰ ÁN DEMO: ADVANCED BOOK STORE API (DDD, CQRS & JWT)

Dự án áp dụng đầy đủ kiến thức về **Domain-Driven Design (DDD)**, **CQRS** và **JWT Authentication** trong cấu trúc **Clean Architecture** sử dụng **.NET 8** và **SQLite**.

## 1. Cấu trúc mã nguồn dự án (DDD & Clean Architecture)

Dự án được tổ chức thành 4 lớp rõ rệt:

*   **`DddCqrsJwtDemo.Domain` (Core Domain Layer):**
    *   *Không phụ thuộc* vào bất kỳ thư viện hay framework ngoài nào.
    *   Chứa các primitives của DDD: `Entity.cs`, `AggregateRoot.cs`, `ValueObject.cs`, `IDomainEvent.cs`.
    *   Chứa các Aggregate Roots: `User`, `Book`, `Order`.
    *   Chứa các Entities: `OrderItem`.
    *   Chứa các Value Objects tự kiểm chứng (Self-validating): `Email`, `Money` (kèm toán tử cộng `+` và nhân), `Address`.
    *   Chứa các Domain Events: `OrderPlacedEvent` và `BookPriceChangedEvent`.
*   **`DddCqrsJwtDemo.Application` (Business Logic Layer):**
    *   Chứa các Abstractions: `IApplicationDbContext`, `IJwtProvider`, `IPasswordHasher`, `ICurrentUser`.
    *   Triển khai CQRS với MediatR:
        *   **Commands:** `RegisterUserCommand`, `LoginCommand`, `CreateBookCommand`, `PlaceOrderCommand`.
        *   **Queries:** `GetBooksQuery` (hỗ trợ search), `GetBookByIdQuery`, `GetOrdersQuery` (tự động lọc theo Role).
    *   Chứa **Domain Event Handlers**: `OrderPlacedEventHandler` thực hiện trừ kho sách (`Book.DecreaseStock()`) khi đơn đặt hàng được đặt thành công.
    *   Chứa **Validation Pipeline Behavior**: Tự động chặn các Request lỗi thông qua FluentValidation.
*   **`DddCqrsJwtDemo.Infrastructure` (Hạ tầng & Triển khai cụ thể):**
    *   DbContext SQLite: `ApplicationDbContext.cs` thực thi cơ chế **Tự động bắt và phát tán sự kiện Domain (Automatic Domain Event Dispatching)** trước khi lưu DB để chạy cùng 1 transaction.
    *   Cấu hình EF Core Fluent API mapping Value Objects (`UserConfiguration`, `OrderConfiguration`, v.v.).
    *   Triển khai `JwtProvider` (sử dụng Token Handler của .NET) và `PasswordHasher` (sử dụng thuật toán PBKDF2 SHA256 bảo mật cao không dùng thư viện ngoài).
    *   Chứa `DbInitializer` tự động cập nhật Database và nạp seed data gồm sách và tài khoản test.
*   **`DddCqrsJwtDemo.Api` (Presentation/Host Layer):**
    *   Các Controllers thực thi API endpoints: `AuthController`, `BooksController`, `OrdersController`.
    *   Lớp helper `CurrentUser.cs` giải mã claims từ JWT trong HttpContext để cung cấp thông tin định danh cho Application Layer.
    *   `ExceptionHandlingMiddleware.cs` tự động bắt lỗi Validation ném ra 400 Bad Request, lỗi logic Domain ném ra 400 và lỗi phân quyền ném ra 401.

---

## 2. Các điểm sáng Kỹ thuật (Technical Highlights)

| Chủ đề | Vị trí áp dụng | Cơ chế hoạt động & Ý nghĩa |
|---|---|---|
| **DDD Value Objects** | `Email`, `Money`, `Address` | Các thuộc tính bất biến, tự xác thực nghiệp vụ khi tạo. Ví dụ: Khởi tạo `Email` sai định dạng sẽ throw exception ngay tại Domain, ngăn ngừa dữ liệu rác. |
| **Domain Events** | `OrderPlacedEvent` -> `OrderPlacedEventHandler` | Khi đặt hàng, Aggregate `Order` nâng sự kiện đặt hàng. Trong EF Core `SaveChangesAsync`, sự kiện này được bốc ra và dispatch qua MediatR tới handler để cập nhật kho của Aggregate `Book` trong cùng 1 Db Transaction. |
| **CQRS Segregation** | Read / Write handlers | Các Query dùng `.AsNoTracking()` không sinh cache quản lý của EF Core, tối ưu hiệu năng đọc. Các Command tập trung xử lý nghiệp vụ chặt chẽ. |
| **JWT Stateless Security** | `[Authorize(Roles = "...")]` & `CurrentUser` | API được bảo mật hoàn toàn không cần Session. Sử dụng phân quyền Roles trực tiếp trên Endpoint. `CurrentUser` giúp Application Layer lấy được User Id của request mà không bị phụ thuộc vào namespace `Microsoft.AspNetCore`. |

---

## 3. Danh sách Endpoints & Hướng dẫn Test nhanh bằng Swagger

### 3.1. Các Endpoints có sẵn

1.  **Xác thực (Auth):**
    *   `POST /api/auth/register`: Đăng ký tài khoản (mặc định Role là `Customer`).
    *   `POST /api/auth/login`: Đăng nhập bằng Email và Password, trả về Token JWT và thông tin User.
2.  **Quản lý Sách (Books):**
    *   `GET /api/books`: Lấy danh sách sách (hỗ trợ query param `searchTerm` lọc theo Title/Author). *Công khai (Anonymous).*
    *   `GET /api/books/{id}`: Lấy chi tiết sách theo Id. *Công khai (Anonymous).*
    *   `POST /api/books`: Tạo sách mới (Yêu cầu Token JWT của **Admin**).
3.  **Đơn hàng (Orders):**
    *   `POST /api/orders`: Đặt đơn hàng mới (Yêu cầu Token JWT của **Customer**).
    *   `GET /api/orders`: Lấy danh sách đơn hàng (Yêu cầu Đăng nhập. Nếu là **Customer** thì chỉ xem được đơn hàng của chính mình; nếu là **Admin** thì xem được toàn bộ đơn hàng trong hệ thống).

### 3.2. Tài khoản Seed sẵn để test nhanh
*   **Tài khoản Admin:**
    *   **Email:** `admin@bookstore.com`
    *   **Password:** `adminpassword`
*   **Tài khoản Customer:**
    *   **Email:** `customer@bookstore.com`
    *   **Password:** `customerpassword`

---

## 💻 HƯỚNG DẪN CHẠY DỰ ÁN TRÊN MÁY LOCAL

### Khởi động dự án bằng Docker Compose
Mở Terminal tại thư mục `learning_23-6/` và chạy lệnh:
```bash
docker compose up --build -d
```
Sau đó truy cập **[http://localhost:8080/swagger](http://localhost:8080/swagger)** để mở giao diện Swagger UI.

### Hướng dẫn kiểm thử phân quyền JWT trên Swagger:
1.  Gọi API `POST /api/auth/login` với tài khoản test (ví dụ Admin).
2.  Copy chuỗi `token` trả về từ response JSON.
3.  Nhấp vào nút **Authorize** màu xanh ở góc trên cùng Swagger UI.
4.  Nhập vào ô value: `Bearer <chuỗi_token_vừa_copy>` (lưu ý có chữ `Bearer` và dấu cách ở giữa). Nhấn **Authorize**.
5.  Bây giờ bạn có thể gọi các API có bảo mật như `POST /api/books` hay `GET /api/orders`.

Để tắt dự án và giải phóng tài nguyên:
```bash
docker compose down
```
