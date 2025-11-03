# 🎁 Promotion & Voucher API

## 1. Mô tả

Module Promotion & Voucher quản lý các chương trình khuyến mãi và mã giảm giá trong hệ thống, bao gồm:
- Quản lý voucher codes (mã giảm giá do khách nhập)
- Quản lý promotions (khuyến mãi tự động áp dụng)
- Tính toán discount (Percent hoặc Amount)
- Validate điều kiện áp dụng
- Theo dõi số lần sử dụng
- Hết hạn tự động

## 2. Danh sách Endpoint

### 2.1 Voucher Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/vouchers` | Danh sách voucher | Query params | List<VoucherDTO> | Admin/Manager |
| GET | `/api/vouchers/{id}` | Chi tiết voucher | voucherId | VoucherDTO | All |
| GET | `/api/vouchers/available` | Voucher khả dụng | - | List<VoucherDTO> | Customer |
| POST | `/api/vouchers/validate` | Kiểm tra mã voucher | ValidateVoucherDTO | VoucherValidationDTO | Customer |
| POST | `/api/vouchers` | Tạo voucher | CreateVoucherDTO | VoucherDTO | Admin/Manager |
| PUT | `/api/vouchers/{id}` | Cập nhật voucher | UpdateVoucherDTO | VoucherDTO | Admin/Manager |
| DELETE | `/api/vouchers/{id}` | Xóa voucher | voucherId | Success message | Admin |

### 2.2 Promotion Management

| Method | Endpoint | Mô tả | Input | Output | Auth |
|--------|----------|-------|-------|--------|------|
| GET | `/api/promotions` | Danh sách khuyến mãi | Query params | List<PromotionDTO> | Public |
| GET | `/api/promotions/{id}` | Chi tiết khuyến mãi | promotionId | PromotionDTO | Public |
| GET | `/api/promotions/active` | Khuyến mãi đang hoạt động | - | List<PromotionDTO> | Public |
| POST | `/api/promotions` | Tạo khuyến mãi | CreatePromotionDTO | PromotionDTO | Admin/Manager |
| PUT | `/api/promotions/{id}` | Cập nhật khuyến mãi | UpdatePromotionDTO | PromotionDTO | Admin/Manager |
| DELETE | `/api/promotions/{id}` | Xóa khuyến mãi | promotionId | Success message | Admin |

## 3. Data Transfer Objects (DTOs)

### 3.1 VoucherDTO
```json
{
  "voucherId": 10,
  "code": "SUMMER2025",
  "description": "Giảm 10% cho đơn hàng từ 200k",
  "discountType": "Percent",
  "discountValue": 10,
  "minPurchaseAmount": 200000,
  "maxDiscountAmount": 50000,
  "expiryDate": "2025-12-31",
  "usageLimit": 1000,
  "usedCount": 350,
  "remainingUses": 650,
  "isActive": true,
  "createdAt": "2025-10-01T00:00:00Z"
}
```

### 3.2 CreateVoucherDTO
```json
{
  "code": "NEWYEAR2026",
  "description": "Giảm 50k cho đơn hàng từ 300k",
  "discountType": "Amount",
  "discountValue": 50000,
  "minPurchaseAmount": 300000,
  "maxDiscountAmount": null,
  "expiryDate": "2026-01-31",
  "usageLimit": 500,
  "isActive": true,
  "applicableMovieIds": [123, 456],
  "applicableCinemaIds": [1, 2, 3]
}
```

**Validation Rules:**
- `code`: Required, 6-50 ký tự, unique, uppercase + numbers
- `discountType`: Required, values: "Percent" hoặc "Amount"
- `discountValue`: Required
  - Nếu Percent: 1-100
  - Nếu Amount: 5,000 - 500,000 VND
- `minPurchaseAmount`: Optional, >= 0
- `maxDiscountAmount`: Optional, chỉ dùng với Percent
- `expiryDate`: Required, must be future date
- `usageLimit`: Required, >= 1

### 3.3 ValidateVoucherDTO
```json
{
  "voucherCode": "SUMMER2025",
  "bookingAmount": 250000
}
```

### 3.4 VoucherValidationDTO
```json
{
  "isValid": true,
  "voucherId": 10,
  "code": "SUMMER2025",
  "discountType": "Percent",
  "discountValue": 10,
  "discountAmount": 25000,
  "finalAmount": 225000,
  "message": "Voucher hợp lệ"
}
```

**OR nếu invalid:**
```json
{
  "isValid": false,
  "code": "SUMMER2025",
  "message": "Voucher đã hết hạn sử dụng",
  "errorCode": "VOUCHER_EXPIRED"
}
```

### 3.5 PromotionDTO
```json
{
  "promotionId": 5,
  "name": "Black Friday 20%",
  "description": "Giảm 20% cho tất cả suất chiếu vào thứ 6",
  "startDate": "2025-11-01",
  "endDate": "2025-11-30",
  "discountType": "Percent",
  "discountValue": 20,
  "maxDiscountAmount": 100000,
  "conditions": {
    "dayOfWeek": [5],
    "minBookingAmount": 150000,
    "applicableMovieIds": null,
    "applicableCinemaIds": null
  },
  "isActive": true,
  "createdAt": "2025-10-15T00:00:00Z"
}
```

### 3.6 CreatePromotionDTO
```json
{
  "name": "Weekend Special",
  "description": "Giảm 15% cho suất chiếu cuối tuần",
  "startDate": "2025-11-01",
  "endDate": "2025-12-31",
  "discountType": "Percent",
  "discountValue": 15,
  "maxDiscountAmount": 75000,
  "conditions": {
    "dayOfWeek": [6, 0],
    "timeSlots": ["19:00-23:59"],
    "minBookingAmount": 100000
  },
  "isActive": true
}
```

## 4. Luồng xử lý (Flow)

### 4.1 Customer Apply Voucher Flow

```
User đang trong booking flow
↓
Nhập mã voucher "SUMMER2025"
↓
POST /api/vouchers/validate
{
  "voucherCode": "SUMMER2025",
  "bookingAmount": 250000
}
↓
Backend validate:
1. Check voucher exists
   SELECT * FROM Vouchers WHERE Code = 'SUMMER2025'
   
2. Check conditions:
   ├─ IsActive = true?
   ├─ ExpiryDate >= GETDATE()?
   ├─ UsedCount < UsageLimit?
   ├─ bookingAmount >= MinPurchaseAmount?
   └─ All conditions met?
   
3. Calculate discount:
   IF discountType = 'Percent':
       discount = bookingAmount * (discountValue / 100)
       IF maxDiscountAmount:
           discount = MIN(discount, maxDiscountAmount)
   ELSE:
       discount = discountValue
   
   finalAmount = bookingAmount - discount
↓
Return VoucherValidationDTO
{
  "isValid": true,
  "discountAmount": 25000,
  "finalAmount": 225000
}
↓
Frontend hiển thị discount và final amount
↓
User confirm booking → Apply voucher
↓
POST /api/bookings/{id}/apply-voucher
{
  "voucherCode": "SUMMER2025"
}
↓
Backend:
1. Validate lại voucher (double-check)
2. UPDATE Bookings SET VoucherId = 10, TotalAmount = finalAmount
3. Không tăng UsedCount ngay (chỉ tăng khi payment success)
↓
Return updated BookingDTO
```

### 4.2 Auto Apply Promotion Flow

```
User đang booking
↓
Backend tính tổng tiền sau khi apply voucher
↓
Tự động check active promotions:
SELECT * FROM Promotions
WHERE IsActive = 1
  AND StartDate <= GETDATE()
  AND EndDate >= GETDATE()
↓
FOR EACH promotion:
    Check conditions:
    ├─ dayOfWeek matches? (e.g. Friday = 5)
    ├─ timeSlot matches?
    ├─ minBookingAmount met?
    ├─ movieId in applicableMovieIds? (if specified)
    └─ cinemaId in applicableCinemaIds? (if specified)
    
    IF all conditions met:
        Calculate discount
        Add to applicable promotions list
↓
Apply all applicable promotions:
FOR EACH applicable promotion:
    IF discountType = 'Percent':
        discount = subtotal * (discountValue / 100)
        IF maxDiscountAmount:
            discount = MIN(discount, maxDiscountAmount)
    ELSE:
        discount = discountValue
    
    INSERT INTO BookingPromotions (BookingId, PromotionId, DiscountApplied)
    VALUES (bookingId, promotionId, discount)
    
    totalDiscount += discount
↓
UPDATE Bookings SET TotalAmount = subtotal - voucherDiscount - totalPromotionDiscount
↓
Return BookingDetailDTO với all discounts
```

### 4.3 Admin Create Voucher Flow

```
Admin vào "Tạo voucher mới"
↓
Điền form CreateVoucherDTO
↓
POST /api/vouchers
Authorization: Bearer {adminToken}
↓
Backend validate:
├─ Check code unique
├─ Validate discount value range
├─ Validate expiry date is future
└─ Validate usage limit > 0
↓
Generate unique code nếu không nhập:
code = "PROMO" + RANDOM(6 digits) + DATE.format("MMDD")
Example: "PROMO8741231025"
↓
INSERT INTO Vouchers (Code, Description, DiscountType, ...)
VALUES (...)
↓
Return VoucherDTO
↓
Admin có thể share voucher code cho customers
```

### 4.4 Voucher Usage Tracking Flow

```
Payment successful
↓
Trigger: Update voucher usage count
↓
IF booking.VoucherId IS NOT NULL:
    UPDATE Vouchers
    SET UsedCount = UsedCount + 1
    WHERE VoucherId = booking.VoucherId
    
    -- Check if reached limit
    IF usedCount >= usageLimit:
        UPDATE Vouchers SET IsActive = 0
↓
Log voucher usage:
INSERT INTO VoucherUsageLog (VoucherId, BookingId, CustomerId, UsedAt)
VALUES (...)
```

## 5. Business Rules

### 5.1 Voucher Rules
- Chỉ áp dụng 1 voucher per booking
- Voucher không kết hợp với voucher khác
- Voucher có thể kết hợp với promotions
- UsedCount tăng khi payment success (không phải khi apply)
- Auto-deactivate khi UsedCount >= UsageLimit

### 5.2 Promotion Rules
- Có thể áp dụng nhiều promotions cùng lúc
- Tự động áp dụng nếu đủ điều kiện
- Không cần nhập code
- Priority thấp hơn voucher (apply sau voucher)

### 5.3 Discount Calculation Order
```
1. Tính subtotal (seats + combos)
2. Apply voucher discount
3. Apply promotion discounts
4. Final amount = subtotal - voucherDiscount - promotionDiscounts
```

### 5.4 Discount Limits
| Discount Type | Min | Max | Note |
|---------------|-----|-----|------|
| Percent voucher | 1% | 100% | Nên có maxDiscountAmount |
| Amount voucher | 5,000 | 500,000 | Fixed amount |
| Percent promotion | 1% | 50% | Không cho phép > 50% |
| Amount promotion | 5,000 | 200,000 | - |

### 5.5 Condition Types

**Day of Week:**
- 0 = Sunday
- 1 = Monday
- ... 
- 6 = Saturday

**Time Slots:**
- Format: "HH:MM-HH:MM"
- Examples: "10:00-12:00", "19:00-23:59"

**Applicable Entities:**
- `applicableMovieIds`: Chỉ áp dụng cho các phim cụ thể
- `applicableCinemaIds`: Chỉ áp dụng tại các rạp cụ thể
- Null = áp dụng cho tất cả

## 6. Validation Rules

### CreateVoucherDTO Validation
```csharp
public class CreateVoucherDTO
{
    [Required]
    [RegularExpression(@"^[A-Z0-9]{6,50}$", 
        ErrorMessage = "Mã voucher chỉ chứa chữ IN HOA và số, 6-50 ký tự")]
    public string Code { get; set; }

    [Required]
    [MaxLength(255)]
    public string Description { get; set; }

    [Required]
    [RegularExpression("^(Percent|Amount)$")]
    public string DiscountType { get; set; }

    [Required]
    [Range(1, double.MaxValue)]
    public decimal DiscountValue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinPurchaseAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxDiscountAmount { get; set; }

    [Required]
    [FutureDate]
    public DateTime ExpiryDate { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int UsageLimit { get; set; }
}

// Custom validation
public override ValidationResult Validate(ValidationContext context)
{
    if (DiscountType == "Percent" && (DiscountValue < 1 || DiscountValue > 100))
    {
        return new ValidationResult("Percent phải từ 1-100");
    }
    
    if (DiscountType == "Amount" && (DiscountValue < 5000 || DiscountValue > 500000))
    {
        return new ValidationResult("Amount phải từ 5,000 - 500,000 VND");
    }
    
    return ValidationResult.Success;
}
```

## 7. Error Handling

| Status Code | Error Code | Message | Description |
|-------------|-----------|---------|-------------|
| 404 | `VOUCHER_NOT_FOUND` | "Không tìm thấy voucher" | Voucher không tồn tại |
| 400 | `VOUCHER_EXPIRED` | "Voucher đã hết hạn" | ExpiryDate < now |
| 400 | `VOUCHER_INACTIVE` | "Voucher không hoạt động" | IsActive = false |
| 400 | `VOUCHER_USAGE_EXCEEDED` | "Voucher đã hết lượt sử dụng" | UsedCount >= UsageLimit |
| 400 | `MIN_PURCHASE_NOT_MET` | "Chưa đủ điều kiện áp voucher" | Amount < MinPurchaseAmount |
| 409 | `VOUCHER_CODE_EXISTS` | "Mã voucher đã tồn tại" | Duplicate code |
| 400 | `INVALID_DISCOUNT_VALUE` | "Giá trị giảm không hợp lệ" | Out of range |

## 8. Query Optimization

### 8.1 Get Active Promotions
```sql
-- Lấy promotions đang hoạt động
SELECT * FROM Promotions
WHERE IsActive = 1
  AND StartDate <= GETDATE()
  AND EndDate >= GETDATE()
ORDER BY DiscountValue DESC;
```

### 8.2 Check Voucher Availability
```sql
-- Check voucher có thể sử dụng không
SELECT 
    *,
    (UsageLimit - UsedCount) AS RemainingUses,
    CASE 
        WHEN IsActive = 0 THEN 'Inactive'
        WHEN ExpiryDate < GETDATE() THEN 'Expired'
        WHEN UsedCount >= UsageLimit THEN 'Limit Exceeded'
        ELSE 'Available'
    END AS Status
FROM Vouchers
WHERE Code = @Code;
```

### 8.3 Voucher Usage Statistics
```sql
-- Thống kê sử dụng voucher
SELECT 
    v.VoucherId,
    v.Code,
    v.Description,
    v.UsageLimit,
    v.UsedCount,
    (v.UsageLimit - v.UsedCount) AS RemainingUses,
    COUNT(DISTINCT b.CustomerId) AS UniqueUsers,
    SUM(b.TotalAmount) AS TotalRevenue,
    AVG(b.TotalAmount) AS AvgOrderValue
FROM Vouchers v
LEFT JOIN Bookings b ON v.VoucherId = b.VoucherId
WHERE b.Status = 'Paid'
GROUP BY v.VoucherId, v.Code, v.Description, v.UsageLimit, v.UsedCount
ORDER BY TotalRevenue DESC;
```

## 9. Caching Strategy

```csharp
// Cache active promotions (30 minutes)
Cache: "promotions:active" → List<PromotionDTO>

// Cache voucher by code (1 hour)
Cache: "voucher:code:{code}" → VoucherDTO

// Cache available vouchers for customer (15 minutes)
Cache: "vouchers:available:customer:{customerId}" → List<VoucherDTO>
```

## 10. Sample API Calls

### Validate voucher
```bash
POST /api/vouchers/validate
Content-Type: application/json

{
  "voucherCode": "SUMMER2025",
  "bookingAmount": 250000
}

Response (Valid):
{
  "success": true,
  "data": {
    "isValid": true,
    "voucherId": 10,
    "code": "SUMMER2025",
    "discountType": "Percent",
    "discountValue": 10,
    "discountAmount": 25000,
    "finalAmount": 225000,
    "message": "Voucher hợp lệ"
  }
}

Response (Invalid):
{
  "success": false,
  "data": {
    "isValid": false,
    "code": "SUMMER2025",
    "message": "Chưa đủ điều kiện sử dụng voucher (Tối thiểu 200,000 VND)",
    "errorCode": "MIN_PURCHASE_NOT_MET"
  }
}
```

### Lấy promotions đang hoạt động
```bash
GET /api/promotions/active

Response:
{
  "success": true,
  "data": [
    {
      "promotionId": 5,
      "name": "Black Friday 20%",
      "description": "Giảm 20% tất cả suất chiếu thứ 6",
      "discountType": "Percent",
      "discountValue": 20,
      "startDate": "2025-11-01",
      "endDate": "2025-11-30"
    }
  ]
}
```

### Tạo voucher (Admin)
```bash
POST /api/vouchers
Authorization: Bearer {adminToken}

{
  "code": "XMAS2025",
  "description": "Giảm 100k cho đơn từ 500k",
  "discountType": "Amount",
  "discountValue": 100000,
  "minPurchaseAmount": 500000,
  "expiryDate": "2025-12-31",
  "usageLimit": 200,
  "isActive": true
}
```

---

**Last Updated**: October 29, 2025
**Module Version**: v1.0
