# 🎁 Promotions Feature - Quick Start Guide

**Created**: November 6, 2025  
**Approach**: 🗄️ **Database Seeding** (No Admin Panel)

---

## 📝 Tóm Tắt Ngắn Gọn

### Cách Triển Khai
✅ **Đổ database trực tiếp** - Không làm admin CRUD  
✅ **Auto-apply promotions** - Tự động giảm giá khi booking  
✅ **Display banner** - Hiển thị trên HomeFragment

### Tại Sao Không Làm Admin Panel?
- ⏰ **Thời gian hạn chế** - Không đủ thời gian làm admin UI
- 🗄️ **Database seeding đơn giản hơn** - Insert SQL trực tiếp
- 🎯 **Focus vào core features** - Tập trung vào booking flow + display

---

## 🚀 Hướng Dẫn Nhanh

### Step 1: Đổ Database (5 phút)
```sql
-- Run file: database/seed-data/02-SEED-PROMOTIONS.sql
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES 
    ('Khuyến Mãi Tháng 11', 'Giảm 20% cho tất cả vé', '2025-11-01', '2025-11-30', 'percentage', 20),
    ('Black Friday Cinema', 'Giảm 30%', '2025-11-29', '2025-11-29', 'percentage', 30),
    ('Opening Week', 'Giảm 50k', '2025-11-01', '2025-11-07', 'fixed', 50000);
```

✅ **Xong phần database!**

---

### Step 2: Backend Implementation (2-3 giờ)

#### 2.1 Tạo Repository Mới
```csharp
// Movie88.Infrastructure/Repositories/BookingPromotionRepository.cs
public class BookingPromotionRepository : IBookingPromotionRepository
{
    public async Task<int> CreateAsync(int bookingId, int promotionId, decimal discountApplied)
    {
        var entity = new Bookingpromotion
        {
            Bookingid = bookingId,
            Promotionid = promotionId,
            Discountapplied = discountApplied
        };
        _context.Bookingpromotions.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Bookingpromotionid;
    }
}
```

#### 2.2 Implement Auto-Apply Logic
```csharp
// Movie88.Application/Services/PromotionService.cs
public async Task<List<AppliedPromotionDTO>> ApplyEligiblePromotionsAsync(int bookingId, decimal totalAmount)
{
    var activePromotions = await _promotionRepository.GetActivePromotionsAsync();
    var appliedPromotions = new List<AppliedPromotionDTO>();
    
    foreach (var promotion in activePromotions)
    {
        decimal discount = CalculateDiscount(promotion, totalAmount);
        await _bookingPromotionRepository.CreateAsync(bookingId, promotion.Promotionid, discount);
        appliedPromotions.Add(new AppliedPromotionDTO
        {
            Promotionid = promotion.Promotionid,
            Name = promotion.Name,
            Discountapplied = discount
        });
    }
    
    return appliedPromotions;
}
```

#### 2.3 Update BookingService
```csharp
// Movie88.Application/Services/BookingService.cs - CreateBookingAsync()
// Thêm logic sau khi tạo booking:

var appliedPromotions = await _promotionService.ApplyEligiblePromotionsAsync(booking.Bookingid, initialTotal);
decimal totalDiscount = appliedPromotions.Sum(p => p.Discountapplied);
booking.Totalamount = initialTotal - totalDiscount;
await _bookingRepository.UpdateAsync(booking);
```

✅ **Xong phần backend!**

---

### Step 3: Frontend Implementation (4-6 giờ)

#### 3.1 Models (30 phút)
```java
// Promotion.java
public class Promotion {
    private int promotionId;
    private String name;
    private String description;
    private String startDate;
    private String endDate;
    private String discountType; // "percentage" or "fixed"
    private double discountValue;
}

// AppliedPromotion.java
public class AppliedPromotion {
    private int promotionId;
    private String name;
    private double discountApplied;
}
```

#### 3.2 HomeFragment - Banner (2 giờ)
```java
// Load promotions
apiService.getActivePromotions().enqueue(new Callback<ApiResponse<List<Promotion>>>() {
    @Override
    public void onResponse(...) {
        bannerAdapter.updateData(response.body().getData());
        startAutoScroll(); // 3-second rotation
    }
});
```

#### 3.3 PaymentSummaryActivity - Display Discount (2 giờ)
```java
// Display applied promotions
if (booking.hasPromotions()) {
    tvOriginalPrice.setText(formatPrice(originalPrice));
    tvPromotionDiscount.setText("-" + formatPrice(totalDiscount));
    tvFinalTotal.setText(formatPrice(booking.getTotalAmount()));
    tvTotalSavings.setText("Bạn tiết kiệm: " + formatPrice(totalDiscount));
}
```

✅ **Xong phần frontend!**

---

## 📊 API Flow

### 1. Display Banner (HomeFragment)
```
GET /api/promotions/active
→ Return active promotions
→ Show in banner carousel
```

### 2. Booking with Auto-Apply
```
POST /api/bookings
→ Backend checks active promotions
→ Auto-apply eligible discounts
→ Insert into bookingpromotions
→ Return booking with appliedPromotions
```

### 3. View Booking Detail
```
GET /api/bookings/{id}
→ Return booking with promotions info
→ Show savings breakdown
```

---

## 🗂️ File Structure

```
Backend:
├── database/seed-data/02-SEED-PROMOTIONS.sql ✅ NEW
├── Movie88.Application/
│   ├── DTOs/Promotions/AppliedPromotionDTO.cs ✅ NEW
│   └── Services/PromotionService.cs (update)
├── Movie88.Infrastructure/
│   └── Repositories/BookingPromotionRepository.cs ✅ NEW
└── Movie88.WebApi/
    └── (No new controllers - skip admin panel)

Frontend:
├── app/src/main/java/com/movie88/
│   ├── models/
│   │   ├── Promotion.java ✅ NEW
│   │   └── AppliedPromotion.java ✅ NEW
│   ├── adapters/
│   │   ├── PromotionBannerAdapter.java ✅ NEW
│   │   └── AppliedPromotionsAdapter.java ✅ NEW
│   ├── fragments/
│   │   └── HomeFragment.java (update)
│   └── activities/
│       └── PaymentSummaryActivity.java (update)
└── app/src/main/res/layout/
    ├── item_promotion_banner.xml ✅ NEW
    └── item_applied_promotion.xml ✅ NEW
```

---

## ✅ Testing Checklist

### Database
- [ ] Run `02-SEED-PROMOTIONS.sql`
- [ ] Verify: `SELECT * FROM promotions WHERE CURRENT_DATE BETWEEN startdate AND enddate;`

### Backend
- [ ] Test: GET `/api/promotions/active` → Returns promotions
- [ ] Test: POST `/api/bookings` → Auto-applies discount
- [ ] Verify: `bookingpromotions` table has records

### Frontend
- [ ] HomeFragment shows banner carousel
- [ ] Banner auto-scrolls every 3 seconds
- [ ] PaymentSummaryActivity shows discount breakdown
- [ ] Savings message displays correctly

---

## 📚 Chi Tiết Documentation

### Backend
📄 [Promotions-Backend-Implementation.md](./Promotions-Backend-Implementation.md)
- Database migration
- Auto-apply logic implementation
- API endpoint updates
- Testing guide

### Frontend
📄 [Promotions-Frontend-Guide.md](./Promotions-Frontend-Guide.md)
- Android components
- UI layouts
- Screen implementations
- Best practices

---

## 🎯 Key Points

### ✅ Làm Gì?
1. Đổ promotions vào database (SQL file)
2. Backend auto-apply khi booking
3. Frontend display banner + discount

### ❌ Không Làm Gì?
1. ~~Admin CRUD endpoints~~ - Không cần
2. ~~Admin panel UI~~ - Không cần
3. ~~Update/Delete promotions từ app~~ - Không cần

### 🔥 Core Feature
**AUTO-APPLY PROMOTIONS** = Tự động giảm giá khi user booking

---

## ⏱️ Time Estimate

| Task | Time |
|------|------|
| Database seeding | 5 mins |
| Backend implementation | 2-3 hours |
| Frontend implementation | 4-6 hours |
| Testing | 1 hour |
| **Total** | **7-10 hours** |

---

**Status**: 📋 **Ready to Start**  
**Last Updated**: November 6, 2025
