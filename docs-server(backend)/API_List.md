# 📋 Movie88 - Danh sách API Tổng hợp

## 📖 Giới thiệu

Tài liệu này liệt kê **TẤT CẢ** các API endpoints trong hệ thống Movie88, được nhóm theo module chức năng.

**Backend Architecture**: 3-Layer (Repository - Service - Controller)
**Payment Gateway**: VNPay

---

## 🔐 1. Authentication & User Management APIs

### Base URL: `/api/auth`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| POST | `/api/auth/register` | Đăng ký tài khoản mới | ❌ | Public |
| POST | `/api/auth/login` | Đăng nhập | ❌ | Public |
| POST | `/api/auth/logout` | Đăng xuất | ✅ | All |
| POST | `/api/auth/refresh-token` | Refresh JWT token | ✅ | All |
| POST | `/api/auth/forgot-password` | Quên mật khẩu | ❌ | Public |
| POST | `/api/auth/reset-password` | Đặt lại mật khẩu | ❌ | Public |
| POST | `/api/auth/change-password` | Đổi mật khẩu | ✅ | All |

### Base URL: `/api/users`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/users` | Lấy danh sách users | ✅ | Admin |
| GET | `/api/users/{id}` | Lấy thông tin user | ✅ | Admin, Self |
| GET | `/api/users/me` | Lấy thông tin user hiện tại | ✅ | All |
| PUT | `/api/users/{id}` | Cập nhật thông tin user | ✅ | Admin, Self |
| DELETE | `/api/users/{id}` | Xóa user | ✅ | Admin |

### Base URL: `/api/customers`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/customers/profile` | Lấy profile khách hàng | ✅ | Customer |
| PUT | `/api/customers/profile` | Cập nhật profile | ✅ | Customer |
| GET | `/api/customers/booking-history` | Lịch sử đặt vé | ✅ | Customer |
| GET | `/api/customers/payment-history` | Lịch sử thanh toán | ✅ | Customer |

---

## 🎬 2. Movie Management APIs

### Base URL: `/api/movies`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/movies` | Lấy danh sách phim | ❌ | Public |
| GET | `/api/movies/{id}` | Lấy chi tiết phim | ❌ | Public |
| GET | `/api/movies/now-showing` | Phim đang chiếu | ❌ | Public |
| GET | `/api/movies/coming-soon` | Phim sắp chiếu | ❌ | Public |
| GET | `/api/movies/search` | Tìm kiếm phim | ❌ | Public |
| GET | `/api/movies/{id}/showtimes` | Suất chiếu của phim | ❌ | Public |
| POST | `/api/movies` | Thêm phim mới | ✅ | Admin, Manager |
| PUT | `/api/movies/{id}` | Cập nhật phim | ✅ | Admin, Manager |
| DELETE | `/api/movies/{id}` | Xóa phim | ✅ | Admin |
| POST | `/api/movies/{id}/upload-poster` | Upload poster | ✅ | Admin, Manager |

---

## 🏢 3. Cinema Management APIs

### Base URL: `/api/cinemas`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/cinemas` | Lấy danh sách rạp | ❌ | Public |
| GET | `/api/cinemas/{id}` | Lấy chi tiết rạp | ❌ | Public |
| GET | `/api/cinemas/{id}/auditoriums` | Danh sách phòng chiếu | ❌ | Public |
| GET | `/api/cinemas/nearby` | Rạp gần vị trí | ❌ | Public |
| POST | `/api/cinemas` | Thêm rạp mới | ✅ | Admin |
| PUT | `/api/cinemas/{id}` | Cập nhật rạp | ✅ | Admin, Manager |
| DELETE | `/api/cinemas/{id}` | Xóa rạp | ✅ | Admin |

### Base URL: `/api/auditoriums`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/auditoriums/{id}` | Chi tiết phòng chiếu | ❌ | Public |
| GET | `/api/auditoriums/{id}/seats` | Sơ đồ ghế | ❌ | Public |
| POST | `/api/auditoriums` | Thêm phòng chiếu | ✅ | Admin, Manager |
| PUT | `/api/auditoriums/{id}` | Cập nhật phòng chiếu | ✅ | Admin, Manager |
| DELETE | `/api/auditoriums/{id}` | Xóa phòng chiếu | ✅ | Admin |

### Base URL: `/api/seats`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/seats/auditorium/{auditoriumId}` | Danh sách ghế theo phòng | ❌ | Public |
| POST | `/api/seats/bulk-create` | Tạo hàng loạt ghế | ✅ | Admin, Manager |
| PUT | `/api/seats/{id}` | Cập nhật ghế | ✅ | Admin, Manager |
| PUT | `/api/seats/{id}/availability` | Cập nhật trạng thái ghế | ✅ | Admin, Manager |

---

## 🕒 4. Showtime Management APIs

### Base URL: `/api/showtimes`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/showtimes` | Danh sách suất chiếu | ❌ | Public |
| GET | `/api/showtimes/{id}` | Chi tiết suất chiếu | ❌ | Public |
| GET | `/api/showtimes/by-movie/{movieId}` | Suất chiếu theo phim | ❌ | Public |
| GET | `/api/showtimes/by-cinema/{cinemaId}` | Suất chiếu theo rạp | ❌ | Public |
| GET | `/api/showtimes/by-date` | Suất chiếu theo ngày | ❌ | Public |
| GET | `/api/showtimes/{id}/available-seats` | Ghế còn trống | ❌ | Public |
| POST | `/api/showtimes` | Thêm suất chiếu | ✅ | Admin, Manager |
| POST | `/api/showtimes/bulk-create` | Tạo hàng loạt suất chiếu | ✅ | Admin, Manager |
| PUT | `/api/showtimes/{id}` | Cập nhật suất chiếu | ✅ | Admin, Manager |
| DELETE | `/api/showtimes/{id}` | Xóa suất chiếu | ✅ | Admin, Manager |

---

## 🎟 5. Booking Management APIs

### Base URL: `/api/bookings`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/bookings` | Danh sách bookings | ✅ | Admin, Manager |
| GET | `/api/bookings/{id}` | Chi tiết booking | ✅ | Customer, Admin |
| GET | `/api/bookings/my-bookings` | Bookings của tôi | ✅ | Customer |
| POST | `/api/bookings/create` | Tạo booking mới | ✅ | Customer |
| POST | `/api/bookings/{id}/select-seats` | Chọn ghế | ✅ | Customer |
| POST | `/api/bookings/{id}/add-combos` | Thêm combo | ✅ | Customer |
| POST | `/api/bookings/{id}/apply-voucher` | Áp dụng voucher | ✅ | Customer |
| PUT | `/api/bookings/{id}/confirm` | Xác nhận booking | ✅ | Customer |
| PUT | `/api/bookings/{id}/cancel` | Hủy booking | ✅ | Customer, Admin |
| DELETE | `/api/bookings/{id}` | Xóa booking | ✅ | Admin |

### Base URL: `/api/combos`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/combos` | Danh sách combo | ❌ | Public |
| GET | `/api/combos/{id}` | Chi tiết combo | ❌ | Public |
| POST | `/api/combos` | Thêm combo | ✅ | Admin, Manager |
| PUT | `/api/combos/{id}` | Cập nhật combo | ✅ | Admin, Manager |
| DELETE | `/api/combos/{id}` | Xóa combo | ✅ | Admin |

---

## 💳 6. Payment APIs

### Base URL: `/api/payments`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/payments` | Danh sách thanh toán | ✅ | Admin, Manager |
| GET | `/api/payments/{id}` | Chi tiết thanh toán | ✅ | Customer, Admin |
| POST | `/api/payments/vnpay/create` | Tạo thanh toán VNPay | ✅ | Customer |
| GET | `/api/payments/vnpay/callback` | Callback VNPay | ❌ | Public |
| POST | `/api/payments/vnpay/ipn` | IPN từ VNPay | ❌ | Public |
| PUT | `/api/payments/{id}/confirm` | Xác nhận thanh toán | ✅ | System |
| PUT | `/api/payments/{id}/cancel` | Hủy thanh toán | ✅ | Customer, Admin |
| POST | `/api/payments/{id}/refund` | Hoàn tiền | ✅ | Admin |

---

## 🎁 7. Promotion & Voucher APIs

### Base URL: `/api/vouchers`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/vouchers` | Danh sách voucher | ✅ | Admin, Manager |
| GET | `/api/vouchers/{id}` | Chi tiết voucher | ✅ | All |
| GET | `/api/vouchers/available` | Voucher khả dụng | ✅ | Customer |
| POST | `/api/vouchers/validate` | Kiểm tra mã voucher | ✅ | Customer |
| POST | `/api/vouchers` | Tạo voucher | ✅ | Admin, Manager |
| PUT | `/api/vouchers/{id}` | Cập nhật voucher | ✅ | Admin, Manager |
| DELETE | `/api/vouchers/{id}` | Xóa voucher | ✅ | Admin |

### Base URL: `/api/promotions`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/promotions` | Danh sách khuyến mãi | ❌ | Public |
| GET | `/api/promotions/{id}` | Chi tiết khuyến mãi | ❌ | Public |
| GET | `/api/promotions/active` | Khuyến mãi đang hoạt động | ❌ | Public |
| POST | `/api/promotions` | Tạo khuyến mãi | ✅ | Admin, Manager |
| PUT | `/api/promotions/{id}` | Cập nhật khuyến mãi | ✅ | Admin, Manager |
| DELETE | `/api/promotions/{id}` | Xóa khuyến mãi | ✅ | Admin |

---

## ⭐ 8. Review APIs

### Base URL: `/api/reviews`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/reviews/movie/{movieId}` | Reviews của phim | ❌ | Public |
| GET | `/api/reviews/{id}` | Chi tiết review | ❌ | Public |
| POST | `/api/reviews` | Tạo review | ✅ | Customer |
| PUT | `/api/reviews/{id}` | Cập nhật review | ✅ | Customer, Self |
| DELETE | `/api/reviews/{id}` | Xóa review | ✅ | Customer, Self, Admin |
| GET | `/api/reviews/my-reviews` | Reviews của tôi | ✅ | Customer |

---

## 👨‍💼 9. Admin & Report APIs

### Base URL: `/api/admin/roles`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/admin/roles` | Danh sách role | ✅ | Admin |
| POST | `/api/admin/roles` | Tạo role | ✅ | Admin |
| PUT | `/api/admin/roles/{id}` | Cập nhật role | ✅ | Admin |
| DELETE | `/api/admin/roles/{id}` | Xóa role | ✅ | Admin |

### Base URL: `/api/admin/dashboard`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/admin/dashboard/stats` | Thống kê tổng quan | ✅ | Admin, Manager |
| GET | `/api/admin/dashboard/revenue` | Báo cáo doanh thu | ✅ | Admin, Manager |
| GET | `/api/admin/dashboard/bookings` | Thống kê booking | ✅ | Admin, Manager |
| GET | `/api/admin/dashboard/movies` | Thống kê phim | ✅ | Admin, Manager |
| GET | `/api/admin/dashboard/customers` | Thống kê khách hàng | ✅ | Admin, Manager |

### Base URL: `/api/reports`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/reports/revenue/daily` | Doanh thu theo ngày | ✅ | Admin, Manager |
| GET | `/api/reports/revenue/monthly` | Doanh thu theo tháng | ✅ | Admin, Manager |
| GET | `/api/reports/revenue/by-movie` | Doanh thu theo phim | ✅ | Admin, Manager |
| GET | `/api/reports/revenue/by-cinema` | Doanh thu theo rạp | ✅ | Admin, Manager |
| GET | `/api/reports/bookings/statistics` | Thống kê booking | ✅ | Admin, Manager |
| GET | `/api/reports/popular-movies` | Phim phổ biến | ✅ | Admin, Manager |
| GET | `/api/reports/export` | Export báo cáo | ✅ | Admin, Manager |

---

## 🔧 10. System & Health Check APIs

### Base URL: `/api/health`

| Method | Endpoint | Mô tả | Auth Required | Role |
|--------|----------|-------|---------------|------|
| GET | `/api/health` | Health check | ❌ | Public |
| GET | `/api/health/database` | Database health | ✅ | Admin |
| GET | `/api/health/cache` | Cache health | ✅ | Admin |

---

## 📊 Tổng kết

| Module | Số lượng Endpoints |
|--------|-------------------|
| Authentication & User | 15 |
| Movie Management | 10 |
| Cinema Management | 14 |
| Showtime Management | 10 |
| Booking Management | 13 |
| Payment | 12 |
| Promotion & Voucher | 11 |
| Review | 6 |
| Admin & Report | 17 |
| System | 3 |
| **TỔNG CỘNG** | **111 endpoints** |

---

## 📝 Quy ước chung

### Response Format

#### Success Response
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Operation successful",
  "data": { ... }
}
```

#### Error Response
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Error message",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

### Pagination Format
```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalItems": 50
  }
}
```

### HTTP Status Codes
- `200` - OK (Success)
- `201` - Created
- `204` - No Content
- `400` - Bad Request
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not Found
- `409` - Conflict
- `422` - Unprocessable Entity
- `500` - Internal Server Error

---

**Last Updated**: October 29, 2025
**API Version**: v1
