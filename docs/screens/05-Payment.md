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

### 📧 Phase 4: Email Confirmation & QR Code ✅ DONE
| # | Feature | Trigger | Purpose | Status | Assign |
|---|---------|---------|---------|--------|--------|
| 9 | Booking Confirmation Email | After VNPay success | Send invoice with QR code (Vietnamese) | ✅ DONE | Trung |
| 10 | QR Code Generation | On payment confirmed | Generate QR from BookingCode (M88-XXXXXXXX) | ✅ DONE | Trung |

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

## 📧 PHASE 4: EMAIL CONFIRMATION & QR CODE ✅ DONE

### 🎯 9. Booking Confirmation Email (Auto-triggered)

**Status**: ✅ **DONE**  
**Trigger**: Automatically sent after VNPay payment success (ResponseCode = "00")  
**Sent From**: `Movie88 <movie88@ezyfix.site>` (via Resend API)  
**Sent To**: Customer's email from `User` table (e.g., ngoctrungtsn111@gmail.com)  
**Auth Required**: N/A (Internal background service)  
**Language**: **Vietnamese** 🇻🇳

### Technical Implementation

**Background Task Pattern**:
```csharp
// In PaymentService.ProcessVNPayCallbackAsync()
if (responseCode == "00" && !string.IsNullOrEmpty(bookingCode))
{
    // ✅ Create new scope to avoid DbContext disposed issue
    _ = Task.Run(async () =>
    {
        using var scope = _serviceProvider.CreateScope();
        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        var qrCodeService = scope.ServiceProvider.GetRequiredService<IQRCodeService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        
        await SendBookingConfirmationEmailAsync(
            bookingId, bookingCode, transactionCode,
            bookingRepository, qrCodeService, emailService);
    });
}
```

**Why New Scope?**
- Callback returns immediately → ASP.NET Core disposes `AppDbContext`
- Background task needs fresh DbContext to query booking details
- `IServiceProvider.CreateScope()` creates isolated DI scope with new context

### Email Content Structure

#### Email Subject
```
🎬 Xác Nhận Đặt Vé - [Tên Phim] - Movie88
Example: 🎬 Xác Nhận Đặt Vé - Avengers: Endgame - Movie88
```

#### Email Template (HTML) - Vietnamese Version

**Design**: Netflix-inspired with dark theme  
**Colors**:
- Primary Red: `#E50914` (Netflix red)
- Background: `#141414` (Dark black)
- Card Background: `#1F1F1F` (Dark gray)
- Secondary Text: `#B3B3B3` (Light gray)
- Accent: `#FFB800` (Gold for important info)

**Sections**:

1. **Header Section** 🎬
   - Movie88 logo with red gradient: `linear-gradient(135deg, #E50914 0%, #831010 100%)`
   - "Xác Nhận Đặt Vé" title (white)
   - Dark background with subtle pattern

2. **Booking Code Card** 🎫
   - Large BookingCode: `M88-00000123` (format: M88-{BookingId:D8})
   - White text on dark card with red border
   - Center aligned for easy screenshot

3. **QR Code Section** 📱
   - QR code image (300x300px) encoding BookingCode
   - Vietnamese text: "Mã QR Của Bạn"
   - Instruction: "Vui lòng xuất trình mã này tại rạp"
   - Note: "Chụp màn hình để sử dụng offline"
   - Attached as inline image with `content_id: "qrcode"`

4. **Booking Details Section** 📋
   - **Thông Tin Đặt Vé**:
     - 🎬 Phim: [Movie Title]
     - 🏢 Rạp Chiếu: [Cinema Name]
     - 📍 Địa Chỉ: [Cinema Address]
     - 📅 Ngày & Giờ Chiếu: [dd/MM/yyyy HH:mm]
     - 🪑 Ghế Ngồi: [A5, A6, B3]
   - Dark cards with white text
   - Icons for visual clarity

5. **Combo Section** 🍿 (if applicable)
   - **Combo Đồ Ăn**:
     - Combo item name × quantity (price formatted: 50.000 VND)
     - Total combo price
   - Only shown if booking has combos

6. **Payment Summary** 💰
   - **Thông Tin Thanh Toán**:
     - Giá Vé: [ticketPrice] VND
     - Tổng Combo: [comboPrice] VND (if applicable)
     - Giảm Giá: -[discountAmount] VND (if voucher applied, shown in red)
     - **Tổng Thanh Toán**: [totalAmount] VND (bold, large font)
     - Phương Thức: VNPay
     - Mã Giao Dịch: [PAY_YYYYMMDDHHMMSS_123]
     - Thời Gian Thanh Toán: [dd/MM/yyyy HH:mm:ss]
   - Currency formatted with thousand separators

7. **Important Information** ⚠️
   - **Lưu Ý Quan Trọng** (gold color):
     - ⏰ Vui lòng đến rạp trước 15 phút
     - 🎫 Xuất trình mã QR hoặc Booking Code tại quầy
     - 🎬 Không hoàn tiền sau 24 giờ trước giờ chiếu
     - 📞 Liên hệ: support@movie88.com
   - Warning style with gold accent

8. **Footer Section** 🎭
   - "Cảm ơn bạn đã chọn Movie88!" (Thank you message)
   - Movie88 branding (red logo)
   - "Email tự động, vui lòng không trả lời"
   - Social media links (optional)
   - Dark footer with subtle border

### Business Logic Flow

**Step-by-Step Execution**:

1. **VNPay Callback Received** (GET /api/payments/vnpay/callback)
   - Validate secure hash ✅
   - Check ResponseCode = "00" (success)
   - Update Payment.Status = "Completed"
   - Update Booking.Status = "Confirmed"
   - Generate BookingCode: `M88-{BookingId:D8}`
   - Save to database

2. **Start Background Email Task** (Non-blocking)
   ```csharp
   if (responseCode == "00" && !string.IsNullOrEmpty(bookingCode))
   {
       _ = Task.Run(async () =>
       {
           // ✅ Create new DI scope to get fresh DbContext
           using var scope = _serviceProvider.CreateScope();
           var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
           var qrCodeService = scope.ServiceProvider.GetRequiredService<IQRCodeService>();
           var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
           
           await SendBookingConfirmationEmailAsync(...);
       });
   }
   ```

3. **Fetch Booking Details** (with fresh DbContext)
   ```csharp
   var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
   // Includes:
   // - Customer.User (for email and fullname)
   // - Showtime.Movie (for title)
   // - Showtime.Auditorium.Cinema (for name and address)
   // - BookingSeats.Seat (for seat numbers)
   // - BookingCombos.Combo (for combo items)
   // - Voucher (for discount info)
   // - Payments (for transaction details)
   ```

4. **Extract Customer Email**
   ```csharp
   var customerEmail = booking.Customer?.User?.Email ?? booking.Customer?.Email ?? "";
   var customerName = booking.Customer?.User?.Fullname ?? booking.Customer?.Fullname ?? "Khách Hàng";
   
   if (string.IsNullOrEmpty(customerEmail))
   {
       _logger.LogError("Customer email not found, skipping email");
       return; // Skip if no email
   }
   ```

5. **Generate QR Code**
   ```csharp
   var qrCodeBase64 = await qrCodeService.GenerateQRCodeBase64Async(bookingCode);
   // Returns: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA..."
   ```

6. **Calculate Discount** (if voucher applied)
   ```csharp
   decimal discountAmount = 0;
   if (booking.Voucherid.HasValue && booking.Voucher != null)
   {
       if (booking.Voucher.Discounttype == "percentage")
       {
           var totalAmount = booking.Totalamount ?? 0;
           var originalAmount = totalAmount / (1 - booking.Voucher.Discountvalue.Value / 100);
           discountAmount = originalAmount - totalAmount;
       }
       else // fixed
       {
           discountAmount = booking.Voucher.Discountvalue.Value;
       }
   }
   ```

7. **Build Email DTO**
   ```csharp
   var emailDto = new BookingConfirmationEmailDTO
   {
       CustomerEmail = customerEmail,
       CustomerName = customerName,
       BookingCode = bookingCode, // M88-00000123
       QRCodeBase64 = qrCodeBase64,
       MovieTitle = booking.Showtime?.Movie?.Title ?? "Movie",
       CinemaName = booking.Showtime?.Auditorium?.Cinema?.Name ?? "Cinema",
       CinemaAddress = booking.Showtime?.Auditorium?.Cinema?.Address ?? "",
       ShowtimeDateTime = booking.Showtime?.Starttime ?? DateTime.Now,
       SeatNumbers = "A5, A6, B3", // Extracted from BookingSeats
       ComboItems = comboItemsList, // List<ComboItemDTO>
       TotalAmount = booking.Totalamount ?? 0,
       DiscountAmount = discountAmount,
       VoucherCode = booking.Voucher?.Code,
       TransactionCode = transactionCode, // PAY_20251105154500_123
       PaymentTime = DateTime.Now
   };
   ```

8. **Send Email via Resend API**
   ```csharp
   var emailSent = await emailService.SendBookingConfirmationAsync(emailDto);
   // POST https://api.resend.com/emails
   // Authorization: Bearer {API_KEY}
   // Body: { from, to, subject, html, attachments }
   ```

9. **Log Results**
   ```csharp
   if (emailSent)
   {
       _logger.LogInformation("✅ Email sent to {Email} for {BookingCode}", 
           customerEmail, bookingCode);
   }
   else
   {
       _logger.LogError("❌ Failed to send email to {Email}", customerEmail);
   }
   ```

### Email Template Example

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Booking Confirmation - Movie88</title>
</head>
<body style="margin: 0; padding: 0; font-family: 'Segoe UI', Arial, sans-serif; background-color: #f5f5f5;">
    
    <!-- Header with Gradient -->
    <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 20px; text-align: center;">
        <h1 style="color: white; margin: 0; font-size: 32px; font-weight: bold;">🎬 Movie88</h1>
        <p style="color: rgba(255,255,255,0.9); margin: 10px 0 0 0; font-size: 18px;">Booking Confirmation</p>
    </div>
    
    <!-- Main Content -->
    <div style="max-width: 600px; margin: 0 auto; background-color: white; border-radius: 8px; margin-top: -20px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);">
        
        <!-- BookingCode Section -->
        <div style="padding: 30px 20px; text-align: center; border-bottom: 2px dashed #e0e0e0;">
            <h2 style="color: #333; margin: 0 0 10px 0;">Booking Code</h2>
            <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px; border-radius: 8px; font-size: 28px; font-weight: bold; letter-spacing: 2px;">
                {{BOOKING_CODE}}
            </div>
        </div>
        
        <!-- QR Code Section -->
        <div style="padding: 30px 20px; text-align: center; background-color: #f9f9f9;">
            <h3 style="color: #333; margin: 0 0 20px 0;">Your QR Code</h3>
            <img src="cid:qrcode" alt="Booking QR Code" style="width: 300px; height: 300px; border: 4px solid #667eea; border-radius: 8px; padding: 10px; background: white;">
            <p style="color: #666; margin: 20px 0 0 0; font-size: 14px;">
                📱 Show this QR code at cinema entrance<br>
                💾 Screenshot and save for offline access
            </p>
        </div>
        
        <!-- Booking Details -->
        <div style="padding: 30px 20px;">
            <h3 style="color: #333; margin: 0 0 20px 0; border-bottom: 2px solid #667eea; padding-bottom: 10px;">Booking Details</h3>
            
            <table style="width: 100%; border-collapse: collapse;">
                <tr>
                    <td style="padding: 10px 0; color: #666; width: 30%;">🎬 Movie</td>
                    <td style="padding: 10px 0; color: #333; font-weight: bold;">{{MOVIE_TITLE}}</td>
                </tr>
                <tr>
                    <td style="padding: 10px 0; color: #666;">🏢 Cinema</td>
                    <td style="padding: 10px 0; color: #333;">{{CINEMA_NAME}}</td>
                </tr>
                <tr>
                    <td style="padding: 10px 0; color: #666;">📅 Date & Time</td>
                    <td style="padding: 10px 0; color: #333;">{{SHOWTIME_DATETIME}}</td>
                </tr>
                <tr>
                    <td style="padding: 10px 0; color: #666;">🪑 Seats</td>
                    <td style="padding: 10px 0; color: #333; font-weight: bold;">{{SEAT_NUMBERS}}</td>
                </tr>
                {{#if COMBO_ITEMS}}
                <tr>
                    <td style="padding: 10px 0; color: #666;">🍿 Combos</td>
                    <td style="padding: 10px 0; color: #333;">{{COMBO_ITEMS}}</td>
                </tr>
                {{/if}}
            </table>
        </div>
        
        <!-- Payment Summary -->
        <div style="padding: 30px 20px; background-color: #f9f9f9;">
            <h3 style="color: #333; margin: 0 0 20px 0; border-bottom: 2px solid #667eea; padding-bottom: 10px;">Payment Summary</h3>
            
            <table style="width: 100%; border-collapse: collapse;">
                <tr>
                    <td style="padding: 8px 0; color: #666;">Ticket Price</td>
                    <td style="padding: 8px 0; color: #333; text-align: right;">{{TICKET_PRICE}} VND</td>
                </tr>
                {{#if COMBO_PRICE}}
                <tr>
                    <td style="padding: 8px 0; color: #666;">Combo</td>
                    <td style="padding: 8px 0; color: #333; text-align: right;">{{COMBO_PRICE}} VND</td>
                </tr>
                {{/if}}
                {{#if DISCOUNT_AMOUNT}}
                <tr>
                    <td style="padding: 8px 0; color: #e74c3c;">Discount ({{VOUCHER_CODE}})</td>
                    <td style="padding: 8px 0; color: #e74c3c; text-align: right;">-{{DISCOUNT_AMOUNT}} VND</td>
                </tr>
                {{/if}}
                <tr style="border-top: 2px solid #ddd;">
                    <td style="padding: 15px 0 0 0; color: #333; font-size: 18px; font-weight: bold;">Total Paid</td>
                    <td style="padding: 15px 0 0 0; color: #667eea; font-size: 20px; font-weight: bold; text-align: right;">{{TOTAL_AMOUNT}} VND</td>
                </tr>
                <tr>
                    <td style="padding: 8px 0; color: #999; font-size: 12px;">Payment Method</td>
                    <td style="padding: 8px 0; color: #999; font-size: 12px; text-align: right;">VNPay - {{TRANSACTION_CODE}}</td>
                </tr>
                <tr>
                    <td style="padding: 8px 0; color: #999; font-size: 12px;">Payment Time</td>
                    <td style="padding: 8px 0; color: #999; font-size: 12px; text-align: right;">{{PAYMENT_TIME}}</td>
                </tr>
            </table>
        </div>
        
        <!-- Important Information -->
        <div style="padding: 30px 20px; border-top: 2px dashed #e0e0e0;">
            <h3 style="color: #333; margin: 0 0 15px 0;">⚠️ Important Information</h3>
            <ul style="color: #666; line-height: 1.8; padding-left: 20px; margin: 0;">
                <li>Please arrive <strong>15 minutes before showtime</strong></li>
                <li>Present this <strong>QR code or Booking Code</strong> at cinema entrance</li>
                <li>No refund after <strong>24 hours before showtime</strong></li>
                <li>Contact support: <a href="mailto:support@movie88.com" style="color: #667eea;">support@movie88.com</a></li>
            </ul>
        </div>
        
        <!-- Call to Action -->
        <div style="padding: 30px 20px; text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
            <p style="color: white; margin: 0 0 15px 0; font-size: 16px;">Enjoy your movie! 🍿🎬</p>
            <a href="https://movie88.com/my-bookings" style="display: inline-block; background-color: white; color: #667eea; padding: 12px 30px; border-radius: 25px; text-decoration: none; font-weight: bold;">View My Bookings</a>
        </div>
    </div>
    
    <!-- Footer -->
    <div style="max-width: 600px; margin: 20px auto; padding: 20px; text-align: center; color: #999; font-size: 12px;">
        <p style="margin: 0 0 10px 0;">This is an automated email from Movie88. Please do not reply.</p>
        <p style="margin: 0;">© 2025 Movie88. All rights reserved.</p>
        <p style="margin: 10px 0 0 0;">
            <a href="#" style="color: #667eea; text-decoration: none; margin: 0 10px;">Facebook</a>
            <a href="#" style="color: #667eea; text-decoration: none; margin: 0 10px;">Instagram</a>
            <a href="#" style="color: #667eea; text-decoration: none; margin: 0 10px;">Twitter</a>
        </p>
    </div>
    
</body>
</html>
```

---

### 🎯 10. QR Code Generation Service

**Status**: ✅ **DONE**  
**Purpose**: Generate QR code from BookingCode for easy cinema check-in  
**Library**: QRCoder (NuGet package) v1.4.3  
**Format**: PNG image, Base64 encoded for email embedding  
**Used In**: Email confirmation, mobile app booking details

### QR Code Specifications

- **Content**: BookingCode in format `M88-{BookingId:D8}` (e.g., "M88-00000123")
- **Size**: 300x300 pixels (20 pixels per module, high resolution for scanning)
- **Error Correction**: Level Q (25% - good balance between size and error correction)
- **Format**: PNG image with white background
- **Encoding**: Base64 string with data URI scheme for inline embedding in HTML emails
- **Color**: Black modules on white background (standard for best scanner compatibility)

### Implementation

#### 1. QR Code Service Interface ✅ IMPLEMENTED

```csharp
// Movie88.Application/Interfaces/IQRCodeService.cs
public interface IQRCodeService
{
    /// <summary>
    /// Generate QR code as Base64 data URI string for email embedding
    /// Returns: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA..."
    /// </summary>
    Task<string> GenerateQRCodeBase64Async(string bookingCode);
    
    /// <summary>
    /// Generate QR code as raw byte array for API response
    /// Used by mobile app to download QR image directly
    /// </summary>
    Task<byte[]> GenerateQRCodeBytesAsync(string bookingCode);
}
```

#### 2. QR Code Service Implementation ✅ DONE

```csharp
// Movie88.Application/Services/QRCodeService.cs
using QRCoder;

public class QRCodeService : IQRCodeService
{
    private readonly ILogger<QRCodeService> _logger;
    
    public QRCodeService(ILogger<QRCodeService> logger)
    {
        _logger = logger;
    }
    
    public async Task<string> GenerateQRCodeBase64Async(string bookingCode)
    {
        try
        {
            return await Task.Run(() =>
            {
                // 1. Create QR generator
                using var qrGenerator = new QRCodeGenerator();
                
                // 2. Generate QR data with error correction level Q (25%)
                using var qrCodeData = qrGenerator.CreateQrCode(
                    bookingCode, 
                    QRCodeGenerator.ECCLevel.Q
                );
                
                // 3. Create PNG byte array QR code
                using var qrCode = new PngByteQRCode(qrCodeData);
                
                // 4. Get graphic with 20 pixels per module (300x300px for 15x15 modules)
                var qrCodeBytes = qrCode.GetGraphic(20);
                
                // 5. Convert to Base64 with data URI scheme for HTML embedding
                var base64String = Convert.ToBase64String(qrCodeBytes);
                return $"data:image/png;base64,{base64String}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR code for booking {BookingCode}", bookingCode);
            throw;
        }
    }
    
    public async Task<byte[]> GenerateQRCodeBytesAsync(string bookingCode)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(
                    bookingCode, 
                    QRCodeGenerator.ECCLevel.Q
                );
                using var qrCode = new PngByteQRCode(qrCodeData);
                
                return qrCode.GetGraphic(20);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR code bytes for booking {BookingCode}", bookingCode);
            throw;
        }
    }
}
```

**Why `Task.Run()`?**
- QRCoder library is synchronous
- Wrap in `Task.Run()` to avoid blocking async pipeline
- Maintains async/await pattern consistency

**Error Correction Level Q (25%)**:
- Balances QR code size vs damage tolerance
- Can recover if 25% of QR code is damaged/dirty
- Level Q is recommended for printing and email

#### 3. Service Registration ✅ DONE

```csharp
// Movie88.Application/Configuration/ServiceExtensions.cs
services.AddScoped<IQRCodeService, QRCodeService>();
services.AddScoped<IEmailService, ResendEmailService>();
services.AddScoped<IBookingCodeGenerator, BookingCodeGenerator>();
```

#### 4. Email Service with QR Code Attachment ✅ IMPLEMENTED

```csharp
// Movie88.Application/Services/ResendEmailService.cs
public async Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmailDTO dto)
{
    try
    {
        // 1. Generate Vietnamese email HTML from template
        var emailHtml = GenerateBookingConfirmationHtml(dto);
        
        // 2. Extract Base64 string from data URI (remove "data:image/png;base64," prefix)
        var base64Data = dto.QRCodeBase64;
        if (base64Data.StartsWith("data:image/png;base64,"))
        {
            base64Data = base64Data.Substring("data:image/png;base64,".Length);
        }
        
        // 3. Create Resend email request with inline QR code attachment
        var emailRequest = new
        {
            from = "Movie88 <movie88@ezyfix.site>",
            to = new[] { dto.CustomerEmail },
            subject = $"🎬 Xác Nhận Đặt Vé - {dto.MovieTitle} - Movie88",
            html = emailHtml,
            attachments = new[]
            {
                new
                {
                    content = base64Data, // Base64 string WITHOUT data URI prefix
                    filename = $"booking-qr-{dto.BookingCode}.png",
                    content_id = "qrcode" // For inline embedding with <img src="cid:qrcode">
                }
            }
        };
        
        // 4. Serialize and send via Resend API
        var jsonContent = JsonSerializer.Serialize(emailRequest, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("emails", httpContent);
        
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(
                "✅ Booking confirmation email sent successfully to {Email} for {BookingCode}. Response: {Response}", 
                dto.CustomerEmail, 
                dto.BookingCode,
                responseBody
            );
            return true;
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "❌ Failed to send booking confirmation email: {StatusCode} - {Error}", 
            response.StatusCode, 
            errorContent
        );
        return false;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ Exception sending booking confirmation email to {Email}", dto.CustomerEmail);
        return false;
    }
}
```

**Key Points**:
- **QR Code Attachment**: Sent as inline image with `content_id: "qrcode"`
- **HTML Embedding**: Use `<img src="cid:qrcode">` in email HTML to display QR inline
- **Base64 Encoding**: Strip `data:image/png;base64,` prefix before sending to Resend
- **Filename**: `booking-qr-M88-00000123.png` for easy identification
- **Non-blocking**: Runs in background Task.Run() to avoid blocking callback response

#### 5. Resend API Configuration ✅ SETUP

```json
// appsettings.json
{
  "Resend": {
    "ApiKey": "re_asyNFWRg_efTChvbEtP58HdCb7wfppYfP",
    "Endpoint": "https://api.resend.com"
  }
}
```

**Resend Email API Request**:
```http
POST https://api.resend.com/emails
Authorization: Bearer re_asyNFWRg_efTChvbEtP58HdCb7wfppYfP
Content-Type: application/json

{
  "from": "Movie88 <movie88@ezyfix.site>",
  "to": ["ngoctrungtsn111@gmail.com"],
  "subject": "🎬 Xác Nhận Đặt Vé - Avengers: Endgame - Movie88",
  "html": "<html>...</html>",
  "attachments": [
    {
      "content": "iVBORw0KGgoAAAANSUhEUgAA...",
      "filename": "booking-qr-M88-00000123.png",
      "content_id": "qrcode"
    }
  ]
}
```

**Resend Response (Success)**:
```json
{
  "id": "49a3999c-0ce1-4ea6-ab68-afcd6dc2e794",
  "from": "Movie88 <movie88@ezyfix.site>",
  "to": ["ngoctrungtsn111@gmail.com"],
  "created_at": "2025-11-05T10:30:45.123Z"
}
```

#### 6. Email HTML Template with Inline QR Code

```html
<!-- In email body -->
<div style="text-align: center; margin: 30px 0;">
    <h2 style="color: #E50914; margin-bottom: 20px;">Mã QR Của Bạn</h2>
    
    <!-- Inline QR Code using content_id -->
    <img src="cid:qrcode" 
         alt="QR Code" 
         style="width: 300px; height: 300px; border: 4px solid #E50914; border-radius: 8px; background: white; padding: 10px;" />
    
    <p style="color: #B3B3B3; margin-top: 15px; font-size: 14px;">
        📱 Vui lòng xuất trình mã này tại rạp
    </p>
    <p style="color: #FFB800; font-size: 13px; font-weight: bold;">
        💡 Chụp màn hình để sử dụng offline
    </p>
</div>
```

**Why `cid:qrcode`?**
- `cid` = Content-ID (inline attachment reference)
- Matches `content_id` in attachments array
- QR code embedded directly in email (not external link)
- Works in all email clients (Gmail, Outlook, Apple Mail)

#### 7. DTO for Email Data ✅ DONE

```csharp
// Movie88.Application/DTOs/Email/BookingConfirmationEmailDTO.cs
public class BookingConfirmationEmailDTO
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BookingCode { get; set; } = string.Empty;
    public string QRCodeBase64 { get; set; } = string.Empty;
    
    public string MovieTitle { get; set; } = string.Empty;
    public string CinemaName { get; set; } = string.Empty;
    public string CinemaAddress { get; set; } = string.Empty;
    public DateTime ShowtimeDateTime { get; set; }
    public string SeatNumbers { get; set; } = string.Empty;
    public List<ComboItemDTO> ComboItems { get; set; } = new();
    
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? VoucherCode { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public DateTime? PaymentTime { get; set; }
}

public class ComboItemDTO
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
```

```csharp
// Movie88.Application/Services/ResendEmailService.cs - ADD METHOD
public async Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmailDTO dto)
{
    try
    {
        // Generate email HTML from template
        var emailHtml = GenerateBookingConfirmationHtml(dto);
        
        // Convert Base64 QR code to bytes for attachment
        var qrCodeBytes = Convert.FromBase64String(dto.QRCodeBase64);
        
        // Create Resend email request with inline QR code
        var emailRequest = new
        {
            from = "Movie88 <movie88@ezyfix.site>",
            to = new[] { dto.CustomerEmail },
            subject = $"🎬 Booking Confirmed - {dto.MovieTitle} - Movie88",
            html = emailHtml,
            attachments = new[]
            {
                new
                {
                    content = dto.QRCodeBase64,
                    filename = $"booking-qr-{dto.BookingCode}.png",
                    content_id = "qrcode" // For inline embedding
                }
            }
        };
        
        var jsonContent = JsonSerializer.Serialize(emailRequest);
        var httpContent = new StringContent(
            jsonContent, 
            Encoding.UTF8, 
            "application/json"
        );
        
        var response = await _httpClient.PostAsync("emails", httpContent);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation(
                "Booking confirmation email sent to {Email} for booking {BookingCode}", 
                dto.CustomerEmail, 
                dto.BookingCode
            );
            return true;
        }
        
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError(
            "Failed to send booking confirmation email: {StatusCode} - {Error}", 
            response.StatusCode, 
            errorContent
        );
        return false;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error sending booking confirmation email to {Email}", dto.CustomerEmail);
        return false;
    }
}

private string GenerateBookingConfirmationHtml(BookingConfirmationEmailDTO dto)
{
    // Use the HTML template above, replace placeholders with actual data
    var html = GetBookingConfirmationTemplate();
    
    html = html.Replace("{{BOOKING_CODE}}", dto.BookingCode);
    html = html.Replace("{{CUSTOMER_NAME}}", dto.CustomerName);
    html = html.Replace("{{MOVIE_TITLE}}", dto.MovieTitle);
    html = html.Replace("{{CINEMA_NAME}}", dto.CinemaName);
    html = html.Replace("{{CINEMA_ADDRESS}}", dto.CinemaAddress);
    html = html.Replace("{{SHOWTIME_DATETIME}}", dto.ShowtimeDateTime.ToString("dddd, dd MMMM yyyy - HH:mm"));
    html = html.Replace("{{SEAT_NUMBERS}}", dto.SeatNumbers);
    
    // Combo items
    if (dto.ComboItems.Any())
    {
        var comboText = string.Join(", ", dto.ComboItems.Select(c => $"{c.Name} x{c.Quantity}"));
        html = html.Replace("{{COMBO_ITEMS}}", comboText);
    }
    else
    {
        html = html.Replace("{{#if COMBO_ITEMS}}.*?{{/if}}", "", RegexOptions.Singleline);
    }
    
    // Pricing
    var ticketPrice = dto.TotalAmount + dto.DiscountAmount;
    var comboPrice = dto.ComboItems.Sum(c => c.Price * c.Quantity);
    
    html = html.Replace("{{TICKET_PRICE}}", ticketPrice.ToString("N0"));
    html = html.Replace("{{COMBO_PRICE}}", comboPrice.ToString("N0"));
    html = html.Replace("{{DISCOUNT_AMOUNT}}", dto.DiscountAmount.ToString("N0"));
    html = html.Replace("{{VOUCHER_CODE}}", dto.VoucherCode ?? "");
    html = html.Replace("{{TOTAL_AMOUNT}}", dto.TotalAmount.ToString("N0"));
    html = html.Replace("{{TRANSACTION_CODE}}", dto.TransactionCode);
    html = html.Replace("{{PAYMENT_TIME}}", dto.PaymentTime?.ToString("dd/MM/yyyy HH:mm:ss") ?? "");
    
    return html;
}
```

#### 4. Update VNPay Callback to Send Email

```csharp
// Movie88.WebApi/Controllers/PaymentsController.cs - VNPayCallback method
[HttpGet("vnpay/callback")]
public async Task<IActionResult> VNPayCallback()
{
    // ... existing validation and payment update code ...
    
    if (vnp_ResponseCode == "00")
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update payment and generate BookingCode
                payment.Status = "Completed";
                payment.Paymenttime = DateTime.Now;
                
                var bookingTime = DateTime.Now;
                var bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
                
                var booking = payment.Booking;
                booking.Status = "Confirmed";
                booking.Bookingcode = bookingCode;
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                // ✨ NEW: Generate QR Code and send confirmation email
                try
                {
                    var qrCodeBase64 = await _qrCodeService.GenerateQRCodeBase64Async(bookingCode);
                    
                    // Get full booking details with includes
                    var fullBooking = await _context.Bookings
                        .Include(b => b.Customer)
                            .ThenInclude(c => c.User)
                        .Include(b => b.Tickets)
                            .ThenInclude(t => t.Seat)
                        .Include(b => b.Bookingcombos)
                            .ThenInclude(bc => bc.Combo)
                        .Include(b => b.Showtime)
                            .ThenInclude(s => s.Movie)
                        .Include(b => b.Showtime)
                            .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                        .Include(b => b.Voucher)
                        .FirstOrDefaultAsync(b => b.Bookingid == booking.Bookingid);
                    
                    var emailDto = new BookingConfirmationEmailDTO
                    {
                        CustomerEmail = fullBooking.Customer.User.Email,
                        CustomerName = fullBooking.Customer.User.Fullname,
                        BookingCode = bookingCode,
                        QRCodeBase64 = qrCodeBase64,
                        MovieTitle = fullBooking.Showtime.Movie.Title,
                        CinemaName = fullBooking.Showtime.Room.Cinema.Name,
                        CinemaAddress = fullBooking.Showtime.Room.Cinema.Address,
                        ShowtimeDateTime = fullBooking.Showtime.Showtime,
                        SeatNumbers = string.Join(", ", fullBooking.Tickets.Select(t => t.Seat.Seatnumber)),
                        ComboItems = fullBooking.Bookingcombos.Select(bc => new ComboItemDTO
                        {
                            Name = bc.Combo.Name,
                            Quantity = bc.Quantity,
                            Price = bc.Combo.Price ?? 0
                        }).ToList(),
                        TotalAmount = payment.Amount,
                        DiscountAmount = fullBooking.Voucher != null ? CalculateDiscount(fullBooking) : 0,
                        VoucherCode = fullBooking.Voucher?.Code,
                        TransactionCode = payment.Transactioncode,
                        PaymentTime = payment.Paymenttime
                    };
                    
                    // Send email asynchronously (don't block callback response)
                    _ = Task.Run(async () => await _emailService.SendBookingConfirmationAsync(emailDto));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send booking confirmation email for booking {BookingId}", booking.Bookingid);
                    // Don't fail the payment if email fails
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
    
    // ... existing redirect code ...
}
```

### NuGet Packages Required

```xml
<!-- Movie88.Application/Movie88.Application.csproj -->
<ItemGroup>
    <PackageReference Include="QRCoder" Version="1.4.3" />
    <PackageReference Include="System.Drawing.Common" Version="8.0.0" />
</ItemGroup>
```

### Service Registration

```csharp
// Movie88.Application/Configuration/ServiceExtensions.cs
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // ... existing services ...
    
    // ✨ NEW: QR Code Service
    services.AddScoped<IQRCodeService, QRCodeService>();
    
    return services;
}
```

### Testing

#### Manual Test Email Send

```csharp
// Test controller endpoint (optional, for debugging)
[HttpPost("test/booking-confirmation-email")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> TestBookingConfirmationEmail([FromBody] string bookingCode)
{
    // Get booking by code
    var booking = await _context.Bookings
        .Include(b => b.Customer).ThenInclude(c => c.User)
        .Include(b => b.Tickets).ThenInclude(t => t.Seat)
        .Include(b => b.Bookingcombos).ThenInclude(bc => bc.Combo)
        .Include(b => b.Showtime).ThenInclude(s => s.Movie)
        .Include(b => b.Showtime).ThenInclude(s => s.Room).ThenInclude(r => r.Cinema)
        .Include(b => b.Voucher)
        .Include(b => b.Payment)
        .FirstOrDefaultAsync(b => b.Bookingcode == bookingCode);
    
    if (booking == null)
        return NotFound("Booking not found");
    
    // Generate QR code
    var qrCodeBase64 = await _qrCodeService.GenerateQRCodeBase64Async(bookingCode);
    
    // Send email
    var emailDto = new BookingConfirmationEmailDTO
    {
        // ... populate from booking ...
    };
    
    var result = await _emailService.SendBookingConfirmationAsync(emailDto);
    
    return Ok(new { success = result, message = "Email sent" });
}
```

### Error Handling

- If QR code generation fails → Log error, don't send email
- If email sending fails → Log error, don't fail payment callback
- VNPay callback must respond quickly (< 30 seconds) → Send email asynchronously with `Task.Run`

### Android Integration

```kotlin
// Display QR code from booking details
fun displayBookingQRCode(bookingCode: String) {
    // Option 1: Generate QR locally (offline support)
    val qrCodeBitmap = QRCode.from(bookingCode).bitmap()
    ivQRCode.setImageBitmap(qrCodeBitmap)
    
    // Option 2: Get from email attachment (user screenshots)
    // User can show screenshot of email QR code
}
```

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
**Progress**: 🔄 Phase 4 in development - Email Confirmation & QR Code  
**Completed Phases**: ✅ Phase 1-3 (8/8 endpoints - 100%)  
**Test File**: `tests/Payment.http` ✅  
**Android Guide**: ✅ Complete with Java + XML examples

---

## 📦 Phase 4 Implementation Checklist

### NuGet Packages
- [ ] Install `QRCoder` version 1.4.3
- [ ] Install `System.Drawing.Common` version 8.0.0

### New Files to Create

#### Domain Layer
- [ ] No new domain models needed (reuse existing)

#### Application Layer
- [ ] `Movie88.Application/Interfaces/IQRCodeService.cs`
- [ ] `Movie88.Application/Services/QRCodeService.cs`
- [ ] `Movie88.Application/DTOs/Email/BookingConfirmationEmailDTO.cs`
- [ ] `Movie88.Application/DTOs/Email/ComboItemDTO.cs`

#### Infrastructure Layer
- [ ] No new infrastructure needed

#### WebApi Layer
- [ ] Update `Movie88.WebApi/Controllers/PaymentsController.cs` - VNPayCallback method
- [ ] Update `Movie88.WebApi/Controllers/PaymentsController.cs` - VNPayIPN method

### Service Updates
- [ ] Extend `IEmailService` with `SendBookingConfirmationAsync` method
- [ ] Implement `ResendEmailService.SendBookingConfirmationAsync`
- [ ] Create email HTML template with professional design
- [ ] Register `IQRCodeService` in `ServiceExtensions.cs`

### Testing
- [ ] Test QR code generation (BK-20251105-0156)
- [ ] Test email sending with QR code attachment
- [ ] Test complete payment flow → email received
- [ ] Verify QR code scannable with mobile camera
- [ ] Test email rendering in Gmail, Outlook, Apple Mail
- [ ] Test with/without voucher discount
- [ ] Test with/without combo items
- [ ] Verify all dynamic placeholders replaced correctly

### Email Template Checklist
- [ ] Header with Movie88 gradient branding
- [ ] BookingCode displayed prominently
- [ ] QR code embedded inline (300x300px)
- [ ] Movie details (title, cinema, showtime, seats)
- [ ] Payment summary with breakdown
- [ ] Voucher discount shown if applied
- [ ] Transaction code and payment time
- [ ] Important information section
- [ ] Professional footer with unsubscribe
- [ ] Mobile-responsive design
- [ ] Test on multiple email clients

### Integration Points
- [ ] VNPayCallback → Generate QR → Send Email (success case)
- [ ] VNPayIPN → Same logic as callback
- [ ] Error handling: Email failure doesn't block payment
- [ ] Async email sending (don't block VNPay response)
- [ ] Logging for debugging

### Documentation
- [x] Update Phase 4 in 05-Payment.md
- [ ] Add email template examples
- [ ] Document QR code specifications
- [ ] Android integration guide for QR display

---

**Phase 4 Status**: 🔄 Ready to implement  
**Estimated Time**: 3-4 hours  
**Priority**: HIGH (improves customer experience significantly)
