# 💳 Payment API

## 1. Mô tả

Module Payment quản lý toàn bộ quy trình thanh toán trong hệ thống Movie88, bao gồm:
- Tích hợp với cổng thanh toán VNPay
- Xử lý callback từ VNPay gateway
- Quản lý trạng thái thanh toán (Pending → Success → Failed)
- Lưu lịch sử giao dịch
- Xử lý hoàn tiền khi hủy vé
- Đảm bảo tính toàn vẹn dữ liệu giữa Booking và Payment

## 2. Danh sách Endpoint

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/payments` | Danh sách thanh toán | Query params | List<PaymentDTO> | Admin/Manager |
| GET | `/api/payments/{id}` | Chi tiết thanh toán | paymentId | PaymentDTO | Customer/Admin |
| POST | `/api/payments/vnpay/create` | Tạo thanh toán VNPay | VNPayRequestDTO | VNPayResponseDTO | Customer |
| GET | `/api/payments/vnpay/callback` | Callback VNPay | Query params | Redirect | Public |
| POST | `/api/payments/vnpay/ipn` | IPN từ VNPay | VNPay IPN | Success | Public |
| PUT | `/api/payments/{id}/confirm` | Xác nhận thanh toán | - | PaymentDTO | System |
| PUT | `/api/payments/{id}/cancel` | Hủy thanh toán | - | Success message | Customer/Admin |
| POST | `/api/payments/{id}/refund` | Hoàn tiền | RefundDTO | RefundResponseDTO | Admin |

## 3. Data Transfer Objects (DTOs)

### 3.1 PaymentMethodDTO
```json
{
  "methodId": 1,
  "name": "VNPay",
  "description": "Thanh toán qua VNPay (ATM, Visa, MasterCard)",
  "logoUrl": "https://example.com/vnpay-logo.png",
  "isActive": true
}
```

### 3.2 CreatePaymentDTO
```json
{
  "bookingId": 1001,
  "customerId": 45,
  "methodId": 1,
  "amount": 196000,
  "returnUrl": "https://movie88.com/payment/result"
}
```

### 3.3 PaymentDTO
```json
{
  "paymentId": 5001,
  "bookingId": 1001,
  "customerId": 45,
  "customerName": "Nguyễn Văn A",
  "methodId": 1,
  "methodName": "VNPay",
  "amount": 196000,
  "status": "Pending",
  "transactionCode": "VNP_20251029_1001",
  "paymentTime": "2025-10-29T11:00:00Z",
  "bookingInfo": {
    "movieTitle": "Avengers: Endgame",
    "cinemaName": "CGV Vincom Center",
    "startTime": "2025-10-30T19:30:00Z",
    "seats": "D5, D6",
    "totalAmount": 196000
  }
}
```

### 3.4 VNPayRequestDTO
```json
{
  "bookingId": 1001,
  "amount": 196000,
  "orderInfo": "Thanh toan ve xem phim #1001",
  "returnUrl": "https://movie88.com/payment/result",
  "ipAddress": "192.168.1.100"
}
```

### 3.5 VNPayResponseDTO
```json
{
  "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=...",
  "transactionCode": "VNP_20251029_1001",
  "expireTime": "2025-10-29T11:15:00Z"
}
```

### 3.6 MoMoRequestDTO
```json
{
  "bookingId": 1001,
  "amount": 196000,
  "orderInfo": "Thanh toan ve xem phim #1001",
  "returnUrl": "https://movie88.com/payment/result",
### 3.6 VNPayCallbackDTO
```json
{
  "vnp_TmnCode": "YOUR_TMN_CODE",
  "vnp_Amount": "19600000",
  "vnp_BankCode": "NCB",
  "vnp_BankTranNo": "VNP14235875",
  "vnp_CardType": "ATM",
  "vnp_OrderInfo": "Thanh toan ve xem phim #1001",
  "vnp_PayDate": "20251029110500",
  "vnp_ResponseCode": "00",
  "vnp_TmnCode": "YOUR_TMN_CODE",
  "vnp_TransactionNo": "14235875",
  "vnp_TransactionStatus": "00",
  "vnp_TxnRef": "VNP_20251029_1001",
  "vnp_SecureHash": "abc123..."
}
```

### 3.7 RefundDTO
```json
{
  "paymentId": 5001,
  "refundAmount": 196000,
  "reason": "Khách hàng hủy vé",
  "refundMethod": "Original"
}
```

## 4. Luồng xử lý (Payment Flow)

### 4.1 VNPay Payment Flow

```
Step 1: Khách chọn phương thức VNPay
├─ Frontend gọi POST /api/payments/vnpay/create
├─ Input: { bookingId, amount, returnUrl }
└─ Backend xử lý:
    ├─ Validate booking exists và status = "Confirmed"
    ├─ Tạo record Payment với Status = "Pending"
    ├─ Generate transaction code unique
    ├─ Tạo VNPay payment URL với các params:
    │   ├─ vnp_TmnCode (Merchant code)
    │   ├─ vnp_Amount (amount * 100)
    │   ├─ vnp_OrderInfo
    │   ├─ vnp_ReturnUrl
    │   ├─ vnp_TxnRef (transaction code)
    │   └─ vnp_SecureHash (HMAC SHA512)
    └─ Return payment URL

Step 2: Redirect khách đến VNPay
├─ Frontend redirect user → VNPay payment page
├─ User nhập thông tin thẻ/ATM
└─ User xác nhận thanh toán trên VNPay

Step 3: VNPay callback
├─ VNPay redirect về: GET /api/payments/vnpay/callback?vnp_ResponseCode=00&...
├─ Backend xử lý callback:
│   ├─ Validate vnp_SecureHash (đảm bảo không bị giả mạo)
│   ├─ Check vnp_ResponseCode:
│   │   ├─ "00" = Success
│   │   └─ Khác "00" = Failed
│   ├─ Update Payment:
│   │   ├─ Status = "Success" hoặc "Failed"
│   │   ├─ TransactionCode = vnp_TransactionNo
│   │   └─ PaymentTime = now
│   ├─ Nếu success:
│   │   ├─ Update Booking.Status = "Paid"
│   │   ├─ Update Seats.IsAvailable = 0
│   │   ├─ Tăng Voucher.UsedCount
│   │   └─ Trigger email confirmation + QR code
│   └─ Redirect user về:
│       ├─ Success → /payment/success?bookingId=1001
│       └─ Failed → /payment/failed?reason=...

Step 4: Frontend hiển thị kết quả
├─ Success page: Hiển thị thông tin booking + QR code
└─ Failed page: Hiển thị lý do thất bại + nút thử lại
```

### 4.2 VNPay IPN (Instant Payment Notification) Flow

```
VNPay gửi IPN đến backend (independent from user callback)
├─ POST /api/payments/vnpay/ipn
├─ Backend xử lý:
│   ├─ Validate vnp_SecureHash
│   ├─ Check payment chưa được process (idempotency)
│   ├─ Update Payment & Booking status
│   ├─ Log transaction
│   └─ Return {"RspCode": "00", "Message": "Confirm Success"}
│
└─ Purpose: Đảm bảo backend nhận được kết quả thanh toán
    ngay cả khi user đóng browser trước khi redirect callback
```

### 4.3 Refund Flow

```
Khách hàng hủy vé → Trigger refund process

Step 1: Admin/System gọi POST /api/payments/{id}/refund
├─ Input: { refundAmount, reason }
├─ Backend validate:
│   ├─ Payment Status = "Success"
│   ├─ Booking đã bị cancel
│   └─ Thời gian hợp lệ để refund

Step 2: Xử lý refund qua VNPay
├─ Gọi VNPay Refund API
├─ Params:
│   ├─ vnp_RequestId (unique refund ID)
│   ├─ vnp_TxnRef (original transaction code)
│   ├─ vnp_Amount (refund amount)
│   ├─ vnp_TransactionType (02 = Full refund, 03 = Partial)
│   ├─ vnp_TransDate (original transaction date)
│   └─ vnp_SecureHash
└─ VNPay response với refund status

Step 3: Cập nhật database
├─ Tạo record Payment mới (Type = "Refund", Amount = -refundAmount)
├─ Link với Payment gốc (ReferencePaymentId)
└─ Gửi email thông báo hoàn tiền

Step 4: VNPay xử lý refund (2-7 ngày)
└─ Tiền về tài khoản khách hàng
```

## 5. Payment Gateway Configuration

### 5.1 VNPay Configuration
```json
{
  "VNPay": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ApiUrl": "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction",
    "ReturnUrl": "https://movie88.com/payment/result",
    "Version": "2.1.0",
    "Command": "pay",
    "CurrencyCode": "VND",
    "Locale": "vn"
  }
}
```

**Environment Variables:**
```bash
VNPAY_TMN_CODE=YOUR_TMN_CODE
VNPAY_HASH_SECRET=YOUR_HASH_SECRET
VNPAY_PAYMENT_URL=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
VNPAY_API_URL=https://sandbox.vnpayment.vn/merchant_webapi/api/transaction
VNPAY_RETURN_URL=https://movie88.com/payment/result
```

## 6. Security Considerations

### 6.1 Signature Validation (VNPay)
```csharp
public bool ValidateVNPaySignature(Dictionary<string, string> vnpParams)
{
    // Remove vnp_SecureHash from params
    string vnp_SecureHash = vnpParams["vnp_SecureHash"];
    vnpParams.Remove("vnp_SecureHash");
    vnpParams.Remove("vnp_SecureHashType");
    
    // Sort params by key
    var sortedParams = vnpParams.OrderBy(x => x.Key);
    
    // Build hash data
    string hashData = string.Join("&", 
        sortedParams.Select(x => $"{x.Key}={x.Value}"));
    
    // Compute HMAC SHA512
    string computedHash = HmacSHA512(hashSecret, hashData);
    
    // Compare
    return computedHash.Equals(vnp_SecureHash, 
        StringComparison.InvariantCultureIgnoreCase);
}
```

### 6.2 Idempotency
- Mỗi payment request phải có unique transaction code
- Ngăn chặn duplicate payment khi user refresh/retry
- Check TransactionCode exists trước khi tạo payment mới

### 6.3 Amount Validation
```csharp
// Validate amount trước khi gửi đến gateway
public bool ValidatePaymentAmount(int bookingId, decimal amount)
{
    var booking = GetBooking(bookingId);
    
    // Amount phải khớp với TotalAmount của booking
    if (booking.TotalAmount != amount)
    {
        throw new InvalidAmountException();
    }
    
    return true;
}
```

## 7. Business Rules

### 7.1 Payment Rules
- Mỗi booking chỉ có 1 payment success duy nhất
- Payment timeout sau 15 phút nếu không hoàn tất
- Không cho phép thanh toán nếu showtime đã qua
- Amount phải khớp chính xác với Booking.TotalAmount

### 7.2 Refund Rules
| Điều kiện | Refund Amount |
|-----------|---------------|
| Hủy > 24h trước showtime | 100% |
| Hủy 2h-24h trước showtime | 80% (phí 20%) |
| Hủy < 2h trước showtime | Không được phép |
| Rạp hủy suất chiếu | 100% + compensation |

### 7.3 Status Transitions
```
Pending → Processing → Success
   ↓          ↓           ↓
Expired   Failed    Refunded
```

## 8. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 400 | `INVALID_AMOUNT` | "Số tiền không hợp lệ" | Amount không khớp với booking |
| 400 | `PAYMENT_TIMEOUT` | "Phiên thanh toán hết hạn" | Quá 15 phút |
| 400 | `BOOKING_NOT_CONFIRMED` | "Booking chưa xác nhận" | Status != Confirmed |
| 409 | `PAYMENT_EXISTS` | "Đã có thanh toán cho booking này" | Duplicate payment |
| 400 | `INVALID_SIGNATURE` | "Chữ ký không hợp lệ" | Callback signature mismatch |
| 400 | `VNPAY_ERROR` | "Lỗi từ cổng thanh toán VNPay" | Error from VNPay gateway |
| 403 | `REFUND_NOT_ALLOWED` | "Không thể hoàn tiền" | Quá thời gian cho phép |

### VNPay Response Codes
| Code | Meaning |
|------|---------|
| 00 | Giao dịch thành công |
| 07 | Trừ tiền thành công, giao dịch bị nghi ngờ (liên hệ VNPAY) |
| 09 | Giao dịch không thành công do thẻ chưa đăng ký dịch vụ InternetBanking |
| 10 | Giao dịch không thành công do khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần |
| 11 | Giao dịch không thành công do đã hết hạn chờ thanh toán |
| 12 | Giao dịch không thành công do thẻ/tài khoản bị khóa |
| 13 | Giao dịch không thành công do nhập sai mật khẩu xác thực giao dịch (OTP) |
| 24 | Giao dịch không thành công do khách hàng hủy giao dịch |
| 51 | Giao dịch không thành công do tài khoản không đủ số dư |
| 65 | Giao dịch không thành công do tài khoản vượt quá hạn mức giao dịch trong ngày |
| 75 | Ngân hàng thanh toán đang bảo trì |
| 79 | Giao dịch không thành công do nhập sai mật khẩu thanh toán quá số lần quy định |
| 99 | Các lỗi khác |

## 9. Database Schema Notes

### Payments Table
```sql
CREATE TABLE Payments (
    PaymentId SERIAL PRIMARY KEY,
    BookingId INT NOT NULL,
    CustomerId INT NOT NULL,
    MethodId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Status VARCHAR(50) DEFAULT 'Pending',
    TransactionCode VARCHAR(255) NULL UNIQUE, -- Unique constraint
    PaymentTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    RefundAmount DECIMAL(10,2) NULL,
    RefundTime TIMESTAMP NULL,
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (MethodId) REFERENCES PaymentMethods(MethodId)
);

-- Index for performance
CREATE INDEX idx_payments_booking ON Payments(BookingId);
CREATE INDEX idx_payments_transaction ON Payments(TransactionCode);
CREATE INDEX idx_payments_status ON Payments(Status);
```

## 10. Sample API Calls

## 10. Sample API Calls

### Tạo thanh toán VNPay
```bash
POST /api/payments/vnpay/create
Authorization: Bearer {token}
Content-Type: application/json

{
  "bookingId": 1001,
  "amount": 196000,
  "orderInfo": "Thanh toan ve xem phim #1001",
  "returnUrl": "https://movie88.com/payment/result",
  "ipAddress": "192.168.1.100"
}

Response:
{
  "success": true,
  "data": {
    "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=...",
    "transactionCode": "VNP_20251029_1001",
    "expireTime": "2025-10-29T11:15:00Z"
  }
}
```

### Callback từ VNPay (GET request)
```bash
GET /api/payments/vnpay/callback?
  vnp_Amount=19600000&
  vnp_BankCode=NCB&
  vnp_ResponseCode=00&
  vnp_TransactionNo=13745556&
  vnp_TxnRef=VNP_20251029_1001&
  vnp_SecureHash=abc123...

Backend xử lý và redirect:
→ Success: https://movie88.com/payment/success?bookingId=1001
→ Failed: https://movie88.com/payment/failed?reason=insufficient_balance
```

### IPN từ VNPay (POST request)
```bash
POST /api/payments/vnpay/ipn
Content-Type: application/json

{
  "vnp_TmnCode": "YOUR_TMN_CODE",
  "vnp_Amount": "19600000",
  "vnp_BankCode": "NCB",
  "vnp_ResponseCode": "00",
  "vnp_TransactionNo": "13745556",
  "vnp_TxnRef": "VNP_20251029_1001",
  "vnp_SecureHash": "abc123..."
}

Response:
{
  "RspCode": "00",
  "Message": "Confirm Success"
}
```

### Hoàn tiền
```bash
POST /api/payments/5001/refund
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "refundAmount": 196000,
  "reason": "Khách hàng hủy vé",
  "refundMethod": "Original"
}

Response:
{
  "success": true,
  "message": "Yêu cầu hoàn tiền đã được gửi",
  "data": {
    "refundId": 5002,
    "status": "Processing",
    "estimatedDate": "2025-11-05"
  }
}
```

## 11. 3-Layer Architecture Implementation

### 11.1 Controller Layer
```csharp
[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost("vnpay/create")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateVNPayPayment(
        [FromBody] VNPayRequestDTO request)
    {
        // Validate DTO
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        // Gọi service layer
        var result = await _paymentService.CreateVNPayPaymentAsync(request);
        
        return Ok(new { success = true, data = result });
    }
    
    [HttpGet("vnpay/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> VNPayCallback()
    {
        // Lấy query params từ VNPay
        var vnpParams = Request.Query.ToDictionary(
            x => x.Key, x => x.Value.ToString());
        
        // Gọi service layer để xử lý
        var result = await _paymentService.HandleVNPayCallbackAsync(vnpParams);
        
        // Redirect về frontend
        if (result.IsSuccess)
            return Redirect($"{_returnUrl}/success?bookingId={result.BookingId}");
        else
            return Redirect($"{_returnUrl}/failed?reason={result.ErrorCode}");
    }
}
```

### 11.2 Service Layer
```csharp
public interface IPaymentService
{
    Task<VNPayResponseDTO> CreateVNPayPaymentAsync(VNPayRequestDTO request);
    Task<PaymentCallbackResult> HandleVNPayCallbackAsync(Dictionary<string, string> vnpParams);
    Task<RefundResponseDTO> ProcessRefundAsync(int paymentId, RefundDTO request);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IVNPayService _vnpayService;
    private readonly IEmailService _emailService;
    
    public PaymentService(
        IPaymentRepository paymentRepository,
        IBookingRepository bookingRepository,
        IVNPayService vnpayService,
        IEmailService emailService)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _vnpayService = vnpayService;
        _emailService = emailService;
    }
    
    public async Task<VNPayResponseDTO> CreateVNPayPaymentAsync(VNPayRequestDTO request)
    {
        // Business logic validation
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
        if (booking == null)
            throw new NotFoundException("Booking not found");
        
        if (booking.Status != "Confirmed")
            throw new BadRequestException("Booking not confirmed");
        
        // Check duplicate payment
        var existingPayment = await _paymentRepository
            .GetByBookingIdAsync(request.BookingId);
        if (existingPayment != null && existingPayment.Status == "Success")
            throw new DuplicatePaymentException("Payment already exists");
        
        // Tạo payment record
        var payment = new Payment
        {
            BookingId = request.BookingId,
            CustomerId = booking.CustomerId,
            Amount = request.Amount,
            Status = "Pending",
            TransactionCode = GenerateTransactionCode(),
            PaymentTime = DateTime.Now
        };
        
        await _paymentRepository.CreateAsync(payment);
        
        // Tạo VNPay payment URL
        var paymentUrl = await _vnpayService.CreatePaymentUrlAsync(
            payment.TransactionCode,
            request.Amount,
            request.OrderInfo,
            request.ReturnUrl,
            request.IpAddress
        );
        
        return new VNPayResponseDTO
        {
            PaymentUrl = paymentUrl,
            TransactionCode = payment.TransactionCode,
            ExpireTime = DateTime.Now.AddMinutes(15)
        };
    }
    
    public async Task<PaymentCallbackResult> HandleVNPayCallbackAsync(
        Dictionary<string, string> vnpParams)
    {
        // Validate signature
        if (!_vnpayService.ValidateSignature(vnpParams))
            throw new InvalidSignatureException("Invalid VNPay signature");
        
        var txnRef = vnpParams["vnp_TxnRef"];
        var responseCode = vnpParams["vnp_ResponseCode"];
        var transactionNo = vnpParams["vnp_TransactionNo"];
        
        // Lấy payment từ DB
        var payment = await _paymentRepository.GetByTransactionCodeAsync(txnRef);
        if (payment == null)
            throw new NotFoundException("Payment not found");
        
        // Update payment status
        if (responseCode == "00") // Success
        {
            payment.Status = "Success";
            payment.TransactionCode = transactionNo;
            payment.PaymentTime = DateTime.Now;
            
            // Update booking status
            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
            booking.Status = "Paid";
            
            await _paymentRepository.UpdateAsync(payment);
            await _bookingRepository.UpdateAsync(booking);
            
            // Send confirmation email
            await _emailService.SendBookingConfirmationAsync(booking);
            
            return new PaymentCallbackResult
            {
                IsSuccess = true,
                BookingId = payment.BookingId
            };
        }
        else // Failed
        {
            payment.Status = "Failed";
            await _paymentRepository.UpdateAsync(payment);
            
            return new PaymentCallbackResult
            {
                IsSuccess = false,
                ErrorCode = MapVNPayErrorCode(responseCode)
            };
        }
    }
    
    private string GenerateTransactionCode()
    {
        return $"VNP_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
}
```

### 11.3 Repository Layer
```csharp
public interface IPaymentRepository
{
    Task<Payment> GetByIdAsync(int paymentId);
    Task<Payment> GetByBookingIdAsync(int bookingId);
    Task<Payment> GetByTransactionCodeAsync(string transactionCode);
    Task<List<Payment>> GetAllAsync(PaymentFilter filter);
    Task<Payment> CreateAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(int paymentId);
}

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    
    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Payment> GetByIdAsync(int paymentId)
    {
        return await _context.Payments
            .Include(p => p.Booking)
            .Include(p => p.Customer)
            .Include(p => p.PaymentMethod)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }
    
    public async Task<Payment> GetByBookingIdAsync(int bookingId)
    {
        return await _context.Payments
            .Where(p => p.BookingId == bookingId && p.Status == "Success")
            .FirstOrDefaultAsync();
    }
    
    public async Task<Payment> GetByTransactionCodeAsync(string transactionCode)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.TransactionCode == transactionCode);
    }
    
    public async Task<Payment> CreateAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
    
    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}
```

## 12. Testing

### Test Cases
1. ✅ Test tạo payment với valid booking
2. ✅ Test tạo payment với invalid booking
3. ✅ Test duplicate payment prevention
4. ✅ Test VNPay signature validation
5. ✅ Test callback processing (success)
6. ✅ Test callback processing (failed)
7. ✅ Test IPN processing
8. ✅ Test payment timeout
9. ✅ Test refund flow
10. ✅ Test concurrent payment attempts

### Sandbox Testing URL
- **VNPay Sandbox**: https://sandbox.vnpayment.vn
- **VNPay Documentation**: https://sandbox.vnpayment.vn/apis/docs/

### Test Cards (VNPay Sandbox)
| Bank | Card Number | Card Holder | Expiry | OTP |
|------|-------------|-------------|--------|-----|
| NCB | 9704198526191432198 | NGUYEN VAN A | 07/15 | 123456 |
| Techcombank | 9704000000000018 | NGUYEN VAN A | Any future date | 147258 |

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
