# 🎫 Screen 4: Booking Flow (10 Endpoints)

**Status**: 🔄 **PENDING** (0/10 endpoints - 0%)  
**Assigned**: Trung

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Screen | Auth | Status | Assign |
|---|--------|----------|--------|------|--------|--------|
| 1 | GET | `/api/cinemas` | SelectCinemaActivity | ❌ | ❌ TODO | Trung |
| 2 | GET | `/api/showtimes/by-movie/{movieId}` | SelectCinemaActivity | ❌ | ❌ TODO | Trung |
| 3 | GET | `/api/showtimes/by-date` | SelectCinemaActivity | ❌ | ❌ TODO | Trung |
| 4 | GET | `/api/showtimes/{id}` | SelectSeatActivity | ❌ | ❌ TODO | Trung |
| 5 | GET | `/api/showtimes/{id}/available-seats` | SelectSeatActivity | ❌ | ❌ TODO | Trung |
| 6 | GET | `/api/auditoriums/{id}/seats` | SelectSeatActivity | ❌ | ❌ TODO | Trung |
| 7 | POST | `/api/bookings/create` | SelectSeatActivity | ✅ | ❌ TODO | Trung |
| 8 | GET | `/api/combos` | SelectComboActivity | ❌ | ❌ TODO | Trung |
| 9 | POST | `/api/bookings/{id}/add-combos` | SelectComboActivity | ✅ | ❌ TODO | Trung |
| 10 | GET | `/api/movies/{id}` | SelectCinemaActivity | ❌ | ✅ DONE | Trung |

---

## 🎯 1. GET /api/cinemas

**Screen**: SelectCinemaActivity  
**Auth Required**: ❌ No

### Query Parameters
```
?city=Ho Chi Minh City
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| city | string | ❌ | Filter by city |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Cinemas retrieved successfully",
  "data": [
    {
      "cinemaid": 1,
      "name": "CGV Vincom Center",
      "address": "72 Le Thanh Ton, District 1",
      "phone": "028-3822-5501",
      "city": "Ho Chi Minh City",
      "createdat": "2024-01-15T00:00:00"
    },
    {
      "cinemaid": 2,
      "name": "CGV Crescent Mall",
      "address": "101 Ton Dat Tien, District 7",
      "phone": "028-5412-8999",
      "city": "Ho Chi Minh City",
      "createdat": "2024-02-20T00:00:00"
    }
  ]
}
```

### Related Entities
**Cinema** (cinemas table):
- ✅ `cinemaid` (int, PK)
- ✅ `name` (string, max 100)
- ✅ `address` (string, max 255)
- ✅ `phone` (string, max 20, nullable)
- ✅ `city` (string, max 100, nullable)
- ✅ `createdat` (timestamp, nullable)
- ❌ KHÔNG có: `latitude`, `longitude`, `imageurl`

### Business Logic
- Return all cinemas
- Optional filter by city
- Sort by name ASC

---

## 🎯 2. GET /api/showtimes/by-movie/{movieId}

**Screen**: SelectCinemaActivity  
**Auth Required**: ❌ No

### Query Parameters
```
?date=2025-11-05&cinemaid=1
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| date | string | ❌ | Filter by date (yyyy-MM-dd), default: today |
| cinemaid | int | ❌ | Filter by cinema |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Showtimes by movie retrieved successfully",
  "data": {
    "movie": {
      "movieid": 1,
      "title": "The Avengers",
      "durationminutes": 143,
      "rating": "PG-13",
      "posterurl": "https://..."
    },
    "showtimesByDate": [
      {
        "date": "2025-11-05",
        "cinemas": [
          {
            "cinemaid": 1,
            "name": "CGV Vincom Center",
            "address": "72 Le Thanh Ton, District 1",
            "showtimes": [
              {
                "showtimeid": 42,
                "starttime": "2025-11-05T10:30:00",
                "endtime": "2025-11-05T12:53:00",
                "price": 100000,
                "format": "2D",
                "languagetype": "Phụ đề",
                "auditoriumid": 5,
                "auditoriumname": "Cinema 3",
                "availableSeats": 145
              }
            ]
          }
        ]
      }
    ]
  }
}
```

### Related Entities
Same as Screen 3: GET /api/movies/{id}/showtimes

### Business Logic
- Same as Screen 3 endpoint
- Group showtimes by date → cinema
- Calculate available seats

---

## 🎯 3. GET /api/showtimes/by-date

**Screen**: SelectCinemaActivity  
**Auth Required**: ❌ No

### Query Parameters
```
?date=2025-11-05&cinemaid=1&movieid=1
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| date | string | ✅ | Filter by date (yyyy-MM-dd) |
| cinemaid | int | ❌ | Filter by cinema |
| movieid | int | ❌ | Filter by movie |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Showtimes by date retrieved successfully",
  "data": [
    {
      "showtimeid": 42,
      "starttime": "2025-11-05T10:30:00",
      "endtime": "2025-11-05T12:53:00",
      "price": 100000,
      "format": "2D",
      "languagetype": "Phụ đề",
      "movie": {
        "movieid": 1,
        "title": "The Avengers",
        "posterurl": "https://...",
        "durationminutes": 143,
        "rating": "PG-13"
      },
      "cinema": {
        "cinemaid": 1,
        "name": "CGV Vincom Center",
        "address": "72 Le Thanh Ton, District 1"
      },
      "auditorium": {
        "auditoriumid": 5,
        "name": "Cinema 3",
        "capacity": 150
      },
      "availableSeats": 145
    }
  ]
}
```

### Business Logic
- Filter by date (required)
- Optional filter by cinemaid, movieid
- Only future showtimes: `starttime >= DateTime.UtcNow`
- Calculate available seats
- Sort by starttime ASC

---

## 🎯 4. GET /api/showtimes/{id}

**Screen**: SelectSeatActivity  
**Auth Required**: ❌ No

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Showtime details retrieved successfully",
  "data": {
    "showtimeid": 42,
    "movieid": 1,
    "auditoriumid": 5,
    "starttime": "2025-11-05T10:30:00",
    "endtime": "2025-11-05T12:53:00",
    "price": 100000,
    "format": "2D",
    "languagetype": "Phụ đề",
    "movie": {
      "movieid": 1,
      "title": "The Avengers",
      "posterurl": "https://...",
      "durationminutes": 143,
      "rating": "PG-13"
    },
    "cinema": {
      "cinemaid": 1,
      "name": "CGV Vincom Center",
      "address": "72 Le Thanh Ton, District 1",
      "city": "Ho Chi Minh City"
    },
    "auditorium": {
      "auditoriumid": 5,
      "name": "Cinema 3",
      "capacity": 150
    },
    "availableSeats": 145
  }
}
```

### Related Entities
**Showtime** (showtimes table):
- ✅ `showtimeid` (int, PK)
- ✅ `movieid` (int, FK)
- ✅ `auditoriumid` (int, FK)
- ✅ `starttime` (timestamp)
- ✅ `endtime` (timestamp, nullable)
- ✅ `price` (decimal(10,2))
- ✅ `format` (string, max 20)
- ✅ `languagetype` (string, max 50)

### Business Logic
- Include Movie, Cinema, Auditorium
- Calculate available seats
- Return 404 if showtime not found

---

## 🎯 5. GET /api/showtimes/{id}/available-seats

**Screen**: SelectSeatActivity  
**Auth Required**: ❌ No

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Available seats retrieved successfully",
  "data": {
    "showtimeid": 42,
    "auditoriumid": 5,
    "totalSeats": 150,
    "availableSeats": 145,
    "bookedSeats": 5,
    "seats": [
      {
        "seatid": 125,
        "row": "A",
        "number": 5,
        "type": "Standard",
        "isavailable": false
      },
      {
        "seatid": 126,
        "row": "A",
        "number": 6,
        "type": "Standard",
        "isavailable": true
      }
    ]
  }
}
```

### Related Entities
**Seat** (seats table):
- ✅ `seatid` (int, PK)
- ✅ `auditoriumid` (int, FK)
- ✅ `row` (string, max 2) - A, B, C...
- ✅ `number` (int) - 1, 2, 3...
- ✅ `type` (string, max 20, nullable) - Standard, VIP, Couple
- ✅ `isavailable` (bool, nullable)
- ❌ KHÔNG có: `price` (price ở Showtime)

**Bookingseat** (bookingseats junction):
- ✅ `bookingseatid` (int, PK)
- ✅ `bookingid` (int, FK)
- ✅ `seatid` (int, FK)
- ✅ `showtimeid` (int, FK)
- ✅ `price` (decimal(10,2))

### Business Logic
- Get all seats in auditorium
- Check if seat is booked for this specific showtime:
  ```sql
  WHERE seatid IN (
    SELECT seatid FROM bookingseats 
    WHERE showtimeid = @showtimeId 
    AND booking.status != 'Cancelled'
  )
  ```
- Mark seat `isavailable = false` if booked
- Return all seats with availability status

---

## 🎯 6. GET /api/auditoriums/{id}/seats

**Screen**: SelectSeatActivity  
**Auth Required**: ❌ No

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Auditorium seats retrieved successfully",
  "data": {
    "auditoriumid": 5,
    "name": "Cinema 3",
    "capacity": 150,
    "cinemaid": 1,
    "seatLayout": {
      "rows": ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"],
      "seatsPerRow": 15,
      "seats": [
        {
          "seatid": 125,
          "row": "A",
          "number": 5,
          "type": "Standard",
          "isavailable": true
        }
      ]
    }
  }
}
```

### Related Entities
**Auditorium** (auditoriums table):
- ✅ `auditoriumid` (int, PK)
- ✅ `cinemaid` (int, FK)
- ✅ `name` (string, max 50)
- ✅ `capacity` (int)

**Seat** (seats table): Same as above

### Business Logic
- Get all seats in auditorium
- Group by row
- Return seat layout information
- Note: This endpoint doesn't check showtime-specific bookings

---

## 🎯 7. POST /api/bookings/create

**Screen**: SelectSeatActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "showtimeid": 42,
  "seatids": [125, 126]
}
```

### Validation Rules
- `showtimeid`: Required, must exist
- `seatids`: Required, array of integers, min 1 seat
- Seats must be available (not booked for this showtime)
- Showtime must be in future

### Response 201 Created
```json
{
  "success": true,
  "statusCode": 201,
  "message": "Booking created successfully",
  "data": {
    "bookingid": 156,
    "bookingcode": "BK-20251103-0156",
    "customerid": 3,
    "showtimeid": 42,
    "voucherid": null,
    "bookingtime": "2025-11-03T15:45:00",
    "totalamount": 200000,
    "status": "Pending",
    "movie": {
      "movieid": 1,
      "title": "The Avengers",
      "posterurl": "https://..."
    },
    "cinema": {
      "cinemaid": 1,
      "name": "CGV Vincom Center"
    },
    "showtime": {
      "starttime": "2025-11-05T10:30:00",
      "format": "2D"
    },
    "seats": [
      {
        "seatid": 125,
        "row": "A",
        "number": 5,
        "type": "Standard",
        "price": 100000
      },
      {
        "seatid": 126,
        "row": "A",
        "number": 6,
        "type": "Standard",
        "price": 100000
      }
    ]
  }
}
```

### Related Entities
**Booking** (bookings table):
- ✅ `bookingid` (int, PK)
- ✅ `bookingcode` (string, max 20, UNIQUE) - **NEW: For QR code (Format: BK-YYYYMMDD-XXXX)**
- ✅ `customerid` (int, FK to customers) - NOT `userid`!
- ✅ `showtimeid` (int, FK)
- ✅ `voucherid` (int, FK, nullable)
- ✅ `bookingtime` (timestamp, nullable)
- ✅ `totalamount` (decimal(10,2), nullable)
- ✅ `status` (string, max 50, nullable) - Use **BookingStatus enum**: Pending, Confirmed, Cancelled, CheckedIn, Completed, Expired

**Bookingseat** (bookingseats junction):
- Create records for each selected seat

### Business Logic
1. **Get User & Customer**:
   ```csharp
   var userId = GetUserIdFromJwt();
   var customer = await _context.Customers
       .FirstOrDefaultAsync(c => c.Userid == userId);
   ```

2. **Validate Showtime**:
   - Must exist
   - Must be in future: `starttime > DateTime.UtcNow`

3. **Validate Seats**:
   - All seats must exist
   - All seats must belong to showtime's auditorium
   - Check if seats are available:
     ```sql
     NOT EXISTS (
       SELECT 1 FROM bookingseats bs
       JOIN bookings b ON bs.bookingid = b.bookingid
       WHERE bs.seatid IN (@seatIds) 
       AND bs.showtimeid = @showtimeId
       AND b.status != 'Cancelled'
     )
     ```

4. **Calculate Total**:
   - `totalamount = Showtime.price × seatCount`

5. **Generate BookingCode**:
   ```csharp
   var bookingTime = DateTime.UtcNow;
   var bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
   // Format: BK-20251103-0001
   ```

6. **Create Booking**:
   - `customerid` from Customer table
   - `bookingcode` from BookingCodeGenerator
   - `bookingtime` = DateTime.UtcNow (as timestamp without time zone)
   - `status` = BookingStatus.Pending.ToString() or "Pending"
   - `voucherid` = null (apply later)

7. **Create Bookingseats**:
   - For each seat, create Bookingseat record:
     - `bookingid`, `seatid`, `showtimeid`, `price` (from Showtime)

8. **Transaction**:
   - Wrap in database transaction for atomicity

### Error Cases
- 401 Unauthorized - No valid token
- 404 Not Found - Showtime or seats don't exist
- 400 Bad Request - Invalid seat selection
- 409 Conflict - Seats already booked

---

## 🎯 8. GET /api/combos

**Screen**: SelectComboActivity  
**Auth Required**: ❌ No

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Combos retrieved successfully",
  "data": [
    {
      "comboid": 1,
      "name": "Combo 1 (Small)",
      "description": "1 Bắp nhỏ + 1 Nước ngọt (M)",
      "price": 65000,
      "imageurl": "https://iguov8nhvyobj.vcdn.cloud/media/concession/combo1.png",
      "isavailable": true
    },
    {
      "comboid": 2,
      "name": "Combo 2 (Medium)",
      "description": "1 Bắp vừa + 2 Nước ngọt (M)",
      "price": 85000,
      "imageurl": "https://iguov8nhvyobj.vcdn.cloud/media/concession/combo2.png",
      "isavailable": true
    },
    {
      "comboid": 3,
      "name": "Combo 3 (Large)",
      "description": "1 Bắp lớn + 2 Nước ngọt (L)",
      "price": 105000,
      "imageurl": "https://iguov8nhvyobj.vcdn.cloud/media/concession/combo3.png",
      "isavailable": true
    }
  ]
}
```

### Related Entities
**Combo** (combos table):
- ✅ `comboid` (int, PK)
- ✅ `name` (string, max 100)
- ✅ `description` (string, max 255, nullable)
- ✅ `price` (decimal(10,2))
- ✅ `imageurl` (string, max 255, nullable)
- ✅ `isavailable` (bool, nullable)

### Business Logic
- Filter: `isavailable = true`
- Sort by price ASC

---

## 🎯 9. POST /api/bookings/{id}/add-combos

**Screen**: SelectComboActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "combos": [
    {
      "comboid": 2,
      "quantity": 1
    },
    {
      "comboid": 1,
      "quantity": 2
    }
  ]
}
```

### Validation Rules
- `combos`: Required, array with min 1 item
- Each combo must have valid `comboid` and `quantity > 0`
- Booking must exist and belong to user
- Booking status must be "Pending"

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Combos added to booking successfully",
  "data": {
    "bookingid": 156,
    "combos": [
      {
        "comboid": 2,
        "name": "Combo 2 (Medium)",
        "quantity": 1,
        "price": 85000
      },
      {
        "comboid": 1,
        "name": "Combo 1 (Small)",
        "quantity": 2,
        "price": 130000
      }
    ],
    "comboTotal": 215000,
    "seatsTotal": 200000,
    "totalamount": 415000
  }
}
```

### Related Entities
**Bookingcombo** (bookingcombos junction):
- ✅ `bookingcomboid` (int, PK)
- ✅ `bookingid` (int, FK)
- ✅ `comboid` (int, FK)
- ✅ `quantity` (int)
- ✅ `price` (decimal(10,2)) - Total price for this combo (Combo.price × quantity)

### Business Logic
1. **Validate Booking**:
   - Get userId from JWT
   - Find Customer by userid
   - Verify booking belongs to customer
   - Verify booking status is "Pending"

2. **Validate Combos**:
   - All comboids must exist
   - All combos must be available

3. **Clear Existing Combos** (optional):
   - Delete existing Bookingcombos for this booking
   - Or just add to existing

4. **Add Combos**:
   - For each combo, create Bookingcombo:
     - `bookingid`, `comboid`, `quantity`
     - `price` = Combo.price × quantity

5. **Update Booking Total**:
   ```csharp
   var seatsTotal = bookingseats.Sum(bs => bs.Price);
   var combosTotal = bookingcombos.Sum(bc => bc.Price);
   booking.Totalamount = seatsTotal + combosTotal;
   ```

6. **Transaction**:
   - Wrap in database transaction

### Error Cases
- 401 Unauthorized - No valid token
- 403 Forbidden - Booking doesn't belong to user
- 404 Not Found - Booking or combo not found
- 400 Bad Request - Invalid quantity or unavailable combo
- 409 Conflict - Booking status not "Pending"

---

## 📊 Implementation Summary

### To Be Created

#### Domain Layer (Movie88.Domain/Models/)
```
❌ (CinemaModel.cs)        - Already needed for Screen 2
❌ (AuditoriumModel.cs)    - Already needed for Screen 2
❌ (SeatModel.cs)          - New
❌ (ShowtimeModel.cs)      - Already needed for Screen 2
❌ (BookingModel.cs)       - Already needed for Screen 2
❌ BookingseatModel.cs     - New
❌ ComboModel.cs           - New
❌ BookingcomboModel.cs    - New
```

#### Application Layer (Movie88.Application/)
```
❌ DTOs/Cinemas/
   - CinemaDTO.cs

❌ DTOs/Showtimes/
   - ShowtimeDetailDTO.cs
   - ShowtimesByMovieDTO.cs
   - ShowtimesByDateDTO.cs

❌ DTOs/Seats/
   - SeatDTO.cs
   - SeatLayoutDTO.cs
   - AvailableSeatsDTO.cs

❌ DTOs/Bookings/
   - CreateBookingRequestDTO.cs
   - CreateBookingResponseDTO.cs
   - AddCombosRequestDTO.cs

❌ DTOs/Combos/
   - ComboDTO.cs

❌ Services/
   - ICinemaService.cs / CinemaService.cs
   - ISeatService.cs / SeatService.cs
   - IComboService.cs / ComboService.cs
   - (BookingService - extend existing)
   - ✅ IBookingCodeGenerator.cs / BookingCodeGenerator.cs (DONE - Nov 3, 2025)
```

#### Infrastructure Layer (Movie88.Infrastructure/)
```
❌ Repositories/
   - ICinemaRepository.cs / CinemaRepository.cs
   - ISeatRepository.cs / SeatRepository.cs
   - IComboRepository.cs / ComboRepository.cs
   - (BookingRepository - extend)
```

#### WebApi Layer (Movie88.WebApi/)
```
❌ Controllers/
   - CinemasController.cs (1 endpoint)
   - ShowtimesController.cs (3 endpoints)
   - AuditoriumsController.cs (1 endpoint)
   - BookingsController.cs (2 endpoints - extend)
   - CombosController.cs (1 endpoint)
```

---

## 📝 Notes for Implementation

### Important Field Mappings

**Seat Entity**:
- ⚠️ Uses separate `row` (string) and `number` (int)
- ⚠️ NOT `rownumber` or `seatnumber`
- ⚠️ NO `price` field (price comes from Showtime)
- ⚠️ Unique constraint: (auditoriumid, row, number)

**Booking Entity**:
- ⚠️ Use `customerid`, NOT `userid`
- ⚠️ Use `bookingtime`, NOT `createdat`
- ⚠️ Use `totalamount`, NOT `totalprice`
- ✅ **`bookingcode`** - MUST generate using IBookingCodeGenerator service (Format: BK-YYYYMMDD-XXXX)
- ✅ **`status`** - Use BookingStatus enum: Pending, Confirmed, Cancelled, CheckedIn, Completed, Expired
- 📝 See: `docs/Booking-Code-Implementation.md` for full details

**Bookingseat Junction**:
- ⚠️ Has own PK `bookingseatid`
- ⚠️ Includes `showtimeid` to track specific showtime
- ⚠️ Stores `price` at time of booking (from Showtime.price)

**Cinema Entity**:
- ❌ NO `latitude`, `longitude`, `imageurl`

### Critical Business Logic

**Seat Availability Check**:
```csharp
// Check if seats are available for specific showtime
var unavailableSeats = await _context.Bookingseats
    .Where(bs => seatIds.Contains(bs.Seatid) 
        && bs.Showtimeid == showtimeId
        && bs.Booking.Status != "Cancelled")
    .Select(bs => bs.Seatid)
    .ToListAsync();

if (unavailableSeats.Any())
    return Conflict($"Seats {string.Join(", ", unavailableSeats)} are already booked");
```

**Create Booking Transaction**:
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // 1. Generate BookingCode
    var bookingTime = DateTime.UtcNow;
    var bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
    
    // 2. Create Booking
    var booking = new Booking
    {
        Customerid = customer.Customerid,
        Showtimeid = request.Showtimeid,
        Bookingcode = bookingCode, // BK-20251103-0001
        Bookingtime = DateTime.SpecifyKind(bookingTime, DateTimeKind.Unspecified),
        Status = BookingStatus.Pending.ToString(), // or "Pending"
        Totalamount = showtime.Price * request.SeatIds.Count
    };
    _context.Bookings.Add(booking);
    await _context.SaveChangesAsync();

    // 3. Create Bookingseats
    foreach (var seatId in request.SeatIds)
    {
        var bookingseat = new Bookingseat
        {
            Bookingid = booking.Bookingid,
            Seatid = seatId,
            Showtimeid = request.Showtimeid,
            Price = showtime.Price
        };
        _context.Bookingseats.Add(bookingseat);
    }
    await _context.SaveChangesAsync();

    await transaction.CommitAsync();
    return booking;
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

**Add Combos to Booking**:
```csharp
// 1. Clear existing combos (optional)
var existingCombos = await _context.Bookingcombos
    .Where(bc => bc.Bookingid == bookingId)
    .ToListAsync();
_context.Bookingcombos.RemoveRange(existingCombos);

// 2. Add new combos
foreach (var comboRequest in request.Combos)
{
    var combo = await _context.Combos.FindAsync(comboRequest.Comboid);
    var bookingcombo = new Bookingcombo
    {
        Bookingid = bookingId,
        Comboid = comboRequest.Comboid,
        Quantity = comboRequest.Quantity,
        Price = combo.Price * comboRequest.Quantity
    };
    _context.Bookingcombos.Add(bookingcombo);
}

// 3. Update booking total
var seatsTotal = await _context.Bookingseats
    .Where(bs => bs.Bookingid == bookingId)
    .SumAsync(bs => bs.Price);
var combosTotal = await _context.Bookingcombos
    .Where(bc => bc.Bookingid == bookingId)
    .SumAsync(bc => bc.Price);
booking.Totalamount = seatsTotal + combosTotal;

await _context.SaveChangesAsync();
```

### PostgreSQL Specific
- timestamp without time zone for bookingtime
- Use transactions for booking creation
- Concurrent booking handling with proper locking

---

## 🧪 Testing Checklist

### GET /api/cinemas
- [ ] Return all cinemas
- [ ] Filter by city works
- [ ] Sort by name

### GET /api/showtimes endpoints
- [ ] Return correct showtimes by movie
- [ ] Return correct showtimes by date
- [ ] Filter by cinemaid works
- [ ] Only show future showtimes
- [ ] Calculate available seats correctly

### GET /api/showtimes/{id}/available-seats
- [ ] Mark booked seats as unavailable
- [ ] Handle cancelled bookings (don't count)
- [ ] Return all seats with availability status

### POST /api/bookings/create
- [ ] Require authentication
- [ ] Validate seat availability
- [ ] Prevent double booking (concurrent requests)
- [ ] Calculate total correctly
- [ ] Use customerid from token
- [ ] Create booking and bookingseats atomically
- [ ] Return 409 for already booked seats

### POST /api/bookings/{id}/add-combos
- [ ] Verify booking ownership
- [ ] Only allow for "Pending" bookings
- [ ] Update total amount correctly
- [ ] Handle clearing existing combos

---

**Created**: November 3, 2025  
**Last Updated**: November 3, 2025  
**Progress**: ❌ 0/10 endpoints (0%)
