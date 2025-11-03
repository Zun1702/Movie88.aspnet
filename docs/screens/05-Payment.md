# 💳 Screen 5: Payment & Vouchers (8 Endpoints)

**Status**: 🔄 **PENDING** (0/8 endpoints - 0%)

---

## 📋 Endpoints Overview

| # | Method | Endpoint | Screen | Auth | Status |
|---|--------|----------|--------|------|--------|
| 1 | GET | `/api/bookings/{id}` | BookingSummaryActivity | ✅ | ✅ DONE (Screen 2) |
| 2 | POST | `/api/vouchers/validate` | BookingSummaryActivity | ✅ | ❌ TODO |
| 3 | POST | `/api/bookings/{id}/apply-voucher` | BookingSummaryActivity | ✅ | ❌ TODO |
| 4 | POST | `/api/payments/vnpay/create` | BookingSummaryActivity | ✅ | ❌ TODO |
| 5 | GET | `/api/payments/vnpay/callback` | VNPayWebViewActivity | ❌ | ❌ TODO |
| 6 | POST | `/api/payments/vnpay/ipn` | PaymentResultActivity | ❌ | ❌ TODO |
| 7 | PUT | `/api/payments/{id}/confirm` | PaymentResultActivity | ✅ | ❌ TODO |
| 8 | GET | `/api/bookings/{id}` | PaymentResultActivity | ✅ | ✅ DONE (Screen 2) |

---

## 🎯 1. GET /api/bookings/{id}

**Screen**: BookingSummaryActivity, PaymentResultActivity  
**Auth Required**: ✅ Yes

### Status
✅ **ALREADY IMPLEMENTED** (see Screen 02-Home-MainScreens.md)

### Response
Returns full booking details including movie, cinema, showtime, seats, combos, voucher, payment info.

---

## 🎯 2. POST /api/vouchers/validate

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

## 🎯 3. POST /api/bookings/{id}/apply-voucher

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

## 🎯 4. POST /api/payments/vnpay/create

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
- ✅ `methodname` (string, max 50) - "VNPay", "MoMo", "Cash", etc.

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
       .FirstOrDefaultAsync(pm => pm.Methodname == "VNPay");
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
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "ReturnUrl": "https://your-api.com/api/payments/vnpay/callback"
  }
}
```

### Error Cases
- 403 Forbidden - Booking doesn't belong to user
- 400 Bad Request - Booking status not "Pending"
- 409 Conflict - Payment already exists for booking

---

## 🎯 5. GET /api/payments/vnpay/callback

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

3. **Update Payment Status**:
   ```csharp
   var payment = await _context.Payments
       .FirstOrDefaultAsync(p => p.Transactioncode == vnp_TxnRef);
   
   if (vnp_ResponseCode == "00")
   {
       payment.Status = "Completed";
       payment.Paymenttime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
       
       // Update booking status
       var booking = await _context.Bookings.FindAsync(payment.Bookingid);
       booking.Status = "Confirmed";
   }
   else
   {
       payment.Status = "Failed";
   }
   
   await _context.SaveChangesAsync();
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

## 🎯 6. POST /api/payments/vnpay/ipn

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

2. **Update Payment Status** (same as callback)

3. **Return Confirmation to VNPay**:
   - `RspCode: "00"` = Success
   - `RspCode: "99"` = Error

### IPN (Instant Payment Notification)
- VNPay calls this endpoint to notify server of payment result
- Must respond within 30 seconds
- VNPay will retry if no response
- Used for server-side confirmation, separate from user-facing callback

---

## 🎯 7. PUT /api/payments/{id}/confirm

**Screen**: PaymentResultActivity  
**Auth Required**: ✅ Yes

### Request Body (Optional)
```json
{
  "status": "Completed"
}
```

### Response 200 OK
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Payment confirmed successfully",
  "data": {
    "paymentid": 22,
    "bookingid": 156,
    "amount": 207500,
    "status": "Completed",
    "methodname": "VNPay",
    "transactioncode": "PAY_20251103154500_156",
    "paymenttime": "2025-11-03T15:45:30"
  }
}
```

### Business Logic
1. **Validate Payment**:
   - Get userId from JWT
   - Find Customer by userid
   - Find Payment by paymentid
   - Verify payment's booking belongs to customer

2. **Update Payment**:
   - Set status to "Completed"
   - Set paymenttime to current timestamp

3. **Update Booking**:
   - Set booking status to "Confirmed"

### Error Cases
- 403 Forbidden - Payment doesn't belong to user
- 400 Bad Request - Payment already confirmed
- 404 Not Found - Payment not found

---

## 📊 Implementation Summary

### To Be Created

#### Domain Layer (Movie88.Domain/Models/)
```
❌ PaymentModel.cs         - Payment entity mapping
❌ VoucherModel.cs         - Voucher entity mapping
❌ PaymentmethodModel.cs   - Payment method entity mapping
```

#### Application Layer (Movie88.Application/)
```
❌ DTOs/Vouchers/
   - ValidateVoucherRequestDTO.cs
   - ValidateVoucherResponseDTO.cs
   - ApplyVoucherRequestDTO.cs

❌ DTOs/Payments/
   - CreatePaymentRequestDTO.cs
   - CreatePaymentResponseDTO.cs
   - VNPayCallbackDTO.cs
   - ConfirmPaymentRequestDTO.cs

❌ Services/
   - IVoucherService.cs / VoucherService.cs
   - IPaymentService.cs / PaymentService.cs
   - IVNPayService.cs / VNPayService.cs (for URL generation, hash validation)
```

#### Infrastructure Layer (Movie88.Infrastructure/)
```
❌ Repositories/
   - IVoucherRepository.cs / VoucherRepository.cs
   - IPaymentRepository.cs / PaymentRepository.cs
   - IPaymentmethodRepository.cs / PaymentmethodRepository.cs
```

#### WebApi Layer (Movie88.WebApi/)
```
❌ Controllers/
   - VouchersController.cs (1 endpoint)
   - PaymentsController.cs (4 endpoints)
   
❌ appsettings.json (add VNPay configuration)
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
- ⚠️ `paymenttime` is timestamp without time zone
- ⚠️ Status: "Pending", "Completed", "Failed", "Cancelled"
- ❌ NO `vnpaydata` JSONB field

**Booking Entity**:
- ⚠️ `voucherid` is nullable FK
- ⚠️ `totalamount` updated when voucher applied

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
private string GenerateTransactionCode()
{
    return $"PAY_{DateTime.Now:yyyyMMddHHmmss}_{bookingId}";
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

**Payment Transaction**:
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Update payment
    payment.Status = "Completed";
    payment.Paymenttime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
    
    // Update booking
    booking.Status = "Confirmed";
    
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

### PostgreSQL Specific
- DateOnly for expirydate
- timestamp without time zone for paymenttime
- Use transactions for payment confirmation

---

## 🧪 Testing Checklist

### POST /api/vouchers/validate
- [ ] Validate expired vouchers
- [ ] Validate usage limit exceeded
- [ ] Validate minimum purchase amount
- [ ] Calculate discount correctly (percentage vs fixed)
- [ ] Return 403 for wrong user

### POST /api/bookings/{id}/apply-voucher
- [ ] Apply voucher correctly
- [ ] Update totalamount
- [ ] Increment usedcount
- [ ] Prevent double application
- [ ] Use transaction

### POST /api/payments/vnpay/create
- [ ] Create payment record
- [ ] Generate valid VNPay URL
- [ ] Generate secure hash correctly
- [ ] Handle VND amount conversion (×100)
- [ ] Return correct transaction code

### GET /api/payments/vnpay/callback
- [ ] Validate secure hash
- [ ] Parse response code correctly
- [ ] Update payment status
- [ ] Update booking status
- [ ] Redirect to app with deep link
- [ ] Handle all VNPay response codes

### POST /api/payments/vnpay/ipn
- [ ] Validate secure hash
- [ ] Update payment status
- [ ] Respond within 30 seconds
- [ ] Return correct RspCode to VNPay

### PUT /api/payments/{id}/confirm
- [ ] Verify payment ownership
- [ ] Update payment and booking status
- [ ] Use transaction
- [ ] Prevent double confirmation

---

**Created**: November 3, 2025  
**Last Updated**: November 3, 2025  
**Progress**: ✅ 2/8 endpoints (25%) - 2 booking detail endpoints reused
