# 🎬 Movie88 - Seed Data Documentation

## 📋 Overview

File này chứa SQL script để seed data test cho hệ thống Movie88, bao gồm:
- ✅ 3 Cinemas (rạp chiếu phim)
- ✅ 13 Auditoriums (phòng chiếu, 60-80 ghế/phòng)
- ✅ ~910 Seats (ghế ngồi)
- ✅ ~640 Showtimes (suất chiếu cho 10 ngày tới)

## 🏢 Cinemas (3 rạp tại TP.HCM)

| ID | Tên | Địa chỉ | Số điện thoại | Số phòng |
|----|-----|---------|---------------|----------|
| 1 | Movie 88 - Nguyễn Huệ | 1 Nguyễn Huệ, P. Bến Nghé, Q.1 | 028 1111 2222 | 4 |
| 2 | Movie 88 - Sư Vạn Hạnh | 10 Sư Vạn Hạnh, P. 12, Q.10 | 028 3333 4444 | 4 |
| 3 | Movie 88 - Võ Văn Ngân | 2 Võ Văn Ngân, P. Linh Chiểu, Thủ Đức | 028 5555 6666 | 5 |

## 🎭 Auditoriums (13 phòng chiếu)

### Cinema 1: Movie 88 - Nguyễn Huệ
- Phòng 1: 70 ghế (7 rows x 10 seats)
- Phòng 2: 60 ghế (6 rows x 10 seats)
- Phòng 3: 80 ghế (8 rows x 10 seats)
- Phòng 4: 70 ghế (7 rows x 10 seats)

### Cinema 2: Movie 88 - Sư Vạn Hạnh
- Phòng 1: 70 ghế (7 rows x 10 seats)
- Phòng 2: 60 ghế (6 rows x 10 seats)
- Phòng 3: 80 ghế (8 rows x 10 seats)
- Phòng 4: 70 ghế (7 rows x 10 seats)

### Cinema 3: Movie 88 - Võ Văn Ngân
- Phòng 1: 70 ghế (7 rows x 10 seats)
- Phòng 2: 60 ghế (6 rows x 10 seats)
- Phòng 3: 80 ghế (8 rows x 10 seats)
- Phòng 4: 80 ghế (8 rows x 10 seats)
- Phòng 5: 60 ghế (6 rows x 10 seats)

## 💺 Seat Layout Design

Mỗi phòng chiếu có layout chuẩn rạp chiếu phim Việt Nam (phòng nhỏ-vừa):

### Layout Structure (Example: 70 ghế - 7 rows x 10 seats)
```
Row A:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← Standard
Row B:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← Standard
Row C:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← Standard
Row D:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← VIP ⭐
Row E:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← Standard
Row F:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← Standard
Row G:  [1] [2] [3] [4] [5] [6] [7] [8] [9] [10]  ← Standard
```

### Seat Count Configurations
- **60 ghế**: 6 rows (A-F) × 10 seats, VIP = row D
- **70 ghế**: 7 rows (A-G) × 10 seats, VIP = row D
- **80 ghế**: 8 rows (A-H) × 10 seats, VIP = rows D, E

### Seat Types
- **Standard** (~85%): Hàng đầu và cuối - Ghế thường
- **VIP** (~15%): Hàng giữa - Vị trí view tốt nhất (giá vẫn = Standard theo flat pricing)

### Properties
- **Seat count range**: 60-80 ghế (chia hết cho 10)
- **All seats have `isavailable = true`** (không có ghế hỏng)
- **Unique constraint**: `(auditoriumid, "Row", "Number")`
- **Rows**: 6-8 hàng tùy kích thước phòng
- **Seats per row**: 10 ghế (cố định)

## 📅 Showtimes Strategy (10 ngày tới)

### Movie Popularity Tiers

#### 🔥 Hot Movies (6 suất/ngày)
- **MovieIDs**: 1, 2, 40, 41, 42
- **Showtimes/day**: 6 suất
- **Coverage**: 3 rạp (mỗi rạp 2 suất)
- **Total**: ~300 showtimes (5 movies × 6 shows × 10 days)

#### ⭐ Normal Movies (4 suất/ngày)
- **MovieIDs**: 3, 43, 44, 45, 46, 47, 48
- **Showtimes/day**: 4 suất
- **Coverage**: 2 rạp
- **Total**: ~280 showtimes (7 movies × 4 shows × 10 days)

#### 📽️ Less Popular Movies (2 suất/ngày)
- **MovieIDs**: 49, 50, 51
- **Showtimes/day**: 2 suất
- **Coverage**: 1 rạp
- **Total**: ~60 showtimes (3 movies × 2 shows × 10 days)

### Time Slots (UTC+7)
- 09:00 - Morning show
- 11:30 - Late morning
- 14:00 - Afternoon matinee
- 16:30 - Early evening
- 19:00 - Prime time ⭐
- 21:30 - Late night
- 23:00 - Midnight show

### Format Distribution
- **2D**: 70% (Price: 80,000đ)
- **3D**: 30% (Price: 120,000đ)

### Language Distribution
- **Phụ đề** (Subtitle): 80%
- **Lồng tiếng** (Dubbed): 20%

## 🚀 How to Run

### Option 1: PostgreSQL Command Line
```bash
psql -U postgres -d movie88db -f 00-SEED-CINEMAS-AUDITORIUMS-SEATS-SHOWTIMES.sql
```

### Option 2: pgAdmin
1. Open pgAdmin
2. Connect to `movie88db`
3. Open Query Tool
4. Load file `00-SEED-CINEMAS-AUDITORIUMS-SEATS-SHOWTIMES.sql`
5. Execute (F5)

### Option 3: DBeaver
1. Connect to database
2. Open SQL Editor
3. Load file
4. Execute

## 📊 Expected Results

After running the script:

```sql
-- Cinemas
SELECT COUNT(*) FROM cinemas;
-- Expected: 3

-- Auditoriums
SELECT COUNT(*) FROM auditoriums;
-- Expected: 13

-- Seats
SELECT COUNT(*) FROM seats;
-- Expected: ~2,000 (depends on auditorium sizes)

-- Showtimes (next 10 days)
SELECT COUNT(*) FROM showtimes 
WHERE starttime >= CURRENT_DATE 
  AND starttime < CURRENT_DATE + INTERVAL '10 days';
-- Expected: ~640 showtimes
```

## 🔍 Verification Queries

Script tự động chạy các queries kiểm tra:

### 1. Count seats per auditorium
```sql
SELECT 
    a.name AS auditorium,
    COUNT(s.seatid) AS total_seats,
    COUNT(CASE WHEN s.type = 'Standard' THEN 1 END) AS standard,
    COUNT(CASE WHEN s.type = 'VIP' THEN 1 END) AS vip
FROM auditoriums a
LEFT JOIN seats s ON a.auditoriumid = s.auditoriumid
GROUP BY a.auditoriumid, a.name;
```

### 2. Check showtimes per movie
```sql
SELECT 
    m.title,
    COUNT(st.showtimeid) AS total_showtimes
FROM movies m
LEFT JOIN showtimes st ON m.movieid = st.movieid
WHERE st.starttime >= CURRENT_DATE
GROUP BY m.movieid, m.title
ORDER BY total_showtimes DESC;
```

### 3. View today's showtimes
```sql
SELECT 
    c.name AS cinema,
    m.title AS movie,
    st.starttime,
    st.format,
    st.price
FROM showtimes st
JOIN auditoriums a ON st.auditoriumid = a.auditoriumid
JOIN cinemas c ON a.cinemaid = c.cinemaid
JOIN movies m ON st.movieid = m.movieid
WHERE DATE(st.starttime) = CURRENT_DATE
ORDER BY st.starttime;
```

## ⚠️ Important Notes

### 1. Prerequisites
- ✅ Movies table must have data for movieids: 1, 2, 3, 40-51
- ✅ Database timezone should be set to UTC+7
- ✅ Run this script BEFORE creating any bookings

### 2. Idempotent Design
- Script uses `ON CONFLICT DO UPDATE` for cinemas/auditoriums
- Safe to re-run multiple times
- Seats and showtimes will be added incrementally

### 3. Booking Compatibility
- All seats start with `isavailable = true`
- Seats become unavailable when booked (via `bookingseats` table)
- Field `isAvailableForShowtime` is computed dynamically in API

### 4. Pricing (Flat Pricing Model)
- All seats in same showtime have **same price** (from `showtime.price`)
- VIP seats only differ in UI display (yellow color)
- No price multiplier per seat type
- See: `/docs/frontend/SELECT-SEAT-GUIDE-ANDROID.md`

## 📦 Bookings Data

### 📄 Files:
- **`01-SEED-BOOKINGS.sql`** - Full script with documentation
- **`quick-seed-bookings.sql`** - Compact version

### 📊 Data Summary:
- **20 bookings** covering all statuses
- **57 booking seats** (2-4 seats per booking)
- **15 payments** (only for paid bookings)

### 🎭 Booking Status Distribution:
- **Completed** (4): Past showtimes, checked in
- **CheckedIn** (3): Today's showtimes, already checked in
- **Confirmed** (6): Upcoming showtimes, paid
- **Pending** (3): Future showtimes, not yet paid
- **Cancelled** (4): Cancelled for various reasons

### 👥 Customer Distribution:
- Customer IDs: **1, 2, 5, 6, 7, 11, 22, 28, 29**
- Payment Method: **4** (all bookings)

### 💰 Financial Summary:
- **Total Revenue**: ~4,400,000đ
- **Average Booking**: 220,000đ
- **Seat Price**: 80,000đ (flat pricing)

### Usage:
```bash
# Run full script
psql -U postgres -d movie88db -f 01-SEED-BOOKINGS.sql

# Or quick version
psql -U postgres -d movie88db -f quick-seed-bookings.sql
```

## 🔄 Update Strategy

### To add more showtimes:
```sql
-- Add showtimes for specific date
INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
VALUES 
    (1, 1, '2025-11-15 19:00:00', '2025-11-15 21:30:00', 80000, '2D', 'Phụ đề'),
    (2, 2, '2025-11-15 21:30:00', '2025-11-16 00:00:00', 120000, '3D', 'Phụ đề');
```

### To clear old showtimes:
```sql
-- Delete showtimes older than 30 days
DELETE FROM showtimes 
WHERE starttime < CURRENT_DATE - INTERVAL '30 days';
```

## 📞 Support

**Questions?** Contact:
- Backend Team: Trung
- Database: See `/docs/database/`
- API: See `/docs/API-Checklist-By-Screen.md`

---

**Last Updated**: 2025-11-06  
**Version**: 1.0  
**Status**: ✅ Ready for use
