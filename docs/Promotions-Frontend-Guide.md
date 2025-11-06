# 🎁 Promotions Feature - Frontend Implementation Guide

**Created**: November 6, 2025  
**Status**: 📋 **PLANNING - Ready for Implementation**  
**Platform**: Android (Java)  
**Purpose**: Hiển thị promotions và xử lý discount tự động

---

## 📋 Mục Lục

1. [Tổng Quan](#1-tổng-quan)
2. [API Integration](#2-api-integration)
3. [UI Components](#3-ui-components)
4. [Screen Implementation](#4-screen-implementation)
5. [Testing Guide](#5-testing-guide)
6. [Best Practices](#6-best-practices)

---

## 1. Tổng Quan

### 1.1 Mục Tiêu
- ✅ Hiển thị banner promotions trên HomeFragment
- ✅ Show discount đã áp dụng khi booking
- ✅ Hiển thị tiết kiệm được bao nhiêu
- ✅ User-friendly UX

### 1.2 User Journey
```
1. User mở app → HomeFragment
   ↓
2. Thấy banner "Khuyến Mãi Tháng 11 - Giảm 20%"
   ↓
3. Click chọn phim → SelectSeatActivity
   ↓
4. Chọn ghế → Thấy giá: 200,000 VND
   ↓
5. Click "Tiếp tục" → PaymentSummaryActivity
   ↓
6. Thấy:
   - Giá gốc: 200,000 VND
   - 🎁 Khuyến mãi: -40,000 VND
   - Tổng: 160,000 VND
   ↓
7. Thanh toán → Success
```

---

## 2. API Integration

### 2.1 API Service Interface

```java
// File: app/src/main/java/com/movie88/api/ApiService.java

public interface ApiService {
    
    // ========== PROMOTIONS ==========
    
    /**
     * Get active promotions for banner display
     * Endpoint: GET /api/promotions/active
     * Auth: Not required
     */
    @GET("api/promotions/active")
    Call<ApiResponse<List<Promotion>>> getActivePromotions();
    
    // ========== BOOKINGS (Updated) ==========
    
    /**
     * Create booking (auto-apply promotions)
     * Endpoint: POST /api/bookings
     * Auth: Required
     * 
     * Response now includes appliedPromotions field
     */
    @POST("api/bookings")
    Call<ApiResponse<BookingResponse>> createBooking(
        @Header("Authorization") String token,
        @Body CreateBookingRequest request
    );
    
    /**
     * Get booking detail
     * Endpoint: GET /api/bookings/{id}
     * Auth: Required
     * 
     * Response now includes promotions field
     */
    @GET("api/bookings/{id}")
    Call<ApiResponse<BookingDetail>> getBookingDetail(
        @Header("Authorization") String token,
        @Path("id") int bookingId
    );
}
```

---

### 2.2 Data Models

#### Promotion Model
```java
// File: app/src/main/java/com/movie88/models/Promotion.java

public class Promotion {
    @SerializedName("promotionid")
    private int promotionId;
    
    @SerializedName("name")
    private String name;
    
    @SerializedName("description")
    private String description;
    
    @SerializedName("startdate")
    private String startDate; // "2025-11-01"
    
    @SerializedName("enddate")
    private String endDate;   // "2025-11-30"
    
    @SerializedName("discounttype")
    private String discountType; // "percentage" or "fixed"
    
    @SerializedName("discountvalue")
    private double discountValue; // 20 (for 20%)
    
    // Getters & Setters
    public int getPromotionId() { return promotionId; }
    public void setPromotionId(int promotionId) { this.promotionId = promotionId; }
    
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    
    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }
    
    public String getStartDate() { return startDate; }
    public void setStartDate(String startDate) { this.startDate = startDate; }
    
    public String getEndDate() { return endDate; }
    public void setEndDate(String endDate) { this.endDate = endDate; }
    
    public String getDiscountType() { return discountType; }
    public void setDiscountType(String discountType) { this.discountType = discountType; }
    
    public double getDiscountValue() { return discountValue; }
    public void setDiscountValue(double discountValue) { this.discountValue = discountValue; }
    
    /**
     * Get formatted discount text
     * Example: "20%", "50,000đ"
     */
    public String getFormattedDiscount() {
        if ("percentage".equals(discountType)) {
            return String.format("%.0f%%", discountValue);
        } else {
            return String.format("%,dđ", (int)discountValue);
        }
    }
}
```

#### Applied Promotion Model (NEW)
```java
// File: app/src/main/java/com/movie88/models/AppliedPromotion.java

public class AppliedPromotion {
    @SerializedName("promotionid")
    private int promotionId;
    
    @SerializedName("name")
    private String name;
    
    @SerializedName("description")
    private String description;
    
    @SerializedName("discountapplied")
    private double discountApplied; // Actual discount amount
    
    // Getters & Setters
    public int getPromotionId() { return promotionId; }
    public void setPromotionId(int promotionId) { this.promotionId = promotionId; }
    
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    
    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }
    
    public double getDiscountApplied() { return discountApplied; }
    public void setDiscountApplied(double discountApplied) { this.discountApplied = discountApplied; }
    
    /**
     * Get formatted discount
     * Example: "-40,000đ"
     */
    public String getFormattedDiscount() {
        return String.format("-%,dđ", (int)discountApplied);
    }
}
```

#### Booking Response (UPDATED)
```java
// File: app/src/main/java/com/movie88/models/BookingResponse.java

public class BookingResponse {
    @SerializedName("bookingid")
    private int bookingId;
    
    @SerializedName("totalamount")
    private double totalAmount;
    
    @SerializedName("status")
    private String status;
    
    // 🆕 NEW FIELD - Applied promotions
    @SerializedName("appliedPromotions")
    private List<AppliedPromotion> appliedPromotions;
    
    // ... other fields ...
    
    // Getters & Setters
    public List<AppliedPromotion> getAppliedPromotions() { 
        return appliedPromotions != null ? appliedPromotions : new ArrayList<>(); 
    }
    
    public void setAppliedPromotions(List<AppliedPromotion> appliedPromotions) { 
        this.appliedPromotions = appliedPromotions; 
    }
    
    /**
     * Check if any promotions were applied
     */
    public boolean hasPromotions() {
        return appliedPromotions != null && !appliedPromotions.isEmpty();
    }
    
    /**
     * Get total promotion discount
     */
    public double getTotalPromotionDiscount() {
        if (appliedPromotions == null) return 0;
        double total = 0;
        for (AppliedPromotion promo : appliedPromotions) {
            total += promo.getDiscountApplied();
        }
        return total;
    }
}
```

---

## 3. UI Components

### 3.1 Promotion Banner Item Layout

```xml
<!-- File: app/src/main/res/layout/item_promotion_banner.xml -->
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="160dp"
    android:layout_margin="8dp"
    app:cardCornerRadius="12dp"
    app:cardElevation="4dp">
    
    <androidx.constraintlayout.widget.ConstraintLayout
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:background="@drawable/gradient_promotion_background"
        android:padding="16dp">
        
        <!-- Promotion Icon -->
        <ImageView
            android:id="@+id/ivPromotionIcon"
            android:layout_width="48dp"
            android:layout_height="48dp"
            android:src="@drawable/ic_promotion"
            android:tint="@color/white"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toTopOf="parent"/>
        
        <!-- Promotion Title -->
        <TextView
            android:id="@+id/tvPromotionTitle"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginStart="12dp"
            android:text="Khuyến Mãi Tháng 11"
            android:textColor="@color/white"
            android:textSize="18sp"
            android:textStyle="bold"
            android:maxLines="1"
            android:ellipsize="end"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toEndOf="@id/ivPromotionIcon"
            app:layout_constraintTop_toTopOf="@id/ivPromotionIcon"/>
        
        <!-- Discount Badge -->
        <TextView
            android:id="@+id/tvDiscountBadge"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:layout_marginStart="12dp"
            android:layout_marginTop="4dp"
            android:background="@drawable/badge_discount"
            android:padding="6dp"
            android:text="20%"
            android:textColor="@color/colorPrimary"
            android:textSize="16sp"
            android:textStyle="bold"
            app:layout_constraintStart_toEndOf="@id/ivPromotionIcon"
            app:layout_constraintTop_toBottomOf="@id/tvPromotionTitle"/>
        
        <!-- Description -->
        <TextView
            android:id="@+id/tvPromotionDescription"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="12dp"
            android:text="Giảm 20% cho tất cả vé trong tháng 11"
            android:textColor="@color/white"
            android:textSize="14sp"
            android:alpha="0.9"
            android:maxLines="2"
            android:ellipsize="end"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/tvDiscountBadge"/>
        
        <!-- Date Range -->
        <TextView
            android:id="@+id/tvDateRange"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:drawableStart="@drawable/ic_calendar"
            android:drawablePadding="6dp"
            android:drawableTint="@color/white"
            android:text="01/11 - 30/11"
            android:textColor="@color/white"
            android:textSize="12sp"
            android:alpha="0.8"
            app:layout_constraintBottom_toBottomOf="parent"
            app:layout_constraintStart_toStartOf="parent"/>
        
    </androidx.constraintlayout.widget.ConstraintLayout>
    
</com.google.android.material.card.MaterialCardView>
```

### 3.2 Promotion Banner Adapter

```java
// File: app/src/main/java/com/movie88/adapters/PromotionBannerAdapter.java

public class PromotionBannerAdapter extends RecyclerView.Adapter<PromotionBannerAdapter.ViewHolder> {
    
    private List<Promotion> promotions;
    private Context context;
    
    public PromotionBannerAdapter(Context context, List<Promotion> promotions) {
        this.context = context;
        this.promotions = promotions;
    }
    
    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_promotion_banner, parent, false);
        return new ViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        Promotion promotion = promotions.get(position);
        holder.bind(promotion);
    }
    
    @Override
    public int getItemCount() {
        return promotions.size();
    }
    
    public void updateData(List<Promotion> newPromotions) {
        this.promotions = newPromotions;
        notifyDataSetChanged();
    }
    
    static class ViewHolder extends RecyclerView.ViewHolder {
        TextView tvTitle, tvDescription, tvDiscountBadge, tvDateRange;
        ImageView ivIcon;
        
        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            tvTitle = itemView.findViewById(R.id.tvPromotionTitle);
            tvDescription = itemView.findViewById(R.id.tvPromotionDescription);
            tvDiscountBadge = itemView.findViewById(R.id.tvDiscountBadge);
            tvDateRange = itemView.findViewById(R.id.tvDateRange);
            ivIcon = itemView.findViewById(R.id.ivPromotionIcon);
        }
        
        public void bind(Promotion promotion) {
            tvTitle.setText(promotion.getName());
            tvDescription.setText(promotion.getDescription());
            tvDiscountBadge.setText(promotion.getFormattedDiscount());
            
            // Format date range
            String dateRange = formatDateRange(promotion.getStartDate(), promotion.getEndDate());
            tvDateRange.setText(dateRange);
        }
        
        private String formatDateRange(String startDate, String endDate) {
            try {
                SimpleDateFormat inputFormat = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());
                SimpleDateFormat outputFormat = new SimpleDateFormat("dd/MM", Locale.getDefault());
                
                Date start = inputFormat.parse(startDate);
                Date end = inputFormat.parse(endDate);
                
                return outputFormat.format(start) + " - " + outputFormat.format(end);
            } catch (Exception e) {
                return startDate + " - " + endDate;
            }
        }
    }
}
```

---

## 4. Screen Implementation

### 4.1 HomeFragment (Banner Carousel)

```xml
<!-- File: app/src/main/res/layout/fragment_home.xml -->
<!-- Add this section after the header -->

<com.google.android.material.card.MaterialCardView
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_margin="16dp"
    app:cardCornerRadius="12dp"
    app:cardElevation="2dp">
    
    <!-- Banner Title -->
    <TextView
        android:id="@+id/tvPromotionsBannerTitle"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="12dp"
        android:text="🎉 Khuyến Mãi Đặc Biệt"
        android:textSize="16sp"
        android:textStyle="bold"
        android:textColor="@color/textPrimary"
        android:background="@color/cardBackground"/>
    
    <!-- ViewPager2 for auto-scroll banner -->
    <androidx.viewpager2.widget.ViewPager2
        android:id="@+id/vpPromotionBanner"
        android:layout_width="match_parent"
        android:layout_height="180dp"
        android:layout_marginTop="48dp"/>
    
    <!-- Indicator -->
    <com.tbuonomo.viewpagerdotsindicator.DotsIndicator
        android:id="@+id/dotsIndicator"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="bottom|center_horizontal"
        android:layout_marginBottom="8dp"
        app:dotsColor="@color/textSecondary"
        app:dotsCornerRadius="4dp"
        app:dotsSize="8dp"
        app:dotsSpacing="4dp"
        app:selectedDotColor="@color/colorPrimary"/>
    
</com.google.android.material.card.MaterialCardView>
```

```java
// File: app/src/main/java/com/movie88/fragments/HomeFragment.java

public class HomeFragment extends Fragment {
    
    private ViewPager2 vpPromotionBanner;
    private DotsIndicator dotsIndicator;
    private PromotionBannerAdapter bannerAdapter;
    private Handler autoScrollHandler;
    private Runnable autoScrollRunnable;
    
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_home, container, false);
        
        initViews(view);
        setupPromotionBanner();
        loadActivePromotions();
        
        return view;
    }
    
    private void initViews(View view) {
        vpPromotionBanner = view.findViewById(R.id.vpPromotionBanner);
        dotsIndicator = view.findViewById(R.id.dotsIndicator);
    }
    
    private void setupPromotionBanner() {
        bannerAdapter = new PromotionBannerAdapter(getContext(), new ArrayList<>());
        vpPromotionBanner.setAdapter(bannerAdapter);
        dotsIndicator.setViewPager2(vpPromotionBanner);
        
        // Setup auto-scroll
        autoScrollHandler = new Handler(Looper.getMainLooper());
        autoScrollRunnable = new Runnable() {
            @Override
            public void run() {
                int currentItem = vpPromotionBanner.getCurrentItem();
                int totalItems = bannerAdapter.getItemCount();
                
                if (totalItems > 0) {
                    int nextItem = (currentItem + 1) % totalItems;
                    vpPromotionBanner.setCurrentItem(nextItem, true);
                }
                
                autoScrollHandler.postDelayed(this, 3000); // Auto-scroll every 3 seconds
            }
        };
    }
    
    private void loadActivePromotions() {
        ApiService apiService = RetrofitClient.getInstance().create(ApiService.class);
        
        apiService.getActivePromotions().enqueue(new Callback<ApiResponse<List<Promotion>>>() {
            @Override
            public void onResponse(Call<ApiResponse<List<Promotion>>> call, Response<ApiResponse<List<Promotion>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<Promotion> promotions = response.body().getData();
                    
                    if (promotions != null && !promotions.isEmpty()) {
                        bannerAdapter.updateData(promotions);
                        startAutoScroll();
                    } else {
                        // Hide banner if no promotions
                        vpPromotionBanner.setVisibility(View.GONE);
                    }
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<List<Promotion>>> call, Throwable t) {
                Log.e("HomeFragment", "Failed to load promotions", t);
                // Hide banner on error
                vpPromotionBanner.setVisibility(View.GONE);
            }
        });
    }
    
    private void startAutoScroll() {
        autoScrollHandler.postDelayed(autoScrollRunnable, 3000);
    }
    
    private void stopAutoScroll() {
        if (autoScrollHandler != null && autoScrollRunnable != null) {
            autoScrollHandler.removeCallbacks(autoScrollRunnable);
        }
    }
    
    @Override
    public void onResume() {
        super.onResume();
        startAutoScroll();
    }
    
    @Override
    public void onPause() {
        super.onPause();
        stopAutoScroll();
    }
}
```

---

### 4.2 PaymentSummaryActivity (Display Applied Promotions)

```xml
<!-- File: app/src/main/res/layout/activity_payment_summary.xml -->
<!-- Add this section after seat/combo details -->

<!-- Applied Promotions Section -->
<LinearLayout
    android:id="@+id/layoutAppliedPromotions"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:orientation="vertical"
    android:padding="16dp"
    android:background="@drawable/bg_promotion_applied"
    android:layout_margin="16dp"
    android:visibility="gone">
    
    <TextView
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="🎁 Khuyến mãi đã áp dụng"
        android:textSize="16sp"
        android:textStyle="bold"
        android:textColor="@color/colorPrimary"
        android:layout_marginBottom="12dp"/>
    
    <!-- Promotions RecyclerView -->
    <androidx.recyclerview.widget.RecyclerView
        android:id="@+id/rvAppliedPromotions"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:nestedScrollingEnabled="false"/>
    
    <!-- Total Savings -->
    <View
        android:layout_width="match_parent"
        android:layout_height="1dp"
        android:background="@color/divider"
        android:layout_marginVertical="8dp"/>
    
    <TextView
        android:id="@+id/tvTotalSavings"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="Bạn tiết kiệm: 40,000đ"
        android:textSize="14sp"
        android:textStyle="bold"
        android:textColor="@color/success"
        android:gravity="end"/>
    
</LinearLayout>

<!-- Price Summary -->
<LinearLayout
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:orientation="vertical"
    android:padding="16dp">
    
    <!-- Original Price -->
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal">
        
        <TextView
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_weight="1"
            android:text="Giá gốc"
            android:textSize="14sp"
            android:textColor="@color/textSecondary"/>
        
        <TextView
            android:id="@+id/tvOriginalPrice"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="200,000đ"
            android:textSize="14sp"
            android:textColor="@color/textSecondary"/>
    </LinearLayout>
    
    <!-- Promotion Discount -->
    <LinearLayout
        android:id="@+id/layoutPromotionDiscount"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal"
        android:layout_marginTop="8dp"
        android:visibility="gone">
        
        <TextView
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_weight="1"
            android:text="Giảm giá khuyến mãi"
            android:textSize="14sp"
            android:textColor="@color/colorPrimary"/>
        
        <TextView
            android:id="@+id/tvPromotionDiscount"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="-40,000đ"
            android:textSize="14sp"
            android:textColor="@color/colorPrimary"
            android:textStyle="bold"/>
    </LinearLayout>
    
    <View
        android:layout_width="match_parent"
        android:layout_height="1dp"
        android:background="@color/divider"
        android:layout_marginVertical="12dp"/>
    
    <!-- Final Total -->
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal">
        
        <TextView
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_weight="1"
            android:text="Tổng thanh toán"
            android:textSize="18sp"
            android:textStyle="bold"
            android:textColor="@color/textPrimary"/>
        
        <TextView
            android:id="@+id/tvFinalTotal"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="160,000đ"
            android:textSize="20sp"
            android:textStyle="bold"
            android:textColor="@color/colorPrimary"/>
    </LinearLayout>
    
</LinearLayout>
```

```java
// File: app/src/main/java/com/movie88/activities/PaymentSummaryActivity.java

public class PaymentSummaryActivity extends AppCompatActivity {
    
    private LinearLayout layoutAppliedPromotions;
    private RecyclerView rvAppliedPromotions;
    private TextView tvTotalSavings, tvOriginalPrice, tvPromotionDiscount, tvFinalTotal;
    private LinearLayout layoutPromotionDiscount;
    
    private BookingResponse bookingResponse;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_payment_summary);
        
        initViews();
        loadBookingData();
    }
    
    private void initViews() {
        layoutAppliedPromotions = findViewById(R.id.layoutAppliedPromotions);
        rvAppliedPromotions = findViewById(R.id.rvAppliedPromotions);
        tvTotalSavings = findViewById(R.id.tvTotalSavings);
        tvOriginalPrice = findViewById(R.id.tvOriginalPrice);
        tvPromotionDiscount = findViewById(R.id.tvPromotionDiscount);
        tvFinalTotal = findViewById(R.id.tvFinalTotal);
        layoutPromotionDiscount = findViewById(R.id.layoutPromotionDiscount);
    }
    
    private void loadBookingData() {
        // Get booking ID from intent
        int bookingId = getIntent().getIntExtra("bookingId", -1);
        
        ApiService apiService = RetrofitClient.getInstance().create(ApiService.class);
        String token = "Bearer " + SharedPrefsHelper.getAccessToken(this);
        
        apiService.getBookingDetail(token, bookingId).enqueue(new Callback<ApiResponse<BookingDetail>>() {
            @Override
            public void onResponse(Call<ApiResponse<BookingDetail>> call, Response<ApiResponse<BookingDetail>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    BookingDetail booking = response.body().getData();
                    displayBookingDetails(booking);
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<BookingDetail>> call, Throwable t) {
                Toast.makeText(PaymentSummaryActivity.this, "Failed to load booking", Toast.LENGTH_SHORT).show();
            }
        });
    }
    
    private void displayBookingDetails(BookingDetail booking) {
        // Calculate original price
        double promotionDiscount = 0;
        if (booking.getAppliedPromotions() != null && !booking.getAppliedPromotions().isEmpty()) {
            for (AppliedPromotion promo : booking.getAppliedPromotions()) {
                promotionDiscount += promo.getDiscountApplied();
            }
        }
        
        double originalPrice = booking.getTotalAmount() + promotionDiscount;
        double finalTotal = booking.getTotalAmount();
        
        // Display prices
        tvOriginalPrice.setText(formatPrice(originalPrice));
        tvFinalTotal.setText(formatPrice(finalTotal));
        
        // Show promotion details if any
        if (booking.hasPromotions()) {
            layoutAppliedPromotions.setVisibility(View.VISIBLE);
            layoutPromotionDiscount.setVisibility(View.VISIBLE);
            
            tvPromotionDiscount.setText(String.format("-%s", formatPrice(promotionDiscount)));
            tvTotalSavings.setText(String.format("Bạn tiết kiệm: %s", formatPrice(promotionDiscount)));
            
            // Setup RecyclerView for applied promotions
            AppliedPromotionsAdapter adapter = new AppliedPromotionsAdapter(this, booking.getAppliedPromotions());
            rvAppliedPromotions.setLayoutManager(new LinearLayoutManager(this));
            rvAppliedPromotions.setAdapter(adapter);
        }
    }
    
    private String formatPrice(double price) {
        return String.format(Locale.getDefault(), "%,dđ", (int)price);
    }
}
```

---

### 4.3 Applied Promotions Adapter (Small List)

```java
// File: app/src/main/java/com/movie88/adapters/AppliedPromotionsAdapter.java

public class AppliedPromotionsAdapter extends RecyclerView.Adapter<AppliedPromotionsAdapter.ViewHolder> {
    
    private Context context;
    private List<AppliedPromotion> promotions;
    
    public AppliedPromotionsAdapter(Context context, List<AppliedPromotion> promotions) {
        this.context = context;
        this.promotions = promotions;
    }
    
    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_applied_promotion, parent, false);
        return new ViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        AppliedPromotion promotion = promotions.get(position);
        holder.bind(promotion);
    }
    
    @Override
    public int getItemCount() {
        return promotions.size();
    }
    
    static class ViewHolder extends RecyclerView.ViewHolder {
        TextView tvName, tvDiscount;
        ImageView ivIcon;
        
        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            tvName = itemView.findViewById(R.id.tvPromotionName);
            tvDiscount = itemView.findViewById(R.id.tvPromotionDiscount);
            ivIcon = itemView.findViewById(R.id.ivPromotionIcon);
        }
        
        public void bind(AppliedPromotion promotion) {
            tvName.setText(promotion.getName());
            tvDiscount.setText(promotion.getFormattedDiscount());
        }
    }
}
```

```xml
<!-- File: app/src/main/res/layout/item_applied_promotion.xml -->
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:orientation="horizontal"
    android:padding="8dp"
    android:gravity="center_vertical">
    
    <ImageView
        android:id="@+id/ivPromotionIcon"
        android:layout_width="24dp"
        android:layout_height="24dp"
        android:src="@drawable/ic_discount"
        android:tint="@color/colorPrimary"/>
    
    <TextView
        android:id="@+id/tvPromotionName"
        android:layout_width="0dp"
        android:layout_height="wrap_content"
        android:layout_weight="1"
        android:layout_marginStart="12dp"
        android:text="Khuyến Mãi Tháng 11"
        android:textSize="14sp"
        android:textColor="@color/textPrimary"/>
    
    <TextView
        android:id="@+id/tvPromotionDiscount"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="-40,000đ"
        android:textSize="14sp"
        android:textStyle="bold"
        android:textColor="@color/colorPrimary"/>
    
</LinearLayout>
```

---

## 5. Testing Guide

### 5.1 Manual Testing Checklist

#### Test Case 1: Banner Display
```
✓ Open app → HomeFragment
✓ Check banner displays promotions
✓ Verify auto-scroll works (3 seconds)
✓ Check indicator dots update
✓ Verify data: name, description, discount, dates
```

#### Test Case 2: Booking with Promotion
```
✓ Select movie → seats → combos
✓ Go to PaymentSummaryActivity
✓ Verify "Khuyến mãi đã áp dụng" section shows
✓ Check discount amount is correct
✓ Verify final total = original - discount
✓ Check "Bạn tiết kiệm" displays correctly
```

#### Test Case 3: Multiple Promotions
```
✓ Create booking with 2+ promotions active
✓ Verify all promotions listed
✓ Check total discount = sum of all discounts
✓ Verify booking total is correct
```

---

### 5.2 Edge Cases

#### No Active Promotions
```
Expected: Banner hides gracefully
Action: No promotions in date range
Result: vpPromotionBanner.setVisibility(View.GONE)
```

#### Network Error
```
Expected: Banner hides, no crash
Action: API call fails
Result: Show fallback UI or hide banner
```

#### Promotion Expired During Booking
```
Expected: Show error message
Action: Promotion ends before payment
Result: Backend returns 400, show "Promotion no longer available"
```

---

## 6. Best Practices

### 6.1 Performance

```java
// Use ViewHolder pattern
// Cache formatted strings
private String cachedFormattedDiscount;

public String getFormattedDiscount() {
    if (cachedFormattedDiscount == null) {
        cachedFormattedDiscount = formatDiscount();
    }
    return cachedFormattedDiscount;
}

// Use DiffUtil for RecyclerView updates
public class PromotionDiffCallback extends DiffUtil.Callback {
    // Implement for efficient updates
}
```

### 6.2 Error Handling

```java
// Graceful degradation
try {
    loadPromotions();
} catch (Exception e) {
    Log.e(TAG, "Failed to load promotions", e);
    // Hide banner, don't crash
    hidePromotionBanner();
}
```

### 6.3 User Experience

```
✅ Show loading indicator while fetching
✅ Cache promotions for offline view
✅ Animate banner transitions
✅ Use clear, concise text
✅ Highlight savings prominently
```

---

## 7. Tóm Tắt

### ✅ Components Cần Tạo
1. **Models**: `Promotion`, `AppliedPromotion`
2. **Adapters**: `PromotionBannerAdapter`, `AppliedPromotionsAdapter`
3. **Layouts**: `item_promotion_banner.xml`, `item_applied_promotion.xml`
4. **Updates**: HomeFragment, PaymentSummaryActivity

### 🎨 UI Elements
- Banner carousel with auto-scroll
- Promotion details display
- Discount summary
- Savings highlight

### 🔗 API Integration
- GET `/api/promotions/active`
- Updated BookingResponse with `appliedPromotions`

---

**Status**: 📋 **Ready for Development**  
**Estimated Effort**: 2-3 days  
**Priority**: HIGH 🔥  
**Dependencies**: Backend implementation must be done first

---

**Xem thêm**: [Promotions-Backend-Implementation.md](./Promotions-Backend-Implementation.md)
