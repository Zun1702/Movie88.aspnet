# 👑 Admin: Quản trị Hệ thống Movie88 (20+ Endpoints)

**Status**: ⚠️ **PENDING IMPLEMENTATION** (0/20+ endpoints - 0%)

---

## 📋 Endpoints Overview

### A. Movie Management (4 endpoints)
| # | Method | Endpoint | Description | Auth | Status |
|---|--------|----------|-------------|------|--------|
| 1 | POST | `/api/movies` | Thêm phim mới | ✅ Admin | ⏳ TODO |
| 2 | PUT | `/api/movies/{id}` | Cập nhật phim | ✅ Admin | ⏳ TODO |
| 3 | DELETE | `/api/movies/{id}` | Xóa phim | ✅ Admin | ⏳ TODO |
| 4 | GET | `/api/admin/movies` | Danh sách phim (admin view) | ✅ Admin | ⏳ TODO |

### B. Cinema & Showtime Management (6 endpoints)
| # | Method | Endpoint | Description | Auth | Status |
|---|--------|----------|-------------|------|--------|
| 5 | POST | `/api/admin/cinemas` | Thêm rạp mới | ✅ Admin | ⏳ TODO |
| 6 | PUT | `/api/admin/cinemas/{id}` | Cập nhật rạp | ✅ Admin | ⏳ TODO |
| 7 | DELETE | `/api/admin/cinemas/{id}` | Xóa rạp | ✅ Admin | ⏳ TODO |
| 8 | POST | `/api/admin/showtimes` | Tạo suất chiếu | ✅ Admin | ⏳ TODO |
| 9 | POST | `/api/admin/showtimes/bulk` | Tạo nhiều suất cùng lúc | ✅ Admin | ⏳ TODO |
| 10 | DELETE | `/api/admin/showtimes/{id}` | Hủy suất chiếu | ✅ Admin | ⏳ TODO |

### C. User Management (4 endpoints)
| # | Method | Endpoint | Description | Auth | Status |
|---|--------|----------|-------------|------|--------|
| 11 | GET | `/api/admin/users` | Danh sách users | ✅ Admin | ⏳ TODO |
| 12 | POST | `/api/admin/users` | Thêm staff/admin | ✅ Admin | ⏳ TODO |
| 13 | PUT | `/api/admin/users/{id}/role` | Cập nhật role | ✅ Admin | ⏳ TODO |
| 14 | PUT | `/api/admin/users/{id}/ban` | Ban/unban user | ✅ Admin | ⏳ TODO |

### D. Reports & Analytics (6 endpoints)
| # | Method | Endpoint | Description | Auth | Status |
|---|--------|----------|-------------|------|--------|
| 15 | GET | `/api/admin/dashboard/stats` | Dashboard overview | ✅ Admin | ⏳ TODO |
| 16 | GET | `/api/admin/reports/revenue/daily` | Báo cáo doanh thu ngày | ✅ Admin | ⏳ TODO |
| 17 | GET | `/api/admin/reports/revenue/monthly` | Báo cáo doanh thu tháng | ✅ Admin | ⏳ TODO |
| 18 | GET | `/api/admin/reports/bookings/statistics` | Thống kê booking | ✅ Admin | ⏳ TODO |
| 19 | GET | `/api/admin/reports/popular-movies` | Phim phổ biến | ✅ Admin | ⏳ TODO |
| 20 | GET | `/api/admin/reports/customers/analytics` | Phân tích khách hàng | ✅ Admin | ⏳ TODO |

---

## 🎯 Vai trò của Admin

**Bạn là quản trị viên hệ thống** Movie88 với toàn quyền quản lý.

### ✅ Quyền hạn đầy đủ
- ✅ **TẤT CẢ** quyền của Staff
- ✅ Quản lý Movies (CRUD)
- ✅ Quản lý Cinemas & Auditoriums (CRUD)
- ✅ Quản lý Showtimes (CRUD)
- ✅ Quản lý Users (CRUD, ban/unban)
- ✅ Xem báo cáo & thống kê
- ✅ Xử lý hoàn tiền & khiếu nại
- ✅ Cấu hình hệ thống

### 📅 Trách nhiệm chính

**Daily**: Check dashboard, resolve tickets, monitor health  
**Weekly**: Review revenue, update schedules  
**Monthly**: Generate reports, analyze trends, planning

---

## 🎯 A. DASHBOARD & REPORTS

## 🎯 1. GET /api/admin/dashboard/stats

**Description**: Dashboard overview với real-time stats  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
GET /api/admin/dashboard/stats
Authorization: Bearer {admin_token}
```

### Response 200 OK

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

### Related Entities
**Dashboard aggregates data from:**
- ✅ Bookings table (revenue, counts)
- ✅ Movies table (popular movies)
- ✅ Customers table (active users)
- ✅ Showtimes table (occupancy rates)

### Implementation Plan
- ⏳ Application: DashboardStatsQuery.cs, DashboardStatsDTO.cs
- ⏳ Infrastructure: Complex aggregation queries
- ⏳ WebApi: AdminController.GetDashboardStats()

### UI Widgets cần có
1. 📈 Revenue Chart (line chart)
2. 🎬 Top Movies (bar chart)
3. 👥 Customer Growth (area chart)
4. 🏢 Cinema Occupancy (pie chart)
5. 📅 Upcoming Showtimes (table)

---

## � B. MOVIE MANAGEMENT

## 🎯 2. POST /api/movies

**Description**: Thêm phim mới vào hệ thống  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
POST /api/movies
Authorization: Bearer {admin_token}
Content-Type: application/json
```

### Request Body
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

### Request Body Fields
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| title | string | ✅ | Movie title (max 200) |
| description | text | ❌ | Movie description |
| durationMinutes | int | ✅ | Duration in minutes |
| director | string | ❌ | Director name (max 100) |
| releaseDate | DateOnly | ❌ | Release date |
| country | string | ❌ | Country of origin |
| rating | string | ✅ | Age rating (G, PG, PG-13, R) |
| genre | string | ❌ | Genres (comma-separated) |
| posterUrl | string | ❌ | Poster image URL |
| trailerUrl | string | ❌ | YouTube trailer URL |
| cast | array | ❌ | Cast members |
| producer | string | ❌ | Producer name |
| language | string | ❌ | Original language |
| subtitle | string | ❌ | Subtitle language |

### Response 201 Created
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

### Related Entities
**Movie** (movies table):
- ✅ `movieid` (int, PK, auto-increment)
- ✅ `title`, `description`, `durationminutes`
- ✅ `director`, `releasedate`, `posterurl`, `trailerurl`
- ✅ `country`, `rating`, `genre`

### Implementation Plan
- ⏳ Application: CreateMovieCommand.cs, CreateMovieDTO.cs
- ⏳ Infrastructure: MovieRepository.Add()
- ⏳ WebApi: MoviesController.CreateMovie() - [Authorize(Roles="Admin")]

---

## 🎯 3. PUT /api/movies/{id}

**Description**: Cập nhật thông tin phim  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
PUT /api/movies/123
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "title": "Updated Title",
  "posterUrl": "https://new-poster-url.jpg",
  "status": "NowShowing"
}
```

### Use Cases
- Sửa thông tin phim (typo, duration)
- Cập nhật poster/trailer
- Thay đổi trạng thái (ComingSoon → NowShowing → Ended)

### Response 200 OK
```json
{
  "success": true,
  "message": "Movie updated successfully"
}
```

---

## 🎯 4. DELETE /api/movies/{id}

**Description**: Xóa phim (soft delete)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
DELETE /api/movies/123
Authorization: Bearer {admin_token}
```

### Business Rules
⚠️ **Chỉ xóa được nếu:**
- Phim KHÔNG có booking nào
- Nếu có booking: Phải cancel tất cả trước
- Soft delete (IsDeleted=true, không xóa DB)

### Response 200 OK
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

### Response 400 Bad Request
```json
{
  "success": false,
  "message": "Cannot delete movie with existing bookings",
  "errors": ["Movie has 45 active bookings"]
}
```

---

## 🎯 5. GET /api/admin/movies

**Description**: Danh sách phim (Admin view với revenue/bookings)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
GET /api/admin/movies?page=1&pageSize=20&status=all&sortBy=revenue
Authorization: Bearer {admin_token}
```

### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| page | int | ❌ | Page number (default: 1) |
| pageSize | int | ❌ | Items per page (default: 20) |
| status | string | ❌ | all, now-showing, coming-soon, ended |
| search | string | ❌ | Tìm theo tên phim |
| sortBy | string | ❌ | title, releaseDate, revenue, bookings |
| sortOrder | string | ❌ | asc, desc |

### Response 200 OK
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

### Related Entities
**Admin view includes aggregated data:**
- ✅ Movie basic info
- ✅ Total bookings count
- ✅ Total revenue
- ✅ Occupancy rate
- ✅ Average rating

---

## 🎯 C. CINEMA & SHOWTIME MANAGEMENT

## 🎯 6. POST /api/admin/cinemas

**Description**: Thêm rạp chiếu phim mới  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
POST /api/admin/cinemas
Authorization: Bearer {admin_token}
Content-Type: application/json

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

### Request Body Fields
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | ✅ | Cinema name |
| address | string | ✅ | Full address |
| city | string | ✅ | City |
| district | string | ❌ | District |
| phone | string | ❌ | Contact phone |
| email | string | ❌ | Contact email |
| facilities | array | ❌ | ["3D", "IMAX", "4DX"] |

### Response 201 Created
```json
{
  "success": true,
  "message": "Cinema created successfully",
  "data": {
    "cinemaId": 2,
    "name": "CGV Landmark 81"
  }
}
```

---

## 🎯 7-8. Cinema Management (PUT, DELETE)

**Status**: ⏳ TODO

```http
PUT /api/admin/cinemas/{id}    # Update cinema
DELETE /api/admin/cinemas/{id}  # Delete cinema (soft delete)
```

**Business Rule**: Chỉ xóa được rạp KHÔNG có showtime đang hoạt động

---

## 🎯 9. POST /api/admin/showtimes

**Description**: Tạo suất chiếu mới  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
POST /api/admin/showtimes
Authorization: Bearer {admin_token}
Content-Type: application/json

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

### Request Body Fields
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| movieId | int | ✅ | Movie ID |
| auditoriumId | int | ✅ | Auditorium ID |
| startTime | DateTime | ✅ | Showtime start |
| format | string | ✅ | 2D, 3D, IMAX |
| language | string | ❌ | Audio language |
| subtitle | string | ❌ | Subtitle language |
| basePrice | decimal | ✅ | Base ticket price |

### Response 201 Created
```json
{
  "success": true,
  "message": "Showtime created successfully",
  "data": {
    "showtimeId": 456,
    "movieTitle": "Avengers",
    "startTime": "2025-11-05T19:30:00",
    "availableSeats": 150
  }
}
```

---

## 🎯 10. POST /api/admin/showtimes/bulk

**Description**: Tạo nhiều suất chiếu cùng lúc (weekly scheduling)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
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

### Response 201 Created
```json
{
  "success": true,
  "message": "35 showtimes created successfully",
  "data": {
    "created": 35,
    "skipped": 0,
    "failed": 0
  }
}
```

**Use Case**: Tạo lịch chiếu cho cả tuần trong 1 lần thay vì tạo từng suất

---

## 🎯 D. USER MANAGEMENT

## 🎯 11. GET /api/admin/users

**Description**: Danh sách users (Customer/Staff/Admin)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
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

### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| role | string | ❌ | all, customer, staff, admin |
| status | string | ❌ | all, active, inactive, banned |
| search | string | ❌ | Tìm theo email/tên |
| page | int | ❌ | Page number |
| pageSize | int | ❌ | Items per page |

---

## 🎯 12. POST /api/admin/users

**Description**: Thêm Staff/Admin mới  
**Status**: ⏳ TODO

### Request Body
```json
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

## 🎯 13-14. User Management (Role, Ban/Unban)

**Status**: ⏳ TODO

```http
PUT /api/admin/users/{id}/role    # Change role (customer→staff, staff→admin)
PUT /api/admin/users/{id}/ban      # Ban/unban user with reason
```

---

## 🎯 E. REPORTS & ANALYTICS

## 🎯 15. GET /api/admin/reports/revenue/daily

**Description**: Báo cáo doanh thu theo ngày  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
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

### Response includes
- Total revenue, bookings, average ticket price
- Breakdown by movie, cinema, hour
- Growth comparison

---

## 🎯 16. GET /api/admin/reports/revenue/monthly

**Description**: Báo cáo doanh thu theo tháng  
**Status**: ⏳ TODO

```http
GET /api/admin/reports/revenue/monthly?month=11&year=2025
```

---

## 🎯 17. GET /api/admin/reports/bookings/statistics

**Description**: Thống kê booking (completion rate, peak hours, etc.)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
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

### Response includes
- Total/completed/cancelled bookings
- Cancellation rate
- Peak hours/days
- Conversion rate

---

## 🎯 18. GET /api/admin/reports/popular-movies

**Description**: Báo cáo phim phổ biến (top 10)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
GET /api/admin/reports/popular-movies?period=month&limit=10
```

### Response 200 OK
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

### Response includes
- Movie ranking by revenue/bookings
- Occupancy rate
- Trend (up/down/stable)

---

## 🎯 19. GET /api/admin/reports/customers/analytics

**Description**: Phân tích khách hàng (retention, churn, demographics)  
**Auth Required**: ✅ Admin  
**Status**: ⏳ TODO

### Request
```http
GET /api/admin/reports/customers/analytics?period=month
```

### Response 200 OK
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

### Response includes
- Total/new/active customers
- Retention & churn rate
- Top customers (lifetime value)
- Demographics (age, gender)

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
