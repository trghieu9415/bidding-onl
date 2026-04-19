# TÀI LIỆU KIẾN TRÚC VÀ LUỒNG DỮ LIỆU (ARCHITECTURE & DATA FLOW GUIDE)

Tài liệu này cung cấp cái nhìn tổng quan về kiến trúc phần mềm của dự án, cách phân tách các thành phần theo tiêu chuẩn Clean Architecture, nguyên lý Domain-Driven Design (DDD), pattern CQRS và hướng dẫn chi tiết cách phát triển một tính năng mới.

---

## 1. Phân Tách Layer & Dependency Inversion (Clean Architecture)

Dự án tuân thủ nghiêm ngặt nguyên lý Dependency Inversion. Các layer bậc cao (Application, Infrastructure) phụ thuộc vào layer bậc thấp (Core/Domain). Lớp Infrastructure tuyệt đối không chứa Business Logic và chỉ đóng vai trò triển khai (implement) các interface được định nghĩa tại Application.

### L1.Core (Domain Layer)
*   **Trách nhiệm:** Trái tim của hệ thống. Chứa toàn bộ Business Rules cốt lõi, không phụ thuộc vào bất kỳ framework hay thư viện bên ngoài nào (kể cả Entity Framework).
*   **Thành phần thực tế:** Nằm trong thư mục `L1.Core/Domain`. Chứa các `Entities`, `ValueObjects`, `Enums`, và định nghĩa các `DomainEvents`.

### L2.Application (Use Case Layer)
*   **Trách nhiệm:** Điều phối các Use Case của hệ thống. Đây là nơi chứa logic ứng dụng, xác định "Hệ thống làm được những gì".
*   **Thành phần thực tế:**
  *   CQRS Components: `Commands`, `Queries`, `Handlers` (thư mục `UseCases/`).
  *   Data Transfer Objects (`DTOs`) và `Filters` dùng cho phân trang/tìm kiếm.
  *   **Ports (Interfaces):** Định nghĩa các hợp đồng giao tiếp mà Infrastructure phải tuân thủ (ví dụ: `IRepository`, `IReadRepository`, `IAuthService`, `IGatewayFactory`).
  *   Pipeline Behaviors: `ValidationBehavior`, `TransactionBehavior`, `LockBehavior`.

### L3.Infrastructure & L3.Worker (Detail Layer)
*   **Trách nhiệm:** Triển khai (implement) các Ports được định nghĩa ở `L2.Application`. Nơi đây giao tiếp trực tiếp với Database, Cache, Message Broker, và các External APIs.
*   **Thành phần thực tế:**
  *   `AppDbContext`, `EfRepository` (truy xuất PostgreSQL).
  *   `JwtService`, `EmailService`, `StripeGateway`.
  *   `HangfireTaskQueue`, `MassTransitEventDispatcher` (nằm ở `L3.Worker`).

### L0.API (Presentation Layer)
*   **Trách nhiệm:** Điểm vào (Entry point) của ứng dụng. Nhận HTTP Requests/WebSockets, chuyển đổi dữ liệu và gửi xuống Application Layer thông qua MediatR.
*   **Thành phần thực tế:** `Controllers`, `Hubs`, `Middlewares`, `GlobalExceptionHandler`, cấu hình Response (`ApiResponse`).

---

## 2. Các Thành Phần Domain-Driven Design (DDD)

Dự án áp dụng thiết kế hướng Domain, tập trung việc bảo vệ tính toàn vẹn dữ liệu bên trong các Aggregate.

*   **Aggregate Root (Căn nguyên kết tập):** Kế thừa từ `AggregateRoot.cs`. Đây là điểm duy nhất cho phép các layer khác tương tác để thay đổi trạng thái dữ liệu.
  *   *Ví dụ:* `Auction`, `CatalogItem`, `Order`. Khi muốn thêm một `Bid` mới, bạn không thao tác trực tiếp trên repository của Bid, mà phải gọi hàm `auction.PlaceBid(...)`.
*   **Entity (Thực thể):** Kế thừa từ `BaseEntity.cs`. Chứa định danh (`Id`) và lifecycle mặc định (`CreatedAt`, `IsDeleted`).
  *   *Ví dụ:* `Bid` là một Entity nằm bên trong Aggregate `Auction`.
*   **Value Object (Đối tượng giá trị):** Là các đối tượng bất biến (Immutable), không có định danh độc lập, được so sánh bằng giá trị của các thuộc tính. Trong dự án này, Value Object được tận dụng triệt để bằng kiểu `record` của C#.
  *   *Ví dụ:* `AuctionRules`, `AuctionTimeFrame`, `Address`.
*   **Domain Event (Sự kiện miền):** Kế thừa từ `DomainEvent.cs`. Được sinh ra ngay bên trong Aggregate Root thông qua phương thức `AddDomainEvent()` khi có một thay đổi trạng thái quan trọng xảy ra.
  *   *Ví dụ:* `BidPlacedEvent`, `AuctionStartedEvent`.

---

## 3. CQRS Pattern & Luồng Dữ Liệu (Data Flow)

Hệ thống tách biệt hoàn toàn luồng Ghi (Command) và luồng Đọc (Query) để tối ưu hiệu năng và dễ bảo trì.

### Luồng Ghi (Command Data Flow)
Sử dụng `IRepository` để làm việc với Aggregate Root, đảm bảo mọi Business Rules được kiểm tra kỹ lưỡng trước khi lưu.

1.  **Controller:** Nhận HTTP Post/Put/Delete, khởi tạo `Command` (có thể implement marker interface `ITransactional` hoặc `ILockable`).
2.  **Controller -> MediatR:** Gọi `Mediator.Send(command)`.
3.  **MediatR Pipeline:**
  *   `ValidationBehavior`: Tự động chạy Validator (FluentValidation) tương ứng. Nếu lỗi, ném `InvalidInputException` (Controller trả về 422).
  *   `LockBehavior`: Nếu Command implement `ILockable`, tự động lấy Distributed Lock qua Redis để tránh Race Condition.
  *   `TransactionBehavior`: Nếu Command implement `ITransactional`, mở DB Transaction.
4.  **Handler:** Load Aggregate Root qua `IRepository`, gọi các phương thức thay đổi trạng thái bên trong Aggregate (ví dụ: `auction.PlaceBid()`), sau đó gọi `repository.UpdateAsync()`.
5.  **Event Dispatching:** `TransactionBehavior` tự động quét các Aggregate đang thay đổi, lấy ra `DomainEvents` và đẩy vào Outbox/Message Broker thông qua `IEventDispatcher`.
6.  **Transaction Commit:** Ghi dữ liệu xuống DB.

### Luồng Đọc (Query Data Flow)
Bỏ qua hoàn toàn Aggregate Root và Tracking để tối ưu tốc độ đọc.

1.  **Controller:** Nhận HTTP Get, khởi tạo `Query` (chứa các filter kế thừa từ `BaseFilter`).
2.  **Controller -> MediatR:** Gọi `Mediator.Send(query)`.
3.  **Handler:** Inject `IReadRepository<Entity, Dto>`.
4.  **Read Repository:** Sử dụng EF Core `AsNoTracking()`, kết hợp `AutoFilterer` để build WHERE clause, và `AutoMapper` (`ProjectTo`) để ánh xạ trực tiếp từ Entity sang DTO ngay tại tầng Database (SQL).
5.  **Return:** Trả về danh sách DTO và dữ liệu phân trang (`Meta`).

---

## 4. Step-by-Step Guide: Thêm mới một Use Case (API)

Dưới đây là các bước thao tác trên IDE (như JetBrains Rider) để tạo một tính năng mới. Ví dụ: Tạo API cập nhật thông tin Danh mục (UpdateCategory).

### Bước 1: Chuẩn bị Domain (L1.Core)
1. Mở file Entity (ví dụ: `Category.cs` trong `L1.Core/Domain/Catalog/Entities`).
2. Kiểm tra hoặc thêm một phương thức chứa logic cập nhật (không dùng Setter public).
   *Mẫu:* `public Category Update(string name, Guid? parentId) { ... logic kiểm tra ... return this; }`

### Bước 2: Tạo Use Case Components (L2.Application)
Di chuyển đến thư mục tính năng tương ứng (ví dụ: `L2.Application/UseCases/Categories/Commands/UpdateCategory`).

1. **Tạo file Command:** Tạo file `UpdateCategoryCommand.cs`.
  * Định nghĩa Request DTO (nếu payload lớn): `public record UpdateCategoryRequest(string Name, Guid? ParentId);`
  * Định nghĩa Command: `public record UpdateCategoryCommand(Guid Id, UpdateCategoryRequest Data) : IRequest<bool>, ITransactional;`

2. **Tạo Validator (Cùng file Command hoặc file riêng):**
  * Tạo class: `public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>`
  * Viết rule trong Constructor: `RuleFor(x => x.Data.Name).NotEmpty()...;`

3. **Tạo file Handler:** Tạo file `UpdateCategoryHandler.cs`.
  * Implement interface: `public class UpdateCategoryHandler(IRepository<Category> repository) : IRequestHandler<UpdateCategoryCommand, bool>`
  * Viết logic hàm `Handle`:
    * Dùng `GetByIdAsync` để lấy dữ liệu.
    * Ném `WorkflowException` kèm mã 404 nếu không tìm thấy.
    * Gọi phương thức `Update` của entity đã tạo ở Bước 1.
    * Gọi `repository.UpdateAsync(category, ct);`
    * Trả về kết quả.

### Bước 3: Đăng ký Endpoint (L0.API)
1. Mở Controller tương ứng (ví dụ: `CategoryController.cs` trong thư mục `Admin`).
2. Định nghĩa hàm HttpPut hoặc HttpPatch.
3. Gắn Data Annotation: `[ProducesSuccess<bool>]` (để định nghĩa type trả về cho Swagger).
4. Code logic gọi Mediator và bọc response:
   *Mẫu:*
   ```csharp
   [HttpPut("{id:guid}")]
   [ProducesSuccess<bool>]
   public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest data, CancellationToken ct) {
       var command = new UpdateCategoryCommand(id, data);
       var result = await Mediator.Send(command, ct);
       return ApiResponse.Success(result, "Cập nhật thành công");
   }
   ```

### Ghi chú về Dependency Injection (DI)
Trong kiến trúc dự án này, **bạn không cần phải đăng ký thủ công** các `Handler` hay `Validator` vào `IServiceCollection` (như file `Program.cs` thông thường).

Hệ thống đã được cấu hình tự động quét (Assembly Scanning) thông qua marker interface `IApplicationMarker`. Khi khởi chạy, phương thức `AddMediatorPipeline()` tại lớp `L3.Infrastructure` sẽ tự động tìm và đăng ký mọi thành phần liên quan đến MediatR và FluentValidation đang có trong Lớp `L2.Application`. Mọi Dependency (như Repository) được tự động Inject qua Primary Constructor.
