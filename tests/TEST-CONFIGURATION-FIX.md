# Test Configuration Fix Summary

## Issue
The newly created test files (`CinemaManagement.http` and `ShowtimeManagement.http`) were experiencing "socket hang up" errors while existing test files worked fine.

## Root Cause
The test files had incorrect baseUrl configurations:

**Incorrect Configuration:**
- `CinemaManagement.http`: `http://localhost:7238` (missing HTTPS and `/api`)
- `ShowtimeManagement.http`: `http://localhost:5148` (wrong port, missing HTTPS and `/api`)

**Correct Configuration (matching working tests):**
- Both files: `https://localhost:7238/api`

## Changes Applied

### 1. CinemaManagement.http
- ✅ Updated baseUrl from `http://localhost:7238` to `https://localhost:7238/api`
- ✅ Removed `/api` prefix from all endpoint paths (since baseUrl includes it)
- Example: `{{baseUrl}}/api/admin/cinemas` → `{{baseUrl}}/admin/cinemas`

### 2. ShowtimeManagement.http
- ✅ Updated baseUrl from `http://localhost:5148` to `https://localhost:7238/api`
- ✅ Removed `/api` prefix from all endpoint paths
- Example: `{{baseUrl}}/api/admin/showtimes` → `{{baseUrl}}/admin/showtimes`

## Test File Pattern

All test files now follow this consistent pattern:

```http
@baseUrl = https://localhost:7238/api

### Authentication
POST {{baseUrl}}/auth/login

### Admin Endpoints
POST {{baseUrl}}/admin/cinemas
GET {{baseUrl}}/admin/cinemas/{{cinemaId}}
PUT {{baseUrl}}/admin/cinemas/{{cinemaId}}
DELETE {{baseUrl}}/admin/cinemas/{{cinemaId}}
```

## Files Fixed
- ✅ `tests/CinemaManagement.http`
- ✅ `tests/ShowtimeManagement.http`

## Result
Both test files should now work correctly without "socket hang up" errors, matching the behavior of existing working test files like `UserProfile.http` and `Movies.http`.

## How to Use
1. Start the application: `dotnet run --project Movie88.WebApi`
2. Run the login test (section 1.1) to get an admin token
3. Copy the token to the `@adminToken` variable at the top
4. Execute any test request

## Environment URLs
- **Local Development**: `https://localhost:7238/api`
- **Railway Deployment**: `https://movie88aspnet-app.up.railway.app/api`
