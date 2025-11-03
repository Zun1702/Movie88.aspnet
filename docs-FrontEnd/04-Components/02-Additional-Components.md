# 🎨 Additional Reusable Components

## Tổng quan

Các components bổ sung: BookingCard, ReviewCard.

---

## 1️⃣ BookingCard Component

### Purpose
Display booking information trong booking history list.

### Layout: `item_booking_card.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    xmlns:tools="http://schemas.android.com/tools"
    android:id="@+id/cardBooking"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_marginHorizontal="@dimen/spacing_medium"
    android:layout_marginVertical="@dimen/spacing_small"
    app:cardCornerRadius="@dimen/card_corner_radius_medium"
    app:cardElevation="@dimen/elevation_small"
    app:strokeWidth="0dp"
    android:clickable="true"
    android:focusable="true"
    android:foreground="?attr/selectableItemBackground">

    <androidx.constraintlayout.widget.ConstraintLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_medium">

        <!-- Movie Poster -->
        <com.google.android.material.card.MaterialCardView
            android:id="@+id/cardPoster"
            android:layout_width="@dimen/poster_width_small"
            android:layout_height="@dimen/poster_height_small"
            app:cardCornerRadius="@dimen/card_corner_radius_small"
            app:cardElevation="@dimen/elevation_small"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toTopOf="parent">

            <ImageView
                android:id="@+id/ivPoster"
                android:layout_width="match_parent"
                android:layout_height="match_parent"
                android:scaleType="centerCrop"
                tools:src="@drawable/placeholder_movie" />

        </com.google.android.material.card.MaterialCardView>

        <!-- Movie Title -->
        <TextView
            android:id="@+id/tvMovieTitle"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginStart="@dimen/spacing_medium"
            android:textAppearance="@style/TextAppearance.App.Heading3"
            android:textColor="@color/textPrimary"
            android:maxLines="2"
            android:ellipsize="end"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toEndOf="@id/cardPoster"
            app:layout_constraintTop_toTopOf="@id/cardPoster"
            tools:text="Avatar: The Way of Water" />

        <!-- Cinema Name -->
        <TextView
            android:id="@+id/tvCinemaName"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Body"
            android:textColor="@color/textSecondary"
            android:maxLines="1"
            android:ellipsize="end"
            android:drawableStart="@drawable/ic_location"
            android:drawableTint="@color/iconSecondary"
            android:drawablePadding="@dimen/spacing_xs"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="@id/tvMovieTitle"
            app:layout_constraintTop_toBottomOf="@id/tvMovieTitle"
            tools:text="CGV Vincom" />

        <!-- Showtime -->
        <TextView
            android:id="@+id/tvShowtime"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Body"
            android:textColor="@color/textSecondary"
            android:maxLines="1"
            android:ellipsize="end"
            android:drawableStart="@drawable/ic_calendar"
            android:drawableTint="@color/iconSecondary"
            android:drawablePadding="@dimen/spacing_xs"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="@id/tvMovieTitle"
            app:layout_constraintTop_toBottomOf="@id/tvCinemaName"
            tools:text="29/10/2025 - 19:30" />

        <!-- Seats -->
        <TextView
            android:id="@+id/tvSeats"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Body"
            android:textColor="@color/textSecondary"
            android:maxLines="1"
            android:ellipsize="end"
            android:drawableStart="@drawable/ic_seat"
            android:drawableTint="@color/iconSecondary"
            android:drawablePadding="@dimen/spacing_xs"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="@id/tvMovieTitle"
            app:layout_constraintTop_toBottomOf="@id/tvShowtime"
            tools:text="A1, A2, A3" />

        <!-- Divider -->
        <View
            android:id="@+id/divider"
            android:layout_width="0dp"
            android:layout_height="1dp"
            android:layout_marginTop="@dimen/spacing_medium"
            android:background="@color/divider"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/cardPoster" />

        <!-- Status Badge -->
        <com.google.android.material.card.MaterialCardView
            android:id="@+id/cardStatus"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_medium"
            app:cardCornerRadius="@dimen/card_corner_radius_small"
            app:cardElevation="0dp"
            app:strokeWidth="0dp"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/divider">

            <TextView
                android:id="@+id/tvStatus"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:paddingHorizontal="@dimen/spacing_small"
                android:paddingVertical="@dimen/spacing_xs"
                android:textAppearance="@style/TextAppearance.App.Caption"
                android:textColor="@color/white"
                android:textStyle="bold"
                tools:text="Đã thanh toán"
                tools:background="@color/statusSuccess" />

        </com.google.android.material.card.MaterialCardView>

        <!-- Total Price -->
        <TextView
            android:id="@+id/tvTotalPrice"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:textAppearance="@style/TextAppearance.App.Heading3"
            android:textColor="@color/colorPrimary"
            app:layout_constraintBottom_toBottomOf="@id/cardStatus"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintTop_toTopOf="@id/cardStatus"
            tools:text="300.000đ" />

        <!-- Action Buttons -->
        <LinearLayout
            android:id="@+id/layoutActions"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_medium"
            android:orientation="horizontal"
            android:gravity="end"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/cardStatus">

            <!-- Cancel Button (visible for pending bookings) -->
            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnCancel"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="@string/cancel"
                android:textColor="@color/statusError"
                style="@style/Widget.Material3.Button.OutlinedButton"
                app:strokeColor="@color/statusError"
                app:icon="@drawable/ic_close"
                app:iconTint="@color/statusError"
                android:visibility="gone"
                tools:visibility="visible" />

            <!-- View Details Button -->
            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnViewDetails"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginStart="@dimen/spacing_small"
                android:text="@string/view_details"
                style="@style/Widget.Material3.Button"
                app:icon="@drawable/ic_chevron_right" />

        </LinearLayout>

    </androidx.constraintlayout.widget.ConstraintLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java: `BookingCard.java`

```java
package com.movie88.ui.components;

import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.bumptech.glide.Glide;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.card.MaterialCardView;
import com.movie88.R;
import com.movie88.data.models.response.Booking;
import com.movie88.utils.CurrencyUtils;
import com.movie88.utils.DateUtils;

public class BookingCard extends MaterialCardView {

    private ImageView ivPoster;
    private TextView tvMovieTitle;
    private TextView tvCinemaName;
    private TextView tvShowtime;
    private TextView tvSeats;
    private MaterialCardView cardStatus;
    private TextView tvStatus;
    private TextView tvTotalPrice;
    private MaterialButton btnCancel;
    private MaterialButton btnViewDetails;

    private OnBookingCardClickListener listener;

    public interface OnBookingCardClickListener {
        void onViewDetailsClick(Booking booking);
        void onCancelClick(Booking booking);
    }

    public BookingCard(@NonNull Context context) {
        super(context);
        init(context);
    }

    public BookingCard(@NonNull Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }

    public BookingCard(@NonNull Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init(context);
    }

    private void init(Context context) {
        LayoutInflater.from(context).inflate(R.layout.item_booking_card, this, true);
        
        ivPoster = findViewById(R.id.ivPoster);
        tvMovieTitle = findViewById(R.id.tvMovieTitle);
        tvCinemaName = findViewById(R.id.tvCinemaName);
        tvShowtime = findViewById(R.id.tvShowtime);
        tvSeats = findViewById(R.id.tvSeats);
        cardStatus = findViewById(R.id.cardStatus);
        tvStatus = findViewById(R.id.tvStatus);
        tvTotalPrice = findViewById(R.id.tvTotalPrice);
        btnCancel = findViewById(R.id.btnCancel);
        btnViewDetails = findViewById(R.id.btnViewDetails);
    }

    public void setBooking(Booking booking) {
        // Load poster
        Glide.with(getContext())
            .load(booking.getMovie().getPosterUrl())
            .placeholder(R.drawable.placeholder_movie)
            .error(R.drawable.placeholder_movie)
            .into(ivPoster);

        // Set movie info
        tvMovieTitle.setText(booking.getMovie().getTitle());
        tvCinemaName.setText(booking.getCinema().getName());
        
        // Format showtime
        String showtimeStr = DateUtils.formatDate(booking.getShowtime().getStartTime(), "dd/MM/yyyy - HH:mm");
        tvShowtime.setText(showtimeStr);
        
        // Format seats
        String seatsStr = String.join(", ", booking.getSeatNames());
        tvSeats.setText(seatsStr);
        
        // Set status
        setStatus(booking.getStatus());
        
        // Set total price
        tvTotalPrice.setText(CurrencyUtils.formatCurrency(booking.getTotalPrice()));
        
        // Setup buttons
        setupButtons(booking);
    }

    private void setStatus(String status) {
        int backgroundColor;
        String statusText;
        
        switch (status.toLowerCase()) {
            case "pending":
                backgroundColor = getContext().getColor(R.color.statusWarning);
                statusText = getContext().getString(R.string.status_pending);
                break;
            case "confirmed":
            case "paid":
                backgroundColor = getContext().getColor(R.color.statusSuccess);
                statusText = getContext().getString(R.string.status_confirmed);
                break;
            case "cancelled":
                backgroundColor = getContext().getColor(R.color.statusError);
                statusText = getContext().getString(R.string.status_cancelled);
                break;
            case "completed":
                backgroundColor = getContext().getColor(R.color.statusInfo);
                statusText = getContext().getString(R.string.status_completed);
                break;
            default:
                backgroundColor = getContext().getColor(R.color.textSecondary);
                statusText = status;
                break;
        }
        
        cardStatus.setCardBackgroundColor(backgroundColor);
        tvStatus.setText(statusText);
    }

    private void setupButtons(Booking booking) {
        // Show cancel button only for pending bookings
        if ("pending".equalsIgnoreCase(booking.getStatus())) {
            btnCancel.setVisibility(VISIBLE);
            btnCancel.setOnClickListener(v -> {
                if (listener != null) {
                    listener.onCancelClick(booking);
                }
            });
        } else {
            btnCancel.setVisibility(GONE);
        }
        
        // View details button
        btnViewDetails.setOnClickListener(v -> {
            if (listener != null) {
                listener.onViewDetailsClick(booking);
            }
        });
    }

    public void setOnBookingCardClickListener(OnBookingCardClickListener listener) {
        this.listener = listener;
    }
}
```

### Usage Example

```java
// In Adapter
public class BookingsAdapter extends RecyclerView.Adapter<BookingsAdapter.ViewHolder> {

    private List<Booking> bookings;
    private OnBookingCardClickListener listener;

    public interface OnBookingCardClickListener {
        void onViewDetailsClick(Booking booking);
        void onCancelClick(Booking booking);
    }

    public BookingsAdapter(List<Booking> bookings, OnBookingCardClickListener listener) {
        this.bookings = bookings;
        this.listener = listener;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        BookingCard bookingCard = new BookingCard(parent.getContext());
        return new ViewHolder(bookingCard);
    }

    @Override
    public void onBindViewHolder(ViewHolder holder, int position) {
        Booking booking = bookings.get(position);
        holder.bookingCard.setBooking(booking);
        holder.bookingCard.setOnBookingCardClickListener(new BookingCard.OnBookingCardClickListener() {
            @Override
            public void onViewDetailsClick(Booking booking) {
                if (listener != null) {
                    listener.onViewDetailsClick(booking);
                }
            }

            @Override
            public void onCancelClick(Booking booking) {
                if (listener != null) {
                    listener.onCancelClick(booking);
                }
            }
        });
    }

    @Override
    public int getItemCount() {
        return bookings.size();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        BookingCard bookingCard;

        ViewHolder(BookingCard bookingCard) {
            super(bookingCard);
            this.bookingCard = bookingCard;
        }
    }
}
```

---

## 2️⃣ ReviewCard Component

### Purpose
Display review trong movie detail screen và review list.

### Layout: `item_review_card.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    xmlns:tools="http://schemas.android.com/tools"
    android:id="@+id/cardReview"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_marginHorizontal="@dimen/spacing_medium"
    android:layout_marginVertical="@dimen/spacing_small"
    app:cardCornerRadius="@dimen/card_corner_radius_medium"
    app:cardElevation="@dimen/elevation_small"
    app:strokeWidth="0dp">

    <androidx.constraintlayout.widget.ConstraintLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_medium">

        <!-- User Avatar -->
        <com.google.android.material.card.MaterialCardView
            android:id="@+id/cardAvatar"
            android:layout_width="@dimen/avatar_size_medium"
            android:layout_height="@dimen/avatar_size_medium"
            app:cardCornerRadius="@dimen/avatar_size_medium"
            app:cardElevation="@dimen/elevation_small"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toTopOf="parent">

            <ImageView
                android:id="@+id/ivAvatar"
                android:layout_width="match_parent"
                android:layout_height="match_parent"
                android:scaleType="centerCrop"
                tools:src="@drawable/placeholder_avatar" />

        </com.google.android.material.card.MaterialCardView>

        <!-- User Name -->
        <TextView
            android:id="@+id/tvUserName"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginStart="@dimen/spacing_medium"
            android:textAppearance="@style/TextAppearance.App.Heading3"
            android:textColor="@color/textPrimary"
            android:maxLines="1"
            android:ellipsize="end"
            app:layout_constraintEnd_toStartOf="@id/layoutRating"
            app:layout_constraintStart_toEndOf="@id/cardAvatar"
            app:layout_constraintTop_toTopOf="@id/cardAvatar"
            tools:text="Nguyễn Văn A" />

        <!-- Timestamp -->
        <TextView
            android:id="@+id/tvTimestamp"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:textAppearance="@style/TextAppearance.App.Caption"
            android:textColor="@color/textSecondary"
            android:maxLines="1"
            android:ellipsize="end"
            app:layout_constraintEnd_toEndOf="@id/tvUserName"
            app:layout_constraintStart_toStartOf="@id/tvUserName"
            app:layout_constraintTop_toBottomOf="@id/tvUserName"
            tools:text="2 ngày trước" />

        <!-- Rating Stars -->
        <LinearLayout
            android:id="@+id/layoutRating"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintTop_toTopOf="@id/cardAvatar"
            app:layout_constraintBottom_toBottomOf="@id/cardAvatar">

            <ImageView
                android:id="@+id/ivStar1"
                android:layout_width="@dimen/icon_size_sm"
                android:layout_height="@dimen/icon_size_sm"
                android:src="@drawable/ic_star_filled"
                app:tint="@color/ratingStarFilled" />

            <ImageView
                android:id="@+id/ivStar2"
                android:layout_width="@dimen/icon_size_sm"
                android:layout_height="@dimen/icon_size_sm"
                android:layout_marginStart="2dp"
                android:src="@drawable/ic_star_filled"
                app:tint="@color/ratingStarFilled" />

            <ImageView
                android:id="@+id/ivStar3"
                android:layout_width="@dimen/icon_size_sm"
                android:layout_height="@dimen/icon_size_sm"
                android:layout_marginStart="2dp"
                android:src="@drawable/ic_star_filled"
                app:tint="@color/ratingStarFilled" />

            <ImageView
                android:id="@+id/ivStar4"
                android:layout_width="@dimen/icon_size_sm"
                android:layout_height="@dimen/icon_size_sm"
                android:layout_marginStart="2dp"
                android:src="@drawable/ic_star_filled"
                app:tint="@color/ratingStarFilled" />

            <ImageView
                android:id="@+id/ivStar5"
                android:layout_width="@dimen/icon_size_sm"
                android:layout_height="@dimen/icon_size_sm"
                android:layout_marginStart="2dp"
                android:src="@drawable/ic_star_outlined"
                app:tint="@color/ratingStarEmpty" />

        </LinearLayout>

        <!-- Review Comment -->
        <TextView
            android:id="@+id/tvComment"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_medium"
            android:textAppearance="@style/TextAppearance.App.Body"
            android:textColor="@color/textPrimary"
            android:lineSpacingExtra="@dimen/line_spacing_extra"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/cardAvatar"
            tools:text="Phim hay, hình ảnh đẹp, kịch bản hấp dẫn. Rất đáng xem!" />

        <!-- Read More Button (shown if text is truncated) -->
        <TextView
            android:id="@+id/tvReadMore"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_xs"
            android:text="@string/read_more"
            android:textAppearance="@style/TextAppearance.App.Caption"
            android:textColor="@color/colorPrimary"
            android:textStyle="bold"
            android:visibility="gone"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/tvComment"
            tools:visibility="visible" />

        <!-- Action Buttons -->
        <LinearLayout
            android:id="@+id/layoutActions"
            android:layout_width="0dp"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_medium"
            android:orientation="horizontal"
            android:gravity="start"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintTop_toBottomOf="@id/tvReadMore">

            <!-- Helpful Button -->
            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnHelpful"
                android:layout_width="wrap_content"
                android:layout_height="36dp"
                android:text="@string/helpful"
                android:textColor="@color/textSecondary"
                android:minHeight="0dp"
                style="@style/Widget.Material3.Button.TextButton"
                app:icon="@drawable/ic_thumb_up"
                app:iconTint="@color/iconSecondary"
                app:iconSize="@dimen/icon_size_sm" />

            <!-- Helpful Count -->
            <TextView
                android:id="@+id/tvHelpfulCount"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_gravity="center_vertical"
                android:layout_marginStart="@dimen/spacing_xs"
                android:textAppearance="@style/TextAppearance.App.Caption"
                android:textColor="@color/textSecondary"
                android:visibility="gone"
                tools:text="12"
                tools:visibility="visible" />

            <!-- Report Button -->
            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnReport"
                android:layout_width="wrap_content"
                android:layout_height="36dp"
                android:layout_marginStart="@dimen/spacing_small"
                android:text="@string/report"
                android:textColor="@color/textSecondary"
                android:minHeight="0dp"
                style="@style/Widget.Material3.Button.TextButton"
                app:icon="@drawable/ic_flag"
                app:iconTint="@color/iconSecondary"
                app:iconSize="@dimen/icon_size_sm" />

        </LinearLayout>

    </androidx.constraintlayout.widget.ConstraintLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java: `ReviewCard.java`

```java
package com.movie88.ui.components;

import android.content.Context;
import android.text.TextUtils;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.bumptech.glide.Glide;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.card.MaterialCardView;
import com.movie88.R;
import com.movie88.data.models.response.Review;
import com.movie88.utils.DateUtils;

public class ReviewCard extends MaterialCardView {

    private ImageView ivAvatar;
    private TextView tvUserName;
    private TextView tvTimestamp;
    private ImageView[] stars;
    private TextView tvComment;
    private TextView tvReadMore;
    private MaterialButton btnHelpful;
    private TextView tvHelpfulCount;
    private MaterialButton btnReport;

    private OnReviewCardClickListener listener;
    private Review review;
    private boolean isExpanded = false;
    private static final int MAX_LINES_COLLAPSED = 3;

    public interface OnReviewCardClickListener {
        void onHelpfulClick(Review review);
        void onReportClick(Review review);
    }

    public ReviewCard(@NonNull Context context) {
        super(context);
        init(context);
    }

    public ReviewCard(@NonNull Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }

    public ReviewCard(@NonNull Context context, @Nullable AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init(context);
    }

    private void init(Context context) {
        LayoutInflater.from(context).inflate(R.layout.item_review_card, this, true);
        
        ivAvatar = findViewById(R.id.ivAvatar);
        tvUserName = findViewById(R.id.tvUserName);
        tvTimestamp = findViewById(R.id.tvTimestamp);
        
        stars = new ImageView[5];
        stars[0] = findViewById(R.id.ivStar1);
        stars[1] = findViewById(R.id.ivStar2);
        stars[2] = findViewById(R.id.ivStar3);
        stars[3] = findViewById(R.id.ivStar4);
        stars[4] = findViewById(R.id.ivStar5);
        
        tvComment = findViewById(R.id.tvComment);
        tvReadMore = findViewById(R.id.tvReadMore);
        btnHelpful = findViewById(R.id.btnHelpful);
        tvHelpfulCount = findViewById(R.id.tvHelpfulCount);
        btnReport = findViewById(R.id.btnReport);
        
        setupReadMore();
    }

    public void setReview(Review review) {
        this.review = review;
        
        // Load avatar
        Glide.with(getContext())
            .load(review.getUser().getAvatarUrl())
            .placeholder(R.drawable.placeholder_avatar)
            .error(R.drawable.placeholder_avatar)
            .circleCrop()
            .into(ivAvatar);

        // Set user info
        tvUserName.setText(review.getUser().getFullName());
        
        // Format timestamp
        String timeAgo = DateUtils.getTimeAgo(review.getCreatedAt());
        tvTimestamp.setText(timeAgo);
        
        // Set rating stars
        setRating(review.getRating());
        
        // Set comment
        tvComment.setText(review.getComment());
        tvComment.setMaxLines(MAX_LINES_COLLAPSED);
        tvComment.setEllipsize(TextUtils.TruncateAt.END);
        
        // Check if comment needs read more
        tvComment.post(() -> {
            if (tvComment.getLineCount() > MAX_LINES_COLLAPSED) {
                tvReadMore.setVisibility(VISIBLE);
            } else {
                tvReadMore.setVisibility(GONE);
            }
        });
        
        // Set helpful count
        if (review.getHelpfulCount() > 0) {
            tvHelpfulCount.setVisibility(VISIBLE);
            tvHelpfulCount.setText(String.valueOf(review.getHelpfulCount()));
        } else {
            tvHelpfulCount.setVisibility(GONE);
        }
        
        // Update helpful button state
        updateHelpfulButton(review.isMarkedHelpful());
        
        setupButtons();
    }

    private void setRating(int rating) {
        for (int i = 0; i < stars.length; i++) {
            if (i < rating) {
                stars[i].setImageResource(R.drawable.ic_star_filled);
                stars[i].setImageTintList(getContext().getColorStateList(R.color.ratingStarFilled));
            } else {
                stars[i].setImageResource(R.drawable.ic_star_outlined);
                stars[i].setImageTintList(getContext().getColorStateList(R.color.ratingStarEmpty));
            }
        }
    }

    private void setupReadMore() {
        tvReadMore.setOnClickListener(v -> {
            if (isExpanded) {
                // Collapse
                tvComment.setMaxLines(MAX_LINES_COLLAPSED);
                tvComment.setEllipsize(TextUtils.TruncateAt.END);
                tvReadMore.setText(R.string.read_more);
                isExpanded = false;
            } else {
                // Expand
                tvComment.setMaxLines(Integer.MAX_VALUE);
                tvComment.setEllipsize(null);
                tvReadMore.setText(R.string.read_less);
                isExpanded = true;
            }
        });
    }

    private void setupButtons() {
        btnHelpful.setOnClickListener(v -> {
            if (listener != null) {
                listener.onHelpfulClick(review);
            }
        });
        
        btnReport.setOnClickListener(v -> {
            if (listener != null) {
                listener.onReportClick(review);
            }
        });
    }

    private void updateHelpfulButton(boolean isMarkedHelpful) {
        if (isMarkedHelpful) {
            btnHelpful.setIconTintResource(R.color.colorPrimary);
            btnHelpful.setTextColor(getContext().getColor(R.color.colorPrimary));
        } else {
            btnHelpful.setIconTintResource(R.color.iconSecondary);
            btnHelpful.setTextColor(getContext().getColor(R.color.textSecondary));
        }
    }

    public void setOnReviewCardClickListener(OnReviewCardClickListener listener) {
        this.listener = listener;
    }
}
```

### Additional DateUtils Method

```java
// Add to DateUtils.java

public static String getTimeAgo(Date date) {
    long timeInMillis = date.getTime();
    long currentTimeInMillis = System.currentTimeMillis();
    long diffInMillis = currentTimeInMillis - timeInMillis;
    
    long seconds = diffInMillis / 1000;
    long minutes = seconds / 60;
    long hours = minutes / 60;
    long days = hours / 24;
    long weeks = days / 7;
    long months = days / 30;
    long years = days / 365;
    
    if (seconds < 60) {
        return "Vừa xong";
    } else if (minutes < 60) {
        return minutes + " phút trước";
    } else if (hours < 24) {
        return hours + " giờ trước";
    } else if (days < 7) {
        return days + " ngày trước";
    } else if (weeks < 4) {
        return weeks + " tuần trước";
    } else if (months < 12) {
        return months + " tháng trước";
    } else {
        return years + " năm trước";
    }
}
```

### String Resources

```xml
<!-- Review -->
<string name="read_more">Xem thêm</string>
<string name="read_less">Thu gọn</string>
<string name="helpful">Hữu ích</string>
<string name="report">Báo cáo</string>

<!-- Booking Status -->
<string name="status_pending">Chờ thanh toán</string>
<string name="status_confirmed">Đã thanh toán</string>
<string name="status_cancelled">Đã hủy</string>
<string name="status_completed">Đã hoàn thành</string>
<string name="view_details">Xem chi tiết</string>
<string name="cancel">Hủy</string>
```

---

**Last Updated**: October 29, 2025
