# Tài liệu Nghiên cứu Lý thuyết: Microservices, API Gateway, gRPC & RabbitMQ

Tài liệu này tổng hợp lý thuyết về các công nghệ cốt lõi trong hệ thống phân tán (Distributed Systems) và kiến trúc Microservices, bao gồm cổng kết nối API Gateway, giao tiếp đồng bộ hiệu năng cao với gRPC, và giao tiếp bất đồng bộ qua hàng đợi thông điệp RabbitMQ.

---

## 1. Kiến trúc Microservices (Kiến trúc Vi dịch vụ)

### 1.1. Khái niệm & Nguyên lý hoạt động
Kiến trúc Microservices (Microservices Architecture) là một hướng tiếp cận thiết kế phần mềm trong đó một ứng dụng lớn được chia nhỏ thành một tập hợp các dịch vụ (services) độc lập, có kích thước nhỏ, tập trung vào một nhiệm vụ nghiệp vụ duy nhất (Single Responsibility). 

Mỗi microservice:
*   Chạy trong tiến trình (process) riêng của nó.
*   Sở hữu cơ sở dữ liệu riêng (Database per Service) để đảm bảo tính độc lập dữ liệu (Loose Coupling).
*   Giao tiếp với các dịch vụ khác thông qua các giao thức gọn nhẹ (HTTP REST, gRPC, AMQP).
*   Có thể được phát triển bằng các ngôn ngữ lập trình và công nghệ khác nhau (Polyglot Programming/Persistence).
*   Được triển khai độc lập (Independent Deployment).

### 1.2. So sánh Monolith (Đơn khối) vs Microservices

| Tiêu chí | Monolithic Architecture | Microservices Architecture |
|---|---|---|
| **Cấu trúc** | Tất cả các module nằm chung một mã nguồn, chạy chung một tiến trình. | Tách biệt thành nhiều dịch vụ chạy trên các tiến trình/máy chủ khác nhau. |
| **Cơ sở dữ liệu** | Sử dụng chung một cơ sở dữ liệu tập trung lớn. | Mỗi dịch vụ tự quản lý DB riêng của nó (Database-per-service). |
| **Triển khai (Deployment)** | Phải build và deploy toàn bộ ứng dụng mỗi lần có thay đổi nhỏ. | Triển khai độc lập từng service mà không ảnh hưởng dịch vụ khác. |
| **Mở rộng (Scaling)** | Mở rộng bằng cách nhân bản toàn bộ ứng dụng (Scale-up/Scale-out toàn bộ). | Chỉ mở rộng những service bị quá tải hoặc cần tài nguyên chuyên biệt. |
| **Khả năng cô lập lỗi** | Một lỗi nghiêm trọng (ví dụ: tràn bộ nhớ) có thể làm sập toàn bộ hệ thống. | Lỗi ở một service chỉ làm mất tính năng đó, các service khác vẫn hoạt động. |
| **Công nghệ** | Bị giới hạn trong một stack công nghệ đồng nhất. | Tự do lựa chọn công nghệ tốt nhất cho từng bài toán cụ thể. |

### 1.3. Ưu và Nhược điểm của Microservices
*   **Ưu điểm:**
    *   **Khả năng mở rộng độc lập:** Tối ưu hóa tài nguyên phần cứng tốt hơn.
    *   **Rút ngắn thời gian release (Time to market):** Các team độc lập có thể làm việc và deploy song song mà không cần đợi nhau.
    *   **Dễ bảo trì và hiểu mã nguồn:** Mỗi service có kích thước nhỏ và tập trung sâu vào một nghiệp vụ cụ thể.
    *   **Khả năng chịu lỗi (Fault Isolation):** Hạn chế tối đa ảnh hưởng dây chuyền khi một thành phần bị lỗi.
*   **Nhược điểm:**
    *   **Độ phức tạp vận hành cao:** Đòi hỏi hạ tầng Container (Docker, Kubernetes), giám sát log tập trung (ELK, Prometheus), CI/CD phức tạp.
    *   **Nhất quán dữ liệu (Data Consistency):** Gặp khó khăn trong giao dịch phân tán (Distributed Transactions). Phải áp dụng tính nhất quán cuối cùng (Eventual Consistency) và Saga Pattern thay vì ACID truyền thống.
    *   **Độ trễ mạng (Network Latency):** Giao tiếp qua mạng liên tục giữa các service làm tăng tổng thời gian phản hồi của hệ thống.
    *   **Khó khăn khi Debug/Test:** Theo dõi luồng xử lý đi qua nhiều service (Distributed Tracing) phức tạp hơn rất nhiều.

---

## 2. API Gateway

### 2.1. Khái niệm & Vai trò
Trong kiến trúc Microservices, Client (Web/Mobile) không nên gọi trực tiếp đến từng microservice riêng lẻ vì các lý do như bảo mật, quản lý IP, giao thức bất đồng nhất và giới hạn CORS. **API Gateway** đóng vai trò là một điểm đầu mối duy nhất (Single Entry Point) đứng trước hệ thống microservices để tiếp nhận và điều hướng toàn bộ request từ Client.

```
Client (Browser/Mobile) ---> [ API Gateway ]
                                 │
         ┌───────────────────────┼──────────────────────┐
         ▼                       ▼                      ▼
  [ Order Service ]      [ Catalog Service ]    [ Identity Service ]
```

Các nhiệm vụ cốt lõi của API Gateway bao gồm:
*   **Định tuyến (Routing / Reverse Proxy):** Nhận request và chuyển hướng tới đúng microservice xử lý.
*   **Xác thực và Phân quyền (Authentication & Authorization):** Kiểm tra JWT Token tại Gateway trước khi cho phép request đi sâu vào các service nội bộ.
*   **Cân bằng tải (Load Balancing):** Phân phối tải giữa các bản sao (instances) của microservice.
*   **Giới hạn lượt gọi (Rate Limiting / Throttling):** Ngăn chặn tấn công DDoS hoặc spam API bằng cách giới hạn số request từ một IP/User trong khoảng thời gian nhất định.
*   **Biến đổi Request/Response (Transformation):** Thay đổi header, body hoặc cấu trúc dữ liệu để tương thích giữa Client và Service nội bộ.
*   **Gom luồng dữ liệu (API Aggregation):** Kết hợp kết quả từ nhiều microservice khác nhau để trả về một Response duy nhất cho Client chỉ trong 1 cuộc gọi mạng.

### 2.2. So sánh YARP và Ocelot trong hệ sinh thái .NET

*   **Ocelot:**
    *   Là thư viện API Gateway lâu đời và phổ biến nhất trong .NET.
    *   Được cấu hình chủ yếu qua file JSON (`ocelot.json`).
    *   Dễ tiếp cận nhưng hiệu năng trung bình và hiện tại ít được cập nhật tính năng mới đột phá.
*   **YARP (Yet Another Reverse Proxy):**
    *   Là dự án mã nguồn mở do chính **Microsoft** phát triển.
    *   Xây dựng trực tiếp trên hạ tầng ASP.NET Core hiệu năng cao.
    *   Tối ưu hóa sâu cho HTTP/2, HTTP/3 và gRPC.
    *   Cực kỳ linh hoạt, cho phép cấu hình bằng code C# hoặc file `appsettings.json`.
    *   **Được khuyến nghị sử dụng** cho các dự án .NET Core hiện đại nhờ tốc độ vượt trội và khả năng mở rộng mạnh mẽ.

---

## 3. gRPC (Google Remote Procedure Call)

### 3.1. Khái niệm & Cơ chế hoạt động
**gRPC** là một framework RPC (Remote Procedure Call) mã nguồn mở, hiệu năng cao do Google phát triển. Nó cho phép một ứng dụng khách (Client) gọi trực tiếp một phương thức trên một ứng dụng máy chủ (Server) đang chạy ở một máy khác giống như là gọi phương thức cục bộ trong bộ nhớ.

Các trụ cột công nghệ của gRPC:
1.  **Protocol Buffers (Protobuf):** Là ngôn ngữ định nghĩa giao diện (IDL - Interface Definition Language) và định dạng serialization dữ liệu dưới dạng nhị phân siêu nhỏ gọn, thay thế cho JSON/XML.
2.  **HTTP/2:** Giao thức truyền tải mặc định của gRPC, mang lại các tính năng vượt trội như:
    *   *Multiplexing:* Truyền nhiều request/response trên cùng 1 kết nối TCP duy nhất song song.
    *   *Header Compression (HPACK):* Nén header giảm thiểu băng thông dư thừa.
    *   *Bidirectional Streaming:* Hỗ trợ truyền dữ liệu dạng luồng (stream) hai chiều liên tục.

### 3.2. So sánh gRPC vs REST API

| Tiêu chí | gRPC | REST API |
|---|---|---|
| **Định dạng dữ liệu** | Nhị phân (Binary - Protocol Buffers) | Văn bản (Text - JSON, XML) |
| **Giao thức mạng** | Bắt buộc HTTP/2 (hoặc HTTP/3) | HTTP/1.1 hoặc HTTP/2 |
| **Khế ước (Contract)** | Nghiêm ngặt (Định nghĩa trước qua file `.proto`) | Lỏng lẻo (Mô tả qua OpenAPI/Swagger - có thể sai lệch) |
| **Độ trễ & Kích thước** | Rất thấp, payload nhị phân rất nhỏ gọn. | Cao hơn gRPC, dữ liệu JSON chứa nhiều chữ dư thừa. |
| **Kiểu truyền tải** | Request/Response, Server-streaming, Client-streaming, Bi-directional streaming. | Chủ yếu là Request/Response (Unary). |
| **Hỗ trợ Trình duyệt** | Hạn chế (Cần proxy như grpc-web để gọi từ JS/TS). | Hỗ trợ tuyệt vời trên toàn bộ trình duyệt phổ thông. |

### 3.3. Khi nào sử dụng gRPC?
gRPC cực kỳ phù hợp cho **Giao tiếp nội bộ giữa các Microservices (East-West Traffic)** nhờ tốc độ phản hồi cực nhanh, type-safe (tránh lỗi cú pháp truyền nhận) và tốn rất ít tài nguyên CPU/Băng thông. Nó không phù hợp làm API công khai trực tiếp cho các thiết bị Front-end (Web Browser) do sự phức tạp trong xử lý giao thức HTTP/2 nhị phân ở Client-side.

---

## 4. RabbitMQ & Message Broker

### 4.1. Khái niệm & Các thành phần chính
Trong kiến trúc Microservices, giao tiếp đồng bộ (gRPC, REST) có thể dẫn đến hiện tượng **chập mạch dây chuyền (coupling & cascading failures)**: nếu Service A gọi Service B đồng bộ, và Service B bị sập, Service A cũng sẽ lỗi theo. 

Để giải quyết vấn đề này, người ta dùng giao tiếp bất đồng bộ qua **Message Broker** (Môi giới thông điệp) như **RabbitMQ**.

Các thành phần chính của RabbitMQ (theo giao thức AMQP 0-9-1):
*   **Producer:** Ứng dụng gửi (publish) tin nhắn (message) lên RabbitMQ.
*   **Consumer:** Ứng dụng đăng ký lắng nghe (subscribe) và xử lý (consume) tin nhắn từ hàng đợi.
*   **Queue:** Hàng đợi lưu trữ tin nhắn tạm thời trong RAM hoặc ổ đĩa cho đến khi Consumer lấy đi.
*   **Exchange:** Bộ phận tiếp nhận tin nhắn từ Producer và quyết định sẽ đẩy tin nhắn đó vào Queue nào dựa trên các quy tắc định tuyến (Routing Key).
*   **Binding:** Mối liên kết (luật định tuyến) giữa một Exchange và một Queue.

```
[ Producer ] ──(Message)──> [ Exchange ] ──(Binding)──> [ Queue ] ──> [ Consumer ]
```

### 4.2. Các loại Exchange thông dụng
1.  **Direct Exchange:** Định tuyến tin nhắn đến Queue có `Binding Key` khớp chính xác 100% với `Routing Key` của tin nhắn. (Thích hợp cho unicast).
2.  **Fanout Exchange:** Sao chép và định tuyến tin nhắn đến **tất cả** các Queue được bind vào nó mà không cần quan tâm đến Routing Key. (Thích hợp cho dạng broadcast/pub-sub).
3.  **Topic Exchange:** Định tuyến tin nhắn dựa trên sự trùng khớp dạng wildcard giữa Routing Key và Binding Key. Ký tự `*` thay thế cho chính xác 1 từ; ký tự `#` thay thế cho 0 hoặc nhiều từ. (Thích hợp cho định tuyến thông minh/phức tạp).
4.  **Headers Exchange:** Định tuyến tin nhắn dựa trên các thuộc tính trong Header của tin nhắn thay vì Routing Key.

### 4.3. Ưu điểm của Giao tiếp Bất đồng bộ (Event-Driven)
*   **Loose Coupling (Gắn kết lỏng):** Các microservice không cần biết về sự tồn tại của nhau. Order Service chỉ cần ném sự kiện `OrderCreated` lên RabbitMQ; nó không cần biết service nào sẽ xử lý sự kiện đó.
*   **Temporal Decoupling:** Service tiêu thụ dữ liệu (Consumer) có thể đang offline khi tin nhắn được gửi. Tin nhắn sẽ xếp hàng trong Queue và sẽ được xử lý ngay khi Consumer hoạt động trở lại.
*   **San bằng tải (Load Leveling / Traffic Shaving):** Hàng đợi hoạt động như một bộ đệm. Khi hệ thống nhận lượng request đột biến, Broker giữ các message lại và Consumer sẽ tiêu thụ từ từ theo khả năng xử lý của nó, tránh sập hệ thống.

---

## 5. Tổng kết: gRPC (Đồng bộ) vs Message Broker (Bất đồng bộ)

| Tiêu chí | gRPC (Synchronous) | RabbitMQ (Asynchronous) |
|---|---|---|
| **Bản chất cuộc gọi** | Đồng bộ (Blocking/Non-blocking but expects immediate response). | Bất đồng bộ (Fire-and-forget - Gửi và quên). |
| **Phản hồi** | Trả về kết quả ngay lập tức (Request-Response). | Không phản hồi ngay, xử lý ngầm (Event-Driven). |
| **Độ tin cậy liên đới** | Cao. Server đích sập sẽ khiến Client lỗi ngay lập tức. | Thấp. Broker lưu trữ tin nhắn, Server sập thì tin nhắn vẫn an toàn trong Queue. |
| **Trường hợp áp dụng** | Truy vấn dữ liệu tức thời (Ví dụ: Order Service truy vấn giá và số lượng tồn kho thực tế của Product). | Thực hiện tác vụ ngầm kéo dài hoặc thông báo sự kiện (Ví dụ: Gửi email xác nhận, cập nhật lịch sử mua hàng, trừ kho vật lý sau khi đơn hàng thành công). |

Một hệ thống Microservices hiện đại, tối ưu luôn kết hợp cả 2 hình thức giao tiếp trên: **gRPC** cho các truy vấn lấy thông tin thời gian thực và **RabbitMQ** cho các luồng xử lý nghiệp vụ bất đồng bộ.
