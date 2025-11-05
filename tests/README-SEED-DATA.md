# 🎬 Database Seeding Scripts

Scripts để tạo test data cho hệ thống đặt vé xem phim.

## 📁 Files

### 1. `seed-seats.sql`
Tạo **50 ghế** cho auditorium (layout 10 hàng x 5 ghế)

**Layout:**
- **Rows A-C** (15 seats): Standard seats (front rows)
- **Rows D-F** (15 seats): Mix Standard + VIP (VIP ở giữa)
- **Row G** (5 seats): All VIP (premium row)
- **Rows H-J** (15 seats): Couple seats (back rows)

**Breakdown:**
- Standard: 18 seats
- VIP: 17 seats  
- Couple: 15 seats
- **Total: 50 seats**

### 2. `seed-seats-100.sql`
Tạo **100 ghế** cho auditorium lớn (layout 10 hàng x 10 ghế)

**Layout:**
- **Rows A-C** (30 seats): Standard seats (front rows)
- **Rows D-F** (30 seats): VIP center + Standard sides
- **Row G** (10 seats): Deluxe seats (premium row)
- **Rows H-J** (30 seats): Couple seats (back rows)

**Breakdown:**
- Standard: 36 seats
- VIP: 18 seats
- Deluxe: 10 seats
- Couple: 30 seats
- **Total: 100 seats**

### 3. `seed-test-bookings.sql`
Tạo test bookings để kiểm tra seat availability

**Test scenarios:**
- ✅ Booking 1: A1, A2, A3 (confirmed)
- ✅ Booking 2: D2, D3, D4 (confirmed)
- ✅ Booking 3: H1, H2 (confirmed)
- ❌ Booking 4: C1 (cancelled - should show as available)

## 🚀 Cách sử dụng

### Option 1: Run từng file riêng lẻ

```bash
# 1. Create 50 seats for auditorium 1
psql -h your-host -U your-user -d your-database -f seed-seats.sql

# 2. (Optional) Create 100 seats instead
psql -h your-host -U your-user -d your-database -f seed-seats-100.sql

# 3. Create test bookings
psql -h your-host -U your-user -d your-database -f seed-test-bookings.sql
```

### Option 2: Copy-paste vào Supabase SQL Editor

1. Mở Supabase Dashboard → SQL Editor
2. Copy nội dung từ `seed-seats.sql`
3. Click "Run" để execute
4. Lặp lại với các file khác

### Option 3: Run trực tiếp từ pgAdmin hoặc DBeaver

1. Connect tới PostgreSQL database
2. Open SQL script
3. Execute

## ⚙️ Customization

### Thay đổi Auditorium ID

Mặc định scripts seed cho `auditoriumid = 1`. Để thay đổi:

**seed-seats.sql:**
```sql
-- Thay đổi auditoriumid trong mỗi INSERT statement
INSERT INTO seats (auditoriumid, row, number, type, isavailable) VALUES
(2, 'A', 1, 'Standard', true),  -- Đổi từ 1 thành 2
...
```

**seed-seats-100.sql:**
```sql
-- Thay đổi biến ở đầu script
v_auditorium_id INT := 2; -- Đổi từ 1 thành 2
```

**seed-test-bookings.sql:**
```sql
-- Thay đổi các biến ở đầu script
v_customer_id INT := 1;    -- Customer ID của bạn
v_showtime_id INT := 42;   -- Showtime ID của bạn
```

### Xóa ghế cũ trước khi seed

Uncomment dòng DELETE trong mỗi script:

```sql
-- Bỏ comment dòng này
DELETE FROM seats WHERE auditoriumid = 1;
```

## 🧪 Testing

Sau khi run scripts, test các endpoints:

### 1. Test GET seat layout (không có booking)
```http
GET https://localhost:7238/api/auditoriums/1/seats
```

**Expected:**
- All 50 seats returned
- All seats have `isAvailable: true`

### 2. Test GET seat layout (có booking)
```http
GET https://localhost:7238/api/auditoriums/1/seats?showtimeId=1
```

**Expected:**
- All 50 seats returned
- Seats A1, A2, A3, D2, D3, D4, H1, H2 có `isAvailable: false`
- Seat C1 có `isAvailable: true` (cancelled booking không ảnh hưởng)
- Remaining seats có `isAvailable: true`

### 3. Test available seats count
```http
GET https://localhost:7238/api/showtimes/1/available-seats
```

**Expected:**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Available seats count retrieved successfully",
  "data": {
    "showtimeid": 1,
    "availableSeats": 42
  }
}
```

Calculation: 50 total - 8 booked (A1,A2,A3,D2,D3,D4,H1,H2) = 42

### 4. Test create booking
```http
POST https://localhost:7238/api/bookings/create
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "showtimeid": 1,
  "seatids": [4, 5]  // A4, A5 (available seats)
}
```

**Expected:** 201 Created

### 5. Test create booking with already booked seat
```http
POST https://localhost:7238/api/bookings/create
Authorization: Bearer YOUR_TOKEN
Content-Type: application/json

{
  "showtimeid": 1,
  "seatids": [1, 2]  // A1, A2 (already booked)
}
```

**Expected:** 400 Bad Request

## 📊 Database Structure

### Seats Table
```sql
CREATE TABLE seats (
    seatid SERIAL PRIMARY KEY,
    auditoriumid INT NOT NULL,
    row VARCHAR(2) NOT NULL,
    number INT NOT NULL,
    type VARCHAR(20),
    isavailable BOOLEAN DEFAULT true,
    UNIQUE(auditoriumid, row, number)
);
```

### Bookingseats Table
```sql
CREATE TABLE bookingseats (
    bookingseatid SERIAL PRIMARY KEY,
    bookingid INT NOT NULL,
    seatid INT NOT NULL,
    showtimeid INT NOT NULL,
    seatprice DECIMAL(10,2) NOT NULL
);
```

### Bookings Table
```sql
CREATE TABLE bookings (
    bookingid SERIAL PRIMARY KEY,
    customerid INT NOT NULL,
    showtimeid INT NOT NULL,
    bookingcode VARCHAR(50),
    bookingtime TIMESTAMP,
    totalamount DECIMAL(10,2),
    status VARCHAR(20)
);
```

## 🎯 Seat Types & Pricing Guide

| Type | Description | Suggested Price Multiplier |
|------|-------------|---------------------------|
| Standard | Regular seats | 1.0x |
| VIP | Premium seats with more space | 1.5x |
| Deluxe | Luxury seats with recline | 2.0x |
| Couple | Double-wide seats for couples | 1.3x |

## 🔍 Verification Queries

### Check total seats per auditorium
```sql
SELECT auditoriumid, COUNT(*) as total_seats
FROM seats
GROUP BY auditoriumid;
```

### Check seat distribution by type
```sql
SELECT 
    auditoriumid,
    "Type",
    COUNT(*) as count
FROM seats
GROUP BY auditoriumid, "Type"
ORDER BY auditoriumid, "Type";
```

### Check booked seats for a showtime
```sql
SELECT 
    s.seatid,
    CONCAT(s."Row", s."Number") as seat,
    s."Type",
    b.bookingcode,
    b.status
FROM seats s
JOIN bookingseats bs ON s.seatid = bs.seatid
JOIN bookings b ON bs.bookingid = b.bookingid
WHERE bs.showtimeid = 1
  AND b.status != 'cancelled'
ORDER BY s."Row", s."Number";
```

### Calculate available seats for showtime
```sql
SELECT 
    a.auditoriumid,
    a.seatscount as total_capacity,
    COUNT(bs.bookingseatid) as booked,
    a.seatscount - COUNT(bs.bookingseatid) as available
FROM auditoriums a
LEFT JOIN seats s ON a.auditoriumid = s.auditoriumid
LEFT JOIN bookingseats bs ON s.seatid = bs.seatid AND bs.showtimeid = 1
LEFT JOIN bookings b ON bs.bookingid = b.bookingid AND b.status != 'cancelled'
WHERE a.auditoriumid = 1
GROUP BY a.auditoriumid, a.seatscount;
```

## 🚨 Troubleshooting

### Error: column "row" of relation "seats" does not exist

**Cause:** PostgreSQL is case-sensitive for column names. Database schema uses capital letters: `"Row"`, `"Number"`, `"Type"`

**Solution:** Scripts use correct case: `"Row"`, `"Number"`, `"Type"` (with capital first letter)

### Error: "duplicate key value violates unique constraint"

**Cause:** Seats already exist in database

**Solution:** 
1. Delete existing seats first:
```sql
DELETE FROM bookingseats WHERE seatid IN (SELECT seatid FROM seats WHERE auditoriumid = 1);
DELETE FROM seats WHERE auditoriumid = 1;
```
2. Run seed script again

### Error: "foreign key constraint violation"

**Cause:** Referenced auditoriumid doesn't exist

**Solution:**
1. Check if auditorium exists:
```sql
SELECT * FROM auditoriums WHERE auditoriumid = 1;
```
2. Create auditorium if missing:
```sql
INSERT INTO auditoriums (cinemaid, name, seatscount)
VALUES (1, 'Phòng 1', 50);
```

### Seats count mismatch

**Problem:** `auditoriums.seatscount` doesn't match actual seat count

**Solution:** Update seatscount after seeding:
```sql
UPDATE auditoriums 
SET seatscount = (SELECT COUNT(*) FROM seats WHERE auditoriumid = auditoriums.auditoriumid)
WHERE auditoriumid = 1;
```

## 📝 Notes

- Scripts sử dụng PostgreSQL syntax
- Seat IDs được auto-generate bởi SERIAL
- `isavailable` column trong `seats` table là metadata, không dùng cho real-time booking
- Real-time availability được tính từ `bookingseats` table
- Cancelled bookings không ảnh hưởng tới availability
- Scripts có thể run nhiều lần (nếu bạn xóa data cũ trước)

## 🎬 Next Steps

Sau khi seed data:

1. ✅ Test Phase 2.2: GET /api/auditoriums/{id}/seats
2. ✅ Test Phase 2.3: POST /api/bookings/create
3. 🚀 Continue to Phase 3: Combo Selection
4. 💳 Implement Payment Flow
5. 🎫 Generate QR codes for tickets

## 📚 References

- [PostgreSQL INSERT Documentation](https://www.postgresql.org/docs/current/sql-insert.html)
- [Supabase SQL Editor](https://supabase.com/docs/guides/database/overview)
- Movie88 Booking Flow API Documentation: `tests/BookingFlow.http`
