# 🎬 Screen 3: Movie Details & Reviews (4 Endpoints)

**Status**: ✅ **COMPLETED** (4/4 endpoints - 100%)  
**Assigned**: Trung

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Screen | Auth | Status | Assign |
|---|--------|----------|--------|------|--------|--------|
| 1 | GET | `/api/movies/{id}` | MovieDetailActivity | ❌ | ✅ DONE | Trung |
| 2 | GET | `/api/movies/{id}/showtimes` | MovieDetailActivity | ❌ | ✅ DONE | Trung |
| 3 | GET | `/api/reviews/movie/{movieId}` | MovieDetailActivity | ❌ | ✅ DONE | Trung |
| 4 | POST | `/api/reviews` | MovieDetailActivity | ✅ | ✅ DONE | Trung |

---

## 🎯 1. GET /api/movies/{id}

**Screen**: MovieDetailActivity, SelectCinemaActivity  
**Auth Required**: ❌ No

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Movie details retrieved successfully",
  "data": {
    "movieid": 1,
    "title": "The Avengers",
    "description": "Earth's mightiest heroes must come together and learn to fight as a team if they are going to stop the mischievous Loki and his alien army from enslaving humanity.",
    "durationminutes": 143,
    "director": "Joss Whedon",
    "trailerurl": "https://www.youtube.com/watch?v=eOrNdBpGMv8",
    "releasedate": "2012-05-04",
    "posterurl": "https://image.tmdb.org/t/p/w500/RYMX2wcKCBAr24UyPD7xwmjaTn.jpg",
    "country": "USA",
    "rating": "PG-13",
    "genre": "Action, Sci-Fi, Adventure",
    "averageRating": 4.5,
    "totalReviews": 128,
    "totalShowtimes": 15
  }
}
```

### Related Entities
**Movie** (movies table):
- ✅ `movieid` (int, PK)
- ✅ `title` (string, max 200)
- ✅ `description` (text, nullable)
- ✅ `durationminutes` (int) - NOT `duration`!
- ✅ `director` (string, max 100, nullable)
- ✅ `trailerurl` (string, max 255, nullable)
- ✅ `releasedate` (DateOnly, nullable)
- ✅ `posterurl` (string, max 255, nullable)
- ✅ `country` (string, max 100, nullable)
- ✅ `rating` (string, max 10) - Age rating (G, PG, PG-13, R)
- ✅ `genre` (string, max 255, nullable)

**Computed Fields**:
- `averageRating` - Calculate from Reviews.rating (average)
- `totalReviews` - Count of Reviews
- `totalShowtimes` - Count of active Showtimes

### Business Logic
- Return 404 if movie not found
- Calculate averageRating from all reviews (if any)
- Count total reviews
- Count showtimes with `starttime >= DateTime.UtcNow`

---

## 🎯 2. GET /api/movies/{id}/showtimes

**Screen**: MovieDetailActivity  
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
  "message": "Showtimes retrieved successfully",
  "data": [
    {
      "date": "2025-11-05",
      "cinemas": [
        {
          "cinemaid": 1,
          "name": "CGV Vincom Center",
          "address": "72 Le Thanh Ton, District 1",
          "city": "Ho Chi Minh City",
          "showtimes": [
            {
              "showtimeid": 42,
              "starttime": "2025-11-05T10:30:00",
              "endtime": "2025-11-05T12:53:00",
              "price": 100000,
              "format": "2D",
              "languagetype": "Phụ đề",
              "auditorium": {
                "auditoriumid": 5,
                "name": "Cinema 3",
                "capacity": 150
              },
              "availableSeats": 145
            },
            {
              "showtimeid": 43,
              "starttime": "2025-11-05T14:00:00",
              "endtime": "2025-11-05T16:23:00",
              "price": 120000,
              "format": "3D",
              "languagetype": "Lồng tiếng",
              "auditorium": {
                "auditoriumid": 6,
                "name": "Cinema 4",
                "capacity": 200
              },
              "availableSeats": 198
            }
          ]
        }
      ]
    }
  ]
}
```

### Related Entities
**Showtime** (showtimes table):
- ✅ `showtimeid` (int, PK)
- ✅ `movieid` (int, FK to movies)
- ✅ `auditoriumid` (int, FK to auditoriums)
- ✅ `starttime` (timestamp without time zone)
- ✅ `endtime` (timestamp without time zone, nullable)
- ✅ `price` (decimal(10,2))
- ✅ `format` (string, max 20) - "2D", "3D", "IMAX"
- ✅ `languagetype` (string, max 50) - "Phụ đề", "Lồng tiếng"

**Cinema** (cinemas table):
- ✅ `cinemaid` (int, PK)
- ✅ `name` (string, max 100)
- ✅ `address` (string, max 255)
- ✅ `city` (string, max 100, nullable)
- ❌ KHÔNG có: `latitude`, `longitude`, `imageurl`

**Auditorium** (auditoriums table):
- ✅ `auditoriumid` (int, PK)
- ✅ `cinemaid` (int, FK to cinemas)
- ✅ `name` (string, max 50)
- ✅ `capacity` (int)

**Computed Fields**:
- `availableSeats` - Calculate: Auditorium.capacity - COUNT(booked seats)

### Business Logic
- Filter showtimes by movieid
- Filter by date (default: today)
- Filter by cinemaid (optional)
- Only show future showtimes: `starttime >= DateTime.UtcNow`
- Group by date, then by cinema
- Calculate available seats: capacity - booked seats (Bookingseat where showtimeid + NOT cancelled)
- Sort: starttime ASC

---

## 🎯 3. GET /api/reviews/movie/{movieId}

**Screen**: MovieDetailActivity  
**Auth Required**: ❌ No

### Query Parameters
```
?page=1&pageSize=10&sort=latest
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| page | int | ❌ | Page number (default: 1) |
| pageSize | int | ❌ | Items per page (default: 10) |
| sort | string | ❌ | Sort by: `latest`, `oldest`, `highest`, `lowest` |

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Reviews retrieved successfully",
  "data": {
    "items": [
      {
        "reviewid": 5,
        "customerid": 3,
        "movieid": 1,
        "rating": 5,
        "comment": "Amazing movie! The action scenes were incredible and the story was engaging throughout.",
        "createdat": "2025-11-02T14:30:00",
        "customer": {
          "customerid": 3,
          "fullname": "John Doe",
          "gender": "Male"
        }
      },
      {
        "reviewid": 12,
        "customerid": 7,
        "movieid": 1,
        "rating": 4,
        "comment": "Great superhero movie. A bit long but worth watching.",
        "createdat": "2025-11-01T20:15:00",
        "customer": {
          "customerid": 7,
          "fullname": "Jane Smith",
          "gender": "Female"
        }
      }
    ],
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 13,
    "totalItems": 128,
    "averageRating": 4.5
  }
}
```

### Related Entities
**Review** (reviews table):
- ✅ `reviewid` (int, PK)
- ✅ `customerid` (int, FK to customers) - NOT `userid`!
- ✅ `movieid` (int, FK to movies)
- ✅ `rating` (int, nullable) - 1-5 stars
- ✅ `comment` (string, max 500, nullable)
- ✅ `createdat` (timestamp, nullable)
- ❌ KHÔNG có: `updatedat`

**Customer** (customers table):
- ✅ `customerid` (int, PK)
- ✅ `userid` (int, FK to users)
- ✅ Navigate to User for fullname

### Business Logic
- Filter reviews by movieid
- Include Customer and User info
- Sort options:
  - `latest`: createdat DESC (default)
  - `oldest`: createdat ASC
  - `highest`: rating DESC, createdat DESC
  - `lowest`: rating ASC, createdat DESC
- Calculate averageRating from all reviews

---

## 🎯 4. POST /api/reviews

**Screen**: MovieDetailActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "movieid": 1,
  "rating": 5,
  "comment": "Fantastic movie! Highly recommended for Marvel fans."
}
```

### Validation Rules
- `movieid`: Required, must exist in database
- `rating`: Required, must be between 1-5
- `comment`: Optional, max 500 characters

### Response 201 Created
```json
{
  "success": true,
  "statusCode": 201,
  "message": "Review created successfully",
  "data": {
    "reviewid": 156,
    "customerid": 3,
    "movieid": 1,
    "rating": 5,
    "comment": "Fantastic movie! Highly recommended for Marvel fans.",
    "createdat": "2025-11-03T15:45:00",
    "customer": {
      "customerid": 3,
      "fullname": "John Doe"
    }
  }
}
```

### Business Logic
1. Get userId from JWT token
2. Find Customer by userid
3. Check if customer already reviewed this movie (optional: allow only one review per user per movie)
4. Validate movieid exists
5. Create review with:
   - customerid (from Customer table)
   - movieid
   - rating (1-5)
   - comment (max 500 chars)
   - createdat = DateTime.UtcNow (as timestamp without time zone)

### Error Cases
- 401 Unauthorized - No valid token
- 404 Not Found - Movie doesn't exist
- 400 Bad Request - Invalid rating (not 1-5)
- 409 Conflict - User already reviewed this movie (optional)

---

## 📊 Implementation Summary

### Implementation Status

#### Domain Layer (Movie88.Domain/Models/)
```
✅ ReviewModel.cs          - Review entity mapping
✅ (MovieModel.cs)         - Already existed from Screen 2
✅ (ShowtimeModel.cs)      - Already existed from Screen 2
✅ (CinemaModel.cs)        - Already existed from Screen 2
✅ (AuditoriumModel.cs)    - Already existed from Screen 2
```

#### Application Layer (Movie88.Application/)
```
✅ DTOs/Movies/
   - MovieDetailDTO.cs (with computed fields: averageRating, totalReviews, totalShowtimes)

✅ DTOs/Showtimes/
   - ShowtimeDTO.cs (ShowtimesByDateDTO, ShowtimesByCinemaDTO, ShowtimeItemDTO)
   - Added: format, languagetype fields

✅ DTOs/Reviews/
   - ReviewDTO.cs (with CustomerInfoDTO)
   - CreateReviewRequestDTO.cs (validation: rating 1-5, comment max 500)
   - ReviewsPagedResultDTO.cs (with averageRating)

✅ Services/
   - IReviewService.cs / ReviewService.cs (pagination, sorting, duplicate check)
   - ShowtimeService.cs (grouping by date and cinema, available seats calculation)
   - MovieService.cs (updated GetByIdAsync to return MovieDetailDTO with computed fields)

✅ Mappers/
   - ReviewMapper.cs (Review ↔ ReviewDTO)
   - ShowtimeMapper.cs (Showtime → ShowtimeItemDTO with format and languagetype)
   - MovieMapper.cs (Movie → MovieDetailDTO with DateOnly→DateTime conversion)
```

#### Infrastructure Layer (Movie88.Infrastructure/)
```
✅ Repositories/
   - IReviewRepository.cs / ReviewRepository.cs (4 sort modes, pagination, average rating)
   - IShowtimeRepository.cs / ShowtimeRepository.cs (filtering, available seats)
   - (MovieRepository.GetByIdAsync already existed)
```

#### WebApi Layer (Movie88.WebApi/)
```
✅ Controllers/
   - MoviesController.cs (updated GetById, added GetShowtimes)
   - ReviewsController.cs (2 endpoints: GetReviews, CreateReview)
```

#### Tests
```
✅ tests/MovieDetails.http - 43 test cases covering all 4 endpoints
```

---

## 📝 Notes for Implementation

### Important Field Mappings

**Review Entity**:
- ⚠️ Use `customerid`, NOT `userid` directly
- ⚠️ `rating` is int (nullable), 1-5 stars
- ⚠️ `comment` max 500 characters
- ⚠️ `createdat` is timestamp without time zone
- ❌ NO `updatedat` field
- Navigate: Review → Customer → User (to get fullname)

**Showtime Entity**:
- ⚠️ `starttime`/`endtime` are timestamp without time zone
- ⚠️ Has `format` (2D/3D/IMAX) and `languagetype` (Phụ đề/Lồng tiếng)
- ⚠️ Calculate available seats: Auditorium.capacity - COUNT(Bookingseat)

**Movie Entity**:
- ⚠️ `rating` is STRING (age rating: G, PG, PG-13, R), NOT review score
- ⚠️ `durationminutes`, NOT `duration`
- ⚠️ Computed: averageRating from Reviews, totalReviews count

**Cinema Entity**:
- ❌ NO `latitude`, `longitude`, `imageurl` fields in database

### Business Logic Notes

**Available Seats Calculation**:
```csharp
// Get booked seats for specific showtime
var bookedSeatsCount = await _context.Bookingseats
    .Where(bs => bs.Showtimeid == showtimeId 
        && bs.Booking.Status != "Cancelled")
    .CountAsync();

var availableSeats = auditorium.Capacity - bookedSeatsCount;
```

**Review Creation**:
```csharp
// Get Customer from User
var userId = GetUserIdFromJwt(); // From JWT claims
var customer = await _context.Customers
    .FirstOrDefaultAsync(c => c.Userid == userId);

if (customer == null)
    return NotFound("Customer profile not found");

// Create review
var review = new Review
{
    Customerid = customer.Customerid,  // NOT userId!
    Movieid = request.Movieid,
    Rating = request.Rating,
    Comment = request.Comment,
    Createdat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
};
```

**Showtimes Grouping**:
```csharp
// Group by date, then by cinema
var groupedShowtimes = showtimes
    .GroupBy(s => s.Starttime.Date)
    .Select(dateGroup => new
    {
        Date = dateGroup.Key,
        Cinemas = dateGroup
            .GroupBy(s => s.Auditorium.Cinema)
            .Select(cinemaGroup => new
            {
                Cinema = cinemaGroup.Key,
                Showtimes = cinemaGroup.OrderBy(s => s.Starttime).ToList()
            })
            .ToList()
    })
    .ToList();
```

### PostgreSQL Specific
- timestamp without time zone for DateTime
- DateOnly for releasedate
- Use DateTime.SpecifyKind(..., DateTimeKind.Unspecified) for createdat

---

## 🧪 Testing Checklist

### GET /api/movies/{id}
- [x] Return 404 for non-existent movieid
- [x] Calculate correct averageRating
- [x] Count totalReviews correctly
- [x] Count only future showtimes
- [x] Fixed: DateOnly→DateTime conversion for releasedate
- [x] Fixed: Use durationminutes field (not duration)
- [x] Fixed: Removed non-existent fields (status, cast, language)

### GET /api/movies/{id}/showtimes
- [x] Default to today's date if not specified
- [x] Filter by cinemaid if provided
- [x] Only show future showtimes
- [x] Calculate available seats correctly
- [x] Group by date and cinema properly
- [x] Sort by starttime ASC
- [x] Added: format field (2D/3D/IMAX)
- [x] Added: languagetype field (Phụ đề/Lồng tiếng)

### GET /api/reviews/movie/{movieId}
- [x] Return empty list for movie with no reviews
- [x] Pagination works correctly
- [x] Sort options work (latest, oldest, highest, lowest)
- [x] Calculate averageRating
- [x] Include customer fullname
- [x] Use customerid (not userid directly)

### POST /api/reviews
- [x] Require authentication
- [x] Validate rating 1-5
- [x] Validate comment max 500 chars
- [x] Return 404 for invalid movieid
- [x] Use customerid from token
- [x] Set createdat correctly
- [x] Prevent duplicate reviews (409 Conflict)

---

## ✅ Implementation Summary

**Key Features Implemented:**
1. ✅ Movie details with computed fields (averageRating, totalReviews, totalShowtimes)
2. ✅ Showtimes grouped by date and cinema with available seats calculation
3. ✅ Reviews with pagination (10 items/page) and 4 sort modes (latest, oldest, highest, lowest)
4. ✅ Create review with authentication, validation, and duplicate prevention

**Bug Fixes Applied:**
1. ✅ Fixed AutoMapper DateOnly→DateTime conversion using `ToDateTime(TimeOnly.MinValue)`
2. ✅ Fixed MovieDetailDTO fields to match Entity (durationminutes, removed status/cast/language)
3. ✅ Added format and languagetype to ShowtimeItemDTO
4. ✅ Organized DTOs into subfolders (Movies/, Showtimes/, Reviews/, Bookings/)

**Test Coverage:**
- ✅ 43 test cases in tests/MovieDetails.http
- ✅ All 4 endpoints tested with various scenarios
- ✅ Integration workflow tested

---

**Created**: November 3, 2025  
**Last Updated**: November 3, 2025  
**Completed**: November 3, 2025  
**Progress**: ✅ 4/4 endpoints (100%)
