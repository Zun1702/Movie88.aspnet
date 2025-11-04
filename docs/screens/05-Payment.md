# 💳 Screen 5: Payment & Vouchers (8 Endpoints)

**Status**: 🔄 **PENDING** (2/8 endpoints - 25%)  
**Assigned**: Trung

> **💳 Payment Integration**: VNPay sandbox integration với voucher system

---

## 📋 Endpoints Overview

Chia thành **3 giai đoạn** để dev hiệu quả:

### 💰 Phase 1: Voucher Management (2 endpoints)
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 1 | POST | `/api/vouchers/validate` | Validate voucher code | ✅ | ❌ TODO | Trung |
| 2 | POST | `/api/bookings/{id}/apply-voucher` | Apply voucher to booking | ✅ | ❌ TODO | Trung |

### 💳 Phase 2: VNPay Payment Integration (4 endpoints)
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 3 | POST | `/api/payments/vnpay/create` | Create VNPay payment URL | ✅ | ❌ TODO | Trung |
| 4 | GET | `/api/payments/vnpay/callback` | Handle VNPay redirect | ❌ | ❌ TODO | Trung |
| 5 | POST | `/api/payments/vnpay/ipn` | Handle VNPay IPN notification | ❌ | ❌ TODO | Trung |
| 6 | GET | `/api/payments/{id}` | Get payment details | ✅ | ❌ TODO | Trung |

### 📊 Phase 3: Booking Info (2 endpoints - Already Done)
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 7 | GET | `/api/bookings/{id}` | Get booking summary | ✅ | ✅ DONE | Trung |
| 8 | GET | `/api/bookings/{id}` | Get booking details | ✅ | ✅ DONE | Trung |

---

## 💰 PHASE 1: VOUCHER MANAGEMENT

### 🎯 1. POST /api/vouchers/validate

**Screen**: BookingSummaryActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "code": "STUDENT50",
  "bookingid": 156
}
```

### Validation Rules
- `code`: Required, voucher code
- `bookingid`: Required, must exist and belong to user

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Voucher is valid",
  "data": {
    "voucherid": 3,
    "code": "STUDENT50",
    "description": "50% discount for students",
    "discounttype": "percentage",
    "discountvalue": 50.00,
    "minpurchaseamount": 100000,
    "expirydate": "2025-12-31",
    "usagelimit": 100,
    "usedcount": 45,
    "isactive": true,
    "applicableDiscount": 100000
  }
}
```

### Related Entities
**Voucher** (vouchers table):
- ✅ `voucherid` (int, PK)
- ✅ `code` (string, max 50, unique)
- ✅ `description` (string, max 255, nullable)
- ✅ `discounttype` (string, max 20, nullable) - "percentage", "fixed"
- ✅ `discountvalue` (decimal(10,2), nullable)
- ✅ `minpurchaseamount` (decimal(10,2), nullable)
- ✅ `expirydate` (DateOnly, nullable)
- ✅ `usagelimit` (int, nullable)
- ✅ `usedcount` (int, nullable)
- ✅ `isactive` (bool, nullable)

### Business Logic
1. **Find Voucher**:
   - Search by code (case-insensitive)
   - Return 404 if not found

2. **Validate Voucher**:
   - ✅ `isactive = true`
   - ✅ `expirydate >= today`
   - ✅ `usedcount < usagelimit` (if usagelimit is set)

3. **Validate Booking**:
   - Get userId from JWT
   - Find Customer by userid
   - Verify booking belongs to customer
   - Verify booking status is "Pending"

4. **Check Minimum Purchase**:
   - `booking.totalamount >= voucher.minpurchaseamount`

5. **Calculate Discount**:
   ```csharp
   decimal applicableDiscount;
   if (voucher.Discounttype == "percentage")
   {
       applicableDiscount = booking.Totalamount * (voucher.Discountvalue / 100);
   }
   else // "fixed"
   {
       applicableDiscount = voucher.Discountvalue;
   }
   ```

6. **Return Validation Result**:
   - Include all voucher details
   - Include calculated discount amount
   - Don't apply yet (just validate)

### Error Cases
- 404 Not Found - Voucher code not found
- 400 Bad Request - Voucher expired, usage limit reached, or minimum purchase not met
- 403 Forbidden - Booking doesn't belong to user

---

### 🎯 2. POST /api/bookings/{id}/apply-voucher

**Screen**: BookingSummaryActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "code": "STUDENT50"
}
```

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Voucher applied successfully",
  "data": {
    "bookingid": 156,
    "voucherid": 3,
    "voucherCode": "STUDENT50",
    "originalAmount": 415000,
    "discountAmount": 207500,
    "totalamount": 207500
  }
}
```

### Business Logic
1. **Validate Voucher** (same as /validate endpoint)

2. **Apply Discount**:
   ```csharp
   decimal discountAmount;
   if (voucher.Discounttype == "percentage")
   {
       discountAmount = booking.Totalamount * (voucher.Discountvalue / 100);
   }
   else // "fixed"
   {
       discountAmount = voucher.Discountvalue;
   }
   
   // Don't let discount exceed total
   if (discountAmount > booking.Totalamount)
       discountAmount = booking.Totalamount;
   ```

3. **Update Booking**:
   ```csharp
   booking.Voucherid = voucher.Voucherid;
   booking.Totalamount = booking.Totalamount - discountAmount;
   await _context.SaveChangesAsync();
   ```

4. **Increment Usage Count**:
   ```csharp
   voucher.Usedcount = (voucher.Usedcount ?? 0) + 1;
   await _context.SaveChangesAsync();
   ```

5. **Transaction**:
   - Wrap in database transaction

### Error Cases
- Same as /validate endpoint
- 409 Conflict - Booking already has a voucher applied

---

## 💳 PHASE 2: VNPAY PAYMENT INTEGRATION

### 🎯 3. POST /api/payments/vnpay/create

**Screen**: BookingSummaryActivity  
**Auth Required**: ✅ Yes

### Request Body
```json
{
  "bookingid": 156,
  "returnurl": "movieapp://payment/result"
}
```

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Payment URL created successfully",
  "data": {
    "paymentid": 22,
    "bookingid": 156,
    "amount": 207500,
    "vnpayUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=20750000&vnp_Command=pay&vnp_CreateDate=20251103154500&vnp_CurrCode=VND&vnp_IpAddr=192.168.1.1&vnp_Locale=vn&vnp_OrderInfo=Thanh+toan+booking+156&vnp_OrderType=other&vnp_ReturnUrl=movieapp%3A%2F%2Fpayment%2Fresult&vnp_TmnCode=YOUR_TMN_CODE&vnp_TxnRef=PAY_20251103154500_156&vnp_Version=2.1.0&vnp_SecureHash=HASH_VALUE",
    "transactioncode": "PAY_20251103154500_156"
  }
}
```

### Related Entities
**Payment** (payments table):
- ✅ `paymentid` (int, PK)
- ✅ `bookingid` (int, FK)
- ✅ `customerid` (int, FK)
- ✅ `methodid` (int, FK to paymentmethods)
- ✅ `amount` (decimal(10,2))
- ✅ `status` (string, max 50, nullable) - "Pending", "Completed", "Failed", "Cancelled"
- ✅ `transactioncode` (string, max 255, nullable)
- ✅ `paymenttime` (timestamp, nullable)
- ❌ KHÔNG có: `vnpaydata` JSONB field

**Paymentmethod** (paymentmethods table):
- ✅ `methodid` (int, PK)
- ✅ `name` (string, max 50) - "VNPay", "MoMo", "Cash", etc.
- ✅ `description` (string, max 255, nullable)

### Business Logic
1. **Validate Booking**:
   - Get userId from JWT
   - Find Customer by userid
   - Verify booking belongs to customer
   - Verify booking status is "Pending"
   - Verify booking.totalamount > 0

2. **Find/Create Payment Method**:
   ```csharp
   var vnpayMethod = await _context.Paymentmethods
       .FirstOrDefaultAsync(pm => pm.Name == "VNPay");
   ```

3. **Create Payment Record**:
   ```csharp
   var payment = new Payment
   {
       Bookingid = booking.Bookingid,
       Customerid = booking.Customerid,
       Methodid = vnpayMethod.Methodid,
       Amount = booking.Totalamount,
       Status = "Pending",
       Transactioncode = GenerateTransactionCode(),
       Paymenttime = null // Set when payment completes
   };
   _context.Payments.Add(payment);
   await _context.SaveChangesAsync();
   ```

4. **Generate VNPay URL**:
   ```csharp
   // VNPay required parameters
   var vnp_Params = new Dictionary<string, string>
   {
       {"vnp_Version", "2.1.0"},
       {"vnp_Command", "pay"},
       {"vnp_TmnCode", _configuration["VNPay:TmnCode"]},
       {"vnp_Amount", (booking.Totalamount * 100).ToString()}, // VNPay uses cents
       {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
       {"vnp_CurrCode", "VND"},
       {"vnp_IpAddr", GetClientIpAddress()},
       {"vnp_Locale", "vn"},
       {"vnp_OrderInfo", $"Thanh toan booking {booking.Bookingid}"},
       {"vnp_OrderType", "other"},
       {"vnp_ReturnUrl", request.ReturnUrl},
       {"vnp_TxnRef", payment.Transactioncode}
   };
   
   // Sort and create query string
   var sortedParams = vnp_Params.OrderBy(x => x.Key);
   var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));
   
   // Generate secure hash
   var hashData = queryString;
   var vnpSecureHash = HmacSHA512(hashData, _configuration["VNPay:HashSecret"]);
   
   // Build final URL
   var vnpayUrl = $"{_configuration["VNPay:Url"]}?{queryString}&vnp_SecureHash={vnpSecureHash}";
   ```

5. **Return Payment Info**:
   - Return paymentid, bookingid, amount, vnpayUrl, transactioncode

### VNPay Configuration (appsettings.json)
```json
{
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "1F8WTZLN",
    "HashSecret": "MHUB7S9TTKIX3ZGI43G6TH7RTCE8RJVB",
    "ReturnUrl": "https://localhost:7238/api/payments/vnpay/callback"
  }
}
```

### VNPay Test Card (Sandbox)
```
Ngân hàng: NCB
Số thẻ: 9704198526191432198
Tên chủ thẻ: NGUYEN VAN A
Ngày phát hành: 07/15
Mật khẩu OTP: 123456
```

### Error Cases
- 403 Forbidden - Booking doesn't belong to user
- 400 Bad Request - Booking status not "Pending"
- 409 Conflict - Payment already exists for booking

---

### 🎯 4. GET /api/payments/vnpay/callback

**Screen**: VNPayWebViewActivity (auto-handled by WebView)  
**Auth Required**: ❌ No (VNPay calls this directly)

### Query Parameters
VNPay will redirect with these parameters:
```
?vnp_Amount=20750000
&vnp_BankCode=NCB
&vnp_BankTranNo=VNP123456
&vnp_CardType=ATM
&vnp_OrderInfo=Thanh+toan+booking+156
&vnp_PayDate=20251103154530
&vnp_ResponseCode=00
&vnp_TmnCode=YOUR_TMN_CODE
&vnp_TransactionNo=123456789
&vnp_TransactionStatus=00
&vnp_TxnRef=PAY_20251103154500_156
&vnp_SecureHash=HASH_VALUE
```

### Response (HTML redirect)
```html
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="refresh" content="0;url=movieapp://payment/result?status=success&bookingid=156" />
</head>
<body>
    <p>Payment successful. Redirecting...</p>
</body>
</html>
```

### Business Logic
1. **Validate Secure Hash**:
   ```csharp
   // Remove vnp_SecureHash from params
   var vnp_SecureHash = Request.Query["vnp_SecureHash"];
   var allParams = Request.Query
       .Where(x => x.Key != "vnp_SecureHash")
       .OrderBy(x => x.Key)
       .ToDictionary(x => x.Key, x => x.Value.ToString());
   
   var hashData = string.Join("&", allParams.Select(x => $"{x.Key}={x.Value}"));
   var checkSum = HmacSHA512(hashData, _configuration["VNPay:HashSecret"]);
   
   if (checkSum != vnp_SecureHash)
       return BadRequest("Invalid signature");
   ```

2. **Parse Response**:
   - `vnp_ResponseCode`: "00" = success, other = failed
   - `vnp_TxnRef`: Transaction code (matches Payment.transactioncode)
   - `vnp_Amount`: Payment amount (in cents)
   - `vnp_TransactionNo`: VNPay transaction number

3. **Update Payment Status & Generate BookingCode** (⚠️ CRITICAL):
   ```csharp
   var payment = await _context.Payments
       .Include(p => p.Booking)
       .FirstOrDefaultAsync(p => p.Transactioncode == vnp_TxnRef);
   
   if (vnp_ResponseCode == "00")
   {
       // Use execution strategy for transaction
       var strategy = _context.Database.CreateExecutionStrategy();
       await strategy.ExecuteAsync(async () =>
       {
           using var transaction = await _context.Database.BeginTransactionAsync();
           try
           {
               // Update payment
               payment.Status = "Completed";
               payment.Paymenttime = DateTime.Now; // Use DateTime.Now
               
               // Generate BookingCode (NOW after payment confirmed!)
               var bookingTime = DateTime.Now;
               var bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
               
               // Update booking
               var booking = payment.Booking;
               booking.Status = "Confirmed";
               booking.Bookingcode = bookingCode; // Set BookingCode here!
               
               await _context.SaveChangesAsync();
               await transaction.CommitAsync();
           }
           catch
           {
               await transaction.RollbackAsync();
               throw;
           }
       });
   }
   else
   {
       payment.Status = "Failed";
       await _context.SaveChangesAsync();
   }
   ```

4. **Redirect to App**:
   - Return HTML page that redirects to app deep link
   - Include status and bookingid in deep link

### VNPay Response Codes
- `00`: Success
- `07`: Transaction pending
- `09`: Card not registered for online payment
- `10`: Authentication failed (wrong OTP)
- `11`: Transaction timeout
- `12`: Card locked
- `13`: Wrong OTP
- `24`: Transaction cancelled
- `51`: Insufficient balance
- `65`: Daily limit exceeded
- `75`: Bank is under maintenance
- `79`: Transaction limit exceeded
- `99`: Unknown error

---

### 🎯 5. POST /api/payments/vnpay/ipn

**Screen**: PaymentResultActivity (auto-handled by VNPay)  
**Auth Required**: ❌ No (VNPay calls this directly)

### Request Body (Form Data)
VNPay sends same parameters as callback, but as POST request.

### Response 200 OK
```json
{
  "RspCode": "00",
  "Message": "Confirm Success"
}
```

### Business Logic
1. **Validate Secure Hash** (same as callback)

2. **Update Payment Status & Generate BookingCode** (same as callback - see endpoint 4)

3. **Return Confirmation to VNPay**:
   - `RspCode: "00"` = Success
   - `RspCode: "99"` = Error
   
⚠️ **IMPORTANT**: Must use same logic as callback to generate BookingCode and update booking status

### IPN (Instant Payment Notification)
- VNPay calls this endpoint to notify server of payment result
- Must respond within 30 seconds
- VNPay will retry if no response
- Used for server-side confirmation, separate from user-facing callback

---

### 🎯 6. GET /api/payments/{id}

**Screen**: PaymentResultActivity  
**Auth Required**: ✅ Yes

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Payment details retrieved successfully",
  "data": {
    "paymentid": 22,
    "bookingid": 156,
    "customerid": 3,
    "amount": 207500,
    "status": "Completed",
    "transactioncode": "PAY_20251103154500_156",
    "paymenttime": "2025-11-03T15:45:30",
    "method": {
      "methodid": 1,
      "name": "VNPay",
      "description": "VNPay Payment Gateway"
    },
    "booking": {
      "bookingid": 156,
      "bookingcode": "BK-20251103-0156",
      "status": "Confirmed",
      "totalamount": 207500
    }
  }
}
```

### Business Logic
1. **Validate Payment**:
   - Get userId from JWT
   - Find Customer by userid
   - Find Payment by paymentid with includes:
     - Method (Paymentmethod)
     - Booking (basic info)
   - Verify payment's booking belongs to customer

2. **Return Payment Details**:
   - Include payment method information
   - Include basic booking information
   - Show payment status and transaction code

### Error Cases
- 403 Forbidden - Payment doesn't belong to user
- 404 Not Found - Payment not found

---

## 📊 PHASE 3: BOOKING INFO (ALREADY DONE)

### 🎯 7-8. GET /api/bookings/{id}

**Status**: ✅ **ALREADY IMPLEMENTED** (see Screen 02-Home-MainScreens.md and Screen 04-Booking-Flow.md)

**Screens**: 
- BookingSummaryActivity (before payment)
- PaymentResultActivity (after payment)

**Auth Required**: ✅ Yes

### Response
Returns full booking details including:
- Movie, cinema, showtime information
- Selected seats with prices
- Applied combos with quantities
- Voucher details (if applied)
- Payment information
- Booking status and code

This endpoint is reused in both payment screens:
1. **BookingSummaryActivity**: Shows booking summary before payment
2. **PaymentResultActivity**: Shows confirmed booking after payment with BookingCode

---

## 📊 Implementation Summary

### ✅ Already Exist (Entities)

#### Infrastructure Layer (Movie88.Infrastructure/Entities/)
```
✅ Voucher.cs              - Already exists
✅ Payment.cs              - Already exists
✅ Paymentmethod.cs        - Already exists
✅ Booking.cs              - Already exists (with Voucherid FK)
```

### 🔄 To Be Created/Extended

#### Domain Layer (Movie88.Domain/)

**Folder: Models/**
```
❌ PaymentModel.cs         - NEW
❌ VoucherModel.cs         - NEW
❌ PaymentmethodModel.cs   - NEW
```

**Folder: Interfaces/**
```
❌ IVoucherRepository.cs   - NEW
❌ IPaymentRepository.cs   - NEW
❌ IPaymentmethodRepository.cs - NEW
```

#### Application Layer (Movie88.Application/)

**Folder: DTOs/Vouchers/**
```
❌ VoucherDTO.cs           - NEW
   - ValidateVoucherRequestDTO
   - ValidateVoucherResponseDTO
   - ApplyVoucherRequestDTO
   - ApplyVoucherResponseDTO
```

**Folder: DTOs/Payments/**
```
❌ PaymentDTO.cs           - NEW
   - CreatePaymentRequestDTO
   - CreatePaymentResponseDTO
   - PaymentDetailDTO
   - VNPayCallbackParamsDTO
```

**Folder: Services/**
```
❌ IVoucherService.cs / VoucherService.cs - NEW
   - ValidateVoucherAsync()
   - ApplyVoucherToBookingAsync()
   - CalculateDiscountAsync()

❌ IPaymentService.cs / PaymentService.cs - NEW
   - CreateVNPayPaymentAsync()
   - ProcessVNPayCallbackAsync()
   - ProcessVNPayIPNAsync()
   - GetPaymentByIdAsync()
   - GetPaymentByTransactionCodeAsync()

❌ IVNPayService.cs / VNPayService.cs - NEW (Helper service)
   - GeneratePaymentUrl()
   - ValidateSignature()
   - GenerateTransactionCode()
   - HmacSHA512()
```

**Folder: Interfaces/**
```
❌ IBookingCodeGenerator.cs - EXTEND (add method if needed)
```

#### Infrastructure Layer (Movie88.Infrastructure/)

**Folder: Repositories/**
```
❌ VoucherRepository.cs    - NEW
   - GetByCodeAsync()
   - IncrementUsageCountAsync()

❌ PaymentRepository.cs    - NEW
   - CreatePaymentAsync()
   - UpdatePaymentStatusAsync()
   - GetByIdWithDetailsAsync()
   - GetByTransactionCodeAsync()

❌ PaymentmethodRepository.cs - NEW
   - GetByNameAsync()
```

**Folder: ServiceExtensions.cs**
```
✅ EXTEND - Register new services and repositories
```

#### WebApi Layer (Movie88.WebApi/)

**Folder: Controllers/**
```
❌ VouchersController.cs   - NEW (1 endpoint)
   - POST /api/vouchers/validate

❌ PaymentsController.cs   - NEW (5 endpoints)
   - POST /api/payments/vnpay/create
   - GET /api/payments/vnpay/callback
   - POST /api/payments/vnpay/ipn
   - GET /api/payments/{id}
   
✅ BookingsController.cs   - EXTEND (1 endpoint)
   - POST /api/bookings/{id}/apply-voucher
```

**File: appsettings.json**
```json
❌ ADD VNPay Configuration:
{
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "1F8WTZLN",
    "HashSecret": "MHUB7S9TTKIX3ZGI43G6TH7RTCE8RJVB",
    "ReturnUrl": "https://localhost:7238/api/payments/vnpay/callback"
  }
}
```

---

## 📝 Notes for Implementation

### Important Field Mappings

**Voucher Entity**:
- ⚠️ `expirydate` is DateOnly, NOT DateTime
- ⚠️ `discounttype`: "percentage" or "fixed"
- ⚠️ `usagelimit` and `usedcount` nullable
- ❌ NO `validfrom`, `validto`, `maxdiscount` fields

**Payment Entity**:
- ⚠️ Uses `methodid` FK to Paymentmethod table
- ⚠️ Uses `transactioncode`, NOT `transactionid`
- ⚠️ `paymenttime` is timestamp without time zone - Use `DateTime.Now` not `DateTime.UtcNow`
- ⚠️ Status: "Pending", "Completed", "Failed", "Cancelled"
- ❌ NO `vnpaydata` JSONB field

**Paymentmethod Entity**:
- ⚠️ Uses `name` field, NOT `methodname`
- ⚠️ Seed data should include: "VNPay", "MoMo", "Cash"

**Booking Entity**:
- ⚠️ `voucherid` is nullable FK
- ⚠️ `totalamount` updated when voucher applied
- ⚠️ `bookingcode` is nullable - **Generated ONLY after payment confirmed in callback/IPN**
- ⚠️ Status changes: "Pending" → "Confirmed" after successful payment

### VNPay Integration

**Hash Generation**:
```csharp
private string HmacSHA512(string data, string key)
{
    var keyBytes = Encoding.UTF8.GetBytes(key);
    using var hmac = new HMACSHA512(keyBytes);
    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
}
```

**Transaction Code Format**:
```csharp
private string GenerateTransactionCode(int bookingId)
{
    return $"PAY_{DateTime.Now:yyyyMMddHHmmss}_{bookingId}";
}
```

**BookingCode Generation** (⚠️ CRITICAL):
```csharp
// ONLY generate BookingCode AFTER payment confirmed
// In VNPay callback/IPN when vnp_ResponseCode == "00"
if (vnp_ResponseCode == "00")
{
    var bookingTime = DateTime.Now;
    var bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
    booking.Bookingcode = bookingCode; // Set it here!
    booking.Status = "Confirmed";
}
```

**Amount Conversion**:
```csharp
// VNPay uses smallest currency unit (cents for VND)
var vnpayAmount = (booking.Totalamount * 100).ToString("0");
```

### Business Logic Notes

**Voucher Validation**:
```csharp
// Check expiry
if (voucher.Expirydate.HasValue && 
    voucher.Expirydate.Value < DateOnly.FromDateTime(DateTime.UtcNow))
    return BadRequest("Voucher has expired");

// Check usage limit
if (voucher.Usagelimit.HasValue && 
    voucher.Usedcount >= voucher.Usagelimit)
    return BadRequest("Voucher usage limit reached");

// Check minimum purchase
if (voucher.Minpurchaseamount.HasValue && 
    booking.Totalamount < voucher.Minpurchaseamount)
    return BadRequest($"Minimum purchase amount is {voucher.Minpurchaseamount}");
```

**Discount Calculation**:
```csharp
decimal CalculateDiscount(Voucher voucher, decimal totalAmount)
{
    decimal discount;
    
    if (voucher.Discounttype == "percentage")
    {
        discount = totalAmount * (voucher.Discountvalue / 100);
    }
    else // "fixed"
    {
        discount = voucher.Discountvalue;
    }
    
    // Don't let discount exceed total
    return Math.Min(discount, totalAmount);
}
```

**Payment Transaction with Execution Strategy**:
```csharp
// MUST use execution strategy for retry compatibility
var strategy = _context.Database.CreateExecutionStrategy();
await strategy.ExecuteAsync(async () =>
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // Update payment
        payment.Status = "Completed";
        payment.Paymenttime = DateTime.Now; // Use DateTime.Now
        
        // Generate BookingCode (ONLY here after payment!)
        var bookingTime = DateTime.Now;
        var bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
        
        // Update booking
        booking.Status = "Confirmed";
        booking.Bookingcode = bookingCode;
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
});
```

### PostgreSQL Specific
- DateOnly for voucher expirydate
- timestamp without time zone for paymenttime - Use `DateTime.Now`
- Use execution strategy pattern for all transactions with retry enabled
- BookingCode nullable until payment confirmed

---

## 🧪 Testing Checklist

### Phase 1: Voucher Management
#### POST /api/vouchers/validate
- [ ] Validate expired vouchers (expirydate < today)
- [ ] Validate usage limit exceeded (usedcount >= usagelimit)
- [ ] Validate minimum purchase amount (totalamount >= minpurchaseamount)
- [ ] Calculate discount correctly (percentage vs fixed)
- [ ] Return 403 for booking not belonging to user
- [ ] Return 404 for voucher code not found
- [ ] Check isactive = true

#### POST /api/bookings/{id}/apply-voucher
- [ ] Apply voucher correctly
- [ ] Update booking.totalamount (original - discount)
- [ ] Increment voucher.usedcount
- [ ] Set booking.voucherid
- [ ] Prevent double application (check if already has voucher)
- [ ] Use execution strategy for transaction
- [ ] Verify booking ownership

### Phase 2: VNPay Payment
#### POST /api/payments/vnpay/create
- [ ] Create payment record with status "Pending"
- [ ] Generate valid VNPay URL with all required params
- [ ] Generate secure hash correctly (HMACSHA512)
- [ ] Handle VND amount conversion (amount × 100)
- [ ] Generate unique transaction code (PAY_YYYYMMDDHHMMSS_bookingId)
- [ ] Return payment URL for redirect
- [ ] Verify booking belongs to user
- [ ] Check booking status is "Pending"

#### GET /api/payments/vnpay/callback
- [ ] Validate secure hash from VNPay
- [ ] Parse vnp_ResponseCode correctly
- [ ] Update payment status (Completed/Failed)
- [ ] **Generate BookingCode ONLY on success (vnp_ResponseCode == "00")**
- [ ] Update booking status to "Confirmed"
- [ ] Use execution strategy for transaction
- [ ] Redirect to app with correct deep link
- [ ] Handle all VNPay response codes (00, 07, 09, 10, 11, 12, 13, 24, 51, 65, 75, 79, 99)
- [ ] Use DateTime.Now not DateTime.UtcNow

#### POST /api/payments/vnpay/ipn
- [ ] Validate secure hash from VNPay
- [ ] Same logic as callback (BookingCode generation)
- [ ] Update payment and booking atomically
- [ ] Respond within 30 seconds
- [ ] Return correct RspCode ("00" or "99") to VNPay
- [ ] Handle retries from VNPay
- [ ] Use execution strategy for transaction

#### GET /api/payments/{id}
- [ ] Return payment details with method info
- [ ] Include booking info (with BookingCode if confirmed)
- [ ] Verify payment belongs to user
- [ ] Return 404 if payment not found
- [ ] Return 403 if not user's payment

### Phase 3: Booking Info
#### GET /api/bookings/{id}
- [x] Already tested in Screen 02 and Screen 04
- [x] Returns full booking with all related data
- [x] Shows BookingCode after payment confirmed
- [x] Shows voucher discount if applied

---

## 📝 VNPay Test Credentials

**Sandbox Environment:**
- URL: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
- TMN Code: `1F8WTZLN`
- Hash Secret: `MHUB7S9TTKIX3ZGI43G6TH7RTCE8RJVB`

**Test Card:**
- Bank: NCB
- Card Number: `9704198526191432198`
- Card Holder: NGUYEN VAN A
- Issue Date: 07/15
- OTP: `123456`

**Merchant Admin:** https://sandbox.vnpayment.vn/merchantv2/
- Email: ngoctrungtsn111@gmail.com

**Test Scenarios:** https://sandbox.vnpayment.vn/vnpaygw-sit-testing/user/login

---

**Created**: November 3, 2025  
**Last Updated**: November 5, 2025  
**Progress**: ✅ 2/8 endpoints (25%) - 2 booking detail endpoints reused  
**Test File**: `tests/Payment.http` ✅
