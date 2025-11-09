# Google Login - Android Integration Guide (Java + XML)

## 🎯 Overview

Hướng dẫn chi tiết tích hợp **Google Sign-In** cho Android app (Java + XML) với Movie88 API.

## 📋 Prerequisites

- Android Studio
- Minimum SDK: 21 (Android 5.0)
- Google Play Services
- **Client ID**: `1005211565279-3ffgkjhh0vd8ans8hv8gt9152gf3pim6.apps.googleusercontent.com`

---

## 🔧 Step 1: Configure build.gradle

### build.gradle (Project level)
```gradle
buildscript {
    repositories {
        google()
        mavenCentral()
    }
    dependencies {
        classpath 'com.android.tools.build:gradle:8.1.0'
        classpath 'com.google.gms:google-services:4.4.0'
    }
}
```

### build.gradle (App level)
```gradle
plugins {
    id 'com.android.application'
    id 'com.google.gms.google-services'
}

android {
    namespace 'com.movie88.app'
    compileSdk 34

    defaultConfig {
        applicationId "com.movie88.app"
        minSdk 21
        targetSdk 34
        versionCode 1
        versionName "1.0"
    }

    compileOptions {
        sourceCompatibility JavaVersion.VERSION_1_8
        targetCompatibility JavaVersion.VERSION_1_8
    }
}

dependencies {
    // Google Sign-In
    implementation 'com.google.android.gms:play-services-auth:21.0.0'
    
    // Retrofit for API calls
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    
    // Other dependencies
    implementation 'androidx.appcompat:appcompat:1.6.1'
    implementation 'com.google.android.material:material:1.11.0'
    implementation 'androidx.constraintlayout:constraintlayout:2.1.4'
}
```

---

## 🎨 Step 2: Create UI Layout

### res/layout/activity_login.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.constraintlayout.widget.ConstraintLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    xmlns:tools="http://schemas.android.com/tools"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:padding="24dp"
    tools:context=".LoginActivity">

    <!-- Logo -->
    <ImageView
        android:id="@+id/imgLogo"
        android:layout_width="150dp"
        android:layout_height="150dp"
        android:src="@drawable/ic_movie_logo"
        android:contentDescription="@string/app_logo"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="80dp"/>

    <!-- App Title -->
    <TextView
        android:id="@+id/tvTitle"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="@string/movie88"
        android:textSize="32sp"
        android:textStyle="bold"
        android:textColor="@color/primary"
        app:layout_constraintTop_toBottomOf="@id/imgLogo"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="24dp"/>

    <!-- Welcome Text -->
    <TextView
        android:id="@+id/tvWelcome"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="@string/sign_in_to_continue"
        android:textSize="16sp"
        android:textColor="@color/text_secondary"
        app:layout_constraintTop_toBottomOf="@id/tvTitle"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="8dp"/>

    <!-- Email Input -->
    <com.google.android.material.textfield.TextInputLayout
        android:id="@+id/tilEmail"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="@string/email"
        app:layout_constraintTop_toBottomOf="@id/tvWelcome"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="48dp">

        <com.google.android.material.textfield.TextInputEditText
            android:id="@+id/etEmail"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:inputType="textEmailAddress"/>
    </com.google.android.material.textfield.TextInputLayout>

    <!-- Password Input -->
    <com.google.android.material.textfield.TextInputLayout
        android:id="@+id/tilPassword"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="@string/password"
        app:passwordToggleEnabled="true"
        app:layout_constraintTop_toBottomOf="@id/tilEmail"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="16dp">

        <com.google.android.material.textfield.TextInputEditText
            android:id="@+id/etPassword"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:inputType="textPassword"/>
    </com.google.android.material.textfield.TextInputLayout>

    <!-- Login Button -->
    <com.google.android.material.button.MaterialButton
        android:id="@+id/btnLogin"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="@string/sign_in"
        android:textSize="16sp"
        android:padding="16dp"
        app:layout_constraintTop_toBottomOf="@id/tilPassword"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="24dp"/>

    <!-- OR Divider -->
    <View
        android:id="@+id/dividerLeft"
        android:layout_width="0dp"
        android:layout_height="1dp"
        android:background="@color/divider"
        app:layout_constraintTop_toBottomOf="@id/btnLogin"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toStartOf="@id/tvOr"
        android:layout_marginTop="24dp"
        android:layout_marginEnd="16dp"/>

    <TextView
        android:id="@+id/tvOr"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="@string/or"
        android:textColor="@color/text_secondary"
        app:layout_constraintTop_toTopOf="@id/dividerLeft"
        app:layout_constraintBottom_toBottomOf="@id/dividerLeft"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

    <View
        android:id="@+id/dividerRight"
        android:layout_width="0dp"
        android:layout_height="1dp"
        android:background="@color/divider"
        app:layout_constraintTop_toTopOf="@id/dividerLeft"
        app:layout_constraintStart_toEndOf="@id/tvOr"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginStart="16dp"/>

    <!-- Google Sign-In Button -->
    <com.google.android.gms.common.SignInButton
        android:id="@+id/btnGoogleSignIn"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        app:layout_constraintTop_toBottomOf="@id/dividerLeft"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="24dp"/>

    <!-- OR: Custom Google Button -->
    <com.google.android.material.button.MaterialButton
        android:id="@+id/btnGoogleSignInCustom"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="@string/sign_in_with_google"
        android:textSize="16sp"
        android:padding="16dp"
        android:drawableStart="@drawable/ic_google"
        android:drawablePadding="16dp"
        style="@style/Widget.Material3.Button.OutlinedButton"
        app:layout_constraintTop_toBottomOf="@id/dividerLeft"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="24dp"
        android:visibility="gone"/>

    <!-- Register Link -->
    <TextView
        android:id="@+id/tvRegister"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="@string/dont_have_account"
        android:textColor="@color/primary"
        android:textStyle="bold"
        app:layout_constraintTop_toBottomOf="@id/btnGoogleSignIn"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"
        android:layout_marginTop="24dp"/>

    <!-- Progress Bar -->
    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:visibility="gone"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toBottomOf="parent"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

</androidx.constraintlayout.widget.ConstraintLayout>
```

---

## 📱 Step 3: Create API Models

### models/GoogleLoginRequest.java
```java
package com.movie88.app.models;

public class GoogleLoginRequest {
    private String idToken;

    public GoogleLoginRequest(String idToken) {
        this.idToken = idToken;
    }

    public String getIdToken() {
        return idToken;
    }

    public void setIdToken(String idToken) {
        this.idToken = idToken;
    }
}
```

### models/LoginResponse.java
```java
package com.movie88.app.models;

public class LoginResponse {
    private boolean success;
    private int statusCode;
    private String message;
    private Data data;

    public static class Data {
        private String token;
        private String refreshToken;
        private String expiresAt;
        private User user;

        // Getters and setters
        public String getToken() { return token; }
        public void setToken(String token) { this.token = token; }
        
        public String getRefreshToken() { return refreshToken; }
        public void setRefreshToken(String refreshToken) { this.refreshToken = refreshToken; }
        
        public String getExpiresAt() { return expiresAt; }
        public void setExpiresAt(String expiresAt) { this.expiresAt = expiresAt; }
        
        public User getUser() { return user; }
        public void setUser(User user) { this.user = user; }
    }

    public static class User {
        private int userId;
        private String fullName;
        private String email;
        private String phoneNumber;
        private int roleId;
        private String roleName;

        // Getters and setters
        public int getUserId() { return userId; }
        public void setUserId(int userId) { this.userId = userId; }
        
        public String getFullName() { return fullName; }
        public void setFullName(String fullName) { this.fullName = fullName; }
        
        public String getEmail() { return email; }
        public void setEmail(String email) { this.email = email; }
        
        public String getPhoneNumber() { return phoneNumber; }
        public void setPhoneNumber(String phoneNumber) { this.phoneNumber = phoneNumber; }
        
        public int getRoleId() { return roleId; }
        public void setRoleId(int roleId) { this.roleId = roleId; }
        
        public String getRoleName() { return roleName; }
        public void setRoleName(String roleName) { this.roleName = roleName; }
    }

    // Getters and setters
    public boolean isSuccess() { return success; }
    public void setSuccess(boolean success) { this.success = success; }
    
    public int getStatusCode() { return statusCode; }
    public void setStatusCode(int statusCode) { this.statusCode = statusCode; }
    
    public String getMessage() { return message; }
    public void setMessage(String message) { this.message = message; }
    
    public Data getData() { return data; }
    public void setData(Data data) { this.data = data; }
}
```

---

## 🌐 Step 4: Create API Service

### api/ApiService.java
```java
package com.movie88.app.api;

import com.movie88.app.models.GoogleLoginRequest;
import com.movie88.app.models.LoginResponse;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.POST;

public interface ApiService {
    
    @POST("auth/google-login")
    Call<LoginResponse> googleLogin(@Body GoogleLoginRequest request);
    
    // Other API methods...
}
```

### api/ApiClient.java
```java
package com.movie88.app.api;

import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class ApiClient {
    private static final String BASE_URL = "https://movie88aspnet-app.up.railway.app/api/";
    private static Retrofit retrofit = null;

    public static Retrofit getClient() {
        if (retrofit == null) {
            retrofit = new Retrofit.Builder()
                    .baseUrl(BASE_URL)
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return retrofit;
    }

    public static ApiService getApiService() {
        return getClient().create(ApiService.class);
    }
}
```

---

## 💾 Step 5: Create Token Manager

### utils/TokenManager.java
```java
package com.movie88.app.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class TokenManager {
    private static final String PREF_NAME = "Movie88Prefs";
    private static final String KEY_ACCESS_TOKEN = "access_token";
    private static final String KEY_REFRESH_TOKEN = "refresh_token";
    private static final String KEY_USER_ID = "user_id";
    private static final String KEY_USER_NAME = "user_name";
    private static final String KEY_USER_EMAIL = "user_email";

    private final SharedPreferences preferences;

    public TokenManager(Context context) {
        preferences = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }

    public void saveTokens(String accessToken, String refreshToken) {
        preferences.edit()
                .putString(KEY_ACCESS_TOKEN, accessToken)
                .putString(KEY_REFRESH_TOKEN, refreshToken)
                .apply();
    }

    public void saveUserInfo(int userId, String name, String email) {
        preferences.edit()
                .putInt(KEY_USER_ID, userId)
                .putString(KEY_USER_NAME, name)
                .putString(KEY_USER_EMAIL, email)
                .apply();
    }

    public String getAccessToken() {
        return preferences.getString(KEY_ACCESS_TOKEN, null);
    }

    public String getRefreshToken() {
        return preferences.getString(KEY_REFRESH_TOKEN, null);
    }

    public boolean isLoggedIn() {
        return getAccessToken() != null;
    }

    public void clearTokens() {
        preferences.edit().clear().apply();
    }
}
```

---

## 🎯 Step 6: Implement LoginActivity

### LoginActivity.java
```java
package com.movie88.app;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.common.SignInButton;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.tasks.Task;
import com.movie88.app.api.ApiClient;
import com.movie88.app.api.ApiService;
import com.movie88.app.models.GoogleLoginRequest;
import com.movie88.app.models.LoginResponse;
import com.movie88.app.utils.TokenManager;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {
    
    private static final String TAG = "LoginActivity";
    private static final int RC_SIGN_IN = 9001;
    private static final String CLIENT_ID = "1005211565279-3ffgkjhh0vd8ans8hv8gt9152gf3pim6.apps.googleusercontent.com";

    private GoogleSignInClient mGoogleSignInClient;
    private SignInButton btnGoogleSignIn;
    private ProgressBar progressBar;
    private TokenManager tokenManager;
    private ApiService apiService;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        // Initialize views
        btnGoogleSignIn = findViewById(R.id.btnGoogleSignIn);
        progressBar = findViewById(R.id.progressBar);

        // Initialize managers
        tokenManager = new TokenManager(this);
        apiService = ApiClient.getApiService();

        // Check if already logged in
        if (tokenManager.isLoggedIn()) {
            navigateToHome();
            return;
        }

        // Configure Google Sign-In
        GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
                .requestIdToken(CLIENT_ID)
                .requestEmail()
                .build();

        mGoogleSignInClient = GoogleSignIn.getClient(this, gso);

        // Set button text and size
        btnGoogleSignIn.setSize(SignInButton.SIZE_WIDE);

        // Set click listener
        btnGoogleSignIn.setOnClickListener(v -> signInWithGoogle());
    }

    /**
     * Start Google Sign-In flow
     */
    private void signInWithGoogle() {
        Intent signInIntent = mGoogleSignInClient.getSignInIntent();
        startActivityForResult(signInIntent, RC_SIGN_IN);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == RC_SIGN_IN) {
            Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
            handleSignInResult(task);
        }
    }

    /**
     * Handle Google Sign-In result
     */
    private void handleSignInResult(Task<GoogleSignInAccount> completedTask) {
        try {
            GoogleSignInAccount account = completedTask.getResult(ApiException.class);
            
            // Get ID Token
            String idToken = account.getIdToken();
            
            if (idToken != null) {
                Log.d(TAG, "ID Token: " + idToken);
                // Send token to backend
                loginWithBackend(idToken);
            } else {
                Log.e(TAG, "ID Token is null");
                Toast.makeText(this, "Failed to get ID Token", Toast.LENGTH_SHORT).show();
            }
            
        } catch (ApiException e) {
            Log.e(TAG, "Sign in failed: " + e.getStatusCode(), e);
            Toast.makeText(this, "Google Sign-In failed", Toast.LENGTH_SHORT).show();
        }
    }

    /**
     * Send ID Token to backend API
     */
    private void loginWithBackend(String idToken) {
        showLoading(true);

        GoogleLoginRequest request = new GoogleLoginRequest(idToken);
        
        apiService.googleLogin(request).enqueue(new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                showLoading(false);

                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginResponse = response.body();
                    
                    if (loginResponse.isSuccess()) {
                        // Save tokens
                        LoginResponse.Data data = loginResponse.getData();
                        tokenManager.saveTokens(data.getToken(), data.getRefreshToken());
                        
                        // Save user info
                        LoginResponse.User user = data.getUser();
                        tokenManager.saveUserInfo(
                            user.getUserId(),
                            user.getFullName(),
                            user.getEmail()
                        );

                        Toast.makeText(LoginActivity.this, 
                            "Welcome, " + user.getFullName() + "!", 
                            Toast.LENGTH_SHORT).show();

                        // Navigate to home
                        navigateToHome();
                    } else {
                        Toast.makeText(LoginActivity.this, 
                            loginResponse.getMessage(), 
                            Toast.LENGTH_SHORT).show();
                    }
                } else {
                    Toast.makeText(LoginActivity.this, 
                        "Login failed: " + response.message(), 
                        Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                showLoading(false);
                Log.e(TAG, "API call failed", t);
                Toast.makeText(LoginActivity.this, 
                    "Network error: " + t.getMessage(), 
                    Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * Show/hide loading indicator
     */
    private void showLoading(boolean show) {
        progressBar.setVisibility(show ? View.VISIBLE : View.GONE);
        btnGoogleSignIn.setEnabled(!show);
    }

    /**
     * Navigate to home screen
     */
    private void navigateToHome() {
        Intent intent = new Intent(this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }
}
```

---

## 📝 Step 7: Add String Resources

### res/values/strings.xml
```xml
<resources>
    <string name="app_name">Movie88</string>
    <string name="movie88">Movie88</string>
    <string name="app_logo">App Logo</string>
    <string name="sign_in_to_continue">Sign in to continue</string>
    <string name="email">Email</string>
    <string name="password">Password</string>
    <string name="sign_in">Sign In</string>
    <string name="or">OR</string>
    <string name="sign_in_with_google">Sign in with Google</string>
    <string name="dont_have_account">Don\'t have an account? Sign up</string>
</resources>
```

### res/values/colors.xml
```xml
<resources>
    <color name="primary">#FF6200EE</color>
    <color name="text_secondary">#757575</color>
    <color name="divider">#E0E0E0</color>
</resources>
```

---

## 🔐 Step 8: Configure AndroidManifest.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- Permissions -->
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

    <application
        android:allowBackup="true"
        android:icon="@mipmap/ic_launcher"
        android:label="@string/app_name"
        android:roundIcon="@mipmap/ic_launcher_round"
        android:supportsRtl="true"
        android:theme="@style/Theme.Movie88"
        android:usesCleartextTraffic="true">

        <!-- Login Activity (Launcher) -->
        <activity
            android:name=".LoginActivity"
            android:exported="true">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>

        <!-- Main Activity -->
        <activity
            android:name=".MainActivity"
            android:exported="false" />

    </application>

</manifest>
```

---

## ✅ Testing Checklist

- [ ] **Build project** - Ensure no compilation errors
- [ ] **Run on device/emulator** - Must have Google Play Services
- [ ] **Click Google Sign-In button** - Opens Google account picker
- [ ] **Select Google account** - Redirects back to app
- [ ] **Check API response** - Receives tokens and user info
- [ ] **Verify token storage** - Tokens saved in SharedPreferences
- [ ] **Navigate to home** - Automatically after successful login
- [ ] **Re-open app** - Should skip login if token exists

---

## 🐛 Troubleshooting

### Error: "Developer Error" or "Sign-In failed: 10"
**Solution**: 
- Verify Client ID matches in both Google Console and code
- Ensure SHA-1 fingerprint is added to Google Console
- Check package name matches

### Error: "Network error" or API call failed
**Solution**:
- Check internet permission in AndroidManifest.xml
- Verify API base URL is correct
- Test API endpoint with Postman first

### Error: "ID Token is null"
**Solution**:
- Ensure `.requestIdToken(CLIENT_ID)` is called in GoogleSignInOptions
- Verify Client ID is correct (Android type, not Web type)

### How to get SHA-1 fingerprint:
```bash
# Debug keystore
keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android

# Release keystore
keytool -list -v -keystore /path/to/your/keystore.jks -alias your_alias_name
```

---

## 📚 API Configuration

**Backend API:**
- Base URL: `https://movie88aspnet-app.up.railway.app/api/`
- Endpoint: `POST /auth/google-login`
- Client ID: `1005211565279-3ffgkjhh0vd8ans8hv8gt9152gf3pim6.apps.googleusercontent.com`

**Request:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIs..."
}
```

**Response:**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Google login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "abc123...",
    "user": {
      "userId": 123,
      "fullName": "John Doe",
      "email": "john@gmail.com"
    }
  }
}
```

---

## 🎉 Done!

Your Android app is now configured for Google Sign-In with Movie88 API! 🚀

### Next Steps:
1. Test on real device with Google Play Services
2. Implement logout functionality
3. Add token refresh mechanism
4. Handle expired tokens gracefully
