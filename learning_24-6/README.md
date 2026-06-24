# [Daily report - 24/06/2026]

*   **Họ tên:** Trà Đức Toàn
*   **Nội dung nghiên cứu hôm nay:**
    1.  **Microservices**: Chia nhỏ hệ thống thành các dịch vụ độc lập (`Order Service`, `Inventory Service`, `API Gateway`), giao tiếp liên dịch vụ (Inter-service communication) và đóng gói triển khai độc lập.
    2.  **API Gateway**: Sử dụng **YARP (Yet Another Reverse Proxy)** của Microsoft làm điểm đầu mối duy nhất để tiếp nhận request từ Client trên cổng `8080` và định tuyến động tới các microservices phía sau.
    3.  **gRPC**: Thiết kế hợp đồng giao tiếp `.proto` và triển khai cơ chế gọi hàm từ xa đồng bộ (Synchronous) hiệu năng cao qua HTTP/2 giữa `Order Service` (Client) và `Inventory Service` (Server) để kiểm tra tồn kho.
    4.  **RabbitMQ & MassTransit**: Cấu hình trao đổi thông điệp bất đồng bộ (Asynchronous Event-Driven) theo mẫu Pub-Sub. Khi đặt hàng thành công, `Order Service` phát sự kiện `OrderCreatedEvent` lên RabbitMQ; `Inventory Service` lắng nghe và tự động trừ kho vật lý.
*   **Tiến độ tự đánh giá:** 100%
*   **Issue:** Không có.
*   **Tài liệu lý thuyết:** Xem chi tiết nghiên cứu lý thuyết 4 keyword tại thư mục [docs/lythuyet_rabbitmq_grpc_api_gateway_microservices.md](./docs/lythuyet_rabbitmq_grpc_api_gateway_microservices.md).

---

# BÁO CÁO CHI TIẾT DỰ ÁN DEMO: MICROSERVICES DEMO SYSTEM

Dự án này là một mô hình thực tế thu nhỏ của kiến trúc Microservices trong hệ sinh thái **.NET 8**, áp dụng đầy đủ các kỹ thuật định tuyến qua API Gateway, giao tiếp đồng bộ gRPC, và truyền tin bất đồng bộ qua RabbitMQ.

## 1. Kiến trúc hệ thống & Luồng xử lý (Data Flow)

```
                       [ CLIENT / SWAGGER ]
                                │
                        (HTTP / Port 8080)
                                ▼
                       [ API Gateway YARP ]
                         │              │
        /api/orders/**   │              │   /api/inventory/**
        ┌────────────────┘              └────────────────┐
        ▼                                                ▼
[ Order Service ] (Port 8081)                    [ Inventory Service ] (Port 8082)
   │                                                ▲              ▲
   │ 1. gRPC: CheckStock (Sync / Port 8083)         │              │
   ├────────────────────────────────────────────────┘              │
   │                                                               │
   │ 2. RabbitMQ: Publish OrderCreatedEvent (Async)                │
   └───────────────► [ RabbitMQ Broker ] ──────────────────────────┘
                          (Port 5672)         3. Consume Event (Deduct Stock)
```

1.  **Định tuyến cuộc gọi (Routing):** Client gửi yêu cầu API thông qua Gateway (`localhost:8080`). Gateway dựa vào cấu hình YARP để định tuyến request `/api/orders/**` tới `Order Service` và `/api/inventory/**` tới `Inventory Service`.
2.  **Kiểm tra tồn kho (gRPC - Đồng bộ):** Khi Client gọi endpoint đặt hàng `POST /api/orders` tại `Order Service`:
    *   `Order Service` làm việc như một gRPC Client, tạo kết nối HTTP/2 siêu tốc tới `Inventory Service` (gRPC Server - Cổng `8083`).
    *   `Inventory Service` truy vấn SQLite database xem sản phẩm có đủ số lượng không, trả về thông tin chi tiết (trạng thái kho, tên sản phẩm, giá bán).
3.  **Lưu đơn hàng & Phát sự kiện (RabbitMQ - Bất đồng bộ):**
    *   Nếu tồn kho hợp lệ, `Order Service` ghi nhận thông tin đơn hàng vào SQLite Database của nó.
    *   Đồng thời, `Order Service` dùng **MassTransit** đẩy sự kiện `OrderCreatedEvent` lên RabbitMQ Broker rồi lập tức trả về phản hồi 200 OK cho Client (mô hình Fire-and-forget).
4.  **Cập nhật tồn kho (Event Consumption):**
    *   `Inventory Service` chạy một background Consumer lắng nghe hàng đợi trên RabbitMQ.
    *   Khi nhận được `OrderCreatedEvent`, nó tự động trừ kho số lượng tương ứng trong Database của nó và ghi log giám sát.

---

## 2. Cấu trúc mã nguồn chi tiết

*   **`src/ApiGateway`**: API Gateway sử dụng Microsoft YARP. Cấu hình định tuyến nằm tại `appsettings.json`, chạy trực tiếp trên Kestrel.
*   **`src/OrderService`**: Dịch vụ quản lý đơn hàng.
    *   `Protos/inventory.proto`: Tệp mô tả contract gRPC đóng vai trò Client.
    *   `Infrastructure/OrderDbContext.cs`: Kết nối tới CSDL SQLite lưu trữ thông tin đơn hàng (`orders.db`).
    *   `Controllers/OrdersController.cs`: Tiếp nhận request HTTP đặt hàng, thực hiện gọi gRPC và publish event.
*   **`src/InventoryService`**: Dịch vụ quản lý kho hàng.
    *   `Protos/inventory.proto`: Tệp mô tả contract gRPC đóng vai trò Server.
    *   `Services/InventoryGrpcService.cs`: Triển khai gRPC Service kế thừa từ protobuf class để phục vụ Order Service.
    *   `Consumers/OrderCreatedConsumer.cs`: Consumer nhận tin nhắn từ RabbitMQ và giảm trừ số lượng tồn kho.
    *   `Controllers/InventoryController.cs`: REST API quản lý xem kho hàng và hỗ trợ nút bấm "seed" dữ liệu ban đầu.

---

## 💻 HƯỚNG DẪN KHỞI CHẠY & KIỂM THỬ DỰ ÁN

### 1. Khởi động toàn bộ hệ thống bằng Docker Compose

Mở terminal tại thư mục `learning_24-6/` và chạy lệnh:
```bash
docker compose up --build -d
```
Lệnh này sẽ tự động tải các base image, build 3 Dockerfile của các dự án, và kéo container RabbitMQ về chạy.

> **Lưu ý:** Vui lòng đợi khoảng 10-15 giây để container RabbitMQ hoàn tất quá trình khởi tạo cấu trúc nội bộ (được kiểm soát tự động bởi `healthcheck` trước khi khởi chạy các microservices phụ thuộc).

Để kiểm tra các container đang hoạt động bình thường:
```bash
docker compose ps
```

### 2. Các cổng dịch vụ mở trên localhost (External Ports)
*   **API Gateway (Cổng chính):** `http://localhost:8080` (Mọi truy cập REST của Client nên đi qua đây).
*   **Order Service REST:** `http://localhost:8081` (Xem trực tiếp Swagger của Order Service tại `/swagger`).
*   **Inventory Service REST:** `http://localhost:8082` (Xem trực tiếp Swagger của Inventory Service tại `/swagger`).
*   **RabbitMQ Management Portal:** `http://localhost:15672` (Tài khoản: `guest` / Mật khẩu: `guest`).

---

### 3. Kịch bản kiểm thử luồng tích hợp (Step-by-Step Test)

#### Bước 3.1: Seed dữ liệu tồn kho ban đầu
Mở trình duyệt hoặc Swagger, gọi API thông qua Gateway để nạp hàng hóa vào kho:
*   **Method:** `POST`
*   **URL:** `http://localhost:8080/api/inventory/seed`
*   **Response nhận được:** Hệ thống tạo sẵn 4 sản phẩm (`laptop` - 10 cái, `mouse` - 50 cái, `keyboard` - 20 cái, `monitor` - 15 cái).

Bạn có thể kiểm tra danh sách kho hiện tại bằng cách gọi API:
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/inventory`

#### Bước 3.2: Đặt đơn hàng thành công (Kiểm tra gRPC và RabbitMQ)
Chúng ta sẽ đặt mua 2 chiếc laptop. Gọi API qua Gateway:
*   **Method:** `POST`
*   **URL:** `http://localhost:8080/api/orders`
*   **Request Body (JSON):**
    ```json
    {
      "productId": "laptop",
      "quantity": 2
    }
    ```
*   **Quá trình xử lý ngầm:**
    1.  `Order Service` gọi gRPC sang `Inventory Service` hỏi xem `laptop` có đủ 2 cái không.
    2.  `Inventory Service` phản hồi: Đủ hàng, giá là $1200/cái.
    3.  `Order Service` ghi DB đơn hàng mới trị giá $2400 và bắn sự kiện qua RabbitMQ.
*   **Response nhận được:** Phản hồi thành công kèm thông tin Order và Id ngẫu nhiên.

#### Bước 3.3: Kiểm tra sự thay đổi tồn kho (Bất đồng bộ)
Gọi lại API kiểm tra kho hàng:
*   **Method:** `GET`
*   **URL:** `http://localhost:8080/api/inventory`
*   **Kết quả:** Số lượng tồn kho của `laptop` đã tự động giảm từ **10** xuống **8** (đã được cập nhật bất đồng bộ bởi Consumer của RabbitMQ).

#### Bước 3.4: Đặt hàng thất bại do hết kho (Kiểm tra chặn gRPC)
Gọi API đặt mua thêm 10 chiếc laptop:
*   **Method:** `POST`
*   **URL:** `http://localhost:8080/api/orders`
*   **Request Body:**
    ```json
    {
      "productId": "laptop",
      "quantity": 10
    }
    ```
*   **Response nhận được:** Lỗi `400 Bad Request` với thông điệp `"Insufficient stock or product 'laptop' not found. Current stock available: 8"`. Việc đặt hàng bị chặn lại ngay lập tức tại bước kiểm tra gRPC, không có Order nào được tạo hay event nào được gửi đi.

---

### 4. Dọn dẹp tài nguyên
Để tắt và xóa toàn bộ các container cũng như mạng ảo đã tạo:
```bash
docker compose down
```
