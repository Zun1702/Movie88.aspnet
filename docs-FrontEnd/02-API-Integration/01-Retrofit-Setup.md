# 🔌 API Integration Guide

## Tổng quan

Cinema Booking System Android App kết nối với **ASP.NET Core Web API Backend** qua **Retrofit 2** library. Tất cả 104 API endpoints đều được implement theo 3-layer architecture (Repository-Service-Controller).

---

## 📡 Backend API Information

### Base URL
```
Development:  https://localhost:7001/
Production:   https://api.movie88.com/
```

### Authentication
```
Type: JWT Bearer Token
Header: Authorization: Bearer {token}
Token Expiry: 24 hours
Refresh Token Expiry: 7 days
```

### Response Format
```json
{
  "success": true,
  "message": "Success",
  "data": { ... }
}
```

---

## 🛠️ Retrofit Setup

### 1. Dependencies (build.gradle)

```gradle
dependencies {
    // Retrofit
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    
    // OkHttp (Logging & Interceptors)
    implementation 'com.squareup.okhttp3:okhttp:4.11.0'
    implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'
    
    // Gson
    implementation 'com.google.code.gson:gson:2.10.1'
}
```

### 2. Internet Permission (AndroidManifest.xml)

```xml
<manifest>
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    
    <application
        android:usesCleartextTraffic="true"  <!-- For localhost testing -->
        ...>
    </application>
</manifest>
```

---

## 📂 File Structure

```
app/src/main/java/com/movie88/data/api/
│
├── ApiClient.java                    # Retrofit Builder (Singleton)
├── ApiService.java                   # All API endpoints
│
├── interceptors/
│   ├── AuthInterceptor.java         # Add JWT token to requests
│   ├── ErrorInterceptor.java        # Handle error responses
│   └── NetworkInterceptor.java      # Check internet connection
│
└── models/
    ├── request/                      # Request DTOs
    │   ├── LoginRequest.java
    │   ├── CreateBookingRequest.java
    │   └── ...
    │
    └── response/                     # Response DTOs
        ├── ApiResponse.java          # Generic wrapper
        ├── LoginResponse.java
        ├── MovieResponse.java
        └── ...
```

---

## 🔧 Implementation

### ApiClient.java (Retrofit Builder)

```java
package com.movie88.data.api;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;
import java.util.concurrent.TimeUnit;

public class ApiClient {
    
    private static ApiClient instance;
    private final Retrofit retrofit;
    private final ApiService apiService;
    
    // Base URL - Change for production
    private static final String BASE_URL = "https://localhost:7001/";
    
    private ApiClient() {
        // Gson configuration
        Gson gson = new GsonBuilder()
            .setLenient()
            .setDateFormat("yyyy-MM-dd'T'HH:mm:ss")
            .create();
        
        // OkHttpClient with interceptors
        OkHttpClient okHttpClient = new OkHttpClient.Builder()
            .addInterceptor(new AuthInterceptor())           // Add JWT token
            .addInterceptor(new NetworkInterceptor())        // Check network
            .addInterceptor(new ErrorInterceptor())          // Handle errors
            .addInterceptor(getLoggingInterceptor())         // Logging
            .connectTimeout(30, TimeUnit.SECONDS)
            .readTimeout(30, TimeUnit.SECONDS)
            .writeTimeout(30, TimeUnit.SECONDS)
            .build();
        
        // Retrofit builder
        retrofit = new Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(okHttpClient)
            .addConverterFactory(GsonConverterFactory.create(gson))
            .build();
        
        apiService = retrofit.create(ApiService.class);
    }
    
    public static synchronized ApiClient getInstance() {
        if (instance == null) {
            instance = new ApiClient();
        }
        return instance;
    }
    
    public ApiService getApiService() {
        return apiService;
    }
    
    private HttpLoggingInterceptor getLoggingInterceptor() {
        HttpLoggingInterceptor interceptor = new HttpLoggingInterceptor();
        interceptor.setLevel(HttpLoggingInterceptor.Level.BODY);
        return interceptor;
    }
}
```

---

## 🔐 Interceptors

### AuthInterceptor.java (Add JWT Token)

```java
package com.movie88.data.api.interceptors;

import com.movie88.utils.TokenManager;
import okhttp3.Interceptor;
import okhttp3.Request;
import okhttp3.Response;
import java.io.IOException;

public class AuthInterceptor implements Interceptor {
    
    @Override
    public Response intercept(Chain chain) throws IOException {
        Request originalRequest = chain.request();
        
        // Get token from TokenManager
        String token = TokenManager.getInstance().getToken();
        
        // If token exists, add Authorization header
        if (token != null && !token.isEmpty()) {
            Request newRequest = originalRequest.newBuilder()
                .header("Authorization", "Bearer " + token)
                .build();
            
            return chain.proceed(newRequest);
        }
        
        return chain.proceed(originalRequest);
    }
}
```

### NetworkInterceptor.java (Check Connection)

```java
package com.movie88.data.api.interceptors;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import com.movie88.MovieApp;
import okhttp3.Interceptor;
import okhttp3.Request;
import okhttp3.Response;
import java.io.IOException;

public class NetworkInterceptor implements Interceptor {
    
    @Override
    public Response intercept(Chain chain) throws IOException {
        if (!isInternetAvailable()) {
            throw new IOException("Không có kết nối internet. Vui lòng kiểm tra lại.");
        }
        
        Request request = chain.request();
        return chain.proceed(request);
    }
    
    private boolean isInternetAvailable() {
        Context context = MovieApp.getInstance().getApplicationContext();
        ConnectivityManager cm = (ConnectivityManager) 
            context.getSystemService(Context.CONNECTIVITY_SERVICE);
        
        if (cm != null) {
            NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
            return activeNetwork != null && activeNetwork.isConnectedOrConnecting();
        }
        return false;
    }
}
```

### ErrorInterceptor.java (Handle HTTP Errors)

```java
package com.movie88.data.api.interceptors;

import okhttp3.Interceptor;
import okhttp3.Response;
import java.io.IOException;

public class ErrorInterceptor implements Interceptor {
    
    @Override
    public Response intercept(Chain chain) throws IOException {
        Response response = chain.proceed(chain.request());
        
        // Handle HTTP status codes
        if (!response.isSuccessful()) {
            String errorMessage;
            
            switch (response.code()) {
                case 400:
                    errorMessage = "Yêu cầu không hợp lệ";
                    break;
                case 401:
                    errorMessage = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
                    // TODO: Clear token and navigate to login
                    break;
                case 403:
                    errorMessage = "Bạn không có quyền truy cập";
                    break;
                case 404:
                    errorMessage = "Không tìm thấy dữ liệu";
                    break;
                case 500:
                    errorMessage = "Lỗi server. Vui lòng thử lại sau.";
                    break;
                default:
                    errorMessage = "Lỗi không xác định: " + response.code();
            }
            
            throw new IOException(errorMessage);
        }
        
        return response;
    }
}
```

---

## 📋 ApiService.java (All Endpoints)

```java
package com.movie88.data.api;

import com.movie88.data.models.request.*;
import com.movie88.data.models.response.*;
import retrofit2.Call;
import retrofit2.http.*;

public interface ApiService {
    
    // ========== AUTHENTICATION APIs (4 endpoints) ==========
    
    @POST("api/auth/login")
    Call<ApiResponse<LoginResponse>> login(@Body LoginRequest request);
    
    @POST("api/auth/register")
    Call<ApiResponse<RegisterResponse>> register(@Body RegisterRequest request);
    
    @POST("api/auth/refresh-token")
    Call<ApiResponse<TokenResponse>> refreshToken(@Body RefreshTokenRequest request);
    
    @POST("api/auth/change-password")
    Call<ApiResponse<Void>> changePassword(@Body ChangePasswordRequest request);
    
    
    // ========== MOVIE APIs (15 endpoints) ==========
    
    @GET("api/movies")
    Call<ApiResponse<PagedResult<Movie>>> getMovies(
        @Query("status") String status,        // "NowShowing", "ComingSoon"
        @Query("genre") String genre,
        @Query("search") String search,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    @GET("api/movies/{id}")
    Call<ApiResponse<Movie>> getMovieById(@Path("id") int movieId);
    
    @GET("api/movies/{id}/showtimes")
    Call<ApiResponse<List<Showtime>>> getMovieShowtimes(
        @Path("id") int movieId,
        @Query("cinemaId") Integer cinemaId,
        @Query("date") String date
    );
    
    @GET("api/movies/genres")
    Call<ApiResponse<List<Genre>>> getGenres();
    
    
    // ========== CINEMA APIs (6 endpoints) ==========
    
    @GET("api/cinemas")
    Call<ApiResponse<List<Cinema>>> getAllCinemas();
    
    @GET("api/cinemas/{id}")
    Call<ApiResponse<Cinema>> getCinemaById(@Path("id") int cinemaId);
    
    @GET("api/cinemas/{id}/auditoriums")
    Call<ApiResponse<List<Auditorium>>> getCinemaAuditoriums(@Path("id") int cinemaId);
    
    
    // ========== SHOWTIME APIs (8 endpoints) ==========
    
    @GET("api/showtimes")
    Call<ApiResponse<List<Showtime>>> getShowtimes(
        @Query("movieId") Integer movieId,
        @Query("cinemaId") Integer cinemaId,
        @Query("date") String date
    );
    
    @GET("api/showtimes/{id}")
    Call<ApiResponse<Showtime>> getShowtimeById(@Path("id") int showtimeId);
    
    @GET("api/showtimes/{id}/seats")
    Call<ApiResponse<List<Seat>>> getShowtimeSeats(@Path("id") int showtimeId);
    
    
    // ========== BOOKING APIs (14 endpoints) ==========
    
    @POST("api/bookings/lock-seats")
    Call<ApiResponse<LockSeatsResponse>> lockSeats(@Body LockSeatsRequest request);
    
    @POST("api/bookings/confirm")
    Call<ApiResponse<Booking>> confirmBooking(@Body CreateBookingRequest request);
    
    @GET("api/bookings")
    Call<ApiResponse<PagedResult<Booking>>> getUserBookings(
        @Query("userId") Integer userId,
        @Query("status") String status,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    @GET("api/bookings/{id}")
    Call<ApiResponse<Booking>> getBookingById(@Path("id") int bookingId);
    
    @PUT("api/bookings/{id}/cancel")
    Call<ApiResponse<Void>> cancelBooking(@Path("id") int bookingId);
    
    
    // ========== COMBO APIs (4 endpoints) ==========
    
    @GET("api/combos")
    Call<ApiResponse<List<Combo>>> getAllCombos();
    
    @GET("api/combos/{id}")
    Call<ApiResponse<Combo>> getComboById(@Path("id") int comboId);
    
    
    // ========== PAYMENT APIs (8 endpoints) ==========
    
    @POST("api/payments/vnpay/create")
    Call<ApiResponse<VNPayCreateResponse>> createVNPayPayment(@Body VNPayCreateRequest request);
    
    @GET("api/payments/vnpay/callback")
    Call<ApiResponse<VNPayCallbackResponse>> handleVNPayCallback(@QueryMap Map<String, String> params);
    
    @POST("api/payments/confirm")
    Call<ApiResponse<Payment>> confirmPayment(@Body ConfirmPaymentRequest request);
    
    @GET("api/payments/{id}")
    Call<ApiResponse<Payment>> getPaymentById(@Path("id") int paymentId);
    
    @GET("api/payments/booking/{bookingId}")
    Call<ApiResponse<Payment>> getPaymentByBookingId(@Path("bookingId") int bookingId);
    
    
    // ========== VOUCHER APIs (7 endpoints) ==========
    
    @GET("api/vouchers")
    Call<ApiResponse<List<Voucher>>> getAvailableVouchers(@Query("userId") int userId);
    
    @POST("api/vouchers/validate")
    Call<ApiResponse<ValidateVoucherResponse>> validateVoucher(@Body ValidateVoucherRequest request);
    
    @GET("api/vouchers/{code}")
    Call<ApiResponse<Voucher>> getVoucherByCode(@Path("code") String code);
    
    
    // ========== REVIEW APIs (6 endpoints) ==========
    
    @GET("api/reviews/movie/{movieId}")
    Call<ApiResponse<PagedResult<Review>>> getMovieReviews(
        @Path("movieId") int movieId,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    @POST("api/reviews")
    Call<ApiResponse<Review>> createReview(@Body CreateReviewRequest request);
    
    @PUT("api/reviews/{id}")
    Call<ApiResponse<Review>> updateReview(
        @Path("id") int reviewId, 
        @Body UpdateReviewRequest request
    );
    
    @DELETE("api/reviews/{id}")
    Call<ApiResponse<Void>> deleteReview(@Path("id") int reviewId);
    
    @GET("api/reviews/user/{userId}")
    Call<ApiResponse<List<Review>>> getUserReviews(@Path("userId") int userId);
    
    
    // ========== USER APIs (8 endpoints) ==========
    
    @GET("api/users/profile")
    Call<ApiResponse<User>> getUserProfile();
    
    @PUT("api/users/profile")
    Call<ApiResponse<User>> updateProfile(@Body UpdateProfileRequest request);
    
    @POST("api/users/avatar")
    @Multipart
    Call<ApiResponse<String>> uploadAvatar(@Part MultipartBody.Part file);
}
```

---

## 📦 Common Models

### ApiResponse.java (Generic Wrapper)

```java
package com.movie88.data.models.response;

public class ApiResponse<T> {
    private boolean success;
    private String message;
    private T data;
    
    public boolean isSuccess() {
        return success;
    }
    
    public String getMessage() {
        return message;
    }
    
    public T getData() {
        return data;
    }
    
    // Setters
    public void setSuccess(boolean success) {
        this.success = success;
    }
    
    public void setMessage(String message) {
        this.message = message;
    }
    
    public void setData(T data) {
        this.data = data;
    }
}
```

### PagedResult.java (Pagination)

```java
package com.movie88.data.models.response;

import java.util.List;

public class PagedResult<T> {
    private List<T> items;
    private int page;
    private int pageSize;
    private int totalCount;
    private int totalPages;
    
    // Getters and setters
    public List<T> getItems() {
        return items;
    }
    
    public void setItems(List<T> items) {
        this.items = items;
    }
    
    public int getPage() {
        return page;
    }
    
    public void setPage(int page) {
        this.page = page;
    }
    
    public int getPageSize() {
        return pageSize;
    }
    
    public void setPageSize(int pageSize) {
        this.pageSize = pageSize;
    }
    
    public int getTotalCount() {
        return totalCount;
    }
    
    public void setTotalCount(int totalCount) {
        this.totalCount = totalCount;
    }
    
    public int getTotalPages() {
        return totalPages;
    }
    
    public void setTotalPages(int totalPages) {
        this.totalPages = totalPages;
    }
    
    public boolean hasNextPage() {
        return page < totalPages;
    }
    
    public boolean hasPreviousPage() {
        return page > 1;
    }
}
```

---

## 🔄 Usage in Repository

### Example: MovieRepository.java

```java
package com.movie88.data.repository;

import com.movie88.data.api.ApiClient;
import com.movie88.data.api.ApiService;
import com.movie88.data.models.response.*;
import com.movie88.utils.ApiCallback;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MovieRepository {
    
    private static MovieRepository instance;
    private final ApiService apiService;
    
    private MovieRepository() {
        apiService = ApiClient.getInstance().getApiService();
    }
    
    public static synchronized MovieRepository getInstance() {
        if (instance == null) {
            instance = new MovieRepository();
        }
        return instance;
    }
    
    // Get movies with filters
    public void getMovies(String status, String genre, String search, 
                         int page, int pageSize, 
                         ApiCallback<PagedResult<Movie>> callback) {
        
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
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Không thể tải danh sách phim");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<PagedResult<Movie>>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    // Get movie by ID
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
}
```

---

## 🧪 Testing API Calls

### Test với Postman
```
1. Import backend API collection
2. Set environment variable: BASE_URL = https://localhost:7001/
3. Test login endpoint → Copy JWT token
4. Add token to Authorization header for other requests
```

### Test trong Android App
```java
// Enable logging in ApiClient
HttpLoggingInterceptor interceptor = new HttpLoggingInterceptor();
interceptor.setLevel(HttpLoggingInterceptor.Level.BODY);

// Check Logcat for API requests/responses
Tag: OkHttp
```

---

## 📚 Reference

- [Retrofit Documentation](https://square.github.io/retrofit/)
- [OkHttp Interceptors](https://square.github.io/okhttp/interceptors/)
- [Backend API Documentation](../../docs/API_List.md)

**Last Updated**: October 29, 2025
