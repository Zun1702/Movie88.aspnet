# 👤 User Flow - Hành trình Người dùng

## 📖 Giới thiệu

Tài liệu này mô tả chi tiết hành trình của người dùng từ khi truy cập hệ thống Movie88 đến khi hoàn thành đặt vé và đánh giá phim.

---

## 🎯 1. User Journey Map

```
┌─────────────┐
│  DISCOVER   │  Browse phim, xem trailer, đọc review
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  REGISTER   │  Đăng ký tài khoản (nếu chưa có)
└──────┬──────┘
       │
       ↓
┌─────────────┐
│   LOGIN     │  Đăng nhập vào hệ thống
└──────┬──────┘
       │
       ↓
┌─────────────┐
│   SELECT    │  Chọn phim → Chọn rạp → Chọn suất chiếu
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  BOOKING    │  Chọn ghế → Chọn combo → Áp voucher
└──────┬──────┘
       │
       ↓
┌─────────────┐
│  PAYMENT    │  Thanh toán qua VNPay
└──────┬──────┘
       │
       ↓
┌─────────────┐
│ CONFIRMATION│  Nhận email + QR code
└──────┬──────┘
       │
       ↓
┌─────────────┐
│   ENJOY     │  Đến rạp → Quét QR → Xem phim
└──────┬──────┘
       │
       ↓
┌─────────────┐
│   REVIEW    │  Đánh giá phim sau khi xem
└─────────────┘
```

---

## 🔍 2. Flow Chi tiết theo Giai đoạn

### 2.1 DISCOVER - Khám phá Phim

**Mục tiêu**: Người dùng tìm kiếm và khám phá phim để xem

#### Actions:
1. Truy cập website/app Movie88
2. Browse danh sách phim:
   - Phim đang chiếu
   - Phim sắp chiếu
   - Phim nổi bật
3. Tìm kiếm phim theo:
   - Tên phim
   - Thể loại
   - Đạo diễn
4. Xem chi tiết phim:
   - Poster, trailer
   - Mô tả, thời lượng, độ tuổi
   - Lịch chiếu
   - Đánh giá từ khách hàng khác

#### API Flow:
```
GET /api/movies/now-showing
→ Hiển thị danh sách phim đang chiếu

GET /api/movies/{id}
→ Xem chi tiết phim

GET /api/movies/{id}/showtimes
→ Xem lịch chiếu của phim

GET /api/reviews/movie/{movieId}
→ Đọc reviews
```

#### UI Components:
- **Homepage**: Carousel phim hot, grid phim đang chiếu/sắp chiếu
- **Movie Detail Page**: Poster lớn, trailer player, thông tin, lịch chiếu
- **Search Bar**: Tìm kiếm real-time

---

### 2.2 REGISTER - Đăng ký Tài khoản

**Mục tiêu**: Tạo tài khoản để đặt vé

#### Actions:
1. Click nút "Đăng ký"
2. Điền form:
   - Họ tên
   - Email
   - Số điện thoại
   - Mật khẩu
   - Xác nhận mật khẩu
3. Đọc và đồng ý điều khoản
4. Click "Đăng ký"
5. Nhận email xác nhận (optional)

#### API Flow:
```
POST /api/auth/register
{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "phone": "0901234567"
}

Response:
{
  "userId": 1,
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "accessToken": "eyJhbG...",
  "refreshToken": "refresh_token_here"
}
```

#### Validation:
- Email chưa tồn tại trong hệ thống
- Password đủ mạnh (8 ký tự, chữ hoa, thường, số, ký tự đặc biệt)
- Số điện thoại đúng format (10 số, bắt đầu bằng 0)

#### Error Handling:
- Email đã tồn tại → Hiển thị lỗi, suggest đăng nhập
- Password yếu → Hiển thị yêu cầu password
- Network error → Retry mechanism

---

### 2.3 LOGIN - Đăng nhập

**Mục tiêu**: Xác thực người dùng để truy cập hệ thống

#### Actions:
1. Click nút "Đăng nhập"
2. Nhập email và password
3. (Optional) Tick "Ghi nhớ đăng nhập"
4. Click "Đăng nhập"
5. Hệ thống redirect:
   - Customer → Homepage
   - Admin/Staff → Admin Dashboard

#### API Flow:
```
POST /api/auth/login
{
  "email": "nguyenvana@example.com",
  "password": "Password123!"
}

Response:
{
  "userId": 1,
  "roleId": 4,
  "roleName": "Customer",
  "fullName": "Nguyễn Văn A",
  "accessToken": "eyJhbG...",
  "refreshToken": "refresh_token_here",
  "tokenExpiry": "2025-10-29T12:00:00Z"
}
```

#### Token Storage:
- **Access Token**: Lưu trong memory hoặc sessionStorage (expire 1h)
- **Refresh Token**: Lưu trong httpOnly cookie (expire 7 days)

#### Error Handling:
- Email không tồn tại → "Email hoặc mật khẩu không đúng"
- Password sai → "Email hoặc mật khẩu không đúng"
- Quá 5 lần đăng nhập sai → Khóa tài khoản 15 phút

---

### 2.4 SELECT - Chọn Phim và Suất chiếu

**Mục tiêu**: Chọn phim, rạp, và suất chiếu cụ thể

#### Actions:
1. Từ trang chi tiết phim, xem lịch chiếu
2. Chọn ngày xem (date picker)
3. Chọn rạp (nếu có nhiều rạp)
4. Chọn suất chiếu:
   - Hiển thị giờ chiếu
   - Format (2D/3D)
   - Ngôn ngữ (Phụ đề/Lồng tiếng)
   - Giá vé
5. Click "Đặt vé"

#### API Flow:
```
GET /api/showtimes/by-movie/{movieId}?date=2025-10-30&cinemaId=1
→ Lấy danh sách suất chiếu theo phim, ngày, rạp

Response:
[
  {
    "showtimeId": 123,
    "startTime": "2025-10-30T19:30:00Z",
    "endTime": "2025-10-30T22:00:00Z",
    "price": 80000,
    "format": "2D",
    "languageType": "Original - Vietsub",
    "availableSeats": 45,
    "cinemaName": "CGV Vincom Center",
    "auditoriumName": "Phòng 3"
  }
]
```

#### UI Components:
- **Date Picker**: Chọn ngày trong 7 ngày tới
- **Cinema Filter**: Dropdown chọn rạp
- **Showtime Grid**: Hiển thị các suất chiếu trong ngày

---

### 2.5 BOOKING - Đặt Vé

**Mục tiêu**: Chọn ghế, combo, áp voucher và tạo booking

#### Step 1: Tạo Booking
```
POST /api/bookings/create
{
  "showtimeId": 123,
  "customerId": 45
}

Response:
{
  "bookingId": 1001,
  "status": "Pending",
  "showtimeId": 123,
  "totalAmount": 0
}
```

#### Step 2: Chọn Ghế
1. Hiển thị sơ đồ ghế:
   - Available (màu xanh)
   - Taken (màu xám)
   - Selected (màu vàng)
   - VIP (màu đỏ)
2. User click để chọn/bỏ chọn ghế
3. Validation:
   - Tối thiểu 1 ghế, tối đa 10 ghế
   - Không chọn ghế đã bán
4. Click "Tiếp tục"

```
GET /api/showtimes/{showtimeId}/available-seats
→ Lấy sơ đồ ghế

POST /api/bookings/1001/select-seats
{
  "seatIds": [45, 46]
}

Response:
{
  "bookingId": 1001,
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
  "pricing": {
    "seatsTotal": 160000,
    "totalAmount": 160000
  }
}
```

#### Step 3: Chọn Combo (Optional)
1. Hiển thị danh sách combo:
   - Combo 1: Bắp + Nước (60k)
   - Combo 2: Bắp + Nước + Snack (75k)
2. Chọn combo và số lượng
3. Click "Tiếp tục"

```
GET /api/combos
→ Danh sách combo

POST /api/bookings/1001/add-combos
{
  "combos": [
    {
      "comboId": 1,
      "quantity": 2
    }
  ]
}

Response:
{
  "bookingId": 1001,
  "combos": [...],
  "pricing": {
    "seatsTotal": 160000,
    "combosTotal": 120000,
    "subtotal": 280000,
    "totalAmount": 280000
  }
}
```

#### Step 4: Áp Voucher (Optional)
1. Nhập mã voucher
2. Click "Áp dụng"
3. Hệ thống validate và tính discount

```
POST /api/bookings/1001/apply-voucher
{
  "voucherCode": "SUMMER2025"
}

Response:
{
  "bookingId": 1001,
  "voucher": {
    "code": "SUMMER2025",
    "discountType": "Percent",
    "discountValue": 10,
    "discountAmount": 28000
  },
  "pricing": {
    "seatsTotal": 160000,
    "combosTotal": 120000,
    "subtotal": 280000,
    "voucherDiscount": 28000,
    "totalAmount": 252000
  }
}
```

#### Step 5: Review và Confirm
1. Hiển thị tóm tắt:
   - Phim, rạp, suất chiếu
   - Ghế đã chọn
   - Combo
   - Discount
   - Tổng tiền
2. Click "Xác nhận"

```
PUT /api/bookings/1001/confirm

Response:
{
  "bookingId": 1001,
  "status": "Confirmed",
  "totalAmount": 252000
}
```

---

### 2.6 PAYMENT - Thanh toán

**Mục tiêu**: Thanh toán để hoàn tất đặt vé

#### Actions:
1. Chọn phương thức thanh toán VNPay
2. Click "Thanh toán"
3. Redirect đến cổng thanh toán VNPay
4. Nhập thông tin thẻ/tài khoản
5. Xác nhận thanh toán
6. Redirect về Movie88

#### API Flow:
```
POST /api/payments/vnpay/create
{
  "bookingId": 1001,
  "amount": 252000,
  "returnUrl": "https://movie88.com/payment/result"
}

Response:
{
  "paymentUrl": "https://sandbox.vnpayment.vn/...",
  "transactionCode": "VNP_20251029_1001"
}

Frontend redirect → paymentUrl

User thanh toán trên VNPay

VNPay callback:
GET /api/payments/vnpay/callback?vnp_ResponseCode=00&...

Backend xử lý và redirect:
→ https://movie88.com/payment/success?bookingId=1001
```

#### Payment Success Page:
- Hiển thị thông tin booking
- QR code để check-in tại rạp
- Nút "Tải vé PDF"
- Nút "Gửi email"

---

### 2.7 CONFIRMATION - Xác nhận và Nhận vé

**Mục tiêu**: Nhận xác nhận và thông tin vé

#### Actions:
1. Nhận email xác nhận:
   - Thông tin phim, rạp, suất chiếu
   - Ghế đã đặt
   - QR code
   - Tổng tiền đã thanh toán
2. Lưu QR code vào điện thoại
3. (Optional) Tải vé PDF

#### Email Template:
```html
<h2>Đặt vé thành công!</h2>
<p>Cảm ơn bạn đã đặt vé tại Movie88</p>

<strong>Thông tin phim:</strong>
- Phim: Avengers: Endgame
- Rạp: CGV Vincom Center
- Phòng: 3
- Suất chiếu: 30/10/2025 - 19:30
- Ghế: D5, D6

<img src="qr_code.png" alt="QR Code">
<p>Vui lòng xuất trình QR code tại quầy vé</p>

<strong>Tổng tiền: 252,000 VND</strong>
```

---

### 2.8 ENJOY - Xem phim tại Rạp

**Mục tiêu**: Sử dụng vé để vào rạp xem phim

#### Actions:
1. Đến rạp trước giờ chiếu 15-30 phút
2. Xuất trình QR code tại quầy vé
3. Staff quét QR code
4. Nhận vé giấy (nếu cần)
5. Nhận combo đồ ăn (nếu có)
6. Vào phòng chiếu
7. Ngồi đúng ghế đã đặt
8. Thưởng thức phim

#### Staff Side (Admin App):
```
GET /api/bookings/{id}?qrCode={qrCode}
→ Validate QR code

PUT /api/bookings/{id}/check-in
→ Đánh dấu đã check-in
```

---

### 2.9 REVIEW - Đánh giá Phim

**Mục tiêu**: Chia sẻ trải nghiệm và đánh giá phim

#### Actions:
1. Sau khi xem phim, nhận notification yêu cầu đánh giá
2. Click "Đánh giá phim"
3. Chọn rating (1-5 sao)
4. Viết bình luận (optional)
5. Submit review

#### API Flow:
```
POST /api/reviews
{
  "movieId": 123,
  "customerId": 45,
  "rating": 5,
  "comment": "Phim rất hay, đáng xem!"
}

Response:
{
  "reviewId": 5001,
  "movieId": 123,
  "customerName": "Nguyễn Văn A",
  "rating": 5,
  "comment": "Phim rất hay, đáng xem!",
  "createdAt": "2025-10-30T22:30:00Z"
}
```

---

## 🔄 3. Alternative Flows

### 3.1 Hủy Vé
```
User vào "Lịch sử đặt vé"
→ Chọn booking cần hủy
→ Click "Hủy vé"
→ Xác nhận lý do hủy
→ PUT /api/bookings/{id}/cancel
→ Xử lý hoàn tiền (nếu đủ điều kiện)
→ Nhận email xác nhận hủy
```

### 3.2 Quên Mật khẩu
```
Click "Quên mật khẩu"
→ Nhập email
→ POST /api/auth/forgot-password
→ Nhận email reset password
→ Click link trong email
→ Nhập mật khẩu mới
→ POST /api/auth/reset-password
→ Redirect đến trang login
```

### 3.3 Đặt vé không cần đăng nhập (Guest Checkout) (optional)
```
Note: Feature này có thể implement sau
- User chọn phim, ghế như bình thường
- Nhập thông tin: email, phone
- Thanh toán
- Nhận vé qua email
- Không lưu lịch sử vào tài khoản
```

---

## 📊 4. Metrics & Analytics (admin)

### Tracking Events:
1. **Page Views**: Xem trang phim, trang đặt vé
2. **Click Events**: Click chọn suất chiếu, chọn ghế
3. **Conversion**: Tỷ lệ hoàn thành booking
4. **Drop-off Points**: Nơi user rời bỏ flow
5. **Payment Success Rate**: Tỷ lệ thanh toán thành công

### KPIs:
- **Booking Completion Rate**: > 70%
- **Payment Success Rate**: > 95%
- **Average Time to Book**: < 5 phút
- **User Retention Rate**: > 50%

---

**Last Updated**: October 29, 2025
**Version**: v1.0
