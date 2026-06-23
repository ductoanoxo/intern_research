# BÁO CÁO LÝ THUYẾT: DDD, CQRS PATTERN & JWT AUTHENTICATION

Tài liệu này tổng hợp chi tiết lý thuyết về 3 chủ đề: **Domain-Driven Design (DDD)**, **CQRS (Command Query Responsibility Segregation)**, và **JWT (JSON Web Token) Authentication** cùng với cách chúng được áp dụng thực tế trong dự án demo.

---

## 1. Domain-Driven Design (DDD) - Thiết kế hướng miền

**Domain-Driven Design (DDD)** là một phương pháp tiếp cận thiết kế phần mềm tập trung vào việc giải quyết các bài toán nghiệp vụ phức tạp bằng cách đặt "Domain" (Miền nghiệp vụ) và logic miền làm trọng tâm của dự án.

### 1.1. Các thuật ngữ cốt lõi trong DDD

*   **Ubiquitous Language (Ngôn ngữ đồng nhất):** Ngôn ngữ chung được sử dụng thống nhất giữa chuyên gia nghiệp vụ (Domain Experts) và lập trình viên (Developers) để tránh hiểu lầm trong quá trình trao đổi và lập trình.
*   **Bounded Context (Ngữ cảnh giới hạn):** Phân chia hệ thống lớn thành các vùng nghiệp vụ độc lập mà trong đó, một thuật ngữ hay mô hình nghiệp vụ có ý nghĩa thống nhất tuyệt đối.
*   **Entity (Thực thể):** Đối tượng được xác định duy nhất bằng **Định danh (Identity - Id)** chứ không phải bởi các thuộc tính của nó. Hai thực thể có cùng các giá trị thuộc tính nhưng khác Id vẫn được coi là hai đối tượng khác nhau (ví dụ: `User`, `Book`).
*   **Value Object (Đối tượng giá trị):** Đối tượng không có định danh (Id) riêng, được định nghĩa hoàn toàn bởi các giá trị thuộc tính của nó. Hai Value Object được coi là bằng nhau nếu mọi thuộc tính của chúng bằng nhau. Value Object phải **bất biến (Immutable)** (ví dụ: `Money`, `Email`, `Address`).
*   **Aggregate Root (Gốc tổng hợp):** Một Thực thể đóng vai trò là điểm truy cập duy nhất vào một nhóm các đối tượng liên quan (Aggregate). Mọi tương tác, thay đổi trạng thái từ bên ngoài vào nhóm đối tượng này bắt buộc phải đi qua Aggregate Root để đảm bảo tính toàn vẹn dữ liệu (**Invariants**).
    *   *Ví dụ:* `Order` là một Aggregate Root quản lý danh sách các `OrderItem`. Bên ngoài không được tự ý sửa đổi `OrderItem` trực tiếp, mà phải gọi qua phương thức của `Order` (như `order.AddItem()`).
*   **Domain Event (Sự kiện miền):** Sự kiện mô tả điều gì đó quan trọng đã xảy ra trong quá trình nghiệp vụ (luôn viết ở thì quá khứ, ví dụ: `OrderPlacedEvent`, `BookPriceChangedEvent`).
*   **Domain Exception:** Các ngoại lệ liên quan đến vi phạm quy tắc nghiệp vụ trong Domain Layer.

### 1.2. Áp dụng DDD trong Dự án Demo

*   **Purity (Độ thuần khiết):** Thư mục `DddCqrsJwtDemo.Domain` hoàn toàn không phụ thuộc vào bất kỳ thư viện hay framework bên ngoài nào (kể cả MediatR hay EF Core).
*   **Base Primitives (Lớp cơ sở):**
    *   `Entity`: Định nghĩa định danh `Id` (Guid) và các toán tử so sánh `==` / `!=` dựa trên Id.
    *   `ValueObject`: Thực thi so sánh bình đẳng dựa trên các thuộc tính cấu thành (`GetEqualityComponents`).
    *   `AggregateRoot`: Kế thừa `Entity` và quản lý danh sách `IDomainEvent` nội bộ.
*   **Encapsulated Logic (Logic đóng gói):**
    *   `Book.UpdatePrice(Money newPrice)` thay đổi giá sách và tự động phát sinh `BookPriceChangedEvent`.
    *   `Order.AddItem(Guid bookId, int quantity, Money price)` kiểm tra tính hợp lệ về mặt tiền tệ trước khi thêm hàng.
    *   `Order.Place()` phát sinh `OrderPlacedEvent` để báo cho hệ thống cập nhật kho.

---

## 2. CQRS Pattern (Command Query Responsibility Segregation)

**CQRS** là mẫu thiết kế phân tách các thao tác đọc dữ liệu (**Queries**) và thao tác ghi/cập nhật dữ liệu (**Commands**) thành các mô hình và luồng xử lý riêng biệt.

```
                    ┌───────────┐
                    │   Client  │
                    └─────┬─────┘
             ┌────────────┴────────────┐
             ▼ (Writes)                ▼ (Reads)
       ┌───────────┐             ┌───────────┐
       │  Commands │             │  Queries  │
       └─────┬─────┘             └─────┬─────┘
             ▼                         ▼
      Command Handlers          Query Handlers
    (Mutates Aggregate)       (Read-only, No tracking)
             ▼                         ▼
       ┌───────────┐             ┌───────────┐
       │ Write DB  │             │  Read DB  │
       └───────────┘             └───────────┘
```

### 2.1. Tại sao cần CQRS?

*   **Tối ưu hóa hiệu năng (Performance Optimization):** Ghi dữ liệu cần tính nhất quán cao, khóa giao dịch (locking) và cập nhật Aggregate phức tạp. Trong khi đó, đọc dữ liệu cần tốc độ nhanh, không khóa, không cần theo dõi trạng thái thay đổi (No Tracking).
*   **Bảo mật tốt hơn (Security):** Dễ dàng áp dụng các chính sách kiểm tra quyền truy cập riêng cho từng Command/Query.
*   **Độc lập phát triển (Scalability):** Có thể tối ưu cơ sở dữ liệu đọc (Read Replica) và ghi (Write Replica) riêng nếu cần thiết.

### 2.2. Áp dụng CQRS trong Dự án Demo

Chúng ta sử dụng thư viện **MediatR** để triển khai CQRS trong lớp Application:

*   **Write Side (Commands):**
    *   Các command như `RegisterUserCommand`, `CreateBookCommand`, `PlaceOrderCommand` thực hiện thay đổi trạng thái hệ thống.
    *   Chúng đi qua một **MediatR Pipeline Behavior (`ValidationBehavior`)** để tự động xác thực dữ liệu đầu vào thông qua **FluentValidation** trước khi vào tới Handler.
*   **Read Side (Queries):**
    *   Các query như `GetBooksQuery`, `GetBookByIdQuery`, `GetOrdersQuery` lấy dữ liệu ra dưới dạng các DTO nhẹ nhàng (`BookDto`, `OrderDto`).
    *   Sử dụng EF Core với phương thức `.AsNoTracking()` để bypass bộ nhớ cache theo dõi thực thể của EF Core giúp truy vấn đạt hiệu năng cao nhất.

---

## 3. JWT Authentication (JSON Web Token)

**JWT (JSON Web Token)** là một tiêu chuẩn mở (RFC 7519) định nghĩa một cách nhỏ gọn và tự chứa (self-contained) để truyền tải thông tin an toàn giữa các bên dưới dạng đối tượng JSON.

### 3.1. Cấu trúc của một JWT

Một JWT gồm 3 phần phân tách nhau bởi dấu chấm (`.`): `Header.Payload.Signature`

1.  **Header:** Chứa thông tin về loại token (thường là JWT) và thuật toán mã hóa chữ ký (ví dụ: HMAC SHA256).
2.  **Payload (Claims):** Chứa các khẳng định về đối tượng (thông tin người dùng). Các claim phổ biến gồm có:
    *   `sub` (Subject - thường là User Id)
    *   `email` (Email người dùng)
    *   `unique_name` (Tên hiển thị)
    *   `role` (Vai trò để phân quyền)
3.  **Signature (Chữ ký):** Được tạo ra bằng cách lấy phần Header và Payload được mã hóa Base64, kết hợp với một mã bí mật phía Server (**Secret Key**) thông qua thuật toán băm để đảm bảo token không bị giả mạo.

### 3.2. Áp dụng JWT trong Dự án Demo

*   **Generation (Tạo Token):** Lớp `JwtProvider` trong Infrastructure sử dụng thư viện `System.IdentityModel.Tokens.Jwt` kết hợp các thuộc tính cấu hình từ `appsettings.json` (`JwtOptions`) để ký và sinh token có thời hạn 2 tiếng.
*   **Authorization Policy (Phân quyền):**
    *   `[Authorize]`: Áp dụng cho `OrdersController`, yêu cầu người dùng phải đăng nhập hợp lệ.
    *   `[Authorize(Roles = "Admin")]`: Áp dụng cho API `POST /api/books`, chỉ cho phép Admin tạo sách mới.
    *   `[Authorize(Roles = "Customer")]`: Áp dụng cho API `POST /api/orders`, chỉ cho phép tài khoản Customer đặt hàng.
*   **Decoupled Current User context (Lấy thông tin User hiện tại):**
    *   Lớp `CurrentUser` trong API triển khai interface `ICurrentUser` để lấy claims từ HttpContext một cách an toàn. Nhờ đó, lớp Application có thể lấy thông tin User Id của người dùng đang đăng nhập để xử lý nghiệp vụ (`PlaceOrderCommandHandler`) mà không bị ràng buộc trực tiếp vào ASP.NET Core `HttpContext`.
