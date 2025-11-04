# 🎟️ Staff: Xác thực Booking Online (3 Endpoints)

**Status**: ⚠️ **PENDING IMPLEMENTATION** (0/3 endpoints - 0%)  
**Assigned**: Việt

> **📱 Online-Only Flow**: Khách hàng đặt vé online → Thanh toán thành công → Nhận BookingCode (QR) → Staff verify & check-in

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Use Case | Auth | Status | Assign |
|---|--------|----------|----------|------|--------|--------|
| 1 | GET | `/api/bookings/verify/{bookingCode}` | Verify booking code/QR | ✅ Staff | ⏳ TODO | Việt |
| 2 | PUT | `/api/bookings/{id}/check-in` | Check-in customer | ✅ Staff | ⏳ TODO | Việt |
| 3 | GET | `/api/bookings/today` | View today's bookings | ✅ Staff | ⏳ TODO | Việt |

---

## 🎯 Vai trò của Staff

**Bạn là nhân viên xác thực vé online** tại rạp chiếu phim Movie88. Nhiệm vụ chính:

### ✅ Quyền hạn
- ✅ Verify booking code/QR code (sau khi khách đặt vé online)
- ✅ Xem thông tin booking đã thanh toán
- ✅ Check-in khách hàng đã có booking

### ❌ Không có quyền
- ❌ Tạo booking mới (chỉ có online booking)
- ❌ Sửa/xóa booking
- ❌ Hoàn tiền (cần Admin)
- ❌ Quản lý phim/rạp/suất chiếu
- ❌ Bán vé tại quầy (không có nghiệp vụ này)

---

## 📱 Online Booking Flow (Nghiệp vụ chính)

> **🎯 QUAN TRỌNG**: Hệ thống Movie88 chỉ xử lý **online booking**. Không có nghiệp vụ mua vé tại quầy.

### Complete Customer Journey

```
┌─────────────────────────────────────────────────────────────────┐
│                    ONLINE BOOKING FLOW                          │
└─────────────────────────────────────────────────────────────────┘

1️⃣ CUSTOMER (Tại nhà/bất kỳ đâu)
   ├─ Mở app/website Movie88
   ├─ Chọn phim, rạp, suất chiếu, ghế
   ├─ Tạo booking → Booking.Status: "Pending"
   ├─ Tạo payment record → Payment.Status: "Pending"
   └─ Booking chưa có BookingCode (chưa generate)

2️⃣ PAYMENT (Online)
   ├─ Thanh toán qua VNPay/MOMO
   ├─ Payment Gateway xác nhận thành công
   └─ Webhook cập nhật: Payment.Status: "Completed"

3️⃣ SYSTEM (Tự động)
   ├─ Phát hiện payment.Status = "Completed"
   ├─ Generate BookingCode: BK20251104001
   ├─ Update Booking.Status: "Confirmed"
   ├─ Generate QR Code chứa BookingCode
   ├─ Gửi email/SMS/notification cho khách
   └─ Khách nhận được: QR Code + BookingCode

4️⃣ CUSTOMER (Đến rạp)
   ├─ Mở app/email để lấy QR Code
   └─ Show QR cho staff tại quầy check-in

5️⃣ STAFF (Tại rạp) ← YOUR ROLE
   ├─ Scan QR hoặc nhập BookingCode
   ├─ Call API: GET /api/bookings/verify/{bookingCode}
   ├─ Kiểm tra: Payment.Status = "Completed" ✅ (via Booking.Payments collection)
   ├─ Xác nhận thông tin: Tên, phim, giờ, ghế
   ├─ Call API: PUT /api/bookings/{id}/check-in
   ├─ Update Booking.Status: "CheckedIn"
   └─ Hướng dẫn khách vào rạp

6️⃣ CUSTOMER
   └─ Vào rạp xem phim 🎬
```

### 🔒 Security Rules

| Rule | Description |
|------|-------------|
| ✅ **Rule 1** | BookingCode chỉ được generate **SAU KHI** thanh toán thành công |
| ✅ **Rule 2** | Chỉ booking có `Payment.Status = "Completed"` mới được verify (check via Booking.Payments collection) |
| ✅ **Rule 3** | Không có nghiệp vụ "mua vé tại quầy" |
| ✅ **Rule 4** | Staff chỉ verify & check-in, không tạo booking mới |
| ✅ **Rule 5** | Mỗi booking chỉ được check-in **1 lần** (Booking.Status = "CheckedIn") |

---

## 🎯 1. GET /api/bookings/verify/{bookingCode}

**Use Case**: Verify booking code/QR after online payment  
**Auth Required**: ✅ Staff/Admin  
**Status**: ⏳ TODO

### Workflow Timeline

| Step | Action | Duration |
|------|--------|----------|
| 1 | Khách đến rạp với QR code (đã thanh toán online) | 5s |
| 2 | Staff scan QR hoặc nhập booking code | 10s |
| 3 | System verify & hiển thị thông tin booking | 2s |
| 4 | Staff kiểm tra: Tên khách, phim, giờ chiếu, ghế | 15s |
| 5 | Xác nhận & check-in khách hàng | 5s |
| 6 | Hướng dẫn khách vào rạp | 5s |
| **Total** | **Complete workflow** | **~40s** |

### Request
```http
GET /api/bookings/verify/BK20251104001
Authorization: Bearer {staff_token}
```

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| bookingCode | string | ✅ | Unique booking code (e.g., BK20251104001) |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Booking verified successfully",
  "data": {
    "bookingId": 12345,
    "bookingCode": "BK20251104001",
    "status": "Confirmed",
    "bookingDate": "2025-11-01T14:30:00",
    "customer": {
      "customerId": 456,
      "fullname": "Nguyen Van A",
      "email": "nguyenvana@example.com",
      "phone": "0901234567"
    },
    "movie": {
      "movieId": 789,
      "title": "Avengers: Endgame",
      "posterUrl": "https://image.tmdb.org/t/p/w500/or06FN3Dka5tukK1e9sl16pB3iy.jpg",
      "duration": 181,
      "rating": "PG-13"
    },
    "showtime": {
      "showtimeId": 101,
      "startTime": "2025-11-04T19:30:00",
      "endTime": "2025-11-04T22:31:00",
      "cinema": {
        "cinemaId": 1,
        "name": "CGV Vincom Center",
        "address": "72 Le Thanh Ton, District 1, HCMC"
      },
      "auditorium": {
        "auditoriumId": 3,
        "name": "Cinema 3",
        "totalSeats": 150
      },
      "format": "2D",
      "language": "Phụ đề Việt"
    },
    "seats": [
      {
        "seatId": 205,
        "row": "A",
        "number": 5,
        "type": "Standard"
      },
      {
        "seatId": 206,
        "row": "A",
        "number": 6,
        "type": "Standard"
      }
    ],
    "pricing": {
      "ticketPrice": 90000,
      "numberOfTickets": 2,
      "subtotal": 180000,
      "discount": 20000,
      "totalAmount": 160000
    },
    "payment": {
      "status": "Completed",
      "paymentMethod": "VNPay",
      "transactionCode": "20251104143500",
      "paidAt": "2025-11-01T14:35:00"
    },
    "bookingStatus": "Confirmed",
    "canCheckIn": true
  }
}
```

### Response 404 Not Found
```json
{
  "success": false,
  "statusCode": 404,
  "message": "Booking code not found",
  "errors": [
    "The booking code 'BK20251104999' does not exist in the system"
  ]
}
```

### Response 400 Bad Request (Already Checked In)
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Booking already checked in",
  "errors": [
    "This booking was already checked in at 18:45 on 2025-11-04"
  ]
}
```

### Response 400 Bad Request (Payment Not Completed)
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Payment not completed",
  "errors": [
    "This booking has not been paid yet. No completed payment found in Payments collection."
  ]
}
```

> **🔒 Security Rule**: Chỉ booking có `Payment.Status = "Completed"` (trong collection Booking.Payments) mới được phép verify và check-in.

### Related Entities

**Booking** (bookings table):
- ✅ `bookingid` (int, PK)
- ✅ `bookingcode` (string?, max 20) - Generated after payment success
- ✅ `customerid` (int, FK → customers)
- ✅ `showtimeid` (int, FK → showtimes)
- ✅ `voucherid` (int?, nullable, FK → vouchers)
- ✅ `totalamount` (decimal(10,2)?, nullable)
- ✅ `status` (string?, max 50) - "Pending", "Confirmed", "CheckedIn", "Cancelled", "Completed", "Expired"
- ✅ `bookingtime` (timestamp without time zone, nullable)
- ✅ Navigation: `ICollection<Payment> Payments` - **Use this to check payment status**
- ❌ NO `paymentstatus` field - Payment status is in separate Payment table
- ❌ NO `checkedinstatus` field - Use Booking.Status = "CheckedIn" instead

**Payment** (payments table) - **SEPARATE TABLE**:
- ✅ `paymentid` (int, PK)
- ✅ `bookingid` (int, FK → bookings)
- ✅ `customerid` (int, FK → customers)
- ✅ `methodid` (int, FK → paymentmethods)
- ✅ `amount` (decimal(10,2))
- ✅ `status` (string?, max 50) - **"Pending", "Completed", "Failed"**
- ✅ `transactioncode` (string?, max 255) - VNPay/MOMO transaction ID
- ✅ `paymenttime` (timestamp without time zone, nullable)
- ✅ Relationship: Booking → ICollection<Payment> (1:N)

**Showtime** (showtimes table):
- ✅ `showtimeid` (int, PK)
- ✅ `movieid` (int, FK)
- ✅ `auditoriumid` (int, FK)
- ✅ `starttime` (DateTime)
- ✅ `endtime` (DateTime)

**Movie** (movies table):
- ✅ `movieid` (int, PK)
- ✅ `title` (string)
- ✅ `posterurl` (string)
- ✅ `durationminutes` (int)

### Implementation Plan
- ⏳ Domain: BookingVerifyDTO.cs
- ⏳ Application: IBookingVerificationService.cs
- ⏳ Infrastructure: Booking verification queries
- ⏳ WebApi: BookingsController.VerifyBookingCode()

---

## 🎯 2. PUT /api/bookings/{id}/check-in

**Use Case**: Check-in customer at counter  
**Auth Required**: ✅ Staff/Admin  
**Status**: ⏳ TODO

### Request
```http
PUT /api/bookings/12345/check-in
Authorization: Bearer {staff_token}
Content-Type: application/json

{
  "checkinTime": "2025-11-04T19:15:00",
  "notes": "Checked in by staff at counter"
}
```

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | int | ✅ | Booking ID |

### Request Body
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| checkinTime | DateTime | ✅ | Check-in timestamp |
| notes | string | ❌ | Optional notes (e.g., "Late arrival") |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Check-in successful",
  "data": {
    "bookingId": 12345,
    "bookingCode": "BK20251104001",
    "status": "CheckedIn",
    "checkedInAt": "2025-11-04T19:15:00",
    "checkedInBy": {
      "staffId": 42,
      "staffName": "Tran Thi B"
    }
  }
}
```

### Response 400 Bad Request
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Booking already checked in",
  "errors": ["Cannot check-in twice"]
}
```

### Related Entities
**Booking** (bookings table):
- ✅ Update `status` = "CheckedIn"
- ✅ Log check-in timestamp in response DTO
- ✅ Log staff who performed check-in (via authentication context)

> **💡 Note**: Current DB schema doesn't have `checkedintime` column. 
> We track check-in status via `Booking.Status = "CheckedIn"`. 
> If detailed check-in audit needed, consider adding columns: `checkedintime`, `checkedinby`.

### Implementation Plan
- ⏳ Domain: Update Booking entity
- ⏳ Application: CheckInCommand.cs, CheckInCommandHandler.cs
- ⏳ Infrastructure: BookingRepository.UpdateCheckInStatus()
- ⏳ WebApi: BookingsController.CheckIn()

---

## 🎯 3. GET /api/bookings/today

**Use Case**: View today's bookings (for staff planning)  
**Auth Required**: ✅ Staff/Admin  
**Status**: ⏳ TODO

### Request
```http
GET /api/bookings/today?cinemaId=1&page=1&pageSize=50
Authorization: Bearer {staff_token}
```

### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| cinemaId | int | ❌ | Filter by cinema (optional) |
| page | int | ❌ | Page number (default: 1) |
| pageSize | int | ❌ | Items per page (default: 50) |
| status | string | ❌ | Filter: all, pending, confirmed, checkedin, cancelled, completed |
| hasPayment | bool | ❌ | Filter: only bookings with completed payment (check via Payments collection) |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Bookings retrieved successfully",
  "data": {
    "bookings": [
      {
        "bookingCode": "BK20251104001",
        "customerName": "Nguyen Van A",
        "movieTitle": "Avengers",
        "showtimeStart": "19:30",
        "status": "Confirmed",
        "paymentStatus": "Completed",
        "canCheckIn": true
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 50,
      "totalPages": 1,
      "totalRecords": 15
    }
  }
}
```

### Related Entities
**Booking** (bookings table):
- ✅ Filter by `bookingtime` = today
- ✅ Join with Customer, Movie, Showtime, Payments
- ✅ Show `status` field (Pending, Confirmed, CheckedIn, etc.)
- ✅ Calculate `paymentStatus` from Payments collection
- ✅ Calculate `canCheckIn` = (Payment.Status == "Completed" && Booking.Status != "CheckedIn")

### Implementation Plan
- ⏳ Domain: TodayBookingDTO.cs
- ⏳ Application: GetTodayBookingsQuery.cs
- ⏳ Infrastructure: BookingRepository.GetTodayBookings()
- ⏳ WebApi: BookingsController.GetTodayBookings()

---

## � Use Cases & Scenarios

### Use Case 1: Khách hàng đặt vé online và đến đúng giờ ✅

**Scenario:**
- Khách: Nguyen Van A
- **Đã đặt vé online và thanh toán thành công qua VNPay/MOMO**
- Booking Code: BK20251104001 (nhận được sau khi thanh toán)
- Suất chiếu: 19:30
- Thời gian đến: 19:15 (trước 15 phút)

**Steps:**

1. **Khách show QR code hoặc booking code (đã có sẵn sau khi thanh toán online)**
   ```
   Khách: (Mở app/email) "Em đã đặt vé online, đây là mã QR ạ"
   Staff: (Scan QR hoặc nhìn thấy code BK20251104001)
   ```

2. **Staff verify qua hệ thống**
   ```
   Staff: Call API GET /api/bookings/verify/BK20251104001
   ```

3. **Kiểm tra response**
   - ✅ Status: "Confirmed" (đã đặt vé)
   - ✅ Payment: "Completed" ← **QUAN TRỌNG** (đã thanh toán online)
   - ✅ Movie: "Avengers: Endgame"
   - ✅ Showtime: 19:30 (OK, còn 15 phút)
   - ✅ Seats: A5, A6
   - ✅ Customer name: Nguyen Van A

4. **Xác nhận với khách**
   ```
   Staff: "Dạ, anh Nguyen Van A phải không ạ? 
          Em xác nhận anh xem phim Avengers lúc 19:30, 
          ghế A5 và A6 đúng không ạ?"
   
   Khách: "Đúng rồi!"
   ```

5. **Check-in**
   ```
   Staff: Call API PUT /api/bookings/12345/check-in
   Response: "Check-in successful"
   ```

6. **Hướng dẫn khách vào rạp**
   ```
   Staff: "Dạ, Cinema 3 nằm ở tầng 2, 
          rẽ trái ra khỏi thang máy. 
          Ghế của anh là hàng A, số 5 và 6 ạ.
          Chúc anh xem phim vui vẻ!"
   ```

**Timeline:** ~40 giây

> **✅ Happy Path**: Đặt vé online → Thanh toán thành công → Nhận QR → Đến rạp → Verify → Check-in → Vào rạp 🎬

---

### Use Case 2: Khách đặt vé online nhưng đến muộn

**Scenario:**
- **Đã đặt vé online và thanh toán thành công**
- Booking Code: BK20251104002 (nhận được sau khi thanh toán)
- Suất chiếu: 19:30
- Thời gian đến: 19:45 (muộn 15 phút)

**Steps:**

1. **Khách show QR code**
   ```
   Khách: (Mở app) "Em đã đặt vé online, đây là mã QR ạ"
   Staff: (Scan QR → BK20251104002)
   ```

2. **Verify booking code**
   ```
   Staff: Call API GET /api/bookings/verify/BK20251104002
   
   Response:
   Booking.Status: "Confirmed" ✅
   Payment.Status: "Completed" ✅ (via Payments collection)
   Showtime: 19:30 (started 15 mins ago)
   ```

3. **Thông báo cho khách**
   ```
   Staff: "Anh ơi, suất chiếu của anh đã bắt đầu từ 19:30, 
          hiện tại phim đã chiếu được 15 phút rồi ạ. 
          Anh có muốn vào xem tiếp không?"
   
   Khách: "Vẫn vào được không?"
   
   Staff: "Dạ được ạ, nhưng anh cần đi nhẹ nhàng 
          để không làm phiền khán giả khác."
   ```

4. **Check-in với note**
   ```json
   PUT /api/bookings/12346/check-in
   {
     "checkinTime": "2025-11-04T19:45:00",
     "notes": "Late arrival - 15 minutes after showtime"
   }
   ```

5. **Hướng dẫn vào rạp nhẹ nhàng**
   ```
   Staff: "Dạ, Cinema 3 ở tầng 2 ạ. 
          Anh vui lòng đi nhẹ tay để không làm ồn nhé."
   ```

> **📝 Note**: Khách vẫn được check-in dù đến muộn vì **đã thanh toán online**. Không hoàn tiền.

---

### Use Case 3: Booking Code không hợp lệ (Nhập sai code)

**Scenario:**
- Khách đã đặt vé online và thanh toán
- Staff nhập sai code: BK20251104999 (thay vì BK20251104001)
- Response: 404 Not Found

**Steps:**

1. **Khách show QR/code**
   ```
   Khách: (Mở app) "Em đã đặt vé online, đây là mã ạ"
   Staff: (Nhập sai code: BK20251104999)
   ```

2. **Nhập code và nhận lỗi**
   ```
   Staff: Call API GET /api/bookings/verify/BK20251104999
   ```
   
   ```json
   Response:
   {
     "success": false,
     "statusCode": 404,
     "message": "Booking code not found"
   }
   ```

3. **Kiểm tra lại với khách**
   ```
   Staff: "Anh cho em xem lại mã booking được không? 
          Em thấy code này chưa có trong hệ thống."
   
   Khách: (Cho staff xem rõ hơn) "Code là BK20251104001 ạ!"
   
   Staff: "À, em xin lỗi. Em thử lại nhé."
   ```

4. **Verify lại code đúng → Success**

**Common Mistakes:**
- ❌ Nhầm chữ O với số 0
- ❌ Nhầm chữ I với số 1  
- ❌ Copy thiếu ký tự
- ❌ Spaces ở đầu/cuối
- ✅ **Best practice**: Dùng QR scanner thay vì nhập tay

---

### Use Case 4: Booking chưa thanh toán ❌ (KHÔNG BAO GIỜ XẢY RA)

> **⚠️ LƯU Ý QUAN TRỌNG**: Use case này **KHÔNG BAO GIỜ XẢY RA** trong hệ thống của chúng ta vì:
> - BookingCode chỉ được generate **SAU KHI** thanh toán thành công
> - Khách không thể nhận được QR/BookingCode nếu chưa thanh toán
> - API `/api/bookings/verify/{bookingCode}` sẽ **LUÔN** trả về booking có `Payment.Status = "Completed"` (trong collection Payments)

**Scenario:** (Chỉ để tham khảo - không xảy ra trong thực tế)
- Booking Code: BK20251104003
- Payment.Status: "Pending" ❌ (KHÔNG THỂ - vì BookingCode chỉ được tạo sau khi Payment.Status = "Completed")

**Lý do không xảy ra:**
```
Flow đúng:
1. Khách đặt vé → Booking.Status: "Pending", Payment.Status: "Pending"
2. Chưa có BookingCode (chưa generate)
3. Thanh toán thành công → Payment.Status: "Completed"
4. Hệ thống generate BookingCode → BK20251104001
5. Update Booking.Status: "Confirmed"
6. Gửi QR/BookingCode cho khách
7. Khách đến rạp → Staff verify → Check-in

❌ Không thể có: BookingCode + Payment.Status "Pending"
✅ Khi có BookingCode → Payment.Status LUÔN là "Completed"
```

**Nếu xảy ra (lỗi hệ thống):**
1. **Thông báo cho khách**
   ```
   Staff: "Anh ơi, em thấy có vấn đề với booking của anh. 
          Vui lòng đợi em liên hệ bộ phận kỹ thuật ạ."
   ```

2. **Escalate to IT/Admin ngay lập tức**
   - Hotline: [số điện thoại nội bộ]
   - Slack: #tech-support
   - Cung cấp: Booking Code, Customer info
   - **Đây là lỗi nghiêm trọng của hệ thống**

---

### Use Case 5: Khách đến sai rạp (Có thể xảy ra)

**Scenario:**
- Khách **đã đặt vé online và thanh toán thành công**
- Khách đến CGV Vincom
- Nhưng booking là cho CGV Landmark

**Steps:**

1. **Verify booking code và phát hiện sai rạp**
   ```
   Staff: Call API GET /api/bookings/verify/BK20251104005
   ```
   
   ```json
   Response:
   {
     "data": {
       "bookingCode": "BK20251104005",
       "status": "Confirmed",
       "payment": {
         "status": "Completed" ✅ (via Payments collection)
       },
       "showtime": {
         "cinema": {
           "name": "CGV Landmark 81",
           "address": "720A Dien Bien Phu, Binh Thanh"
         }
       }
     }
   }
   ```

2. **Thông báo cho khách**
   ```
   Staff: "Anh ơi, em xem booking của anh 
          là cho rạp CGV Landmark 81 ở Bình Thạnh, 
          không phải rạp này ạ."
   
   Khách: "Ôi, em nhầm rồi! Giờ làm sao?"
   
   Staff: "Dạ, rạp Landmark cách đây khoảng 20 phút đi Grab. 
          Suất chiếu của anh là 19:30, 
          còn 30 phút nữa thì kịp ạ."
   ```

3. **Hỗ trợ nếu cần**
   - Gọi Grab/taxi cho khách
   - Gọi cho rạp đích thông báo khách đang trên đường
   - Nếu không kịp: Liên hệ Admin để đổi suất (hoặc hoàn tiền)

---

## ⚠️ Xử lý lỗi

### Lỗi 1: 401 Unauthorized
**Nguyên nhân:** Token hết hạn hoặc không hợp lệ

**Giải pháp:**
```
1. Đăng xuất
2. Đăng nhập lại
3. Lấy token mới
4. Thử lại
```

---

### Lỗi 2: 404 Not Found
**Nguyên nhân:** Booking code không tồn tại

**Giải pháp:**
```
1. Kiểm tra lại code với khách
2. Tìm kiếm bằng số điện thoại/email
3. Nếu vẫn không có: Liên hệ Admin
```

---

### Lỗi 3: 400 Bad Request - Already Checked In
**Nguyên nhân:** Booking đã được check-in rồi

**Giải pháp:**
```
1. Kiểm tra thông tin check-in
2. Nếu khách chưa vào: Có thể bị trùng code
3. Liên hệ Admin để xử lý
```

---

### Lỗi 4: 500 Internal Server Error
**Nguyên nhân:** Lỗi server

**Giải pháp:**
```
1. Thử lại sau 30 giây
2. Nếu vẫn lỗi: Liên hệ IT support
3. Hotline: [số hotline nội bộ]
```

---

## 💡 Best Practices

### ✅ DO's

1. **Luôn verify booking code trước**
   - Đừng bao giờ bỏ qua bước này
   - Kiểm tra kỹ thông tin khách

2. **Kiểm tra giờ chiếu**
   - Thông báo rõ nếu khách đến sớm/muộn
   - Hướng dẫn khách chờ đợi hợp lý

3. **Xác nhận thông tin với khách**
   - Gọi tên khách
   - Nhắc lại tên phim, giờ chiếu, ghế

4. **Thái độ thân thiện**
   - Mỉm cười
   - Nói lời chào
   - Chúc khách xem phim vui vẻ

5. **Ghi chú khi cần**
   - Late arrival
   - Payment issues
   - Special requests

### ❌ DON'Ts

1. **KHÔNG bao giờ cho khách vào mà không verify**
   - Rủi ro: Trùng ghế, mất doanh thu

2. **KHÔNG sửa/xóa booking**
   - Bạn không có quyền này
   - Escalate to Admin

3. **KHÔNG tiết lộ thông tin khách hàng**
   - GDPR compliance
   - Privacy policy

4. **KHÔNG tranh cãi với khách**
   - Luôn giữ bình tĩnh
   - Escalate to Manager nếu cần

5. **KHÔNG bỏ qua payment status**
   - Kiểm tra kỹ trước khi check-in

---

## 🎯 KPIs cho Staff

### Metrics quan trọng

| Metric | Target | Thực tế | Đánh giá |
|--------|--------|---------|----------|
| **Check-in time** | < 60s | 45s | ✅ Excellent |
| **Booking errors** | < 2% | 1.5% | ✅ Good |
| **Customer satisfaction** | > 4.5/5 | 4.7/5 | ✅ Excellent |
| **Late arrivals handled** | 100% | 98% | ✅ Good |

### Đánh giá hiệu suất

**Excellent (5⭐):**
- Check-in time < 45s
- 0 errors trong tuần
- Customer rating > 4.8/5

**Good (4⭐):**
- Check-in time 45-60s
- < 2 errors trong tuần
- Customer rating 4.5-4.8/5

**Needs Improvement (3⭐):**
- Check-in time > 60s
- > 5 errors trong tuần
- Customer rating < 4.5/5

---

## 📞 Support & Escalation

### Khi nào cần escalate?

1. **Payment disputes** → Manager/Admin
2. **System errors** → IT Support
3. **Customer complaints** → Manager
4. **Booking conflicts** → Admin
5. **Technical issues** → IT Support

### Contact

**Manager on duty:** [Phone number]  
**IT Support hotline:** [Phone number]  
**Admin team:** admin@movie88.com  
**Slack channel:** #staff-support

---

## 🔄 Changelog

**Version 1.0** (2025-11-04)
- Initial documentation
- 5 use cases covered
- Best practices defined

**Pending:**
- [ ] Implement verify endpoint
- [ ] Implement check-in endpoint
- [ ] Implement today's bookings endpoint
- [ ] QR code scanning workflow
- [ ] Mobile app for staff

---

## 🧪 Testing Guide

### Quick Start

**Option 1: REST Client (VS Code Extension)**

1. Install REST Client extension
2. Create `tests/Staff.http` file
3. Run API server: `dotnet run`
4. Click "Send Request" on each test

**Option 2: Swagger UI**

1. Run API: `dotnet run`
2. Navigate to: https://localhost:7238/swagger
3. Click "Authorize" và paste staff token
4. Test endpoints với "Try it out"

### Test File Template: `tests/Staff.http`

```http
### Staff API Testing - Booking Verification
@baseUrl = https://movie88aspnet-app.up.railway.app/api
# @baseUrl = https://localhost:7238/api

### Variables
@staffToken = YOUR_STAFF_TOKEN_HERE
@bookingCode = BK20251104001
@bookingId = 12345

###############################################
# 1. VERIFY BOOKING CODE
###############################################

### Test 1.1: Verify valid booking code
GET {{baseUrl}}/bookings/verify/{{bookingCode}}
Authorization: Bearer {{staffToken}}

### Test 1.2: Verify invalid booking code (should return 404)
GET {{baseUrl}}/bookings/verify/BK20251104999
Authorization: Bearer {{staffToken}}

### Test 1.3: Verify already checked-in booking (should return 400)
GET {{baseUrl}}/bookings/verify/BK20251104002
Authorization: Bearer {{staffToken}}

### Test 1.4: Verify booking without payment (should show Pending status)
GET {{baseUrl}}/bookings/verify/BK20251104003
Authorization: Bearer {{staffToken}}

###############################################
# 2. CHECK-IN CUSTOMER
###############################################

### Test 2.1: Check-in on time
PUT {{baseUrl}}/bookings/{{bookingId}}/check-in
Authorization: Bearer {{staffToken}}
Content-Type: application/json

{
  "checkinTime": "2025-11-04T19:15:00",
  "notes": "Checked in at counter"
}

### Test 2.2: Check-in late arrival
PUT {{baseUrl}}/bookings/12346/check-in
Authorization: Bearer {{staffToken}}
Content-Type: application/json

{
  "checkinTime": "2025-11-04T19:45:00",
  "notes": "Late arrival - 15 minutes after showtime"
}

### Test 2.3: Check-in already checked booking (should fail)
PUT {{baseUrl}}/bookings/12345/check-in
Authorization: Bearer {{staffToken}}
Content-Type: application/json

{
  "checkinTime": "2025-11-04T19:50:00",
  "notes": "Duplicate check-in attempt"
}

###############################################
# 3. TODAY'S BOOKINGS
###############################################

### Test 3.1: Get all today's bookings
GET {{baseUrl}}/bookings/today
Authorization: Bearer {{staffToken}}

### Test 3.2: Get today's bookings with pagination
GET {{baseUrl}}/bookings/today?page=1&pageSize=50
Authorization: Bearer {{staffToken}}

### Test 3.3: Filter by cinema
GET {{baseUrl}}/bookings/today?cinemaId=1
Authorization: Bearer {{staffToken}}

### Test 3.4: Filter by status
GET {{baseUrl}}/bookings/today?status=confirmed
Authorization: Bearer {{staffToken}}

### Test 3.5: Filter by check-in status
GET {{baseUrl}}/bookings/today?checkinStatus=not-checked-in
Authorization: Bearer {{staffToken}}

### Test 3.6: Combined filters
GET {{baseUrl}}/bookings/today?cinemaId=1&status=confirmed&checkinStatus=not-checked-in&page=1&pageSize=20
Authorization: Bearer {{staffToken}}
```

### Test Scenarios

#### 1. Verify Booking Code Tests
- ✅ Valid booking code → Success 200
- ✅ Invalid booking code → Error 404
- ✅ Already checked-in → Error 400
- ✅ Payment pending → Show pending status
- ✅ Wrong cinema → Show correct cinema info
- ✅ Expired showtime → Still can verify but show warning

#### 2. Check-in Customer Tests
- ✅ Check-in on time → Success 200
- ✅ Check-in late (after showtime) → Success 200 with note
- ✅ Check-in early (before showtime) → Success 200
- ✅ Double check-in → Error 400
- ✅ Check-in without verify first → Should still work
- ✅ Check-in with invalid booking ID → Error 404

#### 3. Today's Bookings Tests
- ✅ Get all today's bookings
- ✅ Pagination works correctly
- ✅ Filter by cinema
- ✅ Filter by status (pending, confirmed, cancelled)
- ✅ Filter by check-in status
- ✅ Combined filters work together

### Expected Responses

**Success - Verify Booking (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Booking verified successfully",
  "data": {
    "bookingCode": "BK20251104001",
    "status": "Confirmed",
    "customer": {
      "fullname": "Nguyen Van A",
      "phone": "0901234567"
    },
    "movie": {
      "title": "Avengers: Endgame"
    },
    "showtime": {
      "startTime": "2025-11-04T19:30:00",
      "cinema": {
        "name": "CGV Vincom Center"
      }
    },
    "seats": [
      { "row": "A", "number": 5 }
    ],
    "payment": {
      "status": "Completed"
    }
  }
}
```

**Success - Check-in (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Check-in successful",
  "data": {
    "bookingId": 12345,
    "bookingCode": "BK20251104001",
    "status": "CheckedIn",
    "checkedInAt": "2025-11-04T19:15:00"
  }
}
```

**Error - Invalid Booking Code (404):**
```json
{
  "success": false,
  "statusCode": 404,
  "message": "Booking code not found",
  "errors": [
    "The booking code 'BK20251104999' does not exist in the system"
  ]
}
```

**Error - Already Checked In (400):**
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Booking already checked in",
  "errors": [
    "This booking was already checked in at 18:45 on 2025-11-04"
  ]
}
```

**Error - Unauthorized (401):**
```json
{
  "success": false,
  "statusCode": 401,
  "message": "Unauthorized",
  "errors": ["Invalid or expired token"]
}
```

### PowerShell Test Script: `tests/Test-StaffAPI.ps1`

```powershell
# Test Staff API endpoints
$baseUrl = "https://localhost:7238/api"
$staffToken = "YOUR_STAFF_TOKEN_HERE"

$headers = @{
    "Authorization" = "Bearer $staffToken"
    "Content-Type" = "application/json"
}

Write-Host "Testing Staff API..." -ForegroundColor Cyan

# Test 1: Verify Booking Code
Write-Host "`n1. Testing Verify Booking Code..." -ForegroundColor Yellow
$bookingCode = "BK20251104001"
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/bookings/verify/$bookingCode" -Method Get -Headers $headers
    Write-Host "✅ Verify Booking: SUCCESS" -ForegroundColor Green
    Write-Host "   Customer: $($response.data.customer.fullname)" -ForegroundColor White
    Write-Host "   Movie: $($response.data.movie.title)" -ForegroundColor White
    Write-Host "   Payment: $($response.data.payment.status)" -ForegroundColor White
} catch {
    Write-Host "❌ Verify Booking: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Invalid Booking Code (should fail with 404)
Write-Host "`n2. Testing Invalid Booking Code..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/bookings/verify/BK99999999" -Method Get -Headers $headers
    Write-Host "❌ Should have failed with 404" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "✅ Invalid Booking: Correctly returned 404" -ForegroundColor Green
    } else {
        Write-Host "❌ Unexpected error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test 3: Check-in Customer
Write-Host "`n3. Testing Check-in Customer..." -ForegroundColor Yellow
$bookingId = 12345
$checkinData = @{
    checkinTime = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Checked in via PowerShell test"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/bookings/$bookingId/check-in" -Method Put -Headers $headers -Body $checkinData
    Write-Host "✅ Check-in: SUCCESS" -ForegroundColor Green
    Write-Host "   Booking Code: $($response.data.bookingCode)" -ForegroundColor White
    Write-Host "   Check-in Time: $($response.data.checkinTime)" -ForegroundColor White
} catch {
    Write-Host "❌ Check-in: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Get Today's Bookings
Write-Host "`n4. Testing Today's Bookings..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/bookings/today?page=1&pageSize=10" -Method Get -Headers $headers
    Write-Host "✅ Today's Bookings: SUCCESS" -ForegroundColor Green
    Write-Host "   Total Bookings: $($response.data.totalRecords)" -ForegroundColor White
    Write-Host "   Not Checked In: $($response.data.bookings | Where-Object {$_.checkinStatus -eq 'NotCheckedIn'} | Measure-Object).Count" -ForegroundColor White
} catch {
    Write-Host "❌ Today's Bookings: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n✅ Staff API testing completed!" -ForegroundColor Cyan
```

### Common Issues & Solutions

**Issue 1: Token expired (401)**
- Solution: Login lại để lấy token mới
- Staff token có thời hạn 60 phút

**Issue 2: Booking code không tìm thấy (404)**
- Check xem có nhầm O/0, I/1 không
- Verify booking code chính xác từ customer
- Check database xem booking có tồn tại không

**Issue 3: Already checked in (400)**
- Check `checkedintime` trong database
- Có thể customer đã check-in rồi
- Hoặc có lỗi duplicate check-in request

**Issue 4: Forbidden (403)**
- Staff token không có quyền
- Cần Admin role cho một số endpoints
- Check role trong JWT token

---

**Last Updated**: November 4, 2025  
**Author**: Backend Team  
**Status**: ⚠️ APIs chưa implement, cần triển khai
