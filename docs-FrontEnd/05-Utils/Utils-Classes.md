# 🔧 Utility Classes & Helpers

## Tổng quan

Thư viện các utility classes giúp xử lý các tác vụ phổ biến: token management, shared preferences, date formatting, validation, network checking, etc.

---

## 📂 Structure

```
app/src/main/java/com/movie88/utils/
│
├── TokenManager.java              # JWT token storage & validation
├── SharedPrefsManager.java        # SharedPreferences wrapper
├── DateUtils.java                 # Date formatting & parsing
├── ValidationUtils.java           # Email, phone, password validation
├── NetworkUtils.java              # Internet connection check
├── Constants.java                 # API URLs, keys, configs
├── CurrencyUtils.java             # Currency formatting
├── QRCodeUtils.java               # QR code generation
└── ApiCallback.java               # Generic API callback interface
```

---

## 1️⃣ TokenManager.java

### Purpose
Quản lý JWT token: save, retrieve, validate expiry, decode payload.

### Implementation

```java
package com.movie88.utils;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Base64;
import com.movie88.MovieApp;
import org.json.JSONObject;

public class TokenManager {
    
    private static TokenManager instance;
    private final SharedPreferences prefs;
    
    private static final String PREF_NAME = "TokenPrefs";
    private static final String KEY_TOKEN = "jwt_token";
    private static final String KEY_REFRESH_TOKEN = "refresh_token";
    private static final String KEY_TOKEN_EXPIRY = "token_expiry";
    
    private TokenManager() {
        Context context = MovieApp.getInstance().getApplicationContext();
        prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }
    
    public static synchronized TokenManager getInstance() {
        if (instance == null) {
            instance = new TokenManager();
        }
        return instance;
    }
    
    /**
     * Save JWT token
     */
    public void saveToken(String token) {
        prefs.edit().putString(KEY_TOKEN, token).apply();
        
        // Extract and save expiry time
        long expiryTime = extractTokenExpiry(token);
        prefs.edit().putLong(KEY_TOKEN_EXPIRY, expiryTime).apply();
    }
    
    /**
     * Get JWT token
     */
    public String getToken() {
        return prefs.getString(KEY_TOKEN, null);
    }
    
    /**
     * Save refresh token
     */
    public void saveRefreshToken(String refreshToken) {
        prefs.edit().putString(KEY_REFRESH_TOKEN, refreshToken).apply();
    }
    
    /**
     * Get refresh token
     */
    public String getRefreshToken() {
        return prefs.getString(KEY_REFRESH_TOKEN, null);
    }
    
    /**
     * Check if token is expired
     */
    public boolean isTokenExpired() {
        long expiryTime = prefs.getLong(KEY_TOKEN_EXPIRY, 0);
        long currentTime = System.currentTimeMillis();
        
        // Token expired if expiry time is less than current time
        // Add 5 minute buffer for safety
        return expiryTime <= currentTime + (5 * 60 * 1000);
    }
    
    /**
     * Extract expiry time from JWT token
     */
    private long extractTokenExpiry(String token) {
        try {
            // JWT format: header.payload.signature
            String[] parts = token.split("\\.");
            if (parts.length < 2) {
                return 0;
            }
            
            // Decode payload (Base64)
            String payload = parts[1];
            byte[] decodedBytes = Base64.decode(payload, Base64.URL_SAFE);
            String decodedPayload = new String(decodedBytes);
            
            // Parse JSON
            JSONObject json = new JSONObject(decodedPayload);
            
            // Get "exp" field (Unix timestamp in seconds)
            long exp = json.getLong("exp");
            
            // Convert to milliseconds
            return exp * 1000;
            
        } catch (Exception e) {
            e.printStackTrace();
            return 0;
        }
    }
    
    /**
     * Get user ID from token
     */
    public int getUserIdFromToken() {
        String token = getToken();
        if (token == null) {
            return 0;
        }
        
        try {
            String[] parts = token.split("\\.");
            if (parts.length < 2) {
                return 0;
            }
            
            String payload = parts[1];
            byte[] decodedBytes = Base64.decode(payload, Base64.URL_SAFE);
            String decodedPayload = new String(decodedBytes);
            
            JSONObject json = new JSONObject(decodedPayload);
            return json.getInt("userId");
            
        } catch (Exception e) {
            e.printStackTrace();
            return 0;
        }
    }
    
    /**
     * Get user role from token
     */
    public String getUserRoleFromToken() {
        String token = getToken();
        if (token == null) {
            return null;
        }
        
        try {
            String[] parts = token.split("\\.");
            if (parts.length < 2) {
                return null;
            }
            
            String payload = parts[1];
            byte[] decodedBytes = Base64.decode(payload, Base64.URL_SAFE);
            String decodedPayload = new String(decodedBytes);
            
            JSONObject json = new JSONObject(decodedPayload);
            return json.getString("role");
            
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }
    
    /**
     * Clear all tokens
     */
    public void clearTokens() {
        prefs.edit()
            .remove(KEY_TOKEN)
            .remove(KEY_REFRESH_TOKEN)
            .remove(KEY_TOKEN_EXPIRY)
            .apply();
    }
    
    /**
     * Check if user is logged in
     */
    public boolean hasValidToken() {
        return getToken() != null && !isTokenExpired();
    }
}
```

---

## 2️⃣ SharedPrefsManager.java

### Purpose
Wrapper cho SharedPreferences để lưu user data, settings, cache.

### Implementation

```java
package com.movie88.utils;

import android.content.Context;
import android.content.SharedPreferences;
import com.google.gson.Gson;
import com.movie88.MovieApp;
import com.movie88.data.models.response.User;

public class SharedPrefsManager {
    
    private static SharedPrefsManager instance;
    private final SharedPreferences prefs;
    private final Gson gson;
    
    private static final String PREF_NAME = "MovieAppPrefs";
    
    // Keys
    private static final String KEY_USER = "user";
    private static final String KEY_IS_LOGGED_IN = "is_logged_in";
    private static final String KEY_FIRST_TIME = "first_time";
    private static final String KEY_SELECTED_CINEMA = "selected_cinema";
    private static final String KEY_NOTIFICATION_ENABLED = "notification_enabled";
    
    private SharedPrefsManager() {
        Context context = MovieApp.getInstance().getApplicationContext();
        prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        gson = new Gson();
    }
    
    public static synchronized SharedPrefsManager getInstance() {
        if (instance == null) {
            instance = new SharedPrefsManager();
        }
        return instance;
    }
    
    /**
     * Save user object
     */
    public void saveUser(User user) {
        String userJson = gson.toJson(user);
        prefs.edit().putString(KEY_USER, userJson).apply();
    }
    
    /**
     * Get user object
     */
    public User getUser() {
        String userJson = prefs.getString(KEY_USER, null);
        if (userJson == null) {
            return null;
        }
        return gson.fromJson(userJson, User.class);
    }
    
    /**
     * Clear user data
     */
    public void clearUser() {
        prefs.edit().remove(KEY_USER).apply();
    }
    
    /**
     * Set logged in status
     */
    public void setLoggedIn(boolean loggedIn) {
        prefs.edit().putBoolean(KEY_IS_LOGGED_IN, loggedIn).apply();
    }
    
    /**
     * Check if user is logged in
     */
    public boolean isLoggedIn() {
        return prefs.getBoolean(KEY_IS_LOGGED_IN, false);
    }
    
    /**
     * Check if first time launch
     */
    public boolean isFirstTime() {
        return prefs.getBoolean(KEY_FIRST_TIME, true);
    }
    
    /**
     * Set first time launch to false
     */
    public void setNotFirstTime() {
        prefs.edit().putBoolean(KEY_FIRST_TIME, false).apply();
    }
    
    /**
     * Save selected cinema ID
     */
    public void saveSelectedCinema(int cinemaId) {
        prefs.edit().putInt(KEY_SELECTED_CINEMA, cinemaId).apply();
    }
    
    /**
     * Get selected cinema ID
     */
    public int getSelectedCinema() {
        return prefs.getInt(KEY_SELECTED_CINEMA, 0);
    }
    
    /**
     * Enable/disable notifications
     */
    public void setNotificationEnabled(boolean enabled) {
        prefs.edit().putBoolean(KEY_NOTIFICATION_ENABLED, enabled).apply();
    }
    
    /**
     * Check if notifications enabled
     */
    public boolean isNotificationEnabled() {
        return prefs.getBoolean(KEY_NOTIFICATION_ENABLED, true);
    }
    
    /**
     * Clear all preferences
     */
    public void clearAll() {
        prefs.edit().clear().apply();
    }
}
```

---

## 3️⃣ DateUtils.java

### Purpose
Format và parse dates cho hiển thị và API calls.

### Implementation

```java
package com.movie88.utils;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

public class DateUtils {
    
    // Date formats
    private static final String FORMAT_API = "yyyy-MM-dd'T'HH:mm:ss";      // API format
    private static final String FORMAT_DATE = "dd/MM/yyyy";                // Display date
    private static final String FORMAT_TIME = "HH:mm";                     // Display time
    private static final String FORMAT_DATETIME = "dd/MM/yyyy HH:mm";      // Display datetime
    private static final String FORMAT_DAY_MONTH = "dd/MM";                // Short date
    private static final String FORMAT_API_DATE = "yyyy-MM-dd";            // API date only
    
    /**
     * Parse API datetime string to Date object
     */
    public static Date parseApiDateTime(String dateTimeString) {
        try {
            SimpleDateFormat format = new SimpleDateFormat(FORMAT_API, Locale.getDefault());
            return format.parse(dateTimeString);
        } catch (ParseException e) {
            e.printStackTrace();
            return null;
        }
    }
    
    /**
     * Format Date to display format
     */
    public static String formatDate(Date date) {
        SimpleDateFormat format = new SimpleDateFormat(FORMAT_DATE, Locale.getDefault());
        return format.format(date);
    }
    
    /**
     * Format Date to time format
     */
    public static String formatTime(Date date) {
        SimpleDateFormat format = new SimpleDateFormat(FORMAT_TIME, Locale.getDefault());
        return format.format(date);
    }
    
    /**
     * Format Date to datetime format
     */
    public static String formatDateTime(Date date) {
        SimpleDateFormat format = new SimpleDateFormat(FORMAT_DATETIME, Locale.getDefault());
        return format.format(date);
    }
    
    /**
     * Format Date to API date format (yyyy-MM-dd)
     */
    public static String formatApiDate(Date date) {
        SimpleDateFormat format = new SimpleDateFormat(FORMAT_API_DATE, Locale.getDefault());
        return format.format(date);
    }
    
    /**
     * Get today's date in API format
     */
    public static String getTodayApiFormat() {
        return formatApiDate(new Date());
    }
    
    /**
     * Get relative date string (Today, Tomorrow, etc.)
     */
    public static String getRelativeDateString(Date date) {
        Calendar today = Calendar.getInstance();
        Calendar target = Calendar.getInstance();
        target.setTime(date);
        
        // Check if same day
        if (isSameDay(today, target)) {
            return "Hôm nay, " + formatTime(date);
        }
        
        // Check if tomorrow
        today.add(Calendar.DAY_OF_YEAR, 1);
        if (isSameDay(today, target)) {
            return "Ngày mai, " + formatTime(date);
        }
        
        // Check if this week
        today.add(Calendar.DAY_OF_YEAR, -1); // Reset to today
        today.add(Calendar.DAY_OF_YEAR, 7);
        if (target.before(today)) {
            String dayOfWeek = new SimpleDateFormat("EEEE", new Locale("vi")).format(date);
            return dayOfWeek + ", " + formatTime(date);
        }
        
        // Show full date
        return formatDateTime(date);
    }
    
    /**
     * Check if two dates are same day
     */
    private static boolean isSameDay(Calendar cal1, Calendar cal2) {
        return cal1.get(Calendar.YEAR) == cal2.get(Calendar.YEAR) &&
               cal1.get(Calendar.DAY_OF_YEAR) == cal2.get(Calendar.DAY_OF_YEAR);
    }
    
    /**
     * Get duration string (e.g., "2h 30m")
     */
    public static String formatDuration(int minutes) {
        int hours = minutes / 60;
        int mins = minutes % 60;
        
        if (hours > 0 && mins > 0) {
            return hours + "h " + mins + "m";
        } else if (hours > 0) {
            return hours + "h";
        } else {
            return mins + "m";
        }
    }
    
    /**
     * Get time ago string (e.g., "2 hours ago")
     */
    public static String getTimeAgo(Date date) {
        long diff = System.currentTimeMillis() - date.getTime();
        long seconds = diff / 1000;
        long minutes = seconds / 60;
        long hours = minutes / 60;
        long days = hours / 24;
        
        if (days > 0) {
            return days + " ngày trước";
        } else if (hours > 0) {
            return hours + " giờ trước";
        } else if (minutes > 0) {
            return minutes + " phút trước";
        } else {
            return "Vừa xong";
        }
    }
    
    /**
     * Add days to date
     */
    public static Date addDays(Date date, int days) {
        Calendar cal = Calendar.getInstance();
        cal.setTime(date);
        cal.add(Calendar.DAY_OF_YEAR, days);
        return cal.getTime();
    }
}
```

---

## 4️⃣ ValidationUtils.java

### Purpose
Validate email, phone, password, credit card.

### Implementation

```java
package com.movie88.utils;

import android.util.Patterns;
import java.util.regex.Pattern;

public class ValidationUtils {
    
    // Regex patterns
    private static final Pattern PASSWORD_PATTERN = 
        Pattern.compile("^(?=.*[0-9])(?=.*[a-zA-Z]).{6,}$");
    
    private static final Pattern PHONE_PATTERN = 
        Pattern.compile("^(0[3|5|7|8|9])+([0-9]{8})$");
    
    /**
     * Validate email
     */
    public static boolean isValidEmail(String email) {
        return email != null && 
               !email.isEmpty() && 
               Patterns.EMAIL_ADDRESS.matcher(email).matches();
    }
    
    /**
     * Validate password
     * Requirements: At least 6 characters, 1 letter, 1 number
     */
    public static boolean isValidPassword(String password) {
        return password != null && PASSWORD_PATTERN.matcher(password).matches();
    }
    
    /**
     * Validate phone number (Vietnam format)
     */
    public static boolean isValidPhone(String phone) {
        return phone != null && PHONE_PATTERN.matcher(phone).matches();
    }
    
    /**
     * Validate name (not empty, min 2 chars)
     */
    public static boolean isValidName(String name) {
        return name != null && name.trim().length() >= 2;
    }
    
    /**
     * Check if passwords match
     */
    public static boolean doPasswordsMatch(String password, String confirmPassword) {
        return password != null && password.equals(confirmPassword);
    }
    
    /**
     * Get email error message
     */
    public static String getEmailError(String email) {
        if (email == null || email.isEmpty()) {
            return "Email không được để trống";
        }
        if (!isValidEmail(email)) {
            return "Email không hợp lệ";
        }
        return null;
    }
    
    /**
     * Get password error message
     */
    public static String getPasswordError(String password) {
        if (password == null || password.isEmpty()) {
            return "Mật khẩu không được để trống";
        }
        if (password.length() < 6) {
            return "Mật khẩu phải có ít nhất 6 ký tự";
        }
        if (!PASSWORD_PATTERN.matcher(password).matches()) {
            return "Mật khẩu phải có ít nhất 1 chữ và 1 số";
        }
        return null;
    }
    
    /**
     * Get phone error message
     */
    public static String getPhoneError(String phone) {
        if (phone == null || phone.isEmpty()) {
            return "Số điện thoại không được để trống";
        }
        if (!isValidPhone(phone)) {
            return "Số điện thoại không hợp lệ";
        }
        return null;
    }
}
```

---

## 5️⃣ NetworkUtils.java

### Purpose
Check internet connection availability.

### Implementation

```java
package com.movie88.utils;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import com.movie88.MovieApp;

public class NetworkUtils {
    
    /**
     * Check if internet connection is available
     */
    public static boolean isNetworkAvailable() {
        Context context = MovieApp.getInstance().getApplicationContext();
        ConnectivityManager cm = (ConnectivityManager) 
            context.getSystemService(Context.CONNECTIVITY_SERVICE);
        
        if (cm != null) {
            NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
            return activeNetwork != null && activeNetwork.isConnectedOrConnecting();
        }
        return false;
    }
    
    /**
     * Check if connected to WiFi
     */
    public static boolean isConnectedToWiFi() {
        Context context = MovieApp.getInstance().getApplicationContext();
        ConnectivityManager cm = (ConnectivityManager) 
            context.getSystemService(Context.CONNECTIVITY_SERVICE);
        
        if (cm != null) {
            NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
            return activeNetwork != null && 
                   activeNetwork.getType() == ConnectivityManager.TYPE_WIFI &&
                   activeNetwork.isConnected();
        }
        return false;
    }
    
    /**
     * Check if connected to Mobile Data
     */
    public static boolean isConnectedToMobileData() {
        Context context = MovieApp.getInstance().getApplicationContext();
        ConnectivityManager cm = (ConnectivityManager) 
            context.getSystemService(Context.CONNECTIVITY_SERVICE);
        
        if (cm != null) {
            NetworkInfo activeNetwork = cm.getActiveNetworkInfo();
            return activeNetwork != null && 
                   activeNetwork.getType() == ConnectivityManager.TYPE_MOBILE &&
                   activeNetwork.isConnected();
        }
        return false;
    }
}
```

---

## 6️⃣ CurrencyUtils.java

### Purpose
Format currency values (Vietnamese Dong).

### Implementation

```java
package com.movie88.utils;

import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.Locale;

public class CurrencyUtils {
    
    /**
     * Format currency (e.g., 120000 -> "120.000 đ")
     */
    public static String formatCurrency(double amount) {
        DecimalFormat formatter = new DecimalFormat("#,###");
        return formatter.format(amount) + " đ";
    }
    
    /**
     * Format currency without symbol (e.g., 120000 -> "120.000")
     */
    public static String formatCurrencyNoSymbol(double amount) {
        DecimalFormat formatter = new DecimalFormat("#,###");
        return formatter.format(amount);
    }
    
    /**
     * Parse currency string to double
     */
    public static double parseCurrency(String currencyString) {
        try {
            // Remove dots and currency symbol
            String cleaned = currencyString.replace(".", "")
                                          .replace("đ", "")
                                          .trim();
            return Double.parseDouble(cleaned);
        } catch (Exception e) {
            return 0;
        }
    }
}
```

---

## 7️⃣ Constants.java

### Purpose
Central place for all constants (URLs, keys, configs).

### Implementation

```java
package com.movie88.utils;

public class Constants {
    
    // API Configuration
    public static final String BASE_URL_DEV = "https://localhost:7001/";
    public static final String BASE_URL_PROD = "https://api.movie88.com/";
    public static final String BASE_URL = BASE_URL_DEV; // Change for production
    
    // API Timeout
    public static final int CONNECT_TIMEOUT = 30; // seconds
    public static final int READ_TIMEOUT = 30;    // seconds
    public static final int WRITE_TIMEOUT = 30;   // seconds
    
    // Cache Configuration
    public static final int CACHE_SIZE = 10 * 1024 * 1024; // 10 MB
    public static final int CACHE_DURATION = 5 * 60 * 1000; // 5 minutes
    
    // Pagination
    public static final int DEFAULT_PAGE_SIZE = 20;
    public static final int MAX_PAGE_SIZE = 50;
    
    // Booking Configuration
    public static final int SEAT_LOCK_DURATION = 15 * 60 * 1000; // 15 minutes
    public static final int MAX_SEATS_PER_BOOKING = 10;
    public static final int MAX_COMBOS_PER_BOOKING = 10;
    
    // Image Loading
    public static final String PLACEHOLDER_MOVIE = "placeholder_movie.png";
    public static final String PLACEHOLDER_COMBO = "placeholder_combo.png";
    public static final String ERROR_IMAGE = "error_image.png";
    
    // SharedPreferences Keys
    public static final String PREF_NAME = "MovieAppPrefs";
    public static final String KEY_TOKEN = "jwt_token";
    public static final String KEY_USER = "user";
    
    // Intent Keys
    public static final String EXTRA_MOVIE_ID = "MOVIE_ID";
    public static final String EXTRA_CINEMA_ID = "CINEMA_ID";
    public static final String EXTRA_SHOWTIME_ID = "SHOWTIME_ID";
    public static final String EXTRA_BOOKING_ID = "BOOKING_ID";
    public static final String EXTRA_SEAT_IDS = "SEAT_IDS";
    public static final String EXTRA_COMBO_IDS = "COMBO_IDS";
    
    // Date Formats
    public static final String DATE_FORMAT_API = "yyyy-MM-dd'T'HH:mm:ss";
    public static final String DATE_FORMAT_DISPLAY = "dd/MM/yyyy HH:mm";
    
    // VNPay Configuration
    public static final String VNPAY_RETURN_URL = BASE_URL + "api/payments/vnpay/callback";
    
    // Error Messages
    public static final String ERROR_NETWORK = "Lỗi kết nối. Vui lòng kiểm tra internet.";
    public static final String ERROR_UNKNOWN = "Đã có lỗi xảy ra. Vui lòng thử lại.";
    public static final String ERROR_TOKEN_EXPIRED = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
}
```

---

## 8️⃣ ApiCallback.java

### Purpose
Generic callback interface cho API calls.

### Implementation

```java
package com.movie88.utils;

public interface ApiCallback<T> {
    void onSuccess(T data);
    void onError(String errorMessage);
}
```

---

## 🎯 Usage Examples

### TokenManager
```java
// Save token after login
TokenManager.getInstance().saveToken(response.getToken());

// Check if token valid
if (TokenManager.getInstance().hasValidToken()) {
    // Token valid
} else {
    // Token expired, refresh or login
}

// Get user ID
int userId = TokenManager.getInstance().getUserIdFromToken();
```

### SharedPrefsManager
```java
// Save user
SharedPrefsManager.getInstance().saveUser(user);

// Get user
User user = SharedPrefsManager.getInstance().getUser();

// Check logged in
if (SharedPrefsManager.getInstance().isLoggedIn()) {
    // User logged in
}
```

### ValidationUtils
```java
// Validate email
String emailError = ValidationUtils.getEmailError(email);
if (emailError != null) {
    etEmail.setError(emailError);
    return;
}

// Validate password
if (!ValidationUtils.isValidPassword(password)) {
    etPassword.setError("Mật khẩu không hợp lệ");
}
```

### DateUtils
```java
// Parse API date
Date date = DateUtils.parseApiDateTime("2025-10-29T19:30:00");

// Format for display
String displayDate = DateUtils.formatDateTime(date); // "29/10/2025 19:30"

// Get relative string
String relative = DateUtils.getRelativeDateString(date); // "Hôm nay, 19:30"
```

---

**Last Updated**: October 29, 2025
