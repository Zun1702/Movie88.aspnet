# ⚙️ Settings Screen Documentation

## Tổng quan

Screen Settings cho phép user cấu hình app: Notifications, Theme, Language, Cache management. (optional)

---

## 1️⃣ Settings Activity

### Purpose
Main settings screen với các tùy chọn cấu hình app.

### Layout: `activity_settings.xml`

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
            app:title="@string/settings"
            app:navigationIcon="@drawable/ic_back"
            app:titleTextAppearance="@style/TextAppearance.App.Heading3" />

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

            <!-- Notifications Section -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small"
                app:strokeWidth="0dp">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <!-- Section Title -->
                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/notifications"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:paddingBottom="@dimen/spacing_small" />

                    <!-- Enable Notifications -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_notifications"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/enable_notifications"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <com.google.android.material.switchmaterial.SwitchMaterial
                            android:id="@+id/switchNotifications"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:checked="true" />

                    </LinearLayout>

                    <!-- Push Notifications -->
                    <LinearLayout
                        android:id="@+id/layoutPushNotifications"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_bell"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/push_notifications"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <com.google.android.material.switchmaterial.SwitchMaterial
                            android:id="@+id/switchPushNotifications"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:checked="true" />

                    </LinearLayout>

                    <!-- Email Notifications -->
                    <LinearLayout
                        android:id="@+id/layoutEmailNotifications"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_email"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/email_notifications"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <com.google.android.material.switchmaterial.SwitchMaterial
                            android:id="@+id/switchEmailNotifications"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:checked="false" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Appearance Section -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small"
                app:strokeWidth="0dp">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <!-- Section Title -->
                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/appearance"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:paddingBottom="@dimen/spacing_small" />

                    <!-- Theme -->
                    <LinearLayout
                        android:id="@+id/layoutTheme"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_theme"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <LinearLayout
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:orientation="vertical">

                            <TextView
                                android:layout_width="wrap_content"
                                android:layout_height="wrap_content"
                                android:text="@string/theme"
                                android:textAppearance="@style/TextAppearance.App.Body"
                                android:textColor="@color/textPrimary" />

                            <TextView
                                android:id="@+id/tvCurrentTheme"
                                android:layout_width="wrap_content"
                                android:layout_height="wrap_content"
                                android:text="@string/theme_light"
                                android:textAppearance="@style/TextAppearance.App.Caption"
                                android:textColor="@color/textSecondary" />

                        </LinearLayout>

                        <ImageView
                            android:layout_width="@dimen/icon_size_sm"
                            android:layout_height="@dimen/icon_size_sm"
                            android:src="@drawable/ic_chevron_right"
                            app:tint="@color/iconSecondary" />

                    </LinearLayout>

                    <!-- Language -->
                    <LinearLayout
                        android:id="@+id/layoutLanguage"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_language"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <LinearLayout
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:orientation="vertical">

                            <TextView
                                android:layout_width="wrap_content"
                                android:layout_height="wrap_content"
                                android:text="@string/language"
                                android:textAppearance="@style/TextAppearance.App.Body"
                                android:textColor="@color/textPrimary" />

                            <TextView
                                android:id="@+id/tvCurrentLanguage"
                                android:layout_width="wrap_content"
                                android:layout_height="wrap_content"
                                android:text="@string/language_vi"
                                android:textAppearance="@style/TextAppearance.App.Caption"
                                android:textColor="@color/textSecondary" />

                        </LinearLayout>

                        <ImageView
                            android:layout_width="@dimen/icon_size_sm"
                            android:layout_height="@dimen/icon_size_sm"
                            android:src="@drawable/ic_chevron_right"
                            app:tint="@color/iconSecondary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- Data & Storage Section -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small"
                app:strokeWidth="0dp">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <!-- Section Title -->
                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/data_storage"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:paddingBottom="@dimen/spacing_small" />

                    <!-- Clear Cache -->
                    <LinearLayout
                        android:id="@+id/layoutClearCache"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_delete"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <LinearLayout
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:orientation="vertical">

                            <TextView
                                android:layout_width="wrap_content"
                                android:layout_height="wrap_content"
                                android:text="@string/clear_cache"
                                android:textAppearance="@style/TextAppearance.App.Body"
                                android:textColor="@color/textPrimary" />

                            <TextView
                                android:id="@+id/tvCacheSize"
                                android:layout_width="wrap_content"
                                android:layout_height="wrap_content"
                                android:text="125 MB"
                                android:textAppearance="@style/TextAppearance.App.Caption"
                                android:textColor="@color/textSecondary" />

                        </LinearLayout>

                        <ImageView
                            android:layout_width="@dimen/icon_size_sm"
                            android:layout_height="@dimen/icon_size_sm"
                            android:src="@drawable/ic_chevron_right"
                            app:tint="@color/iconSecondary" />

                    </LinearLayout>

                    <!-- Offline Mode -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_offline"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/offline_mode"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <com.google.android.material.switchmaterial.SwitchMaterial
                            android:id="@+id/switchOfflineMode"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:checked="true" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

            <!-- About Section -->
            <com.google.android.material.card.MaterialCardView
                android:layout_width="match_parent"
                android:layout_height="wrap_content"
                android:layout_marginHorizontal="@dimen/spacing_medium"
                android:layout_marginBottom="@dimen/spacing_medium"
                app:cardCornerRadius="@dimen/card_corner_radius_medium"
                app:cardElevation="@dimen/elevation_small"
                app:strokeWidth="0dp">

                <LinearLayout
                    android:layout_width="match_parent"
                    android:layout_height="wrap_content"
                    android:orientation="vertical"
                    android:padding="@dimen/spacing_medium">

                    <!-- Section Title -->
                    <TextView
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:text="@string/about"
                        android:textAppearance="@style/TextAppearance.App.Heading3"
                        android:textColor="@color/textPrimary"
                        android:paddingBottom="@dimen/spacing_small" />

                    <!-- Terms of Service -->
                    <LinearLayout
                        android:id="@+id/layoutTerms"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_document"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/terms_of_service"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <ImageView
                            android:layout_width="@dimen/icon_size_sm"
                            android:layout_height="@dimen/icon_size_sm"
                            android:src="@drawable/ic_chevron_right"
                            app:tint="@color/iconSecondary" />

                    </LinearLayout>

                    <!-- Privacy Policy -->
                    <LinearLayout
                        android:id="@+id/layoutPrivacy"
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small"
                        android:background="?attr/selectableItemBackground"
                        android:clickable="true"
                        android:focusable="true">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_privacy"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/privacy_policy"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <ImageView
                            android:layout_width="@dimen/icon_size_sm"
                            android:layout_height="@dimen/icon_size_sm"
                            android:src="@drawable/ic_chevron_right"
                            app:tint="@color/iconSecondary" />

                    </LinearLayout>

                    <!-- App Version -->
                    <LinearLayout
                        android:layout_width="match_parent"
                        android:layout_height="wrap_content"
                        android:orientation="horizontal"
                        android:gravity="center_vertical"
                        android:paddingVertical="@dimen/spacing_small">

                        <ImageView
                            android:layout_width="@dimen/icon_size_md"
                            android:layout_height="@dimen/icon_size_md"
                            android:src="@drawable/ic_info"
                            app:tint="@color/iconPrimary"
                            android:layout_marginEnd="@dimen/spacing_medium" />

                        <TextView
                            android:layout_width="0dp"
                            android:layout_height="wrap_content"
                            android:layout_weight="1"
                            android:text="@string/app_version"
                            android:textAppearance="@style/TextAppearance.App.Body"
                            android:textColor="@color/textPrimary" />

                        <TextView
                            android:id="@+id/tvAppVersion"
                            android:layout_width="wrap_content"
                            android:layout_height="wrap_content"
                            android:text="1.0.0"
                            android:textAppearance="@style/TextAppearance.App.Caption"
                            android:textColor="@color/textSecondary" />

                    </LinearLayout>

                </LinearLayout>

            </com.google.android.material.card.MaterialCardView>

        </LinearLayout>

    </androidx.core.widget.NestedScrollView>

</androidx.coordinatorlayout.widget.CoordinatorLayout>
```

### Java: `SettingsActivity.java`

```java
package com.movie88.ui.settings;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AppCompatDelegate;

import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.android.material.switchmaterial.SwitchMaterial;
import com.movie88.BuildConfig;
import com.movie88.R;
import com.movie88.utils.SharedPrefsManager;

import java.io.File;

public class SettingsActivity extends AppCompatActivity {

    private MaterialToolbar toolbar;
    
    // Notification switches
    private SwitchMaterial switchNotifications;
    private SwitchMaterial switchPushNotifications;
    private SwitchMaterial switchEmailNotifications;
    private LinearLayout layoutPushNotifications;
    private LinearLayout layoutEmailNotifications;
    
    // Appearance
    private LinearLayout layoutTheme;
    private LinearLayout layoutLanguage;
    private TextView tvCurrentTheme;
    private TextView tvCurrentLanguage;
    
    // Data & Storage
    private LinearLayout layoutClearCache;
    private SwitchMaterial switchOfflineMode;
    private TextView tvCacheSize;
    
    // About
    private LinearLayout layoutTerms;
    private LinearLayout layoutPrivacy;
    private TextView tvAppVersion;
    
    private SharedPrefsManager prefsManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);
        
        prefsManager = SharedPrefsManager.getInstance(this);
        
        initViews();
        setupToolbar();
        loadSettings();
        setupListeners();
    }

    private void initViews() {
        toolbar = findViewById(R.id.toolbar);
        
        // Notifications
        switchNotifications = findViewById(R.id.switchNotifications);
        switchPushNotifications = findViewById(R.id.switchPushNotifications);
        switchEmailNotifications = findViewById(R.id.switchEmailNotifications);
        layoutPushNotifications = findViewById(R.id.layoutPushNotifications);
        layoutEmailNotifications = findViewById(R.id.layoutEmailNotifications);
        
        // Appearance
        layoutTheme = findViewById(R.id.layoutTheme);
        layoutLanguage = findViewById(R.id.layoutLanguage);
        tvCurrentTheme = findViewById(R.id.tvCurrentTheme);
        tvCurrentLanguage = findViewById(R.id.tvCurrentLanguage);
        
        // Data & Storage
        layoutClearCache = findViewById(R.id.layoutClearCache);
        switchOfflineMode = findViewById(R.id.switchOfflineMode);
        tvCacheSize = findViewById(R.id.tvCacheSize);
        
        // About
        layoutTerms = findViewById(R.id.layoutTerms);
        layoutPrivacy = findViewById(R.id.layoutPrivacy);
        tvAppVersion = findViewById(R.id.tvAppVersion);
    }

    private void setupToolbar() {
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        }
        toolbar.setNavigationOnClickListener(v -> onBackPressed());
    }

    private void loadSettings() {
        // Load notification settings
        boolean notificationsEnabled = prefsManager.isNotificationEnabled();
        switchNotifications.setChecked(notificationsEnabled);
        switchPushNotifications.setChecked(prefsManager.isPushNotificationEnabled());
        switchEmailNotifications.setChecked(prefsManager.isEmailNotificationEnabled());
        
        // Enable/disable sub-options based on main switch
        layoutPushNotifications.setEnabled(notificationsEnabled);
        layoutEmailNotifications.setEnabled(notificationsEnabled);
        switchPushNotifications.setEnabled(notificationsEnabled);
        switchEmailNotifications.setEnabled(notificationsEnabled);
        
        // Load theme
        String theme = prefsManager.getTheme();
        updateThemeDisplay(theme);
        
        // Load language
        String language = prefsManager.getLanguage();
        updateLanguageDisplay(language);
        
        // Load offline mode
        switchOfflineMode.setChecked(prefsManager.isOfflineModeEnabled());
        
        // Calculate cache size
        calculateCacheSize();
        
        // Set app version
        tvAppVersion.setText(BuildConfig.VERSION_NAME);
    }

    private void setupListeners() {
        // Notifications
        switchNotifications.setOnCheckedChangeListener((buttonView, isChecked) -> {
            prefsManager.setNotificationEnabled(isChecked);
            
            // Enable/disable sub-options
            layoutPushNotifications.setEnabled(isChecked);
            layoutEmailNotifications.setEnabled(isChecked);
            switchPushNotifications.setEnabled(isChecked);
            switchEmailNotifications.setEnabled(isChecked);
            
            if (!isChecked) {
                switchPushNotifications.setChecked(false);
                switchEmailNotifications.setChecked(false);
            }
        });
        
        switchPushNotifications.setOnCheckedChangeListener((buttonView, isChecked) -> {
            prefsManager.setPushNotificationEnabled(isChecked);
        });
        
        switchEmailNotifications.setOnCheckedChangeListener((buttonView, isChecked) -> {
            prefsManager.setEmailNotificationEnabled(isChecked);
        });
        
        // Theme
        layoutTheme.setOnClickListener(v -> showThemeDialog());
        
        // Language
        layoutLanguage.setOnClickListener(v -> showLanguageDialog());
        
        // Clear cache
        layoutClearCache.setOnClickListener(v -> showClearCacheDialog());
        
        // Offline mode
        switchOfflineMode.setOnCheckedChangeListener((buttonView, isChecked) -> {
            prefsManager.setOfflineModeEnabled(isChecked);
        });
        
        // Terms & Privacy
        layoutTerms.setOnClickListener(v -> openTermsOfService());
        layoutPrivacy.setOnClickListener(v -> openPrivacyPolicy());
    }

    private void showThemeDialog() {
        String currentTheme = prefsManager.getTheme();
        int selectedIndex = 0;
        
        if ("light".equals(currentTheme)) {
            selectedIndex = 0;
        } else if ("dark".equals(currentTheme)) {
            selectedIndex = 1;
        } else {
            selectedIndex = 2; // System default
        }
        
        String[] themes = {
            getString(R.string.theme_light),
            getString(R.string.theme_dark),
            getString(R.string.theme_system)
        };
        
        new MaterialAlertDialogBuilder(this)
            .setTitle(R.string.select_theme)
            .setSingleChoiceItems(themes, selectedIndex, (dialog, which) -> {
                String theme;
                switch (which) {
                    case 0:
                        theme = "light";
                        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
                        break;
                    case 1:
                        theme = "dark";
                        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES);
                        break;
                    default:
                        theme = "system";
                        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_FOLLOW_SYSTEM);
                        break;
                }
                prefsManager.setTheme(theme);
                updateThemeDisplay(theme);
                dialog.dismiss();
            })
            .setNegativeButton(R.string.cancel, null)
            .show();
    }

    private void updateThemeDisplay(String theme) {
        int themeResId;
        switch (theme) {
            case "light":
                themeResId = R.string.theme_light;
                break;
            case "dark":
                themeResId = R.string.theme_dark;
                break;
            default:
                themeResId = R.string.theme_system;
                break;
        }
        tvCurrentTheme.setText(themeResId);
    }

    private void showLanguageDialog() {
        String currentLanguage = prefsManager.getLanguage();
        int selectedIndex = "vi".equals(currentLanguage) ? 0 : 1;
        
        String[] languages = {
            getString(R.string.language_vi),
            getString(R.string.language_en)
        };
        
        new MaterialAlertDialogBuilder(this)
            .setTitle(R.string.select_language)
            .setSingleChoiceItems(languages, selectedIndex, (dialog, which) -> {
                String language = which == 0 ? "vi" : "en";
                prefsManager.setLanguage(language);
                updateLanguageDisplay(language);
                
                // Show restart dialog
                showRestartDialog();
                
                dialog.dismiss();
            })
            .setNegativeButton(R.string.cancel, null)
            .show();
    }

    private void updateLanguageDisplay(String language) {
        int languageResId = "vi".equals(language) ? R.string.language_vi : R.string.language_en;
        tvCurrentLanguage.setText(languageResId);
    }

    private void showRestartDialog() {
        new MaterialAlertDialogBuilder(this)
            .setTitle(R.string.restart_required)
            .setMessage(R.string.restart_required_message)
            .setPositiveButton(R.string.restart_now, (dialog, which) -> {
                // Restart app
                Intent intent = getPackageManager().getLaunchIntentForPackage(getPackageName());
                if (intent != null) {
                    intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK);
                    startActivity(intent);
                    finish();
                    System.exit(0);
                }
            })
            .setNegativeButton(R.string.later, null)
            .show();
    }

    private void showClearCacheDialog() {
        new MaterialAlertDialogBuilder(this)
            .setTitle(R.string.clear_cache)
            .setMessage(R.string.clear_cache_message)
            .setPositiveButton(R.string.clear, (dialog, which) -> {
                clearCache();
            })
            .setNegativeButton(R.string.cancel, null)
            .show();
    }

    private void clearCache() {
        try {
            File cacheDir = getCacheDir();
            if (cacheDir != null && cacheDir.isDirectory()) {
                deleteDir(cacheDir);
            }
            
            // Recalculate cache size
            calculateCacheSize();
            
            // Show success message
            new MaterialAlertDialogBuilder(this)
                .setMessage(R.string.cache_cleared_success)
                .setPositiveButton(R.string.ok, null)
                .show();
                
        } catch (Exception e) {
            e.printStackTrace();
            // Show error message
            new MaterialAlertDialogBuilder(this)
                .setMessage(R.string.cache_cleared_error)
                .setPositiveButton(R.string.ok, null)
                .show();
        }
    }

    private boolean deleteDir(File dir) {
        if (dir != null && dir.isDirectory()) {
            String[] children = dir.list();
            if (children != null) {
                for (String child : children) {
                    boolean success = deleteDir(new File(dir, child));
                    if (!success) {
                        return false;
                    }
                }
            }
            return dir.delete();
        } else if (dir != null && dir.isFile()) {
            return dir.delete();
        }
        return false;
    }

    private void calculateCacheSize() {
        long size = 0;
        File cacheDir = getCacheDir();
        if (cacheDir != null) {
            size = getDirSize(cacheDir);
        }
        
        String sizeStr = formatFileSize(size);
        tvCacheSize.setText(sizeStr);
    }

    private long getDirSize(File dir) {
        long size = 0;
        if (dir.isDirectory()) {
            File[] files = dir.listFiles();
            if (files != null) {
                for (File file : files) {
                    if (file.isDirectory()) {
                        size += getDirSize(file);
                    } else {
                        size += file.length();
                    }
                }
            }
        } else {
            size = dir.length();
        }
        return size;
    }

    private String formatFileSize(long size) {
        if (size < 1024) {
            return size + " B";
        } else if (size < 1024 * 1024) {
            return String.format("%.2f KB", size / 1024.0);
        } else if (size < 1024 * 1024 * 1024) {
            return String.format("%.2f MB", size / (1024.0 * 1024.0));
        } else {
            return String.format("%.2f GB", size / (1024.0 * 1024.0 * 1024.0));
        }
    }

    private void openTermsOfService() {
        String url = "https://movie88.com/terms";
        Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
        startActivity(intent);
    }

    private void openPrivacyPolicy() {
        String url = "https://movie88.com/privacy";
        Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
        startActivity(intent);
    }
}
```

### Additional SharedPrefs Methods

```java
// Add to SharedPrefsManager.java

public boolean isPushNotificationEnabled() {
    return sharedPreferences.getBoolean("push_notifications_enabled", true);
}

public void setPushNotificationEnabled(boolean enabled) {
    sharedPreferences.edit().putBoolean("push_notifications_enabled", enabled).apply();
}

public boolean isEmailNotificationEnabled() {
    return sharedPreferences.getBoolean("email_notifications_enabled", false);
}

public void setEmailNotificationEnabled(boolean enabled) {
    sharedPreferences.edit().putBoolean("email_notifications_enabled", enabled).apply();
}

public String getTheme() {
    return sharedPreferences.getString("theme", "system");
}

public void setTheme(String theme) {
    sharedPreferences.edit().putString("theme", theme).apply();
}

public String getLanguage() {
    return sharedPreferences.getString("language", "vi");
}

public void setLanguage(String language) {
    sharedPreferences.edit().putString("language", language).apply();
}

public boolean isOfflineModeEnabled() {
    return sharedPreferences.getBoolean("offline_mode_enabled", true);
}

public void setOfflineModeEnabled(boolean enabled) {
    sharedPreferences.edit().putBoolean("offline_mode_enabled", enabled).apply();
}
```

### String Resources

```xml
<!-- Settings -->
<string name="settings">Cài đặt</string>
<string name="notifications">Thông báo</string>
<string name="enable_notifications">Bật thông báo</string>
<string name="push_notifications">Thông báo đẩy</string>
<string name="email_notifications">Thông báo email</string>
<string name="appearance">Giao diện</string>
<string name="theme">Chủ đề</string>
<string name="theme_light">Sáng</string>
<string name="theme_dark">Tối</string>
<string name="theme_system">Theo hệ thống</string>
<string name="select_theme">Chọn chủ đề</string>
<string name="language">Ngôn ngữ</string>
<string name="language_vi">Tiếng Việt</string>
<string name="language_en">English</string>
<string name="select_language">Chọn ngôn ngữ</string>
<string name="restart_required">Yêu cầu khởi động lại</string>
<string name="restart_required_message">Ứng dụng cần khởi động lại để thay đổi ngôn ngữ có hiệu lực.</string>
<string name="restart_now">Khởi động lại</string>
<string name="later">Để sau</string>
<string name="data_storage">Dữ liệu &amp; Lưu trữ</string>
<string name="clear_cache">Xóa bộ nhớ đệm</string>
<string name="clear_cache_message">Bạn có chắc muốn xóa toàn bộ bộ nhớ đệm?</string>
<string name="cache_cleared_success">Đã xóa bộ nhớ đệm thành công</string>
<string name="cache_cleared_error">Không thể xóa bộ nhớ đệm</string>
<string name="offline_mode">Chế độ ngoại tuyến</string>
<string name="about">Giới thiệu</string>
<string name="terms_of_service">Điều khoản dịch vụ</string>
<string name="privacy_policy">Chính sách bảo mật</string>
<string name="app_version">Phiên bản ứng dụng</string>
```

---

**Last Updated**: October 29, 2025
