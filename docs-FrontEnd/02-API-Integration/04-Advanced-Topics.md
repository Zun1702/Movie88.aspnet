# 🚀 Advanced API Integration Topics

## Tổng quan

Các kỹ thuật nâng cao cho API integration: Error handling, Offline support, Push notifications, Pagination.

---

## 1️⃣ Error Handling Strategy

### Purpose
Xử lý errors từ API một cách nhất quán, retry mechanism, user-friendly messages.

### Implementation

#### Custom Exception Classes

```java
package com.movie88.data.api.exceptions;

public class ApiException extends Exception {
    private int statusCode;
    private String errorCode;

    public ApiException(String message, int statusCode, String errorCode) {
        super(message);
        this.statusCode = statusCode;
        this.errorCode = errorCode;
    }

    public int getStatusCode() {
        return statusCode;
    }

    public String getErrorCode() {
        return errorCode;
    }
}

public class NetworkException extends Exception {
    public NetworkException(String message) {
        super(message);
    }
}

public class TokenExpiredException extends Exception {
    public TokenExpiredException() {
        super("Token expired. Please login again.");
    }
}
```

#### Error Handler Utility

```java
package com.movie88.utils;

import com.movie88.data.api.exceptions.ApiException;
import com.movie88.data.api.exceptions.NetworkException;
import com.movie88.data.api.exceptions.TokenExpiredException;

import retrofit2.Response;

import java.io.IOException;
import java.net.SocketTimeoutException;
import java.net.UnknownHostException;

public class ErrorHandler {

    public static String handleError(Throwable throwable) {
        if (throwable instanceof ApiException) {
            ApiException apiException = (ApiException) throwable;
            return handleApiError(apiException.getStatusCode(), apiException.getMessage());
        } else if (throwable instanceof NetworkException) {
            return "Lỗi kết nối mạng. Vui lòng kiểm tra internet.";
        } else if (throwable instanceof TokenExpiredException) {
            return "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
        } else if (throwable instanceof UnknownHostException) {
            return "Không thể kết nối tới máy chủ.";
        } else if (throwable instanceof SocketTimeoutException) {
            return "Hết thời gian chờ. Vui lòng thử lại.";
        } else if (throwable instanceof IOException) {
            return "Lỗi kết nối. Vui lòng thử lại.";
        } else {
            return "Đã có lỗi xảy ra. Vui lòng thử lại.";
        }
    }

    private static String handleApiError(int statusCode, String message) {
        switch (statusCode) {
            case 400:
                return message != null ? message : "Yêu cầu không hợp lệ.";
            case 401:
                return "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
            case 403:
                return "Bạn không có quyền truy cập.";
            case 404:
                return "Không tìm thấy dữ liệu.";
            case 409:
                return "Dữ liệu đã tồn tại.";
            case 500:
                return "Lỗi máy chủ. Vui lòng thử lại sau.";
            case 503:
                return "Máy chủ đang bảo trì. Vui lòng thử lại sau.";
            default:
                return message != null ? message : "Đã có lỗi xảy ra.";
        }
    }

    public static <T> ApiException parseApiError(Response<T> response) {
        int statusCode = response.code();
        String errorMessage = "Unknown error";
        String errorCode = null;

        try {
            if (response.errorBody() != null) {
                String errorBody = response.errorBody().string();
                // Parse JSON error response
                org.json.JSONObject json = new org.json.JSONObject(errorBody);
                errorMessage = json.optString("message", errorMessage);
                errorCode = json.optString("errorCode", null);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        return new ApiException(errorMessage, statusCode, errorCode);
    }
}
```

#### Retry Mechanism

```java
package com.movie88.data.repository;

import com.movie88.utils.ApiCallback;
import com.movie88.utils.ErrorHandler;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public abstract class BaseRepository {

    protected static final int MAX_RETRIES = 3;
    protected static final long RETRY_DELAY_MS = 1000;

    protected <T> void executeWithRetry(Call<T> call, ApiCallback<T> callback) {
        executeWithRetry(call, callback, 0);
    }

    private <T> void executeWithRetry(Call<T> call, ApiCallback<T> callback, int retryCount) {
        call.clone().enqueue(new Callback<T>() {
            @Override
            public void onResponse(Call<T> call, Response<T> response) {
                if (response.isSuccessful() && response.body() != null) {
                    callback.onSuccess(response.body());
                } else {
                    // Check if should retry
                    int statusCode = response.code();
                    if (shouldRetry(statusCode) && retryCount < MAX_RETRIES) {
                        // Retry after delay
                        new android.os.Handler().postDelayed(() -> {
                            executeWithRetry(call, callback, retryCount + 1);
                        }, RETRY_DELAY_MS * (retryCount + 1));
                    } else {
                        // No more retries, return error
                        ApiException error = ErrorHandler.parseApiError(response);
                        callback.onError(ErrorHandler.handleError(error));
                    }
                }
            }

            @Override
            public void onFailure(Call<T> call, Throwable t) {
                // Check if should retry
                if (shouldRetryOnFailure(t) && retryCount < MAX_RETRIES) {
                    // Retry after delay
                    new android.os.Handler().postDelayed(() -> {
                        executeWithRetry(call, callback, retryCount + 1);
                    }, RETRY_DELAY_MS * (retryCount + 1));
                } else {
                    // No more retries, return error
                    callback.onError(ErrorHandler.handleError(t));
                }
            }
        });
    }

    private boolean shouldRetry(int statusCode) {
        // Retry on server errors (500, 502, 503, 504) and timeout (408)
        return statusCode >= 500 || statusCode == 408;
    }

    private boolean shouldRetryOnFailure(Throwable t) {
        // Retry on network errors and timeout
        return t instanceof java.io.IOException;
    }
}
```

---

## 2️⃣ Offline Support with Room Database

### Purpose
Cache data locally, sync khi có internet, offline-first approach.

### Room Database Setup

#### Database Class

```java
package com.movie88.data.database;

import androidx.room.Database;
import androidx.room.Room;
import androidx.room.RoomDatabase;
import androidx.room.TypeConverters;

import android.content.Context;

import com.movie88.data.database.dao.MovieDao;
import com.movie88.data.database.dao.BookingDao;
import com.movie88.data.database.dao.CinemaDao;
import com.movie88.data.database.entities.MovieEntity;
import com.movie88.data.database.entities.BookingEntity;
import com.movie88.data.database.entities.CinemaEntity;

@Database(
    entities = {MovieEntity.class, BookingEntity.class, CinemaEntity.class},
    version = 1,
    exportSchema = false
)
@TypeConverters({Converters.class})
public abstract class AppDatabase extends RoomDatabase {

    private static volatile AppDatabase INSTANCE;

    public abstract MovieDao movieDao();
    public abstract BookingDao bookingDao();
    public abstract CinemaDao cinemaDao();

    public static AppDatabase getInstance(Context context) {
        if (INSTANCE == null) {
            synchronized (AppDatabase.class) {
                if (INSTANCE == null) {
                    INSTANCE = Room.databaseBuilder(
                        context.getApplicationContext(),
                        AppDatabase.class,
                        "movie88_database"
                    )
                    .fallbackToDestructiveMigration()
                    .build();
                }
            }
        }
        return INSTANCE;
    }
}
```

#### Type Converters

```java
package com.movie88.data.database;

import androidx.room.TypeConverter;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.Date;
import java.util.List;

public class Converters {

    private static Gson gson = new Gson();

    @TypeConverter
    public static Date fromTimestamp(Long value) {
        return value == null ? null : new Date(value);
    }

    @TypeConverter
    public static Long dateToTimestamp(Date date) {
        return date == null ? null : date.getTime();
    }

    @TypeConverter
    public static String fromList(List<String> list) {
        return gson.toJson(list);
    }

    @TypeConverter
    public static List<String> toList(String value) {
        Type listType = new TypeToken<List<String>>(){}.getType();
        return gson.fromJson(value, listType);
    }
}
```

#### Movie Entity & DAO

```java
package com.movie88.data.database.entities;

import androidx.room.Entity;
import androidx.room.PrimaryKey;

import java.util.Date;

@Entity(tableName = "movies")
public class MovieEntity {

    @PrimaryKey
    private int movieId;
    private String title;
    private String overview;
    private String posterUrl;
    private String backdropUrl;
    private double rating;
    private String genre;
    private int duration;
    private Date cachedAt;

    // Getters and setters...
}

package com.movie88.data.database.dao;

import androidx.room.Dao;
import androidx.room.Insert;
import androidx.room.OnConflictStrategy;
import androidx.room.Query;

import com.movie88.data.database.entities.MovieEntity;

import java.util.List;

@Dao
public interface MovieDao {

    @Query("SELECT * FROM movies")
    List<MovieEntity> getAllMovies();

    @Query("SELECT * FROM movies WHERE movieId = :movieId")
    MovieEntity getMovieById(int movieId);

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    void insertMovies(List<MovieEntity> movies);

    @Query("DELETE FROM movies")
    void deleteAll();

    @Query("SELECT * FROM movies WHERE cachedAt > :minTime")
    List<MovieEntity> getMoviesCachedAfter(long minTime);
}
```

#### Offline-First Repository Pattern

```java
package com.movie88.data.repository;

import com.movie88.data.api.ApiService;
import com.movie88.data.database.AppDatabase;
import com.movie88.data.database.dao.MovieDao;
import com.movie88.data.database.entities.MovieEntity;
import com.movie88.data.models.response.Movie;
import com.movie88.utils.ApiCallback;
import com.movie88.utils.NetworkUtils;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

public class MovieRepository {

    private static MovieRepository instance;
    private final ApiService apiService;
    private final MovieDao movieDao;
    private final Executor executor;

    private static final long CACHE_DURATION = 30 * 60 * 1000; // 30 minutes

    private MovieRepository(AppDatabase database) {
        this.apiService = ApiClient.getApiService();
        this.movieDao = database.movieDao();
        this.executor = Executors.newSingleThreadExecutor();
    }

    public static synchronized MovieRepository getInstance(AppDatabase database) {
        if (instance == null) {
            instance = new MovieRepository(database);
        }
        return instance;
    }

    public void getMovies(ApiCallback<List<Movie>> callback) {
        // Check if online
        if (NetworkUtils.isNetworkAvailable()) {
            // Fetch from API
            fetchMoviesFromApi(callback);
        } else {
            // Fetch from cache
            fetchMoviesFromCache(callback);
        }
    }

    private void fetchMoviesFromApi(ApiCallback<List<Movie>> callback) {
        apiService.getMovies(1, 50).enqueue(new retrofit2.Callback<ApiResponse<PagedResult<Movie>>>() {
            @Override
            public void onResponse(retrofit2.Call<ApiResponse<PagedResult<Movie>>> call,
                                 retrofit2.Response<ApiResponse<PagedResult<Movie>>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<Movie> movies = response.body().getData().getItems();
                    
                    // Cache movies in background
                    executor.execute(() -> cacheMovies(movies));
                    
                    callback.onSuccess(movies);
                } else {
                    // API failed, fallback to cache
                    fetchMoviesFromCache(callback);
                }
            }

            @Override
            public void onFailure(retrofit2.Call<ApiResponse<PagedResult<Movie>>> call, Throwable t) {
                // Network error, fallback to cache
                fetchMoviesFromCache(callback);
            }
        });
    }

    private void fetchMoviesFromCache(ApiCallback<List<Movie>> callback) {
        executor.execute(() -> {
            long minCacheTime = System.currentTimeMillis() - CACHE_DURATION;
            List<MovieEntity> cachedMovies = movieDao.getMoviesCachedAfter(minCacheTime);
            
            if (cachedMovies != null && !cachedMovies.isEmpty()) {
                List<Movie> movies = convertEntitiesToMovies(cachedMovies);
                new android.os.Handler(android.os.Looper.getMainLooper()).post(() -> {
                    callback.onSuccess(movies);
                });
            } else {
                new android.os.Handler(android.os.Looper.getMainLooper()).post(() -> {
                    callback.onError("Không có dữ liệu. Vui lòng kiểm tra kết nối internet.");
                });
            }
        });
    }

    private void cacheMovies(List<Movie> movies) {
        List<MovieEntity> entities = new ArrayList<>();
        long currentTime = System.currentTimeMillis();
        
        for (Movie movie : movies) {
            MovieEntity entity = new MovieEntity();
            entity.setMovieId(movie.getMovieId());
            entity.setTitle(movie.getTitle());
            entity.setOverview(movie.getOverview());
            entity.setPosterUrl(movie.getPosterUrl());
            entity.setBackdropUrl(movie.getBackdropUrl());
            entity.setRating(movie.getRating());
            entity.setGenre(movie.getGenre());
            entity.setDuration(movie.getDuration());
            entity.setCachedAt(new java.util.Date(currentTime));
            entities.add(entity);
        }
        
        movieDao.insertMovies(entities);
    }

    private List<Movie> convertEntitiesToMovies(List<MovieEntity> entities) {
        List<Movie> movies = new ArrayList<>();
        for (MovieEntity entity : entities) {
            Movie movie = new Movie();
            movie.setMovieId(entity.getMovieId());
            movie.setTitle(entity.getTitle());
            movie.setOverview(entity.getOverview());
            movie.setPosterUrl(entity.getPosterUrl());
            movie.setBackdropUrl(entity.getBackdropUrl());
            movie.setRating(entity.getRating());
            movie.setGenre(entity.getGenre());
            movie.setDuration(entity.getDuration());
            movies.add(movie);
        }
        return movies;
    }

    public void clearCache() {
        executor.execute(() -> movieDao.deleteAll());
    }
}
```

---

## 3️⃣ Push Notifications with Firebase

### Purpose
Nhận thông báo về phim mới, suất chiếu, khuyến mãi.

### Firebase Setup

#### Dependencies

```gradle
// Firebase
implementation platform('com.google.firebase:firebase-bom:32.7.0')
implementation 'com.google.firebase:firebase-messaging'
implementation 'com.google.firebase:firebase-analytics'
```

#### FirebaseMessagingService

```java
package com.movie88.services;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Intent;
import android.os.Build;

import androidx.core.app.NotificationCompat;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;
import com.movie88.R;
import com.movie88.ui.MainActivity;

public class MovieFirebaseMessagingService extends FirebaseMessagingService {

    private static final String CHANNEL_ID = "movie88_notifications";
    private static final String CHANNEL_NAME = "Movie88 Notifications";

    @Override
    public void onNewToken(String token) {
        super.onNewToken(token);
        
        // Send token to server
        sendTokenToServer(token);
    }

    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        super.onMessageReceived(remoteMessage);

        // Check if message contains data payload
        if (remoteMessage.getData().size() > 0) {
            String title = remoteMessage.getData().get("title");
            String body = remoteMessage.getData().get("body");
            String type = remoteMessage.getData().get("type");
            String data = remoteMessage.getData().get("data");
            
            showNotification(title, body, type, data);
        }

        // Check if message contains notification payload
        if (remoteMessage.getNotification() != null) {
            String title = remoteMessage.getNotification().getTitle();
            String body = remoteMessage.getNotification().getBody();
            showNotification(title, body, null, null);
        }
    }

    private void showNotification(String title, String body, String type, String data) {
        createNotificationChannel();

        // Create intent
        Intent intent = new Intent(this, MainActivity.class);
        intent.putExtra("NOTIFICATION_TYPE", type);
        intent.putExtra("NOTIFICATION_DATA", data);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);

        PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, intent,
            PendingIntent.FLAG_UPDATE_CURRENT | PendingIntent.FLAG_IMMUTABLE);

        // Build notification
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNEL_ID)
            .setSmallIcon(R.drawable.ic_notification)
            .setContentTitle(title)
            .setContentText(body)
            .setPriority(NotificationCompat.PRIORITY_HIGH)
            .setAutoCancel(true)
            .setContentIntent(pendingIntent);

        // Show notification
        NotificationManager notificationManager = getSystemService(NotificationManager.class);
        notificationManager.notify((int) System.currentTimeMillis(), builder.build());
    }

    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel channel = new NotificationChannel(
                CHANNEL_ID,
                CHANNEL_NAME,
                NotificationManager.IMPORTANCE_HIGH
            );
            channel.setDescription("Notifications for movie updates and promotions");
            
            NotificationManager notificationManager = getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(channel);
        }
    }

    private void sendTokenToServer(String token) {
        // Send FCM token to backend API
        // POST /api/users/fcm-token
    }
}
```

#### Register in AndroidManifest.xml

```xml
<service
    android:name=".services.MovieFirebaseMessagingService"
    android:exported="false">
    <intent-filter>
        <action android:name="com.google.firebase.MESSAGING_EVENT" />
    </intent-filter>
</service>
```

---

## 4️⃣ Pagination with RecyclerView

### Purpose
Load danh sách movies/bookings theo pages, infinite scroll.

### Pagination Helper

```java
package com.movie88.utils;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

public abstract class PaginationScrollListener extends RecyclerView.OnScrollListener {

    private LinearLayoutManager layoutManager;

    public PaginationScrollListener(LinearLayoutManager layoutManager) {
        this.layoutManager = layoutManager;
    }

    @Override
    public void onScrolled(@NonNull RecyclerView recyclerView, int dx, int dy) {
        super.onScrolled(recyclerView, dx, dy);

        int visibleItemCount = layoutManager.getChildCount();
        int totalItemCount = layoutManager.getItemCount();
        int firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition();

        if (!isLoading() && !isLastPage()) {
            if ((visibleItemCount + firstVisibleItemPosition) >= totalItemCount
                && firstVisibleItemPosition >= 0) {
                loadMoreItems();
            }
        }
    }

    protected abstract void loadMoreItems();
    public abstract boolean isLastPage();
    public abstract boolean isLoading();
}
```

### ViewModel with Pagination

```java
package com.movie88.ui.movie;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;

import com.movie88.data.models.response.Movie;
import com.movie88.data.repository.MovieRepository;
import com.movie88.utils.ApiCallback;

import java.util.ArrayList;
import java.util.List;

public class MovieListViewModel extends ViewModel {

    private MovieRepository repository;
    
    private MutableLiveData<List<Movie>> movies = new MutableLiveData<>();
    private MutableLiveData<Boolean> isLoading = new MutableLiveData<>();
    private MutableLiveData<String> error = new MutableLiveData<>();
    
    private int currentPage = 1;
    private boolean isLastPage = false;
    private static final int PAGE_SIZE = 20;

    public MovieListViewModel() {
        repository = MovieRepository.getInstance();
    }

    public void loadMovies() {
        if (isLoading.getValue() != null && isLoading.getValue()) {
            return; // Already loading
        }

        isLoading.setValue(true);
        
        repository.getMovies(currentPage, PAGE_SIZE, new ApiCallback<PagedResult<Movie>>() {
            @Override
            public void onSuccess(PagedResult<Movie> result) {
                isLoading.setValue(false);
                
                // Append new movies to existing list
                List<Movie> currentMovies = movies.getValue();
                if (currentMovies == null) {
                    currentMovies = new ArrayList<>();
                }
                currentMovies.addAll(result.getItems());
                movies.setValue(currentMovies);
                
                // Update pagination state
                currentPage++;
                isLastPage = !result.hasNextPage();
            }

            @Override
            public void onError(String errorMessage) {
                isLoading.setValue(false);
                error.setValue(errorMessage);
            }
        });
    }

    public void refresh() {
        currentPage = 1;
        isLastPage = false;
        movies.setValue(new ArrayList<>());
        loadMovies();
    }

    public boolean isLastPage() {
        return isLastPage;
    }

    // Getters for LiveData
    public LiveData<List<Movie>> getMovies() {
        return movies;
    }

    public LiveData<Boolean> getIsLoading() {
        return isLoading;
    }

    public LiveData<String> getError() {
        return error;
    }
}
```

### Activity with Pagination

```java
RecyclerView recyclerView = findViewById(R.id.recyclerView);
LinearLayoutManager layoutManager = new LinearLayoutManager(this);
recyclerView.setLayoutManager(layoutManager);

MovieAdapter adapter = new MovieAdapter(new ArrayList<>());
recyclerView.setAdapter(adapter);

// Setup pagination
recyclerView.addOnScrollListener(new PaginationScrollListener(layoutManager) {
    @Override
    protected void loadMoreItems() {
        viewModel.loadMovies();
    }

    @Override
    public boolean isLastPage() {
        return viewModel.isLastPage();
    }

    @Override
    public boolean isLoading() {
        Boolean loading = viewModel.getIsLoading().getValue();
        return loading != null && loading;
    }
});

// Observe data
viewModel.getMovies().observe(this, movies -> {
    adapter.updateData(movies);
});

// Initial load
viewModel.loadMovies();
```

---

**Last Updated**: October 29, 2025
