# 🎫 Bookings Seed Data Documentation

## 📋 Overview

This directory contains SQL scripts to seed test booking data for Movie88:
- ✅ **20 Bookings** with all possible statuses
- ✅ **57 Booking Seats** (2-4 seats per booking)
- ✅ **15 Payments** (only for paid bookings)

## 📊 Booking Distribution

### By Status (5 types)
| Status | Count | Description |
|--------|-------|-------------|
| **Completed** | 4 | Past showtimes, customer checked in and watched |
| **CheckedIn** | 3 | Today's showtimes, customer arrived and checked in |
| **Confirmed** | 6 | Upcoming showtimes, paid but not checked in yet |
| **Pending** | 3 | Future showtimes, not yet paid |
| **Cancelled** | 4 | Cancelled bookings (some refunded) |

### By Customer
Customers used: **1, 2, 5, 6, 7, 11, 22, 28, 29**

Each customer has 1-3 bookings with different statuses.

## 💰 Financial Summary

| Metric | Value |
|--------|-------|
| Total Bookings | 20 |
| Total Revenue | 4,400,000đ |
| Average Booking | 220,000đ |
| Total Seats Booked | 57 seats |
| Seat Price | 80,000đ (flat) |

### Revenue by Status
- **Completed**: 960,000đ (4 bookings paid)
- **CheckedIn**: 560,000đ (3 bookings paid)
- **Confirmed**: 1,600,000đ (6 bookings paid)
- **Pending**: 720,000đ (3 bookings NOT paid)
- **Cancelled**: 560,000đ (some refunded)

## 🎭 Booking Details

### Completed Bookings (Past)
```
BK001: Customer 1, 2 seats, 160,000đ ✅ Paid ✅ Watched (3 days ago)
BK002: Customer 2, 3 seats, 240,000đ ✅ Paid ✅ Watched (3 days ago)
BK003: Customer 5, 3 seats, 240,000đ ✅ Paid ✅ Watched (2 days ago)
BK004: Customer 6, 4 seats, 320,000đ ✅ Paid ✅ Watched (2 days ago)
```

### CheckedIn Bookings (Today)
```
BK005: Customer 7, 2 seats, 160,000đ ✅ Paid ✅ Checked in (2 hours ago)
BK006: Customer 11, 3 seats, 240,000đ ✅ Paid ✅ Checked in (3 hours ago)
BK007: Customer 22, 2 seats, 160,000đ ✅ Paid ✅ Checked in (1 hour ago)
```

### Confirmed Bookings (Upcoming)
```
BK008: Customer 28, 3 seats, 240,000đ ✅ Paid ⏳ Not checked in (tomorrow)
BK009: Customer 29, 4 seats, 320,000đ ✅ Paid ⏳ Not checked in (tomorrow)
BK010: Customer 1, 2 seats, 160,000đ ✅ Paid ⏳ Not checked in (12h ahead)
BK011: Customer 2, 3 seats, 240,000đ ✅ Paid ⏳ Not checked in (6h ahead)
BK012: Customer 5, 3 seats, 240,000đ ✅ Paid ⏳ Not checked in (3h ahead)
BK013: Customer 6, 2 seats, 160,000đ ✅ Paid ⏳ Not checked in (now)
```

### Pending Bookings (Not Paid)
```
BK014: Customer 7, 3 seats, 240,000đ ❌ Not paid (just booked)
BK015: Customer 11, 4 seats, 320,000đ ❌ Not paid (just booked)
BK016: Customer 22, 2 seats, 160,000đ ❌ Not paid (30 min ago)
```

### Cancelled Bookings
```
BK017: Customer 28, 3 seats, 240,000đ 🔄 Refunded (cancelled 2 days ago)
BK018: Customer 29, 2 seats, 160,000đ 🔄 Refunded (cancelled 1 day ago)
BK019: Customer 1, 3 seats, 240,000đ ❌ No payment (cancelled 5h ago)
BK020: Customer 2, 4 seats, 320,000đ ❌ No payment (cancelled 2h ago)
```

## 🔗 Relationships

### Bookings → Booking Seats (1:N)
- Each booking has **2-4 seats**
- `bookingseats.bookingid` → `bookings.bookingid`
- `bookingseats.showtimeid` → `showtimes.showtimeid`
- `bookingseats.seatid` → `seats.seatid`

### Bookings → Payments (1:1 or 1:0)
- **Paid bookings** have 1 payment record
- **Unpaid bookings** (Pending, some Cancelled) have no payment
- Payment statuses:
  * **Success**: 13 payments (Completed + CheckedIn + Confirmed)
  * **Refunded**: 2 payments (some Cancelled bookings)

## 📅 Timeline Pattern

```
3 days ago:  BK001 ✅, BK002 ✅ (Completed)
2 days ago:  BK003 ✅, BK004 ✅ (Completed), BK017 ❌ (Cancelled)
1 day ago:   BK008 💵, BK009 💵 (Confirmed), BK018 ❌ (Cancelled)
12 hours:    BK010 💵 (Confirmed)
6 hours:     BK011 💵 (Confirmed)
5 hours:     BK019 ❌ (Cancelled)
3 hours:     BK012 💵 (Confirmed), BK006 ✅ (CheckedIn)
2 hours:     BK005 ✅ (CheckedIn), BK020 ❌ (Cancelled)
1 hour:      BK007 ✅ (CheckedIn)
30 min:      BK016 ⏳ (Pending)
Now:         BK013 💵 (Confirmed), BK014 ⏳, BK015 ⏳ (Pending)
```

Legend:
- ✅ Checked in
- 💵 Confirmed (paid)
- ⏳ Pending (not paid)
- ❌ Cancelled

## 🚀 Usage

### Run Full Script
```bash
psql -U postgres -d movie88db -f 01-SEED-BOOKINGS.sql
```

### Run Quick Version
```bash
psql -U postgres -d movie88db -f quick-seed-bookings.sql
```

### Verify Results
```sql
-- Check booking status distribution
SELECT status, COUNT(*) 
FROM bookings 
WHERE bookingid BETWEEN 1 AND 20 
GROUP BY status;

-- Check total revenue
SELECT SUM(totalamount) AS total_revenue
FROM bookings 
WHERE bookingid BETWEEN 1 AND 20;

-- Check payment summary
SELECT p.status, COUNT(*), SUM(p.amount)
FROM payments p
WHERE p.bookingid BETWEEN 1 AND 20
GROUP BY p.status;
```

## ⚠️ Prerequisites

Before running booking seeds, ensure:
1. ✅ **Cinemas data** seeded (run `quick-seed.sql` first)
2. ✅ **Showtimes** exist with IDs: 1,2,5,8,15,18,20,25,28,32,35,40,45,50,55,60,65,70,75,80
3. ✅ **Customers** exist with IDs: 1,2,5,6,7,11,22,28,29
4. ✅ **Payment method** with ID: 4

## 🔄 Update Sequences

After running, sequences should be updated automatically:
```sql
SELECT setval('bookings_bookingid_seq', (SELECT MAX(bookingid) FROM bookings));
SELECT setval('bookingseats_bookingseatid_seq', (SELECT MAX(bookingseatid) FROM bookingseats));
SELECT setval('payments_paymentid_seq', (SELECT MAX(paymentid) FROM payments));
```

## 📞 Support

**Questions?**
- Backend: Trung
- See: `README.md` for full documentation

---

**Last Updated**: 2025-11-06  
**Version**: 1.0  
**Status**: ✅ Ready
