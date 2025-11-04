# ⚠️ QUAN TRỌNG - ĐỌC TRƯỚC KHI IMPLEMENT ADMIN ENDPOINTS

**Date Updated**: November 5, 2025  
**For**: Tri (Admin Movie/Cinema/Showtime endpoints implementation)

---

## 🚨 CRITICAL: Entity Structure Corrections

**TL;DR**: Admin docs đã được **cập nhật** để phản ánh đúng entity structure. Nhiều field trong docs **KHÔNG TỒN TẠI** trong entities hiện tại.

---

## 📋 What Changed?

### 1. Movie Entity Issues

#### ❌ REMOVED: Soft Delete Flag
**Docs cũ claim**: Movie có soft delete với `IsDeleted` field  
**Thực tế**: Entity **KHÔNG CÓ** field này!

**Current Movie Entity**:
```csharp
[Table("movies")]
public partial class Movie
{
    [Column("movieid")] public int Movieid { get; set; }
    [Column("title")] public string Title { get; set; } // max 200
    [Column("description")] public string? Description { get; set; }
    [Column("durationminutes")] public int Durationminutes { get; set; }
    [Column("director")] public string? Director { get; set; } // max 100
    [Column("releasedate")] public DateOnly? Releasedate { get; set; }
    [Column("posterurl")] public string? Posterurl { get; set; } // max 255
    [Column("trailerurl")] public string? Trailerurl { get; set; } // max 255
    [Column("country")] public string? Country { get; set; } // max 100
    [Column("rating")] public string Rating { get; set; } // max 10
    [Column("genre")] public string? Genre { get; set; } // max 255
    [Column("createdat")] public DateTime? Createdat { get; set; }
    
    // ❌ NO: isdeleted field
    // ❌ NO: status field
    
    // Navigation
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<Showtime> Showtimes { get; set; }
}
```

**DELETE Options**:
- **Option 1**: Hard delete (permanent removal from DB)
- **Option 2**: Add migration to add `status` column ("Active", "Inactive", "Deleted")
- **Option 3**: Add migration to add `isdeleted` boolean column

**Recommended**: Option 2 - Add `status` field for better tracking.

---

### 2. Cinema Entity Issues

#### ⚠️ MISSING FIELDS
**Docs request**: `email`, `latitude`, `longitude`, `facilities`, `parkingAvailable`, `numberOfAuditoriums`  
**Thực tế**: Entity **CHỈ CÓ** 6 fields cơ bản!

**Current Cinema Entity**:
```csharp
[Table("cinemas")]
public partial class Cinema
{
    [Column("cinemaid")] public int Cinemaid { get; set; }
    [Column("name")] public string Name { get; set; } // max 100, not null
    [Column("address")] public string Address { get; set; } // max 255, not null
    [Column("phone")] public string? Phone { get; set; } // max 20
    [Column("city")] public string? City { get; set; } // max 100
    [Column("createdat")] public DateTime? Createdat { get; set; }
    
    // ❌ NO: email field
    // ❌ NO: district field
    // ❌ NO: latitude, longitude fields
    // ❌ NO: facilities field (need JSONB or separate table)
    // ❌ NO: parkingavailable field
    
    // Navigation
    public virtual ICollection<Auditorium> Auditoria { get; set; }
}
```

**Implementation Options**:

**Minimal (Use existing fields only)**:
```csharp
// POST /api/admin/cinemas request
{
  "name": "CGV Landmark 81",
  "address": "720A Dien Bien Phu, Binh Thanh, HCMC",
  "city": "Ho Chi Minh",
  "phone": "1900 6017"
}
// ✅ All fields exist in entity
```

**Full Feature (Need migration)**:
```sql
-- Add missing columns
ALTER TABLE cinemas 
ADD COLUMN email VARCHAR(100),
ADD COLUMN district VARCHAR(100),
ADD COLUMN latitude DECIMAL(10, 7),
ADD COLUMN longitude DECIMAL(10, 7),
ADD COLUMN facilities JSONB, -- ["3D", "IMAX", "4DX"]
ADD COLUMN parkingavailable BOOLEAN DEFAULT false;
```

**Workaround (Without migration)**:
- `district` → Include in `address` field
- `email` → Store in separate contact table or ignore
- `latitude/longitude` → Ignore for now (add later when need map integration)
- `facilities` → Store as comma-separated string in new column or ignore
- `parkingavailable` → Ignore for now
- `numberOfAuditoriums` → Calculate from `Auditoria.Count()` (no storage needed)

---

### 3. Showtime Entity Issues

#### ⚠️ FIELD MAPPING ISSUES
**Docs request**: Separate `language` and `subtitle` fields, per-seat-type `pricing`  
**Thực tế**: Entity có `languagetype` (combined) và single `price`

**Current Showtime Entity**:
```csharp
[Table("showtimes")]
public partial class Showtime
{
    [Column("showtimeid")] public int Showtimeid { get; set; }
    [Column("movieid")] public int Movieid { get; set; }
    [Column("auditoriumid")] public int Auditoriumid { get; set; }
    [Column("starttime")] public DateTime Starttime { get; set; }
    [Column("endtime")] public DateTime? Endtime { get; set; } // Auto-calculate
    [Column("price")] public decimal Price { get; set; } // Single price only
    [Column("format")] public string Format { get; set; } // max 20
    [Column("languagetype")] public string Languagetype { get; set; } // max 50
    
    // ❌ NO separate: language field
    // ❌ NO separate: subtitle field
    // ❌ NO per-seat-type pricing (standard/vip/couple)
    
    // Navigation
    public virtual Movie Movie { get; set; }
    public virtual Auditorium Auditorium { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; }
    public virtual ICollection<Bookingseat> Bookingseats { get; set; }
}
```

**Field Mapping**:

```csharp
// Request DTO → Entity mapping
{
  "language": "English",          // ─┐
  "subtitle": "Vietnamese"        // ─┤ → Combine into languagetype
}                                 // ─┘

// Store as:
showtime.Languagetype = "English - Phụ đề Việt";

// Pricing mapping
{
  "basePrice": 90000              // → Maps directly to price
  "pricing": {                    // ❌ NOT SUPPORTED
    "standard": 90000,
    "vip": 150000,
    "couple": 180000
  }
}
```

**Pricing Options**:
- **Option 1**: Use single `price` for all seats (current entity)
- **Option 2**: Add seat-type pricing via separate `showtimepricing` table
- **Option 3**: Calculate price dynamically based on seat type in booking logic

**Recommended**: Option 1 for now (simplest), add Option 2 if needed later.

**Endtime Calculation**:
```csharp
// Auto-calculate endtime from starttime + movie duration
showtime.Endtime = showtime.Starttime.AddMinutes(movie.Durationminutes);
```

---

## 🎯 Implementation Checklist

### For Movie Management (Tri)

**POST /api/movies** - Create Movie:
- [ ] Use all available entity fields
- [ ] Don't try to set `IsDeleted` or `status` (fields don't exist)
- [ ] Set `createdat = DateTime.UtcNow`
- [ ] Validate: `title` (required, max 200), `durationminutes` (> 0), `rating` (required)

**PUT /api/movies/{id}** - Update Movie:
- [ ] Allow updating nullable fields
- [ ] Don't try to update `IsDeleted` or `status`

**DELETE /api/movies/{id}** - Delete Movie:
- [ ] Check: Movie has no bookings (via Showtimes → Bookings)
- [ ] Use hard delete: `_context.Movies.Remove(movie)`
- [ ] Or implement soft delete by adding `status` column first

**GET /api/admin/movies** - List with Stats:
- [ ] JOIN queries to calculate aggregated data:
  ```csharp
  var movies = await _context.Movies
      .Select(m => new AdminMovieDTO
      {
          MovieId = m.Movieid,
          Title = m.Title,
          // Calculate from relationships
          TotalBookings = m.Showtimes
              .SelectMany(s => s.Bookings)
              .Count(),
          Revenue = m.Showtimes
              .SelectMany(s => s.Bookings)
              .SelectMany(b => b.Payments)
              .Where(p => p.Status == "Completed")
              .Sum(p => p.Amount),
          AverageRating = m.Reviews.Average(r => r.Rating) ?? 0,
          TotalReviews = m.Reviews.Count()
      })
      .ToListAsync();
  ```

---

### For Cinema Management (Tri)

**POST /api/admin/cinemas** - Create Cinema:

**Minimal Implementation (No migration)**:
```csharp
var cinema = new Cinema
{
    Name = request.Name,           // ✅ Required
    Address = request.Address,     // ✅ Required
    City = request.City,           // ✅ Optional
    Phone = request.Phone,         // ✅ Optional
    Createdat = DateTime.UtcNow
    // ❌ Skip: email, latitude, longitude, facilities, parking
};
```

**Full Implementation (After migration)**:
```csharp
var cinema = new Cinema
{
    Name = request.Name,
    Address = request.Address,
    City = request.City,
    Phone = request.Phone,
    Email = request.Email,              // ✅ After migration
    Latitude = request.Latitude,        // ✅ After migration
    Longitude = request.Longitude,      // ✅ After migration
    Facilities = JsonSerializer.Serialize(request.Facilities), // ✅ After migration
    Parkingavailable = request.ParkingAvailable, // ✅ After migration
    Createdat = DateTime.UtcNow
};
```

**GET /api/admin/cinemas** - List Cinemas:
```csharp
// Calculate numberOfAuditoriums from relationship
var cinemas = await _context.Cinemas
    .Select(c => new CinemaDTO
    {
        CinemaId = c.Cinemaid,
        Name = c.Name,
        Address = c.Address,
        City = c.City,
        Phone = c.Phone,
        NumberOfAuditoriums = c.Auditoria.Count() // ✅ Calculate, don't store
    })
    .ToListAsync();
```

---

### For Showtime Management (Tri)

**POST /api/admin/showtimes** - Create Showtime:
```csharp
// Get movie to calculate endtime
var movie = await _context.Movies.FindAsync(request.MovieId);

var showtime = new Showtime
{
    Movieid = request.MovieId,
    Auditoriumid = request.AuditoriumId,
    Starttime = request.StartTime,
    Endtime = request.StartTime.AddMinutes(movie.Durationminutes), // ✅ Auto-calculate
    Price = request.BasePrice, // ✅ Single price
    Format = request.Format,   // "2D", "3D", "IMAX"
    Languagetype = $"{request.Language} - {request.Subtitle}" // ✅ Combine
};

// ❌ Don't try to store per-seat-type pricing in entity
```

**POST /api/admin/showtimes/bulk** - Bulk Create:
```csharp
// For each date in range
for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
{
    // Skip if in skipDays
    if (request.SkipDays.Contains(date.DayOfWeek)) continue;
    
    // Determine price (weekday vs weekend)
    var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || 
                    date.DayOfWeek == DayOfWeek.Sunday;
    var price = isWeekend ? request.Pricing.Weekend : request.Pricing.Weekday;
    
    // For each timeslot
    foreach (var timeslot in request.Timeslots)
    {
        var starttime = date.Add(timeslot);
        var showtime = new Showtime
        {
            Movieid = request.MovieId,
            Auditoriumid = request.AuditoriumId,
            Starttime = starttime,
            Endtime = starttime.AddMinutes(movie.Durationminutes),
            Price = price,
            Format = request.Format,
            Languagetype = request.LanguageType
        };
        showtimes.Add(showtime);
    }
}

await _context.Showtimes.AddRangeAsync(showtimes);
await _context.SaveChangesAsync();
```

---

## 💡 Common Mistakes to Avoid

### ❌ Mistake #1: Accessing non-existent fields
```csharp
// ❌ WRONG - Compile error!
movie.IsDeleted = true;
cinema.Email = "test@cgv.vn";
showtime.Language = "English";
```

### ❌ Mistake #2: Trying to store unsupported data
```csharp
// ❌ WRONG - Entity doesn't support this
var showtime = new Showtime
{
    StandardPrice = 90000,  // Field doesn't exist
    VipPrice = 150000,      // Field doesn't exist
    CouplePrice = 180000    // Field doesn't exist
};
```

### ❌ Mistake #3: Forgetting to calculate derived fields
```csharp
// ❌ WRONG - Endtime not set
var showtime = new Showtime
{
    Starttime = startTime,
    // Missing: Endtime calculation
};

// ✅ CORRECT
var showtime = new Showtime
{
    Starttime = startTime,
    Endtime = startTime.AddMinutes(movie.Durationminutes)
};
```

### ✅ Correct Implementations

```csharp
// ✅ Movie: Use available fields only
var movie = new Movie
{
    Title = request.Title,
    Description = request.Description,
    Durationminutes = request.DurationMinutes,
    Director = request.Director,
    Releasedate = request.ReleaseDate,
    Posterurl = request.PosterUrl,
    Trailerurl = request.TrailerUrl,
    Country = request.Country,
    Rating = request.Rating,
    Genre = request.Genre,
    Createdat = DateTime.UtcNow
};

// ✅ Cinema: Minimal fields
var cinema = new Cinema
{
    Name = request.Name,
    Address = request.Address,
    City = request.City,
    Phone = request.Phone,
    Createdat = DateTime.UtcNow
};

// ✅ Showtime: Combine fields correctly
var showtime = new Showtime
{
    Movieid = request.MovieId,
    Auditoriumid = request.AuditoriumId,
    Starttime = request.StartTime,
    Endtime = request.StartTime.AddMinutes(movie.Durationminutes),
    Price = request.BasePrice,
    Format = request.Format,
    Languagetype = $"{request.Language} - {request.Subtitle}"
};
```

---

## 📞 Questions?

If you have any questions about:
- Missing entity fields and how to handle them
- Whether to add migrations or use workarounds
- How to map request DTOs to entity fields
- Aggregated data calculations

**Ask before coding!** Better to clarify now than refactor later.

---

## 📚 Related Documentation

- **Entity Verification Report**: `ENTITY_VERIFICATION_REPORT.md` (comprehensive analysis)
- **Staff Implementation Guide**: `00-IMPORTANT-READ-FIRST.md` (for Việt)
- **Current Entities**: `Movie88.Infrastructure/Entities/` folder

---

**Summary**: 
- ❌ Movie: No `IsDeleted` field (use hard delete or add status column)
- ❌ Cinema: Missing 6 fields (email, lat/lng, facilities, parking) - need migration or skip
- ❌ Showtime: No separate language/subtitle (combine in `languagetype`), no per-seat pricing
- ✅ All aggregated data (totalBookings, revenue, etc.) are DTO calculations via JOINs
- ✅ Always calculate `endtime` = `starttime + durationminutes`
- ✅ Use existing fields only, or add migrations first

**Happy coding!** 🚀
