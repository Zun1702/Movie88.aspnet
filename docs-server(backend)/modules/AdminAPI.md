# 📊 Admin & Analytics API

## 1. Mô tả

Module Admin & Analytics cung cấp các API cho Dashboard quản trị, báo cáo, thống kê và phân tích dữ liệu, bao gồm:
- Dashboard tổng quan (KPIs, revenue, bookings)
- Báo cáo doanh thu (theo ngày/tháng/năm)
- Thống kê đặt vé (tỷ lệ lấp đầy, phim HOT)
- Quản lý users (roles, permissions)
- Logs và audit trail
- Export reports (PDF, Excel)

## 2. Danh sách Endpoint

### 2.1 Dashboard

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/admin/dashboard` | Dashboard tổng quan | - | DashboardDTO | Admin/Manager |
| GET | `/api/admin/dashboard/revenue` | Biểu đồ doanh thu | startDate, endDate | RevenueChartDTO | Admin/Manager |
| GET | `/api/admin/dashboard/bookings` | Thống kê đặt vé | startDate, endDate | BookingStatsDTO | Admin/Manager |

### 2.2 Reports

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/admin/reports/revenue` | Báo cáo doanh thu | ReportFilterDTO | RevenueReportDTO | Admin/Manager |
| GET | `/api/admin/reports/movies` | Báo cáo phim | startDate, endDate | MovieReportDTO | Admin/Manager |
| GET | `/api/admin/reports/cinemas` | Báo cáo rạp | startDate, endDate | CinemaReportDTO | Admin/Manager |
| GET | `/api/admin/reports/customers` | Báo cáo khách hàng | - | CustomerReportDTO | Admin/Manager |
| POST | `/api/admin/reports/export` | Export báo cáo | ExportReportDTO | File (PDF/Excel) | Admin/Manager |

### 2.3 User Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/admin/users` | Danh sách users | page, size, role | PagedList<UserDTO> | Admin |
| GET | `/api/admin/users/{id}` | Chi tiết user | userId | UserDetailDTO | Admin |
| POST | `/api/admin/users` | Tạo user (staff/manager) | CreateUserDTO | UserDTO | Admin |
| PUT | `/api/admin/users/{id}/role` | Cập nhật role | UpdateRoleDTO | UserDTO | Admin |
| PUT | `/api/admin/users/{id}/status` | Kích hoạt/vô hiệu hóa | status | UserDTO | Admin |
| DELETE | `/api/admin/users/{id}` | Xóa user | userId | Success message | Admin |

### 2.4 Analytics

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/admin/analytics/top-movies` | Top phim bán chạy | startDate, endDate, limit | List<TopMovieDTO> | Admin/Manager |
| GET | `/api/admin/analytics/occupancy-rate` | Tỷ lệ lấp đầy | startDate, endDate | OccupancyRateDTO | Admin/Manager |
| GET | `/api/admin/analytics/customer-behavior` | Phân tích hành vi KH | - | CustomerBehaviorDTO | Admin/Manager |

### 2.5 Audit Logs

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/admin/logs` | Danh sách logs | page, size, action | PagedList<AuditLogDTO> | Admin |
| GET | `/api/admin/logs/{id}` | Chi tiết log | logId | AuditLogDTO | Admin |

## 3. Data Transfer Objects (DTOs)

### 3.1 DashboardDTO
```json
{
  "kpis": {
    "todayRevenue": 45000000,
    "todayBookings": 320,
    "activeCustomers": 15420,
    "averageTicketPrice": 140625,
    "occupancyRate": 72.5
  },
  "revenueComparison": {
    "thisMonth": 850000000,
    "lastMonth": 780000000,
    "growth": 8.97,
    "trend": "up"
  },
  "topMovies": [
    {
      "movieId": 10,
      "title": "Avengers: Endgame",
      "revenue": 120000000,
      "bookings": 8500,
      "occupancyRate": 85.3
    }
  ],
  "recentBookings": [
    {
      "bookingId": 12345,
      "customerName": "Nguyen Van A",
      "movieTitle": "Avengers",
      "totalAmount": 350000,
      "status": "Paid",
      "createdAt": "2025-10-29T14:30:00Z"
    }
  ]
}
```

### 3.2 RevenueChartDTO
```json
{
  "period": "daily",
  "startDate": "2025-10-01",
  "endDate": "2025-10-31",
  "data": [
    {
      "date": "2025-10-01",
      "revenue": 25000000,
      "bookings": 180,
      "tickets": 450
    },
    {
      "date": "2025-10-02",
      "revenue": 30000000,
      "bookings": 210,
      "tickets": 520
    }
  ],
  "summary": {
    "totalRevenue": 850000000,
    "totalBookings": 6200,
    "totalTickets": 15500,
    "averageDaily": 27419355
  }
}
```

### 3.3 RevenueReportDTO
```json
{
  "reportPeriod": {
    "startDate": "2025-10-01",
    "endDate": "2025-10-31",
    "groupBy": "daily"
  },
  "breakdown": {
    "ticketRevenue": 680000000,
    "comboRevenue": 120000000,
    "voucherDiscounts": -25000000,
    "promotionDiscounts": -15000000,
    "netRevenue": 760000000
  },
  "byPaymentMethod": {
    "VNPay": 760000000
  },
  "byCinema": [
    {
      "cinemaId": 1,
      "cinemaName": "CGV Vincom",
      "revenue": 320000000,
      "bookings": 2800,
      "percentage": 42.1
    }
  ],
  "byMovie": [
    {
      "movieId": 10,
      "title": "Avengers: Endgame",
      "revenue": 120000000,
      "bookings": 850,
      "percentage": 15.8
    }
  ]
}
```

### 3.4 BookingStatsDTO
```json
{
  "period": {
    "startDate": "2025-10-01",
    "endDate": "2025-10-31"
  },
  "totalBookings": 6200,
  "statusDistribution": {
    "Paid": 5800,
    "Pending": 250,
    "Cancelled": 100,
    "Expired": 50
  },
  "averageBookingValue": 137097,
  "peakHours": [
    {
      "hour": 19,
      "bookings": 850,
      "percentage": 13.7
    },
    {
      "hour": 20,
      "bookings": 780,
      "percentage": 12.6
    }
  ],
  "peakDays": [
    {
      "dayOfWeek": "Saturday",
      "bookings": 1200,
      "percentage": 19.4
    },
    {
      "dayOfWeek": "Sunday",
      "bookings": 1050,
      "percentage": 16.9
    }
  ]
}
```

### 3.5 TopMovieDTO
```json
{
  "movieId": 10,
  "title": "Avengers: Endgame",
  "poster": "https://...",
  "totalRevenue": 120000000,
  "totalBookings": 850,
  "totalTickets": 2100,
  "averageRating": 4.5,
  "occupancyRate": 85.3,
  "rank": 1
}
```

### 3.6 OccupancyRateDTO
```json
{
  "period": {
    "startDate": "2025-10-01",
    "endDate": "2025-10-31"
  },
  "overall": {
    "totalSeats": 125000,
    "bookedSeats": 90625,
    "occupancyRate": 72.5
  },
  "byCinema": [
    {
      "cinemaId": 1,
      "cinemaName": "CGV Vincom",
      "totalSeats": 45000,
      "bookedSeats": 35000,
      "occupancyRate": 77.8
    }
  ],
  "byTimeSlot": [
    {
      "timeSlot": "19:00-21:00",
      "occupancyRate": 85.2
    },
    {
      "timeSlot": "21:00-23:00",
      "occupancyRate": 68.5
    }
  ],
  "byDayOfWeek": [
    {
      "day": "Saturday",
      "occupancyRate": 88.3
    },
    {
      "day": "Friday",
      "occupancyRate": 82.1
    }
  ]
}
```

### 3.7 CustomerBehaviorDTO
```json
{
  "totalCustomers": 15420,
  "activeCustomers": 8500,
  "newCustomers": 1200,
  "churnRate": 12.5,
  "averageLifetimeValue": 2500000,
  "segmentation": {
    "vip": {
      "count": 850,
      "percentage": 5.5,
      "avgMonthlySpend": 1500000
    },
    "regular": {
      "count": 6500,
      "percentage": 42.2,
      "avgMonthlySpend": 350000
    },
    "occasional": {
      "count": 8070,
      "percentage": 52.3,
      "avgMonthlySpend": 120000
    }
  },
  "bookingFrequency": {
    "once": 5200,
    "2-5times": 7800,
    "6-10times": 1800,
    "10+times": 620
  },
  "preferredGenres": [
    {
      "genre": "Action",
      "bookings": 3500,
      "percentage": 28.5
    },
    {
      "genre": "Comedy",
      "bookings": 2800,
      "percentage": 22.8
    }
  ]
}
```

### 3.8 UserDetailDTO
```json
{
  "userId": 5,
  "email": "manager@movie88.com",
  "name": "Tran Thi B",
  "role": "Manager",
  "isActive": true,
  "createdAt": "2024-01-15T00:00:00Z",
  "lastLogin": "2025-10-29T08:30:00Z",
  "permissions": [
    "ViewReports",
    "ManageMovies",
    "ManageCinemas",
    "ManageShowtimes",
    "ViewCustomers"
  ],
  "activityLog": [
    {
      "action": "UpdateMovie",
      "entity": "Movie",
      "entityId": 10,
      "timestamp": "2025-10-28T14:20:00Z"
    }
  ]
}
```

### 3.9 AuditLogDTO
```json
{
  "logId": 12345,
  "userId": 5,
  "userName": "Manager",
  "action": "UpdateMovie",
  "entity": "Movies",
  "entityId": 10,
  "changes": {
    "Title": {
      "oldValue": "Avenger: Endgame",
      "newValue": "Avengers: Endgame"
    },
    "Status": {
      "oldValue": "ComingSoon",
      "newValue": "NowShowing"
    }
  },
  "ipAddress": "192.168.1.100",
  "timestamp": "2025-10-28T14:20:00Z"
}
```

## 4. Luồng xử lý (Flow)

### 4.1 Load Dashboard Flow

```
Admin login và vào trang Dashboard
↓
GET /api/admin/dashboard
Authorization: Bearer {adminToken}
↓
Backend tính toán KPIs:

-- Today's revenue
SELECT SUM(TotalAmount) AS TodayRevenue
FROM Bookings
WHERE Status = 'Paid'
  AND CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE);

-- Today's bookings
SELECT COUNT(*) AS TodayBookings
FROM Bookings
WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE);

-- Active customers (đặt vé trong 30 ngày qua)
SELECT COUNT(DISTINCT CustomerId) AS ActiveCustomers
FROM Bookings
WHERE Status = 'Paid'
  AND CreatedAt >= DATEADD(DAY, -30, GETDATE());

-- Average ticket price
SELECT AVG(TotalAmount / NULLIF(TotalSeats, 0)) AS AvgTicketPrice
FROM Bookings
WHERE Status = 'Paid'
  AND CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE);

-- Occupancy rate (today)
WITH TodayShowtimes AS (
    SELECT 
        SUM(a.Capacity) AS TotalSeats,
        SUM((SELECT COUNT(*) FROM BookingSeats bs 
             JOIN Bookings b ON bs.BookingId = b.BookingId
             WHERE b.ShowtimeId = s.ShowtimeId 
             AND b.Status = 'Paid')) AS BookedSeats
    FROM Showtimes s
    JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
    WHERE CAST(s.StartTime AS DATE) = CAST(GETDATE() AS DATE)
)
SELECT 
    CAST(BookedSeats AS FLOAT) / NULLIF(TotalSeats, 0) * 100 AS OccupancyRate
FROM TodayShowtimes;

-- Revenue comparison (this month vs last month)
DECLARE @ThisMonth DECIMAL(18,2) = (
    SELECT SUM(TotalAmount) 
    FROM Bookings 
    WHERE Status = 'Paid'
      AND YEAR(CreatedAt) = YEAR(GETDATE())
      AND MONTH(CreatedAt) = MONTH(GETDATE())
);

DECLARE @LastMonth DECIMAL(18,2) = (
    SELECT SUM(TotalAmount) 
    FROM Bookings 
    WHERE Status = 'Paid'
      AND YEAR(CreatedAt) = YEAR(DATEADD(MONTH, -1, GETDATE()))
      AND MONTH(CreatedAt) = MONTH(DATEADD(MONTH, -1, GETDATE()))
);

DECLARE @Growth DECIMAL(5,2) = (
    (@ThisMonth - @LastMonth) / NULLIF(@LastMonth, 0) * 100
);

-- Top 5 movies (by revenue this month)
SELECT TOP 5
    m.MovieId,
    m.Title,
    SUM(b.TotalAmount) AS Revenue,
    COUNT(b.BookingId) AS Bookings,
    (SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews WHERE MovieId = m.MovieId) AS Rating
FROM Movies m
JOIN Showtimes s ON m.MovieId = s.MovieId
JOIN Bookings b ON s.ShowtimeId = b.ShowtimeId
WHERE b.Status = 'Paid'
  AND YEAR(b.CreatedAt) = YEAR(GETDATE())
  AND MONTH(b.CreatedAt) = MONTH(GETDATE())
GROUP BY m.MovieId, m.Title
ORDER BY Revenue DESC;

-- Recent 10 bookings
SELECT TOP 10
    b.BookingId,
    c.Name AS CustomerName,
    m.Title AS MovieTitle,
    b.TotalAmount,
    b.Status,
    b.CreatedAt
FROM Bookings b
JOIN Customers c ON b.CustomerId = c.CustomerId
JOIN Showtimes s ON b.ShowtimeId = s.ShowtimeId
JOIN Movies m ON s.MovieId = m.MovieId
ORDER BY b.CreatedAt DESC;
↓
Return DashboardDTO với all data
↓
Frontend render dashboard với charts và tables
```

### 4.2 Generate Revenue Report Flow

```
Manager vào "Báo cáo doanh thu"
↓
Chọn filters:
- Khoảng thời gian: 01/10/2025 - 31/10/2025
- Group by: Daily
- Cinema: All
↓
GET /api/admin/reports/revenue?startDate=2025-10-01&endDate=2025-10-31&groupBy=daily
↓
Backend query:

-- Revenue breakdown
WITH RevenueBreakdown AS (
    SELECT 
        SUM(b.TotalAmount) AS TotalRevenue,
        SUM(b.TotalAmount - ISNULL(b.VoucherDiscount, 0) - 
            (SELECT SUM(DiscountApplied) FROM BookingPromotions WHERE BookingId = b.BookingId)) 
            AS NetRevenue,
        SUM(ISNULL(b.VoucherDiscount, 0)) AS VoucherDiscounts,
        SUM((SELECT SUM(DiscountApplied) FROM BookingPromotions WHERE BookingId = b.BookingId)) 
            AS PromotionDiscounts,
        SUM((SELECT SUM(Price * Quantity) FROM BookingSeats WHERE BookingId = b.BookingId)) 
            AS TicketRevenue,
        SUM((SELECT SUM(Price * Quantity) FROM BookingCombos WHERE BookingId = b.BookingId)) 
            AS ComboRevenue
    FROM Bookings b
    WHERE b.Status = 'Paid'
      AND b.CreatedAt >= @StartDate
      AND b.CreatedAt <= @EndDate
)
SELECT * FROM RevenueBreakdown;

-- By payment method
SELECT 
    pm.Name AS PaymentMethod,
    SUM(p.Amount) AS Revenue
FROM Payments p
JOIN PaymentMethods pm ON p.PaymentMethodId = pm.PaymentMethodId
WHERE p.Status = 'Success'
  AND p.PaymentDate >= @StartDate
  AND p.PaymentDate <= @EndDate
GROUP BY pm.Name;

-- By cinema
SELECT 
    cin.CinemaId,
    cin.Name AS CinemaName,
    SUM(b.TotalAmount) AS Revenue,
    COUNT(b.BookingId) AS Bookings,
    CAST(SUM(b.TotalAmount) AS FLOAT) / 
        (SELECT SUM(TotalAmount) FROM Bookings WHERE Status = 'Paid' 
         AND CreatedAt >= @StartDate AND CreatedAt <= @EndDate) * 100 AS Percentage
FROM Bookings b
JOIN Showtimes s ON b.ShowtimeId = s.ShowtimeId
JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
JOIN Cinemas cin ON a.CinemaId = cin.CinemaId
WHERE b.Status = 'Paid'
  AND b.CreatedAt >= @StartDate
  AND b.CreatedAt <= @EndDate
GROUP BY cin.CinemaId, cin.Name
ORDER BY Revenue DESC;

-- By movie
SELECT 
    m.MovieId,
    m.Title,
    SUM(b.TotalAmount) AS Revenue,
    COUNT(b.BookingId) AS Bookings,
    CAST(SUM(b.TotalAmount) AS FLOAT) / 
        (SELECT SUM(TotalAmount) FROM Bookings WHERE Status = 'Paid' 
         AND CreatedAt >= @StartDate AND CreatedAt <= @EndDate) * 100 AS Percentage
FROM Bookings b
JOIN Showtimes s ON b.ShowtimeId = s.ShowtimeId
JOIN Movies m ON s.MovieId = m.MovieId
WHERE b.Status = 'Paid'
  AND b.CreatedAt >= @StartDate
  AND b.CreatedAt <= @EndDate
GROUP BY m.MovieId, m.Title
ORDER BY Revenue DESC;

-- Daily data (if groupBy = daily)
SELECT 
    CAST(b.CreatedAt AS DATE) AS Date,
    SUM(b.TotalAmount) AS Revenue,
    COUNT(b.BookingId) AS Bookings,
    SUM((SELECT COUNT(*) FROM BookingSeats WHERE BookingId = b.BookingId)) AS Tickets
FROM Bookings b
WHERE b.Status = 'Paid'
  AND b.CreatedAt >= @StartDate
  AND b.CreatedAt <= @EndDate
GROUP BY CAST(b.CreatedAt AS DATE)
ORDER BY Date;
↓
Return RevenueReportDTO
↓
Frontend render report với charts và tables
↓
User có thể export:
POST /api/admin/reports/export
{
  "reportType": "revenue",
  "format": "pdf",
  "data": { ... }
}
↓
Backend generate PDF/Excel file
Return file download
```

### 4.3 Analyze Top Movies Flow

```
Manager vào "Phân tích phim"
↓
GET /api/admin/analytics/top-movies?startDate=2025-10-01&endDate=2025-10-31&limit=10
↓
Backend query:

SELECT TOP (@Limit)
    m.MovieId,
    m.Title,
    m.Poster,
    SUM(b.TotalAmount) AS TotalRevenue,
    COUNT(b.BookingId) AS TotalBookings,
    SUM((SELECT COUNT(*) FROM BookingSeats WHERE BookingId = b.BookingId)) AS TotalTickets,
    AVG((SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews WHERE MovieId = m.MovieId)) AS AverageRating,
    
    -- Occupancy rate
    CAST(
        SUM((SELECT COUNT(*) FROM BookingSeats bs WHERE bs.BookingId = b.BookingId))
        AS FLOAT
    ) / NULLIF(
        SUM((SELECT a.Capacity FROM Auditoriums a 
             JOIN Showtimes s2 ON a.AuditoriumId = s2.AuditoriumId 
             WHERE s2.ShowtimeId = b.ShowtimeId)), 0
    ) * 100 AS OccupancyRate,
    
    ROW_NUMBER() OVER (ORDER BY SUM(b.TotalAmount) DESC) AS Rank
    
FROM Movies m
JOIN Showtimes s ON m.MovieId = s.MovieId
JOIN Bookings b ON s.ShowtimeId = b.ShowtimeId
WHERE b.Status = 'Paid'
  AND b.CreatedAt >= @StartDate
  AND b.CreatedAt <= @EndDate
GROUP BY m.MovieId, m.Title, m.Poster
ORDER BY TotalRevenue DESC;
↓
Return List<TopMovieDTO>
↓
Frontend hiển thị:
- Bar chart: Top 10 phim theo doanh thu
- Pie chart: Phân bố doanh thu
- Table: Chi tiết từng phim (rank, title, revenue, bookings, rating, occupancy)
```

### 4.4 Admin Create User Flow

```
Admin vào "Quản lý nhân viên"
↓
POST /api/admin/users
Authorization: Bearer {adminToken}
{
  "email": "staff@movie88.com",
  "name": "Le Van C",
  "role": "Staff",
  "cinemaId": 1
}
↓
Backend validate:
1. Check email unique
2. Validate role (Staff, Manager only - không cho tạo Admin)
3. Hash password mặc định: "Movie88@123"
↓
BEGIN TRANSACTION

-- Insert user
INSERT INTO Users (Email, PasswordHash, Role, IsActive, CreatedAt)
VALUES (@Email, @PasswordHash, @Role, TRUE, CURRENT_TIMESTAMP)
RETURNING UserId INTO @UserId;

-- Or use this PostgreSQL syntax:
-- INSERT INTO Users (...) VALUES (...) RETURNING UserId;

-- Insert customer/staff info (depends on role)
IF @Role IN ('Staff', 'Manager'):
    INSERT INTO Customers (UserId, Name, Phone, Email)
    VALUES (@UserId, @Name, NULL, @Email);
    
    -- Link to cinema (if specified)
    IF @CinemaId IS NOT NULL:
        UPDATE Customers SET PreferredCinemaId = @CinemaId 
        WHERE UserId = @UserId;

-- Audit log
INSERT INTO AuditLogs (UserId, Action, Entity, EntityId, Timestamp)
VALUES ((SELECT UserId FROM Users WHERE Email = @AdminEmail), 
        'CreateUser', 'Users', @UserId, GETDATE());

COMMIT TRANSACTION
↓
Send email with login credentials:
Subject: "Tài khoản Movie88 của bạn"
Body: "Email: staff@movie88.com, Password: Movie88@123"
↓
Return UserDTO
```

## 5. Business Rules

### 5.1 Dashboard KPIs
- **Today's Revenue**: Tổng doanh thu bookings status = Paid trong ngày
- **Active Customers**: Customers có ít nhất 1 booking trong 30 ngày qua
- **Occupancy Rate**: (Booked seats / Total seats) * 100 cho tất cả showtimes trong ngày
- **Average Ticket Price**: Total revenue / Total tickets sold

### 5.2 Report Periods
- **Daily**: Group by date
- **Weekly**: Group by week (Mon-Sun)
- **Monthly**: Group by month
- **Yearly**: Group by year

### 5.3 User Roles & Permissions

| Role | Permissions |
|------|-------------|
| **Admin** | Full access: All CRUD, manage users, view all reports |
| **Manager** | View reports, manage movies/cinemas/showtimes, view customers |
| **Staff** | View assigned cinema data, process bookings, basic reports |
| **Customer** | Book tickets, view profile, write reviews |

### 5.4 Audit Log Rules
- Log tất cả actions: Create, Update, Delete
- Không log Read operations (tránh spam logs)
- Lưu old value và new value cho Update actions
- Log IP address, timestamp, userId
- Retention: 90 days (auto-cleanup)

## 6. Performance Optimization

### 6.1 Materialized Views
```sql
-- Pre-calculate daily stats
CREATE VIEW vw_DailyStats AS
SELECT 
    CAST(CreatedAt AS DATE) AS Date,
    COUNT(*) AS TotalBookings,
    SUM(TotalAmount) AS TotalRevenue,
    SUM(CASE WHEN Status = 'Paid' THEN 1 ELSE 0 END) AS PaidBookings,
    AVG(TotalAmount) AS AvgBookingValue
FROM Bookings
GROUP BY CAST(CreatedAt AS DATE);
```

### 6.2 Caching Strategy
```csharp
// Cache dashboard (5 minutes)
Cache: "admin:dashboard" → DashboardDTO

// Cache revenue chart (1 hour)
Cache: "admin:revenue:{startDate}:{endDate}:{groupBy}" → RevenueChartDTO

// Cache top movies (30 minutes)
Cache: "admin:top-movies:{period}" → List<TopMovieDTO>
```

### 6.3 Indexes for Reports
```sql
-- Index for date range queries
CREATE INDEX IX_Bookings_Status_CreatedAt 
ON Bookings(Status, CreatedAt) 
INCLUDE (TotalAmount, CustomerId);

-- Index for payment reports
CREATE INDEX IX_Payments_Status_PaymentDate 
ON Payments(Status, PaymentDate) 
INCLUDE (Amount, PaymentMethodId);

-- Index for audit logs
CREATE INDEX IX_AuditLogs_Timestamp 
ON AuditLogs(Timestamp DESC);
```

## 7. Sample API Calls

### Get Dashboard
```bash
GET /api/admin/dashboard
Authorization: Bearer {adminToken}

Response:
{
  "success": true,
  "data": {
    "kpis": {
      "todayRevenue": 45000000,
      "todayBookings": 320,
      "activeCustomers": 15420,
      "occupancyRate": 72.5
    },
    "revenueComparison": {
      "thisMonth": 850000000,
      "lastMonth": 780000000,
      "growth": 8.97
    }
  }
}
```

### Generate Revenue Report
```bash
GET /api/admin/reports/revenue?startDate=2025-10-01&endDate=2025-10-31&groupBy=daily
Authorization: Bearer {adminToken}

Response:
{
  "success": true,
  "data": {
    "breakdown": {
      "ticketRevenue": 680000000,
      "comboRevenue": 120000000,
      "netRevenue": 760000000
    },
    "byCinema": [...],
    "byMovie": [...]
  }
}
```

### Export Report
```bash
POST /api/admin/reports/export
Authorization: Bearer {adminToken}

{
  "reportType": "revenue",
  "format": "pdf",
  "period": {
    "startDate": "2025-10-01",
    "endDate": "2025-10-31"
  }
}

Response:
Content-Type: application/pdf
Content-Disposition: attachment; filename="revenue_report_2025-10.pdf"
[Binary PDF data]
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
