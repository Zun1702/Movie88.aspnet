# 📱 Frontend Documentation - Android App (XML Layouts)

> **Cinema Booking System - Mobile Application**  
> Platform: Android Studio | Language: Java/Kotlin | UI: XML Layouts | Architecture: MVVM

---

## � Cấu trúc Documentation

```
docs-FrontEnd/
│
├── README.md                          # �📚 Bạn đang đọc file này
│
├── 01-Architecture/                   # 🏗️ Kiến trúc & Design Pattern
│   ├── 01-Overview.md                # Tổng quan hệ thống
│   └── 02-MVVM-Pattern.md            # MVVM implementation chi tiết
│
├── 02-API-Integration/                # 🔌 Tích hợp Backend API
│   ├── 01-Retrofit-Setup.md          # Retrofit, OkHttp, Interceptors
│   └── 02-Repository-Pattern.md      # Repository implementations
│
├── 03-Screens/                        # 📱 Chi tiết từng màn hình
│   ├── 01-Auth.md                    # Authentication screens
│   ├── 02-Main.md                    # Main navigation & Home
│   └── 03-Booking.md                 # Booking flow screens
│
├── 04-Components/                     # 🎨 Reusable UI Components
│   └── Reusable-Components.md        # MoviePosterCard, SeatView, etc.
│
├── 05-Utils/                          # 🔧 Utility Classes
│   └── (Coming soon)                 # TokenManager, SharedPrefs, etc.
│
└── 06-Resources/                      # 📦 Design System Resources
    └── (Coming soon)                 # colors.xml, dimens.xml, strings.xml
```

---

## 🚀 Quick Navigation

### 1️⃣ Architecture (Kiến trúc)

#### [01-Architecture/01-Overview.md](./01-Architecture/01-Overview.md)
**Tổng quan về kiến trúc Android app**
- ✅ 27 màn hình chi tiết (6 modules)
- ✅ Technology Stack (Retrofit, Glide, Room, Material Design)
- ✅ Project Structure & Folder Organization
- ✅ Design System (colors.xml, dimens.xml)
- ✅ Complete User Booking Flow
- ✅ Dependencies & Build Configuration
- ✅ 8-Week Development Timeline

#### [01-Architecture/02-MVVM-Pattern.md](./01-Architecture/02-MVVM-Pattern.md)
**MVVM Pattern implementation chi tiết**
- ✅ MVVM Architecture Diagram
- ✅ Folder Structure (ui/, data/, utils/)
- ✅ VIEW Layer (Activity/Fragment)
- ✅ VIEWMODEL Layer (LiveData, business logic)
- ✅ MODEL Layer (Request/Response DTOs)
- ✅ Data Flow từ User Action → Backend API
- ✅ Best Practices (DO/DON'T)
- ✅ Testing Strategy

---

### 2️⃣ API Integration (Tích hợp Backend)

#### [02-API-Integration/01-Retrofit-Setup.md](./02-API-Integration/01-Retrofit-Setup.md)
**Retrofit 2 configuration & API endpoints**
- ✅ Retrofit Builder với OkHttpClient
- ✅ **3 Interceptors**: AuthInterceptor (JWT), NetworkInterceptor, ErrorInterceptor
- ✅ **ApiService interface** với tất cả 104 endpoints
- ✅ API Response Models (ApiResponse, PagedResult)
- ✅ Error Handling Strategy
- ✅ Testing với Postman & Logcat

**API Modules:**
- Authentication (4 endpoints)
- Movies (15 endpoints)
- Cinemas (6 endpoints)
- Showtimes (8 endpoints)
- Bookings (14 endpoints)
- Combos (4 endpoints)
- Payments (8 endpoints) - **VNPay only**
- Vouchers (7 endpoints)
- Reviews (6 endpoints)
- Users (8 endpoints)

#### [02-API-Integration/02-Repository-Pattern.md](./02-API-Integration/02-Repository-Pattern.md)
**Repository Pattern implementation**
- ✅ **AuthRepository**: Login, register, token refresh
- ✅ **MovieRepository**: Movies with caching strategy (Room Database)
- ✅ **BookingRepository**: Lock seats, create booking
- ⏳ **PaymentRepository**: VNPay integration (coming soon)
- ⏳ **VoucherRepository**: Validate & apply vouchers (coming soon)

**Key Features:**
- Singleton Pattern
- Callback Pattern (ApiCallback)
- Caching Strategy (5-minute cache)
- Offline Support

---

### 3️⃣ Screens (Màn hình chi tiết)

#### [03-Screens/01-Auth.md](./03-Screens/01-Auth.md)
**Authentication Module (4 screens)**
- ✅ **SplashScreen**: Token validation & auto-login
- ✅ **LoginActivity**: Full XML layout + Java implementation + ViewModel
- ✅ **RegisterActivity**: XML layout (Java pending)
- ⏳ **ForgotPasswordActivity**: Password recovery (pending)

**API Mapping:**
- POST `/api/auth/login`
- POST `/api/auth/register`
- POST `/api/auth/refresh-token`

#### [03-Screens/02-Main.md](./03-Screens/02-Main.md)
**Main Navigation & Home (5 screens)**
- ✅ **MainActivity**: BottomNavigationView + ViewPager2
- ✅ **HomeFragment**: Banner carousel + Now Showing + Coming Soon lists
- ⏳ **SearchMovieActivity**: Search & filter (pending)
- ⏳ **MovieListActivity**: View all (pending)

**API Mapping:**
- GET `/api/movies?status=NowShowing&page=1&pageSize=10`
- GET `/api/movies?status=ComingSoon&page=1&pageSize=10`

#### [03-Screens/03-Booking.md](./03-Screens/03-Booking.md)
**Booking Flow (6 screens)**
- ✅ **SeatSelectionActivity**: Interactive seat map với countdown timer (15 phút)
  - GridLayoutManager cho seat grid
  - Multi-select seats với color states
  - Real-time price calculation
  - Lock seats API integration
- ⏳ **SelectCinemaActivity**: Choose location (pending)
- ⏳ **SelectShowtimeActivity**: Date & time picker (pending)
- ⏳ **ComboSelectionActivity**: Food & drinks (pending)
- ⏳ **BookingSummaryActivity**: Review & voucher (pending)

**API Mapping:**
- GET `/api/showtimes/{id}/seats`
- POST `/api/bookings/lock-seats`
- GET `/api/combos`
- POST `/api/bookings/confirm`

---

### 4️⃣ Components (Reusable Components)

#### [04-Components/Reusable-Components.md](./04-Components/Reusable-Components.md)
**Thư viện UI Components tái sử dụng**
- ✅ **MoviePosterCard**: Movie poster với rating badge
- ✅ **SeatView**: Single seat component với status states
- ✅ **ComboItemCard**: Food/drink combo với quantity selector
- ✅ **LoadingDialog**: Fullscreen loading indicator
- ✅ **EmptyStateView**: Empty state với icon + message
- ⏳ **BookingCard**: Booking history item (pending)
- ⏳ **ReviewCard**: Movie review item (pending)

**Usage:**
```java
MoviePosterCard card = findViewById(R.id.moviePosterCard);
card.bind(movie);
card.setOnMovieClickListener(movie -> {
    // Navigate to MovieDetailActivity
});
```

---

### 5️⃣ Utils (Utilities)

#### [05-Utils/Utils-Classes.md](./05-Utils/Utils-Classes.md)
**Utility Classes & Helpers**
- ✅ **TokenManager**: JWT token storage, validation, decode expiry
- ✅ **SharedPrefsManager**: SharedPreferences wrapper cho user data
- ✅ **DateUtils**: Date formatting, parsing, relative time
- ✅ **ValidationUtils**: Email, phone, password validation với regex
- ✅ **NetworkUtils**: Check internet, WiFi, mobile data
- ✅ **CurrencyUtils**: Format VND currency
- ✅ **Constants**: API URLs, keys, configs
- ✅ **ApiCallback**: Generic callback interface

**Usage:**
```java
// Check token validity
if (TokenManager.getInstance().hasValidToken()) {
    // Token valid
}

// Validate email
String error = ValidationUtils.getEmailError(email);
if (error != null) {
    etEmail.setError(error);
}
```

---

### 6️⃣ Resources (Design System)

#### [06-Resources/Colors-Themes.md](./06-Resources/Colors-Themes.md)
**Colors, Themes & Styles**
- ✅ **colors.xml**: Complete color palette (Primary #E50914, seats, status, ratings)
- ✅ **themes.xml**: Material Design theme với Button, TextInput, Card styles
- ✅ **Drawable resources**: bg_button, bg_seat, bg_rating, bg_dialog, bottom_nav_color
- ⏳ **styles.xml**: Extended custom styles (pending)

#### [06-Resources/Dimensions.md](./06-Resources/Dimensions.md)
**Spacing & Dimensions System**
- ✅ **8dp grid system**: spacing_xs (4dp) to spacing_xxxl (64dp)
- ✅ **Typography scale**: text_size_caption (12sp) to text_size_display (32sp)
- ✅ **Component sizes**: Button (48dp), Poster (140x200dp), Seat (32dp), Input (56dp)
- ✅ **Icon sizes**: icon_size_xs (16dp) to icon_size_xxl (64dp)
- ✅ **Responsive sizing**: values-sw600dp for tablets

#### [06-Resources/Strings.md](./06-Resources/Strings.md)
**String Resources (Vietnamese)**
- ✅ **Authentication**: Login, Register, Change Password strings
- ✅ **Navigation**: Bottom nav, Toolbar titles
- ✅ **Booking flow**: Select Cinema, Showtime, Seats, Combos, Summary
- ✅ **Payment**: VNPay, Payment result messages
- ✅ **Profile**: User info, Settings, Logout
- ✅ **Error messages**: Network, validation, server errors
- ✅ **Common actions**: OK, Cancel, Save, Delete, etc.
- ⏳ **Localization (English)**: values-en/strings.xml (pending)

---

## 📈 Development Progress

| Category | Files | Status |
|----------|-------|--------|
| **Architecture** | 2/2 | ✅ 100% |
| **API Integration** | 3/4 | 🟡 75% |
| **Screens** | 3/7 | 🟡 43% |
| **Components** | 1/2 | 🟡 50% |
| **Utils** | 1/6 | � 17% |
| **Resources** | 3/5 | � 60% |
| **TOTAL** | **13/26** | **🟡 50%** |

### Screen Implementation Progress

| Module | Total | Completed | Pending |
|--------|-------|-----------|---------|
| Authentication | 4 | 3 | 1 |
| Main & Home | 5 | 2 | 3 |
| Movie Details | 4 | 0 | 4 |
| Booking Flow | 6 | 1 | 5 |
| Payment | 3 | 0 | 3 |
| Profile | 5 | 0 | 5 |
| **TOTAL** | **27** | **6** | **21** |

---

## 🎨 Design System

### Colors (res/values/colors.xml)
```xml
<color name="colorPrimary">#E50914</color>          <!-- Netflix Red -->
<color name="backgroundColor">#141414</color>       <!-- Dark Background -->
<color name="cardBackground">#1F1F1F</color>        <!-- Card Background -->
<color name="textPrimary">#FFFFFF</color>           <!-- White Text -->
<color name="textSecondary">#B3B3B3</color>         <!-- Gray Text -->

<!-- Seat Colors -->
<color name="seatAvailable">#4CAF50</color>         <!-- Green -->
<color name="seatSelected">#E50914</color>          <!-- Red -->
<color name="seatOccupied">#757575</color>          <!-- Gray -->
<color name="seatVip">#FFD700</color>               <!-- Gold -->
```

### Dimensions (res/values/dimens.xml)
```xml
<!-- Spacing -->
<dimen name="spacing_xs">4dp</dimen>
<dimen name="spacing_sm">8dp</dimen>
<dimen name="spacing_md">16dp</dimen>
<dimen name="spacing_lg">24dp</dimen>
<dimen name="spacing_xl">32dp</dimen>

<!-- Text Sizes -->
<dimen name="text_xs">12sp</dimen>
<dimen name="text_sm">14sp</dimen>
<dimen name="text_md">16sp</dimen>
<dimen name="text_lg">18sp</dimen>
<dimen name="text_xl">24sp</dimen>

<!-- Components -->
<dimen name="button_height">48dp</dimen>
<dimen name="button_corner_radius">8dp</dimen>
<dimen name="card_corner_radius">12dp</dimen>
<dimen name="movie_poster_width">140dp</dimen>
<dimen name="movie_poster_height">200dp</dimen>
<dimen name="seat_size">32dp</dimen>
<dimen name="seat_spacing">4dp</dimen>
```

---

## 🏗️ Project Structure

```
app/
├── src/main/
│   ├── java/com/movie88/
│   │   ├── ui/
│   │   │   ├── auth/              # Authentication screens
│   │   │   ├── main/              # Main navigation & home
│   │   │   ├── movie/             # Movie details & reviews
│   │   │   ├── booking/           # Booking flow
│   │   │   ├── payment/           # Payment screens
│   │   │   └── profile/           # User profile
│   │   ├── data/
│   │   │   ├── api/               # Retrofit API interfaces
│   │   │   ├── models/            # Data models
│   │   │   └── repository/        # Repository pattern
│   │   ├── viewmodels/            # ViewModels (MVVM)
│   │   └── utils/                 # Utility classes
│   ├── res/
│   │   ├── layout/                # XML layouts
│   │   ├── drawable/              # Icons & backgrounds
│   │   ├── values/                # colors.xml, dimens.xml, strings.xml
│   │   └── menu/                  # Menu resources
│   └── AndroidManifest.xml
└── build.gradle
```

---

## 🔗 API Integration

### Base URL
```
https://api.movie88.com/
```

### Authentication
```java
// Add JWT token to all requests
@Headers("Authorization: Bearer {token}")
```

### Retrofit Setup
```java
Retrofit retrofit = new Retrofit.Builder()
    .baseUrl(BASE_URL)
    .addConverterFactory(GsonConverterFactory.create())
    .client(okHttpClient)
    .build();
```

---

## 📦 Dependencies

### Core
- `androidx.appcompat:appcompat:1.6.1`
- `androidx.constraintlayout:constraintlayout:2.1.4`
- `androidx.lifecycle:lifecycle-viewmodel:2.7.0`
- `androidx.lifecycle:lifecycle-livedata:2.7.0`

### Material Design
- `com.google.android.material:material:1.11.0`

### Networking
- `com.squareup.retrofit2:retrofit:2.9.0`
- `com.squareup.retrofit2:converter-gson:2.9.0`
- `com.squareup.okhttp3:logging-interceptor:4.11.0`

### Image Loading
- `com.github.bumptech.glide:glide:4.16.0`

### Local Database
- `androidx.room:room-runtime:2.6.1`

### QR Code
- `com.google.zxing:core:3.5.2`

---

## 📈 Development Progress

| Module | Total Screens | Completed | In Progress | Pending |
|--------|--------------|-----------|-------------|---------|
| Authentication | 4 | 3 | 0 | 1 |
| Main & Home | 5 | 2 | 0 | 3 |
| Movie Details | 4 | 0 | 0 | 4 |
| Booking Flow | 6 | 1 | 0 | 5 |
| Payment | 3 | 0 | 0 | 3 |
| Profile | 5 | 0 | 0 | 5 |
| **TOTAL** | **27** | **6** | **0** | **21** |

**Completion Rate**: 22% (6/27 screens documented)

---

## 🚀 Quick Start Guide

### 1. Clone Backend Repository
```bash
git clone https://github.com/your-repo/cinema-backend.git
cd cinema-backend
dotnet run
```

### 2. Create Android Project
```bash
# In Android Studio:
# File > New > New Project > Empty Activity
# - Name: Movie88
# - Package: com.movie88
# - Language: Java
# - Minimum SDK: API 24 (Android 7.0)
```

### 3. Add Dependencies (build.gradle)
```gradle
dependencies {
    // See full list in Frontend_Overview.md
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.google.android.material:material:1.11.0'
    // ... etc
}
```

### 4. Configure API Base URL
```java
// app/src/main/java/com/movie88/utils/Constants.java
public class Constants {
    public static final String BASE_URL = "https://localhost:7001/";
}
```

### 5. Run Application
```bash
# Connect Android device or start emulator
# Click Run button in Android Studio
```

---

## 📝 Notes

### Backend Integration
- Tất cả các màn hình đều mapping chính xác với **104 API endpoints** từ backend
- Backend sử dụng **3-Layer Architecture** (Repository-Service-Controller)
- Payment chỉ hỗ trợ **VNPay** (không có MoMo/ZaloPay)

### Authentication Flow
1. User login → Nhận JWT token
2. Save token to SharedPreferences
3. Add token to Authorization header cho tất cả requests
4. Token expiry: 24 hours
5. Refresh token: 7 days

### Booking Flow
1. Select Movie → Select Cinema → Select Showtime
2. Select Seats (lock 15 minutes)
3. Select Combos (optional)
4. Review Booking → Apply Voucher (optional)
5. Payment via VNPay
6. Receive QR Code ticket

---

## 🔜 Next Steps

1. ✅ Complete **Screens_Payment.md** (VNPayWebViewActivity, PaymentResultActivity)
2. ✅ Complete **Screens_Profile.md** (ProfileFragment, BookingHistoryFragment)
3. ✅ Complete **Screens_Movie.md** (MovieDetailActivity with trailer & reviews)
4. ✅ Add API error handling documentation
5. ✅ Add offline caching strategy (Room database)
6. ✅ Add push notifications (Firebase Cloud Messaging)

---

**Last Updated**: October 29, 2025  
**Version**: 1.0  
**Author**: Development Team
