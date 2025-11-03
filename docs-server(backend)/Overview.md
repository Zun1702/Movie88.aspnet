#🎬 Movie88 - Tổng quan Hệ thống API

## 📋 Giới thiệu

**Movie88** là hệ thống đặt vé xem phim trực tuyến hiện đại, cung cấp trải nghiệm mượt mà và trực quan cho khán giả. Hệ thống cho phép người dùng duyệt phim, chọn suất chiếu, chọn ghế qua sơ đồ tương tác, và thanh toán an toàn qua nhiều cổng thanh toán.

## 🎯 Mục tiêu Hệ thống

- ✅ Xây dựng cơ sở dữ liệu vững chắc, ngăn chặn đặt ghế trùng lặp
- ✅ Tạo mô hình dữ liệu nhất quán kết nối users, customers, bookings, và payments
- ✅ Cung cấp trải nghiệm khách hàng mượt mà, chuyên nghiệp
- ✅ Hỗ trợ mở rộng module (hệ thống đa rạp, chương trình khách hàng thân thiết, ứng dụng di động)

## 🛠 Technology Stack

| Thành phần | Công nghệ |
|-----------|-----------|
| Backend | ASP.NET Core Web API (3-Layer Architecture) |
| Frontend | ReactJS / NextJS |
| Database | PostgreSQL |
| Payment Gateway | VNPay |
| Authentication | JWT Token |
| ORM | Entity Framework Core |

## 🏗 Kiến trúc Hệ thống

### 3-Layer Architecture

```
┌─────────────────────────────────────────────┐
│           PRESENTATION LAYER                │
│  (Controllers - API Endpoints)              │
│  - Nhận HTTP requests                       │
│  - Validate input DTOs                      │
│  - Gọi Business Logic Layer                 │
│  - Trả về HTTP responses                    │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌─────────────────────────────────────────────┐
│          BUSINESS LOGIC LAYER               │
│  (Services)                                 │
│  - Xử lý business rules                     │
│  - Orchestrate operations                   │
│  - Gọi Data Access Layer                    │
│  - Transform data giữa layers               │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌─────────────────────────────────────────────┐
│          DATA ACCESS LAYER                  │
│  (Repositories)                             │
│  - CRUD operations                          │
│  - Queries với Entity Framework             │
│  - Database transactions                    │
│  - Data mapping                             │
└────────────────┬────────────────────────────┘
                 │
                 ↓
         ┌───────────────┐
         │  PostgreSQL   │
         │   Database    │
         └───────────────┘
```

### System Components

```
┌─────────────────┐
│  Client Apps    │
│ (Web + Mobile)  │
└────────┬────────┘
         │
         ↓
┌─────────────────┐
│   API Gateway   │
│  (ASP.NET Core) │
│   Controllers   │
└────────┬────────┘
         │
    ┌────┴────┐
    │         │
    ↓         ↓
┌─────┐   ┌──────┐
│ SQL │   │VNPay │
│Server│  │Gateway│
└─────┘   └──────┘
```

## 📦 Phân Module Hệ thống

### 1. 👤 User Management Module
- Quản lý người dùng và phân quyền
- Đăng ký, đăng nhập, xác thực
- Quản lý thông tin khách hàng

### 2. 🎬 Movie Management Module
- Quản lý thông tin phim
- Thể loại, trailer, poster
- Lịch chiếu và đánh giá

### 3. 🏢 Cinema Management Module
- Quản lý cụm rạp
- Phòng chiếu và sơ đồ ghế
- Cấu hình chỗ ngồi

### 4. 🕒 Showtime Management Module
- Quản lý suất chiếu
- Lịch chiếu theo rạp/phim
- Giá vé theo suất chiếu

### 5. 🎟 Booking Management Module
- Đặt vé và chọn ghế
- Quản lý combo đồ ăn
- Xử lý booking status

### 6. 💸 Payment Module
- Tích hợp cổng thanh toán
- Xử lý giao dịch
- Lịch sử thanh toán

### 7. 🎁 Promotion & Voucher Module
- Quản lý khuyến mãi
- Mã giảm giá
- Áp dụng ưu đãi

### 8. ⭐ Review Module
- Đánh giá phim
- Bình luận và rating
- Phản hồi khách hàng

## 🗓 Timeline Phát triển API

### 📌 Giai đoạn 1: Phân tích & Thiết kế (Tuần 1)
**Mục tiêu:** Hoàn thành phân tích và thiết kế hệ thống

- [ ] Phân tích chi tiết Functional Requirement
- [ ] Thiết kế sơ đồ ERD & xác nhận DatabaseScript
- [ ] Định nghĩa API Contract và chuẩn Response
- [ ] Setup môi trường development (PostgreSQL, .NET 8)
- [ ] Tạo project structure và base configuration

**Deliverables:**
- ERD Diagram
- API Documentation v1.0
- Project setup hoàn chỉnh

---

### 📌 Giai đoạn 2: API Core (Tuần 2-3)
**Mục tiêu:** Xây dựng các API module cốt lõi

#### Tuần 2: Authentication & User Management
- [ ] Implement User API (Register, Login, Profile)
- [ ] JWT Authentication & Authorization
- [ ] Role-based Access Control
- [ ] Customer Profile Management

#### Tuần 3: Movie & Cinema Management
- [ ] Movie CRUD API
- [ ] Cinema & Auditorium API
- [ ] Seat Management API
- [ ] Showtime API

**Deliverables:**
- User Authentication hoạt động
- Movie & Cinema data có thể quản lý qua API

---

### 📌 Giai đoạn 3: API Nghiệp vụ Chính (Tuần 4-5)
**Mục tiêu:** Hoàn thiện flow đặt vé và thanh toán

#### Tuần 4: Booking System
- [ ] Booking Flow API
- [ ] Seat Selection & Lock mechanism
- [ ] Real-time seat availability check
- [ ] Combo Management API
- [ ] Booking validation & business rules

#### Tuần 5: Payment Integration
- [ ] Payment Gateway Integration (VNPay)
- [ ] Payment callback handling
- [ ] Transaction history API
- [ ] Voucher & Promotion API
- [ ] Discount calculation engine

**Deliverables:**
- Hoàn chỉnh flow: Chọn phim → Chọn ghế → Thanh toán VNPay → Xác nhận
- VNPay payment gateway test thành công

---

### 📌 Giai đoạn 4: API Mở rộng (Tuần 6-7)
**Mục tiêu:** Các tính năng bổ sung và tối ưu

#### Tuần 6: Review & Report
- [ ] Review & Rating API
- [ ] Admin Dashboard APIs
- [ ] Revenue Report API
- [ ] Booking Statistics API
- [ ] User behavior analytics

#### Tuần 7: Optimization & Admin Tools
- [ ] Admin Management APIs
- [ ] Bulk operations (import movies, schedules)
- [ ] Caching strategy implementation
- [ ] Performance optimization
- [ ] API rate limiting

**Deliverables:**
- Admin panel có đủ API để quản lý
- Hệ thống hoạt động ổn định với performance tốt

---

### 📌 Giai đoạn 5: Testing & Documentation (Tuần 8)
**Mục tiêu:** Đảm bảo chất lượng và tài liệu hóa

- [ ] Unit Testing (Coverage > 70%)
- [ ] Integration Testing
- [ ] API Testing với Postman/Swagger
- [ ] Security Testing (OWASP)
- [ ] Hoàn thiện API Documentation
- [ ] Viết User Guide và Developer Guide
- [ ] Load Testing & Performance Tuning

**Deliverables:**
- Test coverage report
- Complete API Documentation
- Deployment guide

---

### 📌 Giai đoạn 6: Deployment & Monitoring (Tuần 9)
**Mục tiêu:** Deploy và giám sát hệ thống

- [ ] Setup Staging Environment
- [ ] Deploy to Production
- [ ] Setup Monitoring & Logging
- [ ] Setup CI/CD Pipeline
- [ ] Health Check APIs
- [ ] Error tracking và alerting

**Deliverables:**
- Hệ thống live trên production
- Monitoring dashboard hoạt động
- Runbook cho operations

---

## 📊 Metrics & KPIs

### Technical Metrics
- API Response Time: < 200ms (95 percentile)
- Error Rate: < 0.1%
- Uptime: > 99.9%
- Database Query Time: < 100ms

### Business Metrics
- Successful Booking Rate: > 95%
- Payment Success Rate: > 98%
- Concurrent Users Support: > 1000
- Seat Double-booking Rate: 0%

## 🔒 Security Considerations

1. **Authentication**: JWT với refresh token
2. **Authorization**: Role-based access control (RBAC)
3. **Data Protection**: Encryption at rest và in transit
4. **API Security**: Rate limiting, CORS configuration
5. **Payment Security**: PCI DSS compliance
6. **Input Validation**: Tất cả input phải được validate
7. **SQL Injection Prevention**: Sử dụng parameterized queries
8. **XSS Protection**: Output encoding

## 📚 Document Structure

```
/docs
│
├── Overview.md               # File này - Tổng quan hệ thống
├── API_List.md               # Danh sách tổng hợp toàn bộ API
│
├── /modules
│   ├── UserAPI.md           # API quản lý người dùng
│   ├── MovieAPI.md          # API quản lý phim
│   ├── CinemaAPI.md         # API quản lý rạp
│   ├── ShowtimeAPI.md       # API quản lý suất chiếu
│   ├── BookingAPI.md        # API đặt vé
│   ├── PaymentAPI.md        # API thanh toán
│   ├── PromotionAPI.md      # API khuyến mãi
│   ├── ReviewAPI.md         # API đánh giá
│   └── AdminAPI.md          # API quản trị
│
└── /flow
    ├── UserFlow.md          # Flow nghiệp vụ người dùng
    ├── DataFlow.md          # Dòng dữ liệu giữa các bảng
    └── AuthFlow.md          # Flow đăng nhập và phân quyền
```

## 🚀 Quick Start

1. **Setup Database**: Chạy `DatabaseScript.txt` trong PostgreSQL
2. **Configure Connection**: Update `appsettings.json` với PostgreSQL connection string
3. **Run Migration**: `dotnet ef database update`
4. **Start API**: `dotnet run`
5. **Access Swagger**: `https://localhost:5001/swagger`

## 📞 Support & Contact

- **Technical Lead**: [Your Name]
- **Project Repository**: [GitHub Link]
- **API Documentation**: [Swagger URL]
- **Issue Tracking**: [Jira/GitHub Issues]

---

**Last Updated**: October 29, 2025
**Version**: 1.0.0
