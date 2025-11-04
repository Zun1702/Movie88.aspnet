# 🎟️ Staff: Xác thực Booking tại Quầy (3 Endpoints)

**Status**: ⚠️ **PENDING IMPLEMENTATION** (0/3 endpoints - 0%)  
**Assigned**: Việt

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Use Case | Auth | Status | Assign |
|---|--------|----------|----------|------|--------|--------|
| 1 | GET | `/api/bookings/verify/{bookingCode}` | Verify booking at counter | ✅ Staff | ⏳ TODO | Việt |
| 2 | PUT | `/api/bookings/{id}/check-in` | Check-in customer | ✅ Staff | ⏳ TODO | Việt |
| 3 | GET | `/api/bookings/today` | View today's bookings | ✅ Staff | ⏳ TODO | Việt |

---

## 🎯 Vai trò của Staff

**Bạn là nhân viên tại quầy check-in** rạp chiếu phim Movie88. Nhiệm vụ chính:

### ✅ Quyền hạn
- ✅ Xem thông tin booking
- ✅ Verify booking code
- ✅ Check-in khách hàng

### ❌ Không có quyền
- ❌ Sửa/xóa booking
- ❌ Hoàn tiền (cần Admin)
- ❌ Quản lý phim/rạp/suất chiếu

---

## 🎯 1. GET /api/bookings/verify/{bookingCode}

**Use Case**: Verify booking at counter  
**Auth Required**: ✅ Staff/Admin  
**Status**: ⏳ TODO

### Workflow Timeline

| Step | Action | Duration |
|------|--------|----------|
| 1 | Customer arrives with booking code | 5s |
| 2 | Staff enters code into system | 10s |
| 3 | System verifies & displays info | 2s |
| 4 | Staff checks information | 15s |
| 5 | Confirm & check-in | 5s |
| 6 | Print ticket/Scan QR | 10s |
| **Total** | **Complete workflow** | **~45s** |

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
      "paymentStatus": "Completed",
      "paymentMethod": "VNPay",
      "paidAt": "2025-11-01T14:35:00"
    },
    "checkinStatus": "NotCheckedIn",
    "checkinTime": null
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

### Related Entities
**Booking** (bookings table):
- ✅ `bookingid` (int, PK)
- ✅ `bookingcode` (string, unique)
- ✅ `customerid` (int, FK)
- ✅ `showtimeid` (int, FK)
- ✅ `totalamount` (decimal)
- ✅ `status` (string) - Pending, Confirmed, Cancelled
- ✅ `paymentstatus` (string) - Pending, Completed, Failed
- ✅ `bookingdate` (DateTime)
- ✅ `checkedinstatus` (string) - NotCheckedIn, CheckedIn
- ✅ `checkedintime` (DateTime, nullable)

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
    "checkinStatus": "CheckedIn",
    "checkinTime": "2025-11-04T19:15:00",
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
- ✅ Update `checkedinstatus` = "CheckedIn"
- ✅ Update `checkedintime` = provided timestamp
- ✅ Log staff who performed check-in

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
| status | string | ❌ | Filter: all, pending, confirmed, cancelled |
| checkinStatus | string | ❌ | Filter: all, not-checked-in, checked-in |

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
        "checkinStatus": "NotCheckedIn"
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
- ✅ Filter by `bookingdate` = today
- ✅ Join with Customer, Movie, Showtime
- ✅ Show `checkedinstatus`

### Implementation Plan
- ⏳ Domain: TodayBookingDTO.cs
- ⏳ Application: GetTodayBookingsQuery.cs
- ⏳ Infrastructure: BookingRepository.GetTodayBookings()
- ⏳ WebApi: BookingsController.GetTodayBookings()

---

## � Use Cases & Scenarios

### Use Case 1: Khách hàng đến đúng giờ

**Scenario:**
- Khách: Nguyen Van A
- Booking Code: BK20251104001
- Suất chiếu: 19:30
- Thời gian đến: 19:15 (trước 15 phút)

**Steps:**

1. **Nhận booking code từ khách**
   ```
   Khách: "Xin chào, em có booking code là BK20251104001"
   ```

2. **Nhập code vào hệ thống**
   ```
   Staff: Call API GET /api/bookings/verify/BK20251104001
   ```

3. **Kiểm tra response**
   - ✅ Status: "Confirmed"
   - ✅ Payment: "Completed"
   - ✅ Movie: "Avengers: Endgame"
   - ✅ Showtime: 19:30 (OK, còn 15 phút)
   - ✅ Seats: A5, A6
   - ✅ Customer name matches ID

4. **Xác nhận với khách**
   ```
   Staff: "Anh Nguyen Van A phải không ạ? 
          Anh xem phim Avengers lúc 19:30, 
          ghế A5 và A6 phải không?"
   
   Khách: "Đúng rồi!"
   ```

5. **Check-in**
   ```
   Staff: Call API PUT /api/bookings/12345/check-in
   Response: "Check-in successful"
   ```

6. **Hướng dẫn khách**
   ```
   Staff: "Dạ, Cinema 3 nằm ở tầng 2, 
          rẽ trái ra khỏi thang máy. 
          Ghế của anh là hàng A, số 5 và 6 ạ.
          Chúc anh xem phim vui vẻ!"
   ```

**Timeline:** ~45 giây

---

### Use Case 2: Khách đến muộn

**Scenario:**
- Booking Code: BK20251104002
- Suất chiếu: 19:30
- Thời gian đến: 19:45 (muộn 15 phút)

**Steps:**

1. **Verify booking code**
   ```
   Status: "Confirmed"
   Showtime: 19:30 (started 15 mins ago)
   ```

2. **Thông báo cho khách**
   ```
   Staff: "Anh ơi, suất chiếu của anh đã bắt đầu từ 19:30, 
          hiện tại phim đã chiếu được 15 phút rồi ạ. 
          Anh có muốn vào xem tiếp không?"
   
   Khách: "Vẫn vào được không?"
   
   Staff: "Dạ được ạ, nhưng anh cần đi nhẹ nhàng 
          để không làm phiền khán giả khác."
   ```

3. **Check-in với note**
   ```json
   PUT /api/bookings/12346/check-in
   {
     "checkinTime": "2025-11-04T19:45:00",
     "notes": "Late arrival - 15 minutes after showtime"
   }
   ```

4. **Hướng dẫn vào rạp nhẹ nhàng**

---

### Use Case 3: Booking Code không hợp lệ

**Scenario:**
- Khách cung cấp code: BK20251104999
- Response: 404 Not Found

**Steps:**

1. **Nhập code và nhận lỗi**
   ```json
   {
     "success": false,
     "statusCode": 404,
     "message": "Booking code not found"
   }
   ```

2. **Kiểm tra lại với khách**
   ```
   Staff: "Anh cho em xem lại booking code được không? 
          Em thấy code này chưa có trong hệ thống."
   
   Khách: (Mở email/SMS) "À code là BK20251104001 ạ!"
   
   Staff: "Dạ vâng, em thử lại nhé."
   ```

3. **Verify lại code đúng**

**Common Mistakes:**
- ❌ Nhầm chữ O với số 0
- ❌ Nhầm chữ I với số 1
- ❌ Copy thiếu ký tự
- ❌ Spaces ở đầu/cuối

---

### Use Case 4: Booking chưa thanh toán

**Scenario:**
- Booking Code: BK20251104003
- Payment Status: "Pending"

**Steps:**

1. **Verify và phát hiện chưa thanh toán**
   ```json
   {
     "payment": {
       "paymentStatus": "Pending",
       "paymentMethod": null,
       "paidAt": null
     }
   }
   ```

2. **Thông báo cho khách**
   ```
   Staff: "Anh ơi, em thấy booking của anh 
          chưa được thanh toán ạ. 
          Anh có muốn thanh toán bây giờ không?"
   
   Khách: "Ủa, em đã chuyển khoản rồi mà?"
   
   Staff: "Vậy anh đợi em liên hệ bộ phận kế toán 
          kiểm tra lại nhé. Xin anh chờ khoảng 5 phút."
   ```

3. **Escalate to Admin/Manager**
   - Gọi hotline: [số điện thoại nội bộ]
   - Hoặc: Liên hệ qua Slack/Teams
   - Cung cấp: Booking Code, Customer info

4. **Xử lý tùy theo policy**
   - Option 1: Cho khách vào nếu có proof of payment
   - Option 2: Yêu cầu thanh toán lại
   - Option 3: Chuyển suất chiếu khác

---

### Use Case 5: Khách đến sai rạp

**Scenario:**
- Khách đến CGV Vincom
- Booking là cho CGV Landmark

**Steps:**

1. **Verify và phát hiện sai rạp**
   ```json
   {
     "showtime": {
       "cinema": {
         "name": "CGV Landmark 81",
         "address": "720A Dien Bien Phu, Binh Thanh"
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
   - Gọi Grab/taxi
   - Gọi cho rạp đích thông báo
   - Nếu không kịp: Liên hệ Admin để đổi suất

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
      "paymentStatus": "Completed"
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
    "checkinStatus": "CheckedIn",
    "checkinTime": "2025-11-04T19:15:00"
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
    Write-Host "   Payment: $($response.data.payment.paymentStatus)" -ForegroundColor White
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
