# 🔗 API Endpoints to Screen Mapping Summary

## Tổng quan

Tài liệu này liệt kê chi tiết **TẤT CẢ** các API endpoints được sử dụng trong từng screen của ứng dụng Android.

**Tổng số endpoints**: 111 endpoints (từ backend)  
**Architecture**: 3-Layer (Repository - Service - Controller)  
**Payment Gateway**: VNPay only

---

## 📱 1. Authentication Screens

### SplashActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/refresh-token` | Refresh JWT token khi token hiện tại hết hạn | ✅ |

### LoginActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/login` | Đăng nhập với email và password, trả về JWT token | ❌ |

### RegisterActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/register` | Đăng ký tài khoản mới | ❌ |
### ForgotPasswordActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/forgot-password` | Quên mật khẩu | ❌ |
---

## 🏠 2. Main Screens

### HomeFragment
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/movies` | Lấy danh sách phim (with filters: page, pageSize) | ❌ |
| GET | `/api/movies/now-showing` | Lấy phim đang chiếu | ❌ |
| GET | `/api/movies/coming-soon` | Lấy phim sắp chiếu | ❌ |
| GET | `/api/promotions/active` | Lấy danh sách khuyến mãi đang hoạt động | ❌ |
| GET | `/api/movies/search` | Tìm kiếm phim theo keyword | ❌ |

### BookingsFragment
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/bookings/my-bookings` | Lấy lịch sử đặt vé của user | ✅ |
| GET | `/api/bookings/{id}` | Lấy chi tiết booking | ✅ |

### ProfileFragment
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/customers/profile` | Lấy thông tin profile khách hàng | ✅ |
| POST | `/api/auth/logout` | Đăng xuất | ✅ |

---

## 🎬 3. Movie Details Screens

### MovieDetailActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/movies/{id}` | Lấy thông tin chi tiết phim | ❌ |
| GET | `/api/movies/{id}/showtimes` | Lấy suất chiếu của phim (query: date) | ❌ |
| GET | `/api/reviews/movie/{movieId}` | Lấy danh sách reviews của phim | ❌ |
| POST | `/api/reviews` | Tạo review mới | ✅ |

### MovieTrailerActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| - | - | Không cần API (load YouTube URL từ Movie object) | - |

### SearchMovieActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/movies/search` | Tìm kiếm phim theo keyword (query: query) | ❌ |
| GET | `/api/movies` | Lấy danh sách phim với filters (genre, year, rating, sort) | ❌ |

---

## 🎫 4. Booking Screens

### SelectCinemaActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/movies/{id}` | Lấy thông tin chi tiết phim | ❌ |
| GET | `/api/cinemas` | Lấy danh sách rạp chiếu | ❌ |
| GET | `/api/showtimes/by-movie/{movieId}` | Lấy suất chiếu theo phim | ❌ |
| GET | `/api/showtimes/by-date` | Lấy suất chiếu theo ngày (query: date) | ❌ |

### SelectSeatActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/showtimes/{id}` | Lấy thông tin chi tiết suất chiếu | ❌ |
| GET | `/api/showtimes/{id}/available-seats` | Lấy danh sách ghế còn trống | ❌ |
| GET | `/api/auditoriums/{id}/seats` | Lấy sơ đồ ghế của phòng chiếu | ❌ |
| POST | `/api/bookings/create` | Tạo booking mới (sau khi chọn ghế) | ✅ |

### SelectComboActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/combos` | Lấy danh sách combo bắp nước | ❌ |
| POST | `/api/bookings/{id}/add-combos` | Thêm combo vào booking | ✅ |

---

## 💳 5. Payment Screens

### BookingSummaryActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/bookings/{id}` | Lấy thông tin booking | ✅ |
| POST | `/api/vouchers/validate` | Kiểm tra mã voucher hợp lệ | ✅ |
| POST | `/api/bookings/{id}/apply-voucher` | Áp dụng voucher vào booking | ✅ |
| POST | `/api/payments/vnpay/create` | Tạo payment URL của VNPay | ✅ |

### VNPayWebViewActivity -> nhúng đường link vào app
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/payments/vnpay/callback` | VNPay callback URL (auto-handled by WebView) | ❌ |

### PaymentResultActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/payments/vnpay/ipn` | IPN notification từ VNPay (auto by VNPay) | ❌ |
| PUT | `/api/payments/{id}/confirm` | Xác nhận thanh toán thành công | ✅ |
| GET | `/api/bookings/{id}` | Lấy chi tiết booking sau thanh toán | ✅ |

---

## 👤 6. Profile Management Screens

### ProfileFragment
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/users/me` | Lấy thông tin user hiện tại | ✅ |
| GET | `/api/customers/profile` | Lấy profile chi tiết khách hàng | ✅ |
| POST | `/api/auth/logout` | Đăng xuất và clear tokens | ✅ |

### EditProfileActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/users/me` | Lấy thông tin user để pre-fill form | ✅ |
| PUT | `/api/users/{id}` | Cập nhật thông tin user | ✅ |
| PUT | `/api/customers/profile` | Cập nhật profile khách hàng (alternative) | ✅ |

### Change Avatar
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/users/avatar` | Upload ảnh avatar mới (multipart) | ✅ |

### Booking History
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/bookings/my-bookings` | Lấy lịch sử đặt vé của user | ✅ |
| GET | `/api/customers/booking-history` | Alternative endpoint cho booking history | ✅ |

### Change Password
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/change-password` | Đổi mật khẩu (oldPassword + newPassword) | ✅ |

---
 
## ⚙️ 7. Settings Screen (optional - kịp thì mới làm)

### SettingsActivity
| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| - | - | Settings managed locally (SharedPreferences) | - |

**Note**: Settings screen không sử dụng API endpoints. Tất cả cài đặt (theme, language, notifications, offline mode) được lưu local qua `SharedPrefsManager`.

---

## 📊 Summary Statistics

### Endpoints by Screen Category

| Screen Category | Public Endpoints | Auth Required | Total |
|----------------|------------------|---------------|-------|
| Authentication | 2 | 1 | 3 |
| Main/Home | 4 | 2 | 6 |
| Movie Details | 3 | 1 | 4 |
| Booking Flow | 7 | 2 | 9 |
| Payment | 5 | 3 | 8 |
| Profile | 6 | 6 | 12 |
| **TOTAL** | **27** | **15** | **42** |

### Most Used Endpoints

1. `GET /api/movies/{id}` - Used in 3 screens (MovieDetail, SelectCinema, Booking)
2. `GET /api/bookings/{id}` - Used in 3 screens (BookingsFragment, BookingSummary, PaymentResult)
3. `GET /api/movies` - Used in 3 screens (Home, Movies, Search)
4. `POST /api/auth/logout` - Used in 2 screens (Profile, ProfileFragment)

### Authentication Requirements

- **Public Endpoints (no auth)**: 27 endpoints
  - Movie browsing, search, cinema list, showtimes
  - Payment callbacks (VNPay)
  
- **Auth Required**: 15 endpoints
  - User profile, booking creation/management
  - Payment creation, voucher application
  - Reviews creation
  - User data modification

---

## 🔐 Authentication Flow

### JWT Token Management

```java
// All authenticated requests include header:
Authorization: Bearer {JWT_TOKEN}

// Token format:
{
  "userId": 123,
  "email": "user@example.com",
  "role": "Customer",
  "exp": 1698643200  // Unix timestamp
}

// Token refresh:
POST /api/auth/refresh-token
Body: { "refreshToken": "..." }
Response: { "token": "new_jwt_token", "refreshToken": "new_refresh_token" }
```

### Interceptor Implementation

```java
public class AuthInterceptor implements Interceptor {
    @Override
    public Response intercept(Chain chain) throws IOException {
        Request original = chain.request();
        String token = SharedPrefsManager.getInstance().getToken();
        
        if (token != null) {
            Request request = original.newBuilder()
                .header("Authorization", "Bearer " + token)
                .build();
            return chain.proceed(request);
        }
        
        return chain.proceed(original);
    }
}
```

---

## 📝 API Request/Response Models

### Common Request Models

```java
// Login
class LoginRequest {
    String email;
    String password;
}

// Create Booking
class CreateBookingRequest {
    int showtimeId;
    List<Integer> seatIds;
}

// Apply Voucher
class ApplyVoucherRequest {
    String voucherCode;
}

// Create Review
class CreateReviewRequest {
    int movieId;
    int rating;  // 1-5
    String comment;
}

// Update Profile
class UpdateProfileRequest {
    String fullName;
    String phoneNumber;
    String dateOfBirth;  // yyyy-MM-dd
    String gender;  // Male/Female/Other
}
```

### Common Response Models

```java
// Generic API Response
class ApiResponse<T> {
    boolean success;
    int statusCode;
    String message;
    T data;
}

// Paged Result
class PagedResult<T> {
    List<T> items;
    int currentPage;
    int pageSize;
    int totalPages;
    int totalItems;
    boolean hasNextPage;
    boolean hasPreviousPage;
}

// Movie
class Movie {
    int movieId;
    String title;
    String overview;
    String posterUrl;
    String backdropUrl;
    String trailerUrl;
    double rating;
    String genre;
    int duration;
    String ageRating;
    Date releaseDate;
}

// Booking
class Booking {
    int bookingId;
    Movie movie;
    Cinema cinema;
    Showtime showtime;
    List<Seat> seats;
    List<String> seatNames;
    List<Combo> combos;
    Voucher appliedVoucher;
    double totalPrice;
    String status;  // Pending, Confirmed, Cancelled, Completed
    Date createdAt;
}
```

---

## 🚀 Best Practices

### 1. Error Handling

```java
// Handle API errors consistently
if (response.isSuccessful() && response.body() != null) {
    ApiResponse<Movie> apiResponse = response.body();
    if (apiResponse.isSuccess()) {
        // Success
        Movie movie = apiResponse.getData();
    } else {
        // API error
        showError(apiResponse.getMessage());
    }
} else {
    // HTTP error
    int statusCode = response.code();
    handleHttpError(statusCode);
}
```

### 2. Loading States

```java
// Show loading before API call
viewModel.getIsLoading().observe(this, isLoading -> {
    if (isLoading) {
        progressBar.setVisibility(View.VISIBLE);
    } else {
        progressBar.setVisibility(View.GONE);
    }
});
```

### 3. Caching Strategy

```java
// Use Room Database for offline support
// Cache duration: 30 minutes for movies
public void getMovies(ApiCallback<List<Movie>> callback) {
    if (NetworkUtils.isNetworkAvailable()) {
        // Fetch from API
        fetchMoviesFromApi(callback);
    } else {
        // Fetch from cache
        fetchMoviesFromCache(callback);
    }
}
```

---

**Last Updated**: October 29, 2025  
**API Version**: v1  
**Backend Total Endpoints**: 111  
**Frontend Used Endpoints**: 42
