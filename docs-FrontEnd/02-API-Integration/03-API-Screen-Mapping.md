# 🔗 Backend API → Frontend Screen Mapping

## Tổng quan

Document này map **104 Backend API endpoints** (từ docs/API_List.md) với **27 Frontend screens** để đảm bảo integration hoàn chỉnh.

---

## 📊 Mapping Summary

| Backend Module | Endpoints | Frontend Screens | Integration Status |
|----------------|-----------|------------------|-------------------|
| Authentication | 4 | 3 | ✅ 75% |
| Movies | 15 | 6 | 🟡 40% |
| Cinemas | 6 | 2 | 🟡 33% |
| Showtimes | 8 | 3 | 🟡 37% |
| Bookings | 14 | 6 | 🟡 43% |
| Combos | 4 | 1 | 🟡 25% |
| Payments | 8 | 3 | 🔴 0% |
| Vouchers | 7 | 2 | 🔴 0% |
| Reviews | 6 | 3 | 🔴 0% |
| Users | 8 | 5 | 🔴 0% |
| Notifications | 6 | 1 | 🔴 0% |
| **TOTAL** | **104** | **27** | **🟡 25%** |

---

## 1️⃣ Authentication APIs → Auth Screens

### Backend Endpoints (4)
```
POST   /api/auth/login
POST   /api/auth/register
POST   /api/auth/refresh-token
POST   /api/auth/change-password
```

### Frontend Screens (3) ✅
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **SplashScreen** | - Token validation (local)<br>- `POST /api/auth/refresh-token` | ✅ Done | 03-Screens/01-Auth.md |
| **LoginActivity** | `POST /api/auth/login` | ✅ Done | 03-Screens/01-Auth.md |
| **RegisterActivity** | `POST /api/auth/register` | 🟡 Layout only | 03-Screens/01-Auth.md |
| **ChangePasswordActivity** | `POST /api/auth/change-password` | ⏳ Pending | - |

### Implementation Details

#### SplashScreen
```java
// Check token expiry
if (TokenManager.getInstance().isTokenExpired()) {
    // Try refresh token
    authRepository.refreshToken(callback);
} else {
    // Token valid → Navigate to MainActivity
    navigateToMain();
}
```

#### LoginActivity
```java
// API Call
authRepository.login(new LoginRequest(email, password), new ApiCallback<LoginResponse>() {
    @Override
    public void onSuccess(LoginResponse response) {
        // Save token
        TokenManager.getInstance().saveToken(response.getToken());
        // Navigate to MainActivity
    }
});
```

---

## 2️⃣ Movie APIs → Movie Screens

### Backend Endpoints (15)
```
GET    /api/movies                    # List với filters
GET    /api/movies/{id}               # Chi tiết phim
GET    /api/movies/{id}/showtimes     # Lịch chiếu phim
GET    /api/movies/genres             # Danh sách thể loại
GET    /api/movies/now-showing        # Phim đang chiếu
GET    /api/movies/coming-soon        # Phim sắp chiếu
POST   /api/movies                    # [Admin] Tạo phim
PUT    /api/movies/{id}               # [Admin] Cập nhật
DELETE /api/movies/{id}               # [Admin] Xóa phim
... (6 admin endpoints)
```

### Frontend Screens (6)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **HomeFragment** | `GET /api/movies?status=NowShowing`<br>`GET /api/movies?status=ComingSoon` | ✅ Done | 03-Screens/02-Main.md |
| **MovieDetailActivity** | `GET /api/movies/{id}`<br>`GET /api/movies/{id}/showtimes` | ⏳ Pending | - |
| **SearchMovieActivity** | `GET /api/movies?search={query}&genre={genre}` | ⏳ Pending | - |
| **MovieListActivity** | `GET /api/movies?status={status}&page={n}` | ⏳ Pending | - |
| **TrailerPlayerActivity** | - (YouTube player, no API) | ⏳ Pending | - |
| **WriteReviewActivity** | `POST /api/reviews` | ⏳ Pending | - |

### Implementation Details

#### HomeFragment (✅ Done)
```java
// Load Now Showing movies
movieRepository.getMovies("NowShowing", null, null, 1, 10, 
    new ApiCallback<PagedResult<Movie>>() {
        @Override
        public void onSuccess(PagedResult<Movie> result) {
            nowShowingAdapter.setMovies(result.getItems());
        }
    });

// Load Coming Soon movies
movieRepository.getMovies("ComingSoon", null, null, 1, 10, callback);
```

#### MovieDetailActivity (⏳ Pending)
```java
// Load movie details
movieRepository.getMovieById(movieId, new ApiCallback<Movie>() {
    @Override
    public void onSuccess(Movie movie) {
        displayMovieDetails(movie);
        loadShowtimes(movie.getMovieId());
    }
});

// Load showtimes
movieRepository.getMovieShowtimes(movieId, null, null, callback);
```

---

## 3️⃣ Cinema APIs → Cinema Screens

### Backend Endpoints (6)
```
GET    /api/cinemas                   # Danh sách rạp
GET    /api/cinemas/{id}              # Chi tiết rạp
GET    /api/cinemas/{id}/auditoriums  # Các phòng chiếu
POST   /api/cinemas                   # [Admin] Tạo rạp
PUT    /api/cinemas/{id}              # [Admin] Cập nhật
DELETE /api/cinemas/{id}              # [Admin] Xóa rạp
```

### Frontend Screens (2)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **SelectCinemaActivity** | `GET /api/cinemas` | ⏳ Pending | - |
| **CinemaDetailDialog** | `GET /api/cinemas/{id}`<br>`GET /api/cinemas/{id}/auditoriums` | ⏳ Pending | - |

### Implementation Details

#### SelectCinemaActivity (⏳ Pending)
```java
// Load all cinemas
cinemaRepository.getAllCinemas(new ApiCallback<List<Cinema>>() {
    @Override
    public void onSuccess(List<Cinema> cinemas) {
        // Group by location
        Map<String, List<Cinema>> grouped = groupByLocation(cinemas);
        adapter.setData(grouped);
    }
});

// User selects cinema → Navigate to SelectShowtimeActivity
intent.putExtra("CINEMA_ID", selectedCinema.getCinemaId());
```

---

## 4️⃣ Showtime APIs → Showtime Screens

### Backend Endpoints (8)
```
GET    /api/showtimes                 # List lịch chiếu
GET    /api/showtimes/{id}            # Chi tiết suất chiếu
GET    /api/showtimes/{id}/seats      # Sơ đồ ghế
POST   /api/showtimes                 # [Admin] Tạo suất
PUT    /api/showtimes/{id}            # [Admin] Cập nhật
DELETE /api/showtimes/{id}            # [Admin] Xóa
GET    /api/showtimes/{id}/available-seats  # Ghế trống
PUT    /api/showtimes/{id}/seats      # [Admin] Update ghế
```

### Frontend Screens (3)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **SelectShowtimeActivity** | `GET /api/showtimes?movieId={id}&cinemaId={id}&date={date}` | ⏳ Pending | - |
| **SeatSelectionActivity** | `GET /api/showtimes/{id}/seats`<br>`POST /api/bookings/lock-seats` | ✅ Done | 03-Screens/03-Booking.md |
| **ShowtimeDetailDialog** | `GET /api/showtimes/{id}` | ⏳ Pending | - |

### Implementation Details

#### SeatSelectionActivity (✅ Done)
```java
// Load seat map
bookingRepository.getShowtimeSeats(showtimeId, new ApiCallback<List<Seat>>() {
    @Override
    public void onSuccess(List<Seat> seats) {
        seatAdapter.setSeats(seats);
    }
});

// Lock selected seats
bookingRepository.lockSeats(new LockSeatsRequest(showtimeId, seatIds), callback);
```

---

## 5️⃣ Booking APIs → Booking Screens

### Backend Endpoints (14)
```
POST   /api/bookings/lock-seats       # Khóa ghế (15 phút)
POST   /api/bookings/confirm          # Xác nhận đặt vé
GET    /api/bookings                  # Lịch sử đặt vé
GET    /api/bookings/{id}             # Chi tiết booking
PUT    /api/bookings/{id}/cancel      # Hủy vé
GET    /api/bookings/{id}/qr-code     # Lấy QR code
POST   /api/bookings/{id}/check-in    # Check-in tại rạp
GET    /api/bookings/upcoming         # Vé sắp tới
GET    /api/bookings/past             # Vé đã xem
... (5 admin endpoints)
```

### Frontend Screens (6)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **SelectCinemaActivity** | `GET /api/cinemas` | ⏳ Pending | - |
| **SelectShowtimeActivity** | `GET /api/showtimes?movieId={id}` | ⏳ Pending | - |
| **SeatSelectionActivity** | `GET /api/showtimes/{id}/seats`<br>`POST /api/bookings/lock-seats` | ✅ Done | 03-Screens/03-Booking.md |
| **ComboSelectionActivity** | `GET /api/combos` | ⏳ Pending | - |
| **BookingSummaryActivity** | `POST /api/bookings/confirm` | ⏳ Pending | - |
| **BookingHistoryFragment** | `GET /api/bookings?userId={id}` | ⏳ Pending | - |

### Implementation Details

#### BookingSummaryActivity (⏳ Pending)
```java
// Confirm booking
CreateBookingRequest request = new CreateBookingRequest(
    showtimeId, seatIds, comboIds, voucherCode
);

bookingRepository.confirmBooking(request, new ApiCallback<Booking>() {
    @Override
    public void onSuccess(Booking booking) {
        // Navigate to VNPayWebViewActivity for payment
        intent.putExtra("BOOKING_ID", booking.getBookingId());
        intent.putExtra("AMOUNT", booking.getTotalAmount());
        startActivity(intent);
    }
});
```

---

## 6️⃣ Payment APIs → Payment Screens

### Backend Endpoints (8) - **VNPay ONLY**
```
POST   /api/payments/vnpay/create     # Tạo payment URL
GET    /api/payments/vnpay/callback   # VNPay redirect
POST   /api/payments/vnpay/ipn        # VNPay IPN notification
POST   /api/payments/confirm          # Xác nhận thanh toán
POST   /api/payments/cancel           # Hủy thanh toán
POST   /api/payments/refund           # Hoàn tiền
GET    /api/payments                  # Lịch sử thanh toán
GET    /api/payments/{id}             # Chi tiết payment
```

### Frontend Screens (3)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **VNPayWebViewActivity** | `POST /api/payments/vnpay/create`<br>`GET /api/payments/vnpay/callback` | ⏳ Pending | - |
| **PaymentResultActivity** | `POST /api/payments/confirm`<br>`GET /api/bookings/{id}/qr-code` | ⏳ Pending | - |
| **PaymentHistoryActivity** | `GET /api/payments?userId={id}` | ⏳ Pending | - |

### Implementation Details

#### VNPayWebViewActivity (⏳ Pending)
```java
// Create VNPay payment
VNPayCreateRequest request = new VNPayCreateRequest(
    bookingId, amount, orderInfo, returnUrl
);

paymentRepository.createVNPayPayment(request, new ApiCallback<VNPayCreateResponse>() {
    @Override
    public void onSuccess(VNPayCreateResponse response) {
        // Load VNPay URL in WebView
        webView.loadUrl(response.getPaymentUrl());
    }
});

// Handle VNPay callback
webView.setWebViewClient(new WebViewClient() {
    @Override
    public boolean shouldOverrideUrlLoading(WebView view, String url) {
        if (url.contains("vnpay-callback")) {
            // Parse callback params
            Map<String, String> params = parseUrl(url);
            handleVNPayCallback(params);
            return true;
        }
        return false;
    }
});
```

---

## 7️⃣ Combo APIs → Combo Screens

### Backend Endpoints (4)
```
GET    /api/combos                    # Danh sách combo
GET    /api/combos/{id}               # Chi tiết combo
POST   /api/combos                    # [Admin] Tạo combo
PUT    /api/combos/{id}               # [Admin] Cập nhật
```

### Frontend Screens (1)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **ComboSelectionActivity** | `GET /api/combos` | ⏳ Pending | - |

### Implementation Details

#### ComboSelectionActivity (⏳ Pending)
```java
// Load combos
comboRepository.getAllCombos(new ApiCallback<List<Combo>>() {
    @Override
    public void onSuccess(List<Combo> combos) {
        comboAdapter.setCombos(combos);
    }
});

// User selects combo quantities
List<ComboItem> selectedCombos = getSelectedCombos();
intent.putExtra("COMBO_IDS", getComboIds(selectedCombos));
intent.putExtra("COMBO_TOTAL", calculateComboTotal(selectedCombos));
```

---

## 8️⃣ Voucher APIs → Voucher Screens

### Backend Endpoints (7)
```
GET    /api/vouchers                  # Voucher khả dụng
POST   /api/vouchers/validate         # Validate voucher code
GET    /api/vouchers/{code}           # Chi tiết voucher
POST   /api/vouchers/apply            # Áp dụng voucher
GET    /api/vouchers/user/{userId}    # Voucher của user
POST   /api/vouchers                  # [Admin] Tạo voucher
PUT    /api/vouchers/{id}             # [Admin] Cập nhật
```

### Frontend Screens (2)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **VoucherListActivity** | `GET /api/vouchers?userId={id}` | ⏳ Pending | - |
| **VoucherInputDialog** | `POST /api/vouchers/validate` | ⏳ Pending | - |

### Implementation Details

#### VoucherInputDialog (⏳ Pending)
```java
// Validate voucher code
ValidateVoucherRequest request = new ValidateVoucherRequest(
    voucherCode, userId, totalAmount
);

voucherRepository.validateVoucher(request, new ApiCallback<ValidateVoucherResponse>() {
    @Override
    public void onSuccess(ValidateVoucherResponse response) {
        // Show discount amount
        double discount = response.getDiscountAmount();
        tvDiscount.setText("-" + formatCurrency(discount));
        
        // Update total
        double newTotal = totalAmount - discount;
        tvFinalTotal.setText(formatCurrency(newTotal));
    }
});
```

---

## 9️⃣ Review APIs → Review Screens

### Backend Endpoints (6)
```
GET    /api/reviews/movie/{movieId}   # Reviews của phim
POST   /api/reviews                   # Tạo review
PUT    /api/reviews/{id}              # Sửa review
DELETE /api/reviews/{id}              # Xóa review
GET    /api/reviews/user/{userId}     # Reviews của user
GET    /api/reviews/{id}              # Chi tiết review
```

### Frontend Screens (3)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **MovieDetailActivity** (Reviews section) | `GET /api/reviews/movie/{movieId}` | ⏳ Pending | - |
| **WriteReviewActivity** | `POST /api/reviews` | ⏳ Pending | - |
| **MyReviewsActivity** | `GET /api/reviews/user/{userId}`<br>`PUT /api/reviews/{id}`<br>`DELETE /api/reviews/{id}` | ⏳ Pending | - |

---

## 🔟 User APIs → Profile Screens

### Backend Endpoints (8)
```
GET    /api/users/profile             # Thông tin user
PUT    /api/users/profile             # Cập nhật profile
POST   /api/users/avatar              # Upload avatar
GET    /api/users/{id}                # Chi tiết user
PUT    /api/users/{id}/role           # [Admin] Đổi role
DELETE /api/users/{id}                # [Admin] Xóa user
GET    /api/users                     # [Admin] List users
POST   /api/users/{id}/block          # [Admin] Block user
```

### Frontend Screens (5)
| Screen | APIs Used | Status | File |
|--------|-----------|--------|------|
| **ProfileFragment** | `GET /api/users/profile` | ⏳ Pending | - |
| **ProfileEditActivity** | `PUT /api/users/profile`<br>`POST /api/users/avatar` | ⏳ Pending | - |
| **ChangePasswordActivity** | `POST /api/auth/change-password` | ⏳ Pending | - |
| **BookingHistoryFragment** | `GET /api/bookings?userId={id}` | ⏳ Pending | - |
| **MyReviewsActivity** | `GET /api/reviews/user/{userId}` | ⏳ Pending | - |

---

## ⚠️ Critical Integration Points

### 1. JWT Token Flow
```
1. Login → Receive JWT token
2. Save token to SharedPreferences (TokenManager)
3. AuthInterceptor adds token to ALL requests
4. If 401 Unauthorized → Refresh token
5. If refresh fails → Clear token → Navigate to LoginActivity
```

### 2. Booking Flow (Multi-step)
```
1. SelectCinemaActivity → GET /api/cinemas
2. SelectShowtimeActivity → GET /api/showtimes
3. SeatSelectionActivity → GET /api/showtimes/{id}/seats + POST /api/bookings/lock-seats
4. ComboSelectionActivity → GET /api/combos
5. BookingSummaryActivity → POST /api/vouchers/validate + POST /api/bookings/confirm
6. VNPayWebViewActivity → POST /api/payments/vnpay/create
7. PaymentResultActivity → POST /api/payments/confirm + GET /api/bookings/{id}/qr-code
```

### 3. Data Caching Strategy
```
Repository Layer decides:
- Movies: Cache 5 minutes (Room Database)
- Showtimes: No cache (always fresh)
- Bookings: Cache user's bookings (Room)
- User Profile: Cache until update (SharedPreferences)
```

---

## 📊 Implementation Priority

### Phase 1: Core Authentication & Browsing (Week 1-2)
- ✅ Auth screens (Login, Register)
- ✅ HomeFragment (Now Showing, Coming Soon)
- ⏳ MovieDetailActivity

### Phase 2: Booking Flow (Week 3-5)
- ⏳ SelectCinemaActivity
- ⏳ SelectShowtimeActivity
- ✅ SeatSelectionActivity
- ⏳ ComboSelectionActivity
- ⏳ BookingSummaryActivity

### Phase 3: Payment Integration (Week 6)
- ⏳ VNPayWebViewActivity
- ⏳ PaymentResultActivity
- ⏳ QR Code generation

### Phase 4: Profile & History (Week 7)
- ⏳ ProfileFragment
- ⏳ BookingHistoryFragment
- ⏳ MyReviewsActivity

### Phase 5: Polish & Testing (Week 8)
- ⏳ Error handling
- ⏳ Offline support
- ⏳ UI/UX improvements

---

**Last Updated**: October 29, 2025  
**Backend API Version**: 3-Layer Architecture (Repository-Service-Controller)  
**Payment Gateway**: VNPay only
