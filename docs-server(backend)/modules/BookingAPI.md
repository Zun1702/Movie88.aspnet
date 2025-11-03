# 🎟 Booking Management API

## 1. Mô tả

Module Booking quản lý toàn bộ quy trình đặt vé xem phim của khách hàng, bao gồm:
- Tạo booking và chọn suất chiếu
- Chọn ghế ngồi với kiểm tra real-time availability
- Thêm combo đồ ăn/nước uống
- Áp dụng voucher và khuyến mãi
- Tính toán tổng giá tiền
- Xử lý trạng thái booking (Pending → Confirmed → Cancelled)
- Ngăn chặn đặt ghế trùng lặp (constraint UQ_ShowtimeSeat)

## 2. Danh sách Endpoint

### 2.1 Booking Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/bookings` | Danh sách tất cả bookings | Query params | List<BookingDTO> | Admin/Manager |
| GET | `/api/bookings/{id}` | Chi tiết booking | bookingId | BookingDetailDTO | Customer/Admin |
| GET | `/api/bookings/my-bookings` | Bookings của tôi | Query params | List<BookingDTO> | Customer |
| POST | `/api/bookings/create` | Tạo booking mới | CreateBookingDTO | BookingDTO | Customer |
| POST | `/api/bookings/{id}/select-seats` | Chọn ghế | SelectSeatsDTO | BookingDetailDTO | Customer |
| POST | `/api/bookings/{id}/add-combos` | Thêm combo | AddCombosDTO | BookingDetailDTO | Customer |
| POST | `/api/bookings/{id}/apply-voucher` | Áp dụng voucher | ApplyVoucherDTO | BookingDetailDTO | Customer |
| PUT | `/api/bookings/{id}/confirm` | Xác nhận booking | - | BookingDTO | Customer |
| PUT | `/api/bookings/{id}/cancel` | Hủy booking | CancelReasonDTO | Success message | Customer/Admin |
| DELETE | `/api/bookings/{id}` | Xóa booking | bookingId | Success message | Admin |

### 2.2 Combo Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/combos` | Danh sách combo | - | List<ComboDTO> | Public |
| GET | `/api/combos/{id}` | Chi tiết combo | comboId | ComboDTO | Public |
| POST | `/api/combos` | Thêm combo mới | CreateComboDTO | ComboDTO | Admin/Manager |
| PUT | `/api/combos/{id}` | Cập nhật combo | UpdateComboDTO | ComboDTO | Admin/Manager |
| DELETE | `/api/combos/{id}` | Xóa combo | comboId | Success message | Admin |

## 3. Data Transfer Objects (DTOs)

### 3.1 CreateBookingDTO
```json
{
  "showtimeId": 123,
  "customerId": 45
}
```

### 3.2 BookingDTO
```json
{
  "bookingId": 1001,
  "customerId": 45,
  "customerName": "Nguyễn Văn A",
  "showtimeId": 123,
  "movieTitle": "Avengers: Endgame",
  "cinemaName": "CGV Vincom Center",
  "auditoriumName": "Phòng 3",
  "startTime": "2025-10-30T19:30:00Z",
  "bookingTime": "2025-10-29T10:00:00Z",
  "status": "Pending",
  "totalAmount": 0,
  "voucherId": null,
  "voucherCode": null
}
```

### 3.3 BookingDetailDTO
```json
{
  "bookingId": 1001,
  "customerId": 45,
  "customerName": "Nguyễn Văn A",
  "customerEmail": "nguyenvana@example.com",
  "customerPhone": "0901234567",
  
  "showtimeInfo": {
    "showtimeId": 123,
    "movieTitle": "Avengers: Endgame",
    "posterUrl": "https://example.com/poster.jpg",
    "cinemaName": "CGV Vincom Center",
    "auditoriumName": "Phòng 3",
    "startTime": "2025-10-30T19:30:00Z",
    "endTime": "2025-10-30T22:00:00Z",
    "format": "2D",
    "languageType": "Original - Vietsub"
  },
  
  "selectedSeats": [
    {
      "seatId": 45,
      "row": "D",
      "number": 5,
      "type": "Standard",
      "price": 80000
    },
    {
      "seatId": 46,
      "row": "D",
      "number": 6,
      "type": "Standard",
      "price": 80000
    }
  ],
  
  "combos": [
    {
      "comboId": 1,
      "name": "Combo 1: Bắp + Nước",
      "quantity": 2,
      "unitPrice": 60000,
      "totalPrice": 120000
    }
  ],
  
  "voucher": {
    "voucherId": 10,
    "code": "SUMMER2025",
    "discountType": "Percent",
    "discountValue": 10,
    "discountAmount": 28000
  },
  
  "promotions": [
    {
      "promotionId": 5,
      "name": "Black Friday 20%",
      "discountAmount": 56000
    }
  ],
  
  "pricing": {
    "seatsTotal": 160000,
    "combosTotal": 120000,
    "subtotal": 280000,
    "voucherDiscount": 28000,
    "promotionDiscount": 56000,
    "totalAmount": 196000
  },
  
  "status": "Pending",
  "bookingTime": "2025-10-29T10:00:00Z"
}
```

### 3.4 SelectSeatsDTO
```json
{
  "seatIds": [45, 46]
}
```

### 3.5 AddCombosDTO
```json
{
  "combos": [
    {
      "comboId": 1,
      "quantity": 2
    },
    {
      "comboId": 3,
      "quantity": 1
    }
  ]
}
```

### 3.6 ApplyVoucherDTO
```json
{
  "voucherCode": "SUMMER2025"
}
```

### 3.7 ComboDTO
```json
{
  "comboId": 1,
  "name": "Combo 1: Bắp Lớn + Nước Ngọt",
  "description": "1 bắp rang bơ lớn + 2 nước ngọt size L",
  "price": 60000,
  "imageUrl": "https://example.com/combo1.jpg"
}
```

### 3.8 CreateComboDTO
```json
{
  "name": "Combo 2: Bắp + Nước + Snack",
  "description": "1 bắp vừa + 1 nước + 1 snack",
  "price": 75000,
  "imageUrl": "https://example.com/combo2.jpg"
}
```

## 4. Luồng xử lý (Booking Flow)

### 4.1 Complete Booking Flow

```
Step 1: Khách chọn phim và suất chiếu
├─ GET /api/movies (browse movies)
├─ GET /api/movies/{id}/showtimes (xem lịch chiếu)
└─ User chọn showtime

Step 2: Tạo booking
├─ POST /api/bookings/create
│   ├─ Input: { showtimeId, customerId }
│   ├─ Backend tạo record Bookings với Status = "Pending"
│   └─ Return: BookingDTO với bookingId

Step 3: Chọn ghế ngồi
├─ GET /api/showtimes/{id}/available-seats
│   └─ Hiển thị sơ đồ ghế (available/taken)
├─ User chọn ghế trên UI
├─ POST /api/bookings/{bookingId}/select-seats
│   ├─ Input: { seatIds: [45, 46] }
│   ├─ Backend kiểm tra:
│   │   ├─ Ghế có available không? (IsAvailable = 1)
│   │   ├─ Ghế đã được booking khác chọn chưa? (check UQ_ShowtimeSeat)
│   │   └─ Lock ghế tạm thời (15 phút)
│   ├─ Insert vào BookingSeats
│   ├─ Tính tổng tiền ghế
│   └─ Return: BookingDetailDTO

Step 4: Chọn combo (optional)
├─ GET /api/combos (hiển thị danh sách combo)
├─ User chọn combo và số lượng
├─ POST /api/bookings/{bookingId}/add-combos
│   ├─ Input: { combos: [{ comboId: 1, quantity: 2 }] }
│   ├─ Insert vào BookingCombos
│   ├─ Tính tổng tiền combo
│   └─ Return: BookingDetailDTO (updated)

Step 5: Áp dụng voucher (optional)
├─ User nhập mã voucher
├─ POST /api/bookings/{bookingId}/apply-voucher
│   ├─ Input: { voucherCode: "SUMMER2025" }
│   ├─ Backend validate voucher:
│   │   ├─ Voucher có tồn tại?
│   │   ├─ Còn hạn sử dụng?
│   │   ├─ Chưa vượt quá UsageLimit?
│   │   ├─ Đủ điều kiện MinPurchaseAmount?
│   │   └─ IsActive = true?
│   ├─ Tính discount (Percent hoặc Amount)
│   ├─ Update Bookings.VoucherId
│   └─ Return: BookingDetailDTO (với discount)

Step 6: Tự động áp dụng khuyến mãi
├─ Backend check các Promotions đang active
├─ Áp dụng các promotion phù hợp
├─ Insert vào BookingPromotions
└─ Tính tổng discount

Step 7: Xác nhận và thanh toán
├─ PUT /api/bookings/{bookingId}/confirm
│   ├─ Update Bookings.Status = "Confirmed"
│   ├─ Tính TotalAmount final
│   └─ Redirect to Payment
├─ POST /api/payments/create
│   └─ (Xem PaymentAPI.md)
└─ Sau khi thanh toán thành công:
    ├─ Update Bookings.Status = "Paid"
    ├─ Update Seats.IsAvailable = 0 (ghế đã bán)
    └─ Gửi email xác nhận + QR code
```

### 4.2 Seat Locking Mechanism

```
Purpose: Ngăn chặn nhiều user cùng đặt 1 ghế

Flow:
1. User chọn ghế → Backend lock ghế trong 15 phút
2. Nếu user không thanh toán sau 15 phút → Auto unlock ghế
3. Nếu user thanh toán thành công → Permanently mark seat as taken

Implementation:
- Thêm cột `LockedUntil` vào bảng BookingSeats
- Cronjob chạy mỗi phút để unlock ghế hết hạn
- UI hiển thị countdown timer 15 phút
```

### 4.3 Cancellation Flow

```
Khách hàng hủy vé:
├─ PUT /api/bookings/{bookingId}/cancel
├─ Input: { reason: "Lý do hủy" }
├─ Backend kiểm tra:
│   ├─ Booking thuộc về customer này?
│   ├─ Status = "Confirmed" hoặc "Paid"?
│   ├─ Thời gian hủy hợp lệ? (trước 2h so với showtime)
│   └─ Nếu đã thanh toán → trigger refund
├─ Update Bookings.Status = "Cancelled"
├─ Update Seats.IsAvailable = 1 (release ghế)
├─ Xử lý hoàn tiền (nếu cần)
└─ Return success message
```

## 5. Business Rules

### 5.1 Seat Selection Rules
- Mỗi booking tối thiểu 1 ghế, tối đa 10 ghế
- Không được chọn ghế đã được booking khác lock hoặc sold
- Ghế được lock trong 15 phút, sau đó auto-release
- Constraint `UQ_ShowtimeSeat` đảm bảo không duplicate

### 5.2 Combo Rules
- Có thể không chọn combo (optional)
- Mỗi combo quantity từ 1-10
- Giá combo cố định, không thay đổi theo suất chiếu

### 5.3 Voucher Rules
- Chỉ áp dụng được 1 voucher/booking
- Voucher phải:
  - IsActive = true
  - ExpiryDate > currentDate
  - UsedCount < UsageLimit
  - Bookings.TotalAmount >= MinPurchaseAmount
- Sau khi áp dụng → tăng Vouchers.UsedCount

### 5.4 Promotion Rules
- Có thể áp dụng nhiều promotions cùng lúc
- Áp dụng tự động nếu đủ điều kiện
- Priority: Voucher → Promotion

### 5.5 Cancellation Rules
| Thời gian hủy | Phí hủy |
|---------------|---------|
| > 24h trước showtime | Miễn phí, hoàn 100% |
| 2h - 24h trước showtime | Phí 20%, hoàn 80% |
| < 2h trước showtime | Không được hủy |

### 5.6 Status Transitions
```
Pending → Confirmed → Paid → Completed
   ↓          ↓         ↓
Expired   Cancelled  Cancelled
```

## 6. Validation Rules

### CreateBookingDTO Validation
```csharp
[Required(ErrorMessage = "ShowtimeId là bắt buộc")]
public int ShowtimeId { get; set; }

[Required(ErrorMessage = "CustomerId là bắt buộc")]
public int CustomerId { get; set; }
```

### SelectSeatsDTO Validation
```csharp
[Required]
[MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 ghế")]
[MaxLength(10, ErrorMessage = "Chỉ được chọn tối đa 10 ghế")]
public List<int> SeatIds { get; set; }
```

### AddCombosDTO Validation
```csharp
[Required]
public List<ComboItem> Combos { get; set; }

public class ComboItem
{
    [Required]
    public int ComboId { get; set; }
    
    [Range(1, 10, ErrorMessage = "Số lượng từ 1-10")]
    public int Quantity { get; set; }
}
```

## 7. Pricing Calculation Logic

```csharp
public decimal CalculateTotalAmount(BookingDetailDTO booking)
{
    // Step 1: Tính tổng tiền ghế
    decimal seatsTotal = booking.SelectedSeats.Sum(s => s.Price);
    
    // Step 2: Tính tổng tiền combo
    decimal combosTotal = booking.Combos.Sum(c => c.TotalPrice);
    
    // Step 3: Subtotal
    decimal subtotal = seatsTotal + combosTotal;
    
    // Step 4: Áp dụng voucher
    decimal voucherDiscount = 0;
    if (booking.Voucher != null)
    {
        if (booking.Voucher.DiscountType == "Percent")
        {
            voucherDiscount = subtotal * (booking.Voucher.DiscountValue / 100);
        }
        else // Amount
        {
            voucherDiscount = booking.Voucher.DiscountValue;
        }
    }
    
    // Step 5: Áp dụng promotion
    decimal promotionDiscount = 0;
    foreach (var promo in booking.Promotions)
    {
        if (promo.DiscountType == "Percent")
        {
            promotionDiscount += subtotal * (promo.DiscountValue / 100);
        }
        else
        {
            promotionDiscount += promo.DiscountValue;
        }
    }
    
    // Step 6: Total
    decimal totalAmount = subtotal - voucherDiscount - promotionDiscount;
    
    // Không được âm
    return Math.Max(totalAmount, 0);
}
```

## 8. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 400 | `INVALID_SHOWTIME` | "Suất chiếu không hợp lệ" | Showtime không tồn tại hoặc đã qua |
| 409 | `SEAT_ALREADY_TAKEN` | "Ghế đã được đặt" | Ghế đã được booking khác chọn |
| 409 | `SEAT_LOCKED` | "Ghế đang được giữ" | Ghế đang bị lock bởi booking khác |
| 400 | `INVALID_VOUCHER` | "Voucher không hợp lệ" | Voucher hết hạn hoặc đã dùng hết |
| 400 | `MIN_PURCHASE_NOT_MET` | "Chưa đủ điều kiện áp voucher" | Không đạt MinPurchaseAmount |
| 400 | `MAX_SEATS_EXCEEDED` | "Vượt quá số ghế cho phép" | Chọn > 10 ghế |
| 403 | `CANNOT_CANCEL` | "Không thể hủy vé" | Hủy quá gần giờ chiếu |
| 404 | `BOOKING_NOT_FOUND` | "Không tìm thấy booking" | Booking không tồn tại |

## 9. Performance Optimization

### 9.1 Caching Strategy
```csharp
// Cache combo list (1 hour)
Cache: "combos:list" → List<ComboDTO>

// Cache available seats per showtime (5 minutes)
Cache: "showtime:{id}:seats" → List<SeatAvailability>
```

### 9.2 Database Indexing
```sql
CREATE INDEX idx_bookings_customer ON Bookings(CustomerId);
CREATE INDEX idx_bookings_showtime ON Bookings(ShowtimeId);
CREATE INDEX idx_bookings_status ON Bookings(Status);
CREATE INDEX idx_booking_seats_showtime ON BookingSeats(ShowtimeId, SeatId);
```

### 9.3 Async Processing
- Gửi email confirmation → Background job
- Cập nhật voucher UsedCount → Background job
- Release locked seats → Cronjob

## 10. Sample API Calls

### Tạo booking
```bash
POST /api/bookings/create
Authorization: Bearer {token}
Content-Type: application/json

{
  "showtimeId": 123,
  "customerId": 45
}
```

### Chọn ghế
```bash
POST /api/bookings/1001/select-seats
Authorization: Bearer {token}
Content-Type: application/json

{
  "seatIds": [45, 46]
}
```

### Thêm combo
```bash
POST /api/bookings/1001/add-combos
Authorization: Bearer {token}
Content-Type: application/json

{
  "combos": [
    {
      "comboId": 1,
      "quantity": 2
    }
  ]
}
```

### Áp dụng voucher
```bash
POST /api/bookings/1001/apply-voucher
Authorization: Bearer {token}
Content-Type: application/json

{
  "voucherCode": "SUMMER2025"
}
```

### Xác nhận booking
```bash
PUT /api/bookings/1001/confirm
Authorization: Bearer {token}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
