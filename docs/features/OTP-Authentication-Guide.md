# 🔐 OTP Authentication Feature - Implementation Guide

**Created**: November 4, 2025  
**Status**: ⚠️ **READY TO IMPLEMENT**

---

## 📋 Overview

Thêm tính năng xác thực OTP (One-Time Password) qua email cho:
- ✅ Email verification khi đăng ký
- ✅ Password reset với OTP
- ✅ 2FA login với OTP (optional)

---

## 🗄️ Database Changes

### Step 1: Run Migration Script

**File**: `database/migrations/002_add_otp_verification.sql`

```bash
# Trên Supabase Dashboard:
1. Vào SQL Editor
2. Copy toàn bộ nội dung file 002_add_otp_verification.sql
3. Click "Run"
4. Verify kết quả với các query ở cuối file
```

### Tables Changed

#### 1. `public.users` - Thêm 3 cột mới

| Column | Type | Default | Description |
|--------|------|---------|-------------|
| `isverified` | BOOLEAN | FALSE | Email đã verify chưa |
| `isactive` | BOOLEAN | TRUE | Account active status |
| `verifiedat` | TIMESTAMP | NULL | Thời điểm verify email |

#### 2. `public.otp_tokens` - Table mới

| Column | Type | Description |
|--------|------|-------------|
| `id` | SERIAL | Primary key |
| `userid` | INTEGER | FK to users.userid |
| `otpcode` | VARCHAR(6) | 6-digit OTP code |
| `otptype` | VARCHAR(20) | EmailVerification, PasswordReset, Login |
| `email` | VARCHAR(100) | Email nhận OTP |
| `createdat` | TIMESTAMP | Thời gian tạo |
| `expiresat` | TIMESTAMP | Thời gian hết hạn |
| `isused` | BOOLEAN | Đã sử dụng chưa |
| `usedat` | TIMESTAMP | Thời gian sử dụng |
| `ipaddress` | VARCHAR(45) | IP address |
| `useragent` | VARCHAR(500) | User agent |

**Indexes**:
- `userid`, `email`, `otpcode`, `createdat`, `expiresat`
- Unique constraint: `(otpcode, otptype, email)`

**Constraints**:
- OTP code must be exactly 6 digits
- OTP type must be one of: EmailVerification, PasswordReset, Login
- Foreign key to users.userid with CASCADE delete

---

## 📁 Files Created

### Domain Layer

```
Movie88.Domain/
├── Models/
│   ├── UserModel.cs (UPDATED - added 3 fields)
│   └── OtpTokenModel.cs (NEW)
└── Interfaces/
    └── IOtpTokenRepository.cs (NEW)
```

### Application Layer

```
Movie88.Application/
├── DTOs/Auth/
│   └── OtpDTO.cs (NEW)
│       ├── SendOtpRequestDTO
│       ├── SendOtpResponseDTO
│       ├── VerifyOtpRequestDTO
│       ├── VerifyOtpResponseDTO
│       └── ResendOtpRequestDTO
├── Interfaces/
│   └── IOtpService.cs (NEW)
└── Services/
    └── OtpService.cs (TO CREATE)
```

### Infrastructure Layer

```
Movie88.Infrastructure/
├── Entities/
│   └── OtpTokenEntity.cs (TO CREATE)
├── Configuration/
│   └── OtpTokenConfiguration.cs (TO CREATE)
├── Repositories/
│   └── OtpTokenRepository.cs (TO CREATE)
└── Mappers/
    └── OtpTokenMapper.cs (TO CREATE)
```

### WebApi Layer

```
Movie88.WebApi/
└── Controllers/
    └── AuthController.cs (UPDATE - add 3 new endpoints)
```

---

## 🎯 New API Endpoints

### 1. POST /api/auth/send-otp

**Description**: Gửi OTP qua email  
**Auth Required**: ❌ No

#### Request Body
```json
{
  "email": "customer@example.com",
  "otpType": "EmailVerification" // or "PasswordReset", "Login"
}
```

#### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "OTP sent successfully",
  "data": {
    "email": "customer@example.com",
    "otpType": "EmailVerification",
    "expiresAt": "2025-11-04T15:40:00Z",
    "expiresInMinutes": 10,
    "message": "OTP has been sent to your email. Please check your inbox."
  }
}
```

#### Business Logic
1. Check email exists in users table
2. Check rate limit (max 3 OTPs per 10 minutes)
3. Generate 6-digit random OTP
4. Save to otp_tokens with expiry (10 minutes)
5. Send email (mock implementation first)
6. Return success response

#### Validation
- ✅ Email required and valid format
- ✅ OTP type must be valid (EmailVerification, PasswordReset, Login)
- ✅ Rate limiting: Max 3 OTPs per 10 minutes
- ❌ Cannot send if active OTP exists (not expired, not used)

---

### 2. POST /api/auth/verify-otp

**Description**: Verify OTP code  
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
  "message": "Email verified successfully",
  "data": {
    "email": "customer@example.com",
    "isVerified": true,
    "verifiedAt": "2025-11-04T15:35:00Z",
    "message": "Your email has been verified successfully. You can now login."
  }
}
```

#### Response 200 OK - Login OTP (with tokens)
```json
{
  "success": true,
  "statusCode": 200,
  "message": "OTP verified successfully",
  "data": {
    "email": "customer@example.com",
    "isVerified": true,
    "verifiedAt": "2025-11-04T15:35:00Z",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
    "message": "Login successful"
  }
}
```

#### Business Logic
1. Validate OTP exists and matches (code + type + email)
2. Check OTP not expired (< 10 minutes old)
3. Check OTP not used
4. Mark OTP as used
5. If EmailVerification: Update users.isverified = true
6. If Login: Generate JWT + refresh token
7. Return success response

#### Error Cases
- ❌ 400: Invalid OTP code
- ❌ 400: OTP expired
- ❌ 400: OTP already used
- ❌ 404: Email not found

---

### 3. POST /api/auth/resend-otp

**Description**: Resend OTP (with rate limiting)  
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
    "expiresAt": "2025-11-04T15:45:00Z",
    "expiresInMinutes": 10,
    "message": "A new OTP has been sent to your email."
  }
}
```

#### Business Logic
1. Invalidate all previous OTPs for this user + type
2. Generate new OTP
3. Check rate limit (max 3 resends per 10 minutes)
4. Send new OTP email
5. Return success response

#### Rate Limiting
- Max 3 OTPs per user per 10 minutes
- Max 5 OTPs per IP per hour (optional)
- Cooldown: 1 minute between requests

---

## 🔨 Implementation Steps

### Step 1: Database Migration ✅ DONE

```bash
# Run on Supabase
/database/migrations/002_add_otp_verification.sql
```

### Step 2: Domain & DTOs ✅ DONE

- ✅ UserModel.cs - Added 3 fields
- ✅ OtpTokenModel.cs - Created
- ✅ IOtpTokenRepository.cs - Created
- ✅ OtpDTO.cs - Created
- ✅ IOtpService.cs - Created

### Step 3: Infrastructure Layer (TO DO)

#### 3.1. Create OTP Entity

**File**: `Movie88.Infrastructure/Entities/OtpTokenEntity.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie88.Infrastructure.Entities
{
    [Table("otp_tokens", Schema = "public")]
    public class OtpTokenEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userid")]
        public int UserId { get; set; }

        [Required]
        [Column("otpcode")]
        [MaxLength(6)]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        [Column("otptype")]
        [MaxLength(20)]
        public string OtpType { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("createdat")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("expiresat")]
        public DateTime ExpiresAt { get; set; }

        [Required]
        [Column("isused")]
        public bool IsUsed { get; set; }

        [Column("usedat")]
        public DateTime? UsedAt { get; set; }

        [Column("ipaddress")]
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [Column("useragent")]
        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }
}
```

#### 3.2. Update AppDbContext

**File**: `Movie88.Infrastructure/Context/AppDbContext.cs`

```csharp
// Add DbSet
public DbSet<OtpTokenEntity> OtpTokens { get; set; }

// In OnModelCreating
modelBuilder.Entity<OtpTokenEntity>(entity =>
{
    entity.ToTable("otp_tokens", "public");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.OtpCode).IsRequired().HasMaxLength(6);
    entity.Property(e => e.OtpType).IsRequired().HasMaxLength(20);
    entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
    entity.HasIndex(e => e.UserId);
    entity.HasIndex(e => e.Email);
    entity.HasIndex(e => e.OtpCode);
});
```

#### 3.3. Update User Entity

Add 3 fields to existing UserEntity:

```csharp
[Column("isverified")]
public bool IsVerified { get; set; } = false;

[Column("isactive")]
public bool IsActive { get; set; } = true;

[Column("verifiedat")]
public DateTime? VerifiedAt { get; set; }
```

#### 3.4. Create OTP Repository

**File**: `Movie88.Infrastructure/Repositories/OtpTokenRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Mappers;

namespace Movie88.Infrastructure.Repositories
{
    public class OtpTokenRepository : IOtpTokenRepository
    {
        private readonly AppDbContext _context;

        public OtpTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OtpTokenModel> CreateAsync(OtpTokenModel otpToken)
        {
            var entity = OtpTokenMapper.ToEntity(otpToken);
            _context.OtpTokens.Add(entity);
            await _context.SaveChangesAsync();
            return OtpTokenMapper.ToModel(entity);
        }

        public async Task<OtpTokenModel?> GetByCodeAsync(string otpCode, string otpType, string email)
        {
            var entity = await _context.OtpTokens
                .Where(o => o.OtpCode == otpCode 
                         && o.OtpType == otpType 
                         && o.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            return entity != null ? OtpTokenMapper.ToModel(entity) : null;
        }

        public async Task<bool> MarkAsUsedAsync(int otpId, string? ipAddress = null, string? userAgent = null)
        {
            var entity = await _context.OtpTokens.FindAsync(otpId);
            if (entity == null) return false;

            entity.IsUsed = true;
            entity.UsedAt = DateTime.UtcNow;
            entity.IpAddress = ipAddress;
            entity.UserAgent = userAgent;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OtpTokenModel?> GetActiveOtpAsync(int userId, string otpType)
        {
            var now = DateTime.UtcNow;
            var entity = await _context.OtpTokens
                .Where(o => o.UserId == userId 
                         && o.OtpType == otpType
                         && !o.IsUsed
                         && o.ExpiresAt > now)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            return entity != null ? OtpTokenMapper.ToModel(entity) : null;
        }

        public async Task<bool> InvalidateUserOtpsAsync(int userId, string otpType)
        {
            var tokens = await _context.OtpTokens
                .Where(o => o.UserId == userId 
                         && o.OtpType == otpType
                         && !o.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> DeleteExpiredOtpsAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-7);
            var expiredTokens = await _context.OtpTokens
                .Where(o => o.ExpiresAt < cutoffDate)
                .ToListAsync();

            _context.OtpTokens.RemoveRange(expiredTokens);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> GetOtpCountAsync(int userId, string otpType, TimeSpan timespan)
        {
            var since = DateTime.UtcNow.Subtract(timespan);
            return await _context.OtpTokens
                .Where(o => o.UserId == userId 
                         && o.OtpType == otpType
                         && o.CreatedAt > since)
                .CountAsync();
        }
    }
}
```

#### 3.5. Create Mapper

**File**: `Movie88.Infrastructure/Mappers/OtpTokenMapper.cs`

```csharp
using Movie88.Domain.Models;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Mappers
{
    public static class OtpTokenMapper
    {
        public static OtpTokenModel ToModel(OtpTokenEntity entity)
        {
            return new OtpTokenModel
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OtpCode = entity.OtpCode,
                OtpType = entity.OtpType,
                Email = entity.Email,
                CreatedAt = DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc),
                ExpiresAt = DateTime.SpecifyKind(entity.ExpiresAt, DateTimeKind.Utc),
                IsUsed = entity.IsUsed,
                UsedAt = entity.UsedAt.HasValue 
                    ? DateTime.SpecifyKind(entity.UsedAt.Value, DateTimeKind.Utc) 
                    : null,
                IpAddress = entity.IpAddress,
                UserAgent = entity.UserAgent
            };
        }

        public static OtpTokenEntity ToEntity(OtpTokenModel model)
        {
            return new OtpTokenEntity
            {
                Id = model.Id,
                UserId = model.UserId,
                OtpCode = model.OtpCode,
                OtpType = model.OtpType,
                Email = model.Email,
                CreatedAt = DateTime.SpecifyKind(model.CreatedAt, DateTimeKind.Unspecified),
                ExpiresAt = DateTime.SpecifyKind(model.ExpiresAt, DateTimeKind.Unspecified),
                IsUsed = model.IsUsed,
                UsedAt = model.UsedAt.HasValue 
                    ? DateTime.SpecifyKind(model.UsedAt.Value, DateTimeKind.Unspecified) 
                    : null,
                IpAddress = model.IpAddress,
                UserAgent = model.UserAgent
            };
        }
    }
}
```

### Step 4: Application Layer - OTP Service (TO DO)

**File**: `Movie88.Application/Services/OtpService.cs`

```csharp
using Movie88.Application.DTOs.Auth;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using System.Security.Cryptography;

namespace Movie88.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IOtpTokenRepository _otpRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OtpService> _logger;

        public OtpService(
            IOtpTokenRepository otpRepository,
            IUserRepository userRepository,
            ILogger<OtpService> logger)
        {
            _otpRepository = otpRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<SendOtpResponseDTO> SendOtpAsync(
            SendOtpRequestDTO request, 
            string? ipAddress = null, 
            string? userAgent = null)
        {
            // 1. Validate email exists
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Email not found");
            }

            // 2. Check rate limit (max 3 OTPs per 10 minutes)
            var otpCount = await _otpRepository.GetOtpCountAsync(
                user.UserId, 
                request.OtpType, 
                TimeSpan.FromMinutes(10)
            );

            if (otpCount >= 3)
            {
                throw new Exception("Too many OTP requests. Please try again after 10 minutes.");
            }

            // 3. Check if active OTP exists
            var activeOtp = await _otpRepository.GetActiveOtpAsync(user.UserId, request.OtpType);
            if (activeOtp != null)
            {
                throw new Exception($"An OTP is already active. Please use the existing OTP or wait {(activeOtp.ExpiresAt - DateTime.UtcNow).TotalMinutes:F0} minutes.");
            }

            // 4. Generate OTP code
            var otpCode = GenerateOtpCode();

            // 5. Create OTP token
            var otpToken = new OtpTokenModel
            {
                UserId = user.UserId,
                OtpCode = otpCode,
                OtpType = request.OtpType,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _otpRepository.CreateAsync(otpToken);

            // 6. Send email
            await SendOtpEmailAsync(request.Email, otpCode, request.OtpType);

            // 7. Return response
            return new SendOtpResponseDTO
            {
                Email = request.Email,
                OtpType = request.OtpType,
                ExpiresAt = otpToken.ExpiresAt,
                ExpiresInMinutes = 10,
                Message = "OTP has been sent to your email. Please check your inbox."
            };
        }

        public async Task<VerifyOtpResponseDTO> VerifyOtpAsync(
            VerifyOtpRequestDTO request, 
            string? ipAddress = null, 
            string? userAgent = null)
        {
            // 1. Get OTP token
            var otpToken = await _otpRepository.GetByCodeAsync(
                request.OtpCode, 
                request.OtpType, 
                request.Email
            );

            if (otpToken == null)
            {
                throw new Exception("Invalid OTP code");
            }

            // 2. Check expired
            if (otpToken.IsExpired())
            {
                throw new Exception("OTP code has expired");
            }

            // 3. Check used
            if (otpToken.IsUsed)
            {
                throw new Exception("OTP code has already been used");
            }

            // 4. Mark as used
            await _otpRepository.MarkAsUsedAsync(otpToken.Id, ipAddress, userAgent);

            // 5. Get user
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // 6. Handle based on OTP type
            var response = new VerifyOtpResponseDTO
            {
                Email = request.Email,
                IsVerified = true,
                VerifiedAt = DateTime.UtcNow
            };

            if (request.OtpType == OtpTypeConstants.EmailVerification)
            {
                // Update user verification status
                user.IsVerified = true;
                user.VerifiedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                
                response.Message = "Your email has been verified successfully. You can now login.";
            }
            else if (request.OtpType == OtpTypeConstants.Login)
            {
                // Generate JWT tokens (implement this in Step 5)
                response.Message = "Login successful";
                // response.Token = ... (to be implemented)
                // response.RefreshToken = ... (to be implemented)
            }
            else if (request.OtpType == OtpTypeConstants.PasswordReset)
            {
                response.Message = "OTP verified. You can now reset your password.";
            }

            return response;
        }

        public async Task<SendOtpResponseDTO> ResendOtpAsync(
            ResendOtpRequestDTO request, 
            string? ipAddress = null, 
            string? userAgent = null)
        {
            // 1. Get user
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Email not found");
            }

            // 2. Invalidate old OTPs
            await _otpRepository.InvalidateUserOtpsAsync(user.UserId, request.OtpType);

            // 3. Send new OTP
            return await SendOtpAsync(new SendOtpRequestDTO
            {
                Email = request.Email,
                OtpType = request.OtpType
            }, ipAddress, userAgent);
        }

        public string GenerateOtpCode()
        {
            // Generate cryptographically secure 6-digit code
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            var otpCode = (randomNumber % 1000000).ToString("D6");
            return otpCode;
        }

        public async Task<bool> SendOtpEmailAsync(string email, string otpCode, string otpType)
        {
            // TODO: Implement real email service
            // For now, just log the OTP
            _logger.LogInformation($"[MOCK EMAIL] Sending OTP to {email}: {otpCode} (Type: {otpType})");
            
            // Mock email content
            var subject = otpType switch
            {
                OtpTypeConstants.EmailVerification => "Verify Your Email - Movie88",
                OtpTypeConstants.PasswordReset => "Reset Your Password - Movie88",
                OtpTypeConstants.Login => "Login Verification - Movie88",
                _ => "OTP Code - Movie88"
            };

            var body = $@"
                <h2>{subject}</h2>
                <p>Your OTP code is: <strong>{otpCode}</strong></p>
                <p>This code will expire in 10 minutes.</p>
                <p>If you didn't request this, please ignore this email.</p>
            ";

            _logger.LogInformation($"Email Subject: {subject}");
            _logger.LogInformation($"Email Body: {body}");

            await Task.CompletedTask;
            return true;
        }
    }
}
```

### Step 5: Update AuthController (TO DO)

**File**: `Movie88.WebApi/Controllers/AuthController.cs`

Add 3 new endpoints:

```csharp
/// <summary>
/// Send OTP to email
/// </summary>
[HttpPost("send-otp")]
[AllowAnonymous]
public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDTO request)
{
    try
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        var result = await _otpService.SendOtpAsync(request, ipAddress, userAgent);
        
        return Ok(new Response<SendOtpResponseDTO>
        {
            Success = true,
            StatusCode = 200,
            Message = "OTP sent successfully",
            Data = result
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new Response<object>
        {
            Success = false,
            StatusCode = 400,
            Message = ex.Message
        });
    }
}

/// <summary>
/// Verify OTP code
/// </summary>
[HttpPost("verify-otp")]
[AllowAnonymous]
public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDTO request)
{
    try
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        var result = await _otpService.VerifyOtpAsync(request, ipAddress, userAgent);
        
        return Ok(new Response<VerifyOtpResponseDTO>
        {
            Success = true,
            StatusCode = 200,
            Message = "OTP verified successfully",
            Data = result
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new Response<object>
        {
            Success = false,
            StatusCode = 400,
            Message = ex.Message
        });
    }
}

/// <summary>
/// Resend OTP
/// </summary>
[HttpPost("resend-otp")]
[AllowAnonymous]
public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDTO request)
{
    try
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        
        var result = await _otpService.ResendOtpAsync(request, ipAddress, userAgent);
        
        return Ok(new Response<SendOtpResponseDTO>
        {
            Success = true,
            StatusCode = 200,
            Message = "OTP resent successfully",
            Data = result
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new Response<object>
        {
            Success = false,
            StatusCode = 400,
            Message = ex.Message
        });
    }
}
```

### Step 6: Register Services (TO DO)

**File**: `Movie88.Application/Configuration/ServiceExtensions.cs`

```csharp
// Add OTP Service
services.AddScoped<IOtpService, OtpService>();
```

**File**: `Movie88.Infrastructure/Configuration/ServiceExtensions.cs`

```csharp
// Add OTP Repository
services.AddScoped<IOtpTokenRepository, OtpTokenRepository>();
```

### Step 7: Update Register Flow (TO DO)

**File**: `Movie88.Application/Services/AuthService.cs`

Update `RegisterAsync()`:

```csharp
// After creating user, send verification OTP
var otpRequest = new SendOtpRequestDTO
{
    Email = request.Email,
    OtpType = OtpTypeConstants.EmailVerification
};
await _otpService.SendOtpAsync(otpRequest);

// User starts with isVerified = false
user.IsVerified = false;
```

---

## 🧪 Testing Guide

### Test File: `tests/Auth-OTP.http`

```http
### OTP Testing
@baseUrl = https://localhost:7238/api
@email = customer@example.com

###############################################
# 1. SEND OTP - Email Verification
###############################################

### Test 1.1: Send OTP for email verification
POST {{baseUrl}}/auth/send-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpType": "EmailVerification"
}

### Test 1.2: Send OTP for password reset
POST {{baseUrl}}/auth/send-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpType": "PasswordReset"
}

### Test 1.3: Rate limit test (send 4 times, 4th should fail)
POST {{baseUrl}}/auth/send-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpType": "EmailVerification"
}

###############################################
# 2. VERIFY OTP
###############################################

### Test 2.1: Verify valid OTP
POST {{baseUrl}}/auth/verify-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpCode": "123456",
  "otpType": "EmailVerification"
}

### Test 2.2: Verify invalid OTP (should fail)
POST {{baseUrl}}/auth/verify-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpCode": "999999",
  "otpType": "EmailVerification"
}

### Test 2.3: Verify expired OTP (wait 11 minutes, should fail)
POST {{baseUrl}}/auth/verify-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpCode": "123456",
  "otpType": "EmailVerification"
}

### Test 2.4: Verify already used OTP (run twice, 2nd should fail)
POST {{baseUrl}}/auth/verify-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpCode": "123456",
  "otpType": "EmailVerification"
}

###############################################
# 3. RESEND OTP
###############################################

### Test 3.1: Resend OTP
POST {{baseUrl}}/auth/resend-otp
Content-Type: application/json

{
  "email": "{{email}}",
  "otpType": "EmailVerification"
}

###############################################
# 4. REGISTRATION WITH OTP FLOW
###############################################

### Test 4.1: Register new user
POST {{baseUrl}}/auth/register
Content-Type: application/json

{
  "fullname": "Test User OTP",
  "email": "testotp@example.com",
  "password": "Test@123",
  "confirmPassword": "Test@123",
  "phone": "0909999999"
}

### Test 4.2: Send OTP to newly registered user
POST {{baseUrl}}/auth/send-otp
Content-Type: application/json

{
  "email": "testotp@example.com",
  "otpType": "EmailVerification"
}

### Test 4.3: Verify OTP (check logs for OTP code)
POST {{baseUrl}}/auth/verify-otp
Content-Type: application/json

{
  "email": "testotp@example.com",
  "otpCode": "PASTE_OTP_FROM_LOGS",
  "otpType": "EmailVerification"
}

### Test 4.4: Try to login before verification (should fail)
POST {{baseUrl}}/auth/login
Content-Type: application/json

{
  "email": "testotp@example.com",
  "password": "Test@123"
}

### Test 4.5: Login after verification (should succeed)
POST {{baseUrl}}/auth/login
Content-Type: application/json

{
  "email": "testotp@example.com",
  "password": "Test@123"
}
```

---

## 📝 Checklist

### Database ✅
- [x] Migration script created
- [ ] Run migration on Supabase
- [ ] Verify tables created
- [ ] Test data inserted

### Domain Layer ✅
- [x] UserModel updated
- [x] OtpTokenModel created
- [x] IOtpTokenRepository created

### Application Layer ✅
- [x] OTP DTOs created
- [x] IOtpService created
- [ ] OtpService implemented

### Infrastructure Layer ⏳
- [ ] OtpTokenEntity created
- [ ] OtpTokenConfiguration created
- [ ] OtpTokenRepository implemented
- [ ] OtpTokenMapper created
- [ ] Update AppDbContext
- [ ] Update UserEntity

### WebApi Layer ⏳
- [ ] AuthController updated (3 endpoints)
- [ ] Services registered
- [ ] Update Register flow

### Testing ⏳
- [ ] Create Auth-OTP.http
- [ ] Test all scenarios
- [ ] Update documentation

---

## 🚀 Deployment Steps

1. **Run Database Migration**
   ```bash
   # On Supabase SQL Editor
   # Run: database/migrations/002_add_otp_verification.sql
   ```

2. **Build & Test Locally**
   ```bash
   dotnet build
   dotnet run --project Movie88.WebApi
   ```

3. **Test OTP Flow**
   ```bash
   # Use tests/Auth-OTP.http
   # Check console logs for OTP codes
   ```

4. **Deploy to Railway**
   ```bash
   git add .
   git commit -m "feat: Add OTP authentication feature"
   git push origin main
   ```

5. **Verify on Production**
   - Test /api/auth/send-otp
   - Check Supabase logs
   - Verify email verification works

---

## 🔒 Security Considerations

1. **Rate Limiting**
   - Max 3 OTPs per 10 minutes per user
   - Max 5 OTPs per hour per IP (optional)
   - 1 minute cooldown between requests

2. **OTP Expiry**
   - 10 minutes default expiry
   - Auto-cleanup after 7 days

3. **OTP Code**
   - 6-digit numeric code
   - Cryptographically secure random generation
   - Unique constraint (code + type + email)

4. **Validation**
   - Check OTP not expired
   - Check OTP not used
   - Check email exists
   - One active OTP per user per type

5. **Audit Trail**
   - Log IP address
   - Log user agent
   - Track usage timestamp
   - Keep expired OTPs for 7 days

---

**Next Steps**: Implement Infrastructure & Application layers, then test thoroughly!
