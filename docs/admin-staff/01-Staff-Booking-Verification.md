# 🎟️ Staff Guide: Xác thực Booking tại Quầy

## 📋 Mục lục
1. [Giới thiệu](#giới-thiệu)
2. [Quy trình làm việc](#quy-trình-làm-việc)
3. [API Endpoints](#api-endpoints)
4. [Use Cases](#use-cases)
5. [Xử lý lỗi](#xử-lý-lỗi)
6. [Best Practices](#best-practices)

---

## 🎯 Giới thiệu

### Vai trò của Staff
Bạn là nhân viên tại quầy check-in rạp chiếu phim Movie88. Nhiệm vụ chính:

1. ✅ **Xác thực Booking Code** từ khách hàng
2. ✅ **Kiểm tra thông tin** booking (phim, suất chiếu, ghế)
3. ✅ **Check-in** khách hàng vào xem phim
4. ✅ **Xử lý các vấn đề** (booking không hợp lệ, đến muộn, v.v.)

### Quyền hạn
- ✅ Xem thông tin booking
- ✅ Verify booking code
- ✅ Check-in khách hàng
- ❌ **KHÔNG** được sửa/xóa booking
- ❌ **KHÔNG** được hoàn tiền (cần Admin)

---

## 🔄 Quy trình làm việc

### Workflow Chuẩn

```mermaid
graph TD
    A[Khách đến quầy] --> B{Có Booking Code?}
    B -->|Có| C[Nhập Booking Code vào hệ thống]
    B -->|Không| D[Hướng dẫn khách đặt vé]
    
    C --> E{Booking Code hợp lệ?}
    E -->|Không| F[Thông báo lỗi]
    E -->|Có| G[Hiển thị thông tin booking]
    
    G --> H{Kiểm tra thông tin}
    H --> I{Đúng suất chiếu?}
    I -->|Không| J[Khách đến sai suất]
    I -->|Có| K{Đã thanh toán?}
    
    K -->|Chưa| L[Yêu cầu thanh toán]
    K -->|Rồi| M[Check-in thành công]
    
    M --> N[In vé/Scan QR vào]
    
    F --> O[Hướng dẫn khách]
    J --> P[Liên hệ Admin]
    L --> P
```

### Timeline Thực tế

| Thời gian | Action | Thời lượng |
|-----------|--------|------------|
| **T-0** | Khách đưa booking code | 5s |
| **T+5s** | Staff nhập code vào hệ thống | 10s |
| **T+15s** | Hệ thống verify & hiển thị info | 2s |
| **T+17s** | Staff kiểm tra thông tin | 15s |
| **T+32s** | Confirm & check-in | 5s |
| **T+37s** | In vé/Scan QR vào rạp | 10s |
| **TOTAL** | **~45 giây** | |

---

## 🔌 API Endpoints

### 1. Verify Booking Code
**⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI**

```http
GET /api/bookings/verify/{bookingCode}
Authorization: Bearer {staff_token}
```

#### Request Example
```http
GET https://movie88aspnet-app.up.railway.app/api/bookings/verify/BK20251104001
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Response Success (200 OK)
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

#### Response Error (404 Not Found)
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

#### Response Error (400 Bad Request - Đã check-in)
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

---

### 2. Check-in Customer
**⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI**

```http
PUT /api/bookings/{bookingId}/check-in
Authorization: Bearer {staff_token}
Content-Type: application/json

{
  "checkinTime": "2025-11-04T19:15:00",
  "notes": "Checked in by staff at counter"
}
```

#### Response Success (200 OK)
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

---

### 3. Get Today's Bookings
**⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI**

```http
GET /api/bookings/today?cinema={cinemaId}&page=1&pageSize=50
Authorization: Bearer {staff_token}
```

#### Response Success (200 OK)
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

---

## 📱 Use Cases

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

**Last Updated**: November 4, 2025  
**Author**: Backend Team  
**Status**: ⚠️ APIs chưa implement, cần triển khai
