# TÀI LIỆU ĐẶC TẢ API SIGNALR (REAL-TIME WEBSOCKETS)

Tài liệu này mô tả chi tiết giao thức giao tiếp thời gian thực giữa Server và Client sử dụng thư viện SignalR. Hệ thống thiết kế các kênh (Hub) phân tách theo mục đích sử dụng và áp dụng Notifier Pattern để push dữ liệu từ server xuống client một cách độc lập.

## 1. Thông Tin Kết Nối & Xác Thực (Connection & Authentication)

Hệ thống cung cấp 2 Endpoint (Hubs) chính:

*   **Auction Hub:** `/hubs/auction` (Dành cho các sự kiện public trong phòng đấu giá và thông báo cá nhân cho Seller).
*   **User Hub:** `/hubs/notification` (Dành cho các thông báo cá nhân bảo mật chuyên biệt cho Bidder).

**Cơ chế xác thực Token:**
SignalR không hỗ trợ truyền Authorization Header trong môi trường Browser Websockets thông thường. Do đó, hệ thống được cấu hình nhận JWT Access Token thông qua **Query Parameter**.
*   Key: `access_token`
*   URL Sample: `wss://<domain>/hubs/notification?access_token=eyJhbGciOiJIUzI1...`

## 2. Client-to-Server Invocations (Lệnh gọi từ Client)

Đối với `AuctionHub`, để nhận được các sự kiện phát ra cho toàn bộ phòng đấu giá (Group Broadcast), Client cần chủ động gọi các phương thức sau lên Server ngay khi kết nối thành công:

| Tên Method (C#) | Tham số truyền lên | Mô tả |
| :--- | :--- | :--- |
| `JoinAuction` | `string auctionId` | Tham gia vào một phòng đấu giá cụ thể để bắt đầu lắng nghe sự kiện `new-bid` và `auction-ended`. |
| `LeaveAuction` | `string auctionId` | Rời khỏi phòng đấu giá, ngưng nhận các sự kiện broadcast. |

---

## 3. Danh Sách Các Event Server Phát Ra (Server-to-Client)

Toàn bộ các sự kiện dưới đây được Server chủ động gọi (Push) xuống Client thông qua các Notifier Adapters. Dữ liệu Payload JSON mặc định tuân theo chuẩn **camelCase**.

### 3.1. Kênh Auction Hub (`/hubs/auction`)

#### A. Group Broadcast (Phát cho toàn bộ người trong phòng đấu giá)

Được quản lý bởi `SignalRAuctionNotifier`. Đối tượng nhận là những user đã gọi lệnh `JoinAuction`.

| Tên Event | Trigger (Ngữ cảnh gọi) | Payload (Mô phỏng JSON) |
| :--- | :--- | :--- |
| `new-bid` | Khi có bất kỳ người dùng nào đặt giá thành công cho sản phẩm trong phiên đấu giá. | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "currentPrice": 250000.00, "bidderName": "Nguyen Van A" } ``` |
| `auction-ended` | Khi thời gian phiên đấu giá kết thúc, hệ thống chốt kết quả. | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ``` |

#### B. Targeted User Broadcast (Phát đích danh cho Seller)

Được quản lý bởi `SignalRSellerNotifier`. Server định danh người nhận thông qua Claim `NameIdentifier` lấy từ Token.

| Tên Event | Trigger (Ngữ cảnh gọi) | Payload (Mô phỏng JSON) |
| :--- | :--- | :--- |
| `item-received-new-bid` | Khi sản phẩm do Seller đăng tải nhận được một lượt đặt giá mới. | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "newPrice": 250000.00 } ``` |
| `auction-started` | Khi phiên đấu giá chứa sản phẩm của Seller chính thức bắt đầu (Live). | ```json { "itemId": "1bc85f64-5717-4562-b3fc-2c963f66a123", "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ``` |
| `auction-finished` | Khi phiên đấu giá kết thúc và sản phẩm đã bán thành công (Current Price >= Reserve Price). | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "finalPrice": 1500000.00 } ``` |
| `auction-failed-no-bids`| Khi phiên đấu giá kết thúc nhưng sản phẩm không bán được (Không có lượt đặt giá hoặc giá chót chưa đạt). | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ``` |
| `item-approved` | Khi sản phẩm đăng ký của Seller được Admin hệ thống phê duyệt thành công. | ```json { "itemId": "1bc85f64-5717-4562-b3fc-2c963f66a123" } ``` |
| `item-rejected` | Khi sản phẩm đăng ký của Seller bị Admin hệ thống từ chối. | ```json { "itemId": "1bc85f64-5717-4562-b3fc-2c963f66a123" } ``` |

---

### 3.2. Kênh User Notification Hub (`/hubs/notification`)

Được quản lý bởi `SignalRBidderNotifier`. Kênh này yêu cầu xác thực (`[Authorize]`) và chỉ phát thông báo đích danh tới Bidder thông qua hệ thống Mapping User của SignalR.

| Tên Event | Trigger (Ngữ cảnh gọi) | Payload (Mô phỏng JSON) |
| :--- | :--- | :--- |
| `outbid` | Khi Bidder hiện tại đang giữ giá cao nhất nhưng bị một Bidder khác đặt mức giá cao hơn. | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "highestPrice": 270000.00 } ``` |
| `auction-won` | Khi phiên đấu giá kết thúc và Bidder là người trả giá cao nhất (Đã vượt qua Reserve Price). | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ``` |
| `auction-lost` | Khi phiên đấu giá kết thúc và Bidder tham gia nhưng không phải là người trả giá cao nhất. | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" } ``` |
| `payment-success` | Khi hệ thống xử lý Webhook từ cổng thanh toán (Stripe/PayPal) thành công cho đơn hàng của Bidder. | ```json { "orderId": "5da85f64-8888-4562-b3fc-2c963f66a456" } ``` |
| `payment-failed` | Khi hệ thống ghi nhận giao dịch lỗi hoặc thanh toán thất bại từ Gateway. | ```json { "orderId": "5da85f64-8888-4562-b3fc-2c963f66a456", "reason": "Insufficient funds or card declined" } ``` |
| `auction-finished` | (Gửi đến Seller trên UserHub) Cảnh báo phiên đấu giá kết thúc thành công với thông tin giá chốt. | ```json { "auctionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "finalPrice": 1500000.00 } ``` |
