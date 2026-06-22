# RESEARCH: SOLID PRINCIPLES, CLEAN ARCHITECTURE & MEDIATR (MEDIATOR PATTERN)

**Họ tên:** Trà Đức Toàn  
**Ngày nghiên cứu:** 22/06/2026  
**Chủ đề:** SOLID Principles, Clean Architecture, MediatR / Mediator Pattern trong .NET 8

---

# 1. SOLID Principles (Các nguyên lý thiết kế hướng đối tượng)

SOLID là bộ 5 nguyên lý thiết kế phần mềm giúp code dễ hiểu, dễ mở rộng và dễ bảo trì hơn. Trong dự án Demo hôm nay, cả 5 nguyên lý này đều được áp dụng triệt để:

## 1.1. Single Responsibility Principle (SRP - Đơn nhiệm)
* **Khái niệm:** Một class chỉ nên giữ một trách nhiệm duy nhất và chỉ có một lý do duy nhất để thay đổi.
* **Áp dụng trong Demo:**
  * Chia tách rõ ràng giữa **Command/Query**, **Handler**, và **Validator**:
    * `CreateBookCommand`: Chỉ chứa dữ liệu đầu vào để tạo sách.
    * `CreateBookCommandValidator`: Chỉ chứa các quy tắc xác thực dữ liệu đầu vào (sử dụng FluentValidation).
    * `CreateBookCommandHandler`: Chỉ chứa logic nghiệp vụ xử lý tạo sách và lưu trữ.
  * Việc này giúp khi luật xác thực thay đổi (ví dụ: thay đổi độ dài tiêu đề), ta chỉ sửa `Validator` mà không động tới logic lưu trữ hay cấu trúc dữ liệu của request.

## 1.2. Open/Closed Principle (OCP - Mở / Đóng)
* **Khái niệm:** Class nên mở rộng cho việc phát triển thêm tính năng mới (`Open for extension`) nhưng đóng đối với việc sửa đổi mã nguồn sẵn có (`Closed for modification`).
* **Áp dụng trong Demo:**
  * **Strategy Pattern cho tính năng giảm giá (Discount Strategy):** Chúng ta định nghĩa interface `IBookDiscountStrategy` và các class cụ thể như `PercentageDiscountStrategy`, `FixedAmountDiscountStrategy`, `NoDiscountStrategy`. Khi cần thêm một loại giảm giá mới (ví dụ: VIP discount hoặc Combo discount), chúng ta chỉ cần tạo một class mới implement `IBookDiscountStrategy` mà không cần sửa đổi class `Book` hay logic tính giá cũ.
  * **MediatR Pipeline Behavior:** Việc ghi log (`LoggingBehavior`) và xác thực dữ liệu (`ValidationBehavior`) được viết dưới dạng các Middleware chạy dọc đường ống xử lý của MediatR. Khi muốn thêm tính năng kiểm tra quyền (Authorization) hay Caching, ta chỉ cần viết thêm một Behavior mới và đăng ký vào DI mà không cần sửa đổi bất kỳ Handler nào hiện có.

## 1.3. Liskov Substitution Principle (LSP - Thay thế Liskov)
* **Khái niệm:** Đối tượng của class con phải có thể thay thế hoàn toàn cho đối tượng của class cha (hoặc interface chung) mà không làm thay đổi tính đúng đắn của chương trình.
* **Áp dụng trong Demo:**
  * Phương thức `ApplyDiscount(IBookDiscountStrategy discountStrategy)` trong class `Book` nhận vào một interface `IBookDiscountStrategy`.
  * Bất kỳ chiến lược giảm giá nào (`PercentageDiscountStrategy`, `FixedAmountDiscountStrategy`, `NoDiscountStrategy`) đều hoạt động trơn tru khi truyền vào phương thức này. Chúng tuân thủ đúng hợp đồng hành vi (contract) mà interface đặt ra và không ném ra các ngoại lệ làm hỏng luồng nghiệp vụ.

## 1.4. Interface Segregation Principle (ISP - Phân tách Interface)
* **Khái niệm:** Thay vì sử dụng một interface lớn chứa tất cả các phương thức, ta nên chia nhỏ thành nhiều interface chuyên biệt để các client không bị buộc phải phụ thuộc vào các phương thức mà chúng không sử dụng.
* **Áp dụng trong Demo:**
  * Interface `IBookRepository` chỉ định nghĩa các phương thức nghiệp vụ liên quan trực tiếp đến thực thể `Book` (`GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`).
  * Chúng ta không ép buộc class repository này phải kế thừa một `IRepository<T>` khổng lồ chứa hàng chục phương thức không cần thiết (như BulkInsert, FindBySpecification...), giúp repository luôn tinh gọn, dễ implement và bảo trì.

## 1.5. Dependency Inversion Principle (DIP - Đảo ngược phụ thuộc)
* **Khái niệm:** 
  1. Các module cấp cao không nên phụ thuộc trực tiếp vào các module cấp thấp. Cả hai nên phụ thuộc vào abstractions (interfaces).
  2. Abstractions không nên phụ thuộc vào chi tiết (details). Chi tiết nên phụ thuộc vào abstractions.
* **Áp dụng trong Demo:**
  * Lớp **Application** (chứa Handler xử lý nghiệp vụ) phụ thuộc vào interface `IBookRepository` (được định nghĩa trong Application/Domain).
  * Lớp **Infrastructure** (cấp thấp hơn, xử lý DB SQLite qua EF Core) implements interface `IBookRepository`.
  * Lớp **API** đăng ký dependencies thông qua cơ chế Dependency Injection (DI) của ASP.NET Core. Lớp Handler hoàn toàn không biết gì về EF Core hay SQLite, nó chỉ tương tác với Abstraction. Nhờ đó, nếu sau này muốn đổi từ SQLite sang SQL Server hoặc MongoDB, ta chỉ cần viết một lớp Repo mới ở Infrastructure và đổi đăng ký trong DI mà không phải thay đổi một dòng code nghiệp vụ nào trong Application.

---

# 2. Clean Architecture (Kiến trúc sạch)

Clean Architecture là mô hình kiến trúc phần mềm tổ chức mã nguồn theo các lớp đồng tâm, với quy tắc bất biến: **Chiều phụ thuộc luôn hướng từ ngoài vào trong (Dependency Rule)**. Lớp bên trong hoàn toàn không biết gì về lớp bên ngoài.

```text
       ┌─────────────────────────────────────────────────────────┐
       │ Presentation / API (Controllers, Swagger, Middleware)   │
       │     └─────────────────────────────────────────────┘     │
       │     │ Infrastructure (EF Core, SQLite, Repositories)│   │
       │     │     └─────────────────────────────────┘       │   │
       │     │     │ Application (Queries, Commands, DTOs)   │   │   │
       │     │     │     └─────────────┐             │       │   │
       │     │     │     │ Domain      │             │       │   │
       │     │     │     │ (Entities)  │             │       │   │
       │     │     │     └─────────────┘             │       │   │
       │     │     └─────────────────────────────────┘       │   │
       │     └─────────────────────────────────────────────┘     │
       └─────────────────────────────────────────────────────────┘
```

## 2.1. Chi tiết các Lớp trong Dự án Demo

### Lớp Domain (Trung tâm)
* **Nhiệm vụ:** Chứa các thực thể lõi của nghiệp vụ (Entities), các enum, định nghĩa các ngoại lệ domain, và các quy tắc nghiệp vụ cốt lõi không thay đổi theo thời gian.
* **Đặc tính:** Hoàn toàn cô lập. Lớp này **không phụ thuộc** vào bất kỳ dự án nào khác, cũng như không phụ thuộc vào bất kỳ thư viện bên ngoài nào (như EF Core, Newtonsoft.Json, v.v.).
* **Trong dự án:** Thư mục `CleanArchitectureDemo.Domain` chứa:
  * Thực thể `Book.cs` (chứa các phương thức kiểm tra tính hợp lệ của dữ liệu sách, thay đổi tồn kho, áp dụng discount).
  * Enum `BookCategory.cs`.
  * Các strategy giảm giá: `IBookDiscountStrategy.cs` và các implement.

### Lớp Application
* **Nhiệm vụ:** Định nghĩa luồng nghiệp vụ của ứng dụng (Use Cases). Nó điều phối các thực thể domain để thực hiện các chức năng cụ thể của hệ thống.
* **Đặc tính:** Chỉ phụ thuộc vào lớp **Domain**. Nó định nghĩa các interface giao tiếp với hạ tầng (như Repositories) nhưng không tự cài đặt chúng.
* **Trong dự án:** Thư mục `CleanArchitectureDemo.Application` chứa:
  * `Common/Interfaces/IBookRepository.cs`: Interface để truy xuất dữ liệu.
  * `Common/Behaviors`: Các pipeline handler (`LoggingBehavior`, `ValidationBehavior`).
  * `Books/Commands` và `Books/Queries`: Các lớp Command, Query, Handler và Validator phục vụ nghiệp vụ (CQRS).
  * `DependencyInjection.cs`: Đăng ký MediatR và FluentValidation.

### Lớp Infrastructure
* **Nhiệm vụ:** Cung cấp các công nghệ và hạ tầng kỹ thuật phục vụ cho ứng dụng (Database access, File system, Email, External API integration).
* **Đặc tính:** Phụ thuộc vào lớp **Application** (để cài đặt các interface đã được định nghĩa ở đó) và lớp **Domain**.
* **Trong dự án:** Thư mục `CleanArchitectureDemo.Infrastructure` chứa:
  * `Persistence/BookStoreDbContext.cs`: DbContext của EF Core kết nối tới SQLite.
  * `Persistence/Repositories/BookRepository.cs`: Triển khai các phương thức truy vấn thực tế xuống SQLite.
  * `Persistence/DbInitializer.cs`: Tạo schema cơ sở dữ liệu và chèn seed data khi khởi động.
  * `DependencyInjection.cs`: Đăng ký DbContext và các Repositories vào DI Container.

### Lớp Presentation (API)
* **Nhiệm vụ:** Tiếp nhận request từ bên ngoài (HTTP request), phân tích và định tuyến đến các Handler tương ứng của lớp Application, sau đó định dạng kết quả trả về cho Client.
* **Đặc tính:** Lớp ngoài cùng. Phụ thuộc vào **Application** (để gửi request qua MediatR) và **Infrastructure** (để khởi tạo DI).
* **Trong dự án:** Thư mục `CleanArchitectureDemo.Api` chứa:
  * `Controllers/BooksController.cs`: Định nghĩa các API endpoints.
  * `Middleware/ExceptionHandlingMiddleware.cs`: Middleware bắt lỗi và format JSON trả về.
  * `Program.cs`: Entry point cấu hình ứng dụng, Swagger, Middleware pipeline và khởi chạy Database.

---

# 3. MediatR / Mediator Pattern (Mẫu thiết kế trung gian)

**Mediator Pattern** là mẫu thiết kế hành vi giúp giảm thiểu sự phụ thuộc trực tiếp giữa các thành phần của hệ thống. Các thành phần không giao tiếp trực tiếp với nhau mà gửi thông tin qua một đối tượng trung gian (Mediator).

Thư viện **MediatR** là thư triển khai Mediator Pattern cực kỳ phổ biến trong hệ sinh thái .NET.

## 3.1. Cơ chế hoạt động của MediatR

```text
HTTP Client ──> Controller ──> Mediator.Send(Command/Query) ──> [ Pipeline Behaviors ] ──> Handler ──> DB
                                                                      │
                                                                      ├─> LoggingBehavior
                                                                      └─> ValidationBehavior
```

1. **Request (Command/Query):** Các object chứa dữ liệu đầu vào. Command dùng để thay đổi trạng thái hệ thống (Ghi/Sửa/Xóa), Query dùng để truy vấn dữ liệu (Đọc). Chúng kế thừa interface `IRequest<TResponse>`.
2. **Handler:** Chứa logic thực thi nghiệp vụ cho một Request cụ thể. Kế thừa interface `IRequestHandler<TRequest, TResponse>`.
3. **Pipeline Behavior:** Các middleware cho MediatR (`IPipelineBehavior<TRequest, TResponse>`). Khi một Request được gửi qua `Mediator.Send()`, nó sẽ đi qua các Behavior đã đăng ký trước khi đến được Handler.

## 3.2. Lợi ích vượt trội của MediatR kết hợp với Clean Architecture

* **Decoupling (Làm lỏng liên kết):** Controllers của API không cần tiêm (inject) hàng tá các Services hay Repositories khác nhau. Nó chỉ cần tiêm duy nhất một `ISender` (MediatR) và gọi `Send()`. Bản thân Controller không cần quan tâm class nào sẽ xử lý request đó hay xử lý như thế nào.
* **Clean Code & SRP:** Mỗi Command/Query có một Handler riêng biệt. Ta không còn tình trạng một class `BookService` phình to lên hàng ngàn dòng code chứa đủ mọi loại hàm CRUD. Mỗi Handler chỉ làm duy nhất một việc.
* **Mở rộng dễ dàng với Behaviors (AOP - Aspect Oriented Programming):** Chúng ta có thể dễ dàng chèn thêm các tính năng bổ trợ (Cross-cutting Concerns) như Logging, Caching, Validation, Transaction Management, Performance Profiling thông qua Pipeline Behaviors một cách hoàn toàn tự động và an toàn.

---

# 4. Khó khăn gặp phải và Cách khắc phục

1. **Lỗi Docker Mount SQLite (Unable to open database file):**
   * *Khó khăn:* Khi mount trực tiếp file SQLite (`./bookstore.db:/app/bookstore.db`), Docker daemon tự động tạo thư mục `bookstore.db` nếu file chưa tồn tại trên host. Điều này khiến SQLite báo lỗi "unable to open database file" do đường dẫn thực tế là một thư mục chứ không phải file.
   * *Giải pháp:* Thay đổi cấu hình mount trong `docker-compose.yml` thành mount một thư mục dữ liệu `./data:/app/data` và cấu hình connection string trỏ vào file nằm trong thư mục đó (`Data Source=data/bookstore.db`). Cách làm này giúp SQLite dễ dàng tự sinh file và bảo toàn dữ liệu khi restart container.
2. **Xử lý lỗi phân quyền file khi Docker chạy với quyền root:**
   * *Khó khăn:* Trong môi trường Linux, các file do container sinh ra mặc định thuộc sở hữu của user `root`. Khi ta cần sửa đổi code hoặc cấu hình từ host, hệ thống sẽ báo lỗi `Permission denied`.
   * *Giải pháp:* Sử dụng lệnh `chown` để gán lại quyền sở hữu thư mục dự án về cho user hiện tại trên host (UID 1000, GID 1000) sau khi chạy các công cụ sinh mã nguồn tự động.
