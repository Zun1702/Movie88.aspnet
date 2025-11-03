# 👤 User Management API

## 1. Mô tả

Module này quản lý toàn bộ vòng đời người dùng trong hệ thống Movie88, bao gồm:
- Đăng ký và xác thực tài khoản
- Đăng nhập/Đăng xuất với JWT Token
- Quản lý thông tin cá nhân
- Phân quyền dựa trên Role (Admin, Manager, Staff, Customer)
- Quản lý profile khách hàng

## 2. Danh sách Endpoint

### 2.1 Authentication Endpoints

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| POST | `/api/auth/register` | Đăng ký tài khoản mới | RegisterDTO | UserDTO + Token | ❌ |
| POST | `/api/auth/login` | Đăng nhập hệ thống | LoginDTO | UserDTO + Token | ❌ |
| POST | `/api/auth/logout` | Đăng xuất | - | Success message | ✅ |
| POST | `/api/auth/refresh-token` | Làm mới access token | RefreshTokenDTO | New Tokens | ✅ |
| POST | `/api/auth/forgot-password` | Yêu cầu đặt lại mật khẩu | Email | Success message | ❌ |
| POST | `/api/auth/reset-password` | Đặt lại mật khẩu | ResetPasswordDTO | Success message | ❌ |
| POST | `/api/auth/change-password` | Đổi mật khẩu | ChangePasswordDTO | Success message | ✅ |

### 2.2 User Management Endpoints

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/users` | Lấy danh sách users | Query params | List<UserDTO> | Admin |
| GET | `/api/users/{id}` | Lấy thông tin user theo ID | userId | UserDTO | Admin/Self |
| GET | `/api/users/me` | Lấy thông tin user hiện tại | - | UserDTO | All |
| PUT | `/api/users/{id}` | Cập nhật thông tin user | UpdateUserDTO | UserDTO | Admin/Self |
| DELETE | `/api/users/{id}` | Xóa user | userId | Success message | Admin |

### 2.3 Customer Profile Endpoints

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/customers/profile` | Lấy profile khách hàng | - | CustomerDTO | Customer |
| PUT | `/api/customers/profile` | Cập nhật profile | UpdateCustomerDTO | CustomerDTO | Customer |
| GET | `/api/customers/booking-history` | Lịch sử đặt vé | Query params | List<BookingDTO> | Customer |
| GET | `/api/customers/payment-history` | Lịch sử thanh toán | Query params | List<PaymentDTO> | Customer |

## 3. Data Transfer Objects (DTOs)

### 3.1 RegisterDTO
```json
{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "phone": "0901234567",
  "roleId": 4,
  "customerInfo": {
    "address": "123 Đường ABC, Quận 1, TP.HCM",
    "dateOfBirth": "1995-05-15",
    "gender": "Male"
  }
}
```

**Validation Rules:**
- `fullName`: Required, 3-100 ký tự
- `email`: Required, valid email format
- `password`: Required, min 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt
- `confirmPassword`: Phải khớp với password
- `phone`: Optional, format: 10 số
- `roleId`: Default = 4 (Customer)

### 3.2 LoginDTO
```json
{
  "email": "nguyenvana@example.com",
  "password": "Password123!"
}
```

### 3.3 UserDTO (Response)
```json
{
  "userId": 1,
  "roleId": 4,
  "roleName": "Customer",
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "phone": "0901234567",
  "createdAt": "2025-10-01T10:00:00Z",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "tokenExpiry": "2025-10-29T12:00:00Z"
}
```

### 3.4 UpdateUserDTO
```json
{
  "fullName": "Nguyễn Văn A Updated",
  "phone": "0987654321"
}
```

### 3.5 ChangePasswordDTO
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!",
  "confirmNewPassword": "NewPassword123!"
}
```

### 3.6 CustomerDTO
```json
{
  "customerId": 1,
  "userId": 5,
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "phone": "0901234567",
  "address": "123 Đường ABC, Quận 1, TP.HCM",
  "dateOfBirth": "1995-05-15",
  "gender": "Male",
  "totalBookings": 15,
  "totalSpent": 2500000
}
```

### 3.7 UpdateCustomerDTO
```json
{
  "address": "456 Đường XYZ, Quận 2, TP.HCM",
  "dateOfBirth": "1995-05-15",
  "gender": "Male"
}
```

## 4. Luồng xử lý (Flow)

### 4.1 Flow Đăng ký (Register)

```
1. User điền form đăng ký → POST /api/auth/register
2. Backend validate input:
   - Check email đã tồn tại chưa
   - Validate password strength
   - Validate phone format
3. Hash password bằng BCrypt
4. Tạo record trong bảng User (với RoleId = 4 - Customer)
5. Tạo record trong bảng Customers (liên kết với UserId)
6. Generate JWT Access Token & Refresh Token
7. Trả về UserDTO + Tokens
8. Frontend lưu token vào localStorage/cookie
9. Redirect đến trang chủ hoặc trang profile
```

### 4.2 Flow Đăng nhập (Login)

```
1. User nhập email/password → POST /api/auth/login
2. Backend tìm User theo email
3. So sánh password hash
4. Nếu đúng:
   - Generate JWT Access Token (expire 1h)
   - Generate Refresh Token (expire 7 days)
   - Trả về UserDTO + Tokens
5. Frontend lưu token và redirect:
   - Customer → App homepage
   - Admin/Manager/Staff → Admin dashboard
```

### 4.3 Flow Refresh Token

```
1. Access token hết hạn → Frontend nhận 401 Unauthorized
2. Frontend gọi POST /api/auth/refresh-token với refreshToken
3. Backend validate refresh token:
   - Check token hợp lệ
   - Check chưa hết hạn
4. Generate JWT Access Token mới
5. Trả về new access token
6. Frontend retry request ban đầu với token mới
```

### 4.4 Flow Quên mật khẩu

```
1. User click "Quên mật khẩu" → POST /api/auth/forgot-password
2. Nhập email → Backend check email tồn tại
3. Generate reset token (expire 30 phút)
4. Gửi email chứa link reset: 
   https://movie88.com/reset-password?token=xxx
5. User click link → Hiển thị form nhập password mới
6. Submit → POST /api/auth/reset-password
7. Backend validate token và update password
8. Redirect đến trang login
```

### 4.5 Flow Cập nhật Profile

```
1. Customer đăng nhập → GET /api/customers/profile
2. Hiển thị form với thông tin hiện tại
3. User chỉnh sửa (địa chỉ, ngày sinh, giới tính)
4. Submit → PUT /api/customers/profile
5. Backend validate và update bảng Customers
6. Trả về CustomerDTO mới
7. Frontend cập nhật UI
```

## 5. Business Rules

### 5.1 Quy tắc Đăng ký
- Email phải unique trong hệ thống
- Password tối thiểu 8 ký tự, có chữ hoa, thường, số và ký tự đặc biệt
- Mặc định role là Customer (roleId = 4)
- Tự động tạo CustomerProfile sau khi tạo User

### 5.2 Quy tắc Đăng nhập
- Giới hạn 5 lần đăng nhập sai liên tiếp → khóa tài khoản 15 phút
- Access token expire sau 1 giờ
- Refresh token expire sau 7 ngày

### 5.3 Quy tắc Phân quyền
| RoleId | RoleName | Quyền truy cập |
|--------|----------|----------------|
| 1 | Admin | Full quyền, quản lý toàn bộ hệ thống |
| 2 | Manager | Quản lý rạp, phim, suất chiếu, xem báo cáo |
| 3 | Staff | Quản lý booking, hỗ trợ khách hàng |
| 4 | Customer | Đặt vé, xem lịch sử, đánh giá phim |

## 6. Validation Rules

### Email Validation
```csharp
[Required(ErrorMessage = "Email là bắt buộc")]
[EmailAddress(ErrorMessage = "Email không hợp lệ")]
[MaxLength(100)]
```

### Password Validation
```csharp
[Required(ErrorMessage = "Mật khẩu là bắt buộc")]
[MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt")]
```

### Phone Validation
```csharp
[Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
[RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 số và bắt đầu bằng 0")]
```

## 7. Error Handling

### Common Error Codes

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 400 | `INVALID_INPUT` | "Dữ liệu đầu vào không hợp lệ" | Validation failed |
| 401 | `UNAUTHORIZED` | "Chưa đăng nhập" | Missing or invalid token |
| 403 | `FORBIDDEN` | "Không có quyền truy cập" | Insufficient permissions |
| 404 | `USER_NOT_FOUND` | "Không tìm thấy người dùng" | User doesn't exist |
| 409 | `EMAIL_EXISTS` | "Email đã được sử dụng" | Duplicate email |
| 409 | `ACCOUNT_LOCKED` | "Tài khoản đã bị khóa" | Too many failed login attempts |
| 422 | `WEAK_PASSWORD` | "Mật khẩu không đủ mạnh" | Password doesn't meet requirements |

### Error Response Example
```json
{
  "success": false,
  "statusCode": 409,
  "message": "Email đã được sử dụng",
  "errorCode": "EMAIL_EXISTS",
  "timestamp": "2025-10-29T10:30:00Z"
}
```

## 8. Security Considerations

### 8.1 Password Security
- Hash password bằng **BCrypt** với cost factor = 12
- Không bao giờ trả password trong response
- Enforce password complexity rules

### 8.2 JWT Token Security
```csharp
// JWT Configuration
{
  "Issuer": "Movie88API",
  "Audience": "Movie88Client",
  "SecretKey": "your-256-bit-secret-key",
  "AccessTokenExpiry": 3600,  // 1 hour
  "RefreshTokenExpiry": 604800 // 7 days
}
```

### 8.3 Rate Limiting
- Login endpoint: 5 requests/minute
- Register endpoint: 3 requests/minute
- Forgot password: 2 requests/hour

### 8.4 CORS Policy
```csharp
// AllowedOrigins
"https://movie88.com"
"https://admin.movie88.com"
"http://localhost:3000" // Development only
```

## 9. Testing Scenarios

### 9.1 Unit Tests
- ✅ Test password hashing
- ✅ Test JWT token generation
- ✅ Test email validation
- ✅ Test duplicate email detection

### 9.2 Integration Tests
- ✅ Test complete registration flow
- ✅ Test login with valid/invalid credentials
- ✅ Test token refresh mechanism
- ✅ Test password reset flow

### 9.3 Security Tests
- ✅ Test SQL injection prevention
- ✅ Test XSS prevention
- ✅ Test brute force protection
- ✅ Test token tampering detection

## 10. Sample API Calls

### Đăng ký
```bash
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "Nguyễn Văn A",
  "email": "nguyenvana@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!",
  "phone": "0901234567"
}
```

### Đăng nhập
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "nguyenvana@example.com",
  "password": "Password123!"
}
```

### Lấy thông tin user hiện tại
```bash
GET /api/users/me
Authorization: Bearer {accessToken}
```

### Cập nhật profile
```bash
PUT /api/customers/profile
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "address": "456 Đường XYZ, Quận 2, TP.HCM",
  "dateOfBirth": "1995-05-15",
  "gender": "Male"
}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
