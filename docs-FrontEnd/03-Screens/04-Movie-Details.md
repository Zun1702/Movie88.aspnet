# 🎬 Movie Details Screens

## Tổng quan

Chi tiết các màn hình hiển thị thông tin phim: MovieDetailActivity, MovieTrailerActivity, SearchMovieActivity.

---

## 1️⃣ MovieDetailActivity

### Purpose
Hiển thị chi tiết đầy đủ về phim: poster, trailer, thông tin, suất chiếu, đánh giá.

### Layout: activity_movie_detail.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.coordinatorlayout.widget.CoordinatorLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <!-- AppBarLayout with collapsing poster image -->
    <com.google.android.material.appbar.AppBarLayout
        android:id="@+id/appBarLayout"
        android:layout_width="match_parent"
        android:layout_height="360dp"
        android:theme="@style/ThemeOverlay.AppCompat.Dark.ActionBar">

        <com.google.android.material.appbar.CollapsingToolbarLayout
            android:id="@+id/collapsingToolbar"
            android:layout_width="match_parent"
            android:layout_height="match_parent"
            app:layout_scrollFlags="scroll|exitUntilCollapsed"
            app:contentScrim="@color/colorPrimary"
            app:expandedTitleMarginStart="16dp"
            app:expandedTitleMarginEnd="64dp">

            <!-- Backdrop Image -->
            <ImageView
                android:id="@+id/ivBackdrop"
                android:layout_width="match_parent"
                android:layout_height="match_parent"
                android:scaleType="centerCrop"
                app:layout_collapseMode="parallax"/>

            <!-- Gradient Overlay -->
            <View
                android:layout_width="match_parent"
                android:layout_height="120dp"
                android:layout_gravity="bottom"
                android:background="@drawable/bg_gradient_bottom"/>

            <!-- Toolbar -->
            <androidx.appcompat.widget.Toolbar
                android:id="@+id/toolbar"
                android:layout_width="match_parent"
                android:layout_height="?attr/actionBarSize"
                app:layout_collapseMode="pin"
                app:navigationIcon="@drawable/ic_back"
                app:popupTheme="@style/ThemeOverlay.AppCompat.Light"/>

        </com.google.android.material.appbar.CollapsingToolbarLayout>

    </com.google.android.material.appbar.AppBarLayout>

    <!-- Content ScrollView -->
    <androidx.core.widget.NestedScrollView
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        app:layout_behavior="@string/appbar_scrolling_view_behavior">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:padding="@dimen/spacing_base">

            <!-- Movie Title & Info Section -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:layout_marginBottom="@dimen/spacing_base">

                <!-- Movie Poster -->
                <com.google.android.material.card.MaterialCardView
                    android:layout_width="120dp"
                    android:layout_height="170dp"
                    app:cardCornerRadius="@dimen/poster_corner_radius"
                    app:cardElevation="@dimen/card_elevation">

                    <ImageView
                        android:id="@+id/ivPoster"
                        android:layout_width="match_parent"
                        android:layout_height="match_parent"
                        android:scaleType="centerCrop"/>

                </com.google.android.material.card.MaterialCardView>

                <!-- Movie Info -->
                <LinearLayout
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:layout_marginStart="@dimen/spacing_base"
                    android:orientation="vertical">

                    <!-- Title -->
                    <TextView
                        android:id="@+id/tvTitle"
                        style="@style/Text.Heading2"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="Avengers: Endgame"/>

                    <!-- Rating -->
                    <LinearLayout
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:orientation="horizontal"
                        android:gravity="center_vertical">

                        <ImageView
                            android:layout_width="16dp"
                            android:layout_height="16dp"
                            android:src="@drawable/ic_star"
                            app:tint="@color/ratingStar"/>

                        <TextView
                            android:id="@+id/tvRating"
                            style="@style/Text.Body"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginStart="4dp"
                            android:text="8.4"/>

                        <TextView
                            style="@style/Text.BodySecondary"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:layout_marginStart="4dp"
                            android:text="/10"/>

                    </LinearLayout>

                    <!-- Genre -->
                    <TextView
                        android:id="@+id/tvGenre"
                        style="@style/Text.BodySecondary"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:text="Action, Adventure, Sci-Fi"/>

                    <!-- Duration & Release Date -->
                    <TextView
                        android:id="@+id/tvDuration"
                        style="@style/Text.BodySecondary"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:text="3h 1m • 2025"/>

                    <!-- Age Rating Badge -->
                    <TextView
                        android:id="@+id/tvAgeRating"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_sm"
                        android:paddingHorizontal="8dp"
                        android:paddingVertical="4dp"
                        android:background="@drawable/bg_age_rating"
                        android:textColor="@color/textPrimary"
                        android:textSize="12sp"
                        android:textStyle="bold"
                        android:text="C16"/>

                </LinearLayout>

            </LinearLayout>

            <!-- Action Buttons -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:layout_marginBottom="@dimen/spacing_lg">

                <!-- Book Now Button -->
                <com.google.android.material.button.MaterialButton
                    android:id="@+id/btnBookNow"
                    style="@style/Button.Primary"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="@string/movie_detail_book_now"
                    app:icon="@drawable/ic_ticket"
                    app:iconGravity="textStart"/>

                <!-- Watch Trailer Button -->
                <com.google.android.material.button.MaterialButton
                    android:id="@+id/btnWatchTrailer"
                    style="@style/Button.Outlined"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:layout_marginStart="@dimen/spacing_sm"
                    android:text="@string/movie_detail_watch_trailer"
                    app:icon="@drawable/ic_play"
                    app:iconGravity="textStart"/>

            </LinearLayout>

            <!-- Overview Section -->
            <TextView
                style="@style/Text.Heading3"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_sm"
                android:text="@string/movie_detail_overview"/>

            <TextView
                android:id="@+id/tvOverview"
                style="@style/Text.Body"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_lg"
                android:lineSpacingExtra="4dp"
                android:text="After the devastating events of Avengers: Infinity War, the universe is in ruins..."/>

            <!-- Showtimes Section -->
            <TextView
                style="@style/Text.Heading3"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_sm"
                android:text="Suất chiếu"/>

            <!-- Date Selector -->
            <HorizontalScrollView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_sm"
                android:scrollbars="none">

                <com.google.android.material.chip.ChipGroup
                    android:id="@+id/chipGroupDates"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    app:singleSelection="true"
                    app:selectionRequired="true">

                    <com.google.android.material.chip.Chip
                        android:id="@+id/chipToday"
                        style="@style/Chip"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="Hôm nay"
                        android:checked="true"/>

                    <com.google.android.material.chip.Chip
                        android:id="@+id/chipTomorrow"
                        style="@style/Chip"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="Ngày mai"/>

                    <!-- More date chips added dynamically -->

                </com.google.android.material.chip.ChipGroup>

            </HorizontalScrollView>

            <!-- Showtimes RecyclerView -->
            <androidx.recyclerview.widget.RecyclerView
                android:id="@+id/rvShowtimes"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_lg"
                android:nestedScrollingEnabled="false"/>

            <!-- Cast Section -->
            <TextView
                style="@style/Text.Heading3"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_sm"
                android:text="@string/movie_detail_cast"/>

            <androidx.recyclerview.widget.RecyclerView
                android:id="@+id/rvCast"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_lg"
                android:nestedScrollingEnabled="false"/>

            <!-- Reviews Section -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:gravity="center_vertical"
                android:layout_marginBottom="@dimen/spacing_sm">

                <TextView
                    style="@style/Text.Heading3"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="@string/movie_detail_reviews"/>

                <com.google.android.material.button.MaterialButton
                    android:id="@+id/btnWriteReview"
                    style="@style/Button.Text"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:text="@string/movie_detail_write_review"
                    app:icon="@drawable/ic_edit"/>

            </LinearLayout>

            <!-- Reviews RecyclerView -->
            <androidx.recyclerview.widget.RecyclerView
                android:id="@+id/rvReviews"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:nestedScrollingEnabled="false"/>

        </LinearLayout>

    </androidx.core.widget.NestedScrollView>

    <!-- Loading Progress -->
    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="center"
        android:visibility="gone"/>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

### Java Implementation: MovieDetailActivity.java

```java
package com.movie88.ui.movie;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.google.android.material.appbar.CollapsingToolbarLayout;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.chip.Chip;
import com.google.android.material.chip.ChipGroup;
import com.movie88.R;
import com.movie88.data.models.response.Movie;
import com.movie88.data.models.response.Showtime;
import com.movie88.data.models.response.Review;
import com.movie88.ui.booking.SelectCinemaActivity;
import com.movie88.ui.adapters.ShowtimeAdapter;
import com.movie88.ui.adapters.CastAdapter;
import com.movie88.ui.adapters.ReviewAdapter;
import com.movie88.utils.Constants;
import com.movie88.utils.DateUtils;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class MovieDetailActivity extends AppCompatActivity {

    // Views
    private CollapsingToolbarLayout collapsingToolbar;
    private Toolbar toolbar;
    private ImageView ivBackdrop, ivPoster;
    private TextView tvTitle, tvRating, tvGenre, tvDuration, tvAgeRating, tvOverview;
    private MaterialButton btnBookNow, btnWatchTrailer, btnWriteReview;
    private ChipGroup chipGroupDates;
    private RecyclerView rvShowtimes, rvCast, rvReviews;
    private ProgressBar progressBar;

    // Adapters
    private ShowtimeAdapter showtimeAdapter;
    private CastAdapter castAdapter;
    private ReviewAdapter reviewAdapter;

    // ViewModel
    private MovieDetailViewModel viewModel;

    // Data
    private int movieId;
    private Movie movie;
    private String selectedDate;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_movie_detail);

        // Get movie ID from intent
        movieId = getIntent().getIntExtra(Constants.EXTRA_MOVIE_ID, 0);
        if (movieId == 0) {
            Toast.makeText(this, "Invalid movie ID", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        initViews();
        setupToolbar();
        setupRecyclerViews();
        setupViewModel();
        setupListeners();

        // Load movie data
        viewModel.loadMovieDetails(movieId);
        
        // Load showtimes for today
        selectedDate = DateUtils.getTodayApiFormat();
        viewModel.loadMovieShowtimes(movieId, selectedDate);
        
        // Load reviews
        viewModel.loadMovieReviews(movieId);
    }

    private void initViews() {
        collapsingToolbar = findViewById(R.id.collapsingToolbar);
        toolbar = findViewById(R.id.toolbar);
        ivBackdrop = findViewById(R.id.ivBackdrop);
        ivPoster = findViewById(R.id.ivPoster);
        tvTitle = findViewById(R.id.tvTitle);
        tvRating = findViewById(R.id.tvRating);
        tvGenre = findViewById(R.id.tvGenre);
        tvDuration = findViewById(R.id.tvDuration);
        tvAgeRating = findViewById(R.id.tvAgeRating);
        tvOverview = findViewById(R.id.tvOverview);
        btnBookNow = findViewById(R.id.btnBookNow);
        btnWatchTrailer = findViewById(R.id.btnWatchTrailer);
        btnWriteReview = findViewById(R.id.btnWriteReview);
        chipGroupDates = findViewById(R.id.chipGroupDates);
        rvShowtimes = findViewById(R.id.rvShowtimes);
        rvCast = findViewById(R.id.rvCast);
        rvReviews = findViewById(R.id.rvReviews);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupToolbar() {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
            getSupportActionBar().setDisplayShowHomeEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void setupRecyclerViews() {
        // Showtimes RecyclerView
        showtimeAdapter = new ShowtimeAdapter(new ArrayList<>());
        showtimeAdapter.setOnShowtimeClickListener(showtime -> {
            // Navigate to seat selection
            Intent intent = new Intent(this, SelectCinemaActivity.class);
            intent.putExtra(Constants.EXTRA_MOVIE_ID, movieId);
            intent.putExtra(Constants.EXTRA_SHOWTIME_ID, showtime.getShowtimeId());
            startActivity(intent);
        });
        rvShowtimes.setLayoutManager(new LinearLayoutManager(this));
        rvShowtimes.setAdapter(showtimeAdapter);

        // Cast RecyclerView
        castAdapter = new CastAdapter(new ArrayList<>());
        rvCast.setLayoutManager(new LinearLayoutManager(this, 
            LinearLayoutManager.HORIZONTAL, false));
        rvCast.setAdapter(castAdapter);

        // Reviews RecyclerView
        reviewAdapter = new ReviewAdapter(new ArrayList<>());
        rvReviews.setLayoutManager(new LinearLayoutManager(this));
        rvReviews.setAdapter(reviewAdapter);
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(MovieDetailViewModel.class);

        // Observe loading state
        viewModel.getIsLoading().observe(this, isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });

        // Observe movie details
        viewModel.getMovieDetails().observe(this, movie -> {
            if (movie != null) {
                this.movie = movie;
                displayMovieDetails(movie);
            }
        });

        // Observe showtimes
        viewModel.getShowtimes().observe(this, showtimes -> {
            if (showtimes != null) {
                showtimeAdapter.updateData(showtimes);
            }
        });

        // Observe reviews
        viewModel.getReviews().observe(this, reviews -> {
            if (reviews != null) {
                reviewAdapter.updateData(reviews);
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
        // Book Now button
        btnBookNow.setOnClickListener(v -> {
            Intent intent = new Intent(this, SelectCinemaActivity.class);
            intent.putExtra(Constants.EXTRA_MOVIE_ID, movieId);
            startActivity(intent);
        });

        // Watch Trailer button
        btnWatchTrailer.setOnClickListener(v -> {
            if (movie != null && movie.getTrailerUrl() != null) {
                Intent intent = new Intent(this, MovieTrailerActivity.class);
                intent.putExtra("TRAILER_URL", movie.getTrailerUrl());
                intent.putExtra("MOVIE_TITLE", movie.getTitle());
                startActivity(intent);
            }
        });

        // Write Review button
        btnWriteReview.setOnClickListener(v -> {
            Intent intent = new Intent(this, WriteReviewActivity.class);
            intent.putExtra(Constants.EXTRA_MOVIE_ID, movieId);
            startActivityForResult(intent, 100);
        });

        // Date chips
        chipGroupDates.setOnCheckedStateChangeListener((group, checkedIds) -> {
            if (!checkedIds.isEmpty()) {
                int checkedId = checkedIds.get(0);
                Chip selectedChip = findViewById(checkedId);
                if (selectedChip != null) {
                    selectedDate = (String) selectedChip.getTag();
                    viewModel.loadMovieShowtimes(movieId, selectedDate);
                }
            }
        });
    }

    private void displayMovieDetails(Movie movie) {
        // Set toolbar title
        collapsingToolbar.setTitle(movie.getTitle());

        // Load images
        Glide.with(this)
            .load(movie.getBackdropUrl())
            .placeholder(R.drawable.placeholder_movie)
            .error(R.drawable.error_image)
            .into(ivBackdrop);

        Glide.with(this)
            .load(movie.getPosterUrl())
            .placeholder(R.drawable.placeholder_movie)
            .error(R.drawable.error_image)
            .into(ivPoster);

        // Set text fields
        tvTitle.setText(movie.getTitle());
        tvRating.setText(String.format("%.1f", movie.getRating()));
        tvGenre.setText(movie.getGenre());
        tvDuration.setText(DateUtils.formatDuration(movie.getDuration()) + " • " + 
            movie.getReleaseYear());
        tvAgeRating.setText(movie.getAgeRating());
        tvOverview.setText(movie.getOverview());

        // Set age rating background color
        setAgeRatingColor(movie.getAgeRating());

        // Setup date chips
        setupDateChips();
    }

    private void setupDateChips() {
        chipGroupDates.removeAllViews();
        
        // Add chips for next 7 days
        for (int i = 0; i < 7; i++) {
            Date date = DateUtils.addDays(new Date(), i);
            String dateStr = DateUtils.formatApiDate(date);
            String displayText = i == 0 ? "Hôm nay" : 
                                i == 1 ? "Ngày mai" : 
                                DateUtils.formatDate(date);

            Chip chip = new Chip(this);
            chip.setText(displayText);
            chip.setTag(dateStr);
            chip.setCheckable(true);
            chip.setChipBackgroundColorResource(R.drawable.chip_background);
            chip.setTextColor(getResources().getColorStateList(R.drawable.chip_text_color));
            
            if (i == 0) {
                chip.setChecked(true);
            }
            
            chipGroupDates.addView(chip);
        }
    }

    private void setAgeRatingColor(String ageRating) {
        int colorRes;
        switch (ageRating) {
            case "P":
                colorRes = R.color.ageRatingP;
                break;
            case "C13":
                colorRes = R.color.ageRatingC13;
                break;
            case "C16":
                colorRes = R.color.ageRatingC16;
                break;
            case "C18":
                colorRes = R.color.ageRatingC18;
                break;
            default:
                colorRes = R.color.ageRatingC16;
        }
        tvAgeRating.setBackgroundColor(getResources().getColor(colorRes));
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == 100 && resultCode == RESULT_OK) {
            // Review was submitted, reload reviews
            viewModel.loadMovieReviews(movieId);
        }
    }
}
```

---

## 2️⃣ MovieTrailerActivity

### Purpose
Hiển thị trailer phim trong WebView (YouTube embedded).

### Layout: activity_movie_trailer.xml

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

    <!-- WebView for trailer -->
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

### Java Implementation: MovieTrailerActivity.java

```java
package com.movie88.ui.movie;

import android.os.Bundle;
import android.view.View;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import com.movie88.R;

public class MovieTrailerActivity extends AppCompatActivity {

    private Toolbar toolbar;
    private WebView webView;
    private ProgressBar progressBar;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_movie_trailer);

        // Get trailer URL
        String trailerUrl = getIntent().getStringExtra("TRAILER_URL");
        String movieTitle = getIntent().getStringExtra("MOVIE_TITLE");

        initViews();
        setupToolbar(movieTitle);
        setupWebView(trailerUrl);
    }

    private void initViews() {
        toolbar = findViewById(R.id.toolbar);
        webView = findViewById(R.id.webView);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupToolbar(String title) {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setTitle(title + " - Trailer");
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void setupWebView(String trailerUrl) {
        // Enable JavaScript
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setDomStorageEnabled(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setUseWideViewPort(true);

        // WebViewClient for page loading
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageFinished(WebView view, String url) {
                progressBar.setVisibility(View.GONE);
            }
        });

        // WebChromeClient for fullscreen video
        webView.setWebChromeClient(new WebChromeClient());

        // Convert YouTube URL to embedded format
        String embedUrl = convertToEmbedUrl(trailerUrl);
        
        // Load HTML with iframe
        String html = "<html><body style='margin:0;padding:0;background:#000;'>" +
                     "<iframe width='100%' height='100%' " +
                     "src='" + embedUrl + "' " +
                     "frameborder='0' " +
                     "allow='accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture' " +
                     "allowfullscreen></iframe>" +
                     "</body></html>";
        
        webView.loadData(html, "text/html", "utf-8");
    }

    private String convertToEmbedUrl(String url) {
        // Convert https://www.youtube.com/watch?v=VIDEO_ID
        // to https://www.youtube.com/embed/VIDEO_ID
        if (url.contains("youtube.com/watch?v=")) {
            String videoId = url.split("v=")[1];
            // Remove additional parameters if any
            if (videoId.contains("&")) {
                videoId = videoId.split("&")[0];
            }
            return "https://www.youtube.com/embed/" + videoId + "?autoplay=1";
        }
        return url;
    }

    @Override
    public void onBackPressed() {
        if (webView.canGoBack()) {
            webView.goBack();
        } else {
            super.onBackPressed();
        }
    }
}
```

---

## 3️⃣ SearchMovieActivity

### Purpose
Tìm kiếm phim với filters (genre, year, rating).

### Layout: activity_search_movie.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:background="@color/backgroundColor">

    <!-- Search Bar -->
    <com.google.android.material.card.MaterialCardView
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_margin="@dimen/spacing_base"
        app:cardCornerRadius="24dp"
        app:cardElevation="4dp"
        app:cardBackgroundColor="@color/backgroundCard">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:padding="@dimen/spacing_sm">

            <ImageView
                android:layout_width="24dp"
                android:layout_height="24dp"
                android:src="@drawable/ic_search"
                app:tint="@color/textSecondary"/>

            <EditText
                android:id="@+id/etSearch"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:layout_marginStart="@dimen/spacing_sm"
                android:background="@null"
                android:hint="@string/search_hint"
                android:textColor="@color/textPrimary"
                android:textColorHint="@color/textHint"
                android:textSize="16sp"
                android:imeOptions="actionSearch"
                android:inputType="text"/>

            <ImageView
                android:id="@+id/btnClear"
                android:layout_width="24dp"
                android:layout_height="24dp"
                android:src="@drawable/ic_close"
                android:visibility="gone"
                app:tint="@color/textSecondary"/>

        </LinearLayout>

    </com.google.android.material.card.MaterialCardView>

    <!-- Filter Chips -->
    <HorizontalScrollView
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_marginHorizontal="@dimen/spacing_base"
        android:scrollbars="none">

        <com.google.android.material.chip.ChipGroup
            android:id="@+id/chipGroupFilters"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content">

            <com.google.android.material.chip.Chip
                android:id="@+id/chipGenre"
                style="@style/Chip"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Thể loại"
                app:chipIcon="@drawable/ic_filter"/>

            <com.google.android.material.chip.Chip
                android:id="@+id/chipYear"
                style="@style/Chip"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Năm"
                app:chipIcon="@drawable/ic_filter"/>

            <com.google.android.material.chip.Chip
                android:id="@+id/chipRating"
                style="@style/Chip"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Đánh giá"
                app:chipIcon="@drawable/ic_filter"/>

            <com.google.android.material.chip.Chip
                android:id="@+id/chipSort"
                style="@style/Chip"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Sắp xếp"
                app:chipIcon="@drawable/ic_sort"/>

        </com.google.android.material.chip.ChipGroup>

    </HorizontalScrollView>

    <!-- Results RecyclerView -->
    <androidx.recyclerview.widget.RecyclerView
        android:id="@+id/rvMovies"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1"
        android:padding="@dimen/spacing_sm"/>

    <!-- Empty State -->
    <com.movie88.ui.components.EmptyStateView
        android:id="@+id/emptyState"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1"
        android:visibility="gone"/>

    <!-- Loading Progress -->
    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="center"
        android:visibility="gone"/>

</LinearLayout>
```

### Java Implementation: SearchMovieActivity.java

```java
package com.movie88.ui.movie;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ProgressBar;

import androidx.appcompat.app.AppCompatActivity;
import androidx.lifecycle.ViewModelProvider;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.chip.Chip;
import com.movie88.R;
import com.movie88.ui.adapters.MovieGridAdapter;
import com.movie88.ui.components.EmptyStateView;

import java.util.ArrayList;

public class SearchMovieActivity extends AppCompatActivity {

    private EditText etSearch;
    private ImageView btnClear;
    private Chip chipGenre, chipYear, chipRating, chipSort;
    private RecyclerView rvMovies;
    private EmptyStateView emptyState;
    private ProgressBar progressBar;

    private MovieGridAdapter adapter;
    private SearchViewModel viewModel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_search_movie);

        initViews();
        setupRecyclerView();
        setupViewModel();
        setupListeners();
    }

    private void initViews() {
        etSearch = findViewById(R.id.etSearch);
        btnClear = findViewById(R.id.btnClear);
        chipGenre = findViewById(R.id.chipGenre);
        chipYear = findViewById(R.id.chipYear);
        chipRating = findViewById(R.id.chipRating);
        chipSort = findViewById(R.id.chipSort);
        rvMovies = findViewById(R.id.rvMovies);
        emptyState = findViewById(R.id.emptyState);
        progressBar = findViewById(R.id.progressBar);
    }

    private void setupRecyclerView() {
        adapter = new MovieGridAdapter(new ArrayList<>());
        rvMovies.setLayoutManager(new GridLayoutManager(this, 2));
        rvMovies.setAdapter(adapter);
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(SearchViewModel.class);

        viewModel.getIsLoading().observe(this, isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });

        viewModel.getMovies().observe(this, movies -> {
            if (movies != null && !movies.isEmpty()) {
                adapter.updateData(movies);
                rvMovies.setVisibility(View.VISIBLE);
                emptyState.setVisibility(View.GONE);
            } else {
                rvMovies.setVisibility(View.GONE);
                emptyState.setVisibility(View.VISIBLE);
                emptyState.configure(
                    R.drawable.ic_search,
                    "Không tìm thấy phim",
                    "Thử tìm kiếm với từ khóa khác",
                    null, null
                );
            }
        });
    }

    private void setupListeners() {
        // Search text change
        etSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                btnClear.setVisibility(s.length() > 0 ? View.VISIBLE : View.GONE);
                viewModel.searchMovies(s.toString());
            }

            @Override
            public void afterTextChanged(Editable s) {}
        });

        // Clear button
        btnClear.setOnClickListener(v -> {
            etSearch.setText("");
        });

        // Filter chips
        chipGenre.setOnClickListener(v -> showGenreFilter());
        chipYear.setOnClickListener(v -> showYearFilter());
        chipRating.setOnClickListener(v -> showRatingFilter());
        chipSort.setOnClickListener(v -> showSortOptions());
    }

    private void showGenreFilter() {
        // Show genre filter dialog
    }

    private void showYearFilter() {
        // Show year filter dialog
    }

    private void showRatingFilter() {
        // Show rating filter dialog
    }

    private void showSortOptions() {
        // Show sort options dialog
    }
}
```

---

## API Endpoints Used

**MovieDetailActivity:**
- `GET /api/movies/{id}` - Lấy thông tin chi tiết phim (title, overview, poster, rating, genre, duration, etc.)
- `GET /api/movies/{id}/showtimes` - Lấy suất chiếu của phim (với query param: date)
- `GET /api/reviews/movie/{movieId}` - Lấy danh sách reviews của phim
- `POST /api/reviews` - Tạo review mới (requires auth)

**MovieTrailerActivity:**
- Không cần API (load YouTube URL từ Movie object)

**SearchMovieActivity:**
- `GET /api/movies/search` - Tìm kiếm phim theo keyword (query param: query)
- `GET /api/movies` - Lấy danh sách phim với filters (query params: genre, year, rating, sort)

---

### API Interface Example
```java
public interface ApiService {
    
    // Get movie details
    @GET("api/movies/{id}")
    Call<ApiResponse<Movie>> getMovieById(@Path("id") int movieId);
    
    // Get movie showtimes by date
    @GET("api/movies/{id}/showtimes")
    Call<ApiResponse<List<Showtime>>> getMovieShowtimes(
        @Path("id") int movieId,
        @Query("date") String date  // Format: yyyy-MM-dd
    );
    
    // Get movie reviews
    @GET("api/reviews/movie/{movieId}")
    Call<ApiResponse<PagedResult<Review>>> getMovieReviews(
        @Path("movieId") int movieId,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    // Create review
    @POST("api/reviews")
    Call<ApiResponse<Review>> createReview(
        @Header("Authorization") String token,
        @Body CreateReviewRequest request
    );
    
    // Search movies
    @GET("api/movies/search")
    Call<ApiResponse<List<Movie>>> searchMovies(
        @Query("query") String query
    );
    
    // Get movies with filters
    @GET("api/movies")
    Call<ApiResponse<PagedResult<Movie>>> getMoviesWithFilters(
        @Query("genre") String genre,
        @Query("year") Integer year,
        @Query("minRating") Double minRating,
        @Query("sort") String sort,  // "title", "rating", "releaseDate"
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
}
```

---

**Last Updated**: October 29, 2025
