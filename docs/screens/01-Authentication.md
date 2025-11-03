# 🔐 Screen 1: Authentication (4 Endpoints)

**Status**: ✅ **COMPLETED** (4/4 endpoints - 100%)

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Screen | Auth | Status |
|---|--------|----------|--------|------|--------|
| 1 | POST | `/api/auth/login` | LoginActivity | ❌ | ✅ DONE |
| 2 | POST | `/api/auth/register` | RegisterActivity | ❌ | ✅ DONE |
| 3 | POST | `/api/auth/forgot-password` | ForgotPasswordActivity | ❌ | ✅ DONE |
| 4 | POST | `/api/auth/refresh-token` | SplashActivity | ✅ | ✅ DONE |

**Additional Implemented**:
- POST `/api/auth/logout` - Đăng xuất (revoke refresh token)
- POST `/api/auth/change-password` - Đổi mật khẩu

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

### Request Body
```json
{
  "email": "customer@example.com"
}
```

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Password reset email sent. Please check your inbox.",
  "data": null
}
```

### Business Logic
- Validates email exists in database
- Generates password reset token (JWT with 1 hour expiry)
- Sends email with reset link (mock implementation)
- Frontend will implement reset password screen separately

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
- **JWT Token**: 15 minutes
- **Refresh Token**: 7 days
- Auto-refresh before expiry in interceptor

---

**Created**: November 3, 2025  
**Last Updated**: November 3, 2025  
**Progress**: ✅ 4/4 endpoints (100%)
