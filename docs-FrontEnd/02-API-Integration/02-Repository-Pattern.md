# 📡 Repository Pattern Implementation

## Tổng quan

Repository Pattern là **Single Source of Truth** cho data trong ứng dụng. Repository quyết định lấy data từ đâu (API, Cache, Database) và coordinate giữa các data sources.

```
┌─────────────────────────────────────────────────────────────┐
│                      VIEWMODEL                              │
│  - Gọi repository methods                                   │
│  - Không biết data đến từ đâu                               │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────┐
│                     REPOSITORY                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Decision Logic:                                    │    │
│  │  - Cache available? → Return cache                  │    │
│  │  - Cache expired? → Fetch from API                  │    │
│  │  - No internet? → Return cached data                │    │
│  │  - API success? → Update cache                      │    │
│  └────────────────────────────────────────────────────┘    │
└──────────────────┬────────────────────┬─────────────────────┘
                   │                    │
        ┌──────────┴──────────┐        │
        ▼                     ▼         ▼
┌──────────────┐      ┌──────────────────┐
│  API SERVICE │      │  LOCAL DATABASE  │
│  (Retrofit)  │      │     (Room)       │
└──────────────┘      └──────────────────┘
```

---

## 📂 Repository Structure

```
app/src/main/java/com/movie88/data/repository/
│
├── AuthRepository.java           # Authentication & user session
├── MovieRepository.java          # Movies, genres, showtimes
├── CinemaRepository.java         # Cinemas, auditoriums
├── BookingRepository.java        # Bookings, seat selection
├── PaymentRepository.java        # VNPay payment processing
├── ComboRepository.java          # Food & drink combos
├── VoucherRepository.java        # Vouchers & discounts
├── ReviewRepository.java         # Movie reviews
└── UserRepository.java           # User profile management
```

---

## 🔐 1. AuthRepository.java

### Purpose
- User authentication (login, register, logout)
- JWT token management
- Session persistence

### Implementation

```java
package com.movie88.data.repository;

import com.movie88.data.api.ApiClient;
import com.movie88.data.api.ApiService;
import com.movie88.data.models.request.*;
import com.movie88.data.models.response.*;
import com.movie88.utils.*;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class AuthRepository {
    
    private static AuthRepository instance;
    private final ApiService apiService;
    private final TokenManager tokenManager;
    private final SharedPrefsManager prefsManager;
    
    private AuthRepository() {
        apiService = ApiClient.getInstance().getApiService();
        tokenManager = TokenManager.getInstance();
        prefsManager = SharedPrefsManager.getInstance();
    }
    
    public static synchronized AuthRepository getInstance() {
        if (instance == null) {
            instance = new AuthRepository();
        }
        return instance;
    }
    
    /**
     * Login user
     * API: POST /api/auth/login
     */
    public void login(LoginRequest request, ApiCallback<LoginResponse> callback) {
        Call<ApiResponse<LoginResponse>> call = apiService.login(request);
        
        call.enqueue(new Callback<ApiResponse<LoginResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<LoginResponse>> call, 
                                 Response<ApiResponse<LoginResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<LoginResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        LoginResponse data = apiResponse.getData();
                        
                        // Save tokens
                        tokenManager.saveToken(data.getToken());
                        tokenManager.saveRefreshToken(data.getRefreshToken());
                        
                        // Save user info
                        prefsManager.saveUser(data.getUser());
                        prefsManager.setLoggedIn(true);
                        
                        callback.onSuccess(data);
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Đăng nhập thất bại. Vui lòng thử lại.");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<LoginResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Register new user
     * API: POST /api/auth/register
     */
    public void register(RegisterRequest request, ApiCallback<RegisterResponse> callback) {
        Call<ApiResponse<RegisterResponse>> call = apiService.register(request);
        
        call.enqueue(new Callback<ApiResponse<RegisterResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<RegisterResponse>> call, 
                                 Response<ApiResponse<RegisterResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<RegisterResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Đăng ký thất bại. Vui lòng thử lại.");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<RegisterResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Refresh access token
     * API: POST /api/auth/refresh-token
     */
    public void refreshToken(ApiCallback<TokenResponse> callback) {
        String refreshToken = tokenManager.getRefreshToken();
        
        if (refreshToken == null || refreshToken.isEmpty()) {
            callback.onError("Refresh token không tồn tại");
            return;
        }
        
        RefreshTokenRequest request = new RefreshTokenRequest(refreshToken);
        Call<ApiResponse<TokenResponse>> call = apiService.refreshToken(request);
        
        call.enqueue(new Callback<ApiResponse<TokenResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<TokenResponse>> call, 
                                 Response<ApiResponse<TokenResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<TokenResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        TokenResponse data = apiResponse.getData();
                        
                        // Update tokens
                        tokenManager.saveToken(data.getToken());
                        tokenManager.saveRefreshToken(data.getRefreshToken());
                        
                        callback.onSuccess(data);
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Làm mới token thất bại");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<TokenResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Change password
     * API: POST /api/auth/change-password
     */
    public void changePassword(ChangePasswordRequest request, ApiCallback<Void> callback) {
        Call<ApiResponse<Void>> call = apiService.changePassword(request);
        
        call.enqueue(new Callback<ApiResponse<Void>>() {
            @Override
            public void onResponse(Call<ApiResponse<Void>> call, 
                                 Response<ApiResponse<Void>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<Void> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(null);
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Đổi mật khẩu thất bại");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<Void>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Logout user (clear local data)
     */
    public void logout() {
        tokenManager.clearTokens();
        prefsManager.clearUser();
        prefsManager.setLoggedIn(false);
    }
    
    /**
     * Check if user is logged in
     */
    public boolean isLoggedIn() {
        return prefsManager.isLoggedIn() && 
               tokenManager.getToken() != null &&
               !tokenManager.isTokenExpired();
    }
    
    /**
     * Get current user from cache
     */
    public User getCurrentUser() {
        return prefsManager.getUser();
    }
}
```

---

## 🎬 2. MovieRepository.java

### Purpose
- Fetch movies (Now Showing, Coming Soon)
- Search & filter movies
- Get movie details, showtimes
- Cache movie data locally

### Implementation

```java
package com.movie88.data.repository;

import com.movie88.data.api.ApiClient;
import com.movie88.data.api.ApiService;
import com.movie88.data.database.AppDatabase;
import com.movie88.data.database.dao.MovieDao;
import com.movie88.data.database.entities.MovieEntity;
import com.movie88.data.models.response.*;
import com.movie88.utils.ApiCallback;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import java.util.List;

public class MovieRepository {
    
    private static MovieRepository instance;
    private final ApiService apiService;
    private final MovieDao movieDao;
    
    private MovieRepository() {
        apiService = ApiClient.getInstance().getApiService();
        movieDao = AppDatabase.getInstance().movieDao();
    }
    
    public static synchronized MovieRepository getInstance() {
        if (instance == null) {
            instance = new MovieRepository();
        }
        return instance;
    }
    
    /**
     * Get movies with filters (with caching)
     * API: GET /api/movies
     * 
     * Caching Strategy:
     * 1. Check cache first
     * 2. If cache valid (< 5 minutes) → return cache
     * 3. If cache expired or no cache → fetch from API
     * 4. Update cache with new data
     */
    public void getMovies(String status, String genre, String search, 
                         int page, int pageSize, 
                         ApiCallback<PagedResult<Movie>> callback) {
        
        // Try to get from cache first (only if no search/filter)
        if (search == null && genre == null && page == 1) {
            List<MovieEntity> cachedMovies = movieDao.getMoviesByStatus(status);
            
            if (!cachedMovies.isEmpty()) {
                // Check if cache is still valid (< 5 minutes)
                long cacheTime = cachedMovies.get(0).getCacheTime();
                long currentTime = System.currentTimeMillis();
                
                if (currentTime - cacheTime < 5 * 60 * 1000) { // 5 minutes
                    // Return cached data
                    List<Movie> movies = MovieEntity.toMovieList(cachedMovies);
                    PagedResult<Movie> result = new PagedResult<>();
                    result.setItems(movies);
                    result.setPage(1);
                    result.setPageSize(movies.size());
                    result.setTotalCount(movies.size());
                    
                    callback.onSuccess(result);
                    return;
                }
            }
        }
        
        // Fetch from API
        Call<ApiResponse<PagedResult<Movie>>> call = apiService.getMovies(
            status, genre, search, page, pageSize
        );
        
        call.enqueue(new Callback<ApiResponse<PagedResult<Movie>>>() {
            @Override
            public void onResponse(Call<ApiResponse<PagedResult<Movie>>> call, 
                                 Response<ApiResponse<PagedResult<Movie>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<PagedResult<Movie>> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        PagedResult<Movie> data = apiResponse.getData();
                        
                        // Update cache (only for first page, no filters)
                        if (search == null && genre == null && page == 1) {
                            updateMovieCache(data.getItems(), status);
                        }
                        
                        callback.onSuccess(data);
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải danh sách phim");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<PagedResult<Movie>>> call, Throwable t) {
                // If API fails, try to return cached data
                List<MovieEntity> cachedMovies = movieDao.getMoviesByStatus(status);
                if (!cachedMovies.isEmpty()) {
                    List<Movie> movies = MovieEntity.toMovieList(cachedMovies);
                    PagedResult<Movie> result = new PagedResult<>();
                    result.setItems(movies);
                    callback.onSuccess(result);
                } else {
                    callback.onError("Lỗi kết nối: " + t.getMessage());
                }
            }
        });
    }
    
    /**
     * Get movie by ID
     * API: GET /api/movies/{id}
     */
    public void getMovieById(int movieId, ApiCallback<Movie> callback) {
        Call<ApiResponse<Movie>> call = apiService.getMovieById(movieId);
        
        call.enqueue(new Callback<ApiResponse<Movie>>() {
            @Override
            public void onResponse(Call<ApiResponse<Movie>> call, 
                                 Response<ApiResponse<Movie>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<Movie> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải thông tin phim");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<Movie>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Get movie showtimes
     * API: GET /api/movies/{id}/showtimes
     */
    public void getMovieShowtimes(int movieId, Integer cinemaId, String date, 
                                  ApiCallback<List<Showtime>> callback) {
        Call<ApiResponse<List<Showtime>>> call = apiService.getMovieShowtimes(
            movieId, cinemaId, date
        );
        
        call.enqueue(new Callback<ApiResponse<List<Showtime>>>() {
            @Override
            public void onResponse(Call<ApiResponse<List<Showtime>>> call, 
                                 Response<ApiResponse<List<Showtime>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<List<Showtime>> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải lịch chiếu");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<List<Showtime>>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Get all genres
     * API: GET /api/movies/genres
     */
    public void getGenres(ApiCallback<List<Genre>> callback) {
        Call<ApiResponse<List<Genre>>> call = apiService.getGenres();
        
        call.enqueue(new Callback<ApiResponse<List<Genre>>>() {
            @Override
            public void onResponse(Call<ApiResponse<List<Genre>>> call, 
                                 Response<ApiResponse<List<Genre>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<List<Genre>> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải thể loại phim");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<List<Genre>>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Update movie cache in database
     */
    private void updateMovieCache(List<Movie> movies, String status) {
        // Run in background thread
        new Thread(() -> {
            // Delete old cache for this status
            movieDao.deleteMoviesByStatus(status);
            
            // Insert new cache
            List<MovieEntity> entities = MovieEntity.fromMovieList(movies, status);
            movieDao.insertAll(entities);
        }).start();
    }
    
    /**
     * Clear all movie cache
     */
    public void clearCache() {
        new Thread(() -> {
            movieDao.deleteAll();
        }).start();
    }
}
```

---

## 🎫 3. BookingRepository.java

### Purpose
- Lock seats for selection
- Create & confirm booking
- Get user booking history
- Cancel booking

### Implementation

```java
package com.movie88.data.repository;

import com.movie88.data.api.ApiClient;
import com.movie88.data.api.ApiService;
import com.movie88.data.models.request.*;
import com.movie88.data.models.response.*;
import com.movie88.utils.ApiCallback;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class BookingRepository {
    
    private static BookingRepository instance;
    private final ApiService apiService;
    
    private BookingRepository() {
        apiService = ApiClient.getInstance().getApiService();
    }
    
    public static synchronized BookingRepository getInstance() {
        if (instance == null) {
            instance = new BookingRepository();
        }
        return instance;
    }
    
    /**
     * Get showtime seats
     * API: GET /api/showtimes/{id}/seats
     */
    public void getShowtimeSeats(int showtimeId, ApiCallback<List<Seat>> callback) {
        Call<ApiResponse<List<Seat>>> call = apiService.getShowtimeSeats(showtimeId);
        
        call.enqueue(new Callback<ApiResponse<List<Seat>>>() {
            @Override
            public void onResponse(Call<ApiResponse<List<Seat>>> call, 
                                 Response<ApiResponse<List<Seat>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<List<Seat>> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải sơ đồ ghế");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<List<Seat>>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Lock selected seats (15 minutes)
     * API: POST /api/bookings/lock-seats
     */
    public void lockSeats(LockSeatsRequest request, ApiCallback<LockSeatsResponse> callback) {
        Call<ApiResponse<LockSeatsResponse>> call = apiService.lockSeats(request);
        
        call.enqueue(new Callback<ApiResponse<LockSeatsResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<LockSeatsResponse>> call, 
                                 Response<ApiResponse<LockSeatsResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<LockSeatsResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể chọn ghế. Vui lòng thử lại.");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<LockSeatsResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Confirm booking
     * API: POST /api/bookings/confirm
     */
    public void confirmBooking(CreateBookingRequest request, ApiCallback<Booking> callback) {
        Call<ApiResponse<Booking>> call = apiService.confirmBooking(request);
        
        call.enqueue(new Callback<ApiResponse<Booking>>() {
            @Override
            public void onResponse(Call<ApiResponse<Booking>> call, 
                                 Response<ApiResponse<Booking>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<Booking> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể xác nhận đặt vé");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<Booking>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Get user bookings
     * API: GET /api/bookings
     */
    public void getUserBookings(Integer userId, String status, int page, int pageSize,
                               ApiCallback<PagedResult<Booking>> callback) {
        Call<ApiResponse<PagedResult<Booking>>> call = apiService.getUserBookings(
            userId, status, page, pageSize
        );
        
        call.enqueue(new Callback<ApiResponse<PagedResult<Booking>>>() {
            @Override
            public void onResponse(Call<ApiResponse<PagedResult<Booking>>> call, 
                                 Response<ApiResponse<PagedResult<Booking>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<PagedResult<Booking>> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải lịch sử đặt vé");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<PagedResult<Booking>>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    /**
     * Cancel booking
     * API: PUT /api/bookings/{id}/cancel
     */
    public void cancelBooking(int bookingId, ApiCallback<Void> callback) {
        Call<ApiResponse<Void>> call = apiService.cancelBooking(bookingId);
        
        call.enqueue(new Callback<ApiResponse<Void>>() {
            @Override
            public void onResponse(Call<ApiResponse<Void>> call, 
                                 Response<ApiResponse<Void>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<Void> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(null);
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể hủy vé");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<Void>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
}
```

---

## 🎯 Key Concepts

### 1. Singleton Pattern
```java
private static MovieRepository instance;

public static synchronized MovieRepository getInstance() {
    if (instance == null) {
        instance = new MovieRepository();
    }
    return instance;
}
```

### 2. Callback Pattern
```java
public interface ApiCallback<T> {
    void onSuccess(T data);
    void onError(String errorMessage);
}
```

### 3. Caching Strategy
```java
// Check cache first
List<MovieEntity> cachedMovies = movieDao.getMoviesByStatus(status);

// If cache valid → return cache
if (isCacheValid(cachedMovies)) {
    callback.onSuccess(cachedMovies);
    return;
}

// Fetch from API → Update cache
fetchFromApi(callback);
```

---

**Next**: PaymentRepository, VoucherRepository implementations

**Last Updated**: October 29, 2025
