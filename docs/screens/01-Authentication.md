# 🔐 Screen 1: Authentication (10 Endpoints)

**Status**: ✅ **COMPLETED** (10/10 endpoints - 100%)  
**Updated**: November 4, 2025 - Added OTP Email Verification & Password Reset  
**Assigned**: Trung

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Screen | Auth | Status | Assign |
|---|--------|----------|--------|------|--------|--------|
| 1 | POST | `/api/auth/login` | LoginActivity | ❌ | ✅ DONE | Trung |
| 2 | POST | `/api/auth/register` | RegisterActivity | ❌ | ✅ DONE | Trung |
| 3 | POST | `/api/auth/forgot-password` | ForgotPasswordActivity | ❌ | ✅ DONE | Trung |
| 4 | POST | `/api/auth/refresh-token` | SplashActivity | ✅ | ✅ DONE | Trung |
| 5 | POST | `/api/auth/logout` | - | ✅ | ✅ DONE | Trung |
| 6 | POST | `/api/auth/change-password` | - | ✅ | ✅ DONE | Trung |
| 7 | POST | `/api/auth/send-otp` | VerifyEmailActivity | ❌ | ✅ NEW | Trung |
| 8 | POST | `/api/auth/verify-otp` | VerifyEmailActivity | ❌ | ✅ NEW | Trung |
| 9 | POST | `/api/auth/resend-otp` | VerifyEmailActivity | ❌ | ✅ NEW | Trung |
| 10 | POST | `/api/auth/reset-password` | ResetPasswordActivity | ❌ | ✅ NEW | Trung |

**✨ New Features**:
- 🔐 Email verification with OTP (6-digit code, 10 minutes expiry)
- 📧 Resend API integration (movie88@ezyfix.site)
- ✉️ Professional HTML email templates
- 🎁 Welcome email after verification
- 🔒 Password reset with OTP verification & audit trail

---

## 🎯 1. POST /api/auth/login

**Screen**: LoginActivity  
**Auth Required**: ❌ No

### Request Body
```json
{
  "email": "customer@example.com",
  "password": "Customer@123"
}
```

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Login successful",
  "data": {
    "userId": 6,
    "fullname": "Customer User",
    "email": "customer@example.com",
    "phone": "0901234567",
    "roleName": "Customer",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
    "tokenExpiration": "2025-11-03T10:30:00Z"
  }
}
```

### Related Entities
- **User** (users table): `userid`, `email`, `passwordhash`, `fullname`, `phone`, `roleid`
- **Role** (roles table): `roleid`, `rolename`
- **UserRefreshToken** (public.refresh_tokens): `id`, `token`, `user_id`, `revoked`

### Implementation
- ✅ Controller: `AuthController.cs`
- ✅ Service: `AuthService.cs` - `LoginAsync()`
- ✅ Repository: `UserRepository.cs`, `RefreshTokenRepository.cs`
- ✅ DTOs: `LoginRequestDTO.cs`, `LoginResponseDTO.cs`
- ✅ Password: BCrypt verification

---

## 🎯 2. POST /api/auth/register

**Screen**: RegisterActivity  
**Auth Required**: ❌ No

### Request Body
```json
{
  "fullname": "New Customer",
  "email": "newuser@example.com",
  "password": "Password@123",
  "confirmPassword": "Password@123",
  "phone": "0912345678"
}
```

### Response 201 Created
```json
{
  "success": true,
  "statusCode": 201,
  "message": "Registration successful",
  "data": {
    "userId": 7,
    "fullname": "New Customer",
    "email": "newuser@example.com",
    "phone": "0912345678",
    "roleName": "Customer"
  }
}
```

### Business Logic
- RoleId = 3 (Customer role)
- Password hashed with BCrypt
- Creates both User and Customer records
- Customer.userid links to User.userid

### Related Entities
- **User**: `userid`, `fullname`, `email`, `passwordhash`, `phone`, `roleid=3`, `createdat`
- **Customer**: `customerid`, `userid`, `address=null`, `dateofbirth=null`, `gender=null`

### Implementation
- ✅ Controller: `AuthController.cs`
- ✅ Service: `AuthService.cs` - `RegisterAsync()`
- ✅ Repository: `UserRepository.cs`
- ✅ DTOs: `RegisterRequestDTO.cs`
- ✅ Password: BCrypt hashing (work factor 12)

---

## 🎯 3. POST /api/auth/forgot-password

**Screen**: ForgotPasswordActivity  
**Auth Required**: ❌ No  
**Updated**: November 5, 2025 - ✅ Fixed email validation (returns error for non-existent emails)

### Request Body
```json
{
  "email": "customer@example.com"
}
```

### Response 200 OK (Email Exists)
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

### Response 400 Bad Request (Email Not Found)
```json
{
  "success": false,
  "statusCode": 400,
  "message": "Email không tồn tại trong hệ thống. Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới.",
  "data": null
}
```

### Business Logic
- ✅ **VALIDATES email exists in database** (returns 400 if not found)
- ✅ Sends OTP only for existing emails (prevents sending OTP to fake emails)
- ✅ Returns structured response with email, otpType, expiresAt
- ✅ OTP expires in 10 minutes
- Frontend should check status code and show appropriate error message

### Related Entities
- **User**: `userid`, `email`

### Implementation
- ✅ Controller: `AuthController.cs`
- ✅ Service: `AuthService.cs` - `ForgotPasswordAsync()`
- ✅ Repository: `UserRepository.cs`
- ✅ DTOs: `ForgotPasswordRequestDTO.cs`

---

## 🎯 4. POST /api/auth/refresh-token

**Screen**: SplashActivity  
**Auth Required**: ✅ Yes (Refresh Token)

### Request Body
```json
{
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}
```

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Token refreshed successfully",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "660e8400-e29b-41d4-a716-446655440001",
    "tokenExpiration": "2025-11-03T11:00:00Z"
  }
}
```

### Business Logic
- Validates refresh token exists and not revoked
- Generates new JWT token (15 min expiry)
- Generates new refresh token (7 days expiry)
- Revokes old refresh token
- Safe parsing with `int.TryParse()` for UserId

### Related Entities
- **UserRefreshToken**: `id`, `token`, `user_id`, `revoked`, `created_at`, `updated_at`
- **User**: `userid`, `email`, `roleid`

### Implementation
- ✅ Controller: `AuthController.cs`
- ✅ Service: `AuthService.cs` - `RefreshTokenAsync()`
- ✅ Repository: `RefreshTokenRepository.cs`
- ✅ DTOs: `RefreshTokenRequestDTO.cs`
- ✅ JWT: JwtService generates new token

---

## 🔧 Additional Endpoints (Implemented)

### POST /api/auth/logout
**Auth Required**: ✅ Yes

```json
// Request Body
{
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}

// Response 200 OK
{
  "success": true,
  "statusCode": 200,
  "message": "Logout successful",
  "data": null
}
```

**Logic**: Revokes refresh token in database

---

### POST /api/auth/change-password
**Auth Required**: ✅ Yes

```json
// Request Body
{
  "oldPassword": "OldPassword@123",
  "newPassword": "NewPassword@456",
  "confirmNewPassword": "NewPassword@456"
}

// Response 200 OK
{
  "success": true,
  "statusCode": 200,
  "message": "Password changed successfully",
  "data": null
}
```

**Logic**:
- Validates old password with BCrypt
- Hashes new password with BCrypt
- Updates User.passwordhash
- Updates User.updatedat timestamp

---

## 📊 Implementation Summary

### Domain Layer (Movie88.Domain/Models/)
```
✅ UserModel.cs        - UserId, Email, Fullname, Phone, Roleid, Passwordhash
✅ RoleModel.cs        - Roleid, Rolename
✅ RefreshTokenModel.cs - Id, Token, UserId, Revoked, CreatedAt
```

### Application Layer (Movie88.Application/)
```
✅ DTOs/Auth/
   - LoginRequestDTO.cs
   - LoginResponseDTO.cs
   - RegisterRequestDTO.cs
   - RefreshTokenRequestDTO.cs
   - ChangePasswordRequestDTO.cs
   - ForgotPasswordRequestDTO.cs

✅ Services/
   - AuthService.cs (IAuthService)
   - JwtService.cs (IJwtService)
   - PasswordHashingService.cs (IPasswordHashingService)

✅ Configuration/
   - JwtSettings.cs (Secret, Issuer, Audience, ExpireMinutes)
```

### Infrastructure Layer (Movie88.Infrastructure/)
```
✅ Entities/
   - UserRefreshToken.cs (custom public.refresh_tokens)

✅ Repositories/
   - UserRepository.cs (GetByEmailAsync, CreateAsync, UpdateAsync)
   - RefreshTokenRepository.cs (GetByTokenAsync, CreateAsync, RevokeAsync)
   - UnitOfWork.cs

✅ Mappers/
   - UserMapper.cs (ToModel, ToEntity)
   - RefreshTokenMapper.cs (ToModel, ToEntity)
```

### WebApi Layer (Movie88.WebApi/)
```
✅ Controllers/
   - AuthController.cs (6 endpoints)
   - HealthController.cs (health check)

✅ Extensions/
   - ServiceExtensions.cs (JWT middleware)
```

---

## 🧪 Test Results

### ✅ All 6 Endpoints Working

| Endpoint | Status | Response Time | Notes |
|----------|--------|---------------|-------|
| POST /api/auth/login | ✅ 200 | ~150ms | JWT + RefreshToken returned |
| POST /api/auth/register | ✅ 201 | ~200ms | RoleId=3 (Customer) |
| POST /api/auth/refresh-token | ✅ 200 | ~100ms | New tokens generated |
| POST /api/auth/logout | ✅ 200 | ~80ms | Token revoked |
| POST /api/auth/change-password | ✅ 200 | ~120ms | BCrypt verified |
| POST /api/auth/forgot-password | ✅ 200 | ~90ms | Mock email sent |

### Test Credentials
```
Email: customer@example.com
Password: Customer@123
Role: Customer (RoleId=3)
```

---

## 🐛 Issues Resolved

### Issue 1: Circular Dependency
**Problem**: Application ↔ Infrastructure dependency  
**Solution**: Created Domain.Models layer (UserModel, RoleModel, RefreshTokenModel)

### Issue 2: DateTime Timezone
**Problem**: Cannot write DateTime with Kind=UTC to PostgreSQL  
**Solution**: Helper methods with `DateTime.SpecifyKind(..., DateTimeKind.Unspecified)`

### Issue 3: auth.refresh_tokens Timeout
**Problem**: Cannot write to Supabase system table  
**Solution**: Created custom public.refresh_tokens with UserRefreshToken entity

### Issue 4: Region Latency
**Problem**: Sydney region (ap-southeast-2) causing timeouts  
**Solution**: Switched to Singapore (ap-southeast-1) - 30-50ms latency

### Issue 5: Transaction Pooler Errors
**Problem**: Port 6543 failing with 400 errors  
**Solution**: Switched to port 5432 (Session pooler)

### Issue 6: Wrong RoleId
**Problem**: Registration creating Staff (RoleId=2) instead of Customer  
**Solution**: Fixed to RoleId=3 (Customer)

### Issue 7: RefreshToken UserId Parsing
**Problem**: `int.Parse()` could throw exception  
**Solution**: Safe parsing with `int.TryParse()`

---

## 📝 Notes for Android Team

### JWT Token Management
```java
// Store tokens after login
SharedPrefsManager.getInstance().saveToken(loginResponse.getToken());
SharedPrefsManager.getInstance().saveRefreshToken(loginResponse.getRefreshToken());

// Add interceptor for auto token refresh
public class AuthInterceptor implements Interceptor {
    @Override
    public Response intercept(Chain chain) throws IOException {
        Request original = chain.request();
        String token = SharedPrefsManager.getInstance().getToken();
        
        Request request = original.newBuilder()
            .header("Authorization", "Bearer " + token)
            .build();
            
        Response response = chain.proceed(request);
        
        // Handle 401 - refresh token
        if (response.code() == 401) {
            synchronized (this) {
                String newToken = refreshToken();
                if (newToken != null) {
                    return chain.proceed(
                        original.newBuilder()
                            .header("Authorization", "Bearer " + newToken)
                            .build()
                    );
                }
            }
        }
        
        return response;
    }
}
```

### Error Codes
- `200` - Success
- `201` - Created (register)
- `400` - Bad request (validation errors)
- `401` - Unauthorized (invalid credentials)
- `404` - Not found (user not found)
- `500` - Server error

### Token Expiration
- **JWT Token**: 60 minutes
- **Refresh Token**: 7 days
- **OTP Code**: 10 minutes
- Auto-refresh before expiry in interceptor

---

## 🆕 OTP Email Verification Endpoints

### 7. POST /api/auth/send-otp

**Description**: Send OTP code to email for verification  
**Auth Required**: ❌ No

#### Request Body
```json
{
  "email": "customer@example.com",
  "otpType": "EmailVerification"
}
```

**OTP Types**:
- `EmailVerification` - Verify email after registration (one-time only)
- `PasswordReset` - Reset password with OTP
- `Login` - 2FA login verification (future feature)

#### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "OTP sent successfully",
  "data": {
    "email": "customer@example.com",
    "otpType": "EmailVerification",
    "expiresAt": "2025-11-04T16:40:00Z",
    "expiresInMinutes": 10,
    "message": "OTP has been sent to your email. Please check your inbox."
  }
}
```

#### Business Logic
1. Check email exists in database
2. Check user not already verified (for EmailVerification type)
3. Check rate limit: Max 3 OTPs per 10 minutes per user
4. Check no active OTP exists (not expired, not used)
5. Generate 6-digit cryptographically secure OTP
6. Save to `otp_tokens` table with 10-minute expiry
7. Send professional HTML email via Resend API
8. Track IP address and User-Agent for audit

#### Error Cases
- ❌ 400: Email not found
- ❌ 400: Email already verified (for EmailVerification)
- ❌ 400: Too many OTP requests (rate limit)
- ❌ 400: Active OTP already exists

#### Email Template
```html
Subject: 🔐 Verify Your Email - Movie88
From: Movie88 <movie88@ezyfix.site>

- Professional gradient header
- Large 6-digit OTP code in dashed box
- 10-minute expiry warning
- Security reminder
- Branded footer
```

---

### 8. POST /api/auth/verify-otp

**Description**: Verify OTP code and complete action  
**Auth Required**: ❌ No

#### Request Body
```json
{
  "email": "customer@example.com",
  "otpCode": "123456",
  "otpType": "EmailVerification"
}
```

#### Response 200 OK - Email Verification
```json
{
  "success": true,
  "statusCode": 200,
  "message": "OTP verified successfully",
  "data": {
    "email": "customer@example.com",
    "isVerified": true,
    "verifiedAt": "2025-11-04T16:35:00Z",
    "message": "Your email has been verified successfully. You can now login."
  }
}
```

#### Business Logic
1. Find OTP by code + type + email
2. Validate OTP not expired (< 10 minutes old)
3. Validate OTP not already used
4. Mark OTP as used (timestamp + IP + UserAgent)
5. **For EmailVerification**:
   - Update `users.isverified = true` (one-time only)
   - Set `users.verifiedat = NOW()`
   - Send welcome email with account benefits
6. **For PasswordReset**:
   - Return success (frontend will show reset form)
7. **For Login**:
   - Generate JWT + refresh token (future)

#### Error Cases
- ❌ 400: Invalid OTP code
- ❌ 400: OTP expired (> 10 minutes)
- ❌ 400: OTP already used
- ❌ 404: Email not found

#### Welcome Email (After Verification)
```html
Subject: 🎬 Welcome to Movie88!
From: Movie88 <movie88@ezyfix.site>

- Personalized greeting
- Account benefits (booking, reviews, promotions)
- Call to action
```

---

### 9. POST /api/auth/resend-otp

**Description**: Resend OTP with rate limiting  
**Auth Required**: ❌ No

#### Request Body
```json
{
  "email": "customer@example.com",
  "otpType": "EmailVerification"
}
```

#### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "OTP resent successfully",
  "data": {
    "email": "customer@example.com",
    "otpType": "EmailVerification",
    "expiresAt": "2025-11-04T16:45:00Z",
    "expiresInMinutes": 10,
    "message": "A new OTP has been sent to your email."
  }
}
```

#### Business Logic
1. Validate email exists
2. Check not already verified (for EmailVerification)
3. Invalidate all previous OTPs for user + type
4. Generate and send new OTP (same as send-otp)
5. Apply same rate limiting rules

#### Rate Limiting Rules
- Max 3 OTPs per user per type per 10 minutes
- If limit exceeded, user must wait full 10 minutes
- Counter resets after 10 minutes from first OTP

---

### 10. POST /api/auth/reset-password

**Description**: Reset password with OTP verification  
**Auth Required**: ❌ No

#### Request Body
```json
{
  "email": "customer@example.com",
  "otpCode": "123456",
  "newPassword": "NewPassword@123",
  "confirmPassword": "NewPassword@123"
}
```

#### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Password reset successful",
  "data": {
    "email": "customer@example.com",
    "message": "Your password has been reset successfully. You can now login with your new password.",
    "success": true
  }
}
```

#### Business Logic
1. Verify OTP code (type: PasswordReset)
2. Validate OTP not expired and not used
3. Mark OTP as used with **IP address and User-Agent tracking**
4. Hash new password with BCrypt
5. Update `users.passwordhash`
6. Send password reset confirmation email
7. User can login with new password

#### Security Features
- ✅ OTP must be valid and not expired
- ✅ OTP can only be used once
- ✅ Password strength validation (min 6 chars)
- ✅ Password confirmation check
- ✅ Audit trail: IP address + User-Agent tracked
- ✅ BCrypt hashing (work factor 12)
- ✅ Confirmation email sent after reset

#### Error Cases
- ❌ 400: Invalid or expired OTP
- ❌ 400: OTP already used
- ❌ 400: Password mismatch
- ❌ 400: Weak password
- ❌ 404: Email not found

#### Password Reset Confirmation Email
```html
Subject: 🔒 Password Reset Successful - Movie88
From: Movie88 <movie88@ezyfix.site>

- Confirmation message
- Security notice (if you didn't do this, contact support)
- Login button
- Timestamp of change
```

#### ⚠️ Important Flow Notes
**DO NOT call verify-otp before reset-password!**

❌ **WRONG** (OTP gets marked as used twice):
```
1. POST /api/auth/forgot-password → Send OTP
2. POST /api/auth/verify-otp → ❌ Marks OTP as used
3. POST /api/auth/reset-password → ❌ Fails: "OTP already used"
```

✅ **CORRECT** (reset-password verifies OTP internally):
```
1. POST /api/auth/forgot-password → Send OTP
2. POST /api/auth/reset-password → ✅ Verifies + Resets password
```

The `reset-password` endpoint internally calls `VerifyOtpAsync` which marks the OTP as used, so you don't need to call `verify-otp` separately.

---

## 🗄️ Database Schema Updates

### Updated `public."User"` Table
```sql
ALTER TABLE public."User"
ADD COLUMN isverified BOOLEAN NOT NULL DEFAULT FALSE,
ADD COLUMN isactive BOOLEAN NOT NULL DEFAULT TRUE,
ADD COLUMN verifiedat TIMESTAMP NULL;
```

### New `public.otp_tokens` Table
```sql
CREATE TABLE public.otp_tokens (
    id SERIAL PRIMARY KEY,
    userid INTEGER NOT NULL,
    otpcode VARCHAR(6) NOT NULL,
    otptype VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL,
    createdat TIMESTAMP NOT NULL DEFAULT NOW(),
    expiresat TIMESTAMP NOT NULL,
    isused BOOLEAN NOT NULL DEFAULT FALSE,
    usedat TIMESTAMP NULL,
    ipaddress VARCHAR(45) NULL,
    useragent VARCHAR(500) NULL,
    
    CONSTRAINT fk_otp_userid FOREIGN KEY (userid) 
        REFERENCES public."User"(userid) ON DELETE CASCADE,
    CONSTRAINT idx_otp_code_type UNIQUE (otpcode, otptype, email),
    CONSTRAINT chk_otp_code_length CHECK (LENGTH(otpcode) = 6),
    CONSTRAINT chk_otp_type CHECK (otptype IN ('EmailVerification', 'PasswordReset', 'Login'))
);

CREATE INDEX idx_otp_userid ON public.otp_tokens(userid);
CREATE INDEX idx_otp_email ON public.otp_tokens(email);
CREATE INDEX idx_otp_createdat ON public.otp_tokens(createdat DESC);
```

**Migration File**: `database/migrations/002_add_otp_verification.sql`

---

## 📧 Email Service (Resend API)

### Configuration
```json
{
  "Resend": {
    "ApiKey": "re_asyNFWRg_efTChvbEtP58HdCb7wfppYfP",
    "Endpoint": "https://api.resend.com"
  }
}
```

### Sender Domain
- **From**: `Movie88 <movie88@ezyfix.site>`
- **Domain**: Verified on Resend
- **DNS Records**: Configured for SPF, DKIM, DMARC

### Email Types
1. **OTP Email** - 6-digit verification code
2. **Welcome Email** - After email verification
3. **Password Reset Confirmation** - After successful reset

### Features
- ✅ Professional HTML templates with gradient headers
- ✅ Responsive design (mobile-friendly)
- ✅ Brand colors (purple gradient: #667eea → #764ba2)
- ✅ Security warnings and expiry notices
- ✅ No-reply messaging
- ✅ Error handling and logging

---

## 🔒 Security Features

### OTP Security
1. **Cryptographically Secure Random**: Using `RandomNumberGenerator.Create()`
2. **Short Expiry**: 10 minutes (600 seconds)
3. **One-Time Use**: Marked as used after verification
4. **Rate Limiting**: Max 3 per user per 10 minutes
5. **Unique Constraint**: (code, type, email) prevents duplicates

### Audit Trail
- IP address tracking
- User-Agent logging
- Creation timestamps
- Usage timestamps
- Expired OTPs kept for 7 days

### Email Verification Flow
```
1. User registers → isverified=false
2. Auto-send OTP email
3. User enters OTP code
4. Verify OTP → Update isverified=true (ONE-TIME ONLY)
5. Send welcome email
6. User can now login
```

**Important**: 
- `isverified` can only be set to `true` once
- Re-verification is not allowed
- Existing users (from migration) auto-marked as verified

---

## 📊 Implementation Summary

### New Files Created

#### Application Layer
```
Movie88.Application/
├── DTOs/Auth/
│   ├── RegisterResponseDTO.cs (NEW - no tokens)
│   ├── ResetPasswordRequestDTO.cs (NEW)
│   └── ResetPasswordResponseDTO.cs (NEW)
├── DTOs/Email/
│   └── ResendEmailDTO.cs (Request/Response)
├── Interfaces/
│   ├── IAuthService.cs (UPDATED - ResetPasswordAsync signature)
│   ├── IEmailService.cs
│   └── IOtpService.cs (already existed)
└── Services/
    ├── AuthService.cs (UPDATED - ResetPasswordAsync with audit trail)
    ├── ResendEmailService.cs (Resend API integration)
    └── OtpService.cs (OTP business logic)
```

#### Infrastructure Layer
```
Movie88.Infrastructure/
├── Entities/
│   ├── User.cs (UPDATED - 3 fields)
│   └── OtpToken.cs (NEW)
├── Context/
│   └── AppDbContext.cs (UPDATED - DbSet + config)
├── Mappers/
│   └── OtpTokenMapper.cs (NEW)
└── Repositories/
    └── OtpTokenRepository.cs (NEW)
```

#### WebApi Layer
```
Movie88.WebApi/
├── Controllers/
│   └── AuthController.cs (UPDATED - 4 endpoints: send/verify/resend-otp, reset-password)
└── appsettings.json (UPDATED - Resend config)
```

#### Domain Layer
```
Movie88.Domain/
├── Models/
│   ├── UserModel.cs (UPDATED - 3 fields)
│   └── OtpTokenModel.cs (already existed)
└── Interfaces/
    └── IOtpTokenRepository.cs (already existed)
```

### Updated Services
- ✅ `AuthService.cs` - Auto-send OTP after registration
- ✅ `ServiceExtensions.cs` - Register OTP & Email services
- ✅ `Program.cs` - Register HttpClient
- ✅ User starts with `isVerified=false`, `isActive=true`

### Packages Added
```xml
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
```

---

## 🧪 Testing Guide

### Registration + OTP Flow

#### Step 1: Register New User
```http
POST https://localhost:7238/api/auth/register
Content-Type: application/json

{
  "fullName": "Test OTP User",
  "email": "test.otp@example.com",
  "password": "Test@123",
  "confirmPassword": "Test@123",
  "phoneNumber": "0909999999"
}

Response:
- User created with isVerified=false
- OTP email sent automatically
- Can login but not verified yet
```

#### Step 2: Check Email for OTP
```
Subject: 🔐 Verify Your Email - Movie88
From: Movie88 <movie88@ezyfix.site>
To: test.otp@example.com

Your OTP Code: 123456
Expires in: 10 minutes
```

#### Step 3: Verify OTP
```http
POST https://localhost:7238/api/auth/verify-otp
Content-Type: application/json

{
  "email": "test.otp@example.com",
  "otpCode": "123456",
  "otpType": "EmailVerification"
}

Response:
- isVerified set to true
- Welcome email sent
- User fully activated
```

#### Step 4: Check Welcome Email
```
Subject: 🎬 Welcome to Movie88!
From: Movie88 <movie88@ezyfix.site>

- Greeting with user's name
- List of benefits
- Call to action
```

### Password Reset with OTP

#### Step 1: Forgot Password (Send OTP)
```http
POST https://localhost:7238/api/auth/forgot-password
Content-Type: application/json

{
  "email": "customer@example.com"
}

Response:
- OTP sent to email (type: PasswordReset)
- User receives email with 6-digit code
```

#### Step 2: Check Email for OTP
```
Subject: 🔐 Reset Your Password - Movie88
From: Movie88 <movie88@ezyfix.site>
To: customer@example.com

Your OTP Code: 654321
Expires in: 10 minutes
```

#### Step 3: Reset Password with OTP
```http
POST https://localhost:7238/api/auth/reset-password
Content-Type: application/json

{
  "email": "customer@example.com",
  "otpCode": "654321",
  "newPassword": "NewPassword@123",
  "confirmPassword": "NewPassword@123"
}

Response:
- OTP verified internally (marked as used)
- Password reset successful
- Confirmation email sent
- IP address & User-Agent tracked in database
```

#### Step 4: Check Confirmation Email
```
Subject: 🔒 Password Reset Successful - Movie88
From: Movie88 <movie88@ezyfix.site>

- Confirmation of password change
- Security notice
- Timestamp
```

#### ⚠️ Important
**DO NOT call `/api/auth/verify-otp` before `/api/auth/reset-password`!**  
The reset-password endpoint verifies the OTP internally. If you call verify-otp first, the OTP will be marked as used and reset-password will fail.

### Rate Limiting Test
```http
# Send OTP 3 times quickly
POST /api/auth/send-otp (1st) ✅ Success
POST /api/auth/send-otp (2nd) ✅ Success  
POST /api/auth/send-otp (3rd) ✅ Success
POST /api/auth/send-otp (4th) ❌ Rate limit error

Error: "Too many OTP requests. Please try again after 10 minutes."
```

### Resend OTP Test
```http
POST https://localhost:7238/api/auth/resend-otp
Content-Type: application/json

{
  "email": "customer@example.com",
  "otpType": "EmailVerification"
}

Result:
- Previous OTPs invalidated
- New OTP generated
- New email sent
- Same rate limiting applies
```

---

## 📱 Android Integration Notes

### Registration Flow with OTP
```java
// 1. Register user
registerResponse = authApi.register(request);
showSuccessMessage("Registration successful! Please check your email for OTP.");

// 2. Navigate to OTP verification screen
Intent intent = new Intent(this, VerifyEmailActivity.class);
intent.putExtra("email", request.getEmail());
intent.putExtra("otpType", "EmailVerification");
startActivity(intent);

// 3. In VerifyEmailActivity
void onVerifyClicked() {
    VerifyOtpRequest request = new VerifyOtpRequest(email, otpCode, otpType);
    verifyOtpResponse = authApi.verifyOtp(request);
    
    if (verifyOtpResponse.isSuccess()) {
        showSuccessMessage("Email verified! You can now login.");
        navigateToLogin();
    }
}

// 4. Resend OTP
void onResendClicked() {
    ResendOtpRequest request = new ResendOtpRequest(email, otpType);
    authApi.resendOtp(request);
    startCountdown(60); // 60 second cooldown
}
```

### OTP Input UI
```xml
<com.chaos.view.PinView
    android:id="@+id/pinView"
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    app:itemCount="6"
    app:itemWidth="48dp"
    app:itemHeight="48dp"
    app:cursorVisible="true"
    app:hideLineWhenFilled="false" />

<TextView
    android:id="@+id/tvTimer"
    android:text="Expires in: 09:45"
    android:textColor="@color/orange" />

<Button
    android:id="@+id/btnResend"
    android:text="Resend OTP"
    android:enabled="false" />
```

### Timer Implementation
```java
CountDownTimer otpTimer = new CountDownTimer(600000, 1000) {
    public void onTick(long millisUntilFinished) {
        long minutes = millisUntilFinished / 60000;
        long seconds = (millisUntilFinished % 60000) / 1000;
        tvTimer.setText(String.format("Expires in: %02d:%02d", minutes, seconds));
    }
    
    public void onFinish() {
        tvTimer.setText("OTP Expired");
        tvTimer.setTextColor(Color.RED);
        btnResend.setEnabled(true);
    }
}.start();
```

---

**Created**: November 3, 2025  
**Last Updated**: November 4, 2025  
**Progress**: ✅ 9/9 endpoints (100%)  
**New Features**: ✨ OTP Email Verification with Resend API
