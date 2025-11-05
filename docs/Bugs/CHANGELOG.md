# 🐛 Bug Fixes & Changes Log

**Purpose**: Document all bugs reported by Frontend team and backend fixes  
**Created**: November 5, 2025  
**Maintained by**: Backend Team

---

## 📋 Table of Contents
- [Bug #1: Forgot-Password Email Validation](#bug-1-forgot-password-email-validation)

---

## Bug #1: Forgot-Password Email Validation

### 📅 Date
**Reported**: November 5, 2025  
**Fixed**: November 5, 2025  
**Status**: ✅ **RESOLVED**

### 🔴 Problem Reported by Frontend
**Endpoint**: `POST /api/auth/forgot-password`

**Issue**:
```
Khi user nhập email không tồn tại trong hệ thống:
- Backend vẫn trả về 200 OK success
- Frontend hiển thị "OTP đã được gửi"
- User không nhận được OTP (vì email không tồn tại)
- User bị confused, không biết email sai hay OTP chưa tới
- Frontend không thể phân biệt email có tồn tại hay không
```

**Expected Behavior**:
```
- Nếu email không tồn tại → return 400 Bad Request với message rõ ràng
- Nếu email tồn tại → send OTP và return 200 OK với data
- Frontend có thể hiển thị lỗi ngay lập tức
```

### ✅ Solution Implemented

#### Before Fix (Security-First Approach)
```csharp
// OLD LOGIC - Always return success (prevent email enumeration)
public async Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(...)
{
    var user = await _userRepository.GetByEmailAsync(request.Email);
    
    // Return same response regardless of email existence
    var response = new ForgotPasswordResponseDTO { ... };
    
    if (user == null)
    {
        // Don't send OTP, but return success (security)
        return response;
    }
    
    // Send OTP only if user exists
    await _otpService.SendOtpAsync(...);
    return response;
}
```

**Security Trade-off**:
- ✅ Prevents email enumeration attacks
- ❌ Poor UX: User doesn't know if email is wrong
- ❌ Wastes user time (wait 10 minutes for nothing)

#### After Fix (UX-First Approach)
```csharp
// NEW LOGIC - Check email and throw exception if not found
public async Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(...)
{
    var user = await _userRepository.GetByEmailAsync(request.Email);
    
    // Check if email exists
    if (user == null)
    {
        throw new InvalidOperationException(
            "Email không tồn tại trong hệ thống. Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới."
        );
    }
    
    // Send OTP only for existing emails
    await _otpService.SendOtpAsync(...);
    
    return new ForgotPasswordResponseDTO
    {
        Email = request.Email,
        OtpType = "PasswordReset",
        ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        Message = "OTP đã được gửi đến email của bạn..."
    };
}
```

**UX Improvement**:
- ✅ User knows immediately if email is wrong
- ✅ No fake OTP sent to non-existent emails
- ✅ Better error messages
- ⚠️ Security: Allows checking if email exists in system

### 📝 Changes Made

#### Files Modified:
1. **`Movie88.Application/Services/AuthService.cs`**
   - Updated `ForgotPasswordAsync` method
   - Added email existence validation
   - Throws `InvalidOperationException` if email not found
   - Only sends OTP for existing emails

2. **`Movie88.WebApi/Controllers/AuthController.cs`**
   - Added try-catch block for `InvalidOperationException`
   - Returns 400 Bad Request if email not found
   - Returns structured Response<ForgotPasswordResponseDTO> on success

3. **`docs/screens/01-Authentication.md`**
   - Updated forgot-password endpoint documentation
   - Added 400 Bad Request response example
   - Updated business logic description

### 🔄 API Response Changes

#### Success Response (200 OK)
```json
{
  "success": true,
  "statusCode": 200,
  "message": "OTP đã được gửi đến email của bạn",
  "data": {
    "email": "customer@example.com",
    "otpType": "PasswordReset",
    "expiresAt": "2025-11-05T10:30:00Z",
    "message": "OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư (kể cả thư mục spam)."
  }
}
```

#### Error Response (400 Bad Request) - NEW
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Email không tồn tại trong hệ thống. Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới.",
  "data": null
}
```

### 📱 Frontend Integration Guide

#### Android/Kotlin Code Example
```kotlin
// Call forgot-password API
viewModelScope.launch {
    try {
        val response = authRepository.forgotPassword(email)
        
        if (response.isSuccessful && response.body()?.success == true) {
            // Email exists, OTP sent
            val data = response.body()?.data
            showSuccess("OTP đã được gửi đến ${data?.email}")
            
            // Navigate to reset password screen with data
            navigateToResetPassword(
                email = data?.email,
                expiresAt = data?.expiresAt
            )
        }
    } catch (e: HttpException) {
        if (e.code() == 400) {
            // Email not found
            val errorBody = e.response()?.errorBody()?.string()
            val error = Gson().fromJson(errorBody, ErrorResponse::class.java)
            
            showError(error.message) // "Email không tồn tại trong hệ thống..."
            // Option: Show "Register" button
        }
    }
}
```

#### Error Handling
```kotlin
sealed class ForgotPasswordResult {
    data class Success(val data: ForgotPasswordResponseDTO) : ForgotPasswordResult()
    data class EmailNotFound(val message: String) : ForgotPasswordResult()
    data class NetworkError(val message: String) : ForgotPasswordResult()
}

// In ViewModel
fun forgotPassword(email: String) {
    viewModelScope.launch {
        _forgotPasswordState.value = ForgotPasswordState.Loading
        
        val result = authRepository.forgotPassword(email)
        _forgotPasswordState.value = when (result) {
            is ForgotPasswordResult.Success -> {
                // Navigate to reset password
                ForgotPasswordState.Success(result.data)
            }
            is ForgotPasswordResult.EmailNotFound -> {
                // Show error with register option
                ForgotPasswordState.Error(
                    message = result.message,
                    showRegisterButton = true
                )
            }
            else -> ForgotPasswordState.Error(result.message)
        }
    }
}
```

### ⚠️ Security Considerations

**Trade-off Decision**: UX-First vs Security-First

| Aspect | Before (Security-First) | After (UX-First) |
|--------|-------------------------|------------------|
| Email Enumeration | ✅ Protected | ⚠️ Allowed (attacker can check emails) |
| User Experience | ❌ Confusing | ✅ Clear error messages |
| Fake OTPs | ❌ Generated (wasted) | ✅ Not generated |
| User Trust | ❌ Low (misleading) | ✅ High (transparent) |

**Why we chose UX-First**:
1. Movie booking app (not banking/high-security)
2. Email enumeration risk is low for this use case
3. User frustration with unclear errors hurts business more
4. Majority of modern apps use this approach (Google, Facebook, etc.)
5. Can add CAPTCHA or rate limiting if enumeration becomes issue

### 🧪 Testing

#### Test Case 1: Email Not Found
```http
POST https://movie88aspnet-app.up.railway.app/api/auth/forgot-password
Content-Type: application/json

{
  "email": "nonexistent@fake.com"
}

Expected: 400 Bad Request
{
  "success": false,
  "statusCode": 400,
  "message": "Email không tồn tại trong hệ thống. Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới."
}
```

#### Test Case 2: Email Exists
```http
POST https://movie88aspnet-app.up.railway.app/api/auth/forgot-password
Content-Type: application/json

{
  "email": "customer@example.com"
}

Expected: 200 OK
{
  "success": true,
  "statusCode": 200,
  "message": "OTP đã được gửi đến email của bạn",
  "data": {
    "email": "customer@example.com",
    "otpType": "PasswordReset",
    "expiresAt": "2025-11-05T10:30:00Z",
    "message": "OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư (kể cả thư mục spam)."
  }
}
```

### 📊 Impact Summary

**Backend Changes**: 2 files modified  
**Frontend Impact**: Must handle 400 status code  
**Breaking Change**: ⚠️ YES - Frontend must update error handling  
**Deployment**: ✅ Deployed to Railway  
**Database**: No schema changes required

### 🚀 Deployment Status

- ✅ Code committed to `main` branch
- ✅ Built successfully (0 errors)
- ✅ Deployed to Railway: https://movie88aspnet-app.up.railway.app
- ✅ Documentation updated
- ⏳ Waiting for frontend team to test and integrate

### 📞 Contact

**Backend Team**: Available for questions  
**Test Credentials**:
```
Existing email: customer@example.com
Non-existent email: fake@test.com
```

---

**Legend**:
- ✅ Completed
- ⏳ In Progress
- ❌ Issue
- ⚠️ Warning/Important
