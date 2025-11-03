# 👤 Profile & User Screens

## Tổng quan

Chi tiết các màn hình quản lý user: ProfileFragment, EditProfileActivity, BookingHistoryFragment, SettingsActivity.

---

## 1️⃣ ProfileFragment

### Purpose
Hiển thị thông tin user, menu settings, booking history.

### Layout: fragment_profile.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<ScrollView 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="@color/backgroundColor">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical">

        <!-- User Info Card -->
        <com.google.android.material.card.MaterialCardView
            style="@style/CardView"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_margin="@dimen/spacing_base">

            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal"
                android:padding="@dimen/spacing_base"
                android:gravity="center_vertical">

                <!-- Avatar -->
                <com.google.android.material.card.MaterialCardView
                    android:layout_width="@dimen/avatar_size_large"
                    android:layout_height="@dimen/avatar_size_large"
                    app:cardCornerRadius="@dimen/avatar_size_large"
                    app:cardElevation="0dp">

                    <ImageView
                        android:id="@+id/ivAvatar"
                        android:layout_width="match_parent"
                        android:layout_height="match_parent"
                        android:scaleType="centerCrop"
                        android:src="@drawable/ic_avatar_placeholder"/>

                    <ImageView
                        android:id="@+id/btnChangeAvatar"
                        android:layout_width="24dp"
                        android:layout_height="24dp"
                        android:layout_gravity="bottom|end"
                        android:layout_margin="4dp"
                        android:padding="4dp"
                        android:src="@drawable/ic_camera"
                        android:background="@drawable/bg_circle_primary"/>

                </com.google.android.material.card.MaterialCardView>

                <!-- User Info -->
                <LinearLayout
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:layout_marginStart="@dimen/spacing_base"
                    android:orientation="vertical">

                    <TextView
                        android:id="@+id/tvFullName"
                        style="@style/Text.Heading2"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:text="Nguyễn Văn A"/>

                    <TextView
                        android:id="@+id/tvEmail"
                        style="@style/Text.BodySecondary"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:text="nguyenvana@email.com"/>

                    <TextView
                        android:id="@+id/tvPhone"
                        style="@style/Text.BodySecondary"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:layout_marginTop="@dimen/spacing_xs"
                        android:text="0987654321"/>

                </LinearLayout>

                <!-- Edit Button -->
                <ImageView
                    android:id="@+id/btnEditProfile"
                    android:layout_width="@dimen/icon_size_lg"
                    android:layout_height="@dimen/icon_size_lg"
                    android:padding="@dimen/spacing_xs"
                    android:src="@drawable/ic_edit"
                    app:tint="@color/colorPrimary"/>

            </LinearLayout>

        </com.google.android.material.card.MaterialCardView>

        <!-- Menu Items -->
        <com.google.android.material.card.MaterialCardView
            style="@style/CardView"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_marginHorizontal="@dimen/spacing_base"
            android:layout_marginBottom="@dimen/spacing_base">

            <LinearLayout
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="vertical"
                android:divider="@drawable/divider_horizontal"
                android:showDividers="middle">

                <!-- Booking History -->
                <LinearLayout
                    android:id="@+id/btnBookingHistory"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_vertical"
                    android:background="?attr/selectableItemBackground">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_ticket"
                        app:tint="@color/textSecondary"/>

                    <TextView
                        style="@style/Text.Body"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:text="Lịch sử đặt vé"/>

                    <ImageView
                        android:layout_width="@dimen/icon_size_sm"
                        android:layout_height="@dimen/icon_size_sm"
                        android:src="@drawable/ic_chevron_right"
                        app:tint="@color/textSecondary"/>

                </LinearLayout>

                <!-- Change Password -->
                <LinearLayout
                    android:id="@+id/btnChangePassword"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_vertical"
                    android:background="?attr/selectableItemBackground">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_lock"
                        app:tint="@color/textSecondary"/>

                    <TextView
                        style="@style/Text.Body"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:text="@string/profile_change_password"/>

                    <ImageView
                        android:layout_width="@dimen/icon_size_sm"
                        android:layout_height="@dimen/icon_size_sm"
                        android:src="@drawable/ic_chevron_right"
                        app:tint="@color/textSecondary"/>

                </LinearLayout>

                <!-- Notifications -->
                <LinearLayout
                    android:id="@+id/btnNotifications"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_vertical"
                    android:background="?attr/selectableItemBackground">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_notifications"
                        app:tint="@color/textSecondary"/>

                    <TextView
                        style="@style/Text.Body"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:text="@string/profile_notifications"/>

                    <com.google.android.material.switchmaterial.SwitchMaterial
                        android:id="@+id/switchNotifications"
                        android:layout_width="wrap_content"
                        android:layout_height="wrap_content"
                        android:checked="true"/>

                </LinearLayout>

                <!-- Settings -->
                <LinearLayout
                    android:id="@+id/btnSettings"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_vertical"
                    android:background="?attr/selectableItemBackground">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_settings"
                        app:tint="@color/textSecondary"/>

                    <TextView
                        style="@style/Text.Body"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:text="@string/profile_settings"/>

                    <ImageView
                        android:layout_width="@dimen/icon_size_sm"
                        android:layout_height="@dimen/icon_size_sm"
                        android:src="@drawable/ic_chevron_right"
                        app:tint="@color/textSecondary"/>

                </LinearLayout>

                <!-- Help -->
                <LinearLayout
                    android:id="@+id/btnHelp"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_vertical"
                    android:background="?attr/selectableItemBackground">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_help"
                        app:tint="@color/textSecondary"/>

                    <TextView
                        style="@style/Text.Body"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:text="@string/profile_help"/>

                    <ImageView
                        android:layout_width="@dimen/icon_size_sm"
                        android:layout_height="@dimen/icon_size_sm"
                        android:src="@drawable/ic_chevron_right"
                        app:tint="@color/textSecondary"/>

                </LinearLayout>

                <!-- About -->
                <LinearLayout
                    android:id="@+id/btnAbout"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="horizontal"
                    android:padding="@dimen/spacing_base"
                    android:gravity="center_vertical"
                    android:background="?attr/selectableItemBackground">

                    <ImageView
                        android:layout_width="@dimen/icon_size_md"
                        android:layout_height="@dimen/icon_size_md"
                        android:src="@drawable/ic_info"
                        app:tint="@color/textSecondary"/>

                    <TextView
                        style="@style/Text.Body"
                        android:layout_width="0dp"
                        android:layout_height="wrap_content"
                        android:layout_weight="1"
                        android:layout_marginStart="@dimen/spacing_base"
                        android:text="@string/profile_about"/>

                    <ImageView
                        android:layout_width="@dimen/icon_size_sm"
                        android:layout_height="@dimen/icon_size_sm"
                        android:src="@drawable/ic_chevron_right"
                        app:tint="@color/textSecondary"/>

                </LinearLayout>

            </LinearLayout>

        </com.google.android.material.card.MaterialCardView>

        <!-- Logout Button -->
        <com.google.android.material.button.MaterialButton
            android:id="@+id/btnLogout"
            style="@style/Button.Outlined"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:layout_marginHorizontal="@dimen/spacing_base"
            android:layout_marginBottom="@dimen/spacing_base"
            android:text="@string/profile_logout"
            android:textColor="@color/statusError"
            app:strokeColor="@color/statusError"
            app:icon="@drawable/ic_logout"
            app:iconTint="@color/statusError"/>

        <!-- App Version -->
        <TextView
            style="@style/Text.Caption"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:layout_gravity="center_horizontal"
            android:layout_marginBottom="@dimen/spacing_base"
            android:text="Version 1.0.0"/>

    </LinearLayout>

</ScrollView>
```

### Java Implementation: ProfileFragment.java

```java
package com.movie88.ui.profile;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProvider;

import com.bumptech.glide.Glide;
import com.google.android.material.button.MaterialButton;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.android.material.switchmaterial.SwitchMaterial;
import com.movie88.R;
import com.movie88.data.models.response.User;
import com.movie88.ui.auth.LoginActivity;
import com.movie88.utils.SharedPrefsManager;

public class ProfileFragment extends Fragment {

    // Views
    private ImageView ivAvatar, btnChangeAvatar, btnEditProfile;
    private TextView tvFullName, tvEmail, tvPhone;
    private View btnBookingHistory, btnChangePassword, btnNotifications, 
                 btnSettings, btnHelp, btnAbout;
    private SwitchMaterial switchNotifications;
    private MaterialButton btnLogout;

    // ViewModel
    private ProfileViewModel viewModel;

    private static final int REQUEST_EDIT_PROFILE = 100;
    private static final int REQUEST_CHANGE_AVATAR = 101;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, 
                            @Nullable ViewGroup container, 
                            @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.fragment_profile, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        initViews(view);
        setupViewModel();
        setupListeners();

        // Load user profile
        viewModel.loadUserProfile();
    }

    private void initViews(View view) {
        ivAvatar = view.findViewById(R.id.ivAvatar);
        btnChangeAvatar = view.findViewById(R.id.btnChangeAvatar);
        btnEditProfile = view.findViewById(R.id.btnEditProfile);
        tvFullName = view.findViewById(R.id.tvFullName);
        tvEmail = view.findViewById(R.id.tvEmail);
        tvPhone = view.findViewById(R.id.tvPhone);
        btnBookingHistory = view.findViewById(R.id.btnBookingHistory);
        btnChangePassword = view.findViewById(R.id.btnChangePassword);
        btnNotifications = view.findViewById(R.id.btnNotifications);
        switchNotifications = view.findViewById(R.id.switchNotifications);
        btnSettings = view.findViewById(R.id.btnSettings);
        btnHelp = view.findViewById(R.id.btnHelp);
        btnAbout = view.findViewById(R.id.btnAbout);
        btnLogout = view.findViewById(R.id.btnLogout);
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(ProfileViewModel.class);

        // Observe user data
        viewModel.getUserProfile().observe(getViewLifecycleOwner(), user -> {
            if (user != null) {
                displayUserProfile(user);
            }
        });
    }

    private void setupListeners() {
        // Edit profile
        btnEditProfile.setOnClickListener(v -> {
            Intent intent = new Intent(getActivity(), EditProfileActivity.class);
            startActivityForResult(intent, REQUEST_EDIT_PROFILE);
        });

        // Change avatar
        btnChangeAvatar.setOnClickListener(v -> {
            // Show image picker
            Intent intent = new Intent(Intent.ACTION_PICK);
            intent.setType("image/*");
            startActivityForResult(intent, REQUEST_CHANGE_AVATAR);
        });

        // Booking history
        btnBookingHistory.setOnClickListener(v -> {
            Intent intent = new Intent(getActivity(), BookingHistoryActivity.class);
            startActivity(intent);
        });

        // Change password
        btnChangePassword.setOnClickListener(v -> {
            Intent intent = new Intent(getActivity(), ChangePasswordActivity.class);
            startActivity(intent);
        });

        // Notifications
        switchNotifications.setOnCheckedChangeListener((buttonView, isChecked) -> {
            SharedPrefsManager.getInstance().setNotificationEnabled(isChecked);
        });

        // Load notification setting
        switchNotifications.setChecked(
            SharedPrefsManager.getInstance().isNotificationEnabled()
        );

        // Settings
        btnSettings.setOnClickListener(v -> {
            Intent intent = new Intent(getActivity(), SettingsActivity.class);
            startActivity(intent);
        });

        // Help
        btnHelp.setOnClickListener(v -> {
            // Navigate to help screen
        });

        // About
        btnAbout.setOnClickListener(v -> {
            // Navigate to about screen
        });

        // Logout
        btnLogout.setOnClickListener(v -> {
            showLogoutConfirmation();
        });
    }

    private void displayUserProfile(User user) {
        tvFullName.setText(user.getFullName());
        tvEmail.setText(user.getEmail());
        tvPhone.setText(user.getPhoneNumber());

        // Load avatar
        if (user.getAvatarUrl() != null && !user.getAvatarUrl().isEmpty()) {
            Glide.with(this)
                .load(user.getAvatarUrl())
                .placeholder(R.drawable.ic_avatar_placeholder)
                .circleCrop()
                .into(ivAvatar);
        }
    }

    private void showLogoutConfirmation() {
        new MaterialAlertDialogBuilder(requireContext())
            .setTitle(R.string.profile_logout)
            .setMessage(R.string.profile_logout_confirm)
            .setPositiveButton(R.string.action_yes, (dialog, which) -> {
                logout();
            })
            .setNegativeButton(R.string.action_no, null)
            .show();
    }

    private void logout() {
        viewModel.logout();
        
        // Navigate to login screen
        Intent intent = new Intent(getActivity(), LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        requireActivity().finish();
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        
        if (resultCode == Activity.RESULT_OK) {
            if (requestCode == REQUEST_EDIT_PROFILE) {
                // Reload user profile
                viewModel.loadUserProfile();
            } else if (requestCode == REQUEST_CHANGE_AVATAR && data != null) {
                // Upload new avatar
                viewModel.uploadAvatar(data.getData());
            }
        }
    }
}
```

---

## 2️⃣ EditProfileActivity

### Purpose
Chỉnh sửa thông tin cá nhân: tên, email, phone, birthday, gender.

### Layout: activity_edit_profile.xml

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
        app:title="Chỉnh sửa thông tin"
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

            <!-- Full Name -->
            <com.google.android.material.textfield.TextInputLayout
                style="@style/TextInputLayout"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base"
                android:hint="@string/profile_full_name">

                <com.google.android.material.textfield.TextInputEditText
                    android:id="@+id/etFullName"
                    style="@style/TextInputEditText"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:inputType="textPersonName"/>

            </com.google.android.material.textfield.TextInputLayout>

            <!-- Email (read-only) -->
            <com.google.android.material.textfield.TextInputLayout
                style="@style/TextInputLayout"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base"
                android:hint="@string/profile_email">

                <com.google.android.material.textfield.TextInputEditText
                    android:id="@+id/etEmail"
                    style="@style/TextInputEditText"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:inputType="textEmailAddress"
                    android:enabled="false"/>

            </com.google.android.material.textfield.TextInputLayout>

            <!-- Phone Number -->
            <com.google.android.material.textfield.TextInputLayout
                style="@style/TextInputLayout"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base"
                android:hint="@string/profile_phone">

                <com.google.android.material.textfield.TextInputEditText
                    android:id="@+id/etPhone"
                    style="@style/TextInputEditText"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:inputType="phone"
                    android:maxLength="10"/>

            </com.google.android.material.textfield.TextInputLayout>

            <!-- Birthday -->
            <com.google.android.material.textfield.TextInputLayout
                style="@style/TextInputLayout"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_base"
                android:hint="@string/profile_birthday">

                <com.google.android.material.textfield.TextInputEditText
                    android:id="@+id/etBirthday"
                    style="@style/TextInputEditText"
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:inputType="none"
                    android:focusable="false"
                    android:clickable="true"/>

            </com.google.android.material.textfield.TextInputLayout>

            <!-- Gender -->
            <TextView
                style="@style/Text.Body"
                android:layout_width="wrap_content"
                android:layout_height="wrap_content"
                android:layout_marginBottom="@dimen/spacing_sm"
                android:text="@string/profile_gender"/>

            <RadioGroup
                android:id="@+id/rgGender"
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:orientation="horizontal">

                <com.google.android.material.radiobutton.MaterialRadioButton
                    android:id="@+id/rbMale"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="@string/profile_male"/>

                <com.google.android.material.radiobutton.MaterialRadioButton
                    android:id="@+id/rbFemale"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="@string/profile_female"/>

                <com.google.android.material.radiobutton.MaterialRadioButton
                    android:id="@+id/rbOther"
                    android:layout_width="0dp"
                    android:layout_height="wrap_content"
                    android:layout_weight="1"
                    android:text="@string/profile_other"/>

            </RadioGroup>

        </LinearLayout>

    </ScrollView>

    <!-- Save Button -->
    <com.google.android.material.button.MaterialButton
        android:id="@+id/btnSave"
        style="@style/Button.Primary"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_margin="@dimen/spacing_base"
        android:text="@string/action_save"/>

</LinearLayout>
```

### Java Implementation (simplified)

```java
package com.movie88.ui.profile;

import android.app.DatePickerDialog;
import android.os.Bundle;
import android.widget.EditText;
import android.widget.RadioGroup;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.lifecycle.ViewModelProvider;

import com.google.android.material.button.MaterialButton;
import com.movie88.R;
import com.movie88.data.models.request.UpdateProfileRequest;
import com.movie88.utils.DateUtils;
import com.movie88.utils.ValidationUtils;

import java.util.Calendar;
import java.util.Date;

public class EditProfileActivity extends AppCompatActivity {

    private EditText etFullName, etEmail, etPhone, etBirthday;
    private RadioGroup rgGender;
    private MaterialButton btnSave;
    
    private ProfileViewModel viewModel;
    private Date selectedBirthday;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit_profile);

        initViews();
        setupToolbar();
        setupViewModel();
        setupListeners();
    }

    private void initViews() {
        etFullName = findViewById(R.id.etFullName);
        etEmail = findViewById(R.id.etEmail);
        etPhone = findViewById(R.id.etPhone);
        etBirthday = findViewById(R.id.etBirthday);
        rgGender = findViewById(R.id.rgGender);
        btnSave = findViewById(R.id.btnSave);
    }

    private void setupToolbar() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void setupViewModel() {
        viewModel = new ViewModelProvider(this).get(ProfileViewModel.class);

        // Load current user data
        viewModel.getUserProfile().observe(this, user -> {
            if (user != null) {
                etFullName.setText(user.getFullName());
                etEmail.setText(user.getEmail());
                etPhone.setText(user.getPhoneNumber());
                // Set birthday and gender...
            }
        });

        // Observe update result
        viewModel.getUpdateSuccess().observe(this, success -> {
            if (success) {
                Toast.makeText(this, R.string.profile_update_success, 
                    Toast.LENGTH_SHORT).show();
                setResult(RESULT_OK);
                finish();
            }
        });
    }

    private void setupListeners() {
        // Birthday picker
        etBirthday.setOnClickListener(v -> showDatePicker());

        // Save button
        btnSave.setOnClickListener(v -> saveProfile());
    }

    private void showDatePicker() {
        Calendar calendar = Calendar.getInstance();
        new DatePickerDialog(this, 
            (view, year, month, dayOfMonth) -> {
                calendar.set(year, month, dayOfMonth);
                selectedBirthday = calendar.getTime();
                etBirthday.setText(DateUtils.formatDate(selectedBirthday));
            },
            calendar.get(Calendar.YEAR) - 20,
            calendar.get(Calendar.MONTH),
            calendar.get(Calendar.DAY_OF_MONTH)
        ).show();
    }

    private void saveProfile() {
        String fullName = etFullName.getText().toString().trim();
        String phone = etPhone.getText().toString().trim();

        // Validation
        if (!ValidationUtils.isValidName(fullName)) {
            etFullName.setError("Tên không hợp lệ");
            return;
        }

        if (!ValidationUtils.isValidPhone(phone)) {
            etPhone.setError(ValidationUtils.getPhoneError(phone));
            return;
        }

        // Get selected gender
        int selectedGenderId = rgGender.getCheckedRadioButtonId();
        String gender = "Other";
        if (selectedGenderId == R.id.rbMale) gender = "Male";
        else if (selectedGenderId == R.id.rbFemale) gender = "Female";

        // Create request
        UpdateProfileRequest request = new UpdateProfileRequest();
        request.setFullName(fullName);
        request.setPhoneNumber(phone);
        request.setDateOfBirth(selectedBirthday);
        request.setGender(gender);

        // Update profile
        viewModel.updateProfile(request);
    }
}
```

---

## 3️⃣ BookingHistoryFragment

(Similar to existing BookingsActivity, displayed in ProfileFragment navigation)

---

## API Endpoints Used

**ProfileFragment:**
- `GET /api/users/me` - Lấy thông tin user hiện tại (requires auth)
- `GET /api/customers/profile` - Lấy profile chi tiết khách hàng
- `POST /api/auth/logout` - Đăng xuất và clear tokens

**EditProfileActivity:**
- `GET /api/users/me` - Lấy thông tin user để pre-fill form
- `PUT /api/users/{id}` - Cập nhật thông tin user (fullName, phoneNumber, dateOfBirth, gender)
- Alternatively: `PUT /api/customers/profile` - Cập nhật profile khách hàng

**Change Avatar:**
- `POST /api/users/avatar` - Upload ảnh avatar mới (multipart/form-data)
- Request body: `file` (image file)

**Booking History:**
- `GET /api/bookings/my-bookings` - Lấy lịch sử đặt vé của user
- `GET /api/customers/booking-history` - Alternative endpoint cho booking history

**Change Password:**
- `POST /api/auth/change-password` - Đổi mật khẩu (requires oldPassword + newPassword)

---

### API Interface Example
```java
public interface ApiService {
    
    // Get current user
    @GET("api/users/me")
    Call<ApiResponse<User>> getCurrentUser(
        @Header("Authorization") String token
    );
    
    // Get customer profile
    @GET("api/customers/profile")
    Call<ApiResponse<Customer>> getCustomerProfile(
        @Header("Authorization") String token
    );
    
    // Update user info
    @PUT("api/users/{id}")
    Call<ApiResponse<User>> updateUser(
        @Header("Authorization") String token,
        @Path("id") int userId,
        @Body UpdateUserRequest request
    );
    
    // Update customer profile
    @PUT("api/customers/profile")
    Call<ApiResponse<Customer>> updateCustomerProfile(
        @Header("Authorization") String token,
        @Body UpdateProfileRequest request
    );
    
    // Upload avatar
    @Multipart
    @POST("api/users/avatar")
    Call<ApiResponse<UploadAvatarResponse>> uploadAvatar(
        @Header("Authorization") String token,
        @Part MultipartBody.Part file
    );
    
    // Get my bookings
    @GET("api/bookings/my-bookings")
    Call<ApiResponse<List<Booking>>> getMyBookings(
        @Header("Authorization") String token,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    // Get booking history
    @GET("api/customers/booking-history")
    Call<ApiResponse<PagedResult<Booking>>> getBookingHistory(
        @Header("Authorization") String token,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    // Change password
    @POST("api/auth/change-password")
    Call<ApiResponse<Void>> changePassword(
        @Header("Authorization") String token,
        @Body ChangePasswordRequest request  // { oldPassword, newPassword }
    );
    
    // Logout
    @POST("api/auth/logout")
    Call<ApiResponse<Void>> logout(
        @Header("Authorization") String token
    );
}
```

---

**Last Updated**: October 29, 2025
