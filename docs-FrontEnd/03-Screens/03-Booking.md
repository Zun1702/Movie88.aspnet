# 📱 Screen Specifications - Booking Flow

## Booking Flow Overview

Quy trình đặt vé được chia thành **5 bước riêng biệt**, mỗi màn hình thực hiện 1 chức năng cụ thể:

```
1. SelectCinemaActivity (Chọn rạp & suất chiếu)
   ↓
2. SelectSeatActivity (Chọn ghế)
   ↓
3. SelectComboActivity (Chọn combo bắp nước - Optional)
   ↓
4. BookingSummaryActivity (Xác nhận & áp voucher)
   ↓
5. VNPayWebViewActivity → PaymentResultActivity (Thanh toán)
```

**Navigation Pattern**: Mỗi bước chuyển tiếp bằng `Intent`, truyền `bookingId` qua `EXTRA_BOOKING_ID`.

---

## 1️⃣ SelectCinemaActivity

### Purpose
Chọn rạp chiếu và suất chiếu cụ thể cho phim.

### Flow
1. Load danh sách rạp có chiếu phim này
2. Chọn rạp → Load suất chiếu theo rạp
3. Chọn ngày → Filter suất chiếu theo ngày
4. Chọn suất chiếu cụ thể → Navigate to SelectSeatActivity

### Layout: `activity_select_cinema.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.coordinatorlayout.widget.CoordinatorLayout
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/background">

    <!-- App Bar -->
    <com.google.android.material.appbar.AppBarLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:background="@color/surface"
        app:elevation="@dimen/elevation_small">

        <com.google.android.material.appbar.MaterialToolbar
            android:id="@+id/toolbar"
            android:layout_width="match_parent"
            android:layout_height="?attr/actionBarSize"
            app:title="Chọn rạp &amp; suất chiếu"
            app:navigationIcon="@drawable/ic_back" />

    </com.google.android.material.appbar.AppBarLayout>

    <!-- Content -->
    <androidx.core.widget.NestedScrollView
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        app:layout_behavior="@string/appbar_scrolling_view_behavior">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:paddingVertical="@dimen/spacing_medium">

            <!-- Movie Info Card -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_medium">

                    <!-- Movie Poster -->
                    <ImageView
                        android:id="@+id/ivMoviePoster"
                        android:layout_width="80dp"
                        android:layout_height="120dp"
                        android:scaleType="centerCrop" />

                    <!-- Movie Info -->
                    <LinearLayout
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_medium"
                        android:orientation="vertical">

                        <TextView
                            android:id="@+id/tvMovieTitle"
                            android:layout_width="match_parent"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Heading3"
                            android:textColor="@color/textPrimary"
                            android:maxLines="2"
                            android:ellipsize="end" />

                        <TextView
                            android:id="@+id/tvMovieInfo"
                            android:layout_width="match_parent"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:textAppearance="@style/TextAppearance.App.Caption"
                            android:textColor="@color/textSecondary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Date Selector -->
            <HorizontalScrollView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_medium"
                android:scrollbars="none">

                <com.google.android.material.chip.ChipGroup
                    android:id="@+id/chipGroupDates"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:paddingHorizontal="@dimen/spacing_medium"
                    app:singleSelection="true"
                    app:selectionRequired="true" />

            </HorizontalScrollView>

            <!-- Cinemas List -->
            <androidx.recyclerview.widget.RecyclerView
                android:id="@+id/rvCinemas"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:nestedScrollingEnabled="false" />

        </LinearLayout>

    </androidx.core.widget.NestedScrollView>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

### Cinema Item Layout: `item_cinema_showtime.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_marginHorizontal="@dimen/spacing_medium"
    android:layout_marginBottom="@dimen/spacing_medium"
    app:cardCornerRadius="@dimen/card_corner_radius_medium"
    app:cardElevation="@dimen/elevation_small">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical"
        android:padding="@dimen/spacing_medium">

        <!-- Cinema Name -->
        <TextView
            android:id="@+id/tvCinemaName"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:textAppearance="@style/TextAppearance.App.Heading3"
            android:textColor="@color/textPrimary"
            android:drawableStart="@drawable/ic_location"
            android:drawablePadding="@dimen/spacing_small"
            android:drawableTint="@color/colorPrimary" />

        <!-- Cinema Address -->
        <TextView
            android:id="@+id/tvCinemaAddress"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Caption"
            android:textColor="@color/textSecondary"
            android:maxLines="2"
            android:ellipsize="end" />

        <!-- Distance -->
        <TextView
            android:id="@+id/tvDistance"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Caption"
            android:textColor="@color/colorPrimary"
            android:drawableStart="@drawable/ic_navigation"
            android:drawablePadding="@dimen/spacing_xs"
            android:drawableTint="@color/colorPrimary" />

        <!-- Showtimes Grid -->
        <com.google.android.material.chip.ChipGroup
            android:id="@+id/chipGroupShowtimes"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_medium" />

    </LinearLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java: `SelectCinemaActivity.java`

```java
package com.movie88.ui.booking;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.chip.Chip;
import com.google.android.material.chip.ChipGroup;
import com.movie88.R;
import com.movie88.data.models.response.Movie;
import com.movie88.utils.DateUtils;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class SelectCinemaActivity extends AppCompatActivity {

    public static final String EXTRA_MOVIE_ID = "movie_id";
    public static final String EXTRA_SHOWTIME_ID = "showtime_id";

    private MaterialToolbar toolbar;
    private ImageView ivMoviePoster;
    private TextView tvMovieTitle;
    private TextView tvMovieInfo;
    private ChipGroup chipGroupDates;
    private RecyclerView rvCinemas;

    private SelectCinemaViewModel viewModel;
    private CinemaShowtimeAdapter adapter;

    private int movieId;
    private String selectedDate;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_cinema);

        movieId = getIntent().getIntExtra(EXTRA_MOVIE_ID, 0);

        initViews();
        setupToolbar();
        setupViewModel();
        setupRecyclerView();
        setupDateSelector();
        loadData();
    }

    private void initViews() {
        toolbar = findViewById(R.id.toolbar);
        ivMoviePoster = findViewById(R.id.ivMoviePoster);
        tvMovieTitle = findViewById(R.id.tvMovieTitle);
        tvMovieInfo = findViewById(R.id.tvMovieInfo);
        chipGroupDates = findViewById(R.id.chipGroupDates);
        rvCinemas = findViewById(R.id.rvCinemas);
    }

    private void setupToolbar() {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(SelectCinemaViewModel.class);

        // Observe movie details
        viewModel.getMovie().observe(this, this::displayMovieInfo);

        // Observe cinemas with showtimes
        viewModel.getCinemasWithShowtimes().observe(this, cinemas -> {
            adapter.updateData(cinemas);
        });

        // Observe errors
        viewModel.getError().observe(this, error -> {
            // Show error message
        });
    }

    private void setupRecyclerView() {
        adapter = new CinemaShowtimeAdapter(new ArrayList<>());
        adapter.setOnShowtimeClickListener(showtime -> {
            // Navigate to SelectSeatActivity
            Intent intent = new Intent(this, SelectSeatActivity.class);
            intent.putExtra(SelectSeatActivity.EXTRA_SHOWTIME_ID, showtime.getShowtimeId());
            startActivity(intent);
        });

        rvCinemas.setLayoutManager(new LinearLayoutManager(this));
        rvCinemas.setAdapter(adapter);
    }

    private void setupDateSelector() {
        // Generate next 7 days
        for (int i = 0; i < 7; i++) {
            Date date = DateUtils.addDays(new Date(), i);
            String dateStr = DateUtils.formatDate(date, "yyyy-MM-dd");
            String displayText;

            if (i == 0) {
                displayText = "Hôm nay\n" + DateUtils.formatDate(date, "dd/MM");
            } else if (i == 1) {
                displayText = "Ngày mai\n" + DateUtils.formatDate(date, "dd/MM");
            } else {
                displayText = DateUtils.formatDate(date, "EEE\ndd/MM");
            }

            Chip chip = new Chip(this);
            chip.setText(displayText);
            chip.setTag(dateStr);
            chip.setCheckable(true);

            if (i == 0) {
                chip.setChecked(true);
                selectedDate = dateStr;
            }

            chipGroupDates.addView(chip);
        }

        // Listen to date selection
        chipGroupDates.setOnCheckedStateChangeListener((group, checkedIds) -> {
            if (!checkedIds.isEmpty()) {
                Chip selectedChip = findViewById(checkedIds.get(0));
                selectedDate = (String) selectedChip.getTag();
                viewModel.loadCinemasWithShowtimes(movieId, selectedDate);
            }
        });
    }

    private void loadData() {
        viewModel.loadMovieDetails(movieId);
        viewModel.loadCinemasWithShowtimes(movieId, selectedDate);
    }

    private void displayMovieInfo(Movie movie) {
        Glide.with(this)
            .load(movie.getPosterUrl())
            .into(ivMoviePoster);

        tvMovieTitle.setText(movie.getTitle());
        tvMovieInfo.setText(String.format("%s • %d phút • %s",
            movie.getGenre(),
            movie.getDuration(),
            movie.getAgeRating()));
    }
}
```

**API Endpoints:**
- `GET /api/movies/{id}` - Lấy thông tin phim
- `GET /api/cinemas` - Lấy danh sách rạp
- `GET /api/showtimes/by-movie/{movieId}` - Lấy suất chiếu theo phim (filter by date)

---

## 2️⃣ SelectSeatActivity

---

## 2️⃣ SelectSeatActivity

### Purpose
Chọn ghế ngồi trong phòng chiếu - Màn hình quan trọng nhất của booking flow.

### Features
- Interactive seat map với layout thực tế của auditorium
- Multi-select: Chọn nhiều ghế cùng lúc
- Real-time status: Available/Selected/Occupied
- Countdown timer: 15 phút giữ ghế
- Tính tiền tự động theo loại ghế

### Layout: `activity_select_seat.xml`
```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.constraintlayout.widget.ConstraintLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <!-- Toolbar -->
    <androidx.appcompat.widget.Toolbar
        android:id="@+id/toolbar"
        android:layout_width="match_parent"
        android:layout_height="?attr/actionBarSize"
        android:background="@color/cardBackground"
        android:elevation="4dp"
        app:title="Chọn ghế"
        app:titleTextColor="@color/textPrimary"
        app:navigationIcon="@drawable/ic_arrow_back"
        app:layout_constraintTop_toTopOf="parent"/>

    <!-- Movie Info Card -->
    <androidx.cardview.widget.CardView
        android:id="@+id/cardMovieInfo"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_margin="@dimen/spacing_md"
        app:cardBackgroundColor="@color/cardBackground"
        app:cardCornerRadius="@dimen/card_corner_radius"
        app:cardElevation="@dimen/card_elevation"
        app:layout_constraintTop_toBottomOf="@id/toolbar">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:padding="@dimen/spacing_md">

            <ImageView
                android:id="@+id/ivMoviePoster"
                android:layout_width="60dp"
                android:layout_height="80dp"
                android:scaleType="centerCrop"
                android:contentDescription="Movie Poster"/>

            <LinearLayout
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:orientation="vertical"
                android:layout_marginStart="@dimen/spacing_md">

                <TextView
                    android:id="@+id/tvMovieTitle"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="Avengers: Endgame"
                    android:textSize="@dimen/text_lg"
                    android:textColor="@color/textPrimary"
                    android:textStyle="bold"/>

                <TextView
                    android:id="@+id/tvCinemaName"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="CGV Vincom Center"
                    android:textSize="@dimen/text_sm"
                    android:textColor="@color/textSecondary"
                    android:layout_marginTop="@dimen/spacing_xs"/>

                <TextView
                    android:id="@+id/tvShowtime"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="30/10/2025 - 19:30"
                    android:textSize="@dimen/text_sm"
                    android:textColor="@color/textSecondary"
                    android:layout_marginTop="@dimen/spacing_xs"/>

            </LinearLayout>

        </LinearLayout>

    </androidx.cardview.widget.CardView>

    <!-- Timer Countdown -->
    <LinearLayout
        android:id="@+id/layoutTimer"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal"
        android:gravity="center"
        android:background="@color/statusWarning"
        android:padding="@dimen/spacing_sm"
        app:layout_constraintTop_toBottomOf="@id/cardMovieInfo">

        <ImageView
            android:layout_width="20dp"
            android:layout_height="20dp"
            android:src="@drawable/ic_timer"
            android:tint="@color/textPrimary"/>

        <TextView
            android:id="@+id/tvTimer"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="14:59"
            android:textSize="@dimen/text_md"
            android:textColor="@color/textPrimary"
            android:textStyle="bold"
            android:layout_marginStart="@dimen/spacing_sm"/>

        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text=" còn lại để hoàn tất đặt vé"
            android:textSize="@dimen/text_sm"
            android:textColor="@color/textPrimary"/>

    </LinearLayout>

    <!-- Screen Label -->
    <TextView
        android:id="@+id/tvScreen"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="MÀN HÌNH"
        android:textSize="@dimen/text_md"
        android:textColor="@color/textSecondary"
        android:textAlignment="center"
        android:background="@drawable/bg_screen"
        android:padding="@dimen/spacing_sm"
        android:layout_margin="@dimen/spacing_lg"
        app:layout_constraintTop_toBottomOf="@id/layoutTimer"/>

    <!-- Seat Map (RecyclerView) -->
    <androidx.recyclerview.widget.RecyclerView
        android:id="@+id/rvSeatMap"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_margin="@dimen/spacing_md"
        android:clipToPadding="false"
        app:layout_constraintTop_toBottomOf="@id/tvScreen"
        app:layout_constraintBottom_toTopOf="@id/layoutLegend"/>

    <!-- Legend (Chú thích) -->
    <LinearLayout
        android:id="@+id/layoutLegend"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal"
        android:gravity="center"
        android:padding="@dimen/spacing_md"
        android:background="@color/cardBackground"
        app:layout_constraintBottom_toTopOf="@id/layoutBottom">

        <LinearLayout
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:layout_marginEnd="@dimen/spacing_md">

            <View
                android:layout_width="20dp"
                android:layout_height="20dp"
                android:background="@drawable/bg_seat_available"/>

            <TextView
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Trống"
                android:textSize="@dimen/text_sm"
                android:textColor="@color/textSecondary"
                android:layout_marginStart="@dimen/spacing_xs"/>

        </LinearLayout>

        <LinearLayout
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:layout_marginEnd="@dimen/spacing_md">

            <View
                android:layout_width="20dp"
                android:layout_height="20dp"
                android:background="@drawable/bg_seat_selected"/>

            <TextView
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Đang chọn"
                android:textSize="@dimen/text_sm"
                android:textColor="@color/textSecondary"
                android:layout_marginStart="@dimen/spacing_xs"/>

        </LinearLayout>

        <LinearLayout
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:layout_marginEnd="@dimen/spacing_md">

            <View
                android:layout_width="20dp"
                android:layout_height="20dp"
                android:background="@drawable/bg_seat_occupied"/>

            <TextView
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Đã đặt"
                android:textSize="@dimen/text_sm"
                android:textColor="@color/textSecondary"
                android:layout_marginStart="@dimen/spacing_xs"/>

        </LinearLayout>

        <LinearLayout
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical">

            <View
                android:layout_width="20dp"
                android:layout_height="20dp"
                android:background="@drawable/bg_seat_vip"/>

            <TextView
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="VIP"
                android:textSize="@dimen/text_sm"
                android:textColor="@color/textSecondary"
                android:layout_marginStart="@dimen/spacing_xs"/>

        </LinearLayout>

    </LinearLayout>

    <!-- Bottom Summary -->
    <LinearLayout
        android:id="@+id/layoutBottom"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical"
        android:background="@color/cardBackground"
        android:padding="@dimen/spacing_md"
        android:elevation="8dp"
        app:layout_constraintBottom_toBottomOf="parent">

        <!-- Selected Seats -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical">

            <TextView
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="Ghế đã chọn:"
                android:textSize="@dimen/text_md"
                android:textColor="@color/textSecondary"/>

            <TextView
                android:id="@+id/tvSelectedSeats"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="D5, D6"
                android:textSize="@dimen/text_md"
                android:textColor="@color/textPrimary"
                android:textStyle="bold"/>

        </LinearLayout>

        <!-- Total Amount -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:layout_marginTop="@dimen/spacing_sm">

            <TextView
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="Tổng tiền:"
                android:textSize="@dimen/text_lg"
                android:textColor="@color/textSecondary"/>

            <TextView
                android:id="@+id/tvTotalAmount"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="196.000 đ"
                android:textSize="@dimen/text_xl"
                android:textColor="@color/colorPrimary"
                android:textStyle="bold"/>

        </LinearLayout>

        <!-- Continue Button -->
        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnContinue"
            android:layout_width="match_parent"
            android:layout_height="@dimen/button_height"
            android:text="Tiếp tục"
            android:textSize="@dimen/text_lg"
            android:textColor="@color/textPrimary"
            android:backgroundTint="@color/colorPrimary"
            app:cornerRadius="@dimen/button_corner_radius"
            android:layout_marginTop="@dimen/spacing_md"
            android:enabled="false"/>

    </LinearLayout>

</androidx.constraintlayout.widget.ConstraintLayout>
```

### Seat Item Layout (item_seat.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<FrameLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="@dimen/seat_size"
    android:layout_height="@dimen/seat_size"
    android:layout_margin="@dimen/seat_spacing">

    <TextView
        android:id="@+id/tvSeatNumber"
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:text="A1"
        android:textSize="@dimen/text_xs"
        android:textColor="@color/textPrimary"
        android:gravity="center"
        android:background="@drawable/bg_seat_available"/>

</FrameLayout>
```

### Seat Backgrounds (drawable/)
```xml
<!-- bg_seat_available.xml -->
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="@color/seatAvailable"/>
    <corners android:radius="4dp"/>
</shape>

<!-- bg_seat_selected.xml -->
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="@color/seatSelected"/>
    <corners android:radius="4dp"/>
</shape>

<!-- bg_seat_occupied.xml -->
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="@color/seatOccupied"/>
    <corners android:radius="4dp"/>
</shape>

<!-- bg_seat_vip.xml -->
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="@color/seatVip"/>
    <corners android:radius="4dp"/>
</shape>
```

### Java Code (SeatSelectionActivity.java)
```java
public class SeatSelectionActivity extends AppCompatActivity {
    
    private RecyclerView rvSeatMap;
    private TextView tvSelectedSeats, tvTotalAmount, tvTimer;
    private MaterialButton btnContinue;
    
    private SeatAdapter seatAdapter;
    private SeatSelectionViewModel viewModel;
    
    private int showtimeId;
    private List<Seat> selectedSeats = new ArrayList<>();
    
    private CountDownTimer countDownTimer;
    private static final long LOCK_DURATION = 15 * 60 * 1000; // 15 minutes
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_seat_selection);
        
        // Get data from intent
        showtimeId = getIntent().getIntExtra("SHOWTIME_ID", 0);
        
        initViews();
        initViewModel();
        setupRecyclerView();
        setupListeners();
        observeViewModel();
        
        // Load seats
        viewModel.loadSeats(showtimeId);
        
        // Start countdown
        startCountdown();
    }
    
    private void initViews() {
        rvSeatMap = findViewById(R.id.rvSeatMap);
        tvSelectedSeats = findViewById(R.id.tvSelectedSeats);
        tvTotalAmount = findViewById(R.id.tvTotalAmount);
        tvTimer = findViewById(R.id.tvTimer);
        btnContinue = findViewById(R.id.btnContinue);
    }
    
    private void initViewModel() {
        viewModel = new ViewModelProvider(this).get(SeatSelectionViewModel.class);
    }
    
    private void setupRecyclerView() {
        seatAdapter = new SeatAdapter(new SeatAdapter.OnSeatClickListener() {
            @Override
            public void onSeatClick(Seat seat) {
                handleSeatClick(seat);
            }
        });
        
        // Grid layout (số cột = số ghế trong 1 hàng)
        GridLayoutManager layoutManager = new GridLayoutManager(this, 10);
        rvSeatMap.setLayoutManager(layoutManager);
        rvSeatMap.setAdapter(seatAdapter);
    }
    
    private void setupListeners() {
        btnContinue.setOnClickListener(v -> {
            if (!selectedSeats.isEmpty()) {
                lockSeats();
            }
        });
    }
    
    private void observeViewModel() {
        // Observe seats data
        viewModel.getSeats().observe(this, seats -> {
            if (seats != null) {
                seatAdapter.setSeats(seats);
            }
        });
        
        // Observe lock seats result
        viewModel.getLockSeatsResult().observe(this, result -> {
            if (result.isSuccess()) {
                // Navigate to Combo Selection
                Intent intent = new Intent(this, ComboSelectionActivity.class);
                intent.putExtra("SHOWTIME_ID", showtimeId);
                intent.putIntegerArrayListExtra("SEAT_IDS", 
                    getSeatIds(selectedSeats));
                startActivity(intent);
            } else {
                Toast.makeText(this, "Không thể chọn ghế: " + result.getError(), 
                    Toast.LENGTH_SHORT).show();
            }
        });
        
        // Observe errors
        viewModel.getError().observe(this, error -> {
            if (error != null) {
                Toast.makeText(this, error, Toast.LENGTH_SHORT).show();
            }
        });
    }
    
    private void handleSeatClick(Seat seat) {
        if (seat.getStatus().equals("Occupied")) {
            Toast.makeText(this, "Ghế đã được đặt", Toast.LENGTH_SHORT).show();
            return;
        }
        
        if (selectedSeats.contains(seat)) {
            // Deselect
            selectedSeats.remove(seat);
            seat.setSelected(false);
        } else {
            // Select
            selectedSeats.add(seat);
            seat.setSelected(true);
        }
        
        seatAdapter.notifyDataSetChanged();
        updateBottomSummary();
    }
    
    private void updateBottomSummary() {
        if (selectedSeats.isEmpty()) {
            tvSelectedSeats.setText("Chưa chọn ghế");
            tvTotalAmount.setText("0 đ");
            btnContinue.setEnabled(false);
        } else {
            // Display selected seats
            StringBuilder seatsText = new StringBuilder();
            for (int i = 0; i < selectedSeats.size(); i++) {
                seatsText.append(selectedSeats.get(i).getSeatNumber());
                if (i < selectedSeats.size() - 1) {
                    seatsText.append(", ");
                }
            }
            tvSelectedSeats.setText(seatsText.toString());
            
            // Calculate total amount
            double total = 0;
            for (Seat seat : selectedSeats) {
                total += seat.getPrice();
            }
            tvTotalAmount.setText(formatCurrency(total));
            
            btnContinue.setEnabled(true);
        }
    }
    
    private void lockSeats() {
        List<Integer> seatIds = getSeatIds(selectedSeats);
        
        LockSeatsRequest request = new LockSeatsRequest(showtimeId, seatIds);
        viewModel.lockSeats(request);
    }
    
    private ArrayList<Integer> getSeatIds(List<Seat> seats) {
        ArrayList<Integer> ids = new ArrayList<>();
        for (Seat seat : seats) {
            ids.add(seat.getSeatId());
        }
        return ids;
    }
    
    private void startCountdown() {
        countDownTimer = new CountDownTimer(LOCK_DURATION, 1000) {
            @Override
            public void onTick(long millisUntilFinished) {
                long minutes = millisUntilFinished / 1000 / 60;
                long seconds = (millisUntilFinished / 1000) % 60;
                tvTimer.setText(String.format("%02d:%02d", minutes, seconds));
            }
            
            @Override
            public void onFinish() {
                // Time expired
                showTimeExpiredDialog();
            }
        };
        countDownTimer.start();
    }
    
    private void showTimeExpiredDialog() {
        new MaterialAlertDialogBuilder(this)
            .setTitle("Hết thời gian")
            .setMessage("Thời gian chọn ghế đã hết. Vui lòng thử lại.")
            .setPositiveButton("OK", (dialog, which) -> finish())
            .setCancelable(false)
            .show();
    }
    
    private String formatCurrency(double amount) {
        DecimalFormat formatter = new DecimalFormat("#,###");
        return formatter.format(amount) + " đ";
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

### Seat Adapter (SeatAdapter.java)
```java
public class SeatAdapter extends RecyclerView.Adapter<SeatAdapter.SeatViewHolder> {
    
    private List<Seat> seats = new ArrayList<>();
    private OnSeatClickListener listener;
    
    public interface OnSeatClickListener {
        void onSeatClick(Seat seat);
    }
    
    public SeatAdapter(OnSeatClickListener listener) {
        this.listener = listener;
    }
    
    @NonNull
    @Override
    public SeatViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
            .inflate(R.layout.item_seat, parent, false);
        return new SeatViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull SeatViewHolder holder, int position) {
        Seat seat = seats.get(position);
        holder.bind(seat, listener);
    }
    
    @Override
    public int getItemCount() {
        return seats.size();
    }
    
    public void setSeats(List<Seat> seats) {
        this.seats = seats;
        notifyDataSetChanged();
    }
    
    static class SeatViewHolder extends RecyclerView.ViewHolder {
        
        private TextView tvSeatNumber;
        
        public SeatViewHolder(@NonNull View itemView) {
            super(itemView);
            tvSeatNumber = itemView.findViewById(R.id.tvSeatNumber);
        }
        
        public void bind(Seat seat, OnSeatClickListener listener) {
            tvSeatNumber.setText(seat.getSeatNumber());
            
            // Set background based on status
            int backgroundRes;
            if (seat.isSelected()) {
                backgroundRes = R.drawable.bg_seat_selected;
            } else if (seat.getStatus().equals("Occupied")) {
                backgroundRes = R.drawable.bg_seat_occupied;
                tvSeatNumber.setEnabled(false);
            } else if (seat.getType().equals("VIP")) {
                backgroundRes = R.drawable.bg_seat_vip;
            } else {
                backgroundRes = R.drawable.bg_seat_available;
            }
            
            tvSeatNumber.setBackgroundResource(backgroundRes);
            
            // Click listener
            itemView.setOnClickListener(v -> {
                if (!seat.getStatus().equals("Occupied")) {
                    listener.onSeatClick(seat);
                }
            });
        }
    }
}
```

### ViewModel (SeatSelectionViewModel.java)
```java
public class SeatSelectionViewModel extends ViewModel {
    
    private final BookingRepository bookingRepository;
    
    private final MutableLiveData<List<Seat>> seats = new MutableLiveData<>();
    private final MutableLiveData<Result<LockSeatsResponse>> lockSeatsResult = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    
    public SeatSelectionViewModel() {
        bookingRepository = BookingRepository.getInstance();
    }
    
    public void loadSeats(int showtimeId) {
        bookingRepository.getShowtimeSeats(showtimeId, new ApiCallback<List<Seat>>() {
            @Override
            public void onSuccess(List<Seat> data) {
                seats.setValue(data);
            }
            
            @Override
            public void onError(String errorMessage) {
                error.setValue(errorMessage);
            }
        });
    }
    
    public void lockSeats(LockSeatsRequest request) {
        bookingRepository.lockSeats(request, new ApiCallback<LockSeatsResponse>() {
            @Override
            public void onSuccess(LockSeatsResponse response) {
                lockSeatsResult.setValue(Result.success(response));
            }
            
            @Override
            public void onError(String errorMessage) {
                lockSeatsResult.setValue(Result.error(errorMessage));
            }
        });
    }
    
    // Getters
    public LiveData<List<Seat>> getSeats() {
        return seats;
    }
    
    public LiveData<Result<LockSeatsResponse>> getLockSeatsResult() {
        return lockSeatsResult;
    }
    
    public LiveData<String> getError() {
        return error;
    }
}
```

### Models
```java
// Seat.java
public class Seat {
    private int seatId;
    private String seatNumber;
    private String type; // "Standard", "VIP", "Couple"
    private String status; // "Available", "Occupied"
    private double price;
    private boolean isSelected; // Client-side state
    
    // Getters and setters
}

// LockSeatsRequest.java
public class LockSeatsRequest {
    private int showtimeId;
    private List<Integer> seatIds;
    
    public LockSeatsRequest(int showtimeId, List<Integer> seatIds) {
        this.showtimeId = showtimeId;
        this.seatIds = seatIds;
    }
    
    // Getters and setters
}

// LockSeatsResponse.java
public class LockSeatsResponse {
    private int bookingId;
    private String expiryTime;
    
    // Getters and setters
}
```

---

## API Endpoints Used

**SelectCinemaActivity:**
- `GET /api/movies/{id}` - Lấy thông tin chi tiết phim
- `GET /api/cinemas` - Lấy danh sách rạp chiếu
- `GET /api/showtimes/by-movie/{movieId}` - Lấy suất chiếu theo phim
- `GET /api/showtimes/by-date` - Lấy suất chiếu theo ngày (với query param date)

**SelectSeatActivity:**
- `GET /api/showtimes/{id}` - Lấy thông tin chi tiết suất chiếu
- `GET /api/showtimes/{id}/available-seats` - Lấy danh sách ghế còn trống
- `GET /api/auditoriums/{id}/seats` - Lấy sơ đồ ghế của phòng chiếu
- `POST /api/bookings/create` - Tạo booking mới (sau khi chọn ghế)

**SelectComboActivity:**
- `GET /api/combos` - Lấy danh sách combo bắp nước
- `POST /api/bookings/{id}/add-combos` - Thêm combo vào booking

---

### API Interface Example
```java
public interface ApiService {
    
    // Get movie details
    @GET("api/movies/{id}")
    Call<ApiResponse<Movie>> getMovieById(@Path("id") int movieId);
    
    // Get cinemas
    @GET("api/cinemas")
    Call<ApiResponse<List<Cinema>>> getCinemas();
    
    // Get showtimes by movie
    @GET("api/showtimes/by-movie/{movieId}")
    Call<ApiResponse<List<Showtime>>> getShowtimesByMovie(@Path("movieId") int movieId);
    
    // Get showtimes by date
    @GET("api/showtimes/by-date")
    Call<ApiResponse<List<Showtime>>> getShowtimesByDate(@Query("date") String date);
    
    // Get showtime details
    @GET("api/showtimes/{id}")
    Call<ApiResponse<Showtime>> getShowtimeById(@Path("id") int showtimeId);
    
    // Get available seats for showtime
    @GET("api/showtimes/{id}/available-seats")
    Call<ApiResponse<List<Seat>>> getAvailableSeats(@Path("id") int showtimeId);
    
    // Get seat layout of auditorium
    @GET("api/auditoriums/{id}/seats")
    Call<ApiResponse<List<Seat>>> getAuditoriumSeats(@Path("id") int auditoriumId);
    
    // Create booking
    @POST("api/bookings/create")
    Call<ApiResponse<Booking>> createBooking(
        @Header("Authorization") String token,
        @Body CreateBookingRequest request
    );
    
    // Get combos
    @GET("api/combos")
    Call<ApiResponse<List<Combo>>> getCombos();
    
    // Add combos to booking
    @POST("api/bookings/{id}/add-combos")
    Call<ApiResponse<Booking>> addCombos(
        @Header("Authorization") String token,
        @Path("id") int bookingId,
        @Body AddCombosRequest request
    );
}
```

---

## 3️⃣ SelectComboActivity

### Purpose
Chọn combo bắp nước - Màn hình optional, có thể skip.

### Features
- Hiển thị danh sách combos với hình ảnh
- Chọn số lượng cho mỗi combo
- Tính tổng tiền combos
- Button "Bỏ qua" để skip màn hình này

### Layout: `activity_select_combo.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.coordinatorlayout.widget.CoordinatorLayout
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/background">

    <!-- App Bar -->
    <com.google.android.material.appbar.AppBarLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:background="@color/surface"
        app:elevation="@dimen/elevation_small">

        <com.google.android.material.appbar.MaterialToolbar
            android:id="@+id/toolbar"
            android:layout_width="match_parent"
            android:layout_height="?attr/actionBarSize"
            app:title="Chọn combo"
            app:navigationIcon="@drawable/ic_back" />

    </com.google.android.material.appbar.AppBarLayout>

    <!-- Content -->
    <androidx.core.widget.NestedScrollView
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        app:layout_behavior="@string/appbar_scrolling_view_behavior">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:paddingVertical="@dimen/spacing_medium"
            android:paddingBottom="100dp">

            <!-- Info Text -->
            <TextView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                android:text="@string/combo_selection_hint"
                android:textAppearance="@style/TextAppearance.App.Body"
                android:textColor="@color/textSecondary" />

            <!-- Combos List -->
            <androidx.recyclerview.widget.RecyclerView
                android:id="@+id/rvCombos"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:nestedScrollingEnabled="false" />

        </LinearLayout>

    </androidx.core.widget.NestedScrollView>

    <!-- Bottom Action Bar -->
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_gravity="bottom"
        android:orientation="vertical"
        android:background="@color/surface"
        android:elevation="@dimen/elevation_medium"
        android:padding="@dimen/spacing_medium">

        <!-- Total Price -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:layout_marginBottom="@dimen/spacing_small">

            <TextView
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="@string/total_combo"
                android:textAppearance="@style/TextAppearance.App.Body"
                android:textColor="@color/textSecondary" />

            <TextView
                android:id="@+id/tvTotalCombo"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="0đ"
                android:textAppearance="@style/TextAppearance.App.Heading3"
                android:textColor="@color/colorPrimary" />

        </LinearLayout>

        <!-- Buttons -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="end">

            <!-- Skip Button -->
            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnSkip"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="@string/skip"
                style="@style/Widget.Material3.Button.OutlinedButton" />

            <!-- Continue Button -->
            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnContinue"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:layout_marginStart="@dimen/spacing_small"
                android:text="@string/continue_text"
                style="@style/Widget.Material3.Button" />

        </LinearLayout>

    </LinearLayout>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

### Combo Item Layout: `item_combo.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_marginHorizontal="@dimen/spacing_medium"
    android:layout_marginBottom="@dimen/spacing_medium"
    app:cardCornerRadius="@dimen/card_corner_radius_medium"
    app:cardElevation="@dimen/elevation_small">

    <androidx.constraintlayout.widget.ConstraintLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_medium">

        <!-- Combo Image -->
        <ImageView
            android:id="@+id/ivCombo"
            android:layout_width="80dp"
            android:layout_height="80dp"
            android:scaleType="centerCrop"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toTopOf="parent" />

        <!-- Combo Name -->
        <TextView
            android:id="@+id/tvComboName"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginStart="@dimen/spacing_medium"
            android:textAppearance="@style/TextAppearance.App.Heading3"
            android:textColor="@color/textPrimary"
            android:maxLines="2"
            android:ellipsize="end"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toEndOf="@id/ivCombo"
            app:layout_constraintTop_toTopOf="@id/ivCombo" />

        <!-- Combo Description -->
        <TextView
            android:id="@+id/tvComboDescription"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Caption"
            android:textColor="@color/textSecondary"
            android:maxLines="2"
            android:ellipsize="end"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="@id/tvComboName"
            app:layout_constraintTop_toBottomOf="@id/tvComboName" />

        <!-- Price & Quantity Selector -->
        <LinearLayout
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_medium"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/ivCombo">

            <!-- Price -->
            <TextView
                android:id="@+id/tvComboPrice"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:textAppearance="@style/TextAppearance.App.Heading3"
                android:textColor="@color/colorPrimary" />

            <!-- Quantity Selector -->
            <LinearLayout
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:gravity="center_vertical">

                <!-- Decrease Button -->
                <com.google.android.material.button.MaterialButton
                    android:id="@+id/btnDecrease"
                    android:layout_width="36dp"
                    android:layout_height="36dp"
                    android:minWidth="0dp"
                    android:minHeight="0dp"
                    android:padding="0dp"
                    android:insetTop="0dp"
                    android:insetBottom="0dp"
                    android:text="-"
                    android:textSize="18sp"
                    style="@style/Widget.Material3.Button.OutlinedButton" />

                <!-- Quantity -->
                <TextView
                    android:id="@+id/tvQuantity"
                    android:layout_width="40dp"
                    android:layout_height="wrap_content"
                    android:text="0"
                    android:textAlignment="center"
                    android:textAppearance="@style/TextAppearance.App.Body"
                    android:textColor="@color/textPrimary" />

                <!-- Increase Button -->
                <com.google.android.material.button.MaterialButton
                    android:id="@+id/btnIncrease"
                    android:layout_width="36dp"
                    android:layout_height="36dp"
                    android:minWidth="0dp"
                    android:minHeight="0dp"
                    android:padding="0dp"
                    android:insetTop="0dp"
                    android:insetBottom="0dp"
                    android:text="+"
                    android:textSize="18sp"
                    style="@style/Widget.Material3.Button" />

            </LinearLayout>

        </LinearLayout>

    </androidx.constraintlayout.widget.ConstraintLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java: `SelectComboActivity.java`

```java
package com.movie88.ui.booking;

import android.content.Intent;
import android.os.Bundle;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.button.MaterialButton;
import com.movie88.R;
import com.movie88.utils.CurrencyUtils;

import java.util.ArrayList;

public class SelectComboActivity extends AppCompatActivity {

    public static final String EXTRA_BOOKING_ID = "booking_id";

    private MaterialToolbar toolbar;
    private RecyclerView rvCombos;
    private TextView tvTotalCombo;
    private MaterialButton btnSkip;
    private MaterialButton btnContinue;

    private SelectComboViewModel viewModel;
    private ComboAdapter adapter;

    private int bookingId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_select_combo);

        bookingId = getIntent().getIntExtra(EXTRA_BOOKING_ID, 0);

        initViews();
        setupToolbar();
        setupViewModel();
        setupRecyclerView();
        setupButtons();
        loadData();
    }

    private void initViews() {
        toolbar = findViewById(R.id.toolbar);
        rvCombos = findViewById(R.id.rvCombos);
        tvTotalCombo = findViewById(R.id.tvTotalCombo);
        btnSkip = findViewById(R.id.btnSkip);
        btnContinue = findViewById(R.id.btnContinue);
    }

    private void setupToolbar() {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(SelectComboViewModel.class);

        // Observe combos
        viewModel.getCombos().observe(this, combos -> {
            adapter.updateData(combos);
        });

        // Observe total price
        viewModel.getTotalComboPrice().observe(this, total -> {
            tvTotalCombo.setText(CurrencyUtils.formatCurrency(total));
        });

        // Observe save success
        viewModel.getSaveSuccess().observe(this, success -> {
            if (success) {
                navigateToBookingSummary();
            }
        });
    }

    private void setupRecyclerView() {
        adapter = new ComboAdapter(new ArrayList<>());
        adapter.setOnQuantityChangeListener((combo, quantity) -> {
            viewModel.updateComboQuantity(combo, quantity);
        });

        rvCombos.setLayoutManager(new LinearLayoutManager(this));
        rvCombos.setAdapter(adapter);
    }

    private void setupButtons() {
        // Skip button - go directly to summary
        btnSkip.setOnClickListener(v -> {
            navigateToBookingSummary();
        });

        // Continue button - save combos and go to summary
        btnContinue.setOnClickListener(v -> {
            viewModel.saveCombosToBooking(bookingId);
        });
    }

    private void loadData() {
        viewModel.loadCombos();
    }

    private void navigateToBookingSummary() {
        Intent intent = new Intent(this, BookingSummaryActivity.class);
        intent.putExtra(BookingSummaryActivity.EXTRA_BOOKING_ID, bookingId);
        startActivity(intent);
        finish();
    }
}
```

**API Endpoints:**
- `GET /api/combos` - Lấy danh sách combos
- `POST /api/bookings/{id}/add-combos` - Thêm combos vào booking (khi click Continue)

---

## 4️⃣ BookingSummaryActivity

### Purpose
Xác nhận thông tin booking cuối cùng, áp dụng voucher, và thanh toán.

### Features
- Hiển thị tóm tắt: Movie, Cinema, Showtime, Seats, Combos
- Áp dụng mã voucher giảm giá
- Hiển thị breakdown giá: Subtotal, Discount, Total
- Countdown timer: 15 phút (tiếp tục từ SelectSeatActivity)
- Button thanh toán → Navigate to VNPayWebViewActivity

### Layout: `activity_booking_summary.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.coordinatorlayout.widget.CoordinatorLayout
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/background">

    <!-- App Bar -->
    <com.google.android.material.appbar.AppBarLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:background="@color/surface"
        app:elevation="@dimen/elevation_small">

        <com.google.android.material.appbar.MaterialToolbar
            android:id="@+id/toolbar"
            android:layout_width="match_parent"
            android:layout_height="?attr/actionBarSize"
            app:title="Xác nhận đặt vé"
            app:navigationIcon="@drawable/ic_back" />

    </com.google.android.material.appbar.AppBarLayout>

    <!-- Content -->
    <androidx.core.widget.NestedScrollView
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        app:layout_behavior="@string/appbar_scrolling_view_behavior">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:paddingVertical="@dimen/spacing_medium"
            android:paddingBottom="150dp">

            <!-- Timer Warning -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardBackgroundColor="@color/statusWarning"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="0dp">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:gravity="center_vertical"
                    android:padding="@dimen/spacing_medium">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_timer"
                        app:tint="@color/white" />

                    <TextView
                        android:id="@+id/tvTimeRemaining"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_small"
                        android:text="Thời gian còn lại: 14:35"
                        android:textAppearance="@style/TextAppearance.App.Body"
                        android:textColor="@color/white"
                        android:textStyle="bold" />

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Movie Info Card -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_medium">

                    <ImageView
                        android:id="@+id/ivMoviePoster"
                        android:layout_width="80dp"
                        android:layout_height="120dp"
                        android:scaleType="centerCrop" />

                    <LinearLayout
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_medium"
                        android:orientation="vertical">

                        <TextView
                            android:id="@+id/tvMovieTitle"
                            android:layout_width="match_parent"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Heading3"
                            android:textColor="@color/textPrimary"
                            android:maxLines="2"
                            android:ellipsize="end" />

                        <TextView
                            android:id="@+id/tvCinemaName"
                            android:layout_width="match_parent"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textSecondary"
                            android:drawableStart="@drawable/ic_location"
                            android:drawablePadding="@dimen/spacing_xs"
                            android:drawableTint="@color/iconSecondary" />

                        <TextView
                            android:id="@+id/tvShowtime"
                            android:layout_width="match_parent"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textSecondary"
                            android:drawableStart="@drawable/ic_calendar"
                            android:drawablePadding="@dimen/spacing_xs"
                            android:drawableTint="@color/iconSecondary" />

                        <TextView
                            android:id="@+id/tvAuditorium"
                            android:layout_width="match_parent"
                            android:layout_height="wrap_content"
                            android:layout_marginTop="@dimen/spacing_xs"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textSecondary"
                            android:drawableStart="@drawable/ic_theater"
                            android:drawablePadding="@dimen/spacing_xs"
                            android:drawableTint="@color/iconSecondary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Seats Info Card -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/seats_info"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="1dp"
                        android:background="@color/divider"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            android:id="@+id/tvSeatsLabel"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/seats"
                            android:textAppearance="@style/TextAppearance.App.BodySecondary"
                            android:textColor="@color/textSecondary" />

                        <TextView
                            android:id="@+id/tvSeatsValue"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                    </LinearLayout>

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:layout_marginTop="@dimen/spacing_xs">

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/seat_price"
                            android:textAppearance="@style/TextAppearance.App.BodySecondary"
                            android:textColor="@color/textSecondary" />

                        <TextView
                            android:id="@+id/tvSeatPrice"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Combos Info Card (Visible if has combos) -->
            <com.google.android.material.card.MaterialCardView
                android:id="@+id/cardCombos"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small"
                android:visibility="gone">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/combos"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="1dp"
                        android:background="@color/divider"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <androidx.recyclerview.widget.RecyclerView
                        android:id="@+id/rvCombos"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:nestedScrollingEnabled="false" />

                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:layout_marginTop="@dimen/spacing_small">

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/combo_price"
                            android:textAppearance="@style/TextAppearance.App.BodySecondary"
                            android:textColor="@color/textSecondary" />

                        <TextView
                            android:id="@+id/tvComboPrice"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Voucher Card -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/voucher"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <!-- Voucher Input -->
                    <LinearLayout
                        android:id="@+id/layoutVoucherInput"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical">

                        <com.google.android.material.textfield.TextInputLayout
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:hint="@string/enter_voucher_code"
                            style="@style/Widget.Material3.TextInputLayout.OutlinedBox">

                            <com.google.android.material.textfield.TextInputEditText
                                android:id="@+id/etVoucherCode"
                                android:layout_width="match_parent"
                                android:layout_height="wrap_content"
                                android:inputType="textCapCharacters"
                                android:maxLength="20" />

                        </com.google.android.material.textfield.TextInputLayout>

                        <com.google.android.material.button.MaterialButton
                            android:id="@+id/btnApplyVoucher"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginStart="@dimen/spacing_small"
                            android:text="@string/apply"
                            style="@style/Widget.Material3.Button" />

                    </LinearLayout>

                    <!-- Applied Voucher Display -->
                    <LinearLayout
                        android:id="@+id/layoutAppliedVoucher"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:padding="@dimen/spacing_small"
                        android:background="@drawable/bg_voucher_applied"
                        android:layout_marginTop="@dimen/spacing_small"
                        android:visibility="gone">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_check"
                            app:tint="@color/statusSuccess" />

                        <TextView
                            android:id="@+id/tvAppliedVoucher"
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:layout_marginStart="@dimen/spacing_small"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/statusSuccess" />

                        <ImageView
                            android:id="@+id/ivRemoveVoucher"
                            android:layout_width="@dimen/icon_size_sm"
                            android:layout_height="@dimen/icon_size_sm"
                            android:src="@drawable/ic_close"
                            app:tint="@color/textSecondary"
                            android:clickable="true"
                            android:focusable="true"
                            android:background="?attr/selectableItemBackgroundBorderless" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Price Summary Card -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/payment_summary"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="1dp"
                        android:background="@color/divider"
                        android:layout_marginBottom="@dimen/spacing_small" />

                    <!-- Subtotal -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/subtotal"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textSecondary" />

                        <TextView
                            android:id="@+id/tvSubtotal"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                    </LinearLayout>

                    <!-- Discount -->
                    <LinearLayout
                        android:id="@+id/layoutDiscount"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:visibility="gone">

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/discount"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/statusSuccess" />

                        <TextView
                            android:id="@+id/tvDiscount"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/statusSuccess" />

                    </LinearLayout>

                    <View
                        android:layout_width="match_parent"
                        android:layout_height="1dp"
                        android:background="@color/divider"
                        android:layout_marginVertical="@dimen/spacing_small" />

                    <!-- Total -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal">

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/total"
                            android:textAppearance="@style/TextAppearance.App.Heading2"
                            android:textColor="@color/textPrimary" />

                        <TextView
                            android:id="@+id/tvTotal"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:textAppearance="@style/TextAppearance.App.Heading2"
                            android:textColor="@color/colorPrimary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

        </LinearLayout>

    </androidx.core.widget.NestedScrollView>

    <!-- Bottom Payment Button -->
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_gravity="bottom"
        android:orientation="vertical"
        android:background="@color/surface"
        android:elevation="@dimen/elevation_medium"
        android:padding="@dimen/spacing_medium">

        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnPayment"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:text="@string/proceed_to_payment"
            android:textSize="16sp"
            app:icon="@drawable/ic_payment"
            style="@style/Widget.Material3.Button" />

    </LinearLayout>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

**Note**: Màn hình này đã được implement chi tiết trong file `05-Payment.md`, section BookingSummaryActivity.

**API Endpoints:**
- `GET /api/bookings/{id}` - Lấy thông tin booking đầy đủ
- `POST /api/vouchers/validate` - Validate voucher code
- `POST /api/bookings/{id}/apply-voucher` - Áp dụng voucher
- `POST /api/payments/vnpay/create` - Tạo payment URL VNPay

**Navigation:**
- Click "Thanh toán" → Navigate to `VNPayWebViewActivity` với payment URL

---

## 5️⃣ VNPayWebViewActivity & PaymentResultActivity

### Purpose
**VNPayWebViewActivity**: Load trang thanh toán VNPay trong WebView, intercept callback URL.  
**PaymentResultActivity**: Hiển thị kết quả thanh toán (Success/Failed) với QR code và booking details.

**Note**: Cả 2 màn hình này đã được implement đầy đủ trong file `05-Payment.md`.

### Flow Summary:
1. **VNPayWebViewActivity** loads payment URL from VNPay
2. User completes payment on VNPay page
3. VNPay redirects to callback URL với payment result params
4. WebView intercepts callback URL, extracts `vnp_ResponseCode`
5. Navigate to **PaymentResultActivity** with result
6. Display success/failed UI với QR code (nếu thành công)

**API Endpoints:**
- `GET /api/payments/vnpay/callback` - Auto-handled by VNPay callback
- `POST /api/payments/vnpay/ipn` - IPN notification (auto by VNPay)
- `PUT /api/payments/{id}/confirm` - Confirm payment success
- `GET /api/bookings/{id}` - Lấy booking details sau thanh toán

---

## 📊 Booking Flow Summary

| Step | Screen | Purpose | Can Skip? | Next Screen |
|------|--------|---------|-----------|-------------|
| 1 | SelectCinemaActivity | Chọn rạp & suất chiếu | ❌ | SelectSeatActivity |
| 2 | SelectSeatActivity | Chọn ghế ngồi | ❌ | SelectComboActivity |
| 3 | SelectComboActivity | Chọn combo bắp nước | ✅ | BookingSummaryActivity |
| 4 | BookingSummaryActivity | Xác nhận & áp voucher | ❌ | VNPayWebViewActivity |
| 5 | VNPayWebViewActivity | Thanh toán VNPay | ❌ | PaymentResultActivity |
| 6 | PaymentResultActivity | Kết quả thanh toán | - | MainActivity/BookingDetail |

### Data Flow:
```
SelectCinemaActivity
  └─> showtimeId
      └─> SelectSeatActivity (create booking)
          └─> bookingId
              └─> SelectComboActivity (add combos - optional)
                  └─> bookingId
                      └─> BookingSummaryActivity (apply voucher)
                          └─> bookingId + paymentUrl
                              └─> VNPayWebViewActivity
                                  └─> bookingId + paymentResult
                                      └─> PaymentResultActivity
```

### Timer Management:
- **Start**: SelectSeatActivity khi chọn ghế đầu tiên (15 phút)
- **Continue**: Timer tiếp tục chạy qua SelectComboActivity và BookingSummaryActivity
- **Implementation**: Lưu `expiryTime` (timestamp) vào Intent extras, pass qua các màn hình
- **Warning**: Hiển thị warning khi còn < 2 phút
- **Expire**: Auto-finish tất cả activities và clear booking khi hết time

---
