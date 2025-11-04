# 👤 Screen 6: Profile & History (6 Endpoints)

**Status**: 🔄 **PENDING** (0/6 endpoints - 0%)

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Screen | Auth | Status |
|---|--------|----------|--------|------|--------|
| 1 | GET | `/api/users/me` | ProfileFragment, EditProfileActivity | ✅ | ❌ TODO |
| 2 | GET | `/api/customers/profile` | ProfileFragment | ✅ | ✅ DONE (Screen 2) |
| 3 | PUT | `/api/users/{id}` | EditProfileActivity | ✅ | ❌ TODO |
| 4 | PUT | `/api/customers/profile` | EditProfileActivity | ✅ | ❌ TODO |
| 5 | GET | `/api/bookings/my-bookings` | BookingsFragment | ✅ | ✅ DONE (Screen 2) |
| 6 | POST | `/api/auth/change-password` | ProfileFragment | ✅ | ✅ DONE (Screen 1) |
| 7 | POST | `/api/auth/logout` | ProfileFragment | ✅ | ✅ DONE (Screen 1) |

---

## 🎯 1. GET /api/users/me

**Screen**: ProfileFragment, EditProfileActivity  
**Auth Required**: ✅ Yes

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "User information retrieved successfully",
  "data": {
    "userid": 6,
    "fullname": "Customer User",
    "email": "customer@example.com",
    "phone": "0901234567",
    "roleid": 3,
    "rolename": "Customer",
    "createdat": "2025-10-01T08:00:00",
    "updatedat": "2025-11-03T10:15:00"
  }
}
```

### Related Entities
**User** (users table):
- ✅ `userid` (int, PK)
- ✅ `roleid` (int, FK to roles)
- ✅ `fullname` (string, max 100)
- ✅ `email` (string, max 100)
- ✅ `passwordhash` (string, max 255) - NOT returned
- ✅ `phone` (string, max 20, nullable)
- ✅ `createdat` (timestamp, nullable)
- ✅ `updatedat` (timestamp, nullable)

**Role** (roles table):
- ✅ `roleid` (int, PK)
- ✅ `rolename` (string, max 50)

### Business Logic
- Get userId from JWT token claims
- Find User by userid
- Include Role information
- Do NOT return passwordhash
- Return 404 if user not found

---

## 🎯 2. GET /api/customers/profile

**Screen**: ProfileFragment  
**Auth Required**: ✅ Yes

### Status
✅ **ALREADY IMPLEMENTED** (see Screen 02-Home-MainScreens.md)

### Response
Returns customer profile with user information including fullname, email, phone, address, dateofbirth, gender.

---

## 🎯 3. PUT /api/users/{id}

**Screen**: EditProfileActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "fullname": "Updated Name",
  "phone": "0912345678"
}
```

### Validation Rules
- `fullname`: Required, max 100 characters
- `phone`: Optional, max 20 characters, valid phone format
- Email cannot be changed (omitted from update)

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "User information updated successfully",
  "data": {
    "userid": 6,
    "fullname": "Updated Name",
    "email": "customer@example.com",
    "phone": "0912345678",
    "rolename": "Customer",
    "updatedat": "2025-11-03T16:30:00"
  }
}
```

### Business Logic
1. **Validate User**:
   - Get userId from JWT token
   - Verify {id} matches userId from token (users can only edit their own profile)
   - Return 403 Forbidden if mismatch

2. **Update User**:
   ```csharp
   user.Fullname = request.Fullname;
   user.Phone = request.Phone;
   user.Updatedat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
   
   await _context.SaveChangesAsync();
   ```

3. **Return Updated User**:
   - Include rolename from Role navigation

### Error Cases
- 403 Forbidden - User trying to edit another user's profile
- 400 Bad Request - Validation errors
- 404 Not Found - User not found

---

## 🎯 4. PUT /api/customers/profile

**Screen**: EditProfileActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "address": "123 Nguyen Hue St, District 1",
  "dateofbirth": "1995-05-15",
  "gender": "Male"
}
```

### Validation Rules
- `address`: Optional, max 255 characters
- `dateofbirth`: Optional, valid date format (yyyy-MM-dd)
- `gender`: Optional, max 10 characters (Male, Female, Other)

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Customer profile updated successfully",
  "data": {
    "customerid": 3,
    "userid": 6,
    "fullname": "Updated Name",
    "email": "customer@example.com",
    "phone": "0912345678",
    "address": "123 Nguyen Hue St, District 1",
    "dateofbirth": "1995-05-15",
    "gender": "Male"
  }
}
```

### Related Entities
**Customer** (customers table):
- ✅ `customerid` (int, PK)
- ✅ `userid` (int, FK to users, unique)
- ✅ `address` (string, max 255, nullable)
- ✅ `dateofbirth` (DateOnly, nullable)
- ✅ `gender` (string, max 10, nullable)

### Business Logic
1. **Find Customer**:
   - Get userId from JWT token
   - Find Customer by userid
   - Return 404 if customer profile not found

2. **Update Customer**:
   ```csharp
   customer.Address = request.Address;
   
   if (!string.IsNullOrEmpty(request.DateOfBirth))
   {
       customer.Dateofbirth = DateOnly.Parse(request.DateOfBirth);
   }
   
   customer.Gender = request.Gender;
   
   await _context.SaveChangesAsync();
   ```

3. **Return Updated Profile**:
   - Include User information (fullname, email, phone)

### Error Cases
- 404 Not Found - Customer profile not found
- 400 Bad Request - Invalid date format

---

## 🎯 5. GET /api/bookings/my-bookings

**Screen**: BookingsFragment  
**Auth Required**: ✅ Yes

### Status
✅ **ALREADY IMPLEMENTED** (see Screen 02-Home-MainScreens.md)

### Response
Returns paginated list of user's bookings with movie, cinema, showtime, seats, combos info.

---

## 🎯 6. POST /api/auth/change-password

**Screen**: ProfileFragment  
**Auth Required**: ✅ Yes

### Status
✅ **ALREADY IMPLEMENTED** (see Screen 01-Authentication.md)

### Request Body
```json
{
  "oldPassword": "OldPassword@123",
  "newPassword": "NewPassword@456",
  "confirmNewPassword": "NewPassword@456"
}
```

---

## 🎯 7. POST /api/auth/logout

**Screen**: ProfileFragment  
**Auth Required**: ✅ Yes

### Status
✅ **ALREADY IMPLEMENTED** (see Screen 01-Authentication.md)

### Request Body
```json
{
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

## 📊 Implementation Summary

### To Be Created

#### Domain Layer (Movie88.Domain/Models/)
```
❌ (UserModel.cs)          - Already exists (Screen 1)
❌ (CustomerModel.cs)      - Already needed for Screen 2
```

#### Application Layer (Movie88.Application/)
```
❌ DTOs/Users/
   - UserDTO.cs
   - UpdateUserRequestDTO.cs

❌ DTOs/Customers/
   - UpdateCustomerProfileRequestDTO.cs

❌ Services/
   - IUserService.cs / UserService.cs
   - (CustomerService - extend existing)
```

#### Infrastructure Layer (Movie88.Infrastructure/)
```
❌ Repositories/
   - (UserRepository - extend existing)
   - (CustomerRepository - extend existing)
```

#### WebApi Layer (Movie88.WebApi/)
```
❌ Controllers/
   - UsersController.cs (2 endpoints)
   - CustomersController.cs (1 endpoint - extend)
```

---

## 📝 Notes for Implementation

### Important Field Mappings

**User Entity**:
- ✅ `fullname`, `email`, `phone`
- ✅ `createdat`, `updatedat` timestamps
- ⚠️ Do NOT return `passwordhash` in responses
- ⚠️ Email cannot be changed (security)

**Customer Entity**:
- ✅ `address` (string, max 255, nullable)
- ✅ `dateofbirth` (DateOnly, nullable)
- ✅ `gender` (string, max 10, nullable)

### Business Logic Notes

**User Authorization**:
```csharp
// Get userId from JWT claims
var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (!int.TryParse(userIdClaim, out int userId))
    return Unauthorized();

// Verify user can only edit their own profile
if (id != userId)
    return Forbid("You can only edit your own profile");
```

**Update User**:
```csharp
// Only update allowed fields
user.Fullname = request.Fullname ?? user.Fullname;
user.Phone = request.Phone ?? user.Phone;
user.Updatedat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

await _context.SaveChangesAsync();
```

**Update Customer**:
```csharp
// Find customer by userid
var customer = await _context.Customers
    .Include(c => c.User)
    .ThenInclude(u => u.Role)
    .FirstOrDefaultAsync(c => c.Userid == userId);

if (customer == null)
    return NotFound("Customer profile not found");

// Update fields
customer.Address = request.Address ?? customer.Address;

if (!string.IsNullOrEmpty(request.DateOfBirth))
{
    if (DateOnly.TryParse(request.DateOfBirth, out var dob))
        customer.Dateofbirth = dob;
}

customer.Gender = request.Gender ?? customer.Gender;

await _context.SaveChangesAsync();
```

### PostgreSQL Specific
- DateOnly for dateofbirth
- timestamp without time zone for updatedat
- Use DateTime.SpecifyKind for timezone handling

---

## 🧪 Testing Checklist

### GET /api/users/me
- [ ] Require authentication
- [ ] Return correct user info
- [ ] Include role name
- [ ] Do NOT return passwordhash
- [ ] Return 404 if user deleted

### PUT /api/users/{id}
- [ ] Require authentication
- [ ] Verify user can only edit own profile (403 for others)
- [ ] Update fullname and phone
- [ ] Cannot change email
- [ ] Update updatedat timestamp
- [ ] Validate phone format

### PUT /api/customers/profile
- [ ] Require authentication
- [ ] Find customer by userid from token
- [ ] Update address, dateofbirth, gender
- [ ] Handle null values (don't overwrite)
- [ ] Validate dateofbirth format
- [ ] Return 404 if customer profile not found

---

**Created**: November 3, 2025  
**Last Updated**: November 4, 2025  
**Progress**: ✅ 3/6 endpoints (50%) - 3 already implemented in previous screens
