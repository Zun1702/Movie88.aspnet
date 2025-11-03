# 🏢 Cinema & Auditorium Management API

## 1. Mô tả

Module Cinema quản lý toàn bộ thông tin về rạp chiếu phim, phòng chiếu và sơ đồ ghế ngồi, bao gồm:
- Quản lý thông tin cụm rạp (CRUD)
- Quản lý phòng chiếu trong từng rạp
- Tạo và quản lý sơ đồ ghế ngồi
- Phân loại ghế (Standard, VIP, Couple)
- Hiển thị thông tin rạp cho khách hàng
- Tìm kiếm rạp gần vị trí người dùng

## 2. Danh sách Endpoint

### 2.1 Cinema Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/cinemas` | Lấy danh sách rạp | Query params | List<CinemaDTO> | Public |
| GET | `/api/cinemas/{id}` | Lấy chi tiết rạp | cinemaId | CinemaDetailDTO | Public |
| GET | `/api/cinemas/{id}/auditoriums` | Danh sách phòng chiếu | cinemaId | List<AuditoriumDTO> | Public |
| GET | `/api/cinemas/nearby` | Rạp gần vị trí | lat, lng, radius | List<CinemaDTO> | Public |
| POST | `/api/cinemas` | Thêm rạp mới | CreateCinemaDTO | CinemaDTO | Admin |
| PUT | `/api/cinemas/{id}` | Cập nhật rạp | UpdateCinemaDTO | CinemaDTO | Admin/Manager |
| DELETE | `/api/cinemas/{id}` | Xóa rạp | cinemaId | Success message | Admin |

### 2.2 Auditorium Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/auditoriums/{id}` | Chi tiết phòng chiếu | auditoriumId | AuditoriumDetailDTO | Public |
| GET | `/api/auditoriums/{id}/seats` | Sơ đồ ghế | auditoriumId | SeatMapDTO | Public |
| POST | `/api/auditoriums` | Thêm phòng chiếu | CreateAuditoriumDTO | AuditoriumDTO | Admin/Manager |
| PUT | `/api/auditoriums/{id}` | Cập nhật phòng chiếu | UpdateAuditoriumDTO | AuditoriumDTO | Admin/Manager |
| DELETE | `/api/auditoriums/{id}` | Xóa phòng chiếu | auditoriumId | Success message | Admin |

### 2.3 Seat Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/seats/auditorium/{auditoriumId}` | Danh sách ghế | auditoriumId | List<SeatDTO> | Public |
| POST | `/api/seats/bulk-create` | Tạo hàng loạt ghế | BulkCreateSeatsDTO | Success message | Admin/Manager |
| PUT | `/api/seats/{id}` | Cập nhật ghế | UpdateSeatDTO | SeatDTO | Admin/Manager |
| PUT | `/api/seats/{id}/availability` | Cập nhật trạng thái | IsAvailable | Success message | Admin/Manager |

## 3. Data Transfer Objects (DTOs)

### 3.1 CinemaDTO
```json
{
  "cinemaId": 1,
  "name": "CGV Vincom Center",
  "address": "72 Lê Thánh Tôn, Quận 1, TP.HCM",
  "city": "Hồ Chí Minh",
  "phone": "1900 6017",
  "totalAuditoriums": 8,
  "totalSeats": 1200,
  "facilities": ["3D", "IMAX", "4DX", "Dolby Atmos"],
  "createdAt": "2020-01-01T00:00:00Z"
}
```

### 3.2 CinemaDetailDTO
```json
{
  "cinemaId": 1,
  "name": "CGV Vincom Center",
  "address": "72 Lê Thánh Tôn, Quận 1, TP.HCM",
  "city": "Hồ Chí Minh",
  "phone": "1900 6017",
  "email": "support@cgv.vn",
  "website": "https://cgv.vn",
  "latitude": 10.7718,
  "longitude": 106.7009,
  "openingHours": "09:00 - 23:00",
  "facilities": ["3D", "IMAX", "4DX", "Dolby Atmos"],
  "auditoriums": [
    {
      "auditoriumId": 1,
      "name": "Phòng 1",
      "seatsCount": 150,
      "hasIMAX": false,
      "has3D": true
    },
    {
      "auditoriumId": 2,
      "name": "IMAX Hall",
      "seatsCount": 300,
      "hasIMAX": true,
      "has3D": true
    }
  ],
  "totalAuditoriums": 8,
  "totalSeats": 1200,
  "createdAt": "2020-01-01T00:00:00Z"
}
```

### 3.3 CreateCinemaDTO
```json
{
  "name": "Galaxy Nguyễn Du",
  "address": "116 Nguyễn Du, Quận 1, TP.HCM",
  "city": "Hồ Chí Minh",
  "phone": "1900 2224",
  "email": "support@galaxycine.vn",
  "website": "https://galaxycine.vn",
  "latitude": 10.7756,
  "longitude": 106.6946,
  "openingHours": "08:00 - 24:00"
}
```

**Validation Rules:**
- `name`: Required, 3-100 ký tự, unique per city
- `address`: Required, 10-255 ký tự
- `city`: Required, 2-100 ký tự
- `phone`: Required, format: 10 số hoặc 1900 xxxx
- `latitude`: -90 to 90
- `longitude`: -180 to 180

### 3.4 AuditoriumDTO
```json
{
  "auditoriumId": 1,
  "cinemaId": 1,
  "cinemaName": "CGV Vincom Center",
  "name": "Phòng 1",
  "seatsCount": 150,
  "rows": 10,
  "seatsPerRow": 15,
  "hasIMAX": false,
  "has3D": true,
  "hasDolbyAtmos": true
}
```

### 3.5 AuditoriumDetailDTO
```json
{
  "auditoriumId": 1,
  "cinemaId": 1,
  "cinemaName": "CGV Vincom Center",
  "name": "Phòng 1",
  "seatsCount": 150,
  "rows": 10,
  "seatsPerRow": 15,
  "hasIMAX": false,
  "has3D": true,
  "hasDolbyAtmos": true,
  "seatLayout": {
    "rows": ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"],
    "seatsPerRow": 15,
    "vipRows": ["F", "G"],
    "coupleSeats": ["H7-H8", "I7-I8", "J7-J8"]
  }
}
```

### 3.6 CreateAuditoriumDTO
```json
{
  "cinemaId": 1,
  "name": "Phòng 3",
  "rows": 10,
  "seatsPerRow": 15,
  "hasIMAX": false,
  "has3D": true,
  "hasDolbyAtmos": false
}
```

**Validation Rules:**
- `name`: Required, unique trong cùng cinema
- `rows`: Required, 5-20
- `seatsPerRow`: Required, 10-30
- Tổng ghế = rows × seatsPerRow

### 3.7 SeatDTO
```json
{
  "seatId": 45,
  "auditoriumId": 1,
  "row": "D",
  "number": 5,
  "type": "Standard",
  "isAvailable": true,
  "position": {
    "x": 5,
    "y": 4
  }
}
```

**Seat Types:**
- `Standard`: Ghế thường (giá cơ bản)
- `VIP`: Ghế VIP (giá cao hơn 30%)
- `Couple`: Ghế đôi (giá cao hơn 50%)
- `Wheelchair`: Ghế dành cho người khuyết tật

### 3.8 SeatMapDTO
```json
{
  "auditoriumId": 1,
  "auditoriumName": "Phòng 1",
  "rows": 10,
  "seatsPerRow": 15,
  "totalSeats": 150,
  "seats": [
    {
      "seatId": 1,
      "row": "A",
      "number": 1,
      "type": "Standard",
      "isAvailable": true
    },
    {
      "seatId": 2,
      "row": "A",
      "number": 2,
      "type": "Standard",
      "isAvailable": true
    }
    // ... 148 seats more
  ],
  "legend": {
    "standard": "Ghế thường",
    "vip": "Ghế VIP",
    "couple": "Ghế đôi",
    "wheelchair": "Ghế người khuyết tật"
  }
}
```

### 3.9 BulkCreateSeatsDTO
```json
{
  "auditoriumId": 1,
  "seatConfig": {
    "rowLabels": ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"],
    "seatsPerRow": 15,
    "vipRows": ["F", "G"],
    "coupleSeats": [
      { "row": "H", "numbers": [7, 8] },
      { "row": "I", "numbers": [7, 8] }
    ],
    "skipSeats": [
      { "row": "A", "numbers": [1, 15] }
    ]
  }
}
```

## 4. Luồng xử lý (Flow)

### 4.1 Browse Cinemas Flow

```
User vào trang "Chọn rạp"
↓
GET /api/cinemas?city=Hồ Chí Minh
↓
Backend query:
SELECT * FROM Cinemas
WHERE City = 'Hồ Chí Minh'
ORDER BY Name
↓
Return List<CinemaDTO>
↓
Frontend hiển thị:
- Map với các marker rạp
- List view với thông tin cơ bản
- Filter theo city
```

### 4.2 Find Nearby Cinemas Flow

```
User bật location permission
↓
Frontend lấy lat/lng của user
↓
GET /api/cinemas/nearby?lat=10.7718&lng=106.7009&radius=5
↓
Backend tính khoảng cách (Haversine formula):
SELECT *,
    (6371 * acos(
        cos(radians(10.7718)) * 
        cos(radians(Latitude)) * 
        cos(radians(Longitude) - radians(106.7009)) + 
        sin(radians(10.7718)) * 
        sin(radians(Latitude))
    )) AS distance
FROM Cinemas
HAVING distance < 5
ORDER BY distance
↓
Return List<CinemaDTO> sorted by distance
↓
Frontend hiển thị rạp gần nhất trước
```

### 4.3 View Seat Map Flow

```
User chọn suất chiếu → Redirect to seat selection
↓
GET /api/auditoriums/{auditoriumId}/seats
↓
Backend query:
SELECT s.* 
FROM Seats s
WHERE s.AuditoriumId = {auditoriumId}
ORDER BY s.Row, s.Number
↓
Return SeatMapDTO với grid layout
↓
Frontend render sơ đồ ghế:
- Available seats: Màu xanh
- Selected seats: Màu vàng
- Taken seats: Màu xám
- VIP seats: Màu đỏ
- Couple seats: Ghế đôi liền nhau
```

### 4.4 Admin Create Cinema Flow

```
Admin vào "Thêm rạp mới"
↓
Điền form CreateCinemaDTO
↓
POST /api/cinemas
Authorization: Bearer {adminToken}
↓
Backend validate:
├─ Check name unique trong city
├─ Validate phone format
├─ Validate lat/lng range
└─ Validate all required fields
↓
INSERT INTO Cinemas (Name, Address, City, ...)
VALUES (...)
↓
Return CinemaDTO với cinemaId mới
↓
Admin tiếp tục thêm phòng chiếu:
POST /api/auditoriums
{
  "cinemaId": {newCinemaId},
  "name": "Phòng 1",
  "rows": 10,
  "seatsPerRow": 15
}
↓
INSERT INTO Auditoriums (...)
↓
Auto-generate seats:
POST /api/seats/bulk-create
```

### 4.5 Bulk Create Seats Flow

```
Admin tạo phòng chiếu mới → Tạo ghế tự động
↓
POST /api/seats/bulk-create
{
  "auditoriumId": 1,
  "seatConfig": {
    "rowLabels": ["A", "B", "C", "D", "E"],
    "seatsPerRow": 10,
    "vipRows": ["D", "E"]
  }
}
↓
Backend generate seats:
FOR EACH row IN rowLabels:
    FOR number FROM 1 TO seatsPerRow:
        seatType = (row IN vipRows) ? "VIP" : "Standard"
        
        INSERT INTO Seats (AuditoriumId, Row, Number, Type, IsAvailable)
        VALUES (1, row, number, seatType, 1)
↓
Total seats inserted: 5 × 10 = 50 ghế
↓
Return success message
```

## 5. Business Rules

### 5.1 Cinema Rules
- Mỗi city có thể có nhiều cinemas
- Cinema name phải unique trong cùng city
- Phone phải unique trong hệ thống
- Không xóa cinema nếu có showtimes trong tương lai

### 5.2 Auditorium Rules
- Mỗi cinema có 1-20 auditoriums
- Auditorium name phải unique trong cùng cinema
- Số ghế = rows × seatsPerRow (tối thiểu 50, tối đa 500)
- Không xóa auditorium nếu có showtimes trong tương lai

### 5.3 Seat Rules
- Row labels: A-Z (A gần màn hình nhất)
- Seat numbers: 1-30 (trái sang phải)
- Constraint: UQ_Seat (AuditoriumId, Row, Number)
- VIP rows thường ở giữa phòng (best view)
- Couple seats luôn thành cặp (số chẵn)

### 5.4 Seat Pricing
```
Standard seat: Base price (từ Showtime.Price)
VIP seat: Base price × 1.3
Couple seat: Base price × 1.5
```

## 6. Validation Rules

### CreateCinemaDTO Validation
```csharp
public class CreateCinemaDTO
{
    [Required(ErrorMessage = "Tên rạp là bắt buộc")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 10)]
    public string Address { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string City { get; set; }

    [Required]
    [Phone]
    [RegularExpression(@"^(0\d{9}|1900\s?\d{4})$")]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Url]
    public string Website { get; set; }

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }
}
```

### CreateAuditoriumDTO Validation
```csharp
public class CreateAuditoriumDTO
{
    [Required]
    public int CinemaId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    [Range(5, 20, ErrorMessage = "Số hàng từ 5-20")]
    public int Rows { get; set; }

    [Required]
    [Range(10, 30, ErrorMessage = "Số ghế mỗi hàng từ 10-30")]
    public int SeatsPerRow { get; set; }
}
```

## 7. Complex Queries

### 7.1 Find Nearby Cinemas (Haversine)
```sql
-- Tính khoảng cách từ user location đến các rạp
DECLARE @UserLat FLOAT = 10.7718;
DECLARE @UserLng FLOAT = 106.7009;
DECLARE @Radius FLOAT = 5; -- km

SELECT 
    CinemaId,
    Name,
    Address,
    City,
    Phone,
    (6371 * acos(
        cos(radians(@UserLat)) * 
        cos(radians(Latitude)) * 
        cos(radians(Longitude) - radians(@UserLng)) + 
        sin(radians(@UserLat)) * 
        sin(radians(Latitude))
    )) AS Distance
FROM Cinemas
WHERE Latitude IS NOT NULL 
  AND Longitude IS NOT NULL
HAVING Distance < @Radius
ORDER BY Distance;
```

### 7.2 Get Cinema with Auditoriums Count
```sql
SELECT 
    c.CinemaId,
    c.Name,
    c.Address,
    c.City,
    c.Phone,
    COUNT(DISTINCT a.AuditoriumId) AS TotalAuditoriums,
    SUM(a.SeatsCount) AS TotalSeats
FROM Cinemas c
LEFT JOIN Auditoriums a ON c.CinemaId = a.CinemaId
GROUP BY c.CinemaId, c.Name, c.Address, c.City, c.Phone
ORDER BY c.Name;
```

### 7.3 Generate Seat Map
```sql
-- Lấy tất cả ghế của phòng, sắp xếp theo hàng và số
SELECT 
    s.SeatId,
    s.Row,
    s.Number,
    s.Type,
    s.IsAvailable,
    a.Name AS AuditoriumName
FROM Seats s
INNER JOIN Auditoriums a ON s.AuditoriumId = a.AuditoriumId
WHERE s.AuditoriumId = @AuditoriumId
ORDER BY s.Row, s.Number;
```

## 8. Performance Optimization

### 8.1 Database Indexes
```sql
-- Cinema indexes
CREATE INDEX idx_cinemas_city ON Cinemas(City);
CREATE INDEX idx_cinemas_location ON Cinemas(Latitude, Longitude);

-- Auditorium indexes
CREATE INDEX idx_auditoriums_cinema ON Auditoriums(CinemaId);

-- Seat indexes
CREATE INDEX idx_seats_auditorium ON Seats(AuditoriumId);
CREATE UNIQUE INDEX idx_seats_unique ON Seats(AuditoriumId, Row, Number);
```

### 8.2 Caching
```csharp
// Cache cinema list per city (1 hour)
Cache: "cinemas:city:{cityName}" → List<CinemaDTO>

// Cache seat map (30 minutes)
Cache: "seatmap:auditorium:{auditoriumId}" → SeatMapDTO

// Cache cinema detail (2 hours)
Cache: "cinema:detail:{cinemaId}" → CinemaDetailDTO
```

## 9. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 404 | `CINEMA_NOT_FOUND` | "Không tìm thấy rạp" | Cinema không tồn tại |
| 404 | `AUDITORIUM_NOT_FOUND` | "Không tìm thấy phòng chiếu" | Auditorium không tồn tại |
| 409 | `CINEMA_NAME_EXISTS` | "Tên rạp đã tồn tại trong thành phố này" | Duplicate name |
| 409 | `AUDITORIUM_NAME_EXISTS` | "Tên phòng đã tồn tại trong rạp này" | Duplicate auditorium |
| 409 | `SEAT_EXISTS` | "Ghế đã tồn tại" | Duplicate seat position |
| 409 | `CANNOT_DELETE_CINEMA` | "Không thể xóa rạp có suất chiếu" | Cinema có showtimes |
| 400 | `INVALID_SEAT_CONFIG` | "Cấu hình ghế không hợp lệ" | Invalid seat layout |

## 10. Sample API Calls

### Lấy danh sách rạp
```bash
GET /api/cinemas?city=Hồ Chí Minh

Response:
{
  "success": true,
  "data": [
    {
      "cinemaId": 1,
      "name": "CGV Vincom Center",
      "address": "72 Lê Thánh Tôn, Quận 1",
      "city": "Hồ Chí Minh",
      "phone": "1900 6017",
      "totalAuditoriums": 8
    }
  ]
}
```

### Tìm rạp gần
```bash
GET /api/cinemas/nearby?lat=10.7718&lng=106.7009&radius=5

Response:
{
  "success": true,
  "data": [
    {
      "cinemaId": 1,
      "name": "CGV Vincom Center",
      "distance": 1.2,
      "address": "72 Lê Thánh Tôn"
    },
    {
      "cinemaId": 2,
      "name": "Galaxy Nguyễn Du",
      "distance": 2.5,
      "address": "116 Nguyễn Du"
    }
  ]
}
```

### Xem sơ đồ ghế
```bash
GET /api/auditoriums/1/seats

Response:
{
  "success": true,
  "data": {
    "auditoriumId": 1,
    "auditoriumName": "Phòng 1",
    "rows": 10,
    "seatsPerRow": 15,
    "seats": [
      {
        "seatId": 1,
        "row": "A",
        "number": 1,
        "type": "Standard",
        "isAvailable": true
      }
      // ... more seats
    ]
  }
}
```

### Tạo rạp mới (Admin)
```bash
POST /api/cinemas
Authorization: Bearer {adminToken}

{
  "name": "Lotte Cinema Cộng Hòa",
  "address": "Tầng 3, Lotte Mart Cộng Hòa",
  "city": "Hồ Chí Minh",
  "phone": "1900 5454",
  "latitude": 10.8024,
  "longitude": 106.6476
}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
