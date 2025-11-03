# 🔐 Authentication & Authorization Flow

## 📖 Giới thiệu

Tài liệu này mô tả chi tiết cơ chế xác thực (Authentication) và phân quyền (Authorization) trong hệ thống Movie88.

---

## 🔑 1. Authentication Strategy

### 1.1 JWT (JSON Web Token)

Movie88 sử dụng **JWT Token** làm cơ chế xác thực chính:

```
Access Token: Short-lived (1 hour)
└─ Dùng để xác thực mọi API request
└─ Lưu trong memory hoặc sessionStorage

Refresh Token: Long-lived (7 days)
└─ Dùng để làm mới Access Token khi hết hạn
└─ Lưu trong httpOnly cookie (secure)
```

### 1.2 JWT Structure

#### Access Token Payload:
```json
{
  "sub": "45",                          // User ID
  "email": "nguyenvana@example.com",
  "role": "Customer",                   // Role name
  "roleId": 4,
  "fullName": "Nguyễn Văn A",
  "iat": 1698566400,                    // Issued at
  "exp": 1698570000,                    // Expiry (1h sau)
  "iss": "Movie88API",                  // Issuer
  "aud": "Movie88Client"                // Audience
}
```

#### Refresh Token Payload:
```json
{
  "sub": "45",
  "tokenId": "uuid-refresh-token-id",
  "iat": 1698566400,
  "exp": 1699171200                     // Expiry (7 days sau)
}
```

---

## 🔐 2. Authentication Flow

### 2.1 Registration Flow

```
┌──────┐                ┌─────────┐              ┌──────────┐
│Client│                │  API    │              │ Database │
└──┬───┘                └────┬────┘              └────┬─────┘
   │                         │                        │
   │ POST /api/auth/register │                        │
   │ ───────────────────────>│                        │
   │                         │                        │
   │                         │ Validate input         │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │                         │ Check email exists     │
   │                         │ ──────────────────────>│
   │                         │                        │
   │                         │ Email not found        │
   │                         │<───────────────────────│
   │                         │                        │
   │                         │ Hash password (BCrypt) │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │                         │ Insert User record     │
   │                         │ ──────────────────────>│
   │                         │                        │
   │                         │ Insert Customer record │
   │                         │ ──────────────────────>│
   │                         │                        │
   │                         │ Generate JWT tokens    │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │  Return UserDTO + Tokens│                        │
   │ <───────────────────────│                        │
   │                         │                        │
   │ Store tokens in client  │                        │
   │ ─────────┐              │                        │
   │          │              │                        │
   │<─────────┘              │                        │
```

**Key Steps:**
1. Client gửi thông tin đăng ký
2. Backend validate input (email format, password strength)
3. Check email đã tồn tại chưa
4. Hash password bằng BCrypt (cost factor = 12)
5. Tạo User record (với RoleId = 4 - Customer)
6. Tạo Customer record (link với UserId)
7. Generate Access Token và Refresh Token
8. Return tokens cho client
9. Client lưu tokens và redirect

---

### 2.2 Login Flow

```
┌──────┐                ┌─────────┐              ┌──────────┐
│Client│                │  API    │              │ Database │
└──┬───┘                └────┬────┘              └────┬─────┘
   │                         │                        │
   │ POST /api/auth/login    │                        │
   │ ───────────────────────>│                        │
   │ { email, password }     │                        │
   │                         │                        │
   │                         │ Find User by email     │
   │                         │ ──────────────────────>│
   │                         │                        │
   │                         │ Return User record     │
   │                         │<───────────────────────│
   │                         │                        │
   │                         │ Verify password hash   │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │                         │ Password match ✓       │
   │                         │                        │
   │                         │ Generate JWT tokens    │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │                         │ Update LastLoginAt     │
   │                         │ ──────────────────────>│
   │                         │                        │
   │  Return UserDTO + Tokens│                        │
   │ <───────────────────────│                        │
   │                         │                        │
   │ Store tokens            │                        │
   │ Redirect by role        │                        │
```

**Password Verification:**
```csharp
bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
    inputPassword,      // Password từ client
    user.PasswordHash   // Hash từ database
);
```

**Role-based Redirect:**
```javascript
if (user.roleName === 'Customer') {
  redirect('/');  // Homepage
} else if (['Admin', 'Manager', 'Staff'].includes(user.roleName)) {
  redirect('/admin/dashboard');
}
```

---

### 2.3 Token Refresh Flow

```
┌──────┐                ┌─────────┐              ┌──────────┐
│Client│                │  API    │              │ Database │
└──┬───┘                └────┬────┘              └────┬─────┘
   │                         │                        │
   │ API call with           │                        │
   │ expired access token    │                        │
   │ ───────────────────────>│                        │
   │                         │                        │
   │                         │ Validate token         │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │ Token expired ✗        │
   │                         │                        │
   │ 401 Unauthorized        │                        │
   │<────────────────────────│                        │
   │                         │                        │
   │ POST /api/auth/         │                        │
   │      refresh-token      │                        │
   │ ───────────────────────>│                        │
   │ { refreshToken }        │                        │
   │                         │                        │
   │                         │ Validate refresh token │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │                         │ Token valid ✓          │
   │                         │                        │
   │                         │ Generate new access    │
   │                         │ token                  │
   │                         │ ─────────┐             │
   │                         │          │             │
   │                         │<─────────┘             │
   │                         │                        │
   │  Return new access token│                        │
   │ <───────────────────────│                        │
   │                         │                        │
   │ Retry original API call │                        │
   │ with new token          │                        │
   │ ───────────────────────>│                        │
```

**Client-side Interceptor (Axios example):**
```javascript
axios.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        const { data } = await axios.post('/api/auth/refresh-token', {
          refreshToken: getRefreshToken()
        });
        
        setAccessToken(data.accessToken);
        originalRequest.headers['Authorization'] = `Bearer ${data.accessToken}`;
        
        return axios(originalRequest);
      } catch (refreshError) {
        // Refresh token cũng hết hạn → Logout
        logout();
        redirect('/login');
      }
    }
    
    return Promise.reject(error);
  }
);
```

---

## 🛡️ 3. Authorization (Phân quyền)

### 3.1 Role Hierarchy

```
┌─────────────────────────────────────┐
│            Admin (RoleId: 1)        │
│  - Full quyền, quản lý toàn bộ      │
└─────────────┬───────────────────────┘
              │
    ┌─────────┴─────────┐
    │                   │
┌───▼──────────┐  ┌────▼─────────────┐
│ Manager (2)  │  │   Staff (3)      │
│ - Quản lý    │  │ - Hỗ trợ khách   │
│   rạp, phim  │  │ - Quản lý booking│
└──────────────┘  └──────────────────┘
         │
    ┌────▼──────────┐
    │ Customer (4)  │
    │ - Đặt vé      │
    │ - Xem lịch sử │
    └───────────────┘
```

### 3.2 Permission Matrix

| Resource | Admin | Manager | Staff | Customer |
|----------|-------|---------|-------|----------|
| **Users** |
| View all users | ✅ | ❌ | ❌ | ❌ |
| Create user | ✅ | ❌ | ❌ | ❌ |
| Update any user | ✅ | ❌ | ❌ | ❌ |
| Update self | ✅ | ✅ | ✅ | ✅ |
| Delete user | ✅ | ❌ | ❌ | ❌ |
| **Movies** |
| View movies | ✅ | ✅ | ✅ | ✅ |
| Create movie | ✅ | ✅ | ❌ | ❌ |
| Update movie | ✅ | ✅ | ❌ | ❌ |
| Delete movie | ✅ | ❌ | ❌ | ❌ |
| **Cinemas** |
| View cinemas | ✅ | ✅ | ✅ | ✅ |
| Create cinema | ✅ | ❌ | ❌ | ❌ |
| Update cinema | ✅ | ✅ | ❌ | ❌ |
| **Bookings** |
| View all bookings | ✅ | ✅ | ✅ | ❌ |
| View own bookings | ✅ | ✅ | ✅ | ✅ |
| Create booking | ✅ | ✅ | ✅ | ✅ |
| Cancel booking | ✅ | ✅ | ✅ | ✅ (own) |
| **Payments** |
| View all payments | ✅ | ✅ | ❌ | ❌ |
| Process refund | ✅ | ✅ | ❌ | ❌ |
| **Reports** |
| Revenue reports | ✅ | ✅ | ❌ | ❌ |
| Analytics | ✅ | ✅ | ❌ | ❌ |

---

### 3.3 Authorization Implementation

#### Backend - ASP.NET Core Authorization

**Role-based Authorization:**
```csharp
[Authorize(Roles = "Admin,Manager")]
[HttpPost("api/movies")]
public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDTO dto)
{
    // Only Admin and Manager can access
}
```

**Policy-based Authorization:**
```csharp
// Startup.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("RequireManagerOrAbove", policy => 
        policy.RequireRole("Admin", "Manager"));
    
    options.AddPolicy("CanManageBookings", policy => 
        policy.RequireRole("Admin", "Manager", "Staff"));
});

// Controller
[Authorize(Policy = "RequireManagerOrAbove")]
[HttpPut("api/movies/{id}")]
public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieDTO dto)
{
    // Code
}
```

**Resource-based Authorization (Own Resource):**
```csharp
[Authorize]
[HttpPut("api/bookings/{id}/cancel")]
public async Task<IActionResult> CancelBooking(int id)
{
    var booking = await _bookingService.GetByIdAsync(id);
    
    if (booking == null)
        return NotFound();
    
    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    var currentUserRole = User.FindFirst(ClaimTypes.Role).Value;
    
    // Admin/Manager có thể cancel bất kỳ booking nào
    // Customer chỉ cancel được booking của mình
    if (currentUserRole == "Customer" && booking.CustomerId != currentUserId)
    {
        return Forbid(); // 403 Forbidden
    }
    
    // Proceed with cancellation
}
```

---

### 3.4 Frontend Route Guards

**React Router Example:**
```javascript
// ProtectedRoute.jsx
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

export const ProtectedRoute = ({ children, allowedRoles }) => {
  const { user, isAuthenticated } = useAuth();
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  if (allowedRoles && !allowedRoles.includes(user.roleName)) {
    return <Navigate to="/unauthorized" replace />;
  }
  
  return children;
};

// App routes
<Routes>
  <Route path="/login" element={<LoginPage />} />
  
  <Route path="/" element={
    <ProtectedRoute>
      <Homepage />
    </ProtectedRoute>
  } />
  
  <Route path="/admin/*" element={
    <ProtectedRoute allowedRoles={['Admin', 'Manager', 'Staff']}>
      <AdminLayout />
    </ProtectedRoute>
  } />
  
  <Route path="/profile" element={
    <ProtectedRoute>
      <ProfilePage />
    </ProtectedRoute>
  } />
</Routes>
```

---

## 🔒 4. Security Best Practices

### 4.1 Password Security

```csharp
// Password hashing với BCrypt
public string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
}

// Password verification
public bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```

**Password Requirements:**
- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 digit
- At least 1 special character (@$!%*?&)

---

### 4.2 Token Security

**JWT Configuration:**
```json
{
  "Jwt": {
    "SecretKey": "your-very-long-secret-key-at-least-32-characters",
    "Issuer": "Movie88API",
    "Audience": "Movie88Client",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

**Token Generation:**
```csharp
public string GenerateAccessToken(User user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.RoleName),
        new Claim("RoleId", user.RoleId.ToString()),
        new Claim("FullName", user.FullName)
    };
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(60),
        signingCredentials: creds
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

---

### 4.3 Rate Limiting

```csharp
// Rate limiting configuration
services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
    });
    
    options.AddFixedWindowLimiter("api", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
    });
});

// Apply to endpoints
[EnableRateLimiting("auth")]
[HttpPost("api/auth/login")]
public async Task<IActionResult> Login([FromBody] LoginDTO dto) { }
```

---

### 4.4 CORS Configuration

```csharp
services.AddCors(options =>
{
    options.AddPolicy("Movie88Policy", builder =>
    {
        builder.WithOrigins(
            "https://movie88.com",
            "https://admin.movie88.com",
            "http://localhost:3000"  // Development
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();  // Allow cookies (refresh token)
    });
});
```

---

## 📊 5. Security Monitoring

### 5.1 Failed Login Attempts

```csharp
public async Task<IActionResult> Login([FromBody] LoginDTO dto)
{
    var user = await _userService.GetByEmailAsync(dto.Email);
    
    if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
    {
        // Log failed attempt
        await _securityLogService.LogFailedLoginAsync(dto.Email, Request.HttpContext.Connection.RemoteIpAddress);
        
        // Check if account should be locked
        var failedAttempts = await _securityLogService.GetFailedAttemptsCountAsync(dto.Email, TimeSpan.FromMinutes(15));
        
        if (failedAttempts >= 5)
        {
            await _userService.LockAccountAsync(user.UserId, TimeSpan.FromMinutes(15));
            return BadRequest(new { errorCode = "ACCOUNT_LOCKED", message = "Tài khoản bị khóa do quá nhiều lần đăng nhập sai" });
        }
        
        return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
    }
    
    // Success - clear failed attempts
    await _securityLogService.ClearFailedAttemptsAsync(dto.Email);
    
    // Generate tokens and return
}
```

---

## 🧪 6. Testing

### Unit Tests
```csharp
[Fact]
public void GenerateAccessToken_ShouldContainCorrectClaims()
{
    // Arrange
    var user = new User { UserId = 1, Email = "test@example.com", RoleId = 4 };
    
    // Act
    var token = _authService.GenerateAccessToken(user);
    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    
    // Assert
    Assert.Equal("1", jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    Assert.Equal("test@example.com", jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
}
```

---

**Last Updated**: October 29, 2025
**Version**: v1.0
