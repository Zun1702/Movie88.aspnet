# 🕒 Showtime Management API

## 1. Mô tả

Module Showtime quản lý các suất chiếu phim trong hệ thống, bao gồm:
- Tạo và quản lý lịch chiếu phim
- Quản lý giá vé theo suất chiếu
- Quản lý format chiếu (2D, 3D, IMAX)
- Quản lý ngôn ngữ (Phụ đề, Lồng tiếng)
- Kiểm tra ghế trống real-time
- Tự động tính toán thời gian kết thúc

## 2. Danh sách Endpoint

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/showtimes` | Danh sách suất chiếu | Query params | List<ShowtimeDTO> | Public |
| GET | `/api/showtimes/{id}` | Chi tiết suất chiếu | showtimeId | ShowtimeDetailDTO | Public |
| GET | `/api/showtimes/by-movie/{movieId}` | Suất chiếu theo phim | movieId, date | List<ShowtimeDTO> | Public |
| GET | `/api/showtimes/by-cinema/{cinemaId}` | Suất chiếu theo rạp | cinemaId, date | List<ShowtimeDTO> | Public |
| GET | `/api/showtimes/by-date` | Suất chiếu theo ngày | date | List<ShowtimeDTO> | Public |
| GET | `/api/showtimes/{id}/available-seats` | Ghế còn trống | showtimeId | SeatAvailabilityDTO | Public |
| POST | `/api/showtimes` | Thêm suất chiếu | CreateShowtimeDTO | ShowtimeDTO | Admin/Manager |
| POST | `/api/showtimes/bulk-create` | Tạo hàng loạt | BulkCreateShowtimesDTO | Success message | Admin/Manager |
| PUT | `/api/showtimes/{id}` | Cập nhật suất chiếu | UpdateShowtimeDTO | ShowtimeDTO | Admin/Manager |
| DELETE | `/api/showtimes/{id}` | Xóa suất chiếu | showtimeId | Success message | Admin/Manager |

## 3. Data Transfer Objects (DTOs)

### 3.1 ShowtimeDTO
```json
{
  "showtimeId": 567,
  "movieId": 123,
  "movieTitle": "Avengers: Endgame",
  "posterUrl": "https://example.com/poster.jpg",
  "durationMinutes": 181,
  "auditoriumId": 5,
  "auditoriumName": "Phòng 5",
  "cinemaId": 1,
  "cinemaName": "CGV Vincom Center",
  "cinemaAddress": "72 Lê Thánh Tôn, Q1",
  "startTime": "2025-10-30T19:30:00Z",
  "endTime": "2025-10-30T22:31:00Z",
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub",
  "availableSeats": 120,
  "totalSeats": 150
}
```

### 3.2 ShowtimeDetailDTO
```json
{
  "showtimeId": 567,
  "movie": {
    "movieId": 123,
    "title": "Avengers: Endgame",
    "description": "Sau sự kiện tàn khốc...",
    "posterUrl": "https://example.com/poster.jpg",
    "trailerUrl": "https://youtube.com/watch?v=abc",
    "durationMinutes": 181,
    "rating": "T13",
    "genre": "Action, Adventure, Sci-Fi"
  },
  "cinema": {
    "cinemaId": 1,
    "name": "CGV Vincom Center",
    "address": "72 Lê Thánh Tôn, Quận 1, TP.HCM",
    "phone": "1900 6017"
  },
  "auditorium": {
    "auditoriumId": 5,
    "name": "Phòng 5",
    "seatsCount": 150,
    "has3D": true,
    "hasIMAX": false
  },
  "startTime": "2025-10-30T19:30:00Z",
  "endTime": "2025-10-30T22:31:00Z",
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub",
  "availableSeats": 120,
  "totalSeats": 150,
  "isBookable": true
}
```

### 3.3 CreateShowtimeDTO
```json
{
  "movieId": 123,
  "auditoriumId": 5,
  "startTime": "2025-10-30T19:30:00",
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub"
}
```

**Validation Rules:**
- `movieId`: Required, must exist in Movies
- `auditoriumId`: Required, must exist in Auditoriums
- `startTime`: Required, must be future time
- `price`: Required, 30,000 - 500,000 VND
- `format`: Required, values: 2D, 3D, IMAX, 4DX
- `languageType`: Required

**Auto-calculated:**
- `endTime` = `startTime` + `Movie.DurationMinutes` + 15 phút dọn dẹp

### 3.4 UpdateShowtimeDTO
```json
{
  "startTime": "2025-10-30T20:00:00",
  "price": 85000,
  "format": "3D",
  "languageType": "Original - Vietsub"
}
```

**Note**: Không cho phép thay đổi movieId hoặc auditoriumId sau khi tạo

### 3.5 BulkCreateShowtimesDTO
```json
{
  "movieId": 123,
  "auditoriumId": 5,
  "dateRange": {
    "startDate": "2025-11-01",
    "endDate": "2025-11-07"
  },
  "timeSlots": ["10:00", "13:00", "16:00", "19:00", "22:00"],
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub",
  "skipDates": ["2025-11-05"]
}
```

**Result**: Tạo 7 ngày × 5 slots/ngày - 5 slots (ngày skip) = 30 suất chiếu

### 3.6 SeatAvailabilityDTO
```json
{
  "showtimeId": 567,
  "totalSeats": 150,
  "availableSeats": 120,
  "bookedSeats": 30,
  "seats": [
    {
      "seatId": 1,
      "row": "A",
      "number": 1,
      "type": "Standard",
      "status": "available"
    },
    {
      "seatId": 2,
      "row": "A",
      "number": 2,
      "type": "Standard",
      "status": "booked"
    },
    {
      "seatId": 3,
      "row": "A",
      "number": 3,
      "type": "Standard",
      "status": "locked"
    }
  ]
}
```

**Seat Status:**
- `available`: Ghế trống, có thể đặt
- `booked`: Đã bán (Booking status = Paid)
- `locked`: Đang được giữ (Booking status = Pending/Confirmed)

## 4. Luồng xử lý (Flow)

### 4.1 Browse Showtimes by Movie Flow

```
User xem chi tiết phim
↓
Chọn ngày xem (date picker)
↓
GET /api/showtimes/by-movie/{movieId}?date=2025-10-30
↓
Backend query:
SELECT s.*, c.Name AS CinemaName, a.Name AS AuditoriumName
FROM Showtimes s
INNER JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
INNER JOIN Cinemas c ON a.CinemaId = c.CinemaId
WHERE s.MovieId = {movieId}
  AND CAST(s.StartTime AS DATE) = '2025-10-30'
  AND s.StartTime >= GETDATE()
ORDER BY c.Name, s.StartTime
↓
Return List<ShowtimeDTO> grouped by cinema
↓
Frontend hiển thị:
- Nhóm theo rạp
- Mỗi rạp hiển thị các suất chiếu
- Highlight suất chiếu sắp bắt đầu
```

### 4.2 Check Seat Availability Flow

```
User chọn suất chiếu
↓
GET /api/showtimes/{showtimeId}/available-seats
↓
Backend query:
1. Lấy tất cả ghế của auditorium
2. Check ghế nào đã booked:
   SELECT SeatId 
   FROM BookingSeats bs
   INNER JOIN Bookings b ON bs.BookingId = b.BookingId
   WHERE bs.ShowtimeId = {showtimeId}
     AND b.Status IN ('Confirmed', 'Paid')
3. Mark status cho từng ghế
↓
Return SeatAvailabilityDTO
↓
Frontend render sơ đồ ghế với màu sắc:
- Xanh: Available
- Xám: Booked
- Vàng: Locked (đang được giữ)
```

### 4.3 Admin Create Showtime Flow

```
Admin vào "Thêm suất chiếu"
↓
Chọn phim từ dropdown
↓
Chọn rạp → Chọn phòng chiếu
↓
Chọn ngày giờ chiếu
↓
Nhập giá vé, format, ngôn ngữ
↓
POST /api/showtimes
Authorization: Bearer {adminToken}
↓
Backend validate:
├─ Movie exists?
├─ Auditorium exists?
├─ StartTime in future?
├─ Check time slot conflict:
│   SELECT * FROM Showtimes
│   WHERE AuditoriumId = {auditoriumId}
│     AND (
│       (StartTime <= {newStartTime} AND EndTime > {newStartTime})
│       OR
│       (StartTime < {newEndTime} AND EndTime >= {newEndTime})
│     )
│   → Nếu có conflict: Return error
└─ All valid
↓
Calculate EndTime:
endTime = startTime + movie.DurationMinutes + 15 minutes
↓
INSERT INTO Showtimes (MovieId, AuditoriumId, StartTime, EndTime, Price, Format, LanguageType)
VALUES (...)
↓
Return ShowtimeDTO
```

### 4.4 Bulk Create Showtimes Flow

```
Admin muốn tạo lịch chiếu cho cả tuần
↓
POST /api/showtimes/bulk-create
{
  "movieId": 123,
  "auditoriumId": 5,
  "dateRange": {
    "startDate": "2025-11-01",
    "endDate": "2025-11-07"
  },
  "timeSlots": ["10:00", "13:00", "16:00", "19:00", "22:00"],
  "price": 80000,
  "format": "2D"
}
↓
Backend process:
FOR EACH date IN dateRange:
    IF date NOT IN skipDates:
        FOR EACH timeSlot:
            startTime = COMBINE(date, timeSlot)
            
            -- Check conflict
            IF NO CONFLICT:
                INSERT INTO Showtimes (...)
↓
Total created: 7 days × 5 slots = 35 showtimes
↓
Return summary:
{
  "totalCreated": 35,
  "skipped": 0,
  "conflicts": []
}
```

## 5. Business Rules

### 5.1 Time Slot Rules
- Khoảng cách tối thiểu giữa 2 suất: 15 phút (dọn dẹp phòng)
- Không được tạo suất chiếu trong quá khứ
- EndTime tự động = StartTime + DurationMinutes + 15 phút

### 5.2 Price Rules
| Time Slot | Base Price | Surcharge |
|-----------|------------|-----------|
| 06:00 - 12:00 | 60,000 | - |
| 12:00 - 17:00 | 70,000 | - |
| 17:00 - 22:00 | 80,000 | - |
| 22:00 - 01:00 | 90,000 | +20% (Suất khuya) |
| Weekend | Base | +10,000 |
| Holiday | Base | +20,000 |

### 5.3 Format Pricing
| Format | Price Multiplier |
|--------|------------------|
| 2D | 1.0x |
| 3D | 1.3x |
| IMAX | 1.5x |
| 4DX | 1.7x |

### 5.4 Language Types
- `Original - Vietsub`: Nguyên gốc, phụ đề tiếng Việt
- `Original - Engsub`: Nguyên gốc, phụ đề tiếng Anh
- `Vietnamese Dub`: Lồng tiếng Việt
- `No Subtitle`: Không phụ đề

### 5.5 Deletion Rules
- Không xóa showtime nếu có bookings (Status != Cancelled)
- Chỉ xóa được showtime chưa diễn ra
- Soft delete: Set IsDeleted = true

## 6. Validation Rules

### CreateShowtimeDTO Validation
```csharp
public class CreateShowtimeDTO
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    public int AuditoriumId { get; set; }

    [Required]
    [FutureDate(ErrorMessage = "Thời gian chiếu phải trong tương lai")]
    public DateTime StartTime { get; set; }

    [Required]
    [Range(30000, 500000, ErrorMessage = "Giá vé từ 30,000 - 500,000 VND")]
    public decimal Price { get; set; }

    [Required]
    [RegularExpression("^(2D|3D|IMAX|4DX)$")]
    public string Format { get; set; }

    [Required]
    [MaxLength(50)]
    public string LanguageType { get; set; }
}

// Custom validation attribute
public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object value, 
        ValidationContext validationContext)
    {
        var date = (DateTime)value;
        if (date <= DateTime.Now)
        {
            return new ValidationResult(ErrorMessage);
        }
        return ValidationResult.Success;
    }
}
```

## 7. Complex Queries

### 7.1 Get Showtimes with Available Seats Count
```sql
SELECT 
    s.ShowtimeId,
    s.StartTime,
    s.EndTime,
    s.Price,
    s.Format,
    s.LanguageType,
    m.Title AS MovieTitle,
    c.Name AS CinemaName,
    a.Name AS AuditoriumName,
    a.SeatsCount AS TotalSeats,
    (
        a.SeatsCount - 
        ISNULL((
            SELECT COUNT(DISTINCT bs.SeatId)
            FROM BookingSeats bs
            INNER JOIN Bookings b ON bs.BookingId = b.BookingId
            WHERE bs.ShowtimeId = s.ShowtimeId
              AND b.Status IN ('Confirmed', 'Paid')
        ), 0)
    ) AS AvailableSeats
FROM Showtimes s
INNER JOIN Movies m ON s.MovieId = m.MovieId
INNER JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
INNER JOIN Cinemas c ON a.CinemaId = c.CinemaId
WHERE s.ShowtimeId = @ShowtimeId;
```

### 7.2 Check Time Slot Conflict
```sql
-- Check xem time slot có conflict không
SELECT COUNT(*) AS ConflictCount
FROM Showtimes
WHERE AuditoriumId = @AuditoriumId
  AND (
    -- New showtime starts during existing showtime
    (StartTime <= @NewStartTime AND EndTime > @NewStartTime)
    OR
    -- New showtime ends during existing showtime
    (StartTime < @NewEndTime AND EndTime >= @NewEndTime)
    OR
    -- New showtime completely overlaps existing showtime
    (@NewStartTime <= StartTime AND @NewEndTime >= EndTime)
  );
```

### 7.3 Get Showtimes Grouped by Cinema
```sql
SELECT 
    c.CinemaId,
    c.Name AS CinemaName,
    c.Address,
    JSON_QUERY((
        SELECT 
            s.ShowtimeId,
            s.StartTime,
            s.Price,
            s.Format,
            a.Name AS AuditoriumName
        FROM Showtimes s
        INNER JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
        WHERE a.CinemaId = c.CinemaId
          AND s.MovieId = @MovieId
          AND CAST(s.StartTime AS DATE) = @Date
          AND s.StartTime >= GETDATE()
        ORDER BY s.StartTime
        FOR JSON PATH
    )) AS Showtimes
FROM Cinemas c
WHERE c.CinemaId IN (
    SELECT DISTINCT a.CinemaId
    FROM Showtimes s
    INNER JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
    WHERE s.MovieId = @MovieId
      AND CAST(s.StartTime AS DATE) = @Date
)
ORDER BY c.Name;
```

## 8. Performance Optimization

### 8.1 Database Indexes
```sql
CREATE INDEX idx_showtimes_movie ON Showtimes(MovieId, StartTime);
CREATE INDEX idx_showtimes_auditorium ON Showtimes(AuditoriumId, StartTime);
CREATE INDEX idx_showtimes_starttime ON Showtimes(StartTime);
CREATE INDEX idx_showtimes_movie_date ON Showtimes(MovieId) 
    INCLUDE (StartTime, Price, Format);
```

### 8.2 Caching Strategy
```csharp
// Cache showtimes by movie & date (30 minutes)
Cache: "showtimes:movie:{movieId}:date:{date}" → List<ShowtimeDTO>

// Cache seat availability (5 minutes - short TTL vì real-time)
Cache: "showtimes:{id}:seats" → SeatAvailabilityDTO

// Cache showtimes by cinema (30 minutes)
Cache: "showtimes:cinema:{cinemaId}:date:{date}" → List<ShowtimeDTO>
```

## 9. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 404 | `SHOWTIME_NOT_FOUND` | "Không tìm thấy suất chiếu" | Showtime không tồn tại |
| 400 | `PAST_SHOWTIME` | "Không thể tạo suất chiếu trong quá khứ" | StartTime < Now |
| 409 | `TIME_SLOT_CONFLICT` | "Suất chiếu bị trùng giờ" | Auditorium đang có suất khác |
| 400 | `INVALID_PRICE` | "Giá vé không hợp lệ" | Price out of range |
| 400 | `INVALID_FORMAT` | "Format phim không hợp lệ" | Format not in enum |
| 409 | `CANNOT_DELETE_SHOWTIME` | "Không thể xóa suất chiếu có booking" | Has bookings |
| 400 | `SHOWTIME_SOLD_OUT` | "Suất chiếu đã hết vé" | AvailableSeats = 0 |

## 10. Sample API Calls

### Xem suất chiếu theo phim
```bash
GET /api/showtimes/by-movie/123?date=2025-10-30

Response:
{
  "success": true,
  "data": [
    {
      "cinemaName": "CGV Vincom Center",
      "showtimes": [
        {
          "showtimeId": 567,
          "startTime": "2025-10-30T10:00:00Z",
          "price": 60000,
          "format": "2D",
          "availableSeats": 120
        },
        {
          "showtimeId": 568,
          "startTime": "2025-10-30T13:00:00Z",
          "price": 70000,
          "format": "2D",
          "availableSeats": 95
        }
      ]
    }
  ]
}
```

### Kiểm tra ghế trống
```bash
GET /api/showtimes/567/available-seats

Response:
{
  "success": true,
  "data": {
    "showtimeId": 567,
    "totalSeats": 150,
    "availableSeats": 120,
    "bookedSeats": 30,
    "seats": [ /* array of seats */ ]
  }
}
```

### Tạo suất chiếu (Admin)
```bash
POST /api/showtimes
Authorization: Bearer {adminToken}

{
  "movieId": 123,
  "auditoriumId": 5,
  "startTime": "2025-11-01T19:30:00",
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub"
}
```

### Tạo hàng loạt (Admin)
```bash
POST /api/showtimes/bulk-create
Authorization: Bearer {adminToken}

{
  "movieId": 123,
  "auditoriumId": 5,
  "dateRange": {
    "startDate": "2025-11-01",
    "endDate": "2025-11-07"
  },
  "timeSlots": ["10:00", "13:00", "16:00", "19:00", "22:00"],
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub"
}

Response:
{
  "success": true,
  "message": "Đã tạo 35 suất chiếu",
  "data": {
    "totalCreated": 35,
    "skipped": 0,
    "conflicts": []
  }
}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
