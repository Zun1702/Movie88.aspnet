# 🎫 Screen 4: Booking Flow (11 Endpoints)

**Status**: ✅ **COMPLETED** (11/11 endpoints - 100%)  
**Assigned**: Trung  
**Completed**: November 5, 2025

> **🎯 Core Business Flow**: Đây là luồng nghiệp vụ QUAN TRỌNG NHẤT của dự án - Đặt vé online

---

## 📋 Endpoints Overview

Chia thành **4 giai đoạn** để dev hiệu quả:

### 🎬 Phase 1: Cinema & Showtime Selection (4 endpoints) ✅
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 1 | GET | `/api/cinemas` | Danh sách rạp | ❌ | ✅ DONE | Trung |
| 2 | GET | `/api/showtimes/by-movie/{movieId}` | Suất chiếu theo phim | ❌ | ✅ DONE | Trung |
| 3 | GET | `/api/showtimes/by-date` | Suất chiếu theo ngày | ❌ | ✅ DONE | Trung |
| 4 | GET | `/api/showtimes/{id}` | Chi tiết suất chiếu | ❌ | ✅ DONE | Trung |

### 💺 Phase 2: Seat Selection & Booking Creation (3 endpoints) ✅
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 5 | GET | `/api/showtimes/{id}/available-seats` | Ghế còn trống | ❌ | ✅ DONE | Trung |
| 6 | GET | `/api/auditoriums/{id}/seats` | Sơ đồ ghế phòng chiếu | ❌ | ✅ DONE | Trung |
| 7 | POST | `/api/bookings/create` | **TẠO BOOKING** | ✅ | ✅ DONE | Trung |

### 🍿 Phase 3: Combo Selection (2 endpoints) ✅
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 8 | GET | `/api/combos` | Danh sách combo | ❌ | ✅ DONE | Trung |
| 9 | POST | `/api/bookings/{id}/add-combos` | Thêm combo vào booking | ✅ | ✅ DONE | Trung |

### 🔄 Phase 4: Booking Management (1 endpoint) ✅
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 11 | POST | `/api/bookings/{id}/cancel` | **HỦY BOOKING** | ✅ | ✅ DONE | Trung |

### 📽️ Reference
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 10 | GET | `/api/movies/{id}` | Chi tiết phim | ❌ | ✅ DONE | Trung |

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
      "seatscount": 150
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
- ✅ `seatprice` (decimal(10,2)) ← **ACTUAL FIELD NAME (not "price")**

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
    "seatscount": 150,
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
- ✅ `seatscount` (int) ← **ACTUAL FIELD NAME**

**Seat** (seats table):
- ✅ `seatid` (int, PK)
- ✅ `auditoriumid` (int, FK)
- ✅ `Row` (string, max 2) ← **Capital R**
- ✅ `Number` (int) ← **Capital N**
- ✅ `type` (string, max 20)
- ✅ `isavailable` (bool)

### Business Logic
- Get all seats in auditorium
- Group by Row (capital R)
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
    "bookingcode": null,
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
- ✅ `bookingcode` (string?, max 20, UNIQUE, **NULLABLE**) - **NULL until payment confirmed** (then generated with format: BK-YYYYMMDD-XXXX)
- ✅ `customerid` (int, FK to customers) - NOT `userid`!
- ✅ `showtimeid` (int, FK)
- ✅ `voucherid` (int, FK, nullable)
- ✅ `bookingtime` (timestamp, nullable) - Use `DateTime.Now` not `DateTime.UtcNow`
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

5. **BookingCode** (⚠️ CHANGED):
   - Initial logic: Generate at creation
   - **Current logic**: Set to `null` (generated only after payment confirmed)
   - Field is nullable: `string? Bookingcode { get; set; }`

6. **Create Booking**:
   - `customerid` from Customer table
   - `bookingcode` = **null** (not generated yet)
   - `bookingtime` = `DateTime.Now` (use .Now not .UtcNow for PostgreSQL)
   - `status` = "Pending"
   - `voucherid` = null (apply later)

7. **Create Bookingseats**:
   - For each seat, create Bookingseat record:
     - `bookingid`, `seatid`, `showtimeid`, `seatprice` (from Showtime.Price)

8. **Transaction** (⚠️ MUST USE EXECUTION STRATEGY):
   ```csharp
   var strategy = _context.Database.CreateExecutionStrategy();
   await strategy.ExecuteAsync(async () => {
       using var transaction = await _context.Database.BeginTransactionAsync();
       // ... create booking and bookingseats ...
       await transaction.CommitAsync();
   });
   ```

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
- ❌ **NO `isavailable` field in entity**

### Business Logic
- Return all combos (no filter by availability)
- Or implement soft delete/active flag if needed
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
- ✅ `comboprice` (decimal(10,2), nullable) ← **ACTUAL FIELD NAME (not "price")** - Total price for this combo (Combo.price × quantity)

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
     - `comboprice` = Combo.price × quantity

5. **Update Booking Total**:
   ```csharp
   var seatsTotal = bookingseats.Sum(bs => bs.Seatprice);
   var combosTotal = bookingcombos.Sum(bc => bc.Comboprice ?? 0);
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

## 🎯 11. POST /api/bookings/{id}/cancel

**Screen**: My Bookings / Booking Details  
**Auth Required**: ✅ Yes  
**Status**: ✅ **DONE** (November 5, 2025)

### Purpose
- Allow users to manually cancel their pending bookings
- Release seats back to available pool
- Prevent payment for unwanted bookings

### Request
```http
POST /api/bookings/156/cancel
Authorization: Bearer {token}
```

### Validation Rules
- Booking must exist
- Booking must belong to authenticated user
- Booking status must be "Pending" (cannot cancel Confirmed/Completed/Cancelled bookings)
- Authentication required

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Booking cancelled successfully",
  "data": {
    "bookingid": 156,
    "bookingcode": null,
    "status": "Cancelled",
    "message": "Booking cancelled successfully. Seats have been released.",
    "releasedSeats": ["A5", "A6"],
    "cancelledAt": "2025-11-05T15:45:30"
  }
}
```

### Business Logic

1. **Validate Ownership**:
   ```csharp
   var userId = GetUserIdFromJwt();
   var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Userid == userId);
   var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
   
   if (booking.Customerid != customer.Customerid)
       throw new UnauthorizedAccessException("This booking does not belong to you");
   ```

2. **Validate Status**:
   ```csharp
   if (booking.Status?.ToLower() != "pending")
       throw new InvalidOperationException($"Cannot cancel booking with status: {booking.Status}");
   ```

3. **Cancel and Release Seats** (Transaction):
   ```csharp
   var strategy = _context.Database.CreateExecutionStrategy();
   await strategy.ExecuteAsync(async () =>
   {
       using var transaction = await _context.Database.BeginTransactionAsync();
       try
       {
           // Update booking status
           booking.Status = "Cancelled";
           _context.Bookings.Update(booking);
           
           // Release all seats
           foreach (var bookingSeat in booking.Bookingseats)
           {
               var seat = await _context.Seats.FindAsync(bookingSeat.Seatid);
               if (seat != null)
               {
                   seat.Isavailable = true; // 🔓 Release seat
                   _context.Seats.Update(seat);
               }
           }
           
           await _context.SaveChangesAsync();
           await transaction.CommitAsync();
       }
       catch
       {
           await transaction.RollbackAsync();
           throw;
       }
   });
   ```

4. **Return Response**:
   - Include list of released seat names (e.g., "A5", "A6")
   - Include cancellation timestamp

### Error Cases
- 401 Unauthorized - No valid token
- 403 Forbidden - Booking doesn't belong to user
- 404 Not Found - Booking not found
- 400 Bad Request - Booking status is not "Pending"

### Related Background Service

**Auto-Cancel Expired Bookings** (Background Service):
- Runs every **5 minutes**
- Auto-cancels bookings in "Pending" status older than **15 minutes**
- Releases seats automatically
- Prevents indefinite seat locking

```csharp
// BookingExpirationService.cs
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        // Find pending bookings older than 15 minutes
        var expiredBookings = await _bookingRepository
            .GetPendingBookingsOlderThanAsync(15, stoppingToken);
        
        foreach (var booking in expiredBookings)
        {
            await _bookingRepository.CancelBookingAndReleaseSeatsAsync(
                booking.Bookingid, stoppingToken);
        }
        
        // Wait 5 minutes before next check
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
}
```

### Implementation Files

**New Files Created**:
```
✅ CancelBookingResponseDTO.cs       - Response DTO
✅ BookingExpirationService.cs       - Background service (auto-cancel)
```

**Modified Files**:
```
✅ IBookingRepository.cs             - Added CancelBookingAndReleaseSeatsAsync()
✅ BookingRepository.cs              - Implemented cancel + seat release logic
✅ IBookingService.cs                - Added CancelBookingAsync()
✅ BookingService.cs                 - Implemented cancel business logic
✅ BookingsController.cs             - Added POST {id}/cancel endpoint
✅ Program.cs                        - Registered BookingExpirationService
```

### Critical Changes

**🔒 Seat Locking Fix** (Also in this update):
- **Problem**: Seats remained `isAvailable = true` after booking creation
- **Impact**: Multiple users could book the same seat
- **Fix**: Added seat locking in `CreateBookingAsync`:
  ```csharp
  // In BookingRepository.CreateBookingAsync()
  foreach (var (seatid, seatprice) in seats)
  {
      var bookingSeat = new Bookingseat { /* ... */ };
      _context.Bookingseats.Add(bookingSeat);
      
      // 🔒 CRITICAL: Mark seat as unavailable
      var seat = await _context.Seats.FindAsync(seatid);
      if (seat != null)
      {
          seat.Isavailable = false;
          _context.Seats.Update(seat);
      }
  }
  ```

### Testing Checklist

- [x] Manual cancel works for Pending bookings
- [x] Cannot cancel Confirmed/Completed bookings
- [x] Cannot cancel other user's bookings
- [x] Seats released correctly (isAvailable = true)
- [x] Background service runs every 5 minutes
- [x] Auto-cancel works for 15+ minute old bookings
- [x] Transaction rollback on errors
- [x] Seat locking works on booking creation

---

## 📊 Implementation Summary

### ✅ Already Created (Entity Models)

#### Domain Layer (Movie88.Domain/Models/)
```
✅ CinemaModel.cs           - Already exists
✅ AuditoriumModel.cs       - Already exists
✅ SeatModel.cs             - Already exists
✅ ShowtimeModel.cs         - Already exists
✅ BookingModel.cs          - Already exists
✅ BookingSeatModel.cs      - Already exists (note: PascalCase)
✅ ComboModel.cs            - Already exists
✅ BookingComboModel.cs     - Already exists (note: PascalCase)
```

#### Infrastructure Layer (Movie88.Infrastructure/Entities/)
```
✅ Cinema.cs                - Already exists
✅ Auditorium.cs            - Already exists
✅ Seat.cs                  - Already exists
✅ Showtime.cs              - Already exists
✅ Booking.cs               - Already exists
✅ Bookingseat.cs           - Already exists
✅ Combo.cs                 - Already exists
✅ Bookingcombo.cs          - Already exists
```

### 🔄 To Be Created/Extended

#### Application Layer (Movie88.Application/)

**Folder Structure:**
```
Movie88.Application/
├── DTOs/
│   ├── Cinemas/           ← NEW FOLDER
│   │   └── CinemaDTO.cs
│   ├── Showtimes/         ← EXTEND EXISTING
│   │   ├── ShowtimeDTO.cs (✅ exists)
│   │   ├── ShowtimeDetailDTO.cs
│   │   ├── ShowtimesByMovieDTO.cs
│   │   └── ShowtimesByDateDTO.cs
│   ├── Seats/             ← NEW FOLDER
│   │   ├── SeatDTO.cs
│   │   ├── SeatLayoutDTO.cs
│   │   └── AvailableSeatsDTO.cs
│   ├── Bookings/          ← EXTEND EXISTING
│   │   ├── BookingListDTO.cs (✅ exists)
│   │   ├── CreateBookingRequestDTO.cs
│   │   ├── CreateBookingResponseDTO.cs
│   │   └── AddCombosRequestDTO.cs
│   └── Combos/            ← NEW FOLDER
│       └── ComboDTO.cs
├── Services/
│   ├── BookingService.cs (✅ extend existing)
│   ├── BookingCodeGenerator.cs (✅ already exists)
│   ├── ShowtimeService.cs (✅ extend existing)
│   ├── CinemaService.cs ← NEW
│   ├── SeatService.cs ← NEW
│   └── ComboService.cs ← NEW
└── Interfaces/
    ├── IBookingService.cs (✅ extend)
    ├── IShowtimeService.cs (✅ extend)
    ├── ICinemaService.cs ← NEW
    ├── ISeatService.cs ← NEW
    └── IComboService.cs ← NEW
```

#### Infrastructure Layer (Movie88.Infrastructure/)

**Folder Structure:**
```
Movie88.Infrastructure/
└── Repositories/
    ├── BookingRepository.cs (✅ extend existing)
    ├── ShowtimeRepository.cs (✅ extend existing)
    ├── CinemaRepository.cs ← NEW
    ├── SeatRepository.cs ← NEW
    └── ComboRepository.cs ← NEW
```

#### WebApi Layer (Movie88.WebApi/)

**Folder Structure:**
```
Movie88.WebApi/
└── Controllers/
    ├── BookingsController.cs (✅ extend - add 2 endpoints)
    ├── CinemasController.cs ← NEW (1 endpoint)
    ├── ShowtimesController.cs ← NEW (3 endpoints)
    ├── AuditoriumsController.cs ← NEW (1 endpoint)
    └── CombosController.cs ← NEW (1 endpoint)
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
- ⚠️ Use `DateTime.Now`, NOT `DateTime.UtcNow` (PostgreSQL timestamp without time zone)
- ✅ **`bookingcode`** - **NULL at creation** (generated only after payment confirmed using IBookingCodeGenerator service, Format: BK-YYYYMMDD-XXXX)
- ✅ **`status`** - Use "Pending", "Confirmed", "Cancelled", etc.
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

**Create Booking Transaction** (⚠️ UPDATED):
```csharp
// MUST use execution strategy for retry compatibility
var strategy = _context.Database.CreateExecutionStrategy();
return await strategy.ExecuteAsync(async () =>
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // 1. BookingCode is NULL (generated only after payment)
        // No longer generate at creation time
        
        // 2. Create Booking
        var booking = new Booking
        {
            Customerid = customer.Customerid,
            Showtimeid = request.Showtimeid,
            Bookingcode = null, // ← NULL until payment confirmed
            Bookingtime = DateTime.Now, // ← Use DateTime.Now not UtcNow
            Status = "Pending",
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
                Seatprice = showtime.Price // ← Use Seatprice field
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

**Add Combos to Booking** (⚠️ UPDATED):
```csharp
// MUST use execution strategy for retry compatibility
var strategy = _context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // 1. Add combos (don't clear existing - allows multiple additions)
        foreach (var comboRequest in request.Combos)
        {
            var combo = await _context.Combos.FindAsync(comboRequest.Comboid);
            var bookingcombo = new Bookingcombo
            {
                Bookingid = bookingId,
                Comboid = comboRequest.Comboid,
                Quantity = comboRequest.Quantity,
                Comboprice = combo.Price * comboRequest.Quantity // ← Use Comboprice field
            };
            _context.Bookingcombos.Add(bookingcombo);
        }

        // 2. Update booking total
        var seatsTotal = await _context.Bookingseats
            .Where(bs => bs.Bookingid == bookingId)
            .SumAsync(bs => bs.Seatprice); // ← Use Seatprice
        var combosTotal = await _context.Bookingcombos
            .Where(bc => bc.Bookingid == bookingId)
            .SumAsync(bc => bc.Comboprice ?? 0); // ← Use Comboprice (nullable)
        booking.Totalamount = seatsTotal + combosTotal;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
});
```

### PostgreSQL Specific
- timestamp without time zone for bookingtime
- Use transactions for booking creation
- Concurrent booking handling with proper locking

---

## 🧪 Testing Checklist

### GET /api/cinemas ✅
- [x] Return all cinemas
- [x] Filter by city works
- [x] Sort by name

### GET /api/showtimes endpoints ✅
- [x] Return correct showtimes by movie
- [x] Return correct showtimes by date
- [x] Filter by cinemaid works
- [x] Only show future showtimes
- [x] Calculate available seats correctly

### GET /api/showtimes/{id}/available-seats ✅
- [x] Mark booked seats as unavailable
- [x] Handle cancelled bookings (don't count)
- [x] Return all seats with availability status

### POST /api/bookings/create ✅
- [x] Require authentication
- [x] Validate seat availability
- [x] Prevent double booking (concurrent requests)
- [x] Calculate total correctly
- [x] Use customerid from token
- [x] Create booking and bookingseats atomically
- [x] Return specific error messages for validation failures
- [x] BookingCode is null for pending bookings
- [x] Use DateTime.Now for PostgreSQL compatibility
- [x] Use execution strategy for transaction safety

### POST /api/bookings/{id}/add-combos ✅
- [x] Verify booking ownership
- [x] Only allow for "Pending" bookings
- [x] Update total amount correctly
- [x] Validate combo quantities > 0
- [x] Validate all combos exist
- [x] Use execution strategy for transaction safety

---

## 📝 Critical Field Name Reference

> **⚠️ IMPORTANT**: Dưới đây là các field names THỰC TẾ trong database. Phải dùng CHÍNH XÁC để tránh lỗi!

### Entity Field Names (Case-Sensitive)

#### Cinema
```csharp
✅ Cinemaid (int)
✅ Name (string)
✅ Address (string)
✅ Phone (string, nullable)
✅ City (string, nullable)
✅ Createdat (DateTime, nullable)
```

#### Auditorium
```csharp
✅ Auditoriumid (int)
✅ Cinemaid (int)
✅ Name (string)
✅ Seatscount (int) ← NOT "capacity"
```

#### Seat
```csharp
✅ Seatid (int)
✅ Auditoriumid (int)
✅ Row (string) ← Capital R
✅ Number (int) ← Capital N
✅ Type (string, nullable)
✅ Isavailable (bool, nullable)
```

#### Showtime
```csharp
✅ Showtimeid (int)
✅ Movieid (int)
✅ Auditoriumid (int)
✅ Starttime (DateTime)
✅ Endtime (DateTime, nullable)
✅ Price (decimal)
✅ Format (string)
✅ Languagetype (string)
```

#### Booking
```csharp
✅ Bookingid (int)
✅ Customerid (int) ← NOT "userid"
✅ Showtimeid (int)
✅ Voucherid (int, nullable)
✅ Bookingcode (string?, unique, NULLABLE) ← NULL until payment confirmed
✅ Bookingtime (DateTime, nullable) ← Use DateTime.Now not UtcNow
✅ Totalamount (decimal, nullable)
✅ Status (string, nullable) ← "Pending", "Confirmed", "Cancelled", etc.
```

#### Bookingseat
```csharp
✅ Bookingseatid (int)
✅ Bookingid (int)
✅ Showtimeid (int)
✅ Seatid (int)
✅ Seatprice (decimal) ← NOT "price"
```

#### Combo
```csharp
✅ Comboid (int)
✅ Name (string)
✅ Description (string, nullable)
✅ Price (decimal)
✅ Imageurl (string, nullable)
❌ NO "isavailable" field
```

#### Bookingcombo
```csharp
✅ Bookingcomboid (int)
✅ Bookingid (int)
✅ Comboid (int)
✅ Quantity (int)
✅ Comboprice (decimal, nullable) ← NOT "price"
```

### Common Mistakes to Avoid

❌ **DON'T USE**:
- `capacity` → Use `Seatscount`
- `row`, `number` → Use `Row`, `Number` (capital)
- `price` in Bookingseat → Use `Seatprice`
- `price` in Bookingcombo → Use `Comboprice`
- `userid` in Booking → Use `Customerid`
- `DateTime.UtcNow` → Use `DateTime.Now` (PostgreSQL timestamp without time zone)

✅ **DO USE**:
- Exact casing from entity classes
- `Seatprice` and `Comboprice` for junction tables
- `Seatscount` for auditorium capacity
- `Customerid` (from JWT → Customer table)
- `DateTime.Now` for PostgreSQL compatibility
- `CreateExecutionStrategy()` for transactions with retry enabled
- `string?` for nullable `Bookingcode` (null until payment confirmed)

---

## 🎉 Implementation Notes

### Critical Changes During Development

**1. BookingCode Logic** ✅
- Initial: Generated at booking creation
- Final: **NULL until payment confirmed**
- Entity: `public string? Bookingcode { get; set; }` (nullable)
- Reason: BookingCode only meaningful after payment

**2. DateTime Handling** ✅
- Issue: `Cannot write DateTime with Kind=UTC to PostgreSQL type 'timestamp without time zone'`
- Solution: Use `DateTime.Now` instead of `DateTime.UtcNow`
- Affected: All timestamp fields (Bookingtime, Starttime, etc.)

**3. Execution Strategy Pattern** ✅
- Issue: Retry strategy conflicts with manual transactions
- Solution: Wrap transactions with `CreateExecutionStrategy()`
```csharp
var strategy = _context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () => {
    using var transaction = await _context.Database.BeginTransactionAsync();
    // ... transaction code ...
});
```

**4. Detailed Error Messages** ✅
- Enhanced validation with specific error messages:
  - "Showtime not found"
  - "Showtime has already started"
  - "One or more seats not found"
  - "The following seats are already booked: A1, A2"
- Changed from generic "Failed to create booking" to specific exceptions

**5. Retry Configuration** ✅
```csharp
services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString, npgsqlOptions => {
    npgsqlOptions.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorCodesToAdd: null);
    npgsqlOptions.CommandTimeout(30);
}));
```

**6. Seat Locking Bug Fix** ✅ (November 5, 2025)
- **Critical Bug**: Seats remained `isAvailable = true` after booking creation
- **Impact**: Multiple users could book the same seat simultaneously
- **Root Cause**: `CreateBookingAsync` created `Bookingseat` records but never updated `Seat.Isavailable`
- **Solution**: Added seat locking in transaction:
  ```csharp
  foreach (var (seatid, seatprice) in seats)
  {
      var bookingSeat = new Bookingseat { /* ... */ };
      _context.Bookingseats.Add(bookingSeat);
      
      // 🔒 CRITICAL FIX: Mark seat as unavailable
      var seat = await _context.Seats.FindAsync(new object[] { seatid }, cancellationToken);
      if (seat != null)
      {
          seat.Isavailable = false;
          _context.Seats.Update(seat);
      }
  }
  ```
- **Files Modified**: `BookingRepository.cs` line 175-182

**7. Booking Cancellation Feature** ✅ (November 5, 2025)
- **Missing Feature**: No way to cancel pending bookings or release seats
- **Business Problem**: 
  - Users couldn't cancel unwanted bookings
  - Seats locked forever if user didn't complete payment
  - Poor user experience and wasted inventory
- **Solutions Implemented**:
  
  **a) Manual Cancel API** (`POST /api/bookings/{id}/cancel`):
  - User can cancel their own Pending bookings
  - Validates ownership and status
  - Releases seats atomically (sets `Isavailable = true`)
  - Returns cancelled booking details with released seat list
  
  **b) Auto-Cancel Background Service** (`BookingExpirationService`):
  - Runs every **5 minutes** continuously
  - Finds Pending bookings older than **15 minutes**
  - Auto-cancels expired bookings
  - Releases seats back to available pool
  - Logs all operations for monitoring
  
- **Business Rules**:
  - Only "Pending" bookings can be cancelled
  - User must own the booking (JWT validation)
  - Seats released immediately in same transaction
  - Auto-cancel timer: 15 minutes from `bookingtime`
  - Transaction safety with `ExecutionStrategy`

- **Files Created/Modified**:
  ```
  NEW: CancelBookingResponseDTO.cs
  NEW: BookingExpirationService.cs
  MOD: IBookingRepository.cs (added CancelBookingAndReleaseSeatsAsync, GetPendingBookingsOlderThanAsync)
  MOD: BookingRepository.cs (implemented cancel logic)
  MOD: IBookingService.cs (added CancelBookingAsync, AutoCancelExpiredBookingsAsync)
  MOD: BookingService.cs (implemented cancel business logic)
  MOD: BookingsController.cs (added POST {id}/cancel endpoint)
  MOD: Program.cs (registered background service)
  ```

---

**Created**: November 3, 2025  
**Last Updated**: November 5, 2025  
**Progress**: ✅ 11/11 endpoints (100%) - **COMPLETED**  
**Test File**: `tests/BookingFlow.http` ✅  
**Completion Date**: November 5, 2025

**Critical Fixes** (November 5, 2025):
- ✅ Seat locking bug (seats now locked on booking creation)
- ✅ Booking cancellation feature (manual + auto-cancel after 15 min)
- ✅ Background service for expired booking cleanup
