# 📱 Screen Specifications - Authentication Module

## 1. SplashScreen

### Purpose
Màn hình khởi động app, check authentication status và navigate appropriately.

### Layout (activity_splash.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.constraintlayout.widget.ConstraintLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <ImageView
        android:id="@+id/ivLogo"
        android:layout_width="200dp"
        android:layout_height="200dp"
        android:src="@drawable/ic_logo"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toBottomOf="parent"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

    <TextView
        android:id="@+id/tvAppName"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Movie88"
        android:textSize="@dimen/text_xxl"
        android:textColor="@color/colorPrimary"
        android:textStyle="bold"
        android:layout_marginTop="@dimen/spacing_md"
        app:layout_constraintTop_toBottomOf="@id/ivLogo"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

    <ProgressBar
        android:id="@+id/progressBar"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:indeterminateTint="@color/colorPrimary"
        android:layout_marginTop="@dimen/spacing_lg"
        app:layout_constraintTop_toBottomOf="@id/tvAppName"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

</androidx.constraintlayout.widget.ConstraintLayout>
```

### Java Code (SplashActivity.java)
```java
public class SplashActivity extends AppCompatActivity {
    
    private static final int SPLASH_DURATION = 2000; // 2 seconds
    private SharedPrefsManager prefsManager;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_splash);
        
        prefsManager = SharedPrefsManager.getInstance(this);
        
        // Delay and check authentication
        new Handler(Looper.getMainLooper()).postDelayed(() -> {
            checkAuthentication();
        }, SPLASH_DURATION);
    }
    
    private void checkAuthentication() {
        String token = prefsManager.getToken();
        
        if (token != null && !isTokenExpired(token)) {
            // Token valid → Go to Main
            navigateToMain();
        } else {
            // Token invalid → Go to Login
            navigateToLogin();
        }
    }
    
    private boolean isTokenExpired(String token) {
        try {
            String[] parts = token.split("\\.");
            String payload = new String(Base64.decode(parts[1], Base64.DEFAULT));
            JSONObject jsonObject = new JSONObject(payload);
            long exp = jsonObject.getLong("exp");
            return System.currentTimeMillis() / 1000 > exp;
        } catch (Exception e) {
            return true;
        }
    }
    
    private void navigateToMain() {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }
    
    private void navigateToLogin() {
        Intent intent = new Intent(this, LoginActivity.class);
        startActivity(intent);
        finish();
    }
}
```

---

## 2. LoginActivity

### Purpose
Đăng nhập bằng email và password, nhận JWT token từ backend.

### Layout (activity_login.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<ScrollView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:fillViewport="true"
    android:background="@color/backgroundColor">

    <androidx.constraintlayout.widget.ConstraintLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_lg">

        <!-- Logo -->
        <ImageView
            android:id="@+id/ivLogo"
            android:layout_width="120dp"
            android:layout_height="120dp"
            android:src="@drawable/ic_logo"
            android:layout_marginTop="@dimen/spacing_xl"
            app:layout_constraintTop_toTopOf="parent"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintEnd_toEndOf="parent"/>

        <!-- Welcome Text -->
        <TextView
            android:id="@+id/tvWelcome"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Chào mừng trở lại!"
            android:textSize="@dimen/text_xxl"
            android:textColor="@color/textPrimary"
            android:textStyle="bold"
            android:layout_marginTop="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/ivLogo"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintEnd_toEndOf="parent"/>

        <!-- Email Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilEmail"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Email"
            android:layout_marginTop="@dimen/spacing_xl"
            app:startIconDrawable="@drawable/ic_email"
            app:startIconTint="@color/colorAccent"
            app:boxStrokeColor="@color/colorAccent"
            app:hintTextColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tvWelcome">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etEmail"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="textEmailAddress"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Password Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilPassword"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Mật khẩu"
            android:layout_marginTop="@dimen/spacing_md"
            app:startIconDrawable="@drawable/ic_lock"
            app:startIconTint="@color/colorAccent"
            app:endIconMode="password_toggle"
            app:endIconTint="@color/colorAccent"
            app:boxStrokeColor="@color/colorAccent"
            app:hintTextColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tilEmail">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etPassword"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="textPassword"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Forgot Password -->
        <TextView
            android:id="@+id/tvForgotPassword"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Quên mật khẩu?"
            android:textSize="@dimen/text_md"
            android:textColor="@color/colorAccent"
            android:layout_marginTop="@dimen/spacing_sm"
            app:layout_constraintTop_toBottomOf="@id/tilPassword"
            app:layout_constraintEnd_toEndOf="parent"/>

        <!-- Login Button -->
        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnLogin"
            android:layout_width="match_parent"
            android:layout_height="@dimen/button_height"
            android:text="Đăng nhập"
            android:textSize="@dimen/text_lg"
            android:textColor="@color/textPrimary"
            android:backgroundTint="@color/colorPrimary"
            app:cornerRadius="@dimen/button_corner_radius"
            android:layout_marginTop="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/tvForgotPassword"/>

        <!-- Loading Indicator -->
        <ProgressBar
            android:id="@+id/progressBar"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:visibility="gone"
            android:indeterminateTint="@color/colorPrimary"
            app:layout_constraintTop_toTopOf="@id/btnLogin"
            app:layout_constraintBottom_toBottomOf="@id/btnLogin"
            app:layout_constraintStart_toStartOf="@id/btnLogin"
            app:layout_constraintEnd_toEndOf="@id/btnLogin"/>

        <!-- Divider with "or" -->
        <View
            android:id="@+id/divider1"
            android:layout_width="0dp"
            android:layout_height="1dp"
            android:background="@color/textTertiary"
            android:layout_marginTop="@dimen/spacing_lg"
            android:layout_marginEnd="@dimen/spacing_sm"
            app:layout_constraintTop_toBottomOf="@id/btnLogin"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintEnd_toStartOf="@id/tvOr"
            app:layout_constraintWidth_percent="0.4"/>

        <TextView
            android:id="@+id/tvOr"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="hoặc"
            android:textColor="@color/textSecondary"
            android:layout_marginTop="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/btnLogin"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintEnd_toEndOf="parent"/>

        <View
            android:id="@+id/divider2"
            android:layout_width="0dp"
            android:layout_height="1dp"
            android:background="@color/textTertiary"
            android:layout_marginTop="@dimen/spacing_lg"
            android:layout_marginStart="@dimen/spacing_sm"
            app:layout_constraintTop_toBottomOf="@id/btnLogin"
            app:layout_constraintStart_toEndOf="@id/tvOr"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintWidth_percent="0.4"/>

        <!-- Register Link -->
        <TextView
            android:id="@+id/tvRegister"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Chưa có tài khoản? Đăng ký ngay"
            android:textSize="@dimen/text_md"
            android:textColor="@color/colorAccent"
            android:layout_marginTop="@dimen/spacing_lg"
            android:layout_marginBottom="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/divider1"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintBottom_toBottomOf="parent"/>

    </androidx.constraintlayout.widget.ConstraintLayout>

</ScrollView>
```

### Java Code (LoginActivity.java)
```java
public class LoginActivity extends AppCompatActivity {
    
    private TextInputEditText etEmail, etPassword;
    private MaterialButton btnLogin;
    private ProgressBar progressBar;
    private TextView tvForgotPassword, tvRegister;
    
    private LoginViewModel viewModel;
    private SharedPrefsManager prefsManager;
    
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
        tvForgotPassword = findViewById(R.id.tvForgotPassword);
        tvRegister = findViewById(R.id.tvRegister);
        
        prefsManager = SharedPrefsManager.getInstance(this);
    }
    
    private void initViewModel() {
        viewModel = new ViewModelProvider(this).get(LoginViewModel.class);
    }
    
    private void setupListeners() {
        btnLogin.setOnClickListener(v -> attemptLogin());
        
        tvForgotPassword.setOnClickListener(v -> {
            startActivity(new Intent(this, ForgotPasswordActivity.class));
        });
        
        tvRegister.setOnClickListener(v -> {
            startActivity(new Intent(this, RegisterActivity.class));
        });
    }
    
    private void attemptLogin() {
        String email = etEmail.getText().toString().trim();
        String password = etPassword.getText().toString().trim();
        
        // Validate inputs
        if (!isValidEmail(email)) {
            etEmail.setError("Email không hợp lệ");
            return;
        }
        
        if (password.isEmpty() || password.length() < 6) {
            etPassword.setError("Mật khẩu phải có ít nhất 6 ký tự");
            return;
        }
        
        // Call ViewModel
        LoginRequest request = new LoginRequest(email, password);
        viewModel.login(request);
    }
    
    private void observeViewModel() {
        // Observe loading state
        viewModel.getIsLoading().observe(this, isLoading -> {
            btnLogin.setEnabled(!isLoading);
            progressBar.setVisibility(isLoading ? View.VISIBLE : View.GONE);
            btnLogin.setText(isLoading ? "" : "Đăng nhập");
        });
        
        // Observe login result
        viewModel.getLoginResult().observe(this, result -> {
            if (result.isSuccess()) {
                LoginResponse response = result.getData();
                
                // Save token
                prefsManager.saveToken(response.getToken());
                prefsManager.saveRefreshToken(response.getRefreshToken());
                prefsManager.saveUser(response.getUser());
                
                // Navigate to Main
                Intent intent = new Intent(this, MainActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                startActivity(intent);
                finish();
                
                Toast.makeText(this, "Đăng nhập thành công!", Toast.LENGTH_SHORT).show();
            }
        });
        
        // Observe errors
        viewModel.getError().observe(this, error -> {
            if (error != null) {
                showErrorDialog(error);
            }
        });
    }
    
    private boolean isValidEmail(String email) {
        return android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches();
    }
    
    private void showErrorDialog(String message) {
        new MaterialAlertDialogBuilder(this)
            .setTitle("Đăng nhập thất bại")
            .setMessage(message)
            .setPositiveButton("OK", null)
            .show();
    }
}
```

### ViewModel (LoginViewModel.java)
```java
public class LoginViewModel extends ViewModel {
    
    private final AuthRepository authRepository;
    
    private final MutableLiveData<Boolean> isLoading = new MutableLiveData<>();
    private final MutableLiveData<Result<LoginResponse>> loginResult = new MutableLiveData<>();
    private final MutableLiveData<String> error = new MutableLiveData<>();
    
    public LoginViewModel() {
        authRepository = AuthRepository.getInstance();
    }
    
    public void login(LoginRequest request) {
        isLoading.setValue(true);
        
        authRepository.login(request, new ApiCallback<LoginResponse>() {
            @Override
            public void onSuccess(LoginResponse response) {
                isLoading.setValue(false);
                loginResult.setValue(Result.success(response));
            }
            
            @Override
            public void onError(String errorMessage) {
                isLoading.setValue(false);
                error.setValue(errorMessage);
            }
        });
    }
    
    // Getters
    public LiveData<Boolean> getIsLoading() {
        return isLoading;
    }
    
    public LiveData<Result<LoginResponse>> getLoginResult() {
        return loginResult;
    }
    
    public LiveData<String> getError() {
        return error;
    }
}
```

### API Endpoints Used

**LoginActivity:**
- `POST /api/auth/login` - Đăng nhập với email và password, trả về JWT token

**RegisterActivity:**
- `POST /api/auth/register` - Đăng ký tài khoản mới

**SplashActivity:**
- `POST /api/auth/refresh-token` - Refresh JWT token khi token hiện tại hết hạn

---

### API Integration (AuthRepository.java)
```java
public class AuthRepository {
    
    private static AuthRepository instance;
    private final ApiService apiService;
    
    private AuthRepository() {
        apiService = RetrofitClient.getInstance().create(ApiService.class);
    }
    
    public static AuthRepository getInstance() {
        if (instance == null) {
            instance = new AuthRepository();
        }
        return instance;
    }
    
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
                    callback.onError("Đăng nhập thất bại");
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<LoginResponse>> call, Throwable t) {
                callback.onError("Lỗi kết nối: " + t.getMessage());
            }
        });
    }
}
```

### API Service Interface
```java
public interface ApiService {
    
    @POST("api/auth/login")
    Call<ApiResponse<LoginResponse>> login(@Body LoginRequest request);
    
    @POST("api/auth/register")
    Call<ApiResponse<RegisterResponse>> register(@Body RegisterRequest request);
    
    @POST("api/auth/refresh-token")
    Call<ApiResponse<TokenResponse>> refreshToken(@Body RefreshTokenRequest request);
    
    @POST("api/auth/forgot-password")
    Call<ApiResponse<Void>> forgotPassword(@Body ForgotPasswordRequest request);
}
```

### Models
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

// LoginResponse.java
public class LoginResponse {
    private String token;
    private String refreshToken;
    private User user;
    
    // Getters and setters
}

// User.java
public class User {
    private int userId;
    private String email;
    private String name;
    private String role;
    
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
    
    public String getMessage() {
        return message;
    }
    
    public T getData() {
        return data;
    }
}
```

---

## 3. RegisterActivity

### Layout (activity_register.xml)
```xml
<?xml version="1.0" encoding="utf-8"?>
<ScrollView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:fillViewport="true"
    android:background="@color/backgroundColor">

    <androidx.constraintlayout.widget.ConstraintLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:padding="@dimen/spacing_lg">

        <!-- Back Button -->
        <ImageButton
            android:id="@+id/btnBack"
            android:layout_width="48dp"
            android:layout_height="48dp"
            android:src="@drawable/ic_arrow_back"
            android:background="?attr/selectableItemBackgroundBorderless"
            android:tint="@color/textPrimary"
            app:layout_constraintTop_toTopOf="parent"
            app:layout_constraintStart_toStartOf="parent"/>

        <!-- Title -->
        <TextView
            android:id="@+id/tvTitle"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Đăng ký tài khoản"
            android:textSize="@dimen/text_xxl"
            android:textColor="@color/textPrimary"
            android:textStyle="bold"
            android:layout_marginTop="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/btnBack"
            app:layout_constraintStart_toStartOf="parent"/>

        <!-- Name Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilName"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Họ và tên"
            android:layout_marginTop="@dimen/spacing_lg"
            app:startIconDrawable="@drawable/ic_person"
            app:startIconTint="@color/colorAccent"
            app:boxStrokeColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tvTitle">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etName"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="textPersonName"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Email Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilEmail"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Email"
            android:layout_marginTop="@dimen/spacing_md"
            app:startIconDrawable="@drawable/ic_email"
            app:startIconTint="@color/colorAccent"
            app:boxStrokeColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tilName">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etEmail"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="textEmailAddress"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Phone Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilPhone"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Số điện thoại"
            android:layout_marginTop="@dimen/spacing_md"
            app:startIconDrawable="@drawable/ic_phone"
            app:startIconTint="@color/colorAccent"
            app:boxStrokeColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tilEmail">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etPhone"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="phone"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Password Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilPassword"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Mật khẩu"
            android:layout_marginTop="@dimen/spacing_md"
            app:startIconDrawable="@drawable/ic_lock"
            app:startIconTint="@color/colorAccent"
            app:endIconMode="password_toggle"
            app:boxStrokeColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tilPhone">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etPassword"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="textPassword"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Confirm Password Input -->
        <com.google.android.material.textfield.TextInputLayout
            android:id="@+id/tilConfirmPassword"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:hint="Xác nhận mật khẩu"
            android:layout_marginTop="@dimen/spacing_md"
            app:startIconDrawable="@drawable/ic_lock"
            app:startIconTint="@color/colorAccent"
            app:endIconMode="password_toggle"
            app:boxStrokeColor="@color/colorAccent"
            style="@style/Widget.MaterialComponents.TextInputLayout.OutlinedBox"
            app:layout_constraintTop_toBottomOf="@id/tilPassword">

            <com.google.android.material.textfield.TextInputEditText
                android:id="@+id/etConfirmPassword"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:inputType="textPassword"
                android:textColor="@color/textPrimary"/>

        </com.google.android.material.textfield.TextInputLayout>

        <!-- Terms Checkbox -->
        <CheckBox
            android:id="@+id/cbTerms"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:text="Tôi đồng ý với Điều khoản sử dụng và Chính sách bảo mật"
            android:textColor="@color/textSecondary"
            android:textSize="@dimen/text_sm"
            android:buttonTint="@color/colorAccent"
            android:layout_marginTop="@dimen/spacing_md"
            app:layout_constraintTop_toBottomOf="@id/tilConfirmPassword"/>

        <!-- Register Button -->
        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnRegister"
            android:layout_width="match_parent"
            android:layout_height="@dimen/button_height"
            android:text="Đăng ký"
            android:textSize="@dimen/text_lg"
            android:textColor="@color/textPrimary"
            android:backgroundTint="@color/colorPrimary"
            app:cornerRadius="@dimen/button_corner_radius"
            android:layout_marginTop="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/cbTerms"/>

        <!-- Progress Bar -->
        <ProgressBar
            android:id="@+id/progressBar"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:visibility="gone"
            android:indeterminateTint="@color/colorPrimary"
            app:layout_constraintTop_toTopOf="@id/btnRegister"
            app:layout_constraintBottom_toBottomOf="@id/btnRegister"
            app:layout_constraintStart_toStartOf="@id/btnRegister"
            app:layout_constraintEnd_toEndOf="@id/btnRegister"/>

        <!-- Login Link -->
        <TextView
            android:id="@+id/tvLogin"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Đã có tài khoản? Đăng nhập"
            android:textSize="@dimen/text_md"
            android:textColor="@color/colorAccent"
            android:layout_marginTop="@dimen/spacing_lg"
            android:layout_marginBottom="@dimen/spacing_lg"
            app:layout_constraintTop_toBottomOf="@id/btnRegister"
            app:layout_constraintStart_toStartOf="parent"
            app:layout_constraintEnd_toEndOf="parent"
            app:layout_constraintBottom_toBottomOf="parent"/>

    </androidx.constraintlayout.widget.ConstraintLayout>

</ScrollView>
```

### API Endpoint
```
POST /api/auth/register

Request Body:
{
  "email": "user@example.com",
  "password": "123456",
  "name": "Nguyen Van A",
  "phone": "0901234567"
}

Response:
{
  "success": true,
  "message": "Đăng ký thành công",
  "data": {
    "userId": 123,
    "email": "user@example.com",
    "name": "Nguyen Van A"
  }
}
```

---

**Next**: HomeFragment, MovieDetailActivity specifications

**Last Updated**: October 29, 2025
