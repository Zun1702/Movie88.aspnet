# 📋 Booking Code & Status Implementation Guide

**Date**: November 3, 2025  
**Status**: ✅ Database Migration Completed

---

## 🎯 Overview

Đã thêm 2 tính năng quan trọng cho Booking:
1. **BookingCode** - Mã đặt vé unique để QR scan và check-in tại rạp
2. **BookingStatus Enum** - Standardize booking status values

---

## ✅ Completed Steps

### 1. Booking Status Enum

**File**: `Movie88.Domain/Enums/BookingStatus.cs`

```csharp
public enum BookingStatus
{
    Pending = 0,      // Chờ thanh toán
    Confirmed = 1,    // Đã thanh toán
    Cancelled = 2,    // Đã hủy
    CheckedIn = 3,    // Đã check-in tại rạp
    Completed = 4,    // Đã xem phim xong
    Expired = 5       // Hết hạn (chưa thanh toán đúng hạn)
}
```

### 2. Entity Update

**File**: `Movie88.Infrastructure/Entities/Booking.cs`

```csharp
[Column("bookingcode")]
[StringLength(20)]
public string Bookingcode { get; set; } = null!;
```

### 3. Entity Configuration

**File**: `Movie88.Infrastructure/EntitiesConfiguration/BookingConfiguration.cs`

```csharp
// Unique index for booking code
builder.HasIndex(b => b.Bookingcode)
    .IsUnique()
    .HasDatabaseName("IX_Bookings_BookingCode");
```

**Registered in**: `AppDbContext.OnModelCreating()`

### 4. Booking Code Generator Service

**File**: `Movie88.Application/Services/BookingCodeGenerator.cs`

```csharp
public interface IBookingCodeGenerator
{
    string GenerateBookingCode(DateTime bookingTime);
}

// Format: BK-20251103-0001
```

**Thread-safe** với counter reset mỗi ngày mới.

### 5. SQL Migration Script

**File**: `Movie88.Infrastructure/Migrations/Scripts/001_AddBookingCode.sql`

Includes:
- Add `bookingcode` column
- Create unique index
- Generate codes for existing data
- Set NOT NULL constraint

---

## ⚠️ Pending: Database Migration

### Option 1: Run SQL on Supabase Dashboard (RECOMMENDED)

1. Mở Supabase SQL Editor
2. Copy SQL từ file `001_AddBookingCode.sql`
3. Execute:

```sql
-- Add column (nullable first for safety)
ALTER TABLE public.bookings
ADD COLUMN IF NOT EXISTS bookingcode VARCHAR(20);

-- Create unique index
CREATE UNIQUE INDEX IF NOT EXISTS IX_Bookings_BookingCode 
ON public.bookings(bookingcode);

-- Generate codes for existing records (if any)
DO $$
DECLARE
    booking_record RECORD;
    new_code VARCHAR(20);
    counter INT := 1;
BEGIN
    FOR booking_record IN 
        SELECT bookingid, bookingtime 
        FROM public.bookings 
        WHERE bookingcode IS NULL
        ORDER BY bookingid
    LOOP
        new_code := 'BK-' || 
                    TO_CHAR(COALESCE(booking_record.bookingtime, NOW()), 'YYYYMMDD') || 
                    '-' || 
                    LPAD(counter::TEXT, 4, '0');
        
        UPDATE public.bookings 
        SET bookingcode = new_code 
        WHERE bookingid = booking_record.bookingid;
        
        counter := counter + 1;
    END LOOP;
END $$;

-- Make NOT NULL after data populated
ALTER TABLE public.bookings
ALTER COLUMN bookingcode SET NOT NULL;
```

### Option 2: Use PowerShell Script

```powershell
cd Movie88.Infrastructure/Migrations/Scripts
.\RunMigration.ps1
```

---

## 🔧 Usage in Application Code

### When Creating New Booking

```csharp
// In BookingService.CreateBookingAsync()

// 1. Inject IBookingCodeGenerator
private readonly IBookingCodeGenerator _codeGenerator;

// 2. Generate code when creating booking
var bookingTime = DateTime.UtcNow;
var booking = new Booking
{
    Customerid = customer.Customerid,
    Showtimeid = request.Showtimeid,
    Bookingcode = _codeGenerator.GenerateBookingCode(bookingTime),
    Bookingtime = DateTime.SpecifyKind(bookingTime, DateTimeKind.Unspecified),
    Status = BookingStatus.Pending.ToString(),  // or store as int
    Totalamount = totalAmount
};
```

### Register Service in DI

**File**: `Movie88.Application/Configuration/ServiceExtensions.cs`

```csharp
services.AddSingleton<IBookingCodeGenerator, BookingCodeGenerator>();
```

### Using BookingStatus Enum

```csharp
// Option 1: Store as string
booking.Status = BookingStatus.Confirmed.ToString();

// Option 2: Store as int (need to change column type to integer)
booking.Status = ((int)BookingStatus.Confirmed).ToString();

// Parsing
if (Enum.TryParse<BookingStatus>(booking.Status, out var status))
{
    switch (status)
    {
        case BookingStatus.Pending:
            // Handle pending
            break;
        case BookingStatus.Confirmed:
            // Handle confirmed
            break;
        // ...
    }
}
```

---

## 📱 QR Code Generation

### Generate QR Code for Booking

```csharp
// Use library: QRCoder (Install-Package QRCoder)

public string GenerateQRCode(string bookingCode)
{
    using var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(bookingCode, QRCodeGenerator.ECCLevel.Q);
    var qrCode = new PngByteQRCode(qrCodeData);
    var qrCodeImage = qrCode.GetGraphic(20);
    
    // Return as Base64 string
    return Convert.ToBase64String(qrCodeImage);
}
```

### QR Code Scan & Check-In

```csharp
// POST /api/bookings/check-in
// Request: { "bookingCode": "BK-20251103-0001" }

public async Task<Result> CheckInBookingAsync(string bookingCode)
{
    var booking = await _context.Bookings
        .FirstOrDefaultAsync(b => b.Bookingcode == bookingCode);
    
    if (booking == null)
        return Result.NotFound("Booking not found");
    
    if (booking.Status != BookingStatus.Confirmed.ToString())
        return Result.BadRequest("Booking must be confirmed to check-in");
    
    var showtime = await _context.Showtimes.FindAsync(booking.Showtimeid);
    if (showtime.Starttime > DateTime.UtcNow.AddMinutes(30))
        return Result.BadRequest("Check-in opens 30 minutes before showtime");
    
    // Update status
    booking.Status = BookingStatus.CheckedIn.ToString();
    await _context.SaveChangesAsync();
    
    return Result.Success("Check-in successful");
}
```

---

## 📊 Booking Code Format

```
BK-YYYYMMDD-XXXX
│  │        └─ Sequential number (0001-9999, resets daily)
│  └─ Date (20251103)
└─ Prefix (Booking)

Examples:
- BK-20251103-0001  (First booking of Nov 3, 2025)
- BK-20251103-0125  (125th booking of same day)
- BK-20251104-0001  (First booking of next day)
```

**Why this format?**
- ✅ Human-readable
- ✅ Sortable by date
- ✅ Easy to search
- ✅ Unique across system
- ✅ Max length 17 chars (fits in 20)

---

## 🧪 Testing Checklist

After migration completes:

### Database Level
- [ ] Column `bookingcode` exists in `public.bookings`
- [ ] Column is VARCHAR(20) NOT NULL
- [ ] Unique index `IX_Bookings_BookingCode` exists
- [ ] All existing bookings have unique codes

### Application Level
- [ ] BookingCodeGenerator registered in DI
- [ ] New bookings get auto-generated codes
- [ ] Codes are unique (no duplicates)
- [ ] QR code generation works
- [ ] Check-in endpoint validates booking code
- [ ] Status transitions work correctly:
  - Pending → Confirmed (after payment)
  - Confirmed → CheckedIn (at cinema)
  - CheckedIn → Completed (after showtime)
  - Pending → Expired (timeout)
  - Any → Cancelled (user cancellation)

### API Endpoints to Update
- [ ] POST `/api/bookings/create` - Generate bookingcode
- [ ] GET `/api/bookings/{id}` - Include bookingcode in response
- [ ] GET `/api/bookings/my-bookings` - Include bookingcode
- [ ] POST `/api/bookings/check-in` - NEW endpoint for QR scan
- [ ] GET `/api/bookings/by-code/{code}` - NEW endpoint to lookup

---

## 📝 Documentation Updates

Updated files:
- ✅ `docs/Entity-Reference.md` - Added bookingcode field
- ⏳ `docs/screens/04-Booking-Flow.md` - Update booking response DTOs
- ⏳ `docs/screens/02-Home-MainScreens.md` - Update booking list DTOs

---

## 🚀 Next Steps

1. **Run SQL Migration** (Option 1 recommended)
2. **Register BookingCodeGenerator** in ServiceExtensions
3. **Update BookingService** to generate codes
4. **Update DTOs** to include BookingCode field
5. **Implement QR Code generation** (optional, can use 3rd party)
6. **Create Check-In endpoint** for cinema staff
7. **Test end-to-end** booking flow with new codes

---

**Created**: November 3, 2025  
**Next Action**: Run SQL migration on Supabase Dashboard
