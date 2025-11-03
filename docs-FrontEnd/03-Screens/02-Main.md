# 📱 Screen Specifications - Main Module (Home & Navigation)

## 1. MainActivity (Navigation Container)

### Purpose
Container activity chứa BottomNavigationView và ViewPager2 để navigate giữa 3 main tabs:
- Home (Trang chủ)
- Booking History (Lịch sử đặt vé)
- Profile (Tài khoản)

### Layout (activity_main.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.constraintlayout.widget.ConstraintLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <!-- ViewPager2 for fragments -->
    <androidx.viewpager2.widget.ViewPager2
        android:id="@+id/viewPager"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toTopOf="@id/bottomNavigation"/>

    <!-- Bottom Navigation -->
    <com.google.android.material.bottomnavigation.BottomNavigationView
        android:id="@+id/bottomNavigation"
        android:layout_width="match_parent"
        android:layout_height="@dimen/bottom_nav_height"
        android:background="@color/cardBackground"
        android:elevation="8dp"
        app:menu="@menu/bottom_nav_menu"
        app:labelVisibilityMode="labeled"
        app:itemIconTint="@color/bottom_nav_color"
        app:itemTextColor="@color/bottom_nav_color"
        app:layout_constraintBottom_toBottomOf="parent"/>

</androidx.constraintlayout.widget.ConstraintLayout>
```

### Bottom Navigation Menu (menu/bottom_nav_menu.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<menu xmlns:android="http://schemas.android.com/apk/res/android">
    
    <item
        android:id="@+id/nav_home"
        android:icon="@drawable/ic_home"
        android:title="Trang chủ"/>
    
    <item
        android:id="@+id/nav_bookings"
        android:icon="@drawable/ic_ticket"
        android:title="Đặt vé"/>
    
    <item
        android:id="@+id/nav_profile"
        android:icon="@drawable/ic_person"
        android:title="Tài khoản"/>

</menu>
```

### Bottom Nav Color Selector (color/bottom_nav_color.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<selector xmlns:android="http://schemas.android.com/apk/res/android">
    <item android:color="@color/colorPrimary" android:state_checked="true"/>
    <item android:color="@color/textSecondary" android:state_checked="false"/>
</selector>
```

### Java Code (MainActivity.java)
```java
public class MainActivity extends AppCompatActivity {
    
    private ViewPager2 viewPager;
    private BottomNavigationView bottomNavigation;
    private MainPagerAdapter pagerAdapter;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        initViews();
        setupViewPager();
        setupBottomNavigation();
    }
    
    private void initViews() {
        viewPager = findViewById(R.id.viewPager);
        bottomNavigation = findViewById(R.id.bottomNavigation);
    }
    
    private void setupViewPager() {
        pagerAdapter = new MainPagerAdapter(this);
        viewPager.setAdapter(pagerAdapter);
        viewPager.setUserInputEnabled(false); // Disable swipe
        
        // Sync ViewPager with BottomNavigation
        viewPager.registerOnPageChangeCallback(new ViewPager2.OnPageChangeCallback() {
            @Override
            public void onPageSelected(int position) {
                bottomNavigation.getMenu().getItem(position).setChecked(true);
            }
        });
    }
    
    private void setupBottomNavigation() {
        bottomNavigation.setOnItemSelectedListener(item -> {
            int itemId = item.getItemId();
            
            if (itemId == R.id.nav_home) {
                viewPager.setCurrentItem(0, false);
                return true;
            } else if (itemId == R.id.nav_bookings) {
                viewPager.setCurrentItem(1, false);
                return true;
            } else if (itemId == R.id.nav_profile) {
                viewPager.setCurrentItem(2, false);
                return true;
            }
            
            return false;
        });
    }
}
```

### ViewPager Adapter (MainPagerAdapter.java)
```java
public class MainPagerAdapter extends FragmentStateAdapter {
    
    public MainPagerAdapter(@NonNull FragmentActivity fragmentActivity) {
        super(fragmentActivity);
    }
    
    @NonNull
    @Override
    public Fragment createFragment(int position) {
        switch (position) {
            case 0:
                return new HomeFragment();
            case 1:
                return new BookingHistoryFragment();
            case 2:
                return new ProfileFragment();
            default:
                return new HomeFragment();
        }
    }
    
    @Override
    public int getItemCount() {
        return 3;
    }
}
```

---

## 2. HomeFragment ⭐

### Purpose
Màn hình chính hiển thị:
- Banner carousel
- Now Showing movies
- Coming Soon movies
- Search bar

### Layout (fragment_home.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.coordinatorlayout.widget.CoordinatorLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <com.google.android.material.appbar.AppBarLayout
        android:id="@+id/appBarLayout"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:background="@color/cardBackground"
        android:elevation="4dp">

        <!-- Toolbar -->
        <androidx.appcompat.widget.Toolbar
            android:id="@+id/toolbar"
            android:layout_width="match_parent"
            android:layout_height="?attr/actionBarSize"
            app:title="Movie88"
            app:titleTextColor="@color/colorPrimary"
            app:menu="@menu/home_menu"/>

        <!-- Search Bar -->
        <com.google.android.material.search.SearchBar
            android:id="@+id/searchBar"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_margin="@dimen/spacing_md"
            android:hint="Tìm phim..."
            app:iconifiedByDefault="false"/>

    </com.google.android.material.appbar.AppBarLayout>

    <!-- Scrollable Content -->
    <androidx.core.widget.NestedScrollView
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        app:layout_behavior="@string/appbar_scrolling_view_behavior">

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical">

            <!-- Banner Carousel -->
            <androidx.cardview.widget.CardView
                android:layout_width="match_parent"
                android:layout_height="200dp"
                android:layout_margin="@dimen/spacing_md"
                app:cardCornerRadius="@dimen/card_corner_radius"
                app:cardElevation="@dimen/card_elevation">

                <androidx.viewpager2.widget.ViewPager2
                    android:id="@+id/vpBanner"
                    android:layout_width="match_parent"
                    android:layout_height="match_parent"/>

                <!-- Indicator -->
                <com.google.android.material.tabs.TabLayout
                    android:id="@+id/tabIndicator"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:layout_gravity="bottom|center_horizontal"
                    android:layout_marginBottom="@dimen/spacing_sm"
                    app:tabBackground="@drawable/tab_indicator_selector"
                    app:tabGravity="center"
                    app:tabIndicatorHeight="0dp"/>

            </androidx.cardview.widget.CardView>

            <!-- Now Showing Section -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="vertical"
                android:layout_marginTop="@dimen/spacing_lg">

                <!-- Section Header -->
                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:gravity="center_vertical"
                    android:paddingHorizontal="@dimen/spacing_md">

                    <TextView
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:text="Phim đang chiếu"
                        android:textSize="@dimen/text_xl"
                        android:textColor="@color/textPrimary"
                        android:textStyle="bold"/>

                    <TextView
                        android:id="@+id/tvSeeAllNowShowing"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="Xem tất cả >"
                        android:textSize="@dimen/text_sm"
                        android:textColor="@color/colorPrimary"/>

                </LinearLayout>

                <!-- Now Showing Movies RecyclerView -->
                <androidx.recyclerview.widget.RecyclerView
                    android:id="@+id/rvNowShowing"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:layout_marginTop="@dimen/spacing_md"
                    android:clipToPadding="false"
                    android:paddingHorizontal="@dimen/spacing_md"/>

            </LinearLayout>

            <!-- Coming Soon Section -->
            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="vertical"
                android:layout_marginTop="@dimen/spacing_xl">

                <!-- Section Header -->
                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:gravity="center_vertical"
                    android:paddingHorizontal="@dimen/spacing_md">

                    <TextView
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:text="Phim sắp chiếu"
                        android:textSize="@dimen/text_xl"
                        android:textColor="@color/textPrimary"
                        android:textStyle="bold"/>

                    <TextView
                        android:id="@+id/tvSeeAllComingSoon"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="Xem tất cả >"
                        android:textSize="@dimen/text_sm"
                        android:textColor="@color/colorPrimary"/>

                </LinearLayout>

                <!-- Coming Soon Movies RecyclerView -->
                <androidx.recyclerview.widget.RecyclerView
                    android:id="@+id/rvComingSoon"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:layout_marginTop="@dimen/spacing_md"
                    android:clipToPadding="false"
                    android:paddingHorizontal="@dimen/spacing_md"
                    android:paddingBottom="@dimen/spacing_lg"/>

            </LinearLayout>

        </LinearLayout>

    </androidx.core.widget.NestedScrollView>

    <!-- Loading Indicator -->
    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="center"
        android:visibility="gone"/>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

### Movie Item Layout (item_movie_poster.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="@dimen/movie_poster_width"
    android:layout_height="wrap_content"
    android:layout_marginEnd="@dimen/spacing_md"
    app:cardCornerRadius="@dimen/card_corner_radius"
    app:cardElevation="@dimen/card_elevation">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical">

        <!-- Movie Poster -->
        <ImageView
            android:id="@+id/ivPoster"
            android:layout_width="match_parent"
            android:layout_height="@dimen/movie_poster_height"
            android:scaleType="centerCrop"
            android:contentDescription="Movie Poster"/>

        <!-- Rating Badge -->
        <LinearLayout
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:background="@drawable/bg_rating_badge"
            android:padding="@dimen/spacing_xs"
            android:layout_marginStart="@dimen/spacing_sm"
            android:layout_marginTop="-20dp">

            <ImageView
                android:layout_width="16dp"
                android:layout_height="16dp"
                android:src="@drawable/ic_star"
                android:tint="@color/ratingYellow"/>

            <TextView
                android:id="@+id/tvRating"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="8.5"
                android:textSize="@dimen/text_sm"
                android:textColor="@color/textPrimary"
                android:textStyle="bold"
                android:layout_marginStart="@dimen/spacing_xs"/>

        </LinearLayout>

        <!-- Movie Info -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="vertical"
            android:padding="@dimen/spacing_sm">

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

        </LinearLayout>

    </LinearLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java Code (HomeFragment.java)
```java
public class HomeFragment extends Fragment {
    
    private ViewPager2 vpBanner;
    private TabLayout tabIndicator;
    private RecyclerView rvNowShowing, rvComingSoon;
    private ProgressBar progressBar;
    
    private HomeViewModel viewModel;
    private MovieAdapter nowShowingAdapter, comingSoonAdapter;
    
    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, 
                            @Nullable ViewGroup container, 
                            @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_home, container, false);
        
        initViews(view);
        initViewModel();
        setupBanner();
        setupRecyclerViews();
        setupListeners();
        observeViewModel();
        
        // Load data
        viewModel.loadHomeData();
        
        return view;
    }
    
    private void initViews(View view) {
        vpBanner = view.findViewById(R.id.vpBanner);
        tabIndicator = view.findViewById(R.id.tabIndicator);
        rvNowShowing = view.findViewById(R.id.rvNowShowing);
        rvComingSoon = view.findViewById(R.id.rvComingSoon);
        progressBar = view.findViewById(R.id.progressBar);
    }
    
    private void initViewModel() {
        viewModel = new ViewModelProvider(this).get(HomeViewModel.class);
    }
    
    private void setupBanner() {
        // Setup TabLayout with ViewPager2
        new TabLayoutMediator(tabIndicator, vpBanner, (tab, position) -> {
            // Empty implementation - just for indicator dots
        }).attach();
        
        // Auto-scroll banner every 3 seconds
        Handler bannerHandler = new Handler(Looper.getMainLooper());
        Runnable bannerRunnable = new Runnable() {
            @Override
            public void run() {
                int currentItem = vpBanner.getCurrentItem();
                int itemCount = vpBanner.getAdapter() != null ? 
                    vpBanner.getAdapter().getItemCount() : 0;
                
                if (itemCount > 0) {
                    int nextItem = (currentItem + 1) % itemCount;
                    vpBanner.setCurrentItem(nextItem, true);
                }
                
                bannerHandler.postDelayed(this, 3000); // 3 seconds
            }
        };
        
        // Start auto-scroll
        bannerHandler.postDelayed(bannerRunnable, 3000);
        
        // Pause auto-scroll when user manually swipes
        vpBanner.registerOnPageChangeCallback(new ViewPager2.OnPageChangeCallback() {
            @Override
            public void onPageScrollStateChanged(int state) {
                if (state == ViewPager2.SCROLL_STATE_DRAGGING) {
                    // User is manually dragging - remove callbacks
                    bannerHandler.removeCallbacks(bannerRunnable);
                } else if (state == ViewPager2.SCROLL_STATE_IDLE) {
                    // User finished dragging - restart auto-scroll
                    bannerHandler.postDelayed(bannerRunnable, 3000);
                }
            }
        });
    }
    
    private void setupRecyclerViews() {
        // Now Showing - Horizontal
        nowShowingAdapter = new MovieAdapter(movie -> {
            navigateToMovieDetail(movie.getMovieId());
        });
        
        LinearLayoutManager nowShowingLayout = new LinearLayoutManager(
            getContext(), LinearLayoutManager.HORIZONTAL, false);
        rvNowShowing.setLayoutManager(nowShowingLayout);
        rvNowShowing.setAdapter(nowShowingAdapter);
        
        // Coming Soon - Horizontal
        comingSoonAdapter = new MovieAdapter(movie -> {
            navigateToMovieDetail(movie.getMovieId());
        });
        
        LinearLayoutManager comingSoonLayout = new LinearLayoutManager(
            getContext(), LinearLayoutManager.HORIZONTAL, false);
        rvComingSoon.setLayoutManager(comingSoonLayout);
        rvComingSoon.setAdapter(comingSoonAdapter);
    }
    
    private void setupListeners() {
        // Search bar click
        SearchBar searchBar = getView().findViewById(R.id.searchBar);
        searchBar.setOnClickListener(v -> {
            // Navigate to SearchActivity
            Intent intent = new Intent(getContext(), SearchMovieActivity.class);
            startActivity(intent);
        });
    }
    
    private void observeViewModel() {
        // Loading state
        viewModel.getIsLoading().observe(getViewLifecycleOwner(), isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
        });
        
        // Now Showing movies
        viewModel.getNowShowingMovies().observe(getViewLifecycleOwner(), movies -> {
            if (movies != null) {
                nowShowingAdapter.setMovies(movies);
            }
        });
        
        // Coming Soon movies
        viewModel.getComingSoonMovies().observe(getViewLifecycleOwner(), movies -> {
            if (movies != null) {
                comingSoonAdapter.setMovies(movies);
            }
        });
        
        // Error handling
        viewModel.getError().observe(getViewLifecycleOwner(), error -> {
            if (error != null) {
                Toast.makeText(getContext(), error, Toast.LENGTH_SHORT).show();
            }
        });
    }
    
    private void navigateToMovieDetail(int movieId) {
        Intent intent = new Intent(getContext(), MovieDetailActivity.class);
        intent.putExtra("MOVIE_ID", movieId);
        startActivity(intent);
    }
}
```

### ViewModel (HomeViewModel.java)
```java
public class HomeViewModel extends ViewModel {
    
    private final MovieRepository movieRepository;
    
    private final MutableLiveData<Boolean> isLoading = new MutableLiveData<>(false);
    private final MutableLiveData<List<Movie>> nowShowingMovies = new MutableLiveData<>();
    private final MutableLiveData<List<Movie>> comingSoonMovies = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    
    public HomeViewModel() {
        movieRepository = MovieRepository.getInstance();
    }
    
    public void loadHomeData() {
        isLoading.setValue(true);
        
        // Load Now Showing
        movieRepository.getMovies("NowShowing", null, null, 1, 10, 
            new ApiCallback<PagedResult<Movie>>() {
                @Override
                public void onSuccess(PagedResult<Movie> data) {
                    nowShowingMovies.setValue(data.getItems());
                    loadComingSoon();
                }
                
                @Override
                public void onError(String errorMessage) {
                    error.setValue(errorMessage);
                    isLoading.setValue(false);
                }
            });
    }
    
    private void loadComingSoon() {
        movieRepository.getMovies("ComingSoon", null, null, 1, 10, 
            new ApiCallback<PagedResult<Movie>>() {
                @Override
                public void onSuccess(PagedResult<Movie> data) {
                    comingSoonMovies.setValue(data.getItems());
                    isLoading.setValue(false);
                }
                
                @Override
                public void onError(String errorMessage) {
                    error.setValue(errorMessage);
                    isLoading.setValue(false);
                }
            });
    }
    
    // Getters
    public LiveData<Boolean> getIsLoading() {
        return isLoading;
    }
    
    public LiveData<List<Movie>> getNowShowingMovies() {
        return nowShowingMovies;
    }
    
    public LiveData<List<Movie>> getComingSoonMovies() {
        return comingSoonMovies;
    }
    
    public LiveData<String> getError() {
        return error;
    }
}
```

### Banner Adapter (BannerAdapter.java)
```java
public class BannerAdapter extends RecyclerView.Adapter<BannerAdapter.BannerViewHolder> {
    
    private List<Banner> banners;
    private OnBannerClickListener listener;
    
    public interface OnBannerClickListener {
        void onBannerClick(Banner banner);
    }
    
    public BannerAdapter(OnBannerClickListener listener) {
        this.banners = new ArrayList<>();
        this.listener = listener;
    }
    
    public void setBanners(List<Banner> banners) {
        this.banners = banners;
        notifyDataSetChanged();
    }
    
    @NonNull
    @Override
    public BannerViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
            .inflate(R.layout.item_banner, parent, false);
        return new BannerViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull BannerViewHolder holder, int position) {
        holder.bind(banners.get(position));
    }
    
    @Override
    public int getItemCount() {
        return banners.size();
    }
    
    class BannerViewHolder extends RecyclerView.ViewHolder {
        ImageView ivBanner;
        
        BannerViewHolder(View itemView) {
            super(itemView);
            ivBanner = itemView.findViewById(R.id.ivBanner);
            
            itemView.setOnClickListener(v -> {
                int position = getAdapterPosition();
                if (position != RecyclerView.NO_POSITION && listener != null) {
                    listener.onBannerClick(banners.get(position));
                }
            });
        }
        
        void bind(Banner banner) {
            Glide.with(ivBanner.getContext())
                .load(banner.getImageUrl())
                .centerCrop()
                .into(ivBanner);
        }
    }
}
```

### Banner Item Layout (item_banner.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<ImageView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    android:id="@+id/ivBanner"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:scaleType="centerCrop"/>
```

### Movie Adapter (MovieAdapter.java)
```java
public class MovieAdapter extends RecyclerView.Adapter<MovieAdapter.MovieViewHolder> {
    
    private List<Movie> movies;
    private OnMovieClickListener listener;
    
    public interface OnMovieClickListener {
        void onMovieClick(Movie movie);
    }
    
    public MovieAdapter(OnMovieClickListener listener) {
        this.movies = new ArrayList<>();
        this.listener = listener;
    }
    
    public void setMovies(List<Movie> movies) {
        this.movies = movies;
        notifyDataSetChanged();
    }
    
    @NonNull
    @Override
    public MovieViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
            .inflate(R.layout.item_movie_poster, parent, false);
        return new MovieViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull MovieViewHolder holder, int position) {
        holder.bind(movies.get(position));
    }
    
    @Override
    public int getItemCount() {
        return movies.size();
    }
    
    class MovieViewHolder extends RecyclerView.ViewHolder {
        ImageView ivPoster;
        TextView tvTitle, tvGenre, tvRating;
        
        MovieViewHolder(View itemView) {
            super(itemView);
            ivPoster = itemView.findViewById(R.id.ivPoster);
            tvTitle = itemView.findViewById(R.id.tvTitle);
            tvGenre = itemView.findViewById(R.id.tvGenre);
            tvRating = itemView.findViewById(R.id.tvRating);
            
            itemView.setOnClickListener(v -> {
                int position = getAdapterPosition();
                if (position != RecyclerView.NO_POSITION && listener != null) {
                    listener.onMovieClick(movies.get(position));
                }
            });
        }
        
        void bind(Movie movie) {
            tvTitle.setText(movie.getTitle());
            tvGenre.setText(movie.getGenresString());
            tvRating.setText(String.format("%.1f", movie.getRating()));
            
            Glide.with(ivPoster.getContext())
                .load(movie.getPosterUrl())
                .placeholder(R.drawable.img_poster_placeholder)
                .into(ivPoster);
        }
    }
}
```

---

## 3. BookingHistoryFragment 🎫

### Purpose
Hiển thị danh sách lịch sử đặt vé của user với các trạng thái khác nhau.

### Layout (fragment_booking_history.xml)
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
        app:title="Lịch sử đặt vé"/>

    <!-- Filter Chips -->
    <com.google.android.material.chip.ChipGroup
        android:id="@+id/chipGroup"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_md"
        app:singleSelection="true"
        app:selectionRequired="true">

        <com.google.android.material.chip.Chip
            android:id="@+id/chipAll"
            style="@style/Chip.Filter"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Tất cả"
            android:checked="true"/>

        <com.google.android.material.chip.Chip
            android:id="@+id/chipPending"
            style="@style/Chip.Filter"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Chờ thanh toán"/>

        <com.google.android.material.chip.Chip
            android:id="@+id/chipConfirmed"
            style="@style/Chip.Filter"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Đã xác nhận"/>

        <com.google.android.material.chip.Chip
            android:id="@+id/chipCompleted"
            style="@style/Chip.Filter"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Hoàn thành"/>

        <com.google.android.material.chip.Chip
            android:id="@+id/chipCancelled"
            style="@style/Chip.Filter"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Đã hủy"/>

    </com.google.android.material.chip.ChipGroup>

    <!-- Bookings RecyclerView -->
    <androidx.swiperefreshlayout.widget.SwipeRefreshLayout
        android:id="@+id/swipeRefresh"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1">

        <androidx.recyclerview.widget.RecyclerView
            android:id="@+id/rvBookings"
            android:layout_width="match_parent"
            android:layout_height="match_parent"
            android:clipToPadding="false"
            android:padding="@dimen/spacing_md"/>

    </androidx.swiperefreshlayout.widget.SwipeRefreshLayout>

    <!-- Empty State -->
    <LinearLayout
        android:id="@+id/layoutEmpty"
        android:layout_width="match_parent"
        android:layout_height="0dp"
        android:layout_weight="1"
        android:orientation="vertical"
        android:gravity="center"
        android:padding="@dimen/spacing_xl"
        android:visibility="gone">

        <ImageView
            android:layout_width="120dp"
            android:layout_height="120dp"
            android:src="@drawable/ic_empty_bookings"
            app:tint="@color/textSecondary"
            android:alpha="0.5"/>

        <TextView
            style="@style/Text.Body"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:layout_marginTop="@dimen/spacing_md"
            android:text="Chưa có lịch sử đặt vé"
            android:textColor="@color/textSecondary"/>

    </LinearLayout>

    <!-- Loading -->
    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="center"
        android:visibility="gone"/>

</LinearLayout>
```

### Booking Item Layout (item_booking.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<com.google.android.material.card.MaterialCardView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    style="@style/CardView"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_marginBottom="@dimen/spacing_md">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical"
        android:padding="@dimen/spacing_md">

        <!-- Header: Booking ID + Status -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical"
            android:layout_marginBottom="@dimen/spacing_sm">

            <TextView
                android:id="@+id/tvBookingId"
                style="@style/Text.Caption"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="#BK12345"/>

            <TextView
                android:id="@+id/tvStatus"
                style="@style/Text.Caption"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:paddingHorizontal="8dp"
                android:paddingVertical="4dp"
                android:background="@drawable/bg_status_confirmed"
                android:textColor="@android:color/white"
                android:text="Đã xác nhận"/>

        </LinearLayout>

        <!-- Movie Info -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:layout_marginBottom="@dimen/spacing_sm">

            <!-- Poster -->
            <ImageView
                android:id="@+id/ivPoster"
                android:layout_width="60dp"
                android:layout_height="90dp"
                android:scaleType="centerCrop"
                android:background="@drawable/bg_poster"/>

            <!-- Details -->
            <LinearLayout
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:layout_marginStart="@dimen/spacing_md"
                android:orientation="vertical">

                <TextView
                    android:id="@+id/tvMovieTitle"
                    style="@style/Text.Heading3"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:maxLines="2"
                    android:ellipsize="end"
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
                    android:id="@+id/tvSeats"
                    style="@style/Text.BodySecondary"
                    android:layout_width="wrap_content"
                    android:layout_height="wrap_content"
                    android:layout_marginTop="@dimen/spacing_xs"
                    android:text="Ghế: A1, A2"/>

            </LinearLayout>

        </LinearLayout>

        <View
            android:layout_width="match_parent"
            android:layout_height="@dimen/divider_height"
            android:layout_marginVertical="@dimen/spacing_sm"
            android:background="@color/divider"/>

        <!-- Price + Action Button -->
        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:gravity="center_vertical">

            <TextView
                android:id="@+id/tvTotalPrice"
                style="@style/Text.Heading3"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="240.000 đ"
                android:textColor="@color/colorPrimary"/>

            <com.google.android.material.button.MaterialButton
                android:id="@+id/btnAction"
                style="@style/Button.Outlined"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:text="Xem chi tiết"/>

        </LinearLayout>

    </LinearLayout>

</com.google.android.material.card.MaterialCardView>
```

### Java Implementation (BookingHistoryFragment.java)
```java
public class BookingHistoryFragment extends Fragment {
    
    private RecyclerView rvBookings;
    private SwipeRefreshLayout swipeRefresh;
    private View layoutEmpty;
    private ProgressBar progressBar;
    private ChipGroup chipGroup;
    
    private BookingHistoryViewModel viewModel;
    private BookingAdapter adapter;
    
    private String currentFilter = "All";
    
    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, 
                            @Nullable ViewGroup container, 
                            @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_booking_history, container, false);
        
        initViews(view);
        initViewModel();
        setupRecyclerView();
        setupListeners();
        observeViewModel();
        
        // Load bookings
        viewModel.loadBookings(currentFilter);
        
        return view;
    }
    
    private void initViews(View view) {
        rvBookings = view.findViewById(R.id.rvBookings);
        swipeRefresh = view.findViewById(R.id.swipeRefresh);
        layoutEmpty = view.findViewById(R.id.layoutEmpty);
        progressBar = view.findViewById(R.id.progressBar);
        chipGroup = view.findViewById(R.id.chipGroup);
    }
    
    private void initViewModel() {
        viewModel = new ViewModelProvider(this).get(BookingHistoryViewModel.class);
    }
    
    private void setupRecyclerView() {
        adapter = new BookingAdapter(booking -> {
            navigateToBookingDetail(booking.getBookingId());
        });
        
        rvBookings.setLayoutManager(new LinearLayoutManager(getContext()));
        rvBookings.setAdapter(adapter);
    }
    
    private void setupListeners() {
        // Pull to refresh
        swipeRefresh.setOnRefreshListener(() -> {
            viewModel.loadBookings(currentFilter);
        });
        
        // Filter chips
        chipGroup.setOnCheckedStateChangeListener((group, checkedIds) -> {
            if (!checkedIds.isEmpty()) {
                int checkedId = checkedIds.get(0);
                
                if (checkedId == R.id.chipAll) {
                    currentFilter = "All";
                } else if (checkedId == R.id.chipPending) {
                    currentFilter = "Pending";
                } else if (checkedId == R.id.chipConfirmed) {
                    currentFilter = "Confirmed";
                } else if (checkedId == R.id.chipCompleted) {
                    currentFilter = "Completed";
                } else if (checkedId == R.id.chipCancelled) {
                    currentFilter = "Cancelled";
                }
                
                viewModel.loadBookings(currentFilter);
            }
        });
    }
    
    private void observeViewModel() {
        // Loading state
        viewModel.getIsLoading().observe(getViewLifecycleOwner(), isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            swipeRefresh.setRefreshing(false);
        });
        
        // Bookings data
        viewModel.getBookings().observe(getViewLifecycleOwner(), bookings -> {
            if (bookings != null && !bookings.isEmpty()) {
                adapter.setBookings(bookings);
                rvBookings.setVisibility(View.VISIBLE);
                layoutEmpty.setVisibility(View.GONE);
            } else {
                rvBookings.setVisibility(View.GONE);
                layoutEmpty.setVisibility(View.VISIBLE);
            }
        });
        
        // Error handling
        viewModel.getError().observe(getViewLifecycleOwner(), error -> {
            if (error != null) {
                Toast.makeText(getContext(), error, Toast.LENGTH_SHORT).show();
            }
        });
    }
    
    private void navigateToBookingDetail(int bookingId) {
        Intent intent = new Intent(getContext(), BookingDetailActivity.class);
        intent.putExtra("BOOKING_ID", bookingId);
        startActivity(intent);
    }
}
```

### Booking Adapter (BookingAdapter.java)
```java
public class BookingAdapter extends RecyclerView.Adapter<BookingAdapter.BookingViewHolder> {
    
    private List<Booking> bookings;
    private OnBookingClickListener listener;
    
    public interface OnBookingClickListener {
        void onBookingClick(Booking booking);
    }
    
    public BookingAdapter(OnBookingClickListener listener) {
        this.bookings = new ArrayList<>();
        this.listener = listener;
    }
    
    public void setBookings(List<Booking> bookings) {
        this.bookings = bookings;
        notifyDataSetChanged();
    }
    
    @NonNull
    @Override
    public BookingViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
            .inflate(R.layout.item_booking, parent, false);
        return new BookingViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull BookingViewHolder holder, int position) {
        holder.bind(bookings.get(position));
    }
    
    @Override
    public int getItemCount() {
        return bookings.size();
    }
    
    class BookingViewHolder extends RecyclerView.ViewHolder {
        TextView tvBookingId, tvStatus, tvMovieTitle, tvCinema, tvShowtime, tvSeats, tvTotalPrice;
        ImageView ivPoster;
        MaterialButton btnAction;
        
        BookingViewHolder(View itemView) {
            super(itemView);
            tvBookingId = itemView.findViewById(R.id.tvBookingId);
            tvStatus = itemView.findViewById(R.id.tvStatus);
            tvMovieTitle = itemView.findViewById(R.id.tvMovieTitle);
            tvCinema = itemView.findViewById(R.id.tvCinema);
            tvShowtime = itemView.findViewById(R.id.tvShowtime);
            tvSeats = itemView.findViewById(R.id.tvSeats);
            tvTotalPrice = itemView.findViewById(R.id.tvTotalPrice);
            ivPoster = itemView.findViewById(R.id.ivPoster);
            btnAction = itemView.findViewById(R.id.btnAction);
            
            btnAction.setOnClickListener(v -> {
                int position = getAdapterPosition();
                if (position != RecyclerView.NO_POSITION && listener != null) {
                    listener.onBookingClick(bookings.get(position));
                }
            });
        }
        
        void bind(Booking booking) {
            tvBookingId.setText("#" + booking.getBookingCode());
            tvMovieTitle.setText(booking.getMovieTitle());
            tvCinema.setText(booking.getCinemaName());
            tvShowtime.setText(booking.getShowtimeFormatted());
            tvSeats.setText("Ghế: " + booking.getSeatsString());
            tvTotalPrice.setText(booking.getTotalPriceFormatted());
            
            // Set status badge
            setStatusBadge(booking.getStatus());
            
            // Load poster
            Glide.with(ivPoster.getContext())
                .load(booking.getMoviePosterUrl())
                .placeholder(R.drawable.img_poster_placeholder)
                .into(ivPoster);
                
            // Set action button based on status
            setActionButton(booking.getStatus());
        }
        
        private void setStatusBadge(String status) {
            int backgroundRes;
            String statusText;
            
            switch (status) {
                case "Pending":
                    backgroundRes = R.drawable.bg_status_pending;
                    statusText = "Chờ thanh toán";
                    break;
                case "Confirmed":
                    backgroundRes = R.drawable.bg_status_confirmed;
                    statusText = "Đã xác nhận";
                    break;
                case "Completed":
                    backgroundRes = R.drawable.bg_status_completed;
                    statusText = "Hoàn thành";
                    break;
                case "Cancelled":
                    backgroundRes = R.drawable.bg_status_cancelled;
                    statusText = "Đã hủy";
                    break;
                default:
                    backgroundRes = R.drawable.bg_status_pending;
                    statusText = status;
            }
            
            tvStatus.setBackgroundResource(backgroundRes);
            tvStatus.setText(statusText);
        }
        
        private void setActionButton(String status) {
            if ("Pending".equals(status)) {
                btnAction.setText("Thanh toán");
                btnAction.setVisibility(View.VISIBLE);
            } else if ("Confirmed".equals(status)) {
                btnAction.setText("Xem vé");
                btnAction.setVisibility(View.VISIBLE);
            } else {
                btnAction.setText("Xem chi tiết");
                btnAction.setVisibility(View.VISIBLE);
            }
        }
    }
}
```

### ViewModel (BookingHistoryViewModel.java)
```java
public class BookingHistoryViewModel extends ViewModel {
    
    private final BookingRepository bookingRepository;
    
    private final MutableLiveData<Boolean> isLoading = new MutableLiveData<>(false);
    private final MutableLiveData<List<Booking>> bookings = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    
    public BookingHistoryViewModel() {
        bookingRepository = BookingRepository.getInstance();
    }
    
    public void loadBookings(String status) {
        isLoading.setValue(true);
        
        bookingRepository.getMyBookings(status, new ApiCallback<List<Booking>>() {
            @Override
            public void onSuccess(List<Booking> data) {
                bookings.setValue(data);
                isLoading.setValue(false);
            }
            
            @Override
            public void onError(String errorMessage) {
                error.setValue(errorMessage);
                isLoading.setValue(false);
            }
        });
    }
    
    // Getters
    public LiveData<Boolean> getIsLoading() {
        return isLoading;
    }
    
    public LiveData<List<Booking>> getBookings() {
        return bookings;
    }
    
    public LiveData<String> getError() {
        return error;
    }
}
```

---

## API Endpoints Used

**HomeFragment:**
- `GET /api/movies` - Lấy danh sách phim (with filters: status, genre, search, page, pageSize)
- `GET /api/movies/now-showing` - Lấy phim đang chiếu
- `GET /api/movies/coming-soon` - Lấy phim sắp chiếu
- `GET /api/promotions/active` - Lấy danh sách khuyến mãi đang hoạt động (for banners)

**BookingHistoryFragment:**
- `GET /api/bookings/my-bookings` - Lấy lịch sử đặt vé của user (requires auth)
  - Query params: `status` (All/Pending/Confirmed/Completed/Cancelled), `page`, `pageSize`
- `GET /api/bookings/{id}` - Lấy chi tiết booking cụ thể

**ProfileFragment:** (See 06-Profile.md)
- `GET /api/users/me` - Lấy thông tin user hiện tại
- `GET /api/customers/profile` - Lấy thông tin profile khách hàng (requires auth)
- `POST /api/auth/logout` - Đăng xuất

---

### API Interface Example
```java
public interface ApiService {
    
    // Get movies with filters
    @GET("api/movies")
    Call<ApiResponse<PagedResult<Movie>>> getMovies(
        @Query("status") String status,        // "NowShowing" or "ComingSoon"
        @Query("genre") String genre,
        @Query("search") String search,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    // Get now showing movies
    @GET("api/movies/now-showing")
    Call<ApiResponse<List<Movie>>> getNowShowingMovies();
    
    // Get coming soon movies
    @GET("api/movies/coming-soon")
    Call<ApiResponse<List<Movie>>> getComingSoonMovies();
    
    // Get active promotions/banners
    @GET("api/promotions/active")
    Call<ApiResponse<List<Promotion>>> getActivePromotions();
    
    // Get my bookings with filter
    @GET("api/bookings/my-bookings")
    Call<ApiResponse<List<Booking>>> getMyBookings(
        @Header("Authorization") String token,
        @Query("status") String status,  // All/Pending/Confirmed/Completed/Cancelled
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    // Get booking detail
    @GET("api/bookings/{id}")
    Call<ApiResponse<Booking>> getBookingDetail(
        @Header("Authorization") String token,
        @Path("id") int bookingId
    );
    
    // Get customer profile
    @GET("api/customers/profile")
    Call<ApiResponse<Customer>> getCustomerProfile(
        @Header("Authorization") String token
    );
    
    // Logout
    @POST("api/auth/logout")
    Call<ApiResponse<Void>> logout(
        @Header("Authorization") String token
    );
}
```

---

## 📊 Flow Summary

### MainActivity Navigation Structure

```
MainActivity (ViewPager2 + BottomNavigationView)
│
├─ 📍 Tab 0: HomeFragment
│   ├─ Banner Carousel (auto-scroll 3s)
│   ├─ Now Showing Movies → MovieDetailActivity
│   ├─ Coming Soon Movies → MovieDetailActivity
│   └─ Search Bar → SearchMovieActivity
│
├─ 📍 Tab 1: BookingHistoryFragment
│   ├─ Filter Chips (All/Pending/Confirmed/Completed/Cancelled)
│   ├─ Booking List (SwipeRefresh)
│   └─ Click Booking → BookingDetailActivity
│
└─ 📍 Tab 2: ProfileFragment (See 06-Profile.md)
    ├─ User Info Card
    ├─ Edit Profile → EditProfileActivity
    ├─ Change Password → ChangePasswordActivity
    ├─ Settings → SettingsActivity
    └─ Logout → LoginActivity
```

### Navigation Flow Table

| Screen | Action | Target Screen | Data Passed |
|--------|--------|---------------|-------------|
| **HomeFragment** | Click Movie | MovieDetailActivity | movieId |
| HomeFragment | Click Search | SearchMovieActivity | - |
| HomeFragment | Click Banner | MovieDetailActivity / PromoActivity | promotionId / movieId |
| **BookingHistoryFragment** | Click Booking Item | BookingDetailActivity | bookingId |
| BookingHistoryFragment | Pull to Refresh | Reload bookings | currentFilter |
| BookingHistoryFragment | Select Filter Chip | Reload bookings | status filter |
| **ProfileFragment** | Click Edit Profile | EditProfileActivity | - |
| ProfileFragment | Click Booking History | BookingHistoryActivity | - |
| ProfileFragment | Click Settings | SettingsActivity | - |
| ProfileFragment | Click Logout | LoginActivity | FLAG_CLEAR_TASK |

---

## 🔑 Key Features Implementation

### 1. ViewPager2 with BottomNavigationView Sync
- **Disable Swipe**: `viewPager.setUserInputEnabled(false)` to only navigate via bottom nav
- **Bidirectional Sync**: BottomNav clicks change ViewPager, ViewPager changes update BottomNav selection
- **No Animation**: Use `setCurrentItem(position, false)` for instant switch

### 2. Banner Auto-scroll Logic
- **Handler + Runnable**: Post delayed task every 3 seconds
- **User Interaction Pause**: Remove callback when user drags, restart when idle
- **Infinite Loop**: Use modulo `(currentItem + 1) % itemCount` for circular scrolling
- **TabLayout Indicator**: TabLayoutMediator automatically syncs dots with ViewPager2

### 3. Horizontal Movie Carousels
- **LinearLayoutManager HORIZONTAL**: Scroll movies left-right
- **clipToPadding="false"**: Allow padding without clipping content
- **Item Spacing**: Use `marginEnd` in item layout instead of ItemDecoration
- **Click Handling**: Pass `OnMovieClickListener` interface to adapter

### 4. Booking History Filter System
- **ChipGroup with singleSelection**: Only one filter active at a time
- **selectionRequired="true"**: Always keep one chip selected
- **Reload on Filter Change**: Call `viewModel.loadBookings(newFilter)` when chip changes
- **Status Badge Colors**: Different drawable backgrounds based on booking status

### 5. Empty State Handling
- **Conditional Visibility**: Show `layoutEmpty` when `bookings.isEmpty()`, else show `rvBookings`
- **Empty State Design**: Icon + message with reduced opacity for subtle appearance

### 6. Pull-to-Refresh
- **SwipeRefreshLayout**: Wrap RecyclerView to enable pull-down gesture
- **Stop Refreshing**: Call `swipeRefresh.setRefreshing(false)` in ViewModel observer
- **Load Latest Data**: Trigger `viewModel.loadBookings(currentFilter)` on refresh

---

## 🎨 Design Specifications

### Dimensions (dimens.xml)
```xml
<!-- Movie Poster -->
<dimen name="movie_poster_width">140dp</dimen>
<dimen name="movie_poster_height">200dp</dimen>
<dimen name="poster_corner_radius">12dp</dimen>

<!-- Bottom Navigation -->
<dimen name="bottom_nav_height">64dp</dimen>

<!-- Avatar -->
<dimen name="avatar_size_large">72dp</dimen>

<!-- Icons -->
<dimen name="icon_size_sm">16dp</dimen>
<dimen name="icon_size_md">24dp</dimen>
<dimen name="icon_size_lg">32dp</dimen>

<!-- Card -->
<dimen name="card_corner_radius">16dp</dimen>
<dimen name="card_elevation">4dp</dimen>

<!-- Spacing -->
<dimen name="spacing_xs">4dp</dimen>
<dimen name="spacing_sm">8dp</dimen>
<dimen name="spacing_md">12dp</dimen>
<dimen name="spacing_base">16dp</dimen>
<dimen name="spacing_lg">20dp</dimen>
<dimen name="spacing_xl">24dp</dimen>

<!-- Divider -->
<dimen name="divider_height">1dp</dimen>
```

### Status Badge Drawables

**bg_status_pending.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="#FFA726"/>  <!-- Orange -->
    <corners android:radius="8dp"/>
</shape>
```

**bg_status_confirmed.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="#66BB6A"/>  <!-- Green -->
    <corners android:radius="8dp"/>
</shape>
```

**bg_status_completed.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="#42A5F5"/>  <!-- Blue -->
    <corners android:radius="8dp"/>
</shape>
```

**bg_status_cancelled.xml**
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android">
    <solid android:color="#EF5350"/>  <!-- Red -->
    <corners android:radius="8dp"/>
</shape>
```

---

## 📝 Notes

1. **Authentication Required**: All booking-related endpoints require Bearer token in Authorization header
2. **Error Handling**: Always implement try-catch for API calls, show Toast on error
3. **Image Loading**: Use Glide with placeholder for smooth loading experience
4. **Memory Management**: Remove Handler callbacks in Fragment `onDestroyView()` to prevent leaks
5. **Status Filtering**: Backend should support `status` query parameter: `All`, `Pending`, `Confirmed`, `Completed`, `Cancelled`
6. **Pagination**: Implement endless scroll if booking list is long (load more on scroll to bottom)
7. **Banner Click Actions**: Banner can link to movie detail OR promotion detail based on banner type

---

**Last Updated**: October 29, 2025
