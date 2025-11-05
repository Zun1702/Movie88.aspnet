# 🚀 Quick Start Guide - Seed Cinema Data

## 📝 Files Overview

### Cinema Data (Cinemas, Auditoriums, Seats, Showtimes)
1. **`00-DROP-CINEMA-DATA.sql`** - Xóa toàn bộ data liên quan (standalone)
2. **`00-SEED-CINEMAS-AUDITORIUMS-SEATS-SHOWTIMES.sql`** - Full script với comments (bao gồm DROP + SEED)
3. **`quick-seed.sql`** - Script ngắn gọn (bao gồm DROP + SEED)

### Bookings Data (Bookings, Booking Seats, Payments)
4. **`01-SEED-BOOKINGS.sql`** - Full script với 20 bookings
5. **`quick-seed-bookings.sql`** - Compact version

## ⚡ Quick Start (Recommended)

### 🎬 Step 1: Seed Cinema Data

#### Option 1: One-Click Seed (Fastest)
```bash
psql -U postgres -d movie88db -f quick-seed.sql
```

#### Option 2: Full Script với Comments
```bash
psql -U postgres -d movie88db -f 00-SEED-CINEMAS-AUDITORIUMS-SEATS-SHOWTIMES.sql
```

#### Option 3: Drop trước, Seed sau (2 bước)
```bash
# Step 1: Drop existing data
psql -U postgres -d movie88db -f 00-DROP-CINEMA-DATA.sql

# Step 2: Insert fresh data
psql -U postgres -d movie88db -f quick-seed.sql
```

### 🎫 Step 2: Seed Bookings Data

```bash
# Quick version (recommended)
psql -U postgres -d movie88db -f quick-seed-bookings.sql

# Or full version with documentation
psql -U postgres -d movie88db -f 01-SEED-BOOKINGS.sql
```

## ⚠️ Important Notes

### 1. Data Will Be Deleted
Các script này sẽ **XÓA** toàn bộ data của:
- ✅ `bookingseats` (ghế đã đặt)
- ✅ `bookingcombos` (combo đã đặt)
- ✅ `bookingpromotions` (khuyến mãi đã áp dụng)
- ✅ `payments` (thanh toán)
- ✅ `bookings` (đặt vé)
- ✅ `showtimes` (suất chiếu)
- ✅ `seats` (ghế)
- ✅ `auditoriums` (phòng chiếu)
- ✅ `cinemas` (rạp)

### 2. Prerequisites
- ✅ Movies table phải có data cho movieids: **1, 2, 3, 40-51**
- ✅ Database: `movie88db`
- ✅ User có quyền DELETE và INSERT

### 3. Deletion Order
Script xóa theo đúng thứ tự để **KHÔNG vi phạm khóa ngoại**:
```
bookingseats → bookingcombos → bookingpromotions → payments → bookings
     ↓
showtimes → seats → auditoriums → cinemas
```

### 4. Sequence Reset
Sau khi xóa, tất cả ID sequences sẽ được reset về **1**:
- `cinemaid` starts from 1
- `auditoriumid` starts from 1
- `seatid` starts from 1
- `showtimeid` starts from 1
- ...

## 🔍 Verification

### Check if data seeded successfully:
```sql
SELECT 
    (SELECT COUNT(*) FROM cinemas) AS cinemas,
    (SELECT COUNT(*) FROM auditoriums) AS auditoriums,
    (SELECT COUNT(*) FROM seats) AS seats,
    (SELECT COUNT(*) FROM showtimes 
     WHERE starttime >= CURRENT_DATE 
       AND starttime < CURRENT_DATE + INTERVAL '10 days') AS showtimes;
```

### Expected Results:
- Cinemas: **3**
- Auditoriums: **13**
- Seats: **~910** (60-80 ghế/phòng)
- Showtimes (10 days): **~640**

## 📊 What You'll Get

### 3 Cinemas
1. Movie 88 - Nguyễn Huệ (4 phòng)
2. Movie 88 - Sư Vạn Hạnh (4 phòng)
3. Movie 88 - Võ Văn Ngân (5 phòng)

### 13 Auditoriums (Small-Medium size)
- **60 seats**: 6 rows (A-F) × 10 seats
- **70 seats**: 7 rows (A-G) × 10 seats
- **80 seats**: 8 rows (A-H) × 10 seats

### Seats Layout
- **Standard** (~85%): Hàng đầu và cuối
- **VIP** (~15%): Hàng giữa (row D cho 60-70 ghế, rows D-E cho 80 ghế)
- All seats: `isavailable = true`

### Showtimes (10 days)
- **Hot movies** (1,2,40,41,42): 6 suất/ngày × 10 ngày = 300 suất
- **Normal movies** (3,43-48): 4 suất/ngày × 10 ngày = 280 suất
- **Less popular** (49,50,51): 2 suất/ngày × 10 ngày = 60 suất
- **Time slots**: 09:00, 11:30, 14:00, 16:30, 19:00, 21:30, 23:00
- **Formats**: 2D (70%), 3D (30%)
- **Prices**: 2D = 80,000đ, 3D = 120,000đ

## 🐛 Troubleshooting

### Error: "relation does not exist"
→ Bảng chưa được tạo. Chạy migrations trước.

### Error: "violates foreign key constraint"
→ Script đã xử lý thứ tự xóa đúng. Nếu vẫn lỗi, check xem có bảng nào khác tham chiếu không.

### Error: "movies with id X not found"
→ Bảng `movies` chưa có data cho movieids: 1,2,3,40-51.

### Sequences not reset properly
→ Chạy thủ công:
```sql
ALTER SEQUENCE cinemas_cinemaid_seq RESTART WITH 1;
ALTER SEQUENCE auditoriums_auditoriumid_seq RESTART WITH 1;
-- ... (xem trong script)
```

## 📞 Support

**Need help?**
- Backend Team: Trung
- See: `README.md` (full documentation)
- See: `00-SEED-CINEMAS-AUDITORIUMS-SEATS-SHOWTIMES.sql` (full script)

---

**Last Updated**: 2025-11-06  
**Version**: 1.0  
**Status**: ✅ Ready
