# Google Login Implementation Guide

## 📋 Overview

Feature **Login with Google** đã được implement thành công cho Movie88 API. User có thể login bằng Google account và hệ thống sẽ tự động tạo account nếu chưa tồn tại.

## 🎯 Features

✅ **Auto-create user account** - Tự động tạo user + customer profile khi login lần đầu  
✅ **Email verification** - Auto-verify email nếu Google email đã verified  
✅ **JWT Authentication** - Trả về access token + refresh token  
✅ **Seamless experience** - Không cần đăng ký riêng, chỉ cần login với Google  

## 🏗️ Implementation Details

### 1. **Files Created**

| File | Purpose |
|------|---------|
| `Movie88.Application/DTOs/Auth/GoogleLoginDTO.cs` | Request/Response DTOs |
| `Movie88.Application/Interfaces/IGoogleAuthService.cs` | Service interface |
| `Movie88.Application/Services/GoogleAuthService.cs` | Google token verification |
| `tests/GoogleLogin.http` | API testing file |

### 2. **Files Modified**

| File | Changes |
|------|---------|
| `Movie88.Application/Interfaces/IAuthService.cs` | Added `GoogleLoginAsync` method |
| `Movie88.Application/Services/AuthService.cs` | Implemented Google login logic |
| `Movie88.Application/Configuration/ServiceExtensions.cs` | Registered GoogleAuthService |
| `Movie88.WebApi/Controllers/AuthController.cs` | Added `/auth/google-login` endpoint |
| `Movie88.WebApi/appsettings.json` | Added GoogleOAuth config |
| `Movie88.WebApi/appsettings.Development.json` | Added GoogleOAuth config |

### 3. **NuGet Packages Added**

```xml
<PackageReference Include="Google.Apis.Auth" Version="1.72.0" />
```

## 🔐 Configuration

### appsettings.json
```json
{
  "GoogleOAuth": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com"
  }
}
```

⚠️ **Important**: 
- ClientId must match the one configured in Android app's Google Sign-In setup
- Get your Client ID from [Google Cloud Console](https://console.cloud.google.com/apis/credentials)
- ClientSecret is NOT needed for Android apps (removed for security)

## 📱 Android Integration

### Step 1: Configure Google Sign-In

```kotlin
// build.gradle.kts (app level)
dependencies {
    implementation("com.google.android.gms:play-services-auth:21.0.0")
}
```

### Step 2: Setup GoogleSignInOptions

```kotlin
val gso = GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
    .requestIdToken("YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com")
    .requestEmail()
    .build()

val googleSignInClient = GoogleSignIn.getClient(context, gso)
```

### Step 3: Handle Sign-In Result

```kotlin
// Launch sign-in intent
val signInIntent = googleSignInClient.signInIntent
startActivityForResult(signInIntent, RC_SIGN_IN)

// Handle result
override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
    super.onActivityResult(requestCode, resultCode, data)
    
    if (requestCode == RC_SIGN_IN) {
        val task = GoogleSignIn.getSignedInAccountFromIntent(data)
        try {
            val account = task.getResult(ApiException::class.java)
            val idToken = account?.idToken
            
            // Send idToken to backend
            loginWithGoogle(idToken)
        } catch (e: ApiException) {
            Log.e("GoogleLogin", "Sign in failed", e)
        }
    }
}
```

### Step 4: Call Backend API

```kotlin
suspend fun loginWithGoogle(idToken: String?) {
    if (idToken == null) return
    
    val response = apiService.googleLogin(GoogleLoginRequest(idToken))
    
    if (response.success) {
        // Save tokens
        tokenManager.saveAccessToken(response.data.token)
        tokenManager.saveRefreshToken(response.data.refreshToken)
        
        // Navigate to home screen
        navigateToHome()
    }
}
```

## 🚀 API Endpoint

### POST `/api/auth/google-login`

**Request:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6I..."
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Google login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "abc123...",
    "expiresAt": "2025-11-06T12:00:00Z",
    "user": {
      "userId": 123,
      "fullName": "John Doe",
      "email": "john.doe@gmail.com",
      "phoneNumber": null,
      "roleId": 3,
      "roleName": "Customer"
    }
  }
}
```

**Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "statusCode": 401,
  "message": "Invalid Google token"
}
```

## 🔄 Login Flow

```
┌─────────────┐
│ Android App │
└──────┬──────┘
       │
       │ 1. User clicks "Sign in with Google"
       │
       ▼
┌────────────────┐
│ Google Sign-In │
│      SDK       │
└───────┬────────┘
        │
        │ 2. Returns ID Token
        │
        ▼
┌─────────────┐
│ Android App │ ───────3. POST /auth/google-login────▶ ┌──────────────┐
└─────────────┘                                         │  Backend API │
                                                        └──────┬───────┘
                                                               │
                                                               │ 4. Verify token with Google
                                                               │
                                                        ┌──────▼───────┐
                                                        │   Google     │
                                                        │   Servers    │
                                                        └──────┬───────┘
                                                               │
                                                               │ 5. Token valid ✅
                                                               │
                                                        ┌──────▼───────┐
                                                        │   Backend    │
                                                        │   Database   │
                                                        └──────┬───────┘
                                                               │
                                                               │ 6. Check if user exists
                                                               │ 7. Create user if new
                                                               │ 8. Generate JWT tokens
                                                               │
┌─────────────┐                                        ┌──────▼───────┐
│ Android App │ ◀─────9. Return tokens + user info──── │  Backend API │
└─────────────┘                                        └──────────────┘
```

## 🧪 Testing

### Using REST Client (VSCode)

1. Open `tests/GoogleLogin.http`
2. Get a real Google ID token from Android app (see Android Integration above)
3. Replace `YOUR_GOOGLE_ID_TOKEN_HERE` with the actual token
4. Click "Send Request"

### Test Scenarios

| Test Case | Expected Result |
|-----------|----------------|
| **New user login** | User + Customer created, returns tokens |
| **Existing user login** | Returns tokens for existing account |
| **Invalid token** | 401 Unauthorized |
| **Empty token** | 400 Bad Request |
| **Use access token** | Can access protected endpoints |

## 🔒 Security

- **Token Verification**: ID tokens are verified with Google's servers using Google.Apis.Auth library
- **Email Verification**: Auto-verified if Google email is verified
- **Password Security**: Random password generated (user can reset later if needed)
- **Token Expiration**: Access tokens expire after 60 minutes
- **Refresh Tokens**: Valid for 7 days

## 📊 Database Changes

### New User Created via Google Login

**users table:**
```sql
INSERT INTO users (fullname, email, passwordhash, roleid, isactive, isverified, createdat)
VALUES ('John Doe', 'john@gmail.com', '$2a$11$...', 3, true, true, NOW());
```

**customers table:**
```sql
INSERT INTO customers (userid, fullname, email, createdat)
VALUES (123, 'John Doe', 'john@gmail.com', NOW());
```

**userrefreshtokens table:**
```sql
INSERT INTO userrefreshtokens (userid, token, createdat, updatedat, revoked)
VALUES ('123', 'abc123...', NOW(), NOW(), false);
```

## ✅ Checklist

- [x] Install Google.Apis.Auth package
- [x] Create DTOs for Google login
- [x] Implement GoogleAuthService
- [x] Update AuthService with GoogleLoginAsync
- [x] Add controller endpoint
- [x] Configure GoogleOAuth settings
- [x] Register services in DI
- [x] Create test file
- [x] Build successfully
- [ ] Test with real Google ID token
- [ ] Deploy to Railway
- [ ] Update Android app to use new endpoint

## 🐛 Troubleshooting

### Error: "Invalid Google token"
- Check that ClientId in appsettings.json matches Android app configuration
- Ensure ID token is fresh (not expired)
- Verify network connectivity

### Error: "Failed to create user"
- Check database connection
- Verify users/customers tables exist
- Check for unique constraint violations (duplicate email)

### Error: "Package downgrade: Newtonsoft.Json"
- Update Newtonsoft.Json to version 13.0.4 in Movie88.Infrastructure
- Run `dotnet add Movie88.Infrastructure package Newtonsoft.Json --version 13.0.4`

## 📚 References

- [Google Sign-In for Android](https://developers.google.com/identity/sign-in/android/start-integrating)
- [Google.Apis.Auth Documentation](https://googleapis.github.io/google-api-dotnet-client/docs/doxygen/1.40.0/namespace_google_1_1_apis_1_1_auth.html)
- [JWT Authentication Best Practices](https://auth0.com/blog/a-look-at-the-latest-draft-for-jwt-bcp/)

## 🎉 Done!

Google Login feature is fully implemented and ready to use! 🚀
