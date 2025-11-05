# 🔍 Entity Verification Report - Admin Docs

**Date**: 2025-01-XX  
**Status**: ❌ **CRITICAL ISSUES FOUND**

---

## 📋 Executive Summary

**Kết quả kiểm tra**: Admin/Staff docs có **nhiều sai lầm nghiêm trọng** về entity structure.

| Category | Status | Issues Found |
|----------|--------|--------------|
| ❌ **Non-existent Fields** | CRITICAL | 2 major fields don't exist |
| ⚠️ **Field Naming** | WARNING | JSON uses camelCase, DB uses lowercase |
| ✅ **Relationships** | CORRECT | Booking-Payment relationship exists |
| ⚠️ **Business Logic** | NEEDS REVIEW | Payment status is separate table |

---

## 🚨 CRITICAL ISSUES

### ❌ Issue #1: `paymentStatus` field KHÔNG TỒN TẠI

**Location**: `docs/admin-staff/01-Staff-Booking-Verification.md`

**Lines with errors**:
- Line 52: `Status: "Pending", PaymentStatus: "Pending"`
- Line 58: `PaymentStatus: "Completed"`
- Line 88: `paymentStatus = "Completed"`
- Line 187: `"paymentStatus": "Completed"`
- Line 243: `✅ paymentstatus (string) - **Completed**`

**Reality**:
```csharp
// ❌ Booking entity KHÔNG CÓ field "paymentstatus"
[Table("bookings")]
public partial class Booking
{
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("customerid")] public int Customerid { get; set; }
    [Column("showtimeid")] public int Showtimeid { get; set; }
    [Column("voucherid")] public int? Voucherid { get; set; }
    [Column("bookingcode")] public string? Bookingcode { get; set; }
    [Column("bookingtime")] public DateTime? Bookingtime { get; set; }
    [Column("totalamount")] public decimal? Totalamount { get; set; }
    [Column("status")] public string? Status { get; set; } // ← Chỉ có "status", không có "paymentstatus"
    
    // Navigation properties
    public virtual ICollection<Payment> Payments { get; set; } // ← Relationship 1-N
}

// ✅ Payment entity RIÊNG BIỆT
[Table("payments")]
public partial class Payment
{
    [Column("paymentid")] public int Paymentid { get; set; }
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("status")] public string? Status { get; set; } // ← Payment status ở đây
    [Column("amount")] public decimal Amount { get; set; }
    [Column("transactioncode")] public string? Transactioncode { get; set; }
    [Column("paymenttime")] public DateTime? Paymenttime { get; set; }
}
```

**Correct Approach**:
```csharp
// Docs nên mô tả như này:
// 1. Booking.Status: "Pending", "Confirmed", "Cancelled", "Completed", "Expired"
// 2. Payment.Status (separate table): "Pending", "Completed", "Failed"
// 3. Booking có ICollection<Payment> (1 booking có nhiều payment records)
```

---

### ❌ Issue #2: `checkedinStatus` / `checkedinstatus` field KHÔNG TỒN TẠI

**Location**: `docs/admin-staff/01-Staff-Booking-Verification.md`

**Lines with errors**:
- Line 191: `"checkinStatus": "NotCheckedIn"`
- Line 245: `✅ checkedinstatus (string) - NotCheckedIn, CheckedIn`
- Line 307: `"checkinStatus": "CheckedIn"`
- Line 329: `Update checkedinstatus = "CheckedIn"`
- Line 393: `Show checkedinstatus`

**Reality**:
```csharp
// ❌ Booking entity KHÔNG CÓ field check-in status
[Table("bookings")]
public partial class Booking
{
    // ... other fields ...
    [Column("status")] public string? Status { get; set; } // ← Chỉ có "status" duy nhất
    
    // ❌ KHÔNG CÓ: checkedinstatus
    // ❌ KHÔNG CÓ: checkedintime
    // ❌ KHÔNG CÓ: checkedinby
}
```

**Possible Solutions**:

**Option 1: Add new columns to database** (Recommended)
```sql
ALTER TABLE bookings 
ADD COLUMN checkedintime TIMESTAMP,
ADD COLUMN checkedinby INT REFERENCES users(userid);

-- Status field sẽ dùng để track: "Pending", "Confirmed", "CheckedIn", "Completed"
```

**Option 2: Use existing Status field**
```csharp
// Booking.Status enum values:
// - "Pending" (just created, awaiting payment)
// - "Confirmed" (payment completed, booking active)
// - "CheckedIn" (customer arrived at cinema)
// - "Completed" (showtime finished)
// - "Cancelled" (booking cancelled)
// - "Expired" (payment timeout or showtime passed)
```

---

## ⚠️ Field Naming Conventions

### JSON Response (camelCase) vs Database (lowercase)

**Docs show JSON responses with camelCase**:
```json
{
  "bookingId": 12345,
  "bookingCode": "BK20251104001",
  "paymentStatus": "Completed"  // ← This is DTO field, not entity field
}
```

**Entity uses lowercase**:
```csharp
[Column("bookingid")] public int Bookingid { get; set; }
[Column("bookingcode")] public string? Bookingcode { get; set; }
```

**This is CORRECT** ✅ - DTOs can have different casing than entities.

---

## ✅ Correct Entity Structures

### Booking Entity (ACTUAL)
```csharp
[Table("bookings")]
public partial class Booking
{
    [Key]
    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("customerid")]
    public int Customerid { get; set; }

    [Column("showtimeid")]
    public int Showtimeid { get; set; }

    [Column("voucherid")]
    public int? Voucherid { get; set; } // ✅ Nullable FK to Voucher

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
    public string? Status { get; set; } // ✅ "Pending", "Confirmed", "Cancelled", etc.

    // ❌ NO: paymentstatus field
    // ❌ NO: checkedinstatus field
    // ❌ NO: checkedintime field

    // Navigation properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Showtime Showtime { get; set; } = null!;
    public virtual Voucher? Voucher { get; set; }
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Bookingseat> Bookingseats { get; set; } = new List<Bookingseat>();
    public virtual ICollection<Bookingcombo> Bookingcombos { get; set; } = new List<Bookingcombo>();
    public virtual ICollection<Bookingpromotion> Bookingpromotions { get; set; } = new List<Bookingpromotion>();
}
```

### Payment Entity (ACTUAL)
```csharp
[Table("payments")]
public partial class Payment
{
    [Key]
    [Column("paymentid")]
    public int Paymentid { get; set; }

    [Column("bookingid")]
    public int Bookingid { get; set; } // ✅ FK to Booking

    [Column("customerid")]
    public int Customerid { get; set; }

    [Column("methodid")]
    public int Methodid { get; set; } // ✅ FK to Paymentmethod

    [Column("amount")]
    [Precision(10, 2)]
    public decimal Amount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; } // ✅ "Pending", "Completed", "Failed"

    [Column("transactioncode")]
    [StringLength(255)]
    public string? Transactioncode { get; set; } // ✅ VNPay transaction code

    [Column("paymenttime", TypeName = "timestamp without time zone")]
    public DateTime? Paymenttime { get; set; }

    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual Paymentmethod Method { get; set; } = null!;
}
```

### Voucher Entity (ACTUAL) ✅
```csharp
[Table("vouchers")]
public partial class Voucher
{
    [Key]
    [Column("voucherid")]
    public int Voucherid { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!; // ✅ Unique, not nullable

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("discounttype")]
    [StringLength(20)]
    public string? Discounttype { get; set; } // ��� "percentage", "fixed"

    [Column("discountvalue")]
    [Precision(10, 2)]
    public decimal? Discountvalue { get; set; }

    [Column("minpurchaseamount")]
    [Precision(10, 2)]
    public decimal? Minpurchaseamount { get; set; }

    [Column("expirydate")]
    public DateOnly? Expirydate { get; set; } // ✅ DateOnly, not DateTime

    [Column("usagelimit")]
    public int? Usagelimit { get; set; }

    [Column("usedcount")]
    public int? Usedcount { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    // Navigation property
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
```

---

## 📝 Recommendations

### 1. Fix Documentation (HIGH PRIORITY)

**File**: `docs/admin-staff/01-Staff-Booking-Verification.md`

**Changes needed**:

1. **Remove all references to `paymentStatus` field in Booking entity**
   - Replace with: "Check `Payment.Status` via `Booking.Payments` collection"
   
2. **Remove all references to `checkedinStatus` field**
   - Option A: Add new DB columns (checkedintime, checkedinby)
   - Option B: Use existing `Booking.Status` field with value "CheckedIn"
   
3. **Update "Related Entities" sections**:
   ```markdown
   ### Related Entities
   **Booking** (bookings table):
   - ✅ `bookingid` (int, PK)
   - ✅ `bookingcode` (string?, max 20) - Generated after payment
   - ✅ `customerid` (int, FK)
   - ✅ `showtimeid` (int, FK)
   - ✅ `voucherid` (int?, nullable, FK)
   - ✅ `totalamount` (decimal(10,2)?)
   - ✅ `status` (string?, max 50) - "Pending", "Confirmed", "CheckedIn", "Cancelled", "Completed", "Expired"
   - ✅ `bookingtime` (timestamp without time zone?)
   - ❌ NO `paymentstatus` field (use Payment.Status instead)
   - ❌ NO `checkedinstatus` field (use Booking.Status = "CheckedIn" or add new column)
   
   **Payment** (payments table) - SEPARATE TABLE:
   - ✅ `paymentid` (int, PK)
   - ✅ `bookingid` (int, FK → bookings)
   - ✅ `status` (string?, max 50) - "Pending", "Completed", "Failed"
   - ✅ `transactioncode` (string?, max 255)
   - ✅ `amount` (decimal(10,2))
   - ✅ `paymenttime` (timestamp without time zone?)
   - ✅ Relationship: Booking.Payments (1-N collection)
   ```

### 2. Update Business Logic (MEDIUM PRIORITY)

**Current docs assume**:
```csharp
// ❌ WRONG - Field doesn't exist
if (booking.PaymentStatus == "Completed")
    AllowCheckIn();
```

**Should be**:
```csharp
// ✅ CORRECT - Query Payment table
var hasCompletedPayment = booking.Payments.Any(p => p.Status == "Completed");
if (hasCompletedPayment)
    AllowCheckIn();
```

### 3. Database Migration (OPTIONAL - If want check-in tracking)

**Create migration**:
```sql
ALTER TABLE bookings 
ADD COLUMN checkedintime TIMESTAMP WITHOUT TIME ZONE,
ADD COLUMN checkedinby INT REFERENCES users(userid);

COMMENT ON COLUMN bookings.checkedintime IS 'When customer checked in at cinema counter';
COMMENT ON COLUMN bookings.checkedinby IS 'Staff user ID who performed check-in';
```

**Then update entity**:
```csharp
[Table("bookings")]
public partial class Booking
{
    // ... existing fields ...
    
    [Column("checkedintime", TypeName = "timestamp without time zone")]
    public DateTime? Checkedintime { get; set; }
    
    [Column("checkedinby")]
    public int? Checkedinby { get; set; }
    
    [ForeignKey("Checkedinby")]
    public virtual User? CheckedInByUser { get; set; }
}
```

---

## 🎯 Action Items

### Immediate (Must do before implementation)
- [ ] Update all docs to remove `paymentStatus` from Booking entity
- [ ] Update all docs to remove `checkedinStatus` from Booking entity
- [ ] Document correct Payment relationship (1 booking → N payments)
- [ ] Clarify Booking.Status enum values

### Before Staff Check-in Implementation (Choose one)
- [ ] **Option A**: Add DB columns (checkedintime, checkedinby) + migration
- [ ] **Option B**: Use Booking.Status = "CheckedIn" (simpler, no DB change)

### Quality Assurance
- [ ] Review all endpoint examples in docs
- [ ] Update DTO definitions to match actual entities
- [ ] Add notes about Payment being separate table
- [ ] Test all booking-related queries with correct relationships

---

## 📊 Entity Relationship Summary (CORRECTED)

```
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│   Booking   │────────▶│   Payment   │         │   Voucher   │
│             │ 1     N │             │         │             │
│ bookingid   │         │ paymentid   │         │ voucherid   │
│ bookingcode │         │ bookingid ──┼─────┐   │ code        │
│ customerid  │         │ status ─────┼─┐   │   │ discounttype│
│ showtimeid  │         │ amount      │ │   └───│◀── voucherid│
│ voucherid ──┼─────────┼─────────────┘ │       │             │
│ status      │         │ methodid      │       └─────────────┘
│ totalamount │         │ transaction   │
└─────────────┘         └─────────────┘
       │                                          
       │ ❌ NO paymentstatus field here          
       │ ❌ NO checkedinstatus field here        
       │ ✅ Use Payment.Status instead           
       │ ✅ Use Booking.Status for check-in      
       └─────────────────────────────────────────
```

---

## ✅ Verified Correct Information

### These are CORRECT in docs:
- ✅ Booking.Voucherid exists (nullable FK)
- ✅ Booking.Bookingcode exists (generated after payment)
- ✅ Voucher entity structure is correct
- ✅ Payment entity exists as separate table
- ✅ Booking → Payment relationship is 1:N
- ✅ JSON camelCase vs DB lowercase is standard practice

### These need CORRECTION in docs:
- ❌ `Booking.paymentStatus` field (doesn't exist - use Payment.Status)
- ❌ `Booking.checkedinStatus` field (doesn't exist - use Booking.Status or add column)
- ❌ `Booking.paymentdate` field references (should be Payment.paymenttime)

---

**Report generated**: 2025-01-XX  
**Next steps**: Update all admin-staff documentation before Việt implements Staff endpoints.
