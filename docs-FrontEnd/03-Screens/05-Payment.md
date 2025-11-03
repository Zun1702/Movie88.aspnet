# 💳 Payment Screens

## Tổng quan

Chi tiết các màn hình thanh toán: BookingSummaryActivity, VNPayWebViewActivity, PaymentResultActivity.

---

## 1️⃣ BookingSummaryActivity

### Purpose
Hiển thị tóm tắt đặt vé trước khi thanh toán, áp dụng voucher.

### Layout: activity_booking_summary.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:background="@color/backgroundColor">

    <!-- Toolbar -->
    <androidx.appcompat.widget.Toolbar
        android:id="@+id/toolbar"
        android:layout_width="match_parent"
        android:layout_height="?attr/actionBarSize"
        android:background="@color/backgroundColor"
        android:elevation="4dp"
        app:title="@string/booking_summary_title"
        app:navigationIcon="@drawable/ic_back"/>

    <ScrollView
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:padding="@dimen/spacing_base">

            <!-- Movie Info Card -->
            <com.google.android.material.card.MaterialCardView
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base">

                    <!-- Movie Poster -->
                    <ImageView
                        android:id="@+id/ivPoster"
                        android:layout_width="80dp"
                        android:layout_height="120dp"
                        android:scaleType="centerCrop"
                        android:background="@drawable/bg_poster"/>

                    <!-- Movie Details -->
                    <LinearLayout
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:orientation="vertical">

                        <TextView
                            android:id="@+id/tvMovieTitle"
                            style="@style/Text.Heading3"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:text="Avengers: Endgame"/>

                        <TextView
                            android:id="@+id/tvCinema"
                            style="@style/Text.BodySecondary"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:text="CGV Vincom Center"/>

                        <TextView
                            android:id="@+id/tvShowtime"
                            style="@style/Text.Body"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:text="19:30 - 29/10/2025"/>

                        <TextView
                            android:id="@+id/tvAuditorium"
                            style="@style/Text.BodySecondary"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:text="Phòng 3"/>

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Seats Info -->
            <com.google.android.material.card.MaterialCardView
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base">

                    <TextView
                        style="@style/Text.Heading3"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="@string/booking_summary_seats"/>

                    <TextView
                        android:id="@+id/tvSeats"
                        style="@style/Text.Body"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:text="A1, A2, A3"/>

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="@dimen/divider_height"
                        android:layout_marginVertical="@dimen/spacing_sm"
                        android:background="@color/divider"/>

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="Giá vé"/>

                        <TextView
                            android:id="@+id/tvTicketPrice"
                            style="@style/Text.Body"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:text="240.000 đ"/>

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Combos Info -->
            <com.google.android.material.card.MaterialCardView
                android:id="@+id/cardCombos"
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base"
                android:visibility="gone">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base">

                    <TextView
                        style="@style/Text.Heading3"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="@string/booking_summary_combos"/>

                    <androidx.recyclerview.widget.RecyclerView
                        android:id="@+id/rvCombos"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:nestedScrollingEnabled="false"/>

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="@dimen/divider_height"
                        android:layout_marginVertical="@dimen/spacing_sm"
                        android:background="@color/divider"/>

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="Giá combo"/>

                        <TextView
                            android:id="@+id/tvComboPrice"
                            style="@style/Text.Body"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:text="80.000 đ"/>

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Voucher Section -->
            <com.google.android.material.card.MaterialCardView
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base">

                    <TextView
                        style="@style/Text.Heading3"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="@string/booking_summary_voucher"/>

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:orientation="horizontal">

                        <com.google.android.material.textfield.TextInputLayout
                            style="@style/TextInputLayout"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:hint="@string/voucher_enter_code">

                            <com.google.android.material.textfield.TextInputEditText
                                android:id="@+id/etVoucherCode"
                                style="@style/TextInputEditText"
                                android:layout_width="match_parent"
                                android:layout_height="wrap_content"
                                android:inputType="textCapCharacters"
                                android:maxLines="1"/>

                        </com.google.android.material.textfield.TextInputLayout>

                        <com.google.android.material.button.MaterialButton
                            android:id="@+id/btnApplyVoucher"
                            style="@style/Button.Primary"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginStart="@dimen/spacing_sm"
                            android:text="@string/voucher_apply"/>

                    </LinearLayout>

                    <!-- Applied Voucher Info -->
                    <LinearLayout
                        android:id="@+id/layoutAppliedVoucher"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:visibility="gone">

                        <ImageView
                            android:layout_width="20dp"
                            android:layout_height="20dp"
                            android:src="@drawable/ic_check"
                            app:tint="@color/statusSuccess"/>

                        <TextView
                            android:id="@+id/tvVoucherName"
                            style="@style/Text.BodySecondary"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:layout_marginStart="@dimen/spacing_xs"
                            android:text="Giảm 20% tối đa 50.000đ"/>

                        <ImageView
                            android:id="@+id/btnRemoveVoucher"
                            android:layout_width="20dp"
                            android:layout_height="20dp"
                            android:src="@drawable/ic_close"
                            app:tint="@color/textSecondary"/>

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Price Summary -->
            <com.google.android.material.card.MaterialCardView
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base">

                    <!-- Subtotal -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="Tạm tính"/>

                        <TextView
                            android:id="@+id/tvSubtotal"
                            style="@style/Text.Body"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:text="320.000 đ"/>

                    </LinearLayout>

                    <!-- Discount -->
                    <LinearLayout
                        android:id="@+id/layoutDiscount"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:orientation="horizontal"
                        android:visibility="gone">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/booking_summary_discount"/>

                        <TextView
                            android:id="@+id/tvDiscount"
                            style="@style/Text.Body"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textColor="@color/statusSuccess"
                            android:text="- 50.000 đ"/>

                    </LinearLayout>

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="@dimen/divider_height"
                        android:layout_marginVertical="@dimen/spacing_sm"
                        android:background="@color/divider"/>

                    <!-- Total -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.Heading3"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/booking_summary_total"/>

                        <TextView
                            android:id="@+id/tvTotal"
                            style="@style/Text.Heading2"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textColor="@color/colorPrimary"
                            android:text="270.000 đ"/>

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

        </LinearLayout>

    </ScrollView>

    <!-- Payment Button -->
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_base"
        android:background="@color/backgroundCard"
        android:elevation="8dp"
        android:orientation="vertical">

        <!-- Time Remaining -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_marginBottom="@dimen/spacing_sm"
            android:orientation="horizontal"
            android:gravity="center_vertical">

            <ImageView
                android:layout_width="16dp"
                android:layout_height="16dp"
                android:src="@drawable/ic_timer"
                app:tint="@color/colorPrimary"/>

            <TextView
                android:id="@+id/tvTimeRemaining"
                style="@style/Text.BodySecondary"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginStart="@dimen/spacing_xs"
                android:text="Thời gian còn lại: 14:35"/>

        </LinearLayout>

        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnPayment"
            style="@style/Button.Primary"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:text="@string/booking_summary_payment"
            app:icon="@drawable/ic_payment"
            app:iconGravity="textStart"/>

    </LinearLayout>

</LinearLayout>
```

### Java Implementation: BookingSummaryActivity.java

```java
package com.movie88.ui.payment;

import android.content.Intent;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.card.MaterialCardView;
import com.movie88.R;
import com.movie88.data.models.request.CreateBookingRequest;
import com.movie88.utils.Constants;
import com.movie88.utils.CurrencyUtils;

public class BookingSummaryActivity extends AppCompatActivity {

    // Views
    private Toolbar toolbar;
    private ImageView ivPoster;
    private TextView tvMovieTitle, tvCinema, tvShowtime, tvAuditorium;
    private TextView tvSeats, tvTicketPrice, tvComboPrice, tvSubtotal, tvDiscount, tvTotal;
    private TextView tvTimeRemaining, tvVoucherName;
    private EditText etVoucherCode;
    private MaterialButton btnApplyVoucher, btnPayment;
    private LinearLayout layoutAppliedVoucher, layoutDiscount;
    private ImageView btnRemoveVoucher;
    private MaterialCardView cardCombos;
    private RecyclerView rvCombos;

    // ViewModel
    private BookingSummaryViewModel viewModel;

    // Data
    private int movieId, showtimeId;
    private String bookingId;
    private long expiryTime;
    private CountDownTimer countDownTimer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_booking_summary);

        // Get data from intent
        movieId = getIntent().getIntExtra(Constants.EXTRA_MOVIE_ID, 0);
        showtimeId = getIntent().getIntExtra(Constants.EXTRA_SHOWTIME_ID, 0);
        bookingId = getIntent().getStringExtra(Constants.EXTRA_BOOKING_ID);
        expiryTime = getIntent().getLongExtra("EXPIRY_TIME", 0);

        initViews();
        setupToolbar();
        setupViewModel();
        setupListeners();
        startCountdown();

        // Load booking details
        viewModel.loadBookingDetails(bookingId);
    }

    private void initViews() {
        toolbar = findViewById(R.id.toolbar);
        ivPoster = findViewById(R.id.ivPoster);
        tvMovieTitle = findViewById(R.id.tvMovieTitle);
        tvCinema = findViewById(R.id.tvCinema);
        tvShowtime = findViewById(R.id.tvShowtime);
        tvAuditorium = findViewById(R.id.tvAuditorium);
        tvSeats = findViewById(R.id.tvSeats);
        tvTicketPrice = findViewById(R.id.tvTicketPrice);
        tvComboPrice = findViewById(R.id.tvComboPrice);
        tvSubtotal = findViewById(R.id.tvSubtotal);
        tvDiscount = findViewById(R.id.tvDiscount);
        tvTotal = findViewById(R.id.tvTotal);
        tvTimeRemaining = findViewById(R.id.tvTimeRemaining);
        tvVoucherName = findViewById(R.id.tvVoucherName);
        etVoucherCode = findViewById(R.id.etVoucherCode);
        btnApplyVoucher = findViewById(R.id.btnApplyVoucher);
        btnPayment = findViewById(R.id.btnPayment);
        layoutAppliedVoucher = findViewById(R.id.layoutAppliedVoucher);
        layoutDiscount = findViewById(R.id.layoutDiscount);
        btnRemoveVoucher = findViewById(R.id.btnRemoveVoucher);
        cardCombos = findViewById(R.id.cardCombos);
        rvCombos = findViewById(R.id.rvCombos);
    }

    private void setupToolbar() {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(BookingSummaryViewModel.class);

        // Observe booking details
        viewModel.getBookingDetails().observe(this, booking -> {
            if (booking != null) {
                displayBookingDetails(booking);
            }
        });

        // Observe voucher application
        viewModel.getAppliedVoucher().observe(this, voucher -> {
            if (voucher != null) {
                layoutAppliedVoucher.setVisibility(View.VISIBLE);
                layoutDiscount.setVisibility(View.VISIBLE);
                tvVoucherName.setText(voucher.getName());
                tvDiscount.setText("- " + CurrencyUtils.formatCurrency(voucher.getDiscountAmount()));
                calculateTotal();
            }
        });

        // Observe payment creation
        viewModel.getPaymentUrl().observe(this, paymentUrl -> {
            if (paymentUrl != null) {
                // Navigate to VNPay WebView
                Intent intent = new Intent(this, VNPayWebViewActivity.class);
                intent.putExtra("PAYMENT_URL", paymentUrl);
                intent.putExtra(Constants.EXTRA_BOOKING_ID, bookingId);
                startActivity(intent);
                finish();
            }
        });

        // Observe errors
        viewModel.getError().observe(this, error -> {
            if (error != null) {
                Toast.makeText(this, error, Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void setupListeners() {
        // Apply voucher
        btnApplyVoucher.setOnClickListener(v -> {
            String code = etVoucherCode.getText().toString().trim();
            if (!code.isEmpty()) {
                viewModel.applyVoucher(code, bookingId);
            }
        });

        // Remove voucher
        btnRemoveVoucher.setOnClickListener(v -> {
            viewModel.removeVoucher();
            layoutAppliedVoucher.setVisibility(View.GONE);
            layoutDiscount.setVisibility(View.GONE);
            etVoucherCode.setText("");
            calculateTotal();
        });

        // Payment button
        btnPayment.setOnClickListener(v -> {
            viewModel.createPayment(bookingId);
        });
    }

    private void displayBookingDetails(BookingDetails booking) {
        // Load poster
        Glide.with(this)
            .load(booking.getMovie().getPosterUrl())
            .into(ivPoster);

        // Set text fields
        tvMovieTitle.setText(booking.getMovie().getTitle());
        tvCinema.setText(booking.getCinema().getName());
        tvShowtime.setText(booking.getShowtime().getStartTime());
        tvAuditorium.setText("Phòng " + booking.getAuditorium().getName());
        tvSeats.setText(booking.getSeatsDisplay());
        tvTicketPrice.setText(CurrencyUtils.formatCurrency(booking.getTicketPrice()));
        
        // Combos
        if (booking.getCombos() != null && !booking.getCombos().isEmpty()) {
            cardCombos.setVisibility(View.VISIBLE);
            tvComboPrice.setText(CurrencyUtils.formatCurrency(booking.getComboPrice()));
            // Setup combos RecyclerView
        } else {
            cardCombos.setVisibility(View.GONE);
        }

        calculateTotal();
    }

    private void calculateTotal() {
        double subtotal = viewModel.getSubtotal();
        double discount = viewModel.getDiscount();
        double total = subtotal - discount;

        tvSubtotal.setText(CurrencyUtils.formatCurrency(subtotal));
        tvTotal.setText(CurrencyUtils.formatCurrency(total));
    }

    private void startCountdown() {
        long remainingTime = expiryTime - System.currentTimeMillis();
        
        countDownTimer = new CountDownTimer(remainingTime, 1000) {
            @Override
            public void onTick(long millisUntilFinished) {
                long minutes = (millisUntilFinished / 1000) / 60;
                long seconds = (millisUntilFinished / 1000) % 60;
                tvTimeRemaining.setText(String.format("Thời gian còn lại: %02d:%02d", 
                    minutes, seconds));
            }

            @Override
            public void onFinish() {
                // Booking expired
                Toast.makeText(BookingSummaryActivity.this, 
                    "Hết thời gian giữ ghế", Toast.LENGTH_LONG).show();
                finish();
            }
        };
        countDownTimer.start();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        if (countDownTimer != null) {
            countDownTimer.cancel();
        }
    }
}
```

---

## 2️⃣ VNPayWebViewActivity

### Purpose
Hiển thị trang thanh toán VNPay trong WebView, intercept callback URL.

### Layout: activity_vnpay_webview.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<RelativeLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <!-- Toolbar -->
    <androidx.appcompat.widget.Toolbar
        android:id="@+id/toolbar"
        android:layout_width="match_parent"
        android:layout_height="?attr/actionBarSize"
        android:background="@color/backgroundColor"
        android:elevation="4dp"/>

    <!-- WebView -->
    <WebView
        android:id="@+id/webView"
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:layout_below="@id/toolbar"/>

    <!-- Loading Progress -->
    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_centerInParent="true"/>

</RelativeLayout>
```

### Java Implementation: VNPayWebViewActivity.java

```java
package com.movie88.ui.payment;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.webkit.WebResourceRequest;
import android.webkit.WebSettings;
import android:webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import com.movie88.R;
import com.movie88.utils.Constants;

public class VNPayWebViewActivity extends AppCompatActivity {

    private Toolbar toolbar;
    private WebView webView;
    private ProgressBar progressBar;
    
    private String bookingId;
    private String callbackUrl;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_vnpay_webview);

        // Get payment URL and booking ID
        String paymentUrl = getIntent().getStringExtra("PAYMENT_URL");
        bookingId = getIntent().getStringExtra(Constants.EXTRA_BOOKING_ID);
        callbackUrl = Constants.VNPAY_RETURN_URL;

        initViews();
        setupToolbar();
        setupWebView(paymentUrl);
    }

    private void initViews() {
        toolbar = findViewById(R.id.toolbar);
        webView = findViewById(R.id.webView);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupToolbar() {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setTitle("Thanh toán VNPay");
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> {
            // Show confirmation dialog before closing
            new androidx.appcompat.app.AlertDialog.Builder(this)
                .setTitle("Hủy thanh toán")
                .setMessage("Bạn có chắc muốn hủy thanh toán?")
                .setPositiveButton("Có", (dialog, which) -> {
                    finish();
                })
                .setNegativeButton("Không", null)
                .show();
        });
    }

    private void setupWebView(String paymentUrl) {
        // Enable JavaScript
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setDomStorageEnabled(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setUseWideViewPort(true);

        // WebViewClient to intercept URL loading
        webView.setWebViewClient(new WebViewClient() {
            
            @Override
            public void onPageFinished(WebView view, String url) {
                progressBar.setVisibility(View.GONE);
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                String url = request.getUrl().toString();
                
                // Check if callback URL
                if (url.startsWith(callbackUrl)) {
                    // Extract payment result from URL parameters
                    Uri uri = Uri.parse(url);
                    String responseCode = uri.getQueryParameter("vnp_ResponseCode");
                    String transactionNo = uri.getQueryParameter("vnp_TransactionNo");
                    String amount = uri.getQueryParameter("vnp_Amount");
                    
                    // Navigate to payment result screen
                    Intent intent = new Intent(VNPayWebViewActivity.this, 
                        PaymentResultActivity.class);
                    intent.putExtra("RESPONSE_CODE", responseCode);
                    intent.putExtra("TRANSACTION_NO", transactionNo);
                    intent.putExtra("AMOUNT", amount);
                    intent.putExtra(Constants.EXTRA_BOOKING_ID, bookingId);
                    startActivity(intent);
                    finish();
                    
                    return true;
                }
                
                // Load URL in WebView
                return false;
            }
        });

        // Load payment URL
        webView.loadUrl(paymentUrl);
    }

    @Override
    public void onBackPressed() {
        if (webView.canGoBack()) {
            webView.goBack();
        } else {
            // Show confirmation dialog
            new androidx.appcompat.app.AlertDialog.Builder(this)
                .setTitle("Hủy thanh toán")
                .setMessage("Bạn có chắc muốn hủy thanh toán?")
                .setPositiveButton("Có", (dialog, which) -> {
                    super.onBackPressed();
                })
                .setNegativeButton("Không", null)
                .show();
        }
    }
}
```

---

## 3️⃣ PaymentResultActivity

### Purpose
Hiển thị kết quả thanh toán, QR code vé, thông tin booking.

### Layout: activity_payment_result.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:background="@color/backgroundColor">

    <ScrollView
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:padding="@dimen/spacing_lg"
            android:gravity="center_horizontal">

            <!-- Success/Failure Icon -->
            <ImageView
                android:id="@+id/ivStatus"
                android:layout_width="120dp"
                android:layout_height="120dp"
                android:src="@drawable/ic_success"
                app:tint="@color/statusSuccess"/>

            <!-- Status Title -->
            <TextView
                android:id="@+id/tvStatusTitle"
                style="@style/Text.Heading1"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginTop="@dimen/spacing_base"
                android:text="@string/payment_success"/>

            <!-- Status Message -->
            <TextView
                android:id="@+id/tvStatusMessage"
                style="@style/Text.BodySecondary"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginTop="@dimen/spacing_sm"
                android:textAlignment="center"
                android:text="Vé của bạn đã được đặt thành công!"/>

            <!-- Booking Code Card -->
            <com.google.android.material.card.MaterialCardView
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginTop="@dimen/spacing_lg">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_horizontal">

                    <TextView
                        style="@style/Text.BodySecondary"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="@string/payment_booking_code"/>

                    <TextView
                        android:id="@+id/tvBookingCode"
                        style="@style/Text.Heading1"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:textColor="@color/colorPrimary"
                        android:letterSpacing="0.1"
                        android:text="BOOK123456"/>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- QR Code Card -->
            <com.google.android.material.card.MaterialCardView
                android:id="@+id/cardQRCode"
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginTop="@dimen/spacing_base">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_horizontal">

                    <TextView
                        style="@style/Text.Heading3"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="@string/payment_qr_code"/>

                    <ImageView
                        android:id="@+id/ivQRCode"
                        android:layout_width="@dimen/qr_code_size_medium"
                        android:layout_height="@dimen/qr_code_size_medium"
                        android:layout_marginTop="@dimen/spacing_base"
                        android:background="@color/textPrimary"
                        android:padding="8dp"/>

                    <TextView
                        style="@style/Text.Caption"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:textAlignment="center"
                        android:text="Vui lòng xuất trình mã QR này tại rạp"/>

                    <com.google.android.material.button.MaterialButton
                        android:id="@+id/btnSaveQR"
                        style="@style/Button.Outlined"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:text="@string/payment_save_qr"
                        app:icon="@drawable/ic_download"/>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Booking Details Card -->
            <com.google.android.material.card.MaterialCardView
                style="@style/CardView"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginTop="@dimen/spacing_base">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_base">

                    <TextView
                        style="@style/Text.Heading3"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginBottom="@dimen/spacing_sm"
                        android:text="Thông tin đặt vé"/>

                    <!-- Movie Title -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="100dp"
                            android:layout_height="wrap_content"
                            android:text="Phim"/>

                        <TextView
                            android:id="@+id/tvMovieTitle"
                            style="@style/Text.Body"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="Avengers: Endgame"/>

                    </LinearLayout>

                    <!-- Cinema -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="100dp"
                            android:layout_height="wrap_content"
                            android:text="Rạp"/>

                        <TextView
                            android:id="@+id/tvCinema"
                            style="@style/Text.Body"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="CGV Vincom Center"/>

                    </LinearLayout>

                    <!-- Showtime -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="100dp"
                            android:layout_height="wrap_content"
                            android:text="Suất chiếu"/>

                        <TextView
                            android:id="@+id/tvShowtime"
                            style="@style/Text.Body"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="19:30 - 29/10/2025"/>

                    </LinearLayout>

                    <!-- Seats -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="100dp"
                            android:layout_height="wrap_content"
                            android:text="Ghế"/>

                        <TextView
                            android:id="@+id/tvSeats"
                            style="@style/Text.Body"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="A1, A2, A3"/>

                    </LinearLayout>

                    <!-- Total Amount -->
                    <View
                        android:layout_width="match_parent"
                        android:layout_height="@dimen/divider_height"
                        android:layout_marginVertical="@dimen/spacing_sm"
                        android:background="@color/divider"/>

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            style="@style/Text.Heading3"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="Tổng tiền"/>

                        <TextView
                            android:id="@+id/tvTotalAmount"
                            style="@style/Text.Heading2"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textColor="@color/colorPrimary"
                            android:text="270.000 đ"/>

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

        </LinearLayout>

    </ScrollView>

    <!-- Action Buttons -->
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal"
        android:padding="@dimen/spacing_base"
        android:background="@color/backgroundCard"
        android:elevation="8dp">

        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnBackHome"
            style="@style/Button.Secondary"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_weight="1"
            android:text="@string/payment_back_home"/>

        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnViewTicket"
            style="@style/Button.Primary"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_weight="1"
            android:layout_marginStart="@dimen/spacing_sm"
            android:text="@string/payment_view_ticket"/>

    </LinearLayout>

</LinearLayout>
```

### Java Implementation: PaymentResultActivity.java

```java
package com.movie88.ui.payment;

import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;

import com.google.android.material.button.MaterialButton;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.MultiFormatWriter;
import com.google.zxing.common.BitMatrix;
import com.journeyapps.barcodescanner.BarcodeEncoder;
import com.movie88.R;
import com.movie88.ui.MainActivity;
import com.movie88.ui.booking.BookingDetailActivity;
import com.movie88.utils.Constants;
import com.movie88.utils.CurrencyUtils;

public class PaymentResultActivity extends AppCompatActivity {

    private ImageView ivStatus, ivQRCode;
    private TextView tvStatusTitle, tvStatusMessage, tvBookingCode;
    private TextView tvMovieTitle, tvCinema, tvShowtime, tvSeats, tvTotalAmount;
    private MaterialButton btnSaveQR, btnBackHome, btnViewTicket;

    private PaymentResultViewModel viewModel;
    private String bookingId;
    private boolean isSuccess;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_payment_result);

        // Get payment result from intent
        String responseCode = getIntent().getStringExtra("RESPONSE_CODE");
        bookingId = getIntent().getStringExtra(Constants.EXTRA_BOOKING_ID);
        isSuccess = "00".equals(responseCode);

        initViews();
        setupViewModel();
        setupListeners();

        // Display result
        displayPaymentResult(isSuccess);

        // Confirm payment with backend
        if (isSuccess) {
            viewModel.confirmPayment(bookingId);
        }
    }

    private void initViews() {
        ivStatus = findViewById(R.id.ivStatus);
        ivQRCode = findViewById(R.id.ivQRCode);
        tvStatusTitle = findViewById(R.id.tvStatusTitle);
        tvStatusMessage = findViewById(R.id.tvStatusMessage);
        tvBookingCode = findViewById(R.id.tvBookingCode);
        tvMovieTitle = findViewById(R.id.tvMovieTitle);
        tvCinema = findViewById(R.id.tvCinema);
        tvShowtime = findViewById(R.id.tvShowtime);
        tvSeats = findViewById(R.id.tvSeats);
        tvTotalAmount = findViewById(R.id.tvTotalAmount);
        btnSaveQR = findViewById(R.id.btnSaveQR);
        btnBackHome = findViewById(R.id.btnBackHome);
        btnViewTicket = findViewById(R.id.btnViewTicket);
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(PaymentResultViewModel.class);

        // Observe booking details
        viewModel.getBookingDetails().observe(this, booking -> {
            if (booking != null) {
                displayBookingDetails(booking);
                generateQRCode(booking.getBookingCode());
            }
        });
    }

    private void setupListeners() {
        btnBackHome.setOnClickListener(v -> {
            Intent intent = new Intent(this, MainActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
            startActivity(intent);
            finish();
        });

        btnViewTicket.setOnClickListener(v -> {
            Intent intent = new Intent(this, BookingDetailActivity.class);
            intent.putExtra(Constants.EXTRA_BOOKING_ID, bookingId);
            startActivity(intent);
            finish();
        });

        btnSaveQR.setOnClickListener(v -> {
            // Save QR code to gallery
            saveQRCodeToGallery();
        });
    }

    private void displayPaymentResult(boolean success) {
        if (success) {
            ivStatus.setImageResource(R.drawable.ic_success);
            ivStatus.setColorFilter(getColor(R.color.statusSuccess));
            tvStatusTitle.setText(R.string.payment_success);
            tvStatusMessage.setText("Vé của bạn đã được đặt thành công!");
        } else {
            ivStatus.setImageResource(R.drawable.ic_error);
            ivStatus.setColorFilter(getColor(R.color.statusError));
            tvStatusTitle.setText(R.string.payment_failed);
            tvStatusMessage.setText("Thanh toán không thành công. Vui lòng thử lại.");
            
            // Hide QR code section
            findViewById(R.id.cardQRCode).setVisibility(View.GONE);
            btnViewTicket.setVisibility(View.GONE);
        }
    }

    private void displayBookingDetails(BookingDetails booking) {
        tvBookingCode.setText(booking.getBookingCode());
        tvMovieTitle.setText(booking.getMovie().getTitle());
        tvCinema.setText(booking.getCinema().getName());
        tvShowtime.setText(booking.getShowtime().getStartTime());
        tvSeats.setText(booking.getSeatsDisplay());
        tvTotalAmount.setText(CurrencyUtils.formatCurrency(booking.getTotalAmount()));
    }

    private void generateQRCode(String bookingCode) {
        try {
            MultiFormatWriter writer = new MultiFormatWriter();
            BitMatrix bitMatrix = writer.encode(bookingCode, 
                BarcodeFormat.QR_CODE, 512, 512);
            BarcodeEncoder encoder = new BarcodeEncoder();
            Bitmap bitmap = encoder.createBitmap(bitMatrix);
            ivQRCode.setImageBitmap(bitmap);
        } catch (Exception e) {
            e.printStackTrace();
            Toast.makeText(this, "Lỗi tạo mã QR", Toast.LENGTH_SHORT).show();
        }
    }

    private void saveQRCodeToGallery() {
        // Implementation to save QR code to gallery
        Toast.makeText(this, "Đã lưu mã QR", Toast.LENGTH_SHORT).show();
    }

    @Override
    public void onBackPressed() {
        // Prevent back navigation, force user to use buttons
        Intent intent = new Intent(this, MainActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        startActivity(intent);
        finish();
    }
}
```

---

## API Endpoints Used

**BookingSummaryActivity:**
- `GET /api/bookings/{id}` - Lấy thông tin booking (movie, cinema, showtime, seats, combos)
- `POST /api/vouchers/validate` - Kiểm tra mã voucher hợp lệ
- `POST /api/bookings/{id}/apply-voucher` - Áp dụng voucher vào booking
- `POST /api/payments/vnpay/create` - Tạo payment URL của VNPay

**VNPayWebViewActivity:**
- `GET /api/payments/vnpay/callback` - VNPay callback URL (được handle tự động bởi WebView)
- Intercepted URL contains: `vnp_ResponseCode`, `vnp_TransactionNo`, `vnp_Amount`

**PaymentResultActivity:**
- `POST /api/payments/vnpay/ipn` - IPN notification từ VNPay (được gọi tự động bởi VNPay)
- `PUT /api/payments/{id}/confirm` - Xác nhận thanh toán thành công
- `GET /api/bookings/{id}` - Lấy chi tiết booking sau thanh toán

---

### API Interface Example
```java
public interface ApiService {
    
    // Get booking details
    @GET("api/bookings/{id}")
    Call<ApiResponse<Booking>> getBookingById(
        @Header("Authorization") String token,
        @Path("id") int bookingId
    );
    
    // Validate voucher
    @POST("api/vouchers/validate")
    Call<ApiResponse<VoucherValidation>> validateVoucher(
        @Header("Authorization") String token,
        @Body ValidateVoucherRequest request  // { voucherCode, bookingId }
    );
    
    // Apply voucher to booking
    @POST("api/bookings/{id}/apply-voucher")
    Call<ApiResponse<Booking>> applyVoucher(
        @Header("Authorization") String token,
        @Path("id") int bookingId,
        @Body ApplyVoucherRequest request  // { voucherCode }
    );
    
    // Create VNPay payment
    @POST("api/payments/vnpay/create")
    Call<ApiResponse<VNPayPaymentResponse>> createVNPayPayment(
        @Header("Authorization") String token,
        @Body CreatePaymentRequest request  // { bookingId, amount, returnUrl }
    );
    
    // VNPay callback (handled by VNPay, not directly called by app)
    @GET("api/payments/vnpay/callback")
    Call<Void> vnpayCallback(
        @QueryMap Map<String, String> params
    );
    
    // Confirm payment
    @PUT("api/payments/{id}/confirm")
    Call<ApiResponse<Payment>> confirmPayment(
        @Header("Authorization") String token,
        @Path("id") int paymentId
    );
}
```

---

**Dependencies:**
```gradle
// QR Code generation
implementation 'com.google.zxing:core:3.5.2'
implementation 'com.journeyapps:zxing-android-embedded:4.3.0'
```

**Last Updated**: October 29, 2025
