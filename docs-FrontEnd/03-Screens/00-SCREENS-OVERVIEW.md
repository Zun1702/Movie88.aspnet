# 📱 Cinema Booking System - Frontend Screens Overview

## 📂 Documentation Structure

Tất cả các screen đã được tài liệu hóa chi tiết với:
- ✅ Complete XML layouts (Material Design Components)
- ✅ Full Java Activity/Fragment implementations
- ✅ ViewModel architecture (MVVM pattern)
- ✅ API endpoints mapping
- ✅ Navigation flows
- ✅ Error handling
- ✅ Loading states
- ✅ Data models

---

## 🗂️ Screen Modules

### 1️⃣ Authentication Module (`01-Auth.md`)

**Screens:**
- **SplashActivity** - JWT token validation & auto-login
- **LoginActivity** - Email/password authentication
- **RegisterActivity** - New user registration with validation
- **ForgotPasswordActivity** - Password recovery flow

**Key Features:**
- JWT token storage in SharedPreferences
- Auto-refresh expired tokens
- Email/password validation
- Terms & Conditions checkbox
- Error dialogs with MaterialAlertDialog

**API Endpoints:**
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
POST /api/auth/forgot-password
```

---

### 2️⃣ Main Navigation Module (`02-Main.md`)

**Container:**
- **MainActivity** - ViewPager2 + BottomNavigationView (3 tabs)

**Fragments:**
1. **HomeFragment** 
   - Banner carousel (auto-scroll 3s)
   - Now Showing movies horizontal list
   - Coming Soon movies horizontal list
   - Search bar → SearchMovieActivity

2. **BookingHistoryFragment**
   - Filter chips (All/Pending/Confirmed/Completed/Cancelled)
   - Booking list with status badges
   - Pull-to-refresh
   - Empty state handling
   - Click booking → BookingDetailActivity

3. **ProfileFragment** → See `06-Profile.md`

**Key Features:**
- Auto-scrolling banner with pause on user interaction
- Horizontal movie carousels (RecyclerView HORIZONTAL)
- Status-based filtering for bookings
- Swipe-to-refresh functionality
- Empty state with icon + message

**API Endpoints:**
```
GET /api/movies?status={NowShowing|ComingSoon}&page={}&pageSize={}
GET /api/movies/now-showing
GET /api/movies/coming-soon
GET /api/promotions/active
GET /api/bookings/my-bookings?status={}&page={}&pageSize={}
GET /api/bookings/{id}
```

---

### 3️⃣ Booking Flow Module (`03-Booking.md`) ⭐

**5-Screen Booking Process:**

```
SelectCinemaActivity → SelectSeatActivity → SelectComboActivity → BookingSummaryActivity → VNPayWebViewActivity → PaymentResultActivity
```

1. **SelectCinemaActivity** (~400 lines)
   - Horizontal date selector (RecyclerView)
   - Cinema list with showtime chips
   - Selected showtime highlight
   - Navigate with movieId → showtimeId

2. **SelectSeatActivity** (Existing implementation)
   - Interactive seat map (GridLayoutManager)
   - Seat type legend (Available/Selected/Booked)
   - Real-time price calculation
   - 5-minute countdown timer
   - Navigate with showtimeId → seatIds

3. **SelectComboActivity** (~500 lines) [OPTIONAL]
   - Food & drink combo cards
   - Quantity selector (+/- buttons)
   - Real-time total price update
   - **Skip button** to proceed without combos
   - Navigate with seatIds → comboIds (optional)

4. **BookingSummaryActivity** (Reference to 05-Payment.md)
   - Movie info summary
   - Seats display
   - Combos list (if selected)
   - Voucher input
   - Total price display
   - Confirm booking → Create booking
   - Navigate with bookingId → VNPay

5. **VNPayWebViewActivity** (Reference to 05-Payment.md)
   - WebView for VNPay gateway
   - URL interception for callback
   - Payment status detection

6. **PaymentResultActivity** (Reference to 05-Payment.md)
   - Success/Failed status
   - QR code for ticket (if success)
   - Booking details
   - Download ticket button

**Data Flow:**
```
movieId → showtimeId → seatIds → comboIds (optional) → bookingId → paymentUrl → paymentStatus
```

**Timer Management:**
- Timer starts when seats are selected
- Timer continues across SelectComboActivity & BookingSummaryActivity
- Timer expires → redirect to SelectCinemaActivity with error message

**API Endpoints:**
```
GET /api/cinemas
GET /api/showtimes/movie/{movieId}
GET /api/showtimes/{id}/seats
POST /api/bookings/reserve-seats
GET /api/combos
POST /api/bookings
POST /api/payments/vnpay/create
GET /api/payments/vnpay/callback
GET /api/bookings/{id}
```

---

### 4️⃣ Movie Details Module (`04-Movie-Details.md`)

**Screens:**
1. **MovieDetailActivity**
   - Collapsing toolbar with backdrop image
   - Movie poster + info (rating, genre, duration, age rating)
   - "Book Now" button → SelectCinemaActivity
   - "Watch Trailer" button → MovieTrailerActivity
   - Synopsis
   - Cast & Crew
   - User reviews

2. **MovieTrailerActivity**
   - YouTube WebView or ExoPlayer
   - Fullscreen video player
   - Play/pause controls

3. **SearchMovieActivity**
   - Search input with debounce
   - Filter chips (Genre, Status)
   - Grid movie results
   - Click movie → MovieDetailActivity

**API Endpoints:**
```
GET /api/movies/{id}
GET /api/movies/{id}/trailer
GET /api/movies/search?keyword={}&genre={}&status={}
GET /api/reviews/movie/{movieId}
```

---

### 5️⃣ Payment Module (`05-Payment.md`)

**Screens:**
1. **BookingSummaryActivity** (See 03-Booking.md)
   - Voucher application
   - Price breakdown
   - Confirm booking

2. **VNPayWebViewActivity**
   - VNPay payment gateway WebView
   - URL callback detection
   - Loading indicator

3. **PaymentResultActivity**
   - Success/Failed status display
   - QR code ticket (success)
   - Booking details summary
   - "Download Ticket" → Save as PDF/Image
   - "Back to Home" → MainActivity

**API Endpoints:**
```
POST /api/vouchers/validate
POST /api/bookings
POST /api/payments/vnpay/create
GET /api/payments/vnpay/callback?vnp_ResponseCode={}&vnp_TxnRef={}
GET /api/bookings/{id}/ticket
```

---

### 6️⃣ Profile Module (`06-Profile.md`)

**Screens:**
1. **ProfileFragment**
   - User info card (avatar, name, email, phone)
   - Change avatar (image picker)
   - Edit profile button
   - Menu items:
     - Booking History → BookingHistoryActivity
     - Change Password → ChangePasswordActivity
     - Notifications toggle (SharedPreferences)
     - Settings → SettingsActivity
     - Help
     - About
   - Logout button with confirmation

2. **EditProfileActivity**
   - Full name input
   - Email (read-only)
   - Phone number input with validation
   - Birthday date picker
   - Gender radio buttons (Male/Female/Other)
   - Save button

3. **ChangePasswordActivity**
   - Old password input
   - New password input
   - Confirm password input
   - Password strength indicator
   - Save button

**API Endpoints:**
```
GET /api/users/me
GET /api/customers/profile
PUT /api/users/{id}
PUT /api/customers/profile
POST /api/users/avatar (multipart/form-data)
GET /api/bookings/my-bookings
POST /api/auth/change-password
POST /api/auth/logout
```

---

### 7️⃣ Settings Module (`07-Settings.md`)

**Screen:**
- **SettingsActivity**
  - Notifications (on/off)
  - Language selection
  - Theme (Light/Dark/System)
  - Clear cache
  - App version display

**Storage:**
- Uses **SharedPreferences** only (no API calls)

---

## 🔄 Navigation Flow Diagram

```
SplashActivity
│
├─ (Has valid token) → MainActivity (Tab 0: HomeFragment)
└─ (No token) → LoginActivity
                    │
                    ├─ Register → RegisterActivity → LoginActivity
                    ├─ Forgot Password → ForgotPasswordActivity
                    └─ Success → MainActivity

MainActivity (BottomNavigationView)
│
├─ Tab 0: HomeFragment
│   ├─ Click Movie → MovieDetailActivity
│   │                   └─ Book Now → SelectCinemaActivity (Booking Flow)
│   └─ Search → SearchMovieActivity
│
├─ Tab 1: BookingHistoryFragment
│   └─ Click Booking → BookingDetailActivity
│
└─ Tab 2: ProfileFragment
    ├─ Edit Profile → EditProfileActivity
    ├─ Change Password → ChangePasswordActivity
    ├─ Settings → SettingsActivity
    └─ Logout → LoginActivity (FLAG_CLEAR_TASK)

Booking Flow (5 screens):
SelectCinemaActivity → SelectSeatActivity → SelectComboActivity (optional) → BookingSummaryActivity → VNPayWebViewActivity → PaymentResultActivity
```

---

## 📊 Screen Count Summary

| Module | Screens | Complexity |
|--------|---------|------------|
| **01-Auth** | 4 screens | 🟢 Simple |
| **02-Main** | 1 Activity + 3 Fragments | 🟡 Medium |
| **03-Booking** | 6 screens (5-step flow) | 🔴 Complex |
| **04-Movie-Details** | 3 screens | 🟡 Medium |
| **05-Payment** | 3 screens | 🟡 Medium |
| **06-Profile** | 3 screens | 🟢 Simple |
| **07-Settings** | 1 screen | 🟢 Simple |
| **TOTAL** | **22 unique screens** | - |

---

## 🎯 API Endpoints Summary

**Total Backend Endpoints**: 111 (from `docs/API_List.md`)
**Actively Used by Frontend**: 42 endpoints

**Endpoint Distribution:**
- Authentication: 4 endpoints
- Movies: 8 endpoints
- Cinemas & Showtimes: 6 endpoints
- Bookings: 8 endpoints
- Seats: 3 endpoints
- Combos: 2 endpoints
- Payments (VNPay): 3 endpoints
- Users & Customers: 5 endpoints
- Vouchers: 2 endpoints
- Reviews: 1 endpoint

**See**: `API-Screen-Mapping-Summary.md` for complete mapping.

---

## 🏗️ Architecture Overview

### Tech Stack
- **UI Framework**: Android Native (Java)
- **Layout**: XML with Material Design Components 3
- **Architecture**: MVVM (Model-View-ViewModel)
- **Networking**: Retrofit 2 + OkHttp
- **Image Loading**: Glide
- **Dependency Injection**: Manual (Singleton pattern)
- **Local Storage**: SharedPreferences (JWT tokens, settings)
- **Payment Gateway**: VNPay (WebView integration)

### Key Design Patterns
1. **Repository Pattern**: Centralized data access (AuthRepository, MovieRepository, BookingRepository)
2. **Observer Pattern**: LiveData in ViewModels for reactive UI updates
3. **Singleton Pattern**: Repository instances, SharedPrefsManager
4. **Callback Pattern**: ApiCallback<T> for async API responses
5. **Adapter Pattern**: RecyclerView adapters for dynamic lists

### Folder Structure
```
app/src/main/java/com/movie88/
├── ui/
│   ├── auth/ (LoginActivity, RegisterActivity, etc.)
│   ├── main/ (MainActivity, HomeFragment, BookingHistoryFragment, ProfileFragment)
│   ├── booking/ (SelectCinemaActivity, SelectSeatActivity, SelectComboActivity)
│   ├── movie/ (MovieDetailActivity, SearchMovieActivity)
│   ├── payment/ (BookingSummaryActivity, VNPayWebViewActivity, PaymentResultActivity)
│   └── profile/ (EditProfileActivity, ChangePasswordActivity, SettingsActivity)
├── data/
│   ├── models/ (Movie, Booking, Seat, Combo, User, etc.)
│   ├── repositories/ (AuthRepository, MovieRepository, BookingRepository)
│   └── api/ (ApiService, RetrofitClient, ApiCallback)
├── utils/
│   ├── SharedPrefsManager
│   ├── ValidationUtils
│   ├── DateUtils
│   └── Constants
└── viewmodels/ (LoginViewModel, HomeViewModel, BookingViewModel, etc.)
```

---

## ✅ Completion Checklist

### Documentation Status
- ✅ **01-Auth.md** - 4 screens fully documented
- ✅ **02-Main.md** - 1 container + 3 fragments fully documented with adapters
- ✅ **03-Booking.md** - 5-step booking flow completely restructured (~6000 lines)
- ✅ **04-Movie-Details.md** - 3 screens fully documented
- ✅ **05-Payment.md** - 3 screens fully documented
- ✅ **06-Profile.md** - 3 screens fully documented
- ✅ **07-Settings.md** - 1 screen fully documented
- ✅ **API-Screen-Mapping-Summary.md** - Complete 42 endpoint mapping

### Implementation Requirements
- ⚠️ **Backend API**: Must implement all 42 required endpoints
- ⚠️ **JWT Authentication**: Refresh token logic must be implemented
- ⚠️ **VNPay Integration**: Requires VNPay merchant account
- ⚠️ **Seat Locking**: Temporary seat reservation (5 minutes) must be backend-supported
- ⚠️ **WebSocket** (Optional): Real-time seat updates for concurrent users

---

## 🚀 Next Steps for Developers

### Phase 1: Setup (Week 1)
1. Create Android Studio project (API 24+)
2. Add dependencies (Retrofit, Glide, Material Components)
3. Setup folder structure as documented
4. Implement SharedPrefsManager, Constants, Utils

### Phase 2: Authentication (Week 2)
1. Implement 01-Auth screens (Splash, Login, Register, ForgotPassword)
2. Setup RetrofitClient, ApiService, AuthRepository
3. Test JWT token storage and refresh

### Phase 3: Main Navigation (Week 3)
1. Implement MainActivity with ViewPager2 + BottomNavigation
2. Create HomeFragment with banner auto-scroll
3. Create BookingHistoryFragment with filters
4. Create ProfileFragment with menu items

### Phase 4: Movie Module (Week 4)
1. Implement MovieDetailActivity with collapsing toolbar
2. Implement SearchMovieActivity with filters
3. Implement MovieTrailerActivity (WebView or ExoPlayer)

### Phase 5: Booking Flow (Week 5-6) ⚠️ Complex
1. Implement SelectCinemaActivity with date selector
2. Implement SelectSeatActivity with timer
3. Implement SelectComboActivity with skip option
4. Test data flow: movieId → showtimeId → seatIds → comboIds → bookingId

### Phase 6: Payment (Week 7)
1. Implement BookingSummaryActivity with voucher
2. Implement VNPayWebViewActivity with URL interception
3. Implement PaymentResultActivity with QR code
4. Test end-to-end booking + payment

### Phase 7: Profile & Settings (Week 8)
1. Implement EditProfileActivity with validation
2. Implement ChangePasswordActivity
3. Implement SettingsActivity with SharedPreferences
4. Test logout flow

### Phase 8: Testing & Polish (Week 9-10)
1. Integration testing with backend
2. Error handling & edge cases
3. Loading states & empty states
4. UI/UX polish (animations, transitions)
5. Performance optimization (image caching, list recycling)

---

## 📞 Support & Maintenance

**Documentation Maintainer**: AI Agent (GitHub Copilot)
**Last Major Update**: October 29, 2025
**Version**: 2.0 (Complete restructure with detailed implementations)

**Change Log:**
- **v2.0**: Restructured 03-Booking.md into 5 detailed screens (~6000 lines)
- **v2.0**: Enhanced 02-Main.md with BookingHistoryFragment, adapters, auto-scroll logic
- **v2.0**: Added complete API endpoint mapping (42 endpoints)
- **v2.0**: Created this overview document for easy navigation

**For Questions:**
- Check individual screen documentation files
- Review API-Screen-Mapping-Summary.md for endpoint details
- Refer to backend docs/API_List.md for API specifications

---

**Enjoy building! 🎬🍿**
