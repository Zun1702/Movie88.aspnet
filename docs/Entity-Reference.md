# 📊 Entity Reference - PostgreSQL Database

> **Thực tế entities từ Supabase PostgreSQL sau khi scaffold**  
> **19 tables** đã được scaffold từ database  
> ✅ **BookingCode & BookingStatus** implemented on Nov 3, 2025

---

## ✅ Entities Có Sẵn (19 tables)

### 🎬 Core Entities

#### 1. Movie
```csharp
[Table("movies")]
public partial class Movie
{
    [Column("movieid")] public int Movieid { get; set; }
    [Column("title")] [StringLength(200)] public string Title { get; set; }
    [Column("description")] public string? Description { get; set; }
    [Column("durationminutes")] public int Durationminutes { get; set; }
    [Column("director")] [StringLength(100)] public string? Director { get; set; }
    [Column("trailerurl")] [StringLength(255)] public string? Trailerurl { get; set; }
    [Column("releasedate")] public DateOnly? Releasedate { get; set; }
    [Column("posterurl")] [StringLength(255)] public string? Posterurl { get; set; }
    [Column("country")] [StringLength(100)] public string? Country { get; set; }
    [Column("rating")] [StringLength(10)] public string Rating { get; set; }  // NOT decimal! String (age rating)
    [Column("genre")] [StringLength(255)] public string? Genre { get; set; }
    
    // Navigation
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<Showtime> Showtimes { get; set; }
}
```

**⚠️ Important:**
- `rating` là **STRING** (age rating: G, PG, PG-13), KHÔNG phải decimal!
- `durationminutes` thay vì `duration`
- `description` thay vì `overview`
- KHÔNG có `backdropurl`, `agerating` fields

---

#### 2. Cinema
```csharp
[Table("cinemas")]
public partial class Cinema
{
    [Column("cinemaid")] public int Cinemaid { get; set; }
    [Column("name")] [StringLength(100)] public string Name { get; set; }
    [Column("address")] [StringLength(255)] public string Address { get; set; }
    [Column("phone")] [StringLength(20)] public string? Phone { get; set; }
    [Column("city")] [StringLength(100)] public string? City { get; set; }
    [Column("createdat", TypeName = "timestamp without time zone")] public DateTime? Createdat { get; set; }
    
    // Navigation
    public virtual ICollection<Auditorium> Auditoria { get; set; }
}
```

**⚠️ Important:**
- KHÔNG có `latitude`, `longitude`, `imageurl`

---

#### 3. Auditorium
```csharp
[Table("auditoriums")]
public partial class Auditorium
{
    [Column("auditoriumid")] public int Auditoriumid { get; set; }
    [Column("cinemaid")] public int Cinemaid { get; set; }
    [Column("name")] [StringLength(50)] public string Name { get; set; }
    [Column("capacity")] public int Capacity { get; set; }
    
    // Navigation
    public virtual Cinema Cinema { get; set; }
    public virtual ICollection<Seat> Seats { get; set; }
    public virtual ICollection<Showtime> Showtimes { get; set; }
}
```

**⚠️ Important:**
- KHÔNG có `type` field (Standard/IMAX/3D)

---

#### 4. Showtime
```csharp
[Table("showtimes")]
public partial class Showtime
{
    [Column("showtimeid")] public int Showtimeid { get; set; }
    [Column("movieid")] public int Movieid { get; set; }
    [Column("auditoriumid")] public int Auditoriumid { get; set; }
    [Column("starttime", TypeName = "timestamp without time zone")] public DateTime Starttime { get; set; }
    [Column("endtime", TypeName = "timestamp without time zone")] public DateTime? Endtime { get; set; }
    [Column("price")] [Precision(10, 2)] public decimal Price { get; set; }
    [Column("format")] [StringLength(20)] public string Format { get; set; }  // 2D, 3D, IMAX
    [Column("languagetype")] [StringLength(50)] public string Languagetype { get; set; }  // Phụ đề/Lồng tiếng
    
    // Navigation
    public virtual Movie Movie { get; set; }
    public virtual Auditorium Auditorium { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; }
    public virtual ICollection<Bookingseat> Bookingseats { get; set; }
}
```

**⚠️ Important:**
- KHÔNG có `status` field
- CÓ `format` (2D/3D/IMAX) và `languagetype` (Phụ đề/Lồng tiếng)

---

### 🎫 Booking Entities

#### 5. Booking
```csharp
[Table("bookings")]
[Index("Bookingcode", Name = "IX_Bookings_BookingCode", IsUnique = true)]
public partial class Booking
{
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("customerid")] public int Customerid { get; set; }
    [Column("showtimeid")] public int Showtimeid { get; set; }
    [Column("voucherid")] public int? Voucherid { get; set; }
    [Column("bookingcode")] [StringLength(20)] public string Bookingcode { get; set; } = null!;
    [Column("bookingtime", TypeName = "timestamp without time zone")] public DateTime? Bookingtime { get; set; }
    [Column("totalamount")] [Precision(10, 2)] public decimal? Totalamount { get; set; }
    [Column("status")] [StringLength(50)] public string? Status { get; set; }
    
    // Navigation
    public virtual Customer Customer { get; set; }
    public virtual Showtime Showtime { get; set; }
    public virtual Voucher? Voucher { get; set; }
    public virtual ICollection<Bookingcombo> Bookingcombos { get; set; }
    public virtual ICollection<Bookingseat> Bookingseats { get; set; }
    public virtual ICollection<Bookingpromotion> Bookingpromotions { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
}
```

**⚠️ Important:**
- ✅ `bookingcode` - UNIQUE, dùng cho QR code check-in (Format: BK-YYYYMMDD-XXXX)
- ✅ `status` - Sử dụng enum `BookingStatus`: Pending, Confirmed, Cancelled, CheckedIn, Completed, Expired
- `bookingtime` thay vì `createdat`
- `totalamount` thay vì `totalprice`
- CÓ navigation tới Voucher (nullable)

---

#### 6. Seat
```csharp
[Table("seats")]
[Index("Auditoriumid", "Row", "Number", Name = "uq_seat", IsUnique = true)]
public partial class Seat
{
    [Column("seatid")] public int Seatid { get; set; }
    [Column("auditoriumid")] public int Auditoriumid { get; set; }
    [StringLength(2)] public string Row { get; set; }  // A, B, C...
    public int Number { get; set; }  // 1, 2, 3...
    [Column("type")] [StringLength(20)] public string? Type { get; set; }  // Standard, VIP, Couple
    [Column("isavailable")] public bool? Isavailable { get; set; }
    
    // Navigation
    public virtual Auditorium Auditorium { get; set; }
    public virtual ICollection<Bookingseat> Bookingseats { get; set; }
}
```

**⚠️ Important:**
- `Row` và `Number` riêng biệt, KHÔNG có `rownumber` + `seatnumber`
- KHÔNG có `price` field (price ở Showtime)

---

#### 7. Bookingseat (Junction Table)
```csharp
[Table("bookingseats")]
public partial class Bookingseat
{
    [Column("bookingseatid")] public int Bookingseatid { get; set; }
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("seatid")] public int Seatid { get; set; }
    [Column("showtimeid")] public int Showtimeid { get; set; }
    [Column("price")] [Precision(10, 2)] public decimal Price { get; set; }
    
    // Navigation
    public virtual Booking Booking { get; set; }
    public virtual Seat Seat { get; set; }
    public virtual Showtime Showtime { get; set; }
}
```

**⚠️ Important:**
- CÓ PK riêng `bookingseatid`
- CÓ `showtimeid` để track ghế cho suất chiếu cụ thể

---

#### 8. Combo
```csharp
[Table("combos")]
public partial class Combo
{
    [Column("comboid")] public int Comboid { get; set; }
    [Column("name")] [StringLength(100)] public string Name { get; set; }
    [Column("description")] [StringLength(255)] public string? Description { get; set; }
    [Column("price")] [Precision(10, 2)] public decimal Price { get; set; }
    [Column("imageurl")] [StringLength(255)] public string? Imageurl { get; set; }
    [Column("isavailable")] public bool? Isavailable { get; set; }
    
    // Navigation
    public virtual ICollection<Bookingcombo> Bookingcombos { get; set; }
}
```

---

#### 9. Bookingcombo (Junction Table)
```csharp
[Table("bookingcombos")]
public partial class Bookingcombo
{
    [Column("bookingcomboid")] public int Bookingcomboid { get; set; }
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("comboid")] public int Comboid { get; set; }
    [Column("quantity")] public int Quantity { get; set; }
    [Column("price")] [Precision(10, 2)] public decimal Price { get; set; }
    
    // Navigation
    public virtual Booking Booking { get; set; }
    public virtual Combo Combo { get; set; }
}
```

---

### 💳 Payment Entities

#### 10. Payment
```csharp
[Table("payments")]
public partial class Payment
{
    [Column("paymentid")] public int Paymentid { get; set; }
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("customerid")] public int Customerid { get; set; }
    [Column("methodid")] public int Methodid { get; set; }
    [Column("amount")] [Precision(10, 2)] public decimal Amount { get; set; }
    [Column("status")] [StringLength(50)] public string? Status { get; set; }
    [Column("transactioncode")] [StringLength(255)] public string? Transactioncode { get; set; }
    [Column("paymenttime", TypeName = "timestamp without time zone")] public DateTime? Paymenttime { get; set; }
    
    // Navigation
    public virtual Booking Booking { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual Paymentmethod Method { get; set; }
}
```

**⚠️ Important:**
- `methodid` FK tới `Paymentmethod` table
- `transactioncode` thay vì `transactionid`
- KHÔNG có `vnpaydata` JSONB field

---

#### 11. Paymentmethod
```csharp
[Table("paymentmethods")]
public partial class Paymentmethod
{
    [Column("methodid")] public int Methodid { get; set; }
    [Column("methodname")] [StringLength(50)] public string Methodname { get; set; }
    
    // Navigation
    public virtual ICollection<Payment> Payments { get; set; }
}
```

**⚠️ Important:**
- Riêng table cho payment methods (VNPay, MoMo, Cash, etc.)

---

### 🎁 Promotion & Voucher Entities

#### 12. Voucher
```csharp
[Table("vouchers")]
[Index("Code", Name = "vouchers_code_key", IsUnique = true)]
public partial class Voucher
{
    [Column("voucherid")] public int Voucherid { get; set; }
    [Column("code")] [StringLength(50)] public string Code { get; set; }
    [Column("description")] [StringLength(255)] public string? Description { get; set; }
    [Column("discounttype")] [StringLength(20)] public string? Discounttype { get; set; }
    [Column("discountvalue")] [Precision(10, 2)] public decimal? Discountvalue { get; set; }
    [Column("minpurchaseamount")] [Precision(10, 2)] public decimal? Minpurchaseamount { get; set; }
    [Column("expirydate")] public DateOnly? Expirydate { get; set; }
    [Column("usagelimit")] public int? Usagelimit { get; set; }
    [Column("usedcount")] public int? Usedcount { get; set; }
    [Column("isactive")] public bool? Isactive { get; set; }
    
    // Navigation
    public virtual ICollection<Booking> Bookings { get; set; }
}
```

**⚠️ Important:**
- `expirydate` (DateOnly) thay vì `validfrom` + `validto`
- KHÔNG có `maxdiscount` field

---

#### 13. Promotion
```csharp
[Table("promotions")]
public partial class Promotion
{
    [Column("promotionid")] public int Promotionid { get; set; }
    [Column("name")] [StringLength(100)] public string Name { get; set; }
    [Column("description")] [StringLength(255)] public string? Description { get; set; }
    [Column("startdate")] public DateOnly? Startdate { get; set; }
    [Column("enddate")] public DateOnly? Enddate { get; set; }
    [Column("discounttype")] [StringLength(20)] public string? Discounttype { get; set; }
    [Column("discountvalue")] [Precision(10, 2)] public decimal? Discountvalue { get; set; }
    
    // Navigation
    public virtual ICollection<Bookingpromotion> Bookingpromotions { get; set; }
}
```

**⚠️ Important:**
- KHÔNG có `imageurl`, `isactive` fields
- Dates là `DateOnly` type

---

#### 14. Bookingpromotion (Junction Table)
```csharp
[Table("bookingpromotions")]
public partial class Bookingpromotion
{
    [Column("bookingpromotionid")] public int Bookingpromotionid { get; set; }
    [Column("bookingid")] public int Bookingid { get; set; }
    [Column("promotionid")] public int Promotionid { get; set; }
    
    // Navigation
    public virtual Booking Booking { get; set; }
    public virtual Promotion Promotion { get; set; }
}
```

---

### ⭐ Review Entity

#### 15. Review
```csharp
[Table("reviews")]
public partial class Review
{
    [Column("reviewid")] public int Reviewid { get; set; }
    [Column("customerid")] public int Customerid { get; set; }
    [Column("movieid")] public int Movieid { get; set; }
    [Column("rating")] public int? Rating { get; set; }  // 1-5
    [Column("comment")] [StringLength(500)] public string? Comment { get; set; }
    [Column("createdat", TypeName = "timestamp without time zone")] public DateTime? Createdat { get; set; }
    
    // Navigation
    public virtual Customer Customer { get; set; }
    public virtual Movie Movie { get; set; }
}
```

**⚠️ Important:**
- `customerid` FK, KHÔNG phải `userid`
- KHÔNG có `updatedat` field

---

### 👤 User & Customer Entities

#### 16. User
```csharp
[Table("users")]
public partial class User
{
    [Column("userid")] public int Userid { get; set; }
    [Column("roleid")] public int Roleid { get; set; }
    [Column("fullname")] [StringLength(100)] public string Fullname { get; set; }
    [Column("email")] [StringLength(100)] public string Email { get; set; }
    [Column("passwordhash")] [StringLength(255)] public string Passwordhash { get; set; }
    [Column("phone")] [StringLength(20)] public string? Phone { get; set; }
    [Column("createdat", TypeName = "timestamp without time zone")] public DateTime? Createdat { get; set; }
    [Column("updatedat", TypeName = "timestamp without time zone")] public DateTime? Updatedat { get; set; }
    
    // Navigation
    public virtual Role Role { get; set; }
    public virtual Customer? Customer { get; set; }
}
```

---

#### 17. Customer
```csharp
[Table("customers")]
[Index("Userid", Name = "customers_userid_key", IsUnique = true)]
public partial class Customer
{
    [Column("customerid")] public int Customerid { get; set; }
    [Column("userid")] public int Userid { get; set; }
    [Column("address")] [StringLength(255)] public string? Address { get; set; }
    [Column("dateofbirth")] public DateOnly? Dateofbirth { get; set; }
    [Column("gender")] [StringLength(10)] public string? Gender { get; set; }
    
    // Navigation
    public virtual User User { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
}
```

**⚠️ Important:**
- KHÔNG có `avatarurl`, `loyaltypoints`, `membershiptier` fields
- 1-1 relationship với User (Unique index)

---

#### 18. Role
```csharp
[Table("roles")]
public partial class Role
{
    [Column("roleid")] public int Roleid { get; set; }
    [Column("rolename")] [StringLength(50)] public string Rolename { get; set; }
    
    // Navigation
    public virtual ICollection<User> Users { get; set; }
}
```

---

#### 19. RefreshToken (Supabase auth schema - DO NOT USE)
```csharp
[Table("refresh_tokens", Schema = "auth")]
// Use UserRefreshToken instead for public.refresh_tokens
```

---

#### 20. UserRefreshToken (Custom - public schema)
```csharp
[Table("refresh_tokens", Schema = "public")]
public partial class UserRefreshToken
{
    [Column("id")] public long Id { get; set; }
    [Column("token")] [StringLength(255)] public string Token { get; set; }
    [Column("user_id")] [StringLength(255)] public string UserId { get; set; }
    [Column("revoked")] public bool Revoked { get; set; }
    [Column("created_at", TypeName = "timestamp without time zone")] public DateTime? CreatedAt { get; set; }
    [Column("updated_at", TypeName = "timestamp without time zone")] public DateTime? UpdatedAt { get; set; }
}
```

---

## 📋 Summary of Key Differences

### ❌ Fields KHÔNG tồn tại trong database:

| Entity | Missing Fields |
|--------|---------------|
| Movie | `backdropurl`, `agerating`, `overview` |
| Cinema | `latitude`, `longitude`, `imageurl` |
| Auditorium | `type` (Standard/IMAX) |
| Showtime | `status` |
| Seat | `price`, `rownumber`, `seatnumber` (use `Row` + `Number`) |
| Booking | `createdat` (use `bookingtime`), `totalprice` (use `totalamount`) |
| Payment | `vnpaydata` (JSONB), `transactionid` (use `transactioncode`) |
| Voucher | `validfrom`, `validto`, `maxdiscount` (only `expirydate`) |
| Promotion | `imageurl`, `isactive`, `discountpercentage` |
| Customer | `avatarurl`, `loyaltypoints`, `membershiptier` |
| Review | `updatedat` |

### ✅ Special Naming Conventions:

- **Snake_case in DB** → **PascalCase in C#** (EF auto-converts)
- `movieid` → `Movieid`
- `durationminutes` → `Durationminutes`
- `isavailable` → `Isavailable`

### 🔧 PostgreSQL Specific Types:

- `DateOnly` for dates (not `DateTime`)
- `timestamp without time zone` for DateTime columns
- `Precision(10, 2)` for decimal money fields
- `StringLength` attributes from database

---

**Created**: Nov 3, 2025  
**Source**: EF Core scaffold from Supabase PostgreSQL  
**Purpose**: Reference for creating accurate DTOs and services
