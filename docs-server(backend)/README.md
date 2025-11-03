# 📚 Movie88 API Documentation

Chào mừng đến với tài liệu API đầy đủ của dự án **Movie88** - Hệ thống đặt vé xem phim trực tuyến.

---

## 📖 Giới thiệu

Bộ tài liệu này được tạo tự động dựa trên:
- ✅ **DatabaseScript.txt** - Thiết kế cơ sở dữ liệu
- ✅ **Functional Requirement** - Yêu cầu chức năng hệ thống
- ✅ **request.txt** - Mô tả tổng quan dự án

**Mục đích**: Cung cấp tài liệu API chuyên nghiệp, chi tiết, nhất quán để:
- Hướng dẫn phát triển backend
- Huấn luyện LLM Agent sinh code
- Dễ bảo trì và mở rộng

---

## 📁 Cấu trúc Tài liệu

```
/docs
│
├── README.md                    # File này - Hướng dẫn đọc tài liệu
├── Overview.md                  # Tổng quan hệ thống & timeline phát triển
├── API_List.md                  # Danh sách tổng hợp 111 API endpoints
│
├── /modules                     # Tài liệu chi tiết từng module
│   ├── UserAPI.md              # Authentication & User Management (15 APIs)
│   ├── MovieAPI.md             # Movie Management (10 APIs)
│   ├── CinemaAPI.md            # Cinema & Auditorium Management (14 APIs)
│   ├── ShowtimeAPI.md          # Showtime Management (10 APIs)
│   ├── BookingAPI.md           # Booking & Combo Management (13 APIs)
│   ├── PaymentAPI.md           # Payment Integration (12 APIs)
│   ├── PromotionAPI.md         # Voucher & Promotion (11 APIs)
│   ├── ReviewAPI.md            # Review System (6 APIs)
│   └── AdminAPI.md             # Admin Dashboard & Reports (17 APIs)
│
└── /flow                        # Luồng nghiệp vụ và dữ liệu
    ├── UserFlow.md             # Hành trình người dùng từ A-Z
    ├── DataFlow.md             # Dòng dữ liệu giữa các bảng
    └── AuthFlow.md             # Xác thực & phân quyền chi tiết
```

---

## 🚀 Quick Start

### 1. Đọc Tổng quan
Bắt đầu với [`Overview.md`](./Overview.md) để hiểu:
- Kiến trúc hệ thống
- Phân module
- Timeline phát triển (8 tuần)
- Technology stack

### 2. Xem Danh sách API
Tham khảo [`API_List.md`](./API_List.md) để:
- Xem tổng hợp 111 endpoints
- Hiểu phân nhóm theo module
- Nắm quy ước response format

### 3. Đọc Module chi tiết
Chọn module cần implement, ví dụ:
- **Bắt đầu với User**: [`modules/UserAPI.md`](./modules/UserAPI.md)
- **Sau đó Booking**: [`modules/BookingAPI.md`](./modules/BookingAPI.md)
- **Và Payment**: [`modules/PaymentAPI.md`](./modules/PaymentAPI.md)

### 4. Hiểu Flow nghiệp vụ
Đọc flow để nắm logic:
- [`flow/UserFlow.md`](./flow/UserFlow.md) - Khách hàng đặt vé như thế nào
- [`flow/AuthFlow.md`](./flow/AuthFlow.md) - Xác thực & phân quyền
- [`flow/DataFlow.md`](./flow/DataFlow.md) - Dữ liệu di chuyển ra sao

---

## 📋 Danh mục Module

### 👤 1. User Management
**File**: [`modules/UserAPI.md`](./modules/UserAPI.md)

**APIs**: 15 endpoints
- Đăng ký, đăng nhập, đăng xuất
- JWT token & refresh token
- Quản lý profile khách hàng
- Phân quyền 4 roles: Admin, Manager, Staff, Customer

**Key Features**:
- BCrypt password hashing
- JWT authentication
- Role-based authorization
- Rate limiting

---

### 🎟 2. Booking Management
**File**: [`modules/BookingAPI.md`](./modules/BookingAPI.md)

**APIs**: 13 endpoints
- Tạo booking và chọn suất chiếu
- Chọn ghế với real-time validation
- Thêm combo đồ ăn/nước uống
- Áp dụng voucher & khuyến mãi
- Xử lý hủy vé & hoàn tiền

**Key Features**:
- Seat locking mechanism (15 phút)
- Ngăn chặn đặt ghế trùng (UQ_ShowtimeSeat)
- Tính toán giá động
- Status state machine

---

### 💳 3. Payment Integration
**File**: [`modules/PaymentAPI.md`](./modules/PaymentAPI.md)

**APIs**: 12 endpoints
- Tích hợp VNPay
- Xử lý callback từ payment gateway
- Quản lý giao dịch & refund
- Lịch sử thanh toán

**Key Features**:
- Signature validation (HMAC SHA256/512)
- Idempotency handling
- Async payment processing
- Refund flow theo policy

---

### 🎬 4-9. Các Module khác

| Module | File | Endpoints | Mô tả |
|--------|------|-----------|-------|
| Movie Management | `MovieAPI.md` | 10 | CRUD phim, upload poster, tìm kiếm |
| Cinema Management | `CinemaAPI.md` | 14 | Quản lý rạp, phòng chiếu, sơ đồ ghế |
| Showtime | `ShowtimeAPI.md` | 10 | Lịch chiếu, giá vé, format phim |
| Promotion | `PromotionAPI.md` | 11 | Voucher, khuyến mãi, discount engine |
| Review | `ReviewAPI.md` | 6 | Đánh giá phim, rating, comment |
| Admin | `AdminAPI.md` | 17 | Dashboard, báo cáo, thống kê |

---

## 🔄 Flow Documents

### 📱 User Flow
**File**: [`flow/UserFlow.md`](./flow/UserFlow.md)

Mô tả hành trình đầy đủ của khách hàng:
```
Discover → Register → Login → Select → Booking → Payment → Enjoy → Review
```

Chi tiết từng bước với:
- UI mockup description
- API calls sequence
- Error handling
- Alternative flows

---

### 🔐 Auth Flow
**File**: [`flow/AuthFlow.md`](./flow/AuthFlow.md)

Giải thích cơ chế xác thực:
- Registration flow với validation
- Login flow với JWT generation
- Token refresh mechanism
- Role-based authorization
- Security best practices

---

### 🔄 Data Flow
**File**: [`flow/DataFlow.md`](./flow/DataFlow.md)

Dòng dữ liệu giữa các bảng:
- Complete booking flow (từng bước SQL)
- Entity relationships
- Complex queries
- Data consistency rules
- Performance optimization

---

## 📊 Key Statistics

| Metric | Value |
|--------|-------|
| **Total API Endpoints** | 111 |
| **Total Database Tables** | 18 |
| **Module Count** | 9 |
| **Flow Documents** | 3 |
| **Development Timeline** | 8 weeks |

---

## 🎯 Quy ước chung

### Response Format

#### ✅ Success Response
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Operation successful",
  "data": { ... }
}
```

#### ❌ Error Response
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Error message",
  "errorCode": "ERROR_CODE",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

---

### HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Request thành công |
| 201 | Created | Tạo resource thành công |
| 204 | No Content | Xóa thành công |
| 400 | Bad Request | Input không hợp lệ |
| 401 | Unauthorized | Chưa đăng nhập |
| 403 | Forbidden | Không có quyền |
| 404 | Not Found | Resource không tồn tại |
| 409 | Conflict | Duplicate hoặc conflict |
| 422 | Unprocessable | Validation failed |
| 500 | Server Error | Lỗi server |

---

### Pagination

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

Query params: `?page=1&pageSize=10`

---

### Authentication

Tất cả protected endpoints yêu cầu **Bearer Token**:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 🛠 Development Guidelines

### 1. Implement theo Priority

**Week 1-2**: Core APIs
- ✅ User (Register, Login, Profile)
- ✅ Movie & Cinema (Browse, Search)
- ✅ Showtime (List, Detail)

**Week 3-4**: Booking Flow
- ✅ Booking (Create, Select Seats)
- ✅ Combo Management
- ✅ Voucher & Promotion

**Week 5-6**: Payment & Admin
- ✅ Payment Integration (VNPay đầu tiên)
- ✅ Admin Dashboard
- ✅ Reports

---

### 2. Testing Strategy

```
Unit Tests → Integration Tests → E2E Tests
```

**Target Coverage**: > 70%

**Key Test Cases**:
- Authentication flow
- Booking với seat locking
- Payment callback handling
- Concurrent booking prevention

---

### 3. Database Setup

```bash
# 1. Tạo database trong PostgreSQL
psql -U postgres -f DatabaseScript.txt
# Hoặc sử dụng pgAdmin để chạy script

# 2. Chạy migrations
dotnet ef database update

# 3. Seed initial data
dotnet run --seed
```

---

### 4. API Documentation Tools

- **Swagger/OpenAPI**: Auto-generated từ code
- **Postman Collection**: Import từ Swagger
- **Markdown Docs**: Các file trong /docs này

---

## 📞 Support & Resources

### 📚 Additional Reading

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://jwt.io/)
- [VNPay Documentation](https://sandbox.vnpayment.vn/apis/)

---

### 🤝 Contributing

Khi cập nhật API docs:

1. Giữ nguyên format và structure
2. Viết mô tả bằng tiếng Việt rõ ràng
3. Code, endpoint, field names bằng tiếng Anh
4. Cập nhật version và last updated date
5. Test tất cả code examples

---

### 📝 Changelog

| Version | Date | Changes |
|---------|------|---------|
| v1.0 | 2025-10-29 | Initial API documentation |

---

## ⚡ Quick Reference

### Most Used APIs

```http
# Login
POST /api/auth/login

# Browse movies
GET /api/movies/now-showing

# Get showtimes
GET /api/movies/{id}/showtimes

# Create booking
POST /api/bookings/create

# Select seats
POST /api/bookings/{id}/select-seats

# Payment VNPay
POST /api/payments/vnpay/create

# My bookings
GET /api/bookings/my-bookings
```

---

### Database Tables

**Core Tables**: User, Customers, Movies, Cinemas, Auditoriums, Seats, Showtimes

**Booking Tables**: Bookings, BookingSeats, BookingCombos, BookingPromotions

**Payment Tables**: Payments, PaymentMethods

**Marketing Tables**: Vouchers, Promotions

**Other**: Reviews, Roles, Combos

---

## 🎓 Learning Path

### Cho Backend Developer mới

1. **Tuần 1**: Đọc Overview + User Flow
2. **Tuần 2**: Implement UserAPI (Auth)
3. **Tuần 3**: Implement MovieAPI + CinemaAPI
4. **Tuần 4**: Implement BookingAPI (phức tạp nhất)
5. **Tuần 5**: Implement PaymentAPI (VNPay)
6. **Tuần 6**: Implement Admin APIs
7. **Tuần 7-8**: Testing & Optimization

---

### Cho Frontend Developer

1. Đọc **UserFlow.md** để hiểu UX
2. Đọc **API_List.md** để biết endpoints nào cần gọi
3. Đọc từng module API để biết request/response format
4. Implement theo flow: Auth → Browse → Booking → Payment

---

## 🔗 Quick Links

- 📖 [Overview](./Overview.md)
- 📋 [API List](./API_List.md)
- 👤 [User API](./modules/UserAPI.md)
- 🎟 [Booking API](./modules/BookingAPI.md)
- 💳 [Payment API](./modules/PaymentAPI.md)
- 📱 [User Flow](./flow/UserFlow.md)
- 🔐 [Auth Flow](./flow/AuthFlow.md)
- 🔄 [Data Flow](./flow/DataFlow.md)

---

**Last Updated**: October 29, 2025  
**Documentation Version**: v1.0  
**Project**: Movie88 - Cinema Booking System

---

**Happy Coding! 🚀**
