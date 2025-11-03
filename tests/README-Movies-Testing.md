# 🧪 API Testing Guide - Movies Endpoints

## 📋 Overview

Hướng dẫn test các Movies APIs đã được implement.

---

## 🚀 Quick Start

### Option 1: REST Client (VS Code Extension)

1. **Install REST Client extension**
   - Mở VS Code Extensions (Ctrl+Shift+X)
   - Tìm "REST Client" by Huachao Mao
   - Click Install

2. **Run API server**
   ```bash
   cd Movie88.WebApi
   dotnet run
   ```

3. **Open test file**
   - Mở file: `tests/Movies.http`
   - Click "Send Request" phía trên mỗi request
   - Hoặc Ctrl+Alt+R để send request

### Option 2: PowerShell Script

1. **Run API server**
   ```bash
   cd Movie88.WebApi
   dotnet run
   ```

2. **Run test script**
   ```powershell
   cd tests
   .\Test-MoviesAPI.ps1
   ```

### Option 3: Swagger UI

1. **Run API server**
   ```bash
   cd Movie88.WebApi
   dotnet run
   ```

2. **Open Swagger**
   - Navigate to: https://localhost:7106/swagger
   - Click "Try it out" trên mỗi endpoint
   - Fill parameters và click "Execute"

---

## 📝 Test Cases

### 1. GET /api/movies - List All Movies

**Test scenarios:**
- ✅ Default pagination (page 1, pageSize 10)
- ✅ Custom pagination
- ✅ Filter by genre
- ✅ Filter by year
- ✅ Filter by rating
- ✅ Sort by releasedate_desc
- ✅ Sort by releasedate_asc
- ✅ Sort by title_asc
- ✅ Sort by title_desc
- ✅ Combined filters

**Example requests:**
```http
GET https://localhost:7106/api/movies
GET https://localhost:7106/api/movies?page=1&pageSize=5
GET https://localhost:7106/api/movies?genre=Action
GET https://localhost:7106/api/movies?year=2023
GET https://localhost:7106/api/movies?rating=PG-13
GET https://localhost:7106/api/movies?sort=releasedate_desc
```

**Expected response:**
```json
{
  "isSuccess": true,
  "message": "Movies retrieved successfully",
  "statusCode": 200,
  "data": {
    "items": [
      {
        "movieid": 1,
        "title": "The Avengers",
        "description": "Earth's mightiest heroes...",
        "durationminutes": 143,
        "director": "Joss Whedon",
        "trailerurl": "https://...",
        "releasedate": "2012-05-04",
        "posterurl": "https://...",
        "country": "USA",
        "rating": "PG-13",
        "genre": "Action, Sci-Fi, Adventure"
      }
    ],
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalItems": 48,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "errors": []
}
```

---

### 2. GET /api/movies/now-showing - Currently Showing

**Test scenarios:**
- ✅ Get now showing movies
- ✅ Pagination support

**Example requests:**
```http
GET https://localhost:7106/api/movies/now-showing
GET https://localhost:7106/api/movies/now-showing?page=1&pageSize=5
```

**Business logic:**
- Movies with `releasedate <= today`
- Movies có showtimes với `starttime >= now`

---

### 3. GET /api/movies/coming-soon - Upcoming Movies

**Test scenarios:**
- ✅ Get coming soon movies
- ✅ Pagination support

**Example requests:**
```http
GET https://localhost:7106/api/movies/coming-soon
GET https://localhost:7106/api/movies/coming-soon?page=1&pageSize=5
```

**Business logic:**
- Movies với `releasedate > today`
- OR movies chưa có showtimes trong tương lai

---

### 4. GET /api/movies/search - Search Movies

**Test scenarios:**
- ✅ Search by title
- ✅ Search by director
- ✅ Search by genre
- ✅ Search by description
- ✅ Case-insensitive search
- ✅ Pagination support
- ❌ Empty query returns 400
- ❌ Missing query returns 400

**Example requests:**
```http
GET https://localhost:7106/api/movies/search?query=Avengers
GET https://localhost:7106/api/movies/search?query=Christopher%20Nolan
GET https://localhost:7106/api/movies/search?query=Action
GET https://localhost:7106/api/movies/search?query=Marvel&page=1&pageSize=5
```

**Error cases:**
```http
GET https://localhost:7106/api/movies/search?query=
# Expected: 400 Bad Request

GET https://localhost:7106/api/movies/search
# Expected: 400 Bad Request
```

---

### 5. GET /api/movies/{id} - Get Movie Details

**Test scenarios:**
- ✅ Get movie by valid ID
- ❌ Invalid ID returns 404

**Example requests:**
```http
GET https://localhost:7106/api/movies/1
GET https://localhost:7106/api/movies/2
GET https://localhost:7106/api/movies/99999  # Should return 404
```

**Expected response:**
```json
{
  "isSuccess": true,
  "message": "Movie retrieved successfully",
  "statusCode": 200,
  "data": {
    "movieid": 1,
    "title": "The Avengers",
    "description": "Earth's mightiest heroes...",
    "durationminutes": 143,
    "director": "Joss Whedon",
    "trailerurl": "https://...",
    "releasedate": "2012-05-04",
    "posterurl": "https://...",
    "country": "USA",
    "rating": "PG-13",
    "genre": "Action, Sci-Fi, Adventure"
  },
  "errors": []
}
```

---

## ⚠️ Prerequisites

### 1. Database có dữ liệu test

Cần có ít nhất:
- ✅ 10+ movies cho pagination test
- ✅ Movies với genres khác nhau (Action, Comedy, Drama)
- ✅ Movies với years khác nhau (2020-2024)
- ✅ Movies với ratings khác nhau (G, PG, PG-13, R)
- ✅ Movies có showtimes (cho now-showing test)
- ✅ Movies có releasedate > today (cho coming-soon test)

### 2. API Server đang chạy

```bash
cd Movie88.WebApi
dotnet run
```

Check API:
- HTTPS: https://localhost:7106/swagger
- HTTP: http://localhost:5106/swagger

### 3. Update baseUrl nếu cần

Nếu API chạy trên port khác:
- Sửa `@baseUrl` trong `Movies.http`
- Sửa `$baseUrl` trong `Test-MoviesAPI.ps1`

---

## 🔍 Verification Checklist

Sau khi chạy tests, verify:

### Pagination
- [ ] Default page 1, pageSize 10
- [ ] Custom pageSize hoạt động
- [ ] `hasNextPage` và `hasPreviousPage` đúng
- [ ] `totalPages` và `totalItems` chính xác

### Filtering
- [ ] Genre filter works
- [ ] Year filter works
- [ ] Rating filter works
- [ ] Combined filters work

### Sorting
- [ ] releasedate_desc (newest first)
- [ ] releasedate_asc (oldest first)
- [ ] title_asc (A-Z)
- [ ] title_desc (Z-A)

### Now Showing
- [ ] Only movies with active showtimes
- [ ] Released movies only

### Coming Soon
- [ ] Future release dates
- [ ] OR no active showtimes

### Search
- [ ] Tìm được bằng title
- [ ] Tìm được bằng director
- [ ] Tìm được bằng genre
- [ ] Tìm được bằng description
- [ ] Case-insensitive
- [ ] Empty query returns 400

### Movie Details
- [ ] Valid ID returns movie
- [ ] Invalid ID returns 404
- [ ] All fields populated correctly

---

## 📊 Expected Results

### Success Response Structure
```json
{
  "isSuccess": true,
  "message": "...",
  "statusCode": 200,
  "data": { ... },
  "errors": []
}
```

### Error Response Structure
```json
{
  "isSuccess": false,
  "message": "Movie not found",
  "statusCode": 404,
  "data": null,
  "errors": []
}
```

---

## 🐛 Troubleshooting

### Issue: Connection refused
**Solution**: Đảm bảo API server đang chạy (`dotnet run`)

### Issue: 404 Not Found on all requests
**Solution**: Check baseUrl có đúng port không

### Issue: 500 Internal Server Error
**Solution**: 
1. Check console logs
2. Verify database connection
3. Check migrations đã chạy chưa

### Issue: Empty results
**Solution**: 
1. Verify database có data
2. Check filter parameters
3. Verify showtimes data (cho now-showing test)

---

## 📈 Next Steps

Sau khi test Movies APIs xong, tiếp tục với:

1. **PromotionsController** (1 endpoint)
   - GET `/api/promotions/active`

2. **CustomersController** (1 endpoint)
   - GET `/api/customers/profile`

3. **BookingsController** (2 endpoints)
   - GET `/api/bookings/my-bookings`
   - GET `/api/bookings/{id}`

---

**Created**: November 3, 2025  
**Status**: ✅ Movies APIs Ready for Testing
