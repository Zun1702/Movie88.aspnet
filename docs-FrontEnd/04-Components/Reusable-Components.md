# 🎨 Reusable UI Components

## Tổng quan

Thư viện các reusable components giúp đảm bảo consistency và tái sử dụng code. Tất cả components đều tuân theo Material Design guidelines.

---

## 📂 Component Structure

```
app/src/main/java/com/movie88/ui/components/
│
├── MoviePosterCard.java          # Movie poster với rating
├── SeatView.java                 # Single seat component
├── ComboItemCard.java            # Food & drink combo card
├── BookingCard.java              # Booking history item
├── ReviewCard.java               # Movie review item
├── LoadingDialog.java            # Loading indicator dialog
├── ErrorDialog.java              # Error message dialog
└── EmptyStateView.java           # Empty state placeholder
```

---

## 1️⃣ MoviePosterCard

### Purpose
Hiển thị movie poster với rating badge, genre, title. Được sử dụng trong HomeFragment, SearchActivity, MovieListActivity.

### Layout (component_movie_poster_card.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:id="@+id/cardContainer"
    android:layout_width="@dimen/movie_poster_width"
    android:layout_height="wrap_content"
    android:layout_marginEnd="@dimen/spacing_md"
    app:cardCornerRadius="@dimen/card_corner_radius"
    app:cardElevation="@dimen/card_elevation"
    app:strokeWidth="0dp">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical">

        <!-- Poster Image -->
        <FrameLayout
            android:layout_width="match_parent"
            android:layout_height="@dimen/movie_poster_height">

            <ImageView
                android:id="@+id/ivPoster"
                android:layout_width="match_parent"
                android:layout_height="match_parent"
                android:scaleType="centerCrop"
                android:contentDescription="Movie Poster"/>

            <!-- Rating Badge -->
            <LinearLayout
                android:id="@+id/layoutRating"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:gravity="center_vertical"
                android:background="@drawable/bg_rating_badge"
                android:padding="@dimen/spacing_xs"
                android:layout_gravity="top|end"
                android:layout_margin="@dimen/spacing_sm">

                <ImageView
                    android:layout_width="14dp"
                    android:layout_height="14dp"
                    android:src="@drawable/ic_star"
                    android:tint="@color/ratingYellow"/>

                <TextView
                    android:id="@+id/tvRating"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="8.5"
                    android:textSize="@dimen/text_xs"
                    android:textColor="@color/textPrimary"
                    android:textStyle="bold"
                    android:layout_marginStart="@dimen/spacing_xs"/>

            </LinearLayout>

            <!-- Age Rating Badge -->
            <TextView
                android:id="@+id/tvAgeRating"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="C16"
                android:textSize="@dimen/text_xs"
                android:textColor="@color/textPrimary"
                android:textStyle="bold"
                android:background="@drawable/bg_age_rating"
                android:padding="@dimen/spacing_xs"
                android:layout_gravity="bottom|start"
                android:layout_margin="@dimen/spacing_sm"/>

        </FrameLayout>

        <!-- Movie Info -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:padding="@dimen/spacing_sm"
            android:background="@color/cardBackground">

            <TextView
                android:id="@+id/tvTitle"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:text="Avengers: Endgame"
                android:textSize="@dimen/text_md"
                android:textColor="@color/textPrimary"
                android:textStyle="bold"
                android:maxLines="2"
                android:ellipsize="end"/>

            <TextView
                android:id="@+id/tvGenre"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:text="Action, Sci-Fi"
                android:textSize="@dimen/text_xs"
                android:textColor="@color/textSecondary"
                android:layout_marginTop="@dimen/spacing_xs"
                android:maxLines="1"
                android:ellipsize="end"/>

            <TextView
                android:id="@+id/tvDuration"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:text="181 phút"
                android:textSize="@dimen/text_xs"
                android:textColor="@color/textSecondary"
                android:layout_marginTop="@dimen/spacing_xs"/>

        </LinearLayout>

    </LinearLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java Implementation

```java
package com.movie88.ui.components;

import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.cardview.widget.CardView;
import com.bumptech.glide.Glide;
import com.movie88.R;
import com.movie88.data.models.response.Movie;

public class MoviePosterCard extends CardView {
    
    private ImageView ivPoster;
    private TextView tvRating, tvAgeRating, tvTitle, tvGenre, tvDuration;
    
    private OnMovieClickListener listener;
    
    public interface OnMovieClickListener {
        void onMovieClick(Movie movie);
    }
    
    public MoviePosterCard(Context context) {
        super(context);
        init(context);
    }
    
    public MoviePosterCard(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }
    
    private void init(Context context) {
        LayoutInflater.from(context).inflate(
            R.layout.component_movie_poster_card, this, true
        );
        
        ivPoster = findViewById(R.id.ivPoster);
        tvRating = findViewById(R.id.tvRating);
        tvAgeRating = findViewById(R.id.tvAgeRating);
        tvTitle = findViewById(R.id.tvTitle);
        tvGenre = findViewById(R.id.tvGenre);
        tvDuration = findViewById(R.id.tvDuration);
    }
    
    /**
     * Bind movie data to view
     */
    public void bind(Movie movie) {
        // Load poster image
        Glide.with(getContext())
            .load(movie.getPosterUrl())
            .placeholder(R.drawable.placeholder_movie)
            .error(R.drawable.error_image)
            .into(ivPoster);
        
        // Set rating
        if (movie.getRating() > 0) {
            tvRating.setText(String.format("%.1f", movie.getRating()));
        } else {
            tvRating.setText("N/A");
        }
        
        // Set age rating
        tvAgeRating.setText(movie.getAgeRating());
        
        // Set title
        tvTitle.setText(movie.getTitle());
        
        // Set genre
        tvGenre.setText(movie.getGenreNames());
        
        // Set duration
        tvDuration.setText(movie.getDuration() + " phút");
        
        // Click listener
        setOnClickListener(v -> {
            if (listener != null) {
                listener.onMovieClick(movie);
            }
        });
    }
    
    public void setOnMovieClickListener(OnMovieClickListener listener) {
        this.listener = listener;
    }
}
```

### Usage Example

```java
// In Adapter
@Override
public void onBindViewHolder(@NonNull MovieViewHolder holder, int position) {
    Movie movie = movies.get(position);
    
    holder.moviePosterCard.bind(movie);
    holder.moviePosterCard.setOnMovieClickListener(movie -> {
        // Navigate to MovieDetailActivity
        Intent intent = new Intent(context, MovieDetailActivity.class);
        intent.putExtra("MOVIE_ID", movie.getMovieId());
        context.startActivity(intent);
    });
}
```

---

## 2️⃣ SeatView

### Purpose
Component cho từng ghế trong seat selection grid. Hiển thị trạng thái ghế (Available, Selected, Occupied, VIP).

### Layout (component_seat_view.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<FrameLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:id="@+id/seatContainer"
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

### Java Implementation

```java
package com.movie88.ui.components;

import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.widget.FrameLayout;
import android.widget.TextView;
import com.movie88.R;
import com.movie88.data.models.response.Seat;

public class SeatView extends FrameLayout {
    
    private TextView tvSeatNumber;
    private Seat seat;
    private OnSeatClickListener listener;
    
    public interface OnSeatClickListener {
        void onSeatClick(Seat seat);
    }
    
    public SeatView(Context context) {
        super(context);
        init(context);
    }
    
    public SeatView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }
    
    private void init(Context context) {
        LayoutInflater.from(context).inflate(
            R.layout.component_seat_view, this, true
        );
        
        tvSeatNumber = findViewById(R.id.tvSeatNumber);
    }
    
    /**
     * Bind seat data and update UI
     */
    public void bind(Seat seat) {
        this.seat = seat;
        
        // Set seat number
        tvSeatNumber.setText(seat.getSeatNumber());
        
        // Update background based on status
        updateSeatBackground();
        
        // Click listener (only for available seats)
        setOnClickListener(v -> {
            if (seat.getStatus().equals("Available") && listener != null) {
                seat.setSelected(!seat.isSelected());
                updateSeatBackground();
                listener.onSeatClick(seat);
            }
        });
    }
    
    /**
     * Update seat background color based on status
     */
    private void updateSeatBackground() {
        int backgroundRes;
        
        if (seat.isSelected()) {
            // User selected this seat
            backgroundRes = R.drawable.bg_seat_selected;
        } else if (seat.getStatus().equals("Occupied")) {
            // Seat already booked
            backgroundRes = R.drawable.bg_seat_occupied;
            setEnabled(false);
        } else if (seat.getType().equals("VIP")) {
            // VIP seat
            backgroundRes = R.drawable.bg_seat_vip;
        } else if (seat.getType().equals("Couple")) {
            // Couple seat
            backgroundRes = R.drawable.bg_seat_couple;
        } else {
            // Standard available seat
            backgroundRes = R.drawable.bg_seat_available;
        }
        
        tvSeatNumber.setBackgroundResource(backgroundRes);
    }
    
    public void setOnSeatClickListener(OnSeatClickListener listener) {
        this.listener = listener;
    }
    
    public Seat getSeat() {
        return seat;
    }
}
```

---

## 3️⃣ ComboItemCard

### Purpose
Hiển thị food & drink combo với hình ảnh, tên, giá, và quantity selector.

### Layout (component_combo_item_card.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_margin="@dimen/spacing_sm"
    app:cardCornerRadius="@dimen/card_corner_radius"
    app:cardElevation="@dimen/card_elevation">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="horizontal"
        android:padding="@dimen/spacing_md">

        <!-- Combo Image -->
        <ImageView
            android:id="@+id/ivComboImage"
            android:layout_width="80dp"
            android:layout_height="80dp"
            android:scaleType="centerCrop"
            android:contentDescription="Combo Image"/>

        <!-- Combo Info -->
        <LinearLayout
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_weight="1"
            android:orientation="vertical"
            android:layout_marginStart="@dimen/spacing_md">

            <TextView
                android:id="@+id/tvComboName"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:text="Combo Couple"
                android:textSize="@dimen/text_md"
                android:textColor="@color/textPrimary"
                android:textStyle="bold"/>

            <TextView
                android:id="@+id/tvComboDescription"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:text="2 Bắp + 2 Nước"
                android:textSize="@dimen/text_sm"
                android:textColor="@color/textSecondary"
                android:layout_marginTop="@dimen/spacing_xs"
                android:maxLines="2"
                android:ellipsize="end"/>

            <TextView
                android:id="@+id/tvComboPrice"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="120.000 đ"
                android:textSize="@dimen/text_lg"
                android:textColor="@color/colorPrimary"
                android:textStyle="bold"
                android:layout_marginTop="@dimen/spacing_sm"/>

        </LinearLayout>

        <!-- Quantity Selector -->
        <LinearLayout
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:layout_gravity="center_vertical">

            <ImageButton
                android:id="@+id/btnDecrease"
                android:layout_width="32dp"
                android:layout_height="32dp"
                android:src="@drawable/ic_remove"
                android:background="@drawable/bg_circle_button"
                android:contentDescription="Decrease"/>

            <TextView
                android:id="@+id/tvQuantity"
                android:layout_width="40dp"
                android:layout_height="wrap_content"
                android:text="0"
                android:textSize="@dimen/text_md"
                android:textColor="@color/textPrimary"
                android:textStyle="bold"
                android:gravity="center"
                android:layout_marginHorizontal="@dimen/spacing_sm"/>

            <ImageButton
                android:id="@+id/btnIncrease"
                android:layout_width="32dp"
                android:layout_height="32dp"
                android:src="@drawable/ic_add"
                android:background="@drawable/bg_circle_button"
                android:contentDescription="Increase"/>

        </LinearLayout>

    </LinearLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java Implementation

```java
package com.movie88.ui.components;

import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.cardview.widget.CardView;
import com.bumptech.glide.Glide;
import com.movie88.R;
import com.movie88.data.models.response.Combo;
import java.text.DecimalFormat;

public class ComboItemCard extends CardView {
    
    private ImageView ivComboImage;
    private TextView tvComboName, tvComboDescription, tvComboPrice, tvQuantity;
    private ImageButton btnDecrease, btnIncrease;
    
    private Combo combo;
    private int quantity = 0;
    private OnQuantityChangeListener listener;
    
    public interface OnQuantityChangeListener {
        void onQuantityChanged(Combo combo, int quantity);
    }
    
    public ComboItemCard(Context context) {
        super(context);
        init(context);
    }
    
    public ComboItemCard(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }
    
    private void init(Context context) {
        LayoutInflater.from(context).inflate(
            R.layout.component_combo_item_card, this, true
        );
        
        ivComboImage = findViewById(R.id.ivComboImage);
        tvComboName = findViewById(R.id.tvComboName);
        tvComboDescription = findViewById(R.id.tvComboDescription);
        tvComboPrice = findViewById(R.id.tvComboPrice);
        tvQuantity = findViewById(R.id.tvQuantity);
        btnDecrease = findViewById(R.id.btnDecrease);
        btnIncrease = findViewById(R.id.btnIncrease);
        
        setupListeners();
    }
    
    private void setupListeners() {
        btnDecrease.setOnClickListener(v -> {
            if (quantity > 0) {
                quantity--;
                updateQuantity();
            }
        });
        
        btnIncrease.setOnClickListener(v -> {
            if (quantity < 10) { // Max 10 combos
                quantity++;
                updateQuantity();
            }
        });
    }
    
    /**
     * Bind combo data to view
     */
    public void bind(Combo combo) {
        this.combo = combo;
        
        // Load combo image
        Glide.with(getContext())
            .load(combo.getImageUrl())
            .placeholder(R.drawable.placeholder_combo)
            .error(R.drawable.error_image)
            .into(ivComboImage);
        
        // Set combo info
        tvComboName.setText(combo.getName());
        tvComboDescription.setText(combo.getDescription());
        
        // Format price
        DecimalFormat formatter = new DecimalFormat("#,###");
        tvComboPrice.setText(formatter.format(combo.getPrice()) + " đ");
        
        // Reset quantity
        quantity = 0;
        tvQuantity.setText(String.valueOf(quantity));
        
        updateButtonStates();
    }
    
    private void updateQuantity() {
        tvQuantity.setText(String.valueOf(quantity));
        updateButtonStates();
        
        if (listener != null) {
            listener.onQuantityChanged(combo, quantity);
        }
    }
    
    private void updateButtonStates() {
        btnDecrease.setEnabled(quantity > 0);
        btnIncrease.setEnabled(quantity < 10);
    }
    
    public void setOnQuantityChangeListener(OnQuantityChangeListener listener) {
        this.listener = listener;
    }
    
    public int getQuantity() {
        return quantity;
    }
    
    public Combo getCombo() {
        return combo;
    }
}
```

---

## 4️⃣ LoadingDialog

### Purpose
Fullscreen loading indicator với message.

### Implementation

```java
package com.movie88.ui.components;

import android.app.Dialog;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.TextView;
import androidx.annotation.NonNull;
import com.movie88.R;

public class LoadingDialog extends Dialog {
    
    private TextView tvMessage;
    
    public LoadingDialog(@NonNull Context context) {
        super(context, R.style.TransparentDialog);
        
        View view = LayoutInflater.from(context).inflate(
            R.layout.dialog_loading, null
        );
        
        tvMessage = view.findViewById(R.id.tvMessage);
        
        setContentView(view);
        setCancelable(false);
    }
    
    public void setMessage(String message) {
        tvMessage.setText(message);
    }
}
```

### Layout (dialog_loading.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:orientation="vertical"
    android:gravity="center"
    android:background="@drawable/bg_dialog"
    android:padding="@dimen/spacing_xl">

    <ProgressBar
        android:layout_width="60dp"
        android:layout_height="60dp"
        android:indeterminateTint="@color/colorPrimary"/>

    <TextView
        android:id="@+id/tvMessage"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Đang tải..."
        android:textSize="@dimen/text_md"
        android:textColor="@color/textPrimary"
        android:layout_marginTop="@dimen/spacing_md"/>

</LinearLayout>
```

### Usage

```java
// Show loading
LoadingDialog loadingDialog = new LoadingDialog(this);
loadingDialog.setMessage("Đang xử lý thanh toán...");
loadingDialog.show();

// Hide loading
loadingDialog.dismiss();
```

---

## 5️⃣ EmptyStateView

### Purpose
Hiển thị empty state với icon, message, và action button.

### Layout (component_empty_state.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:gravity="center"
    android:padding="@dimen/spacing_xl">

    <ImageView
        android:id="@+id/ivEmptyIcon"
        android:layout_width="120dp"
        android:layout_height="120dp"
        android:src="@drawable/ic_empty_box"
        android:tint="@color/textSecondary"/>

    <TextView
        android:id="@+id/tvEmptyTitle"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Không có dữ liệu"
        android:textSize="@dimen/text_lg"
        android:textColor="@color/textPrimary"
        android:textStyle="bold"
        android:layout_marginTop="@dimen/spacing_lg"/>

    <TextView
        android:id="@+id/tvEmptyMessage"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Chưa có dữ liệu để hiển thị"
        android:textSize="@dimen/text_md"
        android:textColor="@color/textSecondary"
        android:textAlignment="center"
        android:layout_marginTop="@dimen/spacing_sm"/>

    <com.google.android.material.button.MaterialButton
        android:id="@+id/btnAction"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Thử lại"
        android:textSize="@dimen/text_md"
        android:layout_marginTop="@dimen/spacing_lg"
        android:visibility="gone"/>

</LinearLayout>
```

### Usage

```java
// In Activity/Fragment
View emptyStateView = findViewById(R.id.emptyStateView);
ImageView ivEmptyIcon = emptyStateView.findViewById(R.id.ivEmptyIcon);
TextView tvEmptyTitle = emptyStateView.findViewById(R.id.tvEmptyTitle);
TextView tvEmptyMessage = emptyStateView.findViewById(R.id.tvEmptyMessage);
MaterialButton btnAction = emptyStateView.findViewById(R.id.btnAction);

// Configure empty state
ivEmptyIcon.setImageResource(R.drawable.ic_no_bookings);
tvEmptyTitle.setText("Chưa có đặt vé nào");
tvEmptyMessage.setText("Bạn chưa đặt vé xem phim nào");
btnAction.setVisibility(View.VISIBLE);
btnAction.setText("Đặt vé ngay");
btnAction.setOnClickListener(v -> {
    // Navigate to movie list
});

// Show/hide empty state
emptyStateView.setVisibility(bookings.isEmpty() ? View.VISIBLE : View.GONE);
recyclerView.setVisibility(bookings.isEmpty() ? View.GONE : View.VISIBLE);
```

---

## 🎯 Best Practices

### ✅ DO:
1. **Reusable Components** - Tạo component khi một UI pattern được sử dụng ≥ 3 lần
2. **Custom Attributes** - Dùng attrs.xml cho customizable properties
3. **Separation of Concerns** - Component chỉ lo UI, logic ở ViewModel
4. **Lifecycle Aware** - Clean up resources trong onDetachedFromWindow()

### ❌ DON'T:
1. **Hardcode Values** - Dùng dimens.xml và colors.xml
2. **Business Logic in Components** - Logic phải ở ViewModel/Repository
3. **Direct API Calls** - Components không được gọi API trực tiếp

---

**Last Updated**: October 29, 2025
