# 🔄 Data Flow - Dòng dữ liệu giữa các Bảng

## 📖 Giới thiệu

Tài liệu này mô tả chi tiết cách dữ liệu di chuyển và tương tác giữa các bảng trong database của Movie88.

---

## 📊 1. Entity Relationship Overview

```
┌─────────┐
│  Roles  │
└────┬────┘
     │ 1
     │ has many
     │ *
┌────▼────┐         ┌───────────┐
│  User   │ 1  ─── * │ Customers │
└────┬────┘         └─────┬─────┘
     │                    │
     │                    │ *
     │                    │ has many
     │                    │ 1
     │              ┌─────▼─────┐      ┌──────────┐
     │              │ Bookings  │ * ──1 │Showtimes │
     │              └─────┬─────┘      └────┬─────┘
     │                    │                  │
     │                    │                  │ *
     │                    │                  │ belongs to
     │                    │                  │ 1
     │                    │            ┌─────▼──────┐
     │                    │            │   Movies   │
     │                    │            └────────────┘
     │                    │
     │                    │            ┌─────────────┐
     │                    │            │ Auditoriums │
     │                    │            └──────┬──────┘
     │                    │                   │ 1
     │                    │                   │ has many
     │                    │                   │ *
     │              ┌─────▼─────┐      ┌─────▼─────┐
     │              │BookingSeats│ * ──1 │  Seats   │
     │              └───────────┘      └───────────┘
     │
     │              ┌──────────────┐
     │              │BookingCombos │
     │              └──────┬───────┘
     │                     │ *
     │                     │ references
     │                     │ 1
     │               ┌─────▼─────┐
     │               │  Combos   │
     │               └───────────┘
     │
     │              ┌─────────────────┐
     │              │BookingPromotions│
     │              └────────┬────────┘
     │                       │ *
     │                       │ references
     │                       │ 1
     │                 ┌─────▼──────┐
     │                 │ Promotions │
     │                 └────────────┘
     │
     │              ┌──────────┐
     │         1 ───│ Vouchers │
     │              └──────────┘
     │
     │ 1
     │ has many
     │ *
┌────▼────┐
│Payments │
└─────────┘
```

---

## 🔄 2. Data Flow theo Use Case

### 2.1 User Registration Flow

```
Input: RegisterDTO từ Client
{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "Password123!",
  "phone": "0901234567",
  "customerInfo": {
    "address": "123 ABC Street",
    "dateOfBirth": "1995-05-15",
    "gender": "Male"
  }
}

↓

Step 1: Validate & Hash Password
├─ Check email exists in User table
├─ Validate password strength
└─ Hash password: BCrypt.HashPassword(password, 12)

↓

Step 2: Insert into User table
INSERT INTO [User] (RoleId, FullName, Email, PasswordHash, Phone, CreatedAt)
VALUES (4, 'Nguyễn Văn A', 'nguyenvana@example.com', '$2a$12...', '0901234567', GETDATE())

RETURN UserId = 5

↓

Step 3: Insert into Customers table
INSERT INTO Customers (UserId, Address, DateOfBirth, Gender)
VALUES (5, '123 ABC Street', '1995-05-15', 'Male')

RETURN CustomerId = 3

↓

Output: UserDTO
{
  "userId": 5,
  "customerId": 3,
  "roleId": 4,
  "roleName": "Customer",
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "accessToken": "eyJhbG...",
  "refreshToken": "refresh_token_here"
}
```

---

### 2.2 Movie & Showtime Creation Flow

```
Admin tạo phim mới
↓
POST /api/movies
{
  "title": "Avengers: Endgame",
  "description": "...",
  "durationMinutes": 181,
  "director": "Russo Brothers",
  "releaseDate": "2019-04-26",
  "genre": "Action, Adventure, Sci-Fi",
  "rating": "T13"
}

↓

INSERT INTO Movies (Title, Description, DurationMinutes, Director, ReleaseDate, Genre, Rating, CreatedAt)
VALUES ('Avengers: Endgame', '...', 181, 'Russo Brothers', '2019-04-26', 'Action, Adventure, Sci-Fi', 'T13', GETDATE())

RETURN MovieId = 123

↓

Admin tạo suất chiếu
↓
POST /api/showtimes
{
  "movieId": 123,
  "auditoriumId": 5,
  "startTime": "2025-10-30T19:30:00Z",
  "price": 80000,
  "format": "2D",
  "languageType": "Original - Vietsub"
}

↓

Step 1: Validate
├─ Check MovieId exists in Movies
├─ Check AuditoriumId exists in Auditoriums
├─ Check time slot available (no overlap)
└─ Calculate EndTime = StartTime + Movie.DurationMinutes

↓

Step 2: Insert Showtime
INSERT INTO Showtimes (MovieId, AuditoriumId, StartTime, EndTime, Price, Format, LanguageType)
VALUES (123, 5, '2025-10-30 19:30:00', '2025-10-30 22:31:00', 80000, '2D', 'Original - Vietsub')

RETURN ShowtimeId = 567

↓

Result: ShowtimeDTO
{
  "showtimeId": 567,
  "movieTitle": "Avengers: Endgame",
  "cinemaName": "CGV Vincom Center",
  "auditoriumName": "Phòng 5",
  "startTime": "2025-10-30T19:30:00Z",
  "price": 80000
}
```

---

### 2.3 Complete Booking Flow (Chi tiết nhất)

```
┌─────────────────────────────────────────────────────────────────┐
│ PHASE 1: CREATE BOOKING                                         │
└─────────────────────────────────────────────────────────────────┘

Customer chọn suất chiếu
↓
POST /api/bookings/create
{
  "showtimeId": 567,
  "customerId": 3
}

↓

Validate:
├─ Customer.CustomerId = 3 exists?
├─ Showtime.ShowtimeId = 567 exists?
└─ Showtime.StartTime > NOW()?

↓

INSERT INTO Bookings (CustomerId, ShowtimeId, BookingTime, TotalAmount, Status)
VALUES (3, 567, GETDATE(), 0, 'Pending')

RETURN BookingId = 1001

┌─────────────────────────────────────────────────────────────────┐
│ PHASE 2: SELECT SEATS                                           │
└─────────────────────────────────────────────────────────────────┘

GET /api/showtimes/567/available-seats
↓
Query:
SELECT s.SeatId, s.Row, s.Number, s.Type, s.IsAvailable
FROM Seats s
WHERE s.AuditoriumId = (
    SELECT AuditoriumId FROM Showtimes WHERE ShowtimeId = 567
)
AND s.SeatId NOT IN (
    SELECT SeatId FROM BookingSeats 
    WHERE ShowtimeId = 567 
    AND BookingId IN (
        SELECT BookingId FROM Bookings 
        WHERE Status IN ('Confirmed', 'Paid')
    )
)

↓

Customer chọn ghế: D5, D6
↓
POST /api/bookings/1001/select-seats
{
  "seatIds": [45, 46]
}

↓

Validate:
├─ Check ghế available?
│   SELECT IsAvailable FROM Seats WHERE SeatId IN (45, 46)
├─ Check ghế chưa bị đặt?
│   SELECT * FROM BookingSeats 
│   WHERE ShowtimeId = 567 AND SeatId IN (45, 46)
└─ Check số lượng ghế hợp lệ? (1-10)

↓

FOR EACH seatId IN [45, 46]:
    INSERT INTO BookingSeats (BookingId, ShowtimeId, SeatId, SeatPrice)
    VALUES (1001, 567, seatId, 80000)

↓

Calculate seats total:
SELECT SUM(SeatPrice) FROM BookingSeats WHERE BookingId = 1001
→ seatsTotal = 160000

↓

UPDATE Bookings SET TotalAmount = 160000 WHERE BookingId = 1001

┌─────────────────────────────────────────────────────────────────┐
│ PHASE 3: ADD COMBOS (Optional)                                  │
└─────────────────────────────────────────────────────────────────┘

Customer chọn combo
↓
POST /api/bookings/1001/add-combos
{
  "combos": [
    { "comboId": 1, "quantity": 2 }
  ]
}

↓

Validate:
└─ Check ComboId = 1 exists in Combos?
    SELECT Price FROM Combos WHERE ComboId = 1
    → comboPrice = 60000

↓

INSERT INTO BookingCombos (BookingId, ComboId, Quantity, ComboPrice)
VALUES (1001, 1, 2, 60000)

↓

Calculate total:
├─ seatsTotal = 160000
├─ combosTotal = 60000 * 2 = 120000
└─ subtotal = 280000

↓

UPDATE Bookings SET TotalAmount = 280000 WHERE BookingId = 1001

┌─────────────────────────────────────────────────────────────────┐
│ PHASE 4: APPLY VOUCHER (Optional)                               │
└─────────────────────────────────────────────────────────────────┘

Customer nhập voucher
↓
POST /api/bookings/1001/apply-voucher
{
  "voucherCode": "SUMMER2025"
}

↓

Validate voucher:
SELECT * FROM Vouchers WHERE Code = 'SUMMER2025'

Check:
├─ IsActive = 1?
├─ ExpiryDate > NOW()?
├─ UsedCount < UsageLimit?
└─ Booking.TotalAmount >= MinPurchaseAmount?

↓

Voucher valid:
{
  "voucherId": 10,
  "discountType": "Percent",
  "discountValue": 10,
  "minPurchaseAmount": 100000
}

↓

Calculate discount:
IF discountType = 'Percent':
    discount = subtotal * (discountValue / 100)
    → discount = 280000 * 0.1 = 28000
ELSE:
    discount = discountValue

↓

UPDATE Bookings 
SET VoucherId = 10, 
    TotalAmount = 280000 - 28000
WHERE BookingId = 1001

→ TotalAmount = 252000

┌─────────────────────────────────────────────────────────────────┐
│ PHASE 5: APPLY PROMOTIONS (Auto)                                │
└─────────────────────────────────────────────────────────────────┘

System check active promotions:
SELECT * FROM Promotions 
WHERE IsActive = 1 
AND StartDate <= GETDATE() 
AND EndDate >= GETDATE()

↓

Found promotion:
{
  "promotionId": 5,
  "name": "Weekend Special",
  "discountType": "Percent",
  "discountValue": 5
}

↓

Apply promotion:
INSERT INTO BookingPromotions (BookingId, PromotionId, DiscountApplied)
VALUES (1001, 5, 252000 * 0.05)

→ DiscountApplied = 12600

↓

UPDATE Bookings 
SET TotalAmount = 252000 - 12600
WHERE BookingId = 1001

→ Final TotalAmount = 239400

┌─────────────────────────────────────────────────────────────────┐
│ PHASE 6: PAYMENT                                                │
└─────────────────────────────────────────────────────────────────┘

PUT /api/bookings/1001/confirm
↓
UPDATE Bookings SET Status = 'Confirmed' WHERE BookingId = 1001

↓

POST /api/payments/vnpay/create
{
  "bookingId": 1001,
  "amount": 239400
}

↓

INSERT INTO Payments (BookingId, CustomerId, MethodId, Amount, Status, TransactionCode, PaymentTime)
VALUES (1001, 3, 1, 239400, 'Pending', 'VNP_20251029_1001', GETDATE())

RETURN PaymentId = 5001

↓

Redirect to VNPay
↓
Customer thanh toán thành công
↓
VNPay callback: vnp_ResponseCode = 00

↓

UPDATE Payments SET Status = 'Success' WHERE PaymentId = 5001
UPDATE Bookings SET Status = 'Paid' WHERE BookingId = 1001
UPDATE Vouchers SET UsedCount = UsedCount + 1 WHERE VoucherId = 10

↓

Update seats (mark as sold):
UPDATE Seats SET IsAvailable = 0 
WHERE SeatId IN (
    SELECT SeatId FROM BookingSeats WHERE BookingId = 1001
)

↓

Generate QR Code
Send Confirmation Email

┌─────────────────────────────────────────────────────────────────┐
│ FINAL STATE                                                      │
└─────────────────────────────────────────────────────────────────┘

Bookings table:
{
  "bookingId": 1001,
  "customerId": 3,
  "showtimeId": 567,
  "voucherId": 10,
  "totalAmount": 239400,
  "status": "Paid",
  "bookingTime": "2025-10-29T10:00:00Z"
}

BookingSeats table:
[
  { "bookingSeatId": 1, "bookingId": 1001, "showtimeId": 567, "seatId": 45, "seatPrice": 80000 },
  { "bookingSeatId": 2, "bookingId": 1001, "showtimeId": 567, "seatId": 46, "seatPrice": 80000 }
]

BookingCombos table:
[
  { "bookingComboId": 1, "bookingId": 1001, "comboId": 1, "quantity": 2, "comboPrice": 60000 }
]

BookingPromotions table:
[
  { "bookingPromotionId": 1, "bookingId": 1001, "promotionId": 5, "discountApplied": 12600 }
]

Payments table:
{
  "paymentId": 5001,
  "bookingId": 1001,
  "customerId": 3,
  "methodId": 1,
  "amount": 239400,
  "status": "Success",
  "transactionCode": "VNP_20251029_1001",
  "paymentTime": "2025-10-29T11:00:00Z"
}

Seats table:
UPDATE: SeatId 45, 46 → IsAvailable = 0
```

---

## 📊 3. Complex Queries

### 3.1 Get Booking Details with All Relations

```sql
SELECT 
    -- Booking info
    b.BookingId,
    b.TotalAmount,
    b.Status,
    b.BookingTime,
    
    -- Customer info
    c.CustomerId,
    u.FullName AS CustomerName,
    u.Email,
    u.Phone,
    
    -- Showtime info
    s.ShowtimeId,
    s.StartTime,
    s.EndTime,
    s.Format,
    s.LanguageType,
    
    -- Movie info
    m.MovieId,
    m.Title AS MovieTitle,
    m.PosterUrl,
    m.DurationMinutes,
    
    -- Cinema & Auditorium info
    cin.Name AS CinemaName,
    cin.Address AS CinemaAddress,
    aud.Name AS AuditoriumName,
    
    -- Seats
    (
        SELECT STRING_AGG(CONCAT(seat.Row, seat.Number), ', ')
        FROM BookingSeats bs
        INNER JOIN Seats seat ON bs.SeatId = seat.SeatId
        WHERE bs.BookingId = b.BookingId
    ) AS SelectedSeats,
    
    -- Combos
    (
        SELECT SUM(bc.Quantity * bc.ComboPrice)
        FROM BookingCombos bc
        WHERE bc.BookingId = b.BookingId
    ) AS CombosTotal,
    
    -- Voucher
    v.Code AS VoucherCode,
    v.DiscountType AS VoucherDiscountType,
    v.DiscountValue AS VoucherDiscountValue,
    
    -- Payment
    p.PaymentId,
    p.Status AS PaymentStatus,
    p.TransactionCode,
    pm.Name AS PaymentMethodName

FROM Bookings b
INNER JOIN Customers c ON b.CustomerId = c.CustomerId
INNER JOIN [User] u ON c.UserId = u.UserId
INNER JOIN Showtimes s ON b.ShowtimeId = s.ShowtimeId
INNER JOIN Movies m ON s.MovieId = m.MovieId
INNER JOIN Auditoriums aud ON s.AuditoriumId = aud.AuditoriumId
INNER JOIN Cinemas cin ON aud.CinemaId = cin.CinemaId
LEFT JOIN Vouchers v ON b.VoucherId = v.VoucherId
LEFT JOIN Payments p ON b.BookingId = p.BookingId
LEFT JOIN PaymentMethods pm ON p.MethodId = pm.MethodId

WHERE b.BookingId = 1001;
```

---

### 3.2 Get Available Seats for Showtime

```sql
SELECT 
    s.SeatId,
    s.Row,
    s.Number,
    s.Type,
    CASE 
        WHEN bs.SeatId IS NULL THEN 1
        ELSE 0
    END AS IsAvailable
FROM Seats s
LEFT JOIN (
    SELECT DISTINCT bs.SeatId
    FROM BookingSeats bs
    INNER JOIN Bookings b ON bs.BookingId = b.BookingId
    WHERE bs.ShowtimeId = 567
      AND b.Status IN ('Confirmed', 'Paid')
) bs ON s.SeatId = bs.SeatId
WHERE s.AuditoriumId = (
    SELECT AuditoriumId FROM Showtimes WHERE ShowtimeId = 567
)
ORDER BY s.Row, s.Number;
```

---

### 3.3 Revenue Report by Movie

```sql
SELECT 
    m.MovieId,
    m.Title,
    COUNT(DISTINCT b.BookingId) AS TotalBookings,
    SUM(
        (SELECT SUM(SeatPrice) FROM BookingSeats WHERE BookingId = b.BookingId)
    ) AS SeatsRevenue,
    SUM(
        (SELECT SUM(Quantity * ComboPrice) FROM BookingCombos WHERE BookingId = b.BookingId)
    ) AS CombosRevenue,
    SUM(b.TotalAmount) AS TotalRevenue
FROM Movies m
INNER JOIN Showtimes s ON m.MovieId = s.MovieId
INNER JOIN Bookings b ON s.ShowtimeId = b.ShowtimeId
INNER JOIN Payments p ON b.BookingId = p.BookingId
WHERE p.Status = 'Success'
  AND b.Status = 'Paid'
  AND p.PaymentTime BETWEEN '2025-10-01' AND '2025-10-31'
GROUP BY m.MovieId, m.Title
ORDER BY TotalRevenue DESC;
```

---

## 🔄 4. Data Consistency Rules

### 4.1 Booking Constraints

```sql
-- Constraint: Không đặt trùng ghế
ALTER TABLE BookingSeats
ADD CONSTRAINT UQ_ShowtimeSeat UNIQUE (ShowtimeId, SeatId);

-- Constraint: TotalAmount >= 0
ALTER TABLE Bookings
ADD CONSTRAINT CHK_TotalAmount CHECK (TotalAmount >= 0);

-- Constraint: Chỉ 1 voucher/booking
ALTER TABLE Bookings
ADD CONSTRAINT FK_Voucher FOREIGN KEY (VoucherId) REFERENCES Vouchers(VoucherId);
```

---

### 4.2 Status Transitions (State Machine)

```
Bookings.Status:
Pending → Confirmed → Paid → Completed
   ↓         ↓          ↓
Expired  Cancelled  Cancelled

Payments.Status:
Pending → Processing → Success
   ↓          ↓           ↓
Expired   Failed    Refunded
```

---

### 4.3 Cascading Updates/Deletes

```sql
-- Khi xóa User → Cascade xóa Customer
ALTER TABLE Customers
ADD CONSTRAINT FK_User 
FOREIGN KEY (UserId) REFERENCES [User](UserId)
ON DELETE CASCADE;

-- Khi xóa Booking → Cascade xóa BookingSeats
ALTER TABLE BookingSeats
ADD CONSTRAINT FK_Booking
FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId)
ON DELETE CASCADE;
```

---

## 📈 5. Performance Optimization

### 5.1 Indexes

```sql
-- Booking queries
CREATE INDEX idx_bookings_customer ON Bookings(CustomerId);
CREATE INDEX idx_bookings_showtime ON Bookings(ShowtimeId);
CREATE INDEX idx_bookings_status ON Bookings(Status);

-- Showtime queries
CREATE INDEX idx_showtimes_movie ON Showtimes(MovieId);
CREATE INDEX idx_showtimes_cinema ON Showtimes(AuditoriumId);
CREATE INDEX idx_showtimes_date ON Showtimes(StartTime);

-- Seat availability
CREATE INDEX idx_booking_seats_showtime ON BookingSeats(ShowtimeId, SeatId);
CREATE INDEX idx_seats_auditorium ON Seats(AuditoriumId);

-- Payment lookups
CREATE INDEX idx_payments_booking ON Payments(BookingId);
CREATE INDEX idx_payments_transaction ON Payments(TransactionCode);
```

---

**Last Updated**: October 29, 2025
**Version**: v1.0
