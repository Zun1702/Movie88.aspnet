# 🧪 Testing Guide - Screen by Screen

> Hướng dẫn test từng màn hình với Swagger UI và sample requests.

---

## 🔧 Setup

### 1. Start API
```powershell
cd Movie88.WebApi
dotnet run
```

### 2. Open Swagger UI
```
https://localhost:5001/swagger
```

### 3. Authenticate (for protected endpoints)
1. Click **"Authorize"** button (🔓 icon top-right)
2. Enter: `Bearer <your-jwt-token>`
3. Click **"Authorize"**

---

## ✅ Phase 1: Authentication Testing (COMPLETED)

### 1. Register New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "Test User",
  "email": "test@example.com",
  "password": "Test@123",
  "confirmPassword": "Test@123",
  "phoneNumber": "0123456789"
}
```

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "YbCh+SRX/nKWA5vkvL1T...",
  "expiresAt": "2025-11-03T12:37:50Z",
  "user": {
    "userId": 5,
    "fullName": "Test User",
    "email": "test@example.com",
    "phoneNumber": "0123456789",
    "roleId": 3,
    "roleName": "Customer"
  }
}
```

**Validations:**
- ✅ RoleId should be 3 (Customer)
- ✅ Returns JWT token immediately (auto-login)
- ✅ RefreshToken stored in database

---

### 2. Login Existing User
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Test@123"
}
```

**Expected Response:** Same as Register

**Test Cases:**
- ✅ Correct credentials → 200 OK
- ✅ Wrong password → 401 Unauthorized
- ✅ Non-existent email → 401 Unauthorized

---

### 3. Refresh Token
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "YbCh+SRX/nKWA5vkvL1T..."
}
```

**Expected Response:**
```json
{
  "token": "NEW_JWT_TOKEN",
  "refreshToken": "NEW_REFRESH_TOKEN",
  "expiresAt": "2025-11-03T13:37:50Z",
  "user": { ... }
}
```

**Validations:**
- ✅ Old refresh token revoked (Revoked=true in DB)
- ✅ New tokens generated
- ✅ Invalid/revoked token → 401 Unauthorized

---

### 4. Change Password
```http
POST /api/auth/change-password
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "oldPassword": "Test@123",
  "newPassword": "NewPass@456",
  "confirmPassword": "NewPass@456"
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

**Validations:**
- ✅ Requires authentication (401 if no token)
- ✅ Wrong old password → 401 Unauthorized
- ✅ All refresh tokens revoked after change
- ✅ Must login again with new password

---

### 5. Logout
```http
POST /api/auth/logout
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "refreshToken": "YbCh+SRX/nKWA5vkvL1T..."
}
```

**Expected Response:**
```json
{
  "message": "Logged out successfully"
}
```

**Validations:**
- ✅ Refresh token revoked (Revoked=true)
- ✅ JWT still valid until expiration (stateless)
- ✅ Cannot reuse revoked refresh token

---

### 6. Forgot Password
```http
POST /api/auth/forgot-password
Content-Type: application/json

{
  "email": "test@example.com"
}
```

**Expected Response:**
```json
{
  "message": "If the email exists, a password reset link has been sent"
}
```

**Notes:**
- ⚠️ Currently placeholder (returns true always)
- 🚧 TODO: Email service integration

---

## 🚀 Phase 2: Home Screen Testing (NEXT)

### 1. Get All Movies (with pagination)
```http
GET /api/movies?page=1&pageSize=10
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "movieId": 1,
        "title": "Movie Title",
        "overview": "Description...",
        "posterUrl": "https://...",
        "rating": 8.5,
        "genre": "Action",
        "duration": 120,
        "releaseDate": "2025-11-01"
      }
    ],
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalItems": 50
  }
}
```

---

### 2. Get Now Showing Movies
```http
GET /api/movies/now-showing
```

**Expected:** Movies with `releaseDate <= Today`

---

### 3. Get Coming Soon Movies
```http
GET /api/movies/coming-soon
```

**Expected:** Movies with `releaseDate > Today`

---

### 4. Search Movies
```http
GET /api/movies/search?query=avengers
```

**Expected:** Movies matching keyword in title/overview

---

### 5. Get Active Promotions
```http
GET /api/promotions/active
```

**Expected Response:**
```json
{
  "success": true,
  "data": [
    {
      "promotionId": 1,
      "title": "50% Off First Booking",
      "description": "...",
      "imageUrl": "https://...",
      "discountPercentage": 50,
      "startDate": "2025-11-01",
      "endDate": "2025-11-30"
    }
  ]
}
```

**Validations:**
- ✅ Only active promotions (startDate <= Today <= endDate)
- ✅ Sorted by startDate DESC

---

## 📋 Testing Checklist Template

### For Each New Screen:

#### Before Testing
- [ ] API is running
- [ ] Database tables exist
- [ ] Sample data inserted (if needed)
- [ ] Swagger UI updated

#### Test Cases
- [ ] ✅ Success case (200 OK)
- [ ] ❌ Validation errors (400 Bad Request)
- [ ] 🔒 Unauthorized (401) for protected endpoints
- [ ] 🔍 Not Found (404) for non-existent resources
- [ ] 🐛 Edge cases (empty lists, null values, etc.)

#### After Testing
- [ ] Document test results
- [ ] Update checklist
- [ ] Commit working code
- [ ] Move to next screen

---

## 🛠 Common Issues & Solutions

### Issue: "Npgsql.NpgsqlException: Timeout"
**Solution:** 
- Check connection string (use Session pooler port 5432)
- Increase timeout in connection string
- Check Supabase project status

### Issue: "401 Unauthorized"
**Solution:**
- Verify JWT token not expired
- Check "Authorization: Bearer <token>" header
- Refresh token if expired

### Issue: "Cannot write DateTime with Kind=UTC"
**Solution:**
- Use `DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)`
- Already fixed in AuthService helpers

### Issue: "User already exists"
**Solution:**
- Use different email for testing
- Or delete test user from database

---

## 📊 Test Results Log

### Phase 1: Authentication (Nov 3, 2025)
| Endpoint | Status | Notes |
|----------|--------|-------|
| POST /api/auth/register | ✅ Pass | RoleId=3 correct |
| POST /api/auth/login | ✅ Pass | Returns JWT + refresh |
| POST /api/auth/refresh-token | ✅ Pass | Safe parsing implemented |
| POST /api/auth/change-password | ✅ Pass | Revokes all tokens |
| POST /api/auth/logout | ✅ Pass | Revokes token |
| POST /api/auth/forgot-password | ✅ Pass | Placeholder works |

### Phase 2: Home Screen (Pending)
| Endpoint | Status | Notes |
|----------|--------|-------|
| GET /api/movies | ⏳ TODO | - |
| GET /api/movies/now-showing | ⏳ TODO | - |
| GET /api/movies/coming-soon | ⏳ TODO | - |
| GET /api/movies/search | ⏳ TODO | - |
| GET /api/promotions/active | ⏳ TODO | - |

---

**Last Updated**: November 3, 2025  
**Next Test Target**: Home Screen APIs
