# Booking Flow API Test Results

## Test Environment
- **Base URL**: `https://movie88aspnet-app.up.railway.app/api` (hoặc local)
- **Test Date**: 2025-11-04
- **Build Status**: ✅ SUCCESS

---

## Phase 1: Cinema & Showtime Selection (4 endpoints implemented)

### ✅ 1. GET /api/cinemas
**Status**: ✅ Implemented  
**Test Case**: Get all cinemas  
**Expected Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Cinemas retrieved successfully",
  "data": [
    {
      "cinemaid": 1,
      "name": "Cinema Name",
      "address": "Address",
      "phone": "Phone",
      "city": "City",
      "createdat": "2024-01-01T00:00:00"
    }
  ]
}
```
**Test Result**: ⏳ Pending

---

### ✅ 1.1. GET /api/cinemas?city={city}
**Status**: ✅ Implemented  
**Test Case**: Get cinemas filtered by city  
**Query Params**: `city=Ho Chi Minh City`  
**Expected Response**: Filtered list of cinemas  
**Test Result**: ⏳ Pending

---

### ✅ 2. GET /api/showtimes/by-movie/{movieId}
**Status**: ✅ Implemented  
**Test Case**: Get showtimes grouped by date and cinema for a specific movie  
**Path Params**: `movieId=1`  
**Expected Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Showtimes retrieved successfully",
  "data": {
    "movie": {
      "movieid": 1,
      "title": "Movie Title",
      "posterurl": "url",
      "durationminutes": 120,
      "rating": "PG-13"
    },
    "showtimesByDate": [
      {
        "date": "2025-11-05",
        "cinemas": [
          {
            "cinemaid": 1,
            "name": "Cinema Name",
            "address": "Address",
            "showtimes": [
              {
                "showtimeid": 42,
                "starttime": "2025-11-05T14:00:00",
                "price": 100000,
                "format": "2D",
                "languagetype": "Subtitled",
                "auditoriumName": "Hall 1",
                "availableSeats": 0
              }
            ]
          }
        ]
      }
    ]
  }
}
```
**Test Result**: ⏳ Pending

**Notes**: 
- ⚠️ `availableSeats` currently returns 0 (TODO: implement calculation)
- No filters supported (by design - returns ALL showtimes)

---

### ✅ 2.1. GET /api/movies/{id}/showtimes
**Status**: ✅ Implemented (Alternative endpoint)  
**Test Case**: Get showtimes via Movies controller  
**Path Params**: `movieId=1`  
**Expected Response**: Same as `/api/showtimes/by-movie/{movieId}`  
**Test Result**: ⏳ Pending

---

### ✅ 3. GET /api/showtimes/by-date
**Status**: ✅ Implemented  
**Test Case**: Get showtimes by date with optional filters  
**Query Params**: 
- `date=2025-11-05` (required)
- `cinemaid=1` (optional)
- `movieid=1` (optional)

**Expected Response**:
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
          "name": "Cinema Name",
          "address": "Address",
          "showtimes": [...]
        }
      ]
    }
  ]
}
```
**Test Result**: ⏳ Pending

**Test Variations**:
- 3.1 ⏳ By date only
- 3.2 ⏳ By date + cinema
- 3.3 ⏳ By date + movie
- 3.4 ⏳ By date + cinema + movie

---

### ✅ 4. GET /api/showtimes/{id}
**Status**: ✅ Implemented  
**Test Case**: Get detailed showtime information  
**Path Params**: `showtimeId=42`  
**Expected Response**:
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Showtime retrieved successfully",
  "data": {
    "showtimeid": 42,
    "movieid": 1,
    "auditoriumid": 5,
    "starttime": "2025-11-05T14:00:00",
    "endtime": "2025-11-05T16:00:00",
    "price": 100000,
    "format": "2D",
    "languagetype": "Subtitled",
    "availableSeats": 0,
    "movie": {
      "movieid": 1,
      "title": "Movie Title",
      "posterurl": "url",
      "durationminutes": 120,
      "rating": "PG-13"
    },
    "cinema": {
      "cinemaid": 1,
      "name": "Cinema Name",
      "address": "Address",
      "city": "City"
    },
    "auditorium": {
      "auditoriumid": 5,
      "name": "Hall 1",
      "seatscount": 100
    }
  }
}
```
**Test Result**: ⏳ Pending

**Error Cases**:
- ❌ Showtime not found (404)

---

## Phase 2: Seat Selection & Booking (Not Yet Implemented)

### ⏳ 5. GET /api/showtimes/{id}/available-seats
**Status**: ❌ Not Implemented  
**Priority**: HIGH (needed for seat selection UI)

### ⏳ 6. GET /api/auditoriums/{id}/seats
**Status**: ❌ Not Implemented  
**Priority**: HIGH (needed for seat map layout)

### ⏳ 7. POST /api/bookings/create
**Status**: ❌ Not Implemented  
**Priority**: CRITICAL (core booking transaction)

---

## Phase 3: Combo Selection (Not Yet Implemented)

### ⏳ 8. GET /api/combos
**Status**: ❌ Not Implemented

### ⏳ 9. POST /api/bookings/{id}/add-combos
**Status**: ❌ Not Implemented

---

## Progress Summary

| Phase | Endpoints | Status | Progress |
|-------|-----------|--------|----------|
| Phase 1 | 4 endpoints | ✅ Implemented | 100% |
| Phase 2 | 3 endpoints | ❌ Not Started | 0% |
| Phase 3 | 2 endpoints | ❌ Not Started | 0% |
| **Total** | **9 endpoints** | - | **44%** (4/9) |

**Note**: Original plan had 10 endpoints, but after implementation we have 9 unique endpoints (removed duplicate variations).

---

## Known Issues & TODOs

### 🐛 Issues
1. ⚠️ **availableSeats returns 0**: Need to implement actual seat counting logic
   - Currently `GetAvailableSeatsCountAsync` is implemented but not called in service layer
   - TODO: Update ShowtimeService to call repository method for each showtime

### 📝 TODOs
1. Implement Phase 2 endpoints (seat selection & booking)
2. Implement Phase 3 endpoints (combos)
3. Fix `availableSeats` calculation in ShowtimeService
4. Add authentication/authorization checks
5. Add input validation
6. Add error handling for edge cases

---

## Testing Instructions

### Using BookingFlow.http (VS Code REST Client)

1. **Start the API server**:
   ```bash
   dotnet run --project Movie88.WebApi\Movie88.WebApi.csproj
   ```

2. **Open** `tests/BookingFlow.http` in VS Code

3. **Update variables** at the top:
   ```http
   @baseUrl = http://localhost:5000/api  # or your deployed URL
   @movieId = 1
   @cinemaId = 1
   @showtimeId = 42
   ```

4. **Run tests** by clicking "Send Request" above each endpoint

5. **Verify responses** match expected structure

### Using Postman/Thunder Client

Import the `BookingFlow.http` file or manually create requests following the same structure.

---

## Next Steps

1. ✅ **Complete Phase 1** - DONE (4/4 endpoints)
2. ⏳ **Test Phase 1 endpoints** - Update this document with actual results
3. ⏳ **Fix availableSeats calculation**
4. ⏳ **Implement Phase 2** - Start with seat selection endpoints
5. ⏳ **Implement Phase 3** - Combo endpoints
6. ⏳ **Integration testing** - Test complete booking flow end-to-end

---

**Last Updated**: 2025-11-04  
**Tester**: Ready for manual testing  
**Build Status**: ✅ SUCCESS
