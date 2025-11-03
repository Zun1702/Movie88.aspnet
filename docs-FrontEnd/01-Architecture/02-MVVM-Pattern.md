# 🏗️ MVVM Architecture Pattern

## Tổng quan

Cinema Booking System sử dụng **MVVM (Model-View-ViewModel)** architecture pattern để tách biệt business logic khỏi UI, giúp code dễ test và maintain.

```
┌─────────────────────────────────────────────────────────────┐
│                         VIEW                                │
│  (Activity, Fragment, XML Layout)                           │
│  - Hiển thị UI                                              │
│  - Observe LiveData từ ViewModel                           │
│  - User interactions                                        │
└──────────────────┬──────────────────────────────────────────┘
                   │ observes LiveData
                   │ calls methods
                   ▼
┌─────────────────────────────────────────────────────────────┐
│                      VIEWMODEL                              │
│  - Giữ UI state trong LiveData                             │
│  - Xử lý business logic                                     │
│  - Calls Repository methods                                │
│  - Survive configuration changes                           │
└──────────────────┬──────────────────────────────────────────┘
                   │ calls
                   ▼
┌─────────────────────────────────────────────────────────────┐
│                     REPOSITORY                              │
│  - Single source of truth                                   │
│  - Quản lý data sources (API, Database)                    │
│  - Caching strategy                                         │
│  - Data mapping                                             │
└──────────────────┬──────────────────────────────────────────┘
                   │
        ┌──────────┴───────────┐
        ▼                      ▼
┌──────────────┐      ┌──────────────────┐
│  API SERVICE │      │  LOCAL DATABASE  │
│  (Retrofit)  │      │     (Room)       │
│              │      │                  │
│ - REST APIs  │      │ - Cache data     │
│ - JWT Auth   │      │ - Offline mode   │
└──────────────┘      └──────────────────┘
        │                      │
        └──────────┬───────────┘
                   ▼
              ┌─────────┐
              │  MODEL  │
              │         │
              │ - DTOs  │
              │ - Entities │
              └─────────┘
```

---

## 📂 Folder Structure

```
app/src/main/java/com/movie88/
│
├── ui/                                    # VIEW Layer
│   ├── auth/
│   │   ├── LoginActivity.java           # Activity
│   │   ├── LoginViewModel.java          # ViewModel
│   │   └── RegisterActivity.java
│   ├── main/
│   │   ├── MainActivity.java
│   │   ├── HomeFragment.java
│   │   └── HomeViewModel.java
│   ├── movie/
│   ├── booking/
│   ├── payment/
│   └── profile/
│
├── data/                                  # DATA Layer
│   ├── api/                              # Remote data source
│   │   ├── ApiService.java              # Retrofit interface
│   │   ├── ApiClient.java               # Retrofit builder
│   │   └── interceptors/
│   │       ├── AuthInterceptor.java     # JWT token
│   │       └── LoggingInterceptor.java
│   │
│   ├── database/                         # Local data source
│   │   ├── AppDatabase.java             # Room database
│   │   ├── dao/
│   │   │   ├── MovieDao.java
│   │   │   └── BookingDao.java
│   │   └── entities/
│   │       ├── MovieEntity.java
│   │       └── BookingEntity.java
│   │
│   ├── repository/                       # REPOSITORY Layer
│   │   ├── AuthRepository.java
│   │   ├── MovieRepository.java
│   │   ├── BookingRepository.java
│   │   ├── PaymentRepository.java
│   │   └── UserRepository.java
│   │
│   └── models/                           # MODEL Layer
│       ├── request/                      # API Request DTOs
│       │   ├── LoginRequest.java
│       │   ├── RegisterRequest.java
│       │   └── CreateBookingRequest.java
│       ├── response/                     # API Response DTOs
│       │   ├── LoginResponse.java
│       │   ├── MovieResponse.java
│       │   └── BookingResponse.java
│       └── domain/                       # Domain models
│           ├── Movie.java
│           ├── User.java
│           └── Booking.java
│
└── utils/                                 # UTILITIES
    ├── Constants.java
    ├── SharedPrefsManager.java
    ├── TokenManager.java
    └── DateUtils.java
```

---

## 1️⃣ VIEW Layer (Activity/Fragment)

### Trách nhiệm:
- ✅ Inflate XML layouts
- ✅ Initialize views (findViewById)
- ✅ Observe LiveData từ ViewModel
- ✅ Update UI khi data thay đổi
- ✅ Handle user interactions (onClick, onTextChanged)
- ✅ Navigate giữa các screens

### ❌ KHÔNG được:
- ❌ Gọi API trực tiếp
- ❌ Access database trực tiếp
- ❌ Xử lý business logic
- ❌ Giữ state (state phải ở ViewModel)

### Example: LoginActivity.java

```java
public class LoginActivity extends AppCompatActivity {
    
    // Views
    private TextInputEditText etEmail, etPassword;
    private MaterialButton btnLogin;
    private ProgressBar progressBar;
    
    // ViewModel
    private LoginViewModel viewModel;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        
        initViews();
        initViewModel();
        setupListeners();
        observeViewModel();
    }
    
    private void initViews() {
        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        btnLogin = findViewById(R.id.btnLogin);
        progressBar = findViewById(R.id.progressBar);
    }
    
    private void initViewModel() {
        // ViewModelProvider tạo hoặc lấy existing ViewModel
        viewModel = new ViewModelProvider(this).get(LoginViewModel.class);
    }
    
    private void setupListeners() {
        btnLogin.setOnClickListener(v -> {
            String email = etEmail.getText().toString().trim();
            String password = etPassword.getText().toString().trim();
            
            // Validation
            if (TextUtils.isEmpty(email)) {
                etEmail.setError("Email không được để trống");
                return;
            }
            
            // Call ViewModel method
            viewModel.login(email, password);
        });
    }
    
    private void observeViewModel() {
        // Observe loading state
        viewModel.getIsLoading().observe(this, isLoading -> {
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            btnLogin.setEnabled(!isLoading);
        });
        
        // Observe login result
        viewModel.getLoginResult().observe(this, result -> {
            if (result.isSuccess()) {
                LoginResponse response = result.getData();
                Toast.makeText(this, "Đăng nhập thành công!", Toast.LENGTH_SHORT).show();
                
                // Navigate to MainActivity
                Intent intent = new Intent(this, MainActivity.class);
                startActivity(intent);
                finish();
            } else {
                Toast.makeText(this, result.getError(), Toast.LENGTH_SHORT).show();
            }
        });
        
        // Observe errors
        viewModel.getError().observe(this, error -> {
            if (error != null) {
                showErrorDialog(error);
            }
        });
    }
    
    private void showErrorDialog(String message) {
        new MaterialAlertDialogBuilder(this)
            .setTitle("Lỗi")
            .setMessage(message)
            .setPositiveButton("OK", null)
            .show();
    }
}
```

---

## 2️⃣ VIEWMODEL Layer

### Trách nhiệm:
- ✅ Giữ UI state trong LiveData/MutableLiveData
- ✅ Xử lý business logic
- ✅ Call Repository methods
- ✅ Transform data cho UI
- ✅ Survive configuration changes (screen rotation)
- ✅ Lifecycle-aware (tự động cleanup khi Activity destroyed)

### ❌ KHÔNG được:
- ❌ Hold reference đến Activity/Fragment/View (memory leak!)
- ❌ Hold reference đến Context (dùng AndroidViewModel nếu cần Application context)
- ❌ Gọi API trực tiếp (phải qua Repository)

### Example: LoginViewModel.java

```java
public class LoginViewModel extends ViewModel {
    
    // Repository
    private final AuthRepository authRepository;
    
    // LiveData for UI state
    private final MutableLiveData<Boolean> isLoading = new MutableLiveData<>(false);
    private final MutableLiveData<Result<LoginResponse>> loginResult = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    
    // Constructor
    public LoginViewModel() {
        authRepository = AuthRepository.getInstance();
    }
    
    // Public methods called by View
    public void login(String email, String password) {
        // Validation
        if (!isValidEmail(email)) {
            error.setValue("Email không hợp lệ");
            return;
        }
        
        if (password.length() < 6) {
            error.setValue("Mật khẩu phải có ít nhất 6 ký tự");
            return;
        }
        
        // Set loading state
        isLoading.setValue(true);
        
        // Create request
        LoginRequest request = new LoginRequest(email, password);
        
        // Call repository
        authRepository.login(request, new ApiCallback<LoginResponse>() {
            @Override
            public void onSuccess(LoginResponse response) {
                isLoading.setValue(false);
                
                // Save token
                TokenManager.getInstance().saveToken(response.getToken());
                TokenManager.getInstance().saveRefreshToken(response.getRefreshToken());
                
                // Save user info
                SharedPrefsManager.getInstance().saveUser(response.getUser());
                
                // Notify View
                loginResult.setValue(Result.success(response));
            }
            
            @Override
            public void onError(String errorMessage) {
                isLoading.setValue(false);
                loginResult.setValue(Result.error(errorMessage));
            }
        });
    }
    
    private boolean isValidEmail(String email) {
        return Patterns.EMAIL_ADDRESS.matcher(email).matches();
    }
    
    // Getters for LiveData (View sẽ observe)
    public LiveData<Boolean> getIsLoading() {
        return isLoading;
    }
    
    public LiveData<Result<LoginResponse>> getLoginResult() {
        return loginResult;
    }
    
    public LiveData<String> getError() {
        return error;
    }
    
    @Override
    protected void onCleared() {
        super.onCleared();
        // Cleanup if needed (cancel ongoing requests, etc.)
    }
}
```

---

## 3️⃣ REPOSITORY Layer

### Trách nhiệm:
- ✅ Single source of truth cho data
- ✅ Decide data source (API vs Cache)
- ✅ Handle caching strategy
- ✅ Map API responses to domain models
- ✅ Coordinate giữa multiple data sources
- ✅ Error handling

### Example: AuthRepository.java

```java
public class AuthRepository {
    
    private static AuthRepository instance;
    private final ApiService apiService;
    
    // Singleton pattern
    private AuthRepository() {
        apiService = ApiClient.getInstance().getApiService();
    }
    
    public static synchronized AuthRepository getInstance() {
        if (instance == null) {
            instance = new AuthRepository();
        }
        return instance;
    }
    
    // Login method
    public void login(LoginRequest request, ApiCallback<LoginResponse> callback) {
        Call<ApiResponse<LoginResponse>> call = apiService.login(request);
        
        call.enqueue(new Callback<ApiResponse<LoginResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<LoginResponse>> call, 
                                 Response<ApiResponse<LoginResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<LoginResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Đăng nhập thất bại. Vui lòng thử lại.");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<LoginResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
    
    // Register method
    public void register(RegisterRequest request, ApiCallback<RegisterResponse> callback) {
        Call<ApiResponse<RegisterResponse>> call = apiService.register(request);
        
        call.enqueue(new Callback<ApiResponse<RegisterResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<RegisterResponse>> call, 
                                 Response<ApiResponse<RegisterResponse>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    ApiResponse<RegisterResponse> apiResponse = response.body();
                    
                    if (apiResponse.isSuccess()) {
                        callback.onSuccess(apiResponse.getData());
                    } else {
                        callback.onError(apiResponse.getMessage());
                    }
                } else {
                    callback.onError("Đăng ký thất bại. Vui lòng thử lại.");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<RegisterResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
}
```

---

## 4️⃣ MODEL Layer

### Request DTOs
```java
// LoginRequest.java
public class LoginRequest {
    private String email;
    private String password;
    
    public LoginRequest(String email, String password) {
        this.email = email;
        this.password = password;
    }
    
    // Getters and setters
}
```

### Response DTOs
```java
// LoginResponse.java
public class LoginResponse {
    private String token;
    private String refreshToken;
    private User user;
    
    // Getters and setters
}

// ApiResponse.java (Generic wrapper)
public class ApiResponse<T> {
    private boolean success;
    private String message;
    private T data;
    
    public boolean isSuccess() {
        return success;
    }
    
    // Getters and setters
}
```

### Domain Models
```java
// User.java
public class User {
    private int userId;
    private String email;
    private String name;
    private String phone;
    private String role;
    
    // Getters and setters
}
```

---

## 5️⃣ Utility Classes

### Result.java (Wrapper cho success/error)
```java
public class Result<T> {
    private final T data;
    private final String error;
    private final boolean success;
    
    private Result(T data, String error, boolean success) {
        this.data = data;
        this.error = error;
        this.success = success;
    }
    
    public static <T> Result<T> success(T data) {
        return new Result<>(data, null, true);
    }
    
    public static <T> Result<T> error(String error) {
        return new Result<>(null, error, false);
    }
    
    public boolean isSuccess() {
        return success;
    }
    
    public T getData() {
        return data;
    }
    
    public String getError() {
        return error;
    }
}
```

### ApiCallback.java (Interface cho async callbacks)
```java
public interface ApiCallback<T> {
    void onSuccess(T data);
    void onError(String errorMessage);
}
```

---

## 📊 Data Flow Example: User Login

```
1. USER ACTION
   LoginActivity: User clicks "Đăng nhập" button
   ↓

2. VIEW → VIEWMODEL
   LoginActivity.onClick() → viewModel.login(email, password)
   ↓

3. VIEWMODEL → REPOSITORY
   LoginViewModel.login() → authRepository.login(request, callback)
   ↓

4. REPOSITORY → API
   AuthRepository.login() → apiService.login(request)
   ↓

5. API → BACKEND
   Retrofit Call → POST /api/auth/login
   ↓

6. BACKEND RESPONSE
   Backend returns: { success: true, data: { token, user } }
   ↓

7. API → REPOSITORY
   onResponse() → Check apiResponse.isSuccess()
   ↓

8. REPOSITORY → VIEWMODEL
   callback.onSuccess(loginResponse)
   ↓

9. VIEWMODEL → VIEW
   loginResult.setValue(Result.success(response))
   ↓

10. VIEW UPDATES UI
    LoginActivity.observe() → Navigate to MainActivity
```

---

## 🎯 Best Practices

### ✅ DO:
1. **ViewModel survive configuration changes**
   - Dùng ViewModelProvider để tạo ViewModel
   - ViewModel sẽ survive screen rotation

2. **LiveData cho reactive UI**
   - View observe LiveData
   - UI tự động update khi data thay đổi

3. **Single Responsibility**
   - View chỉ lo UI
   - ViewModel chỉ lo logic
   - Repository chỉ lo data

4. **Dependency Injection**
   - Repository inject vào ViewModel (constructor)
   - ViewModel inject vào View (ViewModelProvider)

5. **Error Handling**
   - Repository catch exceptions
   - ViewModel transform errors
   - View show errors to user

### ❌ DON'T:
1. **View KHÔNG gọi API trực tiếp**
   ```java
   // ❌ BAD
   apiService.getMovies().enqueue(new Callback<>() { ... });
   
   // ✅ GOOD
   viewModel.loadMovies();
   ```

2. **ViewModel KHÔNG hold View reference**
   ```java
   // ❌ BAD (Memory Leak!)
   private LoginActivity activity;
   
   // ✅ GOOD
   private MutableLiveData<String> message = new MutableLiveData<>();
   ```

3. **Repository KHÔNG biết về View**
   ```java
   // ❌ BAD
   public void login(LoginRequest request, LoginActivity activity) { ... }
   
   // ✅ GOOD
   public void login(LoginRequest request, ApiCallback<LoginResponse> callback) { ... }
   ```

---

## 🧪 Testing Strategy

### Unit Test ViewModel
```java
@Test
public void login_withValidCredentials_shouldSucceed() {
    // Given
    String email = "user@example.com";
    String password = "password123";
    
    // When
    viewModel.login(email, password);
    
    // Then
    LiveData<Result<LoginResponse>> result = viewModel.getLoginResult();
    assertTrue(result.getValue().isSuccess());
}
```

### Mock Repository
```java
@Mock
AuthRepository mockRepository;

@Before
public void setup() {
    MockitoAnnotations.initMocks(this);
    viewModel = new LoginViewModel(mockRepository);
}
```

---

## 📚 Reference

- [Android MVVM Architecture](https://developer.android.com/topic/architecture)
- [LiveData Overview](https://developer.android.com/topic/libraries/architecture/livedata)
- [ViewModel Overview](https://developer.android.com/topic/libraries/architecture/viewmodel)

**Last Updated**: October 29, 2025
