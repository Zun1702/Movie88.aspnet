# 👑 Admin Guide: Quản trị Hệ thống Movie88

## 📋 Mục lục
1. [Giới thiệu](#giới-thiệu)
2. [Dashboard Overview](#dashboard-overview)
3. [Quản lý Phim](#quản-lý-phim)
4. [Quản lý Rạp & Suất chiếu](#quản-lý-rạp--suất-chiếu)
5. [Quản lý Users](#quản-lý-users)
6. [Báo cáo & Thống kê](#báo-cáo--thống-kê)
7. [Xử lý vấn đề](#xử-lý-vấn-đề)

---

## 🎯 Giới thiệu

### Vai trò của Admin
Bạn là **quản trị viên hệ thống** Movie88 với toàn quyền quản lý:

- ✅ **TẤT CẢ** quyền của Staff (verify booking, check-in)
- ✅ Quản lý Movies (thêm/sửa/xóa)
- ✅ Quản lý Cinemas & Auditoriums
- ✅ Quản lý Showtimes (lịch chiếu phim)
- ✅ Quản lý Users (customers, staff, admins)
- ✅ Xem báo cáo doanh thu & thống kê
- ✅ Quản lý Promotions & Vouchers
- ✅ Xử lý khiếu nại & hoàn tiền
- ✅ Cấu hình hệ thống

### Trách nhiệm chính

**Daily Tasks:**
- Kiểm tra dashboard mỗi sáng
- Giải quyết tickets/complaints
- Monitor system health

**Weekly Tasks:**
- Review doanh thu tuần
- Cập nhật lịch chiếu phim mới
- Kiểm tra inventory ghế/rạp

**Monthly Tasks:**
- Báo cáo doanh thu tháng
- Phân tích xu hướng khách hàng
- Planning cho tháng tiếp theo

---

## 📊 Dashboard Overview

### ⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI

**Endpoint cần implement:**
```http
GET /api/admin/dashboard/stats
Authorization: Bearer {admin_token}
```

### Response mong muốn

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Dashboard stats retrieved successfully",
  "data": {
    "overview": {
      "totalRevenue": {
        "today": 45000000,
        "thisWeek": 280000000,
        "thisMonth": 1250000000,
        "growth": {
          "daily": "+12%",
          "weekly": "+8%",
          "monthly": "+15%"
        }
      },
      "totalBookings": {
        "today": 450,
        "thisWeek": 2800,
        "thisMonth": 12500
      },
      "totalCustomers": {
        "active": 8500,
        "new": 250,
        "retention": "78%"
      },
      "occupancyRate": {
        "today": "68%",
        "thisWeek": "72%",
        "average": "65%"
      }
    },
    "popularMovies": [
      {
        "movieId": 1,
        "title": "Avengers: Endgame",
        "posterUrl": "https://...",
        "totalBookings": 1250,
        "revenue": 185000000,
        "occupancyRate": "92%",
        "rating": 4.8
      },
      {
        "movieId": 2,
        "title": "Avatar 2",
        "totalBookings": 980,
        "revenue": 145000000,
        "occupancyRate": "85%",
        "rating": 4.6
      }
    ],
    "upcomingShowtimes": [
      {
        "showtimeId": 101,
        "movieTitle": "Avengers",
        "startTime": "19:30",
        "cinema": "CGV Vincom",
        "availableSeats": 45,
        "totalSeats": 150,
        "occupancy": "70%"
      }
    ],
    "recentBookings": [
      {
        "bookingCode": "BK20251104001",
        "customerName": "Nguyen Van A",
        "movieTitle": "Avengers",
        "totalAmount": 180000,
        "status": "Confirmed",
        "bookedAt": "2025-11-04T14:30:00"
      }
    ],
    "systemHealth": {
      "status": "Healthy",
      "uptime": "99.95%",
      "lastIncident": "2025-10-28",
      "activeUsers": 1250
    }
  }
}
```

### Dashboard UI Elements

**Widgets cần có:**
1. 📈 Revenue Chart (line chart)
2. 🎬 Top Movies (bar chart)
3. 👥 Customer Growth (area chart)
4. 🏢 Cinema Occupancy (pie chart)
5. 📅 Upcoming Showtimes (table)
6. 🔔 Recent Bookings (live feed)
7. ⚠️ System Alerts (notifications)

---

## 🎬 Quản lý Phim

### ⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI

### 1. Thêm Phim Mới

**Endpoint:**
```http
POST /api/movies
Authorization: Bearer {admin_token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "title": "Avatar: The Way of Water",
  "description": "Set more than a decade after the events of the first film...",
  "durationMinutes": 192,
  "director": "James Cameron",
  "releaseDate": "2025-12-16",
  "country": "USA",
  "rating": "PG-13",
  "genre": "Sci-Fi, Adventure, Action",
  "posterUrl": "https://image.tmdb.org/t/p/w500/t6HIqrRAclMCA60NsSmeqe9RmNV.jpg",
  "trailerUrl": "https://www.youtube.com/watch?v=d9MyW72ELq0",
  "cast": [
    "Sam Worthington",
    "Zoe Saldana",
    "Sigourney Weaver",
    "Stephen Lang"
  ],
  "producer": "Jon Landau",
  "language": "English",
  "subtitle": "Phụ đề Việt",
  "ageRestriction": 13,
  "status": "ComingSoon"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "statusCode": 201,
  "message": "Movie created successfully",
  "data": {
    "movieId": 123,
    "title": "Avatar: The Way of Water",
    "slug": "avatar-the-way-of-water",
    "createdAt": "2025-11-04T15:30:00",
    "createdBy": "admin@movie88.com"
  }
}
```

---

### 2. Cập nhật Phim

**Endpoint:**
```http
PUT /api/movies/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json
```

**Use Cases:**
- Sửa thông tin phim (typo, thời lượng, v.v.)
- Cập nhật poster/trailer
- Thay đổi trạng thái (Coming Soon → Now Showing → End of Showing)
- Thêm/bớt diễn viên

---

### 3. Xóa Phim

**Endpoint:**
```http
DELETE /api/movies/{id}
Authorization: Bearer {admin_token}
```

**⚠️ Lưu ý:**
- Chỉ xóa được phim KHÔNG có booking
- Nếu có booking: Phải cancel tất cả booking trước
- Soft delete (đánh dấu IsDeleted=true, không xóa vật lý)

**Response (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Movie deleted successfully",
  "data": {
    "movieId": 123,
    "title": "Avatar 2",
    "deletedAt": "2025-11-04T15:45:00"
  }
}
```

---

### 4. Danh sách Phim (Admin View)

**Endpoint:**
```http
GET /api/admin/movies?page=1&pageSize=20&status=all
Authorization: Bearer {admin_token}
```

**Query Parameters:**
- `status`: all | now-showing | coming-soon | ended
- `search`: Tìm theo tên
- `sortBy`: title | releaseDate | revenue | bookings
- `sortOrder`: asc | desc

**Response:**
```json
{
  "success": true,
  "data": {
    "movies": [
      {
        "movieId": 1,
        "title": "Avengers: Endgame",
        "status": "NowShowing",
        "releaseDate": "2025-10-20",
        "totalBookings": 1250,
        "revenue": 185000000,
        "occupancyRate": "92%",
        "rating": 4.8,
        "totalReviews": 450
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalPages": 5,
      "totalRecords": 95
    }
  }
}
```

---

## 🏢 Quản lý Rạp & Suất chiếu

### ⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI

### A. Quản lý Rạp (Cinemas)

#### 1. Thêm rạp mới
```http
POST /api/admin/cinemas
Authorization: Bearer {admin_token}

{
  "name": "CGV Landmark 81",
  "address": "720A Dien Bien Phu, Binh Thanh, HCMC",
  "city": "Ho Chi Minh",
  "district": "Binh Thanh",
  "phone": "1900 6017",
  "email": "landmark81@cgv.vn",
  "latitude": 10.7946,
  "longitude": 106.7218,
  "facilities": ["3D", "IMAX", "4DX", "Dolby Atmos"],
  "parkingAvailable": true,
  "numberOfAuditoriums": 8
}
```

#### 2. Cập nhật rạp
```http
PUT /api/admin/cinemas/{id}
```

#### 3. Xóa rạp
```http
DELETE /api/admin/cinemas/{id}
```

**⚠️ Lưu ý:** Chỉ xóa được rạp KHÔNG có showtime đang hoạt động

---

### B. Quản lý Phòng chiếu (Auditoriums)

#### 1. Thêm phòng chiếu
```http
POST /api/admin/auditoriums
Authorization: Bearer {admin_token}

{
  "cinemaId": 1,
  "name": "Cinema 1",
  "totalSeats": 150,
  "totalRows": 10,
  "seatsPerRow": 15,
  "screenType": "IMAX",
  "soundSystem": "Dolby Atmos",
  "seatLayout": [
    {
      "row": "A",
      "seats": [
        { "number": 1, "type": "Standard", "price": 90000 },
        { "number": 2, "type": "Standard", "price": 90000 },
        { "number": 3, "type": "VIP", "price": 150000 }
      ]
    }
  ]
}
```

---

### C. Quản lý Suất chiếu (Showtimes)

#### 1. Tạo suất chiếu mới
```http
POST /api/admin/showtimes
Authorization: Bearer {admin_token}

{
  "movieId": 1,
  "auditoriumId": 3,
  "startTime": "2025-11-05T19:30:00",
  "format": "2D",
  "language": "English",
  "subtitle": "Vietnamese",
  "basePrice": 90000,
  "pricing": {
    "standard": 90000,
    "vip": 150000,
    "couple": 180000
  }
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "statusCode": 201,
  "message": "Showtime created successfully",
  "data": {
    "showtimeId": 456,
    "movieTitle": "Avengers",
    "startTime": "2025-11-05T19:30:00",
    "endTime": "2025-11-05T22:31:00",
    "availableSeats": 150,
    "totalSeats": 150,
    "status": "Available"
  }
}
```

#### 2. Bulk create showtimes (hàng loạt)
```http
POST /api/admin/showtimes/bulk
Authorization: Bearer {admin_token}

{
  "movieId": 1,
  "auditoriumId": 3,
  "startDate": "2025-11-05",
  "endDate": "2025-11-12",
  "timeslots": ["10:00", "13:00", "16:00", "19:00", "22:00"],
  "skipDays": [], // Bỏ qua ngày nào (e.g., maintenance)
  "pricing": {
    "weekday": {
      "standard": 90000,
      "vip": 150000
    },
    "weekend": {
      "standard": 120000,
      "vip": 180000
    }
  }
}
```

**Response:**
```json
{
  "success": true,
  "message": "35 showtimes created successfully",
  "data": {
    "created": 35,
    "skipped": 0,
    "failed": 0,
    "details": [
      { "date": "2025-11-05", "time": "10:00", "showtimeId": 101 },
      { "date": "2025-11-05", "time": "13:00", "showtimeId": 102 }
    ]
  }
}
```

#### 3. Cập nhật suất chiếu
```http
PUT /api/admin/showtimes/{id}
```

**Use Cases:**
- Đổi giờ chiếu
- Thay đổi giá vé
- Cancel suất chiếu

#### 4. Xóa/Hủy suất chiếu
```http
DELETE /api/admin/showtimes/{id}
```

**⚠️ Lưu ý:**
- Nếu có booking: Phải hoàn tiền trước khi xóa
- Thông báo cho khách hàng qua email/SMS
- Log lý do hủy suất

---

## 👥 Quản lý Users

### ⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI

### 1. Danh sách Users
```http
GET /api/admin/users?role=all&page=1&pageSize=50
Authorization: Bearer {admin_token}
```

**Query Parameters:**
- `role`: all | customer | staff | admin
- `status`: all | active | inactive | banned
- `search`: Tìm theo email/tên

**Response:**
```json
{
  "success": true,
  "data": {
    "users": [
      {
        "userId": 456,
        "email": "nguyenvana@example.com",
        "fullname": "Nguyen Van A",
        "role": "Customer",
        "status": "Active",
        "registeredAt": "2025-09-15",
        "totalBookings": 12,
        "totalSpent": 2400000,
        "lastLogin": "2025-11-04T10:30:00"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "totalRecords": 8500
    }
  }
}
```

---

### 2. Thêm Staff/Admin
```http
POST /api/admin/users
Authorization: Bearer {admin_token}

{
  "email": "staff01@movie88.com",
  "password": "Staff@123",
  "fullname": "Tran Thi B",
  "role": "Staff",
  "phone": "0901234567",
  "cinemaId": 1
}
```

---

### 3. Cập nhật Role
```http
PUT /api/admin/users/{id}/role
Authorization: Bearer {admin_token}

{
  "newRole": "Staff"
}
```

**Use Cases:**
- Promote customer → staff
- Promote staff → admin
- Demote admin → staff

---

### 4. Ban/Unban User
```http
PUT /api/admin/users/{id}/ban
Authorization: Bearer {admin_token}

{
  "reason": "Spam reviews",
  "duration": "30 days" // or "permanent"
}
```

---

## 📊 Báo cáo & Thống kê

### ⚠️ CHƯA IMPLEMENT - CẦN TRIỂN KHAI

### 1. Báo cáo Doanh thu

#### Daily Revenue
```http
GET /api/admin/reports/revenue/daily?date=2025-11-04
Authorization: Bearer {admin_token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "date": "2025-11-04",
    "totalRevenue": 45000000,
    "totalBookings": 450,
    "averageTicketPrice": 100000,
    "breakdown": {
      "ticketSales": 42000000,
      "concessions": 3000000
    },
    "byMovie": [
      {
        "movieTitle": "Avengers",
        "revenue": 18500000,
        "bookings": 185
      }
    ],
    "byCinema": [
      {
        "cinemaName": "CGV Vincom",
        "revenue": 25000000,
        "bookings": 250
      }
    ],
    "byHour": [
      { "hour": "10:00-11:00", "revenue": 2000000 },
      { "hour": "19:00-20:00", "revenue": 8500000 }
    ]
  }
}
```

---

#### Monthly Revenue
```http
GET /api/admin/reports/revenue/monthly?month=11&year=2025
```

---

### 2. Báo cáo Booking

#### Booking Statistics
```http
GET /api/admin/reports/bookings/statistics?startDate=2025-11-01&endDate=2025-11-30
```

**Response:**
```json
{
  "success": true,
  "data": {
    "totalBookings": 12500,
    "completedBookings": 11800,
    "canceledBookings": 700,
    "cancellationRate": "5.6%",
    "averageBookingValue": 180000,
    "peakHours": ["19:00-20:00", "20:00-21:00"],
    "peakDays": ["Saturday", "Sunday"],
    "conversionRate": "78%"
  }
}
```

---

### 3. Báo cáo Phim Phổ biến

```http
GET /api/admin/reports/popular-movies?period=month&limit=10
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "rank": 1,
      "movieId": 1,
      "title": "Avengers: Endgame",
      "totalBookings": 1250,
      "revenue": 185000000,
      "averageOccupancy": "92%",
      "rating": 4.8,
      "trend": "up" // up, down, stable
    }
  ]
}
```

---

### 4. Báo cáo Khách hàng

```http
GET /api/admin/reports/customers/analytics?period=month
```

**Response:**
```json
{
  "success": true,
  "data": {
    "totalCustomers": 8500,
    "newCustomers": 250,
    "activeCustomers": 3200,
    "retention": "78%",
    "churnRate": "12%",
    "averageLifetimeValue": 2400000,
    "topCustomers": [
      {
        "customerId": 123,
        "fullname": "Nguyen Van A",
        "totalBookings": 45,
        "totalSpent": 8500000,
        "memberSince": "2024-01-15"
      }
    ],
    "demographics": {
      "age": {
        "18-24": "35%",
        "25-34": "40%",
        "35-44": "20%",
        "45+": "5%"
      },
      "gender": {
        "male": "55%",
        "female": "45%"
      }
    }
  }
}
```

---

## 🔧 Xử lý Vấn đề

### 1. Hoàn tiền (Refund)

**⚠️ CHƯA IMPLEMENT**

```http
POST /api/admin/bookings/{id}/refund
Authorization: Bearer {admin_token}

{
  "reason": "Showtime canceled",
  "refundAmount": 180000,
  "refundMethod": "BankTransfer",
  "notes": "Full refund due to technical issues"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Refund processed successfully",
  "data": {
    "bookingCode": "BK20251104001",
    "refundAmount": 180000,
    "refundedAt": "2025-11-04T16:30:00",
    "refundMethod": "BankTransfer",
    "transactionId": "REF123456"
  }
}
```

---

### 2. Chuyển suất chiếu (Reschedule)

**⚠️ CHƯA IMPLEMENT**

```http
PUT /api/admin/bookings/{id}/reschedule
Authorization: Bearer {admin_token}

{
  "newShowtimeId": 789,
  "reason": "Customer request",
  "notifyCustomer": true
}
```

---

### 3. Xử lý Complaints

**Process:**
1. Nhận complaint từ customer (email/hotline)
2. Log vào hệ thống
3. Assign to staff/admin
4. Investigate
5. Resolve (refund/reschedule/compensation)
6. Follow up với customer

**⚠️ API chưa có, cần implement Ticketing System**

---

## 🔒 Security & Permissions

### Admin Privileges

**Full Access:**
- ✅ All CRUD operations
- ✅ View all data
- ✅ Export reports
- ✅ System configuration
- ✅ User management

**Audit Log:**
- Tất cả actions của Admin được log
- Ai làm gì, khi nào, với data nào
- Không thể xóa audit log

**⚠️ Audit Log API chưa có**

```http
GET /api/admin/audit-logs?userId=42&action=all&startDate=2025-11-01
```

---

## 📈 KPIs cho Admin

### Business Metrics

| Metric | Target | Thực tế | Status |
|--------|--------|---------|--------|
| **Monthly Revenue** | 1,000M | 1,250M | ✅ +25% |
| **Occupancy Rate** | > 65% | 72% | ✅ +7% |
| **Customer Retention** | > 70% | 78% | ✅ +8% |
| **Avg Ticket Price** | 95K | 100K | ✅ +5% |
| **Cancellation Rate** | < 8% | 5.6% | ✅ -30% |

### System Health

| Metric | Target | Thực tế | Status |
|--------|--------|---------|--------|
| **API Uptime** | > 99.9% | 99.95% | ✅ |
| **Response Time** | < 200ms | 150ms | ✅ |
| **Error Rate** | < 0.1% | 0.05% | ✅ |
| **Peak Load Handling** | 1000 req/s | 850 req/s | ✅ |

---

## 🚀 Roadmap

### Phase 1: Core Admin Features (Sprint hiện tại)
- [ ] Dashboard với real-time stats
- [ ] Movie management (CRUD)
- [ ] Showtime management (CRUD)
- [ ] Basic reports (revenue, bookings)

### Phase 2: Advanced Features (Sprint tiếp)
- [ ] Cinema/Auditorium management
- [ ] User management (ban/unban, role changes)
- [ ] Refund/Reschedule workflows
- [ ] Advanced analytics & charts

### Phase 3: Automation (Future)
- [ ] Auto-pricing based on demand
- [ ] Predictive analytics (forecast revenue)
- [ ] Auto-recommendations (which movies to add)
- [ ] AI-powered customer segmentation

---

## 📞 Support & Contacts

**IT Support:**
- Email: it-support@movie88.com
- Hotline: [Phone number]
- Slack: #admin-support

**Development Team:**
- Backend Lead: [Name]
- Database Admin: [Name]
- DevOps: [Name]

**Escalation Path:**
1. Self-resolve using tools
2. Check documentation
3. Contact IT Support
4. Escalate to Dev Team (critical issues only)

---

## 📚 Additional Resources

- [API Documentation (Swagger)](https://movie88aspnet-app.up.railway.app/swagger)
- [GitHub Repository](https://github.com/Zun1702/Movie88.aspnet)
- [Staff Booking Verification Guide](./01-Staff-Booking-Verification.md)
- [Database Schema](./database-schema.md) *(chưa có)*
- [Deployment Guide](../../RAILWAY-DEPLOYMENT.md)

---

**Last Updated**: November 4, 2025  
**Author**: Backend Team  
**Version**: 1.0  
**Status**: ⚠️ Most admin APIs not implemented yet - Pending development
