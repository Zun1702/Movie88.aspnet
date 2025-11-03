# 📱 Movie88 - Frontend Android App Overview

## 📋 Giới thiệu

**Movie88 Android App** là ứng dụng di động cho hệ thống đặt vé xem phim, được phát triển bằng **Android Studio** với **XML Layouts** và **Retrofit** để tích hợp API từ backend.

## 🎯 Mục tiêu

- ✅ Xây dựng giao diện người dùng trực quan, dễ sử dụng
- ✅ Tích hợp đầy đủ API từ backend ASP.NET Core
- ✅ Hỗ trợ thanh toán VNPay qua WebView
- ✅ Real-time seat selection với interactive seat map
- ✅ Offline caching cho trải nghiệm mượt mà
- ✅ Push notifications cho booking confirmation

## 🛠 Technology Stack

| Thành phần | Công nghệ |
|-----------|-----------|
| Language | Java / Kotlin |
| IDE | Android Studio |
| UI Framework | XML Layouts |
| API Client | Retrofit 2 |
| Image Loading | Glide / Picasso |
| Storage | SharedPreferences, Room Database |
| Authentication | JWT Token |
| Payment Integration | WebView (VNPay) |
| Min SDK | API 24 (Android 7.0) |
| Target SDK | API 34 (Android 14) |

## 🏗 Kiến trúc Android App

### MVVM Architecture

```
┌─────────────────────────────────────────────┐
│              VIEW LAYER                     │
│        (Activities & Fragments)             │
│                                             │
│  • XML Layouts                              │
│  • UI Logic                                 │
│  • User Interactions                        │
│  • Observe ViewModel LiveData               │
└────────────────┬────────────────────────────┘
                 │
                 │ Observe LiveData
                 │
                 ↓
┌─────────────────────────────────────────────┐
│           VIEWMODEL LAYER                   │
│                                             │
│  • Business Logic                           │
│  • LiveData / StateFlow                     │
│  • Call Repository                          │
│  • Handle UI State                          │
└────────────────┬────────────────────────────┘
                 │
                 │ Call Methods
                 │
                 ↓
┌─────────────────────────────────────────────┐
│          REPOSITORY LAYER                   │
│                                             │
│  • API Calls (Retrofit)                     │
│  • Local Database (Room)                    │
│  • Cache Management                         │
│  • Data Mapping                             │
└────────────────┬────────────────────────────┘
                 │
                 │ HTTP Requests
                 │
                 ↓
         ┌───────────────┐
         │  Backend API  │
         │  (ASP.NET)    │
         └───────────────┘
```

## 📱 Danh sách Màn hình (Screens)

### 1. 🔐 Authentication Module

| Screen | Mô tả | API Endpoints |
|--------|-------|--------------|
| **SplashScreen** | Màn hình chào, check login | - |
| **LoginActivity** | Đăng nhập | POST `/api/auth/login` |
| **RegisterActivity** | Đăng ký tài khoản | POST `/api/auth/register` |
| **ForgotPasswordActivity** | Quên mật khẩu | POST `/api/auth/forgot-password` |

### 2. 🏠 Main Module

| Screen | Mô tả | API Endpoints |
|--------|-------|--------------|
| **MainActivity** | Container với BottomNavigation | - |
| **HomeFragment** | Trang chủ: Now Showing, Coming Soon | GET `/api/movies?status=NowShowing`<br>GET `/api/movies?status=ComingSoon` |
| **CinemasFragment** | Danh sách rạp | GET `/api/cinemas`<br>GET `/api/cinemas/nearby` |
| **BookingsFragment** | Lịch sử đặt vé | GET `/api/customers/booking-history` |
| **ProfileFragment** | Thông tin cá nhân | GET `/api/customers/profile` |

### 3. 🎬 Movie Module

| Screen | Mô tả | API Endpoints |
|--------|-------|--------------|
| **MovieDetailActivity** | Chi tiết phim, trailer | GET `/api/movies/{id}`<br>GET `/api/reviews/movie/{id}` |
| **MovieSearchActivity** | Tìm kiếm phim | GET `/api/movies/search?keyword={keyword}` |
| **ReviewsActivity** | Danh sách đánh giá | GET `/api/reviews/movie/{id}` |
| **WriteReviewActivity** | Viết đánh giá | POST `/api/reviews` |

### 4. 🎟 Booking Module

| Screen | Mô tả | API Endpoints |
|--------|-------|--------------|
| **SelectCinemaActivity** | Chọn rạp và suất chiếu | GET `/api/showtimes/movie/{id}` |
| **SelectShowtimeActivity** | Chọn giờ chiếu | GET `/api/showtimes/by-movie-cinema` |
| **SeatSelectionActivity** | Chọn ghế (Interactive) | GET `/api/showtimes/{id}/seats`<br>POST `/api/bookings/lock-seats` |
| **ComboSelectionActivity** | Chọn combo bắp nước | GET `/api/combos` |
| **BookingSummaryActivity** | Tóm tắt booking | GET `/api/vouchers/validate`<br>POST `/api/bookings` |
| **BookingDetailActivity** | Chi tiết booking + QR | GET `/api/bookings/{id}` |

### 5. 💳 Payment Module

| Screen | Mô tả | API Endpoints |
|--------|-------|--------------|
| **PaymentMethodActivity** | Chọn VNPay | - |
| **VNPayWebViewActivity** | WebView thanh toán VNPay | POST `/api/payments/vnpay/create` |
| **PaymentResultActivity** | Kết quả thanh toán | GET `/api/payments/{id}` |

### 6. 👤 Profile Module

| Screen | Mô tả | API Endpoints |
|--------|-------|--------------|
| **ProfileEditActivity** | Chỉnh sửa profile | PUT `/api/customers/profile` |
| **ChangePasswordActivity** | Đổi mật khẩu | POST `/api/auth/change-password` |
| **PaymentHistoryActivity** | Lịch sử thanh toán | GET `/api/customers/payment-history` |
| **MyReviewsActivity** | Đánh giá của tôi | GET `/api/reviews/my-reviews` |
| **VouchersActivity** | Voucher của tôi | GET `/api/vouchers/available` |

## 📂 Project Structure

```
app/
├── src/main/
│   ├── java/com/movie88/
│   │   ├── ui/
│   │   │   ├── auth/
│   │   │   │   ├── LoginActivity.java
│   │   │   │   ├── RegisterActivity.java
│   │   │   │   └── LoginViewModel.java
│   │   │   ├── home/
│   │   │   │   ├── HomeFragment.java
│   │   │   │   └── HomeViewModel.java
│   │   │   ├── movie/
│   │   │   │   ├── MovieDetailActivity.java
│   │   │   │   ├── MovieDetailViewModel.java
│   │   │   │   └── adapters/
│   │   │   ├── booking/
│   │   │   │   ├── SeatSelectionActivity.java
│   │   │   │   ├── SeatSelectionViewModel.java
│   │   │   │   └── adapters/
│   │   │   ├── payment/
│   │   │   │   ├── VNPayWebViewActivity.java
│   │   │   │   └── PaymentViewModel.java
│   │   │   └── profile/
│   │   │       └── ProfileFragment.java
│   │   ├── data/
│   │   │   ├── api/
│   │   │   │   ├── ApiService.java
│   │   │   │   ├── RetrofitClient.java
│   │   │   │   └── interceptors/
│   │   │   │       └── AuthInterceptor.java
│   │   │   ├── repository/
│   │   │   │   ├── MovieRepository.java
│   │   │   │   ├── BookingRepository.java
│   │   │   │   └── PaymentRepository.java
│   │   │   ├── local/
│   │   │   │   ├── database/
│   │   │   │   │   └── AppDatabase.java
│   │   │   │   └── dao/
│   │   │   │       └── MovieDao.java
│   │   │   └── models/
│   │   │       ├── Movie.java
│   │   │       ├── Booking.java
│   │   │       ├── Seat.java
│   │   │       └── responses/
│   │   │           └── ApiResponse.java
│   │   └── utils/
│   │       ├── Constants.java
│   │       ├── SharedPrefsManager.java
│   │       ├── DateUtils.java
│   │       └── ImageLoader.java
│   └── res/
│       ├── layout/
│       │   ├── activity_login.xml
│       │   ├── activity_main.xml
│       │   ├── activity_movie_detail.xml
│       │   ├── activity_seat_selection.xml
│       │   ├── fragment_home.xml
│       │   ├── item_movie.xml
│       │   ├── item_seat.xml
│       │   └── ...
│       ├── drawable/
│       │   ├── bg_button_primary.xml
│       │   ├── ic_seat_available.xml
│       │   ├── ic_seat_selected.xml
│       │   └── ...
│       ├── values/
│       │   ├── colors.xml
│       │   ├── strings.xml
│       │   ├── styles.xml
│       │   └── dimens.xml
│       └── menu/
│           └── bottom_navigation_menu.xml
└── build.gradle
```

## 🔗 API Integration với Retrofit

### Dependencies (build.gradle)

```gradle
dependencies {
    // AndroidX
    implementation 'androidx.appcompat:appcompat:1.6.1'
    implementation 'androidx.constraintlayout:constraintlayout:2.1.4'
    implementation 'androidx.recyclerview:recyclerview:1.3.2'
    implementation 'androidx.cardview:cardview:1.0.0'
    implementation 'androidx.viewpager2:viewpager2:1.0.0'
    implementation 'com.google.android.material:material:1.11.0'
    
    // Architecture Components
    implementation 'androidx.lifecycle:lifecycle-viewmodel:2.7.0'
    implementation 'androidx.lifecycle:lifecycle-livedata:2.7.0'
    implementation 'androidx.navigation:navigation-fragment:2.7.6'
    implementation 'androidx.navigation:navigation-ui:2.7.6'
    
    // Retrofit
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'
    
    // Image Loading
    implementation 'com.github.bumptech.glide:glide:4.16.0'
    annotationProcessor 'com.github.bumptech.glide:compiler:4.16.0'
    
    // Room Database
    implementation 'androidx.room:room-runtime:2.6.1'
    annotationProcessor 'androidx.room:room-compiler:2.6.1'
    
    // Coroutines
    implementation 'org.jetbrains.kotlinx:kotlinx-coroutines-android:1.7.3'
    
    // QR Code
    implementation 'com.google.zxing:core:3.5.2'
    implementation 'com.journeyapps:zxing-android-embedded:4.3.0'
}
```

## 🎨 Design System

### Colors (colors.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <!-- Primary Colors -->
    <color name="colorPrimary">#E50914</color>
    <color name="colorPrimaryDark">#B2070F</color>
    <color name="colorAccent">#FFD700</color>
    
    <!-- Background -->
    <color name="backgroundColor">#141414</color>
    <color name="cardBackground">#1F1F1F</color>
    
    <!-- Text -->
    <color name="textPrimary">#FFFFFF</color>
    <color name="textSecondary">#B3B3B3</color>
    <color name="textTertiary">#808080</color>
    
    <!-- Seat Colors -->
    <color name="seatAvailable">#4CAF50</color>
    <color name="seatSelected">#E50914</color>
    <color name="seatOccupied">#757575</color>
    <color name="seatVip">#FFD700</color>
    <color name="seatCouple">#FF69B4</color>
    
    <!-- Status -->
    <color name="statusSuccess">#4CAF50</color>
    <color name="statusWarning">#FF9800</color>
    <color name="statusError">#F44336</color>
    <color name="statusInfo">#2196F3</color>
</resources>
```

### Dimensions (dimens.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <!-- Spacing -->
    <dimen name="spacing_xs">4dp</dimen>
    <dimen name="spacing_sm">8dp</dimen>
    <dimen name="spacing_md">16dp</dimen>
    <dimen name="spacing_lg">24dp</dimen>
    <dimen name="spacing_xl">32dp</dimen>
    
    <!-- Text Size -->
    <dimen name="text_xs">10sp</dimen>
    <dimen name="text_sm">12sp</dimen>
    <dimen name="text_md">14sp</dimen>
    <dimen name="text_lg">16sp</dimen>
    <dimen name="text_xl">18sp</dimen>
    <dimen name="text_xxl">24sp</dimen>
    
    <!-- Button -->
    <dimen name="button_height">48dp</dimen>
    <dimen name="button_corner_radius">8dp</dimen>
    
    <!-- Card -->
    <dimen name="card_corner_radius">12dp</dimen>
    <dimen name="card_elevation">4dp</dimen>
    
    <!-- Seat Size -->
    <dimen name="seat_size">40dp</dimen>
    <dimen name="seat_spacing">4dp</dimen>
</resources>
```

## 🔄 User Flow trong App

### Complete Booking Flow
```
Launch App
    ↓
Splash Screen (Check token)
    ├─ Token valid → MainActivity (Home)
    └─ Token invalid → LoginActivity
    
MainActivity → HomeFragment
    ↓
Browse Movies (Now Showing / Coming Soon)
    ↓
Click Movie → MovieDetailActivity
    ├─ View Details (Title, Genre, Rating, Duration)
    ├─ Watch Trailer (YouTube/WebView)
    ├─ Read Reviews
    └─ Click "Đặt vé"
    
        ↓
SelectCinemaActivity
    ├─ Choose Cinema (Nearby/Search)
    └─ Select Date
    
        ↓
SelectShowtimeActivity
    ├─ View Available Showtimes
    ├─ See Formats (2D/3D/IMAX)
    └─ Select Showtime
    
        ↓
SeatSelectionActivity ⭐
    ├─ Load Seat Map (Interactive Grid)
    ├─ Select Seats (Multi-select)
    ├─ Show Selected: D5, D6 (2 seats)
    ├─ Calculate Price (Real-time)
    └─ Lock Seats (15 min countdown)
    
        ↓
ComboSelectionActivity (Optional)
    ├─ View Combos (Bắp + Nước)
    ├─ Add to cart
    └─ Calculate Total
    
        ↓
BookingSummaryActivity
    ├─ Review Booking Details
    ├─ Apply Voucher Code (Optional)
    ├─ See Final Amount
    └─ Confirm Booking
    
        ↓
API: POST /api/bookings
    ↓
PaymentMethodActivity
    ├─ Select VNPay
    └─ Confirm Payment
    
        ↓
VNPayWebViewActivity
    ├─ Load VNPay Payment URL
    ├─ User enters card info
    ├─ VNPay processes
    └─ Callback to App
    
        ↓
PaymentResultActivity
    ├─ Success → Show QR Code + Details
    │   ├─ Display BookingId, MovieTitle, Cinema, Seats
    │   ├─ Generate QR Code for check-in
    │   └─ Save to "Vé của tôi"
    │
    └─ Failed → Show Error + Retry Option
```

## 📊 Key Features Implementation

### 1. JWT Token Management
- Store token in SharedPreferences
- Auto-refresh token before expiry
- Intercept API calls to add Authorization header
- Handle 401 Unauthorized → Redirect to Login

### 2. Real-time Seat Selection
- RecyclerView Grid Layout
- Click listener for seat selection
- Different states: Available, Selected, Occupied, VIP, Couple
- Color coding for easy identification
- Lock selected seats via API (15-minute timeout)

### 3. VNPay Payment Integration
- Load payment URL in WebView
- Detect callback URL pattern
- Parse payment result
- Update booking status
- Handle errors gracefully

### 4. Offline Support
- Cache movies using Room Database
- Display cached data when offline
- Sync when online
- Show offline indicator

### 5. Push Notifications
- Firebase Cloud Messaging (FCM)
- Booking confirmation notification
- Payment success notification
- Showtime reminder (1 hour before)

## 🔒 Security Considerations

1. **API Security**:
   - HTTPS only
   - Certificate pinning
   - JWT token encryption
   - Secure storage (EncryptedSharedPreferences)

2. **Input Validation**:
   - Client-side validation
   - Sanitize user inputs
   - Prevent SQL injection

3. **Payment Security**:
   - No card data stored locally
   - WebView with SSL
   - Validate callback URLs

## 📈 Performance Optimization

1. **Image Loading**:
   - Use Glide with caching
   - Load thumbnails first
   - Lazy loading in RecyclerView

2. **API Calls**:
   - Debounce search queries
   - Cancel pending requests
   - Implement pagination

3. **Database**:
   - Index frequently queried columns
   - Background thread operations
   - Clean old cache periodically

## 🧪 Testing Strategy

1. **Unit Tests**:
   - ViewModel logic
   - Repository methods
   - Utility functions

2. **UI Tests**:
   - Espresso for UI testing
   - Test navigation flows
   - Test seat selection

3. **Integration Tests**:
   - API integration
   - Database operations
   - End-to-end booking flow

## 📅 Development Timeline

| Phase | Duration | Deliverables |
|-------|----------|--------------|
| **Phase 1: Setup & Auth** | Week 1 | Project setup, Login, Register |
| **Phase 2: Home & Movies** | Week 2 | Home screen, Movie list, Movie detail |
| **Phase 3: Booking Core** | Week 3-4 | Seat selection, Combo selection, Booking summary |
| **Phase 4: Payment** | Week 5 | VNPay integration, Payment flow |
| **Phase 5: Profile** | Week 6 | Profile, Booking history, Reviews |
| **Phase 6: Polish** | Week 7 | UI/UX refinement, Bug fixes |
| **Phase 7: Testing** | Week 8 | Comprehensive testing, Performance tuning |

---

**Last Updated**: October 29, 2025  
**Document Version**: v1.0
