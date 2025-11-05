# 💳 Screen 5: Payment & Vouchers (8 Endpoints)

**Status**: ✅ **DONE** (8/8 endpoints - 100%)  
**Assigned**: Trung

> **💳 Payment Integration**: VNPay sandbox integration với voucher system  
> ✅ Phase 1 Complete | ✅ Phase 2 Complete | ✅ Phase 3 Done

---

## 📋 Endpoints Overview

Chia thành **3 giai đoạn** để dev hiệu quả:

### 💰 Phase 1: Voucher Management (2 endpoints) ✅ DONE
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 1 | POST | `/api/vouchers/validate` | Validate voucher code | ✅ | ✅ DONE | Trung |
| 2 | POST | `/api/bookings/{id}/apply-voucher` | Apply voucher to booking | ✅ | ✅ DONE | Trung |

### 💳 Phase 2: VNPay Payment Integration (4 endpoints) ✅ DONE
| # | Method | Endpoint | Purpose | Auth | Status | Assign |
|---|--------|----------|---------|------|--------|--------|
| 3 | POST | `/api/payments/vnpay/create` | Create VNPay payment URL | ✅ | ✅ DONE | Trung |
| 4 | GET | `/api/payments/vnpay/callback` | Handle VNPay redirect | ❌ | ✅ DONE | Trung |
| 5 | POST | `/api/payments/vnpay/ipn` | Handle VNPay IPN notification | ❌ | ✅ DONE | Trung |
| 6 | GET | `/api/payments/{id}` | Get payment details | ✅ | ✅ DONE | Trung |

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

## 📱 Android Integration Guide (Java + XML)

### 🔧 Setup Deep Link for VNPay Callback

#### 1. AndroidManifest.xml Configuration

Add deep link intent filter to your Payment Result Activity:

```xml
<!-- AndroidManifest.xml -->
<application>
    <!-- ... other activities ... -->
    
    <!-- Payment Result Activity -->
    <activity
        android:name=".activities.PaymentResultActivity"
        android:exported="true"
        android:launchMode="singleTask">
        
        <!-- Deep Link for VNPay callback -->
        <intent-filter>
            <action android:name="android.intent.action.VIEW" />
            <category android:name="android.intent.category.DEFAULT" />
            <category android:name="android.intent.category.BROWSABLE" />
            
            <!-- Deep link scheme: movieapp://payment/result -->
            <data
                android:scheme="movieapp"
                android:host="payment"
                android:pathPrefix="/result" />
        </intent-filter>
    </activity>
</application>
```

#### 2. Gradle Dependencies

```gradle
// build.gradle (Module: app)
dependencies {
    // Retrofit for API calls
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    implementation 'com.squareup.okhttp3:logging-interceptor:4.11.0'
    
    // JWT token handling
    implementation 'com.auth0.android:jwtdecode:2.0.1'
    
    // Chrome Custom Tabs for VNPay payment
    implementation 'androidx.browser:browser:1.7.0'
}
```

### 📦 API Service Setup

#### 3. Payment API Interface

```java
// PaymentApiService.java
public interface PaymentApiService {
    
    @POST("payments/vnpay/create")
    Call<ApiResponse<CreatePaymentResponse>> createVNPayPayment(
        @Header("Authorization") String token,
        @Body CreatePaymentRequest request
    );
    
    @GET("payments/{id}")
    Call<ApiResponse<PaymentDetailDTO>> getPaymentDetails(
        @Header("Authorization") String token,
        @Path("id") int paymentId
    );
}
```

#### 4. Request/Response Models

```java
// CreatePaymentRequest.java
public class CreatePaymentRequest {
    private int bookingid;
    private String returnurl; // Optional, uses default if not provided
    
    public CreatePaymentRequest(int bookingid) {
        this.bookingid = bookingid;
    }
    
    public CreatePaymentRequest(int bookingid, String returnurl) {
        this.bookingid = bookingid;
        this.returnurl = returnurl;
    }
    
    // Getters and setters
}

// CreatePaymentResponse.java
public class CreatePaymentResponse {
    private int paymentid;
    private int bookingid;
    private double amount;
    private String vnpayUrl;
    private String transactioncode;
    
    // Getters and setters
}

// PaymentDetailDTO.java
public class PaymentDetailDTO {
    private int paymentid;
    private double amount;
    private String status;
    private String transactioncode;
    private String paymenttime;
    private PaymentMethodInfo method;
    private BookingSummary booking;
    
    // Getters and setters
}

// ApiResponse.java (wrapper)
public class ApiResponse<T> {
    private boolean success;
    private int statusCode;
    private String message;
    private T data;
    
    // Getters and setters
}
```

### 🎨 UI Implementation

#### 5. Booking Summary Layout (XML)

```xml
<!-- activity_booking_summary.xml -->
<?xml version="1.0" encoding="utf-8"?>
<ScrollView xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/background">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical"
        android:padding="16dp">

        <!-- Booking Info -->
        <TextView
            android:id="@+id/tvBookingId"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Booking #156"
            android:textSize="18sp"
            android:textStyle="bold" />

        <TextView
            android:id="@+id/tvTotalAmount"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Total: 415,000 VND"
            android:textSize="16sp"
            android:layout_marginTop="8dp" />

        <!-- Voucher Section -->
        <com.google.android.material.card.MaterialCardView
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_marginTop="16dp"
            app:cardCornerRadius="8dp">

            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="vertical"
                android:padding="16dp">

                <TextView
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="Apply Voucher"
                    android:textStyle="bold" />

                <EditText
                    android:id="@+id/etVoucherCode"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:hint="Enter voucher code"
                    android:layout_marginTop="8dp" />

                <Button
                    android:id="@+id/btnApplyVoucher"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:text="Apply Voucher"
                    android:layout_marginTop="8dp" />

                <TextView
                    android:id="@+id/tvDiscount"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="Discount: 0 VND"
                    android:textColor="@color/green"
                    android:layout_marginTop="8dp"
                    android:visibility="gone" />
            </LinearLayout>
        </com.google.android.material.card.MaterialCardView>

        <!-- Payment Button -->
        <Button
            android:id="@+id/btnPayNow"
            android:layout_width="match_parent"
            android:layout_height="60dp"
            android:text="Pay with VNPay"
            android:textSize="18sp"
            android:layout_marginTop="24dp"
            android:backgroundTint="@color/primary" />

    </LinearLayout>
</ScrollView>
```

#### 6. Payment Result Layout (XML)

```xml
<!-- activity_payment_result.xml -->
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:gravity="center"
    android:padding="24dp">

    <ImageView
        android:id="@+id/ivResultIcon"
        android:layout_width="120dp"
        android:layout_height="120dp"
        android:src="@drawable/ic_success"
        android:contentDescription="Payment Result" />

    <TextView
        android:id="@+id/tvResultTitle"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Payment Successful!"
        android:textSize="24sp"
        android:textStyle="bold"
        android:layout_marginTop="24dp" />

    <TextView
        android:id="@+id/tvResultMessage"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Your booking has been confirmed"
        android:textAlignment="center"
        android:layout_marginTop="8dp" />

    <TextView
        android:id="@+id/tvBookingCode"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Booking Code: BK-20251105-0156"
        android:textSize="18sp"
        android:textStyle="bold"
        android:layout_marginTop="16dp" />

    <TextView
        android:id="@+id/tvTransactionCode"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Transaction: PAY_20251105154500_156"
        android:textSize="14sp"
        android:textColor="@color/gray"
        android:layout_marginTop="8dp" />

    <Button
        android:id="@+id/btnViewBooking"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="View Booking Details"
        android:layout_marginTop="32dp" />

    <Button
        android:id="@+id/btnBackHome"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="Back to Home"
        android:layout_marginTop="8dp"
        style="@style/Widget.MaterialComponents.Button.OutlinedButton" />

</LinearLayout>
```

### 💻 Java Implementation

#### 7. Booking Summary Activity

```java
// BookingSummaryActivity.java
public class BookingSummaryActivity extends AppCompatActivity {
    
    private TextView tvBookingId, tvTotalAmount, tvDiscount;
    private EditText etVoucherCode;
    private Button btnApplyVoucher, btnPayNow;
    
    private int bookingId;
    private double totalAmount;
    private String jwtToken;
    
    private PaymentApiService apiService;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_booking_summary);
        
        initViews();
        initApiService();
        loadBookingData();
        
        btnApplyVoucher.setOnClickListener(v -> applyVoucher());
        btnPayNow.setOnClickListener(v -> createPayment());
    }
    
    private void initViews() {
        tvBookingId = findViewById(R.id.tvBookingId);
        tvTotalAmount = findViewById(R.id.tvTotalAmount);
        tvDiscount = findViewById(R.id.tvDiscount);
        etVoucherCode = findViewById(R.id.etVoucherCode);
        btnApplyVoucher = findViewById(R.id.btnApplyVoucher);
        btnPayNow = findViewById(R.id.btnPayNow);
    }
    
    private void initApiService() {
        // Get from intent or SharedPreferences
        jwtToken = getIntent().getStringExtra("JWT_TOKEN");
        bookingId = getIntent().getIntExtra("BOOKING_ID", 0);
        
        Retrofit retrofit = new Retrofit.Builder()
            .baseUrl("https://movie88aspnet-app.up.railway.app/api/")
            .addConverterFactory(GsonConverterFactory.create())
            .build();
            
        apiService = retrofit.create(PaymentApiService.class);
    }
    
    private void loadBookingData() {
        // Load booking details from previous activity or API
        tvBookingId.setText("Booking #" + bookingId);
        tvTotalAmount.setText(String.format("Total: %,d VND", (int)totalAmount));
    }
    
    private void applyVoucher() {
        String voucherCode = etVoucherCode.getText().toString().trim();
        if (voucherCode.isEmpty()) {
            Toast.makeText(this, "Please enter voucher code", Toast.LENGTH_SHORT).show();
            return;
        }
        
        // TODO: Call validate/apply voucher API
        // After success, update totalAmount and show discount
        tvDiscount.setVisibility(View.VISIBLE);
        tvDiscount.setText("Discount: 50,000 VND");
    }
    
    private void createPayment() {
        // Show loading
        ProgressDialog progressDialog = new ProgressDialog(this);
        progressDialog.setMessage("Creating payment...");
        progressDialog.show();
        
        // Use API callback URL (registered with VNPay)
        // VNPay → API → Process & validate → Redirect to app via deep link
        CreatePaymentRequest request = new CreatePaymentRequest(
            bookingId,
            "https://movie88aspnet-app.up.railway.app/api/payments/vnpay/callback"
        );
        
        apiService.createVNPayPayment("Bearer " + jwtToken, request)
            .enqueue(new Callback<ApiResponse<CreatePaymentResponse>>() {
                @Override
                public void onResponse(Call<ApiResponse<CreatePaymentResponse>> call, 
                                     Response<ApiResponse<CreatePaymentResponse>> response) {
                    progressDialog.dismiss();
                    
                    if (response.isSuccessful() && response.body() != null) {
                        ApiResponse<CreatePaymentResponse> apiResponse = response.body();
                        
                        if (apiResponse.isSuccess() && apiResponse.getData() != null) {
                            String vnpayUrl = apiResponse.getData().getVnpayUrl();
                            int paymentId = apiResponse.getData().getPaymentid();
                            
                            // Save payment ID for later verification
                            savePaymentId(paymentId);
                            
                            // Open VNPay URL in Chrome Custom Tab
                            openVNPayPayment(vnpayUrl);
                        } else {
                            showError(apiResponse.getMessage());
                        }
                    } else {
                        showError("Failed to create payment");
                    }
                }
                
                @Override
                public void onFailure(Call<ApiResponse<CreatePaymentResponse>> call, Throwable t) {
                    progressDialog.dismiss();
                    showError("Network error: " + t.getMessage());
                }
            });
    }
    
    private void openVNPayPayment(String vnpayUrl) {
        // Use Chrome Custom Tabs for better UX
        CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
        builder.setToolbarColor(getResources().getColor(R.color.primary));
        builder.setShowTitle(true);
        
        CustomTabsIntent customTabsIntent = builder.build();
        customTabsIntent.launchUrl(this, Uri.parse(vnpayUrl));
    }
    
    private void savePaymentId(int paymentId) {
        SharedPreferences prefs = getSharedPreferences("PaymentPrefs", MODE_PRIVATE);
        prefs.edit().putInt("PENDING_PAYMENT_ID", paymentId).apply();
    }
    
    private void showError(String message) {
        Toast.makeText(this, message, Toast.LENGTH_LONG).show();
    }
}
```

#### 8. Payment Result Activity (Handle Deep Link)

```java
// PaymentResultActivity.java
public class PaymentResultActivity extends AppCompatActivity {
    
    private ImageView ivResultIcon;
    private TextView tvResultTitle, tvResultMessage, tvBookingCode, tvTransactionCode;
    private Button btnViewBooking, btnBackHome;
    
    private String jwtToken;
    private int paymentId;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_payment_result);
        
        initViews();
        handleDeepLink();
    }
    
    private void initViews() {
        ivResultIcon = findViewById(R.id.ivResultIcon);
        tvResultTitle = findViewById(R.id.tvResultTitle);
        tvResultMessage = findViewById(R.id.tvResultMessage);
        tvBookingCode = findViewById(R.id.tvBookingCode);
        tvTransactionCode = findViewById(R.id.tvTransactionCode);
        btnViewBooking = findViewById(R.id.btnViewBooking);
        btnBackHome = findViewById(R.id.btnBackHome);
        
        btnViewBooking.setOnClickListener(v -> viewBookingDetails());
        btnBackHome.setOnClickListener(v -> goHome());
    }
    
    private void handleDeepLink() {
        Intent intent = getIntent();
        Uri data = intent.getData();
        
        if (data != null && "movieapp".equals(data.getScheme())) {
            // Deep link from API callback redirect
            // Format: movieapp://payment/result?bookingId=156&status=success
            String bookingIdParam = data.getQueryParameter("bookingId");
            String status = data.getQueryParameter("status");
            
            if (bookingIdParam != null && status != null) {
                // Get stored payment ID
                SharedPreferences prefs = getSharedPreferences("PaymentPrefs", MODE_PRIVATE);
                paymentId = prefs.getInt("PENDING_PAYMENT_ID", 0);
                
                if ("success".equals(status)) {
                    // Payment successful (already validated by API server)
                    showSuccessResult(null);
                    // Get payment details and BookingCode from API
                    verifyPaymentStatus();
                } else {
                    // Payment failed
                    showFailureResult("99");
                }
                return;
            }
        }
        
        // Direct access (not from deep link)
        Toast.makeText(this, "Invalid access", Toast.LENGTH_SHORT).show();
        finish();
    }
    
    private void showSuccessResult(String txnRef) {
        ivResultIcon.setImageResource(R.drawable.ic_success);
        tvResultTitle.setText("Payment Successful!");
        tvResultMessage.setText("Your booking has been confirmed");
        tvTransactionCode.setText("Transaction: " + txnRef);
    }
    
    private void showFailureResult(String responseCode) {
        ivResultIcon.setImageResource(R.drawable.ic_failed);
        tvResultTitle.setText("Payment Failed");
        tvResultMessage.setText(getVNPayErrorMessage(responseCode));
        tvBookingCode.setVisibility(View.GONE);
        tvTransactionCode.setVisibility(View.GONE);
        btnViewBooking.setVisibility(View.GONE);
    }
    
    private void verifyPaymentStatus() {
        // Get JWT token
        SharedPreferences prefs = getSharedPreferences("AuthPrefs", MODE_PRIVATE);
        jwtToken = prefs.getString("JWT_TOKEN", "");
        
        // Call API to get payment details
        Retrofit retrofit = new Retrofit.Builder()
            .baseUrl("https://movie88aspnet-app.up.railway.app/api/")
            .addConverterFactory(GsonConverterFactory.create())
            .build();
            
        PaymentApiService apiService = retrofit.create(PaymentApiService.class);
        
        apiService.getPaymentDetails("Bearer " + jwtToken, paymentId)
            .enqueue(new Callback<ApiResponse<PaymentDetailDTO>>() {
                @Override
                public void onResponse(Call<ApiResponse<PaymentDetailDTO>> call, 
                                     Response<ApiResponse<PaymentDetailDTO>> response) {
                    if (response.isSuccessful() && response.body() != null) {
                        ApiResponse<PaymentDetailDTO> apiResponse = response.body();
                        
                        if (apiResponse.isSuccess() && apiResponse.getData() != null) {
                            PaymentDetailDTO payment = apiResponse.getData();
                            
                            // Update UI with booking code
                            if (payment.getBooking() != null && 
                                payment.getBooking().getBookingcode() != null) {
                                tvBookingCode.setText("Booking Code: " + 
                                    payment.getBooking().getBookingcode());
                            }
                        }
                    }
                }
                
                @Override
                public void onFailure(Call<ApiResponse<PaymentDetailDTO>> call, Throwable t) {
                    // Handle error silently, deep link params are enough
                }
            });
    }
    
    private String getVNPayErrorMessage(String responseCode) {
        switch (responseCode) {
            case "07": return "Transaction successful. Suspicious transaction (related to fraud, unusual transaction)";
            case "09": return "Transaction failed: Card not yet registered for InternetBanking at the bank";
            case "10": return "Transaction failed: Customer entered incorrect card/account information more than 3 times";
            case "11": return "Transaction failed: Payment deadline has expired. Please try again";
            case "12": return "Transaction failed: Card is locked";
            case "13": return "Transaction failed: Incorrect transaction authentication password (OTP)";
            case "24": return "Transaction failed: Customer cancelled transaction";
            case "51": return "Transaction failed: Your account balance is insufficient";
            case "65": return "Transaction failed: Your account has exceeded the daily transaction limit";
            case "75": return "Payment bank is under maintenance";
            case "79": return "Transaction failed: Incorrect payment password more than specified number of times";
            case "99": return "Other errors";
            default: return "Transaction failed with code: " + responseCode;
        }
    }
    
    private void viewBookingDetails() {
        // Navigate to booking details screen
        Intent intent = new Intent(this, BookingDetailsActivity.class);
        // Get bookingId from payment details
        intent.putExtra("BOOKING_ID", /* bookingId from payment */);
        startActivity(intent);
        finish();
    }
    
    private void goHome() {
        Intent intent = new Intent(this, HomeActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK);
        startActivity(intent);
        finish();
    }
}
```

### 🔐 Important Notes for Android

#### Payment Flow with API Callback (Current Setup)

**Complete Flow:**
```
1. User clicks "Pay Now" in BookingSummaryActivity
2. App calls POST /api/payments/vnpay/create with Railway callback URL
3. API returns VNPay payment URL
4. App opens VNPay URL in Chrome Custom Tab
5. User completes payment with test card
6. VNPay redirects to: https://movie88aspnet-app.up.railway.app/api/payments/vnpay/callback?vnp_...
7. API receives callback:
   - Validates VNPay signature
   - Updates payment status to "Completed"
   - Updates booking status to "Confirmed"
   - Generates BookingCode: "BK-YYYYMMDD-XXXX"
8. API returns HTML with meta refresh redirect:
   <meta http-equiv="refresh" content="0;url=movieapp://payment/result?bookingId=156&status=success">
9. Browser/Chrome Custom Tab triggers deep link
10. Android OS opens PaymentResultActivity
11. App receives deep link, extracts bookingId
12. App calls GET /api/payments/{id} to verify and get BookingCode
13. Show success screen with BookingCode
```

**Why this approach?**
- ✅ VNPay already registered with `movie88.com`
- ✅ API validates payment BEFORE app receives result
- ✅ Database updated atomically
- ✅ More secure (server-side validation)
- ✅ Works with current VNPay sandbox configuration

#### Deep Link Testing

1. **Test with ADB command:**
   ```bash
   adb shell am start -W -a android.intent.action.VIEW -d "movieapp://payment/result?bookingId=156&status=success"
   ```

2. **Test complete flow:**
   - Click "Pay Now" → Opens VNPay in Chrome Custom Tab
   - Complete payment with test card (9704198526191432198, OTP: 123456)
   - VNPay redirects to Railway API callback
   - API processes payment and returns HTML redirect
   - Android opens PaymentResultActivity via deep link
   - App calls API to verify payment and get BookingCode

#### ReturnUrl Configuration

**✅ USE API CALLBACK URL**

For Android app, always use the API callback URL as ReturnUrl:

```java
// Production (Railway)
CreatePaymentRequest request = new CreatePaymentRequest(
    bookingId,
    "https://movie88aspnet-app.up.railway.app/api/payments/vnpay/callback"
);

// OR for localhost testing:
CreatePaymentRequest request = new CreatePaymentRequest(
    bookingId,
    "https://localhost:7238/api/payments/vnpay/callback"
);
```

**Why this approach?**: 
- ✅ Already registered with VNPay sandbox (`movie88.com`)
- ✅ Works immediately without additional VNPay configuration
- ✅ API validates payment signature before app receives result
- ✅ Database updated atomically on server
- ✅ More secure (server-side validation first)
- ✅ API returns HTML that triggers deep link to open app
- ✅ User doesn't see intermediate page (instant redirect)

**Current VNPay Registration:**
- Registered domain: `movie88.com`
- TMN Code: `1F8WTZLN`
- API ReturnUrl: `https://movie88aspnet-app.up.railway.app/api/payments/vnpay/callback`

**Recommendation for Android:**
Use **Option 1** (API callback URL). Your app will:
1. Create payment with Railway URL
2. Open VNPay in Chrome Custom Tab
3. User completes payment
4. VNPay redirects to API callback
5. API processes payment → updates database → generates BookingCode
6. API returns HTML with meta refresh redirect to app deep link
7. App receives deep link and shows result

**API Callback Response (PaymentsController.VNPayCallback):**
```csharp
// Success case
return Content($@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='refresh' content='0;url=movieapp://payment/result?bookingId={bookingId}&status=success'>
    <title>Payment Successful</title>
</head>
<body>
    <p>Payment successful! Redirecting to app...</p>
</body>
</html>
", "text/html");

// Failure case
return Content($@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='refresh' content='0;url=movieapp://payment/result?bookingId={bookingId}&status=failed'>
    <title>Payment Failed</title>
</head>
<body>
    <p>Payment failed. Redirecting to app...</p>
</body>
</html>
", "text/html");
```

This HTML triggers the deep link automatically when Chrome Custom Tab loads it.

#### Security Best Practices

1. **Always verify payment status via API** after receiving deep link callback
2. **Don't trust only query parameters** from VNPay redirect
3. **Store JWT token securely** in EncryptedSharedPreferences
4. **Validate BookingCode** exists before showing QR code
5. **Handle all VNPay response codes** (00, 07, 09, 10, 11, 12, 13, 24, 51, 65, 75, 79, 99)

#### Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| Deep link not working | Check AndroidManifest.xml intent-filter configuration |
| App not opening after payment | Verify scheme matches exactly: `movieapp://` |
| Chrome Custom Tab not closing | Use `android:launchMode="singleTask"` for PaymentResultActivity |
| Payment status not updated | Call `getPaymentDetails` API to verify actual status |
| Token expired during payment | Refresh token before creating payment |

---

**Created**: November 3, 2025  
**Last Updated**: November 5, 2025  
**Progress**: ✅ 8/8 endpoints (100%) - All phases complete  
**Test File**: `tests/Payment.http` ✅  
**Android Guide**: ✅ Complete with Java + XML examples
