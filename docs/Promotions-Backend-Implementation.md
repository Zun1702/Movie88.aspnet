# 🎁 Promotions Feature - Backend Implementation Guide

**Created**: November 6, 2025  
**Status**: 📋 **PLANNING - Ready for Implementation**  
**Purpose**: Tự động áp dụng khuyến mãi vào booking dựa trên điều kiện thời gian

---

## 📋 Mục Lục

1. [Tổng Quan](#1-tổng-quan)
2. [Database Migration](#2-database-migration)
3. [API Endpoints Cần Phát Triển](#3-api-endpoints-cần-phát-triển)
4. [Implementation Details](#4-implementation-details)
5. [Testing Guide](#5-testing-guide)
6. [Deployment Checklist](#6-deployment-checklist)

---

## 1. Tổng Quan

### 1.1 Mục Tiêu
Xây dựng hệ thống Promotions có thể:
- ✅ Tạo khuyến mãi theo khoảng thời gian (startdate → enddate)
- ✅ Tự động áp dụng promotion khi user booking
- ✅ Tính toán và lưu discount vào `bookingpromotions`
- ✅ Hiển thị promotions đang active trên app

### 1.2 Use Case: "Khuyến Mãi Tháng 11"
```
Admin tạo promotion:
- Tên: "Khuyến Mãi Tháng 11"
- Mô tả: "Giảm 20% cho tất cả vé trong tháng 11"
- Thời gian: 01/11/2025 → 30/11/2025
- Loại: percentage
- Giá trị: 20%

User đặt vé ngày 15/11/2025:
- Chọn phim + ghế → Total: 200,000 VND
- Backend tự động:
  ✓ Check promotions active
  ✓ Tìm thấy "Khuyến Mãi Tháng 11"
  ✓ Apply: 200,000 × 20% = 40,000 VND
  ✓ Insert vào bookingpromotions
  ✓ Update booking.totalamount = 160,000 VND
- User thấy: Giảm 40,000 VND → Thanh toán 160,000 VND
```

### 1.3 Luồng Dữ Liệu
```
┌─────────────────────────────────────────────────────┐
│ PHASE 1: Admin tạo Promotion                        │
└─────────────────────────────────────────────────────┘
POST /api/admin/promotions
↓
INSERT INTO promotions (name, startdate, enddate, discounttype, discountvalue)
↓
Return PromotionDTO

┌─────────────────────────────────────────────────────┐
│ PHASE 2: User booking                               │
└─────────────────────────────────────────────────────┘
POST /api/bookings
↓
Calculate initial total (seats + combos)
↓
Check active promotions:
  GET promotions WHERE startdate <= NOW() AND enddate >= NOW()
↓
Apply eligible promotions:
  FOR EACH promotion:
    Calculate discount
    INSERT INTO bookingpromotions
    totalDiscount += discount
↓
Update booking.totalamount = initialTotal - totalDiscount
↓
Return BookingDTO with applied promotions

┌─────────────────────────────────────────────────────┐
│ PHASE 3: Display trên App                          │
└─────────────────────────────────────────────────────┘
GET /api/promotions/active
↓
Return list promotions đang active
↓
App hiển thị banner/carousel
```

---

## 2. Database Migration

### 2.1 Bảng Hiện Tại: `promotions`
```sql
-- ✅ ĐÃ CÓ
CREATE TABLE promotions (
    promotionid SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(255),
    startdate DATE,
    enddate DATE,
    discounttype VARCHAR(20),      -- "percentage", "fixed"
    discountvalue DECIMAL(10,2)
);
```

### 2.2 Migration: Thêm Fields Mới (Optional - Cho tương lai)
```sql
-- File: database/migrations/003_enhance_promotions.sql

-- Thêm các fields để mở rộng tính năng sau này
ALTER TABLE promotions ADD COLUMN IF NOT EXISTS is_active BOOLEAN DEFAULT TRUE;
ALTER TABLE promotions ADD COLUMN IF NOT EXISTS priority INTEGER DEFAULT 0;
ALTER TABLE promotions ADD COLUMN IF NOT EXISTS usage_limit INTEGER;
ALTER TABLE promotions ADD COLUMN IF NOT EXISTS usage_count INTEGER DEFAULT 0;
ALTER TABLE promotions ADD COLUMN IF NOT EXISTS created_at TIMESTAMP DEFAULT NOW();
ALTER TABLE promotions ADD COLUMN IF NOT EXISTS updated_at TIMESTAMP DEFAULT NOW();

-- Add comments
COMMENT ON COLUMN promotions.is_active IS 'Enable/disable promotion without deleting';
COMMENT ON COLUMN promotions.priority IS 'Higher number = higher priority when multiple promotions apply';
COMMENT ON COLUMN promotions.usage_limit IS 'Max number of times this promotion can be used';
COMMENT ON COLUMN promotions.usage_count IS 'Current usage count';

-- Create index for performance
CREATE INDEX IF NOT EXISTS idx_promotions_active_dates 
ON promotions(startdate, enddate) 
WHERE is_active = TRUE;
```

### 2.3 Bảng `bookingpromotions` (Đã có - Không cần thay đổi)
```sql
CREATE TABLE bookingpromotions (
    bookingpromotionid SERIAL PRIMARY KEY,
    bookingid INTEGER NOT NULL,
    promotionid INTEGER NOT NULL,
    discountapplied DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (bookingid) REFERENCES bookings(bookingid),
    FOREIGN KEY (promotionid) REFERENCES promotions(promotionid)
);
```

---

## 3. API Endpoints Cần Phát Triển

### 3.1 Endpoints Hiện Có

#### ✅ GET /api/promotions/active
**Status**: ✅ ĐÃ CÓ  
**Purpose**: Lấy danh sách promotions đang active (cho banner)  
**Auth**: ❌ Public

**Response hiện tại:**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Active promotions retrieved successfully",
  "data": [
    {
      "promotionid": 1,
      "name": "Khuyến Mãi Tháng 11",
      "description": "Giảm 20% cho tất cả vé",
      "startdate": "2025-11-01",
      "enddate": "2025-11-30",
      "discounttype": "percentage",
      "discountvalue": 20.00
    }
  ]
}
```

**✅ KHÔNG CẦN THAY ĐỔI** - Endpoint này hoạt động tốt cho mục đích hiển thị banner

---

### 3.2 Endpoints Cần Tạo Mới

#### 🆕 POST /api/admin/promotions
**Status**: ⚠️ CẦN TẠO MỚI  
**Purpose**: Admin tạo promotion mới  
**Auth**: ✅ Admin/Manager only

**Request Body:**
```json
{
  "name": "Khuyến Mãi Tháng 11",
  "description": "Giảm 20% cho tất cả vé trong tháng 11",
  "startdate": "2025-11-01",
  "enddate": "2025-11-30",
  "discounttype": "percentage",
  "discountvalue": 20
}
```

**Response 201 Created:**
```json
{
  "success": true,
  "statusCode": 201,
  "message": "Promotion created successfully",
  "data": {
    "promotionid": 5,
    "name": "Khuyến Mãi Tháng 11",
    "description": "Giảm 20% cho tất cả vé trong tháng 11",
    "startdate": "2025-11-01",
    "enddate": "2025-11-30",
    "discounttype": "percentage",
    "discountvalue": 20.00,
    "isactive": true,
    "createdat": "2025-11-06T10:00:00Z"
  }
}
```

**Validation Rules:**
```csharp
- name: Required, MaxLength(100)
- description: Optional, MaxLength(255)
- startdate: Required, must be valid date
- enddate: Required, must be >= startdate
- discounttype: Required, must be "percentage" or "fixed"
- discountvalue: Required, > 0
  - If percentage: must be 1-100
  - If fixed: must be reasonable (5000-500000)
```

---

#### 🆕 GET /api/admin/promotions
**Status**: ⚠️ CẦN TẠO MỚI  
**Purpose**: Admin xem tất cả promotions (kể cả expired)  
**Auth**: ✅ Admin/Manager only

**Query Parameters:**
```
?page=1&pageSize=10&status=active
```

**Response 200 OK:**
```json
{
  "success": true,
  "statusCode": 200,
  "message": "Promotions retrieved successfully",
  "data": {
    "items": [
      {
        "promotionid": 1,
        "name": "Khuyến Mãi Tháng 11",
        "description": "Giảm 20% cho tất cả vé",
        "startdate": "2025-11-01",
        "enddate": "2025-11-30",
        "discounttype": "percentage",
        "discountvalue": 20.00,
        "isactive": true,
        "usagecount": 150,
        "usagelimit": null,
        "createdat": "2025-10-25T10:00:00Z"
      }
    ],
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 3,
    "totalItems": 25
  }
}
```

---

#### 🆕 PUT /api/admin/promotions/{id}
**Status**: ⚠️ CẦN TẠO MỚI  
**Purpose**: Admin cập nhật promotion  
**Auth**: ✅ Admin/Manager only

**Request Body:**
```json
{
  "name": "Khuyến Mãi Tháng 11 - EXTENDED",
  "description": "Giảm 25% cho tất cả vé (tăng từ 20%)",
  "enddate": "2025-12-15",
  "discountvalue": 25,
  "isactive": true
}
```

---

#### 🆕 DELETE /api/admin/promotions/{id}
**Status**: ⚠️ CẦN TẠO MỚI  
**Purpose**: Admin xóa/vô hiệu hóa promotion  
**Auth**: ✅ Admin/Manager only

**Soft Delete** (recommended):
```sql
UPDATE promotions SET is_active = FALSE WHERE promotionid = :id
```

---

### 3.3 Endpoints Cần Cập Nhật

#### 🔄 POST /api/bookings (CẬP NHẬT)
**Status**: ⚠️ CẦN CẬP NHẬT LOGIC  
**Purpose**: Thêm logic tự động áp dụng promotions

**Current Flow:**
```
1. User chọn ghế + combo
2. Calculate total
3. Create booking
4. Return booking
```

**New Flow:**
```
1. User chọn ghế + combo
2. Calculate initial total
3. ✨ Check active promotions (NEW)
4. ✨ Apply eligible promotions (NEW)
5. ✨ Insert into bookingpromotions (NEW)
6. Update booking total with discount
7. Return booking with promotions info
```

**Response cần thêm:**
```json
{
  "success": true,
  "data": {
    "bookingid": 123,
    "totalamount": 160000,
    "appliedPromotions": [  // ← NEW FIELD
      {
        "promotionid": 5,
        "name": "Khuyến Mãi Tháng 11",
        "discountapplied": 40000
      }
    ]
  }
}
```

---

#### 🔄 GET /api/bookings/{id} (CẬP NHẬT)
**Status**: ⚠️ CẦN CẬP NHẬT RESPONSE  
**Purpose**: Thêm thông tin promotions đã áp dụng

**Response cần thêm:**
```json
{
  "success": true,
  "data": {
    "bookingid": 123,
    "totalamount": 160000,
    "promotions": [  // ← NEW FIELD
      {
        "promotionid": 5,
        "name": "Khuyến Mãi Tháng 11",
        "description": "Giảm 20% cho tất cả vé",
        "discountapplied": 40000
      }
    ],
    "voucher": {
      "code": "STUDENT50",
      "discountapplied": 50000
    }
  }
}
```

---

## 4. Implementation Details

### 4.1 Backend Structure

#### Layer 1: Domain Layer
```
Movie88.Domain/
├── Models/
│   └── PromotionModel.cs (✅ ĐÃ CÓ)
├── Interfaces/
│   └── IPromotionRepository.cs (✅ ĐÃ CÓ - Cần thêm methods)
```

**Cần thêm vào `IPromotionRepository.cs`:**
```csharp
public interface IPromotionRepository
{
    // ✅ Existing
    Task<List<PromotionModel>> GetActivePromotionsAsync();
    
    // 🆕 New methods needed
    Task<PromotionModel?> GetByIdAsync(int promotionId, CancellationToken cancellationToken = default);
    Task<PromotionModel> CreateAsync(PromotionModel promotion, CancellationToken cancellationToken = default);
    Task<PromotionModel> UpdateAsync(PromotionModel promotion, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int promotionId, CancellationToken cancellationToken = default);
    Task<PagedResult<PromotionModel>> GetAllAsync(int page, int pageSize, string? status, CancellationToken cancellationToken = default);
    Task IncrementUsageCountAsync(int promotionId, CancellationToken cancellationToken = default);
}
```

---

#### Layer 2: Application Layer

**DTOs cần tạo:**

```csharp
// Movie88.Application/DTOs/Promotions/CreatePromotionDTO.cs
public class CreatePromotionDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    [Required]
    public DateOnly Startdate { get; set; }
    
    [Required]
    public DateOnly Enddate { get; set; }
    
    [Required]
    [RegularExpression("^(percentage|fixed)$")]
    public string Discounttype { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Discountvalue { get; set; }
}

// Movie88.Application/DTOs/Promotions/UpdatePromotionDTO.cs
public class UpdatePromotionDTO
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(255)]
    public string? Description { get; set; }
    
    public DateOnly? Enddate { get; set; }
    
    public decimal? Discountvalue { get; set; }
    
    public bool? IsActive { get; set; }
}

// Movie88.Application/DTOs/Promotions/AppliedPromotionDTO.cs
public class AppliedPromotionDTO
{
    public int Promotionid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Discountapplied { get; set; }
}
```

**Service methods cần thêm:**

```csharp
// Movie88.Application/Interfaces/IPromotionService.cs
public interface IPromotionService
{
    // ✅ Existing
    Task<Result<List<PromotionDTO>>> GetActivePromotionsAsync();
    
    // 🆕 New methods needed
    Task<Result<PromotionDTO>> CreatePromotionAsync(CreatePromotionDTO dto, CancellationToken cancellationToken = default);
    Task<Result<PromotionDTO>> UpdatePromotionAsync(int id, UpdatePromotionDTO dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeletePromotionAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PromotionDTO>>> GetAllPromotionsAsync(int page, int pageSize, string? status, CancellationToken cancellationToken = default);
    
    // 🔥 CRITICAL: Auto-apply logic
    Task<List<AppliedPromotionDTO>> ApplyEligiblePromotionsAsync(
        int bookingId, 
        decimal totalAmount, 
        CancellationToken cancellationToken = default);
}
```

---

#### Layer 3: Infrastructure Layer

**BookingPromotionRepository (MỚI):**

```csharp
// Movie88.Infrastructure/Repositories/BookingPromotionRepository.cs
public interface IBookingPromotionRepository
{
    Task<int> CreateAsync(int bookingId, int promotionId, decimal discountApplied, CancellationToken cancellationToken = default);
    Task<List<BookingPromotionModel>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
}

public class BookingPromotionRepository : IBookingPromotionRepository
{
    private readonly AppDbContext _context;

    public BookingPromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(int bookingId, int promotionId, decimal discountApplied, CancellationToken cancellationToken = default)
    {
        var entity = new Bookingpromotion
        {
            Bookingid = bookingId,
            Promotionid = promotionId,
            Discountapplied = discountApplied
        };

        _context.Bookingpromotions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Bookingpromotionid;
    }

    public async Task<List<BookingPromotionModel>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Bookingpromotions
            .Include(bp => bp.Promotion)
            .Where(bp => bp.Bookingid == bookingId)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new BookingPromotionModel
        {
            Bookingpromotionid = e.Bookingpromotionid,
            Bookingid = e.Bookingid,
            Promotionid = e.Promotionid,
            Discountapplied = e.Discountapplied,
            PromotionName = e.Promotion?.Name ?? ""
        }).ToList();
    }
}
```

---

#### Layer 4: WebApi Layer

**Controller mới:**

```csharp
// Movie88.WebApi/Controllers/Admin/AdminPromotionsController.cs
[ApiController]
[Route("api/admin/promotions")]
[Authorize(Roles = "Admin,Manager")]
public class AdminPromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public AdminPromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    /// <summary>
    /// Get all promotions (including expired) - Admin only
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
    {
        var result = await _promotionService.GetAllPromotionsAsync(page, pageSize, status);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Create new promotion - Admin only
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePromotionDTO dto)
    {
        var result = await _promotionService.CreatePromotionAsync(dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Update existing promotion - Admin only
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePromotionDTO dto)
    {
        var result = await _promotionService.UpdatePromotionAsync(id, dto);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// Delete/Deactivate promotion - Admin only
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _promotionService.DeletePromotionAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}
```

---

### 4.2 Core Logic: Auto-Apply Promotions

**Trong `BookingService.cs` (Cập nhật method CreateBooking):**

```csharp
public async Task<Result<BookingResponseDTO>> CreateBookingAsync(CreateBookingDTO dto, int customerId, CancellationToken cancellationToken = default)
{
    using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    
    try
    {
        // 1. Calculate initial total (seats + combos)
        decimal initialTotal = await CalculateInitialTotalAsync(dto);
        
        // 2. Create booking
        var booking = new Booking
        {
            Customerid = customerId,
            Showtimeid = dto.ShowtimeId,
            Totalamount = initialTotal,
            Status = "Pending",
            Bookingtime = DateTime.Now
        };
        
        await _bookingRepository.CreateAsync(booking, cancellationToken);
        
        // 3. Save seats and combos
        await SaveBookingDetailsAsync(booking.Bookingid, dto, cancellationToken);
        
        // 4. 🔥 AUTO-APPLY PROMOTIONS (NEW)
        var appliedPromotions = await _promotionService.ApplyEligiblePromotionsAsync(
            booking.Bookingid, 
            initialTotal, 
            cancellationToken);
        
        // 5. Calculate total discount from promotions
        decimal totalPromotionDiscount = appliedPromotions.Sum(p => p.Discountapplied);
        
        // 6. Update booking total amount
        booking.Totalamount = initialTotal - totalPromotionDiscount;
        await _bookingRepository.UpdateAsync(booking, cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        
        // 7. Return response with applied promotions
        return Result<BookingResponseDTO>.Success(new BookingResponseDTO
        {
            Bookingid = booking.Bookingid,
            Totalamount = booking.Totalamount,
            AppliedPromotions = appliedPromotions
        });
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync(cancellationToken);
        return Result<BookingResponseDTO>.Error($"Error creating booking: {ex.Message}");
    }
}
```

**Implement `ApplyEligiblePromotionsAsync` trong `PromotionService.cs`:**

```csharp
public async Task<List<AppliedPromotionDTO>> ApplyEligiblePromotionsAsync(
    int bookingId, 
    decimal totalAmount, 
    CancellationToken cancellationToken = default)
{
    var appliedPromotions = new List<AppliedPromotionDTO>();
    
    try
    {
        // 1. Get all active promotions
        var activePromotions = await _promotionRepository.GetActivePromotionsAsync();
        
        if (activePromotions == null || !activePromotions.Any())
        {
            return appliedPromotions; // No promotions to apply
        }
        
        // 2. Apply each eligible promotion
        foreach (var promotion in activePromotions)
        {
            // Calculate discount based on type
            decimal discount = CalculateDiscount(promotion, totalAmount);
            
            if (discount <= 0)
                continue;
            
            // 3. Insert into bookingpromotions table
            await _bookingPromotionRepository.CreateAsync(
                bookingId, 
                promotion.Promotionid, 
                discount, 
                cancellationToken);
            
            // 4. Increment usage count (if tracking)
            await _promotionRepository.IncrementUsageCountAsync(promotion.Promotionid, cancellationToken);
            
            // 5. Add to result list
            appliedPromotions.Add(new AppliedPromotionDTO
            {
                Promotionid = promotion.Promotionid,
                Name = promotion.Name,
                Description = promotion.Description,
                Discountapplied = discount
            });
        }
        
        _logger.LogInformation(
            "Applied {Count} promotions to booking {BookingId}. Total discount: {Discount}",
            appliedPromotions.Count,
            bookingId,
            appliedPromotions.Sum(p => p.Discountapplied));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error applying promotions to booking {BookingId}", bookingId);
        // Don't throw - promotions are optional
    }
    
    return appliedPromotions;
}

private decimal CalculateDiscount(PromotionModel promotion, decimal totalAmount)
{
    if (promotion.Discounttype == "percentage")
    {
        return totalAmount * (promotion.Discountvalue / 100);
    }
    else // "fixed"
    {
        return promotion.Discountvalue;
    }
}
```

---

## 5. Testing Guide

### 5.1 Test Cases

#### Test Case 1: Tạo Promotion (Admin)
```http
POST {{baseUrl}}/admin/promotions
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Khuyến Mãi Tháng 11",
  "description": "Giảm 20% cho tất cả vé",
  "startdate": "2025-11-01",
  "enddate": "2025-11-30",
  "discounttype": "percentage",
  "discountvalue": 20
}

### Expected: 201 Created
### Verify: Check promotions table
```

#### Test Case 2: Booking Tự Động Áp Promotion
```http
POST {{baseUrl}}/bookings
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "showtimeId": 567,
  "seatIds": [45, 46],
  "comboIds": []
}

### Expected: 200 OK
### Response should include:
{
  "bookingid": 123,
  "totalamount": 160000,  // 200k - 20% = 160k
  "appliedPromotions": [
    {
      "promotionid": 5,
      "name": "Khuyến Mãi Tháng 11",
      "discountapplied": 40000
    }
  ]
}

### Verify: 
- bookingpromotions table có record mới
- booking.totalamount = 160000
```

#### Test Case 3: GET Active Promotions (Public)
```http
GET {{baseUrl}}/promotions/active

### Expected: 200 OK
### Should return promotions with startdate <= today <= enddate
```

---

### 5.2 Test File

```http
# File: tests/Promotions.http

@baseUrl = https://localhost:7001/api
@adminToken = eyJhbG...
@customerToken = eyJhbG...

###############################################
# ADMIN: PROMOTION MANAGEMENT
###############################################

### 1. Create Promotion - Khuyến Mãi Tháng 11
POST {{baseUrl}}/admin/promotions
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "name": "Khuyến Mãi Tháng 11",
  "description": "Giảm 20% cho tất cả vé trong tháng 11",
  "startdate": "2025-11-01",
  "enddate": "2025-11-30",
  "discounttype": "percentage",
  "discountvalue": 20
}

### 2. Get All Promotions (Admin)
GET {{baseUrl}}/admin/promotions?page=1&pageSize=10
Authorization: Bearer {{adminToken}}

### 3. Update Promotion
PUT {{baseUrl}}/admin/promotions/1
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "discountvalue": 25,
  "description": "Tăng lên 25%!"
}

### 4. Delete Promotion
DELETE {{baseUrl}}/admin/promotions/1
Authorization: Bearer {{adminToken}}

###############################################
# PUBLIC: ACTIVE PROMOTIONS
###############################################

### 5. Get Active Promotions (Public)
GET {{baseUrl}}/promotions/active

###############################################
# CUSTOMER: BOOKING WITH AUTO-APPLY PROMOTION
###############################################

### 6. Create Booking (Auto-apply promotion)
POST {{baseUrl}}/bookings
Authorization: Bearer {{customerToken}}
Content-Type: application/json

{
  "showtimeId": 567,
  "seatIds": [45, 46],
  "comboIds": []
}

### 7. Get Booking Detail (Check applied promotions)
GET {{baseUrl}}/bookings/123
Authorization: Bearer {{customerToken}}
```

---

## 6. Deployment Checklist

### 6.1 Pre-Deployment
- [ ] Run database migration script
- [ ] Update Entity Framework entities
- [ ] Implement all new endpoints
- [ ] Add unit tests for PromotionService
- [ ] Add integration tests for BookingService with promotions
- [ ] Update Swagger documentation

### 6.2 Deployment
- [ ] Deploy database changes to Supabase
- [ ] Deploy backend API to Railway
- [ ] Test in staging environment
- [ ] Verify promotion auto-apply works
- [ ] Check performance with multiple promotions

### 6.3 Post-Deployment
- [ ] Create sample promotions via admin panel
- [ ] Test end-to-end booking flow
- [ ] Monitor logs for errors
- [ ] Document API changes for frontend team

---

## 7. Tóm Tắt

### ✅ Endpoints Cần Tạo
1. **POST** `/api/admin/promotions` - Tạo promotion
2. **GET** `/api/admin/promotions` - List promotions (admin)
3. **PUT** `/api/admin/promotions/{id}` - Update promotion
4. **DELETE** `/api/admin/promotions/{id}` - Delete promotion

### 🔄 Endpoints Cần Cập Nhật
1. **POST** `/api/bookings` - Thêm logic auto-apply promotions
2. **GET** `/api/bookings/{id}` - Thêm field `appliedPromotions`

### ✅ Endpoints Không Đổi
1. **GET** `/api/promotions/active` - Giữ nguyên

### 🗄️ Database
- ✅ `promotions` table đã có đủ fields cơ bản
- ⚠️ Cần thêm `is_active`, `priority`, `usage_count` (optional)
- ✅ `bookingpromotions` table đã sẵn sàng

### 🔥 Core Logic
- Implement `ApplyEligiblePromotionsAsync()` trong `PromotionService`
- Update `CreateBookingAsync()` trong `BookingService`
- Create `BookingPromotionRepository` mới

---

**Next Steps**: Xem [Promotions-Frontend-Guide.md](./Promotions-Frontend-Guide.md) để implement phía Android app.

---

**Status**: 📋 **Ready for Development**  
**Estimated Effort**: 2-3 days  
**Priority**: HIGH 🔥
