# 📋 Movie88 API Checklist - Theo Màn Hình

> Tài liệu này liệt kê API endpoints cần thiết cho từng màn hình Android.  
> **Chiến lược**: Xây dựng theo màn hình, không build toàn bộ 111 endpoints.  
> **Mục tiêu**: Hoàn thành từng screen một, test ngay, đảm bảo chất lượng.

---

## 📱 1. Authentication Screens (✅ COMPLETED)

### 🔐 LoginActivity & RegisterActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ✅ | POST | `/api/auth/login` | Đăng nhập với email & password | P0 |
| ✅ | POST | `/api/auth/register` | Đăng ký tài khoản mới (role=Customer) | P0 |
| ✅ | POST | `/api/auth/refresh-token` | Refresh JWT token khi hết hạn | P0 |
| ✅ | POST | `/api/auth/logout` | Đăng xuất, revoke refresh token | P0 |
| ✅ | POST | `/api/auth/change-password` | Đổi mật khẩu | P1 |
| ✅ | POST | `/api/auth/forgot-password` | Quên mật khẩu (placeholder) | P2 |

**Implementation Notes:**
- ✅ JWT Service với claims-based tokens
- ✅ BCrypt password hashing
- ✅ RefreshToken stored in `public.refresh_tokens`
- ✅ Clean Architecture với Domain Models
- ✅ Singapore region (port 5432 - Session pooler)

---

## 🏠 2. Home Screen (NOT STARTED)

### 🏠 HomeFragment
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/movies` | Danh sách phim (with pagination) | P0 |
| ❌ | GET | `/api/movies/now-showing` | Phim đang chiếu | P0 |
| ❌ | GET | `/api/movies/coming-soon` | Phim sắp chiếu | P0 |
| ❌ | GET | `/api/promotions/active` | Khuyến mãi đang hoạt động | P1 |
| ❌ | GET | `/api/movies/search` | Tìm kiếm phim theo keyword | P1 |

**Entities Required:**
- [ ] `Movie` (movieid, title, overview, posterurl, backdropurl, rating, genre, duration, releasedate)
- [ ] `Promotion` (promotionid, title, description, imageurl, startdate, enddate)

**DTOs Required:**
- [ ] `MovieResponseDTO`
- [ ] `MovieListResponseDTO` (with pagination)
- [ ] `PromotionResponseDTO`

**Controllers Required:**
- [ ] `MoviesController`
- [ ] `PromotionsController`

---

## 📜 3. Booking History Screen (NOT STARTED)

### 📖 BookingsFragment
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/bookings/my-bookings` | Lịch sử đặt vé của user | P0 |
| ❌ | GET | `/api/bookings/{id}` | Chi tiết 1 booking | P0 |

**Entities Required:**
- [ ] `Booking` (bookingid, userid, showtimeid, totalprice, status, createdat)
- [ ] `BookingDetail` (detailid, bookingid, seatid, price)
- [ ] `BookingCombo` (id, bookingid, comboid, quantity)

**DTOs Required:**
- [ ] `BookingResponseDTO`
- [ ] `BookingDetailResponseDTO`

**Controllers Required:**
- [ ] `BookingsController`

---

## 👤 4. Profile Screen (NOT STARTED)

### 👤 ProfileFragment & EditProfileActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/customers/profile` | Lấy profile khách hàng | P0 |
| ❌ | GET | `/api/users/me` | Lấy thông tin user hiện tại | P0 |
| ❌ | PUT | `/api/users/{id}` | Cập nhật thông tin user | P0 |
| ❌ | PUT | `/api/customers/profile` | Cập nhật profile khách hàng | P0 |
| ❌ | POST | `/api/users/avatar` | Upload avatar (multipart) | P2 |

**Entities Required:**
- [x] `User` (already exists)
- [ ] `Customer` (customerid, userid, dateofbirth, gender, loyaltypoints)

**DTOs Required:**
- [ ] `CustomerProfileResponseDTO`
- [ ] `UpdateProfileRequestDTO`

**Controllers Required:**
- [ ] `CustomersController`
- [ ] `UsersController`

---

## 🎬 5. Movie Details Screen (NOT STARTED)

### 🎥 MovieDetailActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/movies/{id}` | Chi tiết phim | P0 |
| ❌ | GET | `/api/movies/{id}/showtimes` | Suất chiếu của phim | P0 |
| ❌ | GET | `/api/reviews/movie/{movieId}` | Reviews của phim | P1 |
| ❌ | POST | `/api/reviews` | Tạo review mới | P1 |

**Entities Required:**
- [ ] `Movie` (same as Home)
- [ ] `Showtime` (showtimeid, movieid, auditoriumid, starttime, endtime, price)
- [ ] `Review` (reviewid, userid, movieid, rating, comment, createdat)

**DTOs Required:**
- [ ] `MovieDetailResponseDTO`
- [ ] `ShowtimeResponseDTO`
- [ ] `ReviewResponseDTO`
- [ ] `CreateReviewRequestDTO`

**Controllers Required:**
- [ ] `MoviesController` (extend)
- [ ] `ReviewsController`

---

## 🎟 6. Booking Flow Screens (NOT STARTED)

### 🏢 SelectCinemaActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/movies/{id}` | Thông tin phim | P0 |
| ❌ | GET | `/api/cinemas` | Danh sách rạp | P0 |
| ❌ | GET | `/api/showtimes/by-movie/{movieId}` | Suất chiếu theo phim | P0 |
| ❌ | GET | `/api/showtimes/by-date` | Suất chiếu theo ngày | P0 |

**Entities Required:**
- [ ] `Cinema` (cinemaid, name, address, city, latitude, longitude, phone)
- [ ] `Auditorium` (auditoriumid, cinemaid, name, capacity, type)

**DTOs Required:**
- [ ] `CinemaResponseDTO`
- [ ] `ShowtimeGroupedByCinemaDTO`

**Controllers Required:**
- [ ] `CinemasController`
- [ ] `ShowtimesController`

---

### 💺 SelectSeatActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/showtimes/{id}` | Chi tiết suất chiếu | P0 |
| ❌ | GET | `/api/showtimes/{id}/available-seats` | Ghế còn trống | P0 |
| ❌ | GET | `/api/auditoriums/{id}/seats` | Sơ đồ ghế phòng chiếu | P0 |
| ❌ | POST | `/api/bookings/create` | Tạo booking (chọn ghế) | P0 |

**Entities Required:**
- [ ] `Seat` (seatid, auditoriumid, rownumber, seatnumber, seattype, price)
- [ ] `BookingSeat` (junction table: bookingid, seatid)

**DTOs Required:**
- [ ] `SeatResponseDTO`
- [ ] `SeatMapResponseDTO`
- [ ] `CreateBookingRequestDTO`

**Controllers Required:**
- [ ] `ShowtimesController` (extend)
- [ ] `BookingsController` (extend)

---

### 🍿 SelectComboActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/combos` | Danh sách combo bắp nước | P0 |
| ❌ | POST | `/api/bookings/{id}/add-combos` | Thêm combo vào booking | P0 |

**Entities Required:**
- [ ] `Combo` (comboid, name, description, price, imageurl, isavailable)

**DTOs Required:**
- [ ] `ComboResponseDTO`
- [ ] `AddComboRequestDTO`

**Controllers Required:**
- [ ] `CombosController`

---

## 💳 7. Payment Screens (NOT STARTED)

### 💰 BookingSummaryActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/bookings/{id}` | Thông tin booking | P0 |
| ❌ | POST | `/api/vouchers/validate` | Validate voucher | P1 |
| ❌ | POST | `/api/bookings/{id}/apply-voucher` | Áp dụng voucher | P1 |
| ❌ | POST | `/api/payments/vnpay/create` | Tạo VNPay payment URL | P0 |

**Entities Required:**
- [ ] `Payment` (paymentid, bookingid, amount, paymentmethod, status, transactionid)
- [ ] `Voucher` (voucherid, code, discounttype, discountvalue, validfrom, validto)

**DTOs Required:**
- [ ] `BookingSummaryResponseDTO`
- [ ] `ValidateVoucherRequestDTO`
- [ ] `VNPayCreateRequestDTO`
- [ ] `VNPayCreateResponseDTO`

**Controllers Required:**
- [ ] `PaymentsController`
- [ ] `VouchersController`

---

### 🌐 VNPayWebViewActivity & PaymentResultActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/payments/vnpay/callback` | VNPay callback URL | P0 |
| ❌ | POST | `/api/payments/vnpay/ipn` | VNPay IPN notification | P0 |
| ❌ | PUT | `/api/payments/{id}/confirm` | Xác nhận thanh toán | P0 |
| ❌ | GET | `/api/bookings/{id}` | Chi tiết booking sau payment | P0 |

**DTOs Required:**
- [ ] `VNPayCallbackResponseDTO`
- [ ] `PaymentResultResponseDTO`

**Controllers Required:**
- [ ] `PaymentsController` (extend)

---

## 🔍 8. Search Screen (NOT STARTED - Optional)

### 🔍 SearchMovieActivity
| Status | Method | Endpoint | Mô tả | Priority |
|--------|--------|----------|-------|----------|
| ❌ | GET | `/api/movies/search` | Tìm kiếm theo keyword | P1 |
| ❌ | GET | `/api/movies` | Lọc phim (genre, year, rating) | P1 |

**Note**: Reuse `MoviesController` endpoints from Home screen.

---

## 📊 Progress Summary

| Screen Category | Endpoints | Completed | Remaining | Progress |
|----------------|-----------|-----------|-----------|----------|
| **Authentication** | 6 | 6 ✅ | 0 | 100% |
| **Home** | 5 | 0 | 5 | 0% |
| **Booking History** | 2 | 0 | 2 | 0% |
| **Profile** | 5 | 0 | 5 | 0% |
| **Movie Details** | 4 | 0 | 4 | 0% |
| **Select Cinema** | 4 | 0 | 4 | 0% |
| **Select Seat** | 4 | 0 | 4 | 0% |
| **Select Combo** | 2 | 0 | 2 | 0% |
| **Payment** | 7 | 0 | 7 | 0% |
| **Search** | 2 | 0 | 2 | 0% |
| **TOTAL** | **41** | **6** | **35** | **14.6%** |

---

## 🎯 Development Roadmap

### ✅ Phase 1: Authentication (COMPLETED)
- [x] Auth DTOs (Login, Register, RefreshToken, ChangePassword)
- [x] JWT Service
- [x] Password Hashing Service
- [x] AuthService với 6 methods
- [x] AuthController với 6 endpoints
- [x] Database: `public.refresh_tokens` table
- [x] Supabase Singapore region setup

### 🚀 Phase 2: Core Movie Features (NEXT)
**Priority: P0 - Critical for MVP**

#### 2.1 Home Screen
- [ ] Movie entity scaffolded
- [ ] Promotion entity scaffolded
- [ ] MoviesController (5 endpoints)
- [ ] PromotionsController (1 endpoint)
- [ ] Test: Get movies, now-showing, coming-soon

#### 2.2 Movie Details
- [ ] Showtime entity
- [ ] Review entity
- [ ] ShowtimesController (2 endpoints)
- [ ] ReviewsController (2 endpoints)
- [ ] Test: Movie details, reviews

### 🎫 Phase 3: Booking Flow
**Priority: P0 - Critical for revenue**

#### 3.1 Select Cinema & Showtime
- [ ] Cinema entity
- [ ] Auditorium entity
- [ ] CinemasController (2 endpoints)
- [ ] ShowtimesController extend (2 endpoints)
- [ ] Test: List cinemas, showtimes

#### 3.2 Select Seats
- [ ] Seat entity
- [ ] BookingSeat junction table
- [ ] Booking entity
- [ ] BookingsController (2 endpoints)
- [ ] Test: Seat map, create booking

#### 3.3 Select Combos
- [ ] Combo entity
- [ ] BookingCombo junction table
- [ ] CombosController (2 endpoints)
- [ ] Test: List combos, add to booking

### 💰 Phase 4: Payment Integration
**Priority: P0 - Critical for revenue**

#### 4.1 VNPay Integration
- [ ] Payment entity
- [ ] VNPay helper service
- [ ] PaymentsController (5 endpoints)
- [ ] Test: Create payment, callback, IPN

#### 4.2 Voucher System
- [ ] Voucher entity
- [ ] VouchersController (2 endpoints)
- [ ] Test: Validate, apply voucher

### 👤 Phase 5: User Management
**Priority: P1 - Important for UX**

#### 5.1 Profile Management
- [ ] Customer entity
- [ ] CustomersController (2 endpoints)
- [ ] UsersController (3 endpoints)
- [ ] Test: Get/update profile

#### 5.2 Booking History
- [ ] BookingsController extend (2 endpoints)
- [ ] Test: My bookings, booking details

### 🔍 Phase 6: Search & Filters (Optional)
**Priority: P2 - Nice to have**
- [ ] Search implementation
- [ ] Filter by genre, year, rating

---

## 📝 Implementation Notes

### Database Strategy
1. **Entities đã có từ scaffolding**: User, Role, RefreshToken (Supabase)
2. **Entities cần tạo mới**: 
   - Movie, Cinema, Auditorium, Seat
   - Showtime, Booking, BookingDetail
   - Combo, Payment, Voucher, Promotion, Review, Customer

### API Development Workflow
1. **Entity First**: Kiểm tra entity trong database
2. **DTOs**: Tạo Request/Response DTOs
3. **Repository**: Implement repository pattern
4. **Service**: Business logic
5. **Controller**: API endpoints
6. **Test**: Dùng Swagger UI test ngay

### Testing Strategy
- Test từng screen một
- Dùng Swagger UI (`https://localhost:5001/swagger`)
- Test cả success và error cases
- Test authentication với Bearer token

---

## 🔧 Technical Stack

- **.NET 8.0**: Web API
- **EF Core 8.0.11**: ORM
- **Npgsql 8.0.11**: PostgreSQL provider
- **Supabase**: Database (Singapore region)
- **JWT**: Authentication
- **BCrypt**: Password hashing
- **Clean Architecture**: Domain → Application → Infrastructure → WebApi

---

**Created**: November 3, 2025  
**Last Updated**: November 3, 2025  
**Next Target**: Phase 2 - Home Screen (Movies & Promotions)
