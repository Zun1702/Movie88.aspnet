# ⚠️ QUAN TRỌNG - ĐỌC TRƯỚC KHI IMPLEMENT

**Date Updated**: November 5, 2025  
**For**: Việt (Staff & Admin endpoints implementation)

---

## 🚨 CRITICAL: Entity Structure Corrections

**TL;DR**: Admin docs đã được **cập nhật toàn bộ** để phản ánh đúng entity structure hiện tại. Các field `paymentStatus` và `checkedinStatus` **KHÔNG TỒN TẠI** trong Booking entity.

---

## 📋 What Changed?

### ❌ REMOVED (Don't use these)

**These fields DO NOT exist in Booking entity:**

1. **`Booking.paymentStatus`** ❌
   - Docs cũ claim: Booking có field `paymentstatus`
   - **Thực tế**: Không tồn tại!
   - **Replacement**: Use `Payment.Status` (separate table)

2. **`Booking.checkedinStatus`** ❌
   - Docs cũ claim: Booking có field `checkedinstatus`
   - **Thực tế**: Không tồn tại!
   - **Replacement**: Use `Booking.Status = "CheckedIn"`

3. **`Booking.checkedinTime` và `Booking.checkedinBy`** ✅ **NOW AVAILABLE**
   - Docs cũ claim: Booking không có fields này
   - **Update**: ✅ **ĐÃ THÊM VÀO DATABASE** (Migration: 2025-11-05)
   - **New fields**:
     - `checkedintime` (timestamp) - Thời gian check-in
     - `checkedinby` (int, FK → User) - Staff user ID
   - **Navigation**: `CheckedInByUser` → User entity

---

## ✅ CORRECT Entity Structures

### Booking Entity (ACTUAL in Database)

```csharp
[Table("bookings")]
public partial class Booking
{
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("customerid")] public int Customerid { get; set; }
    [Column("showtimeid")] public int Showtimeid { get; set; }
    [Column("voucherid")] public int? Voucherid { get; set; } // ✅ nullable
    
    [Column("bookingcode")]
    [StringLength(20)]
    public string? Bookingcode { get; set; } // ✅ Generated after payment
    
    [Column("bookingtime", TypeName = "timestamp without time zone")]
    public DateTime? Bookingtime { get; set; }
    
    [Column("totalamount")]
    [Precision(10, 2)]
    public decimal? Totalamount { get; set; }
    
    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; } // ✅ Use this for check-in status
    
    [Column("checkedintime", TypeName = "timestamp without time zone")]
    public DateTime? Checkedintime { get; set; } // ✅ NEW: When customer checked in
    
    [Column("checkedinby")]
    public int? Checkedinby { get; set; } // ✅ NEW: Staff user ID who performed check-in
    
    // ❌ NO: paymentstatus
    // ❌ NO: checkedinstatus (use Status field instead)
    
    // Navigation properties
    public virtual ICollection<Payment> Payments { get; set; } // ✅ Check payment via this
    
    [ForeignKey("Checkedinby")]
    [InverseProperty("BookingsCheckedInBy")]
    public virtual User? CheckedInByUser { get; set; } // ✅ NEW: Staff who checked in
}
```

### Payment Entity (SEPARATE TABLE)

```csharp
[Table("payments")]
public partial class Payment
{
    [Column("paymentid")] public int Paymentid { get; set; }
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("customerid")] public int Customerid { get; set; }
    [Column("methodid")] public int Methodid { get; set; }
    
    [Column("amount")]
    [Precision(10, 2)]
    public decimal Amount { get; set; }
    
    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; } // ✅ "Pending", "Completed", "Failed"
    
    [Column("transactioncode")]
    [StringLength(255)]
    public string? Transactioncode { get; set; } // ✅ VNPay code
    
    [Column("paymenttime", TypeName = "timestamp without time zone")]
    public DateTime? Paymenttime { get; set; }
    
    // Navigation
    public virtual Booking Booking { get; set; } = null!;
}
```

---

## 🎯 How to Implement Correctly

### 1. Check Payment Status (CORRECT Way)

**❌ WRONG (docs cũ):**
```csharp
if (booking.PaymentStatus == "Completed") // ← Field doesn't exist!
    AllowCheckIn();
```

**✅ CORRECT (updated docs):**
```csharp
// Option 1: Check via navigation property
var hasCompletedPayment = booking.Payments.Any(p => p.Status == "Completed");
if (hasCompletedPayment)
    AllowCheckIn();

// Option 2: Explicit join query
var booking = await _context.Bookings
    .Include(b => b.Payments)
    .FirstOrDefaultAsync(b => b.Bookingcode == bookingCode);

if (booking.Payments.Any(p => p.Status == "Completed"))
    AllowCheckIn();
```

### 2. Check-in Status (CORRECT Way)

**❌ WRONG (docs cũ):**
```csharp
booking.CheckedinStatus = "CheckedIn"; // ← Field doesn't exist!
booking.CheckedinTime = DateTime.Now;
```

**✅ CORRECT (updated docs):**
```csharp
// ✅ NEW: Use database fields for check-in tracking (Added 2025-11-05)
booking.Status = nameof(BookingStatus.CheckedIn); // "CheckedIn"
booking.Checkedintime = DateTime.UtcNow; // ✅ NEW FIELD
booking.Checkedinby = currentStaffUserId; // ✅ NEW FIELD - from JWT token
await _context.SaveChangesAsync();

// Include in response DTO
var dto = new BookingVerifyDTO
{
    BookingId = booking.Bookingid,
    Status = booking.Status,
    CheckedInAt = booking.Checkedintime, // ✅ From DB now
    CheckedInBy = new StaffInfoDTO
    {
        UserId = booking.Checkedinby,
        StaffName = booking.CheckedInByUser?.Fullname // ✅ Via navigation
    }
};
```

**Migration Already Applied** ✅:
- Migration script: `docs/migrations/add-checkin-tracking.sql`
- Columns added: `checkedintime`, `checkedinby`
- Foreign key: `checkedinby` → `User.userid`
- Database ready for check-in tracking!

### 3. Booking Status Enum Values (CORRECT)

Use `BookingStatus` enum consistently:

```csharp
public enum BookingStatus
{
    Pending = 0,      // Just created, awaiting payment
    Confirmed = 1,    // Payment completed, booking active
    Cancelled = 2,    // Booking cancelled
    CheckedIn = 3,    // Customer checked in at cinema
    Completed = 4,    // Showtime finished
    Expired = 5       // Payment timeout or showtime passed
}

// Usage in code
booking.Status = nameof(BookingStatus.Confirmed);
booking.Status = nameof(BookingStatus.CheckedIn);
```

---

## 📊 Correct Relationships

### Booking → Payment (1:N) & Booking → User (Check-in Tracking)

```
┌──────────────┐         ┌──────────────┐
│   Booking    │────────▶│   Payment    │
│              │ 1     N │              │
│ bookingid    │         │ paymentid    │
│ bookingcode  │         │ bookingid    │
│ status       │         │ status ──────┼─┐ "Completed"
│ totalamount  │         │ amount       │ │
│              │         └──────────────┘ │
│ ✅ NEW:      │                          │
│ checkedintime│         ┌──────────────┐ │
│ checkedinby ─┼────────▶│     User     │ │
│              │    N:1  │              │ │
└──────────────┘         │ userid       │ │
       │                 │ fullname     │ │
       │                 │ roleid       │ │
       │                 └──────────────┘ │
       │ ❌ NO paymentstatus here         │
       │ ✅ Check via Payments ───────────┘
       │ ✅ Check-in tracked via checkedintime, checkedinby
       └──────────────────────────────────
```

---

## 🔍 Verify Implementation Checklist

Before you start coding, check these:

### For GET /api/bookings/verify/{bookingCode}

- [ ] Query includes `.Include(b => b.Payments)`
- [ ] Check `booking.Payments.Any(p => p.Status == "Completed")`
- [ ] Return `Payment.Status` in response DTO (not `booking.paymentStatus`)
- [ ] Check `booking.Status` for check-in eligibility
- [ ] Use `nameof(BookingStatus.Confirmed)` for comparisons

### For PUT /api/bookings/{id}/check-in

- [ ] Update `booking.Status = nameof(BookingStatus.CheckedIn)`
- [ ] ✅ **NEW**: Set `booking.Checkedintime = DateTime.UtcNow`
- [ ] ✅ **NEW**: Set `booking.Checkedinby = currentStaffUserId` (from JWT)
- [ ] Include `.Include(b => b.CheckedInByUser)` to load staff details
- [ ] Validate `booking.Status != "CheckedIn"` (prevent double check-in)
- [ ] Verify payment completed via `Payments` collection

### For GET /api/bookings/today

- [ ] Filter by `bookingtime` date (not `bookingdate` - doesn't exist)
- [ ] Join with `Payments` to show payment status
- [ ] ✅ **NEW**: Include `checkedintime` and `checkedinby` in response
- [ ] ✅ **NEW**: Load `.Include(b => b.CheckedInByUser)` for staff name
- [ ] Calculate `canCheckIn` flag based on `checkedintime == null`
- [ ] Use `booking.Status` for filter (not `checkedinstatus`)

---

## 📝 Updated Documentation Files

**Already updated** ✅:
- `01-Staff-Booking-Verification.md` - All references corrected
- `ENTITY_VERIFICATION_REPORT.md` - Detailed analysis report

**Need to check**:
- `02-Admin-Overview.md` - Review if has similar issues

---

## 🚀 Next Steps for Việt

### Immediate Actions:
1. ✅ Read this file completely
2. ✅ Read `ENTITY_VERIFICATION_REPORT.md` for full details
3. ✅ Review updated `01-Staff-Booking-Verification.md`
4. ⏳ Open `Movie88.Infrastructure/Entities/Booking.cs` to see actual structure
5. ⏳ Open `Movie88.Infrastructure/Entities/Payment.cs` to see actual structure
6. ⏳ Review `Movie88.Domain/Enums/BookingStatus.cs` for enum values

### Before Coding:
- [ ] Understand Booking → Payment relationship (1:N)
- [ ] Know how to query payment status via `Payments` collection
- [ ] Decide: Use `Booking.Status` for check-in or add new columns?
- [ ] Use `nameof(BookingStatus.X)` instead of hardcoded strings

### During Implementation:
- [ ] Always `.Include(b => b.Payments)` when querying bookings
- [ ] Never try to access `booking.PaymentStatus` (doesn't exist)
- [ ] Never try to access `booking.CheckedinStatus` (doesn't exist)
- [ ] Use `booking.Status` for all status tracking
- [ ] Check payment via `booking.Payments.Any(p => p.Status == "Completed")`

---

## 💡 Common Mistakes to Avoid

### ❌ Mistake #1: Accessing non-existent fields
```csharp
// ❌ WRONG - Compile error!
if (booking.PaymentStatus == "Completed")
if (booking.CheckedinStatus == "CheckedIn")
```

### ❌ Mistake #2: Hardcoded status strings
```csharp
// ❌ WRONG - Typo-prone
booking.Status = "Confirmed";
booking.Status = "checkedin"; // ← lowercase typo
```

### ❌ Mistake #3: Forgetting to include Payments
```csharp
// ❌ WRONG - Payments collection will be empty
var booking = await _context.Bookings
    .FirstOrDefaultAsync(b => b.Bookingcode == code);
var paymentStatus = booking.Payments.FirstOrDefault()?.Status; // ← null!
```

### ✅ Correct Implementations

```csharp
// ✅ CORRECT: Include navigation property
var booking = await _context.Bookings
    .Include(b => b.Payments)
    .Include(b => b.Customer)
    .Include(b => b.Showtime)
        .ThenInclude(s => s.Movie)
    .FirstOrDefaultAsync(b => b.Bookingcode == code);

// ✅ CORRECT: Check payment status
var hasPayment = booking.Payments.Any(p => p.Status == "Completed");

// ✅ CORRECT: Use enum for status
booking.Status = nameof(BookingStatus.CheckedIn);

// ✅ CORRECT: Type-safe comparison
if (booking.Status == nameof(BookingStatus.Confirmed))
    AllowCheckIn();
```

---

## 📞 Questions?

If you have any questions about:
- Entity relationships
- Why fields don't exist
- How to implement correctly
- Need database migration for check-in columns

**Ask before coding!** Better to clarify now than refactor later.

---

**Summary**: 
- ❌ `Booking.paymentStatus` doesn't exist → Use `Payment.Status` via `Payments` collection
- ❌ `Booking.checkedinStatus` doesn't exist → Use `Booking.Status = "CheckedIn"`
- ✅ All docs updated to reflect correct entity structure
- ✅ Use `nameof(BookingStatus.X)` for type safety
- ✅ Always `.Include(b => b.Payments)` when checking payment status

**Happy coding!** 🚀
