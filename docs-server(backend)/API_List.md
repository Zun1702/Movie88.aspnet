# 📋 Movie88 - Danh sách API Đã Triển Khai

## 📖 Giới thiệu

Tài liệu này liệt kê **CÁC API ĐÃ TRIỂN KHAI** trong hệ thống Movie88 Backend.

**Backend Architecture**: Clean Architecture (Domain - Application - Infrastructure - WebApi)
**Database**: PostgreSQL (Supabase)
**Authentication**: JWT Bearer Token
**Deployment**: Railway.app

## 👥 Roles trong hệ thống

- **Customer**: Người dùng thông thường (đặt vé, xem phim, review)
- **Staff**: Nhân viên (xác thực booking tại quầy)
- **Admin**: Quản trị viên (quản lý toàn bộ hệ thống)

---

## 🔐 1. Authentication APIs

### Base URL: `/api/auth`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| POST | `/api/auth/register` | Đăng ký tài khoản mới | ❌ | Public | ✅ DONE |
| POST | `/api/auth/login` | Đăng nhập | ❌ | Public | ✅ DONE |
| POST | `/api/auth/logout` | Đăng xuất | ✅ | All | ✅ DONE |
| POST | `/api/auth/refresh-token` | Refresh JWT token | ✅ | All | ✅ DONE |

### Base URL: `/api/customers`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| GET | `/api/customers/profile` | Lấy profile khách hàng | ✅ | Customer | ✅ DONE |
| PUT | `/api/customers/profile` | Cập nhật profile | ✅ | Customer | ✅ DONE |

---

## 🎬 2. Movie APIs

### Base URL: `/api/movies`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| GET | `/api/movies` | Lấy danh sách phim (pagination, filters) | ❌ | Public | ✅ DONE |
| GET | `/api/movies/{id}` | Lấy chi tiết phim | ❌ | Public | ✅ DONE |
| GET | `/api/movies/now-showing` | Phim đang chiếu | ❌ | Public | ✅ DONE |
| GET | `/api/movies/coming-soon` | Phim sắp chiếu | ❌ | Public | ✅ DONE |
| GET | `/api/movies/search` | Tìm kiếm phim | ❌ | Public | ✅ DONE |
| GET | `/api/movies/{id}/showtimes` | Suất chiếu của phim | ❌ | Public | ✅ DONE |

---

## ⭐ 3. Review APIs

### Base URL: `/api/reviews`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| GET | `/api/reviews/movie/{movieId}` | Reviews của phim (pagination, sorting) | ❌ | Public | ✅ DONE |
| POST | `/api/reviews` | Tạo review (duplicate prevention) | ✅ | Customer | ✅ DONE |

---

## 🎟 4. Booking APIs

### Base URL: `/api/bookings`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| GET | `/api/bookings/my-bookings` | Bookings của tôi (pagination, status filter) | ✅ | Customer | ✅ DONE |

---

## 🎁 5. Promotion APIs

### Base URL: `/api/promotions`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| GET | `/api/promotions/active` | Khuyến mãi đang hoạt động | ❌ | Public | ✅ DONE |

---

## 🔧 6. System APIs

### Base URL: `/api/health`

| Method | Endpoint | Mô tả | Auth Required | Role | Status |
|--------|----------|-------|---------------|------|--------|
| GET | `/health` | Health check | ❌ | Public | ✅ DONE |

---

## 📊 Tổng kết Endpoints Đã Triển Khai

| Module | Endpoints Đã Triển Khai | Trạng thái |
|--------|-------------------------|------------|
| Authentication | 6 | ✅ Screen 1 - DONE |
| Movies | 6 | ✅ Screen 2 - DONE |
| Reviews | 2 | ✅ Screen 3 - DONE |
| Bookings | 1 | ✅ Screen 2 - DONE |
| Promotions | 1 | ✅ Screen 2 - DONE |
| System | 1 | ✅ DONE |
| **TỔNG CỘNG** | **17 endpoints** | **3 Screens Completed** |

## 🚀 Deployment Information

- **Production URL**: https://movie88aspnet-app.up.railway.app
- **Swagger UI**: https://movie88aspnet-app.up.railway.app/swagger
- **Health Check**: https://movie88aspnet-app.up.railway.app/health
- **GitHub**: https://github.com/Zun1702/Movie88.aspnet

---

## 📝 Quy ước chung

### Response Format

#### Success Response
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Operation successful",
  "data": { ... }
}
```

#### Error Response
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Error message",
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ]
}
```

### Pagination Format
```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalItems": 50
  }
}
```

### HTTP Status Codes
- `200` - OK (Success)
- `201` - Created
- `204` - No Content
- `400` - Bad Request
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not Found
- `409` - Conflict
- `422` - Unprocessable Entity
- `500` - Internal Server Error

---

**Last Updated**: October 29, 2025
**API Version**: v1
