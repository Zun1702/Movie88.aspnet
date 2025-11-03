# 🎨 Colors & Themes

## Tổng quan

Design system của ứng dụng bao gồm color palette, themes, styles cho Material Design.

---

## 📂 Files Structure

```
app/src/main/res/
│
├── values/
│   ├── colors.xml              # Color palette
│   ├── themes.xml              # Light theme
│   ├── dimens.xml              # Spacing, sizes
│   └── strings.xml             # All text strings
│
└── values-night/
    └── themes.xml              # Dark theme
```

---

## 1️⃣ colors.xml

### Purpose
Định nghĩa toàn bộ color palette của ứng dụng.

### Implementation

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    
    <!-- ============================================ -->
    <!-- PRIMARY COLORS (Netflix-inspired Red)       -->
    <!-- ============================================ -->
    <color name="colorPrimary">#E50914</color>           <!-- Netflix Red -->
    <color name="colorPrimaryDark">#B20710</color>       <!-- Darker Red -->
    <color name="colorPrimaryLight">#FF2E38</color>      <!-- Lighter Red -->
    
    <color name="colorAccent">#FFB800</color>            <!-- Gold/Yellow for highlights -->
    
    <!-- ============================================ -->
    <!-- BACKGROUND COLORS (Dark theme inspired)     -->
    <!-- ============================================ -->
    <color name="backgroundColor">#141414</color>         <!-- Main background (very dark gray) -->
    <color name="backgroundCard">#1F1F1F</color>          <!-- Card background (lighter dark) -->
    <color name="backgroundDialog">#2B2B2B</color>        <!-- Dialog background -->
    <color name="backgroundInput">#2F2F2F</color>         <!-- Input field background -->
    
    <!-- ============================================ -->
    <!-- TEXT COLORS                                  -->
    <!-- ============================================ -->
    <color name="textPrimary">#FFFFFF</color>             <!-- Main text (white) -->
    <color name="textSecondary">#B3B3B3</color>           <!-- Secondary text (gray) -->
    <color name="textHint">#808080</color>                <!-- Hint text (darker gray) -->
    <color name="textDisabled">#606060</color>            <!-- Disabled text -->
    
    <!-- ============================================ -->
    <!-- SEAT COLORS                                  -->
    <!-- ============================================ -->
    <color name="seatAvailable">#4A4A4A</color>           <!-- Gray -->
    <color name="seatSelected">#E50914</color>            <!-- Red (Primary) -->
    <color name="seatOccupied">#2F2F2F</color>            <!-- Dark gray -->
    <color name="seatVIP">#FFB800</color>                 <!-- Gold (Accent) -->
    <color name="seatCouple">#FF6B9D</color>              <!-- Pink -->
    
    <!-- ============================================ -->
    <!-- STATUS COLORS                                -->
    <!-- ============================================ -->
    <color name="statusSuccess">#4CAF50</color>           <!-- Green -->
    <color name="statusError">#F44336</color>             <!-- Red -->
    <color name="statusWarning">#FF9800</color>           <!-- Orange -->
    <color name="statusInfo">#2196F3</color>              <!-- Blue -->
    
    <!-- ============================================ -->
    <!-- BUTTON COLORS                                -->
    <!-- ============================================ -->
    <color name="buttonPrimary">#E50914</color>           <!-- Red -->
    <color name="buttonSecondary">#4A4A4A</color>         <!-- Gray -->
    <color name="buttonDisabled">#2F2F2F</color>          <!-- Dark gray -->
    <color name="buttonTextPrimary">#FFFFFF</color>       <!-- White -->
    <color name="buttonTextSecondary">#B3B3B3</color>     <!-- Gray -->
    
    <!-- ============================================ -->
    <!-- DIVIDER & BORDER COLORS                      -->
    <!-- ============================================ -->
    <color name="divider">#333333</color>                 <!-- Divider line -->
    <color name="border">#4A4A4A</color>                  <!-- Border -->
    <color name="borderFocused">#E50914</color>           <!-- Focused border (Red) -->
    
    <!-- ============================================ -->
    <!-- RATING COLORS                                -->
    <!-- ============================================ -->
    <color name="ratingGood">#4CAF50</color>              <!-- Green (> 7.0) -->
    <color name="ratingOk">#FF9800</color>                <!-- Orange (5.0 - 7.0) -->
    <color name="ratingBad">#F44336</color>               <!-- Red (< 5.0) -->
    <color name="ratingStar">#FFB800</color>              <!-- Gold star -->
    
    <!-- ============================================ -->
    <!-- AGE RATING COLORS                            -->
    <!-- ============================================ -->
    <color name="ageRatingP">#4CAF50</color>              <!-- Green (P - All ages) -->
    <color name="ageRatingC13">#FF9800</color>            <!-- Orange (C13) -->
    <color name="ageRatingC16">#F44336</color>            <!-- Red (C16) -->
    <color name="ageRatingC18">#B71C1C</color>            <!-- Dark Red (C18) -->
    
    <!-- ============================================ -->
    <!-- PAYMENT STATUS COLORS                        -->
    <!-- ============================================ -->
    <color name="paymentPending">#FF9800</color>          <!-- Orange -->
    <color name="paymentCompleted">#4CAF50</color>        <!-- Green -->
    <color name="paymentFailed">#F44336</color>           <!-- Red -->
    <color name="paymentCancelled">#757575</color>        <!-- Gray -->
    
    <!-- ============================================ -->
    <!-- TRANSPARENT COLORS                           -->
    <!-- ============================================ -->
    <color name="transparent">#00000000</color>
    <color name="blackOverlay">#80000000</color>          <!-- 50% black -->
    <color name="blackOverlayDark">#CC000000</color>      <!-- 80% black -->
    
    <!-- ============================================ -->
    <!-- SHIMMER EFFECT COLORS                        -->
    <!-- ============================================ -->
    <color name="shimmerBase">#1F1F1F</color>             <!-- Base color -->
    <color name="shimmerHighlight">#2F2F2F</color>        <!-- Highlight color -->
    
</resources>
```

---

## 2️⃣ themes.xml (Light Theme)

### Purpose
Theme chính của ứng dụng (Dark theme inspired by Netflix).

### Implementation

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    
    <!-- Base application theme (Dark) -->
    <style name="Theme.Movie88" parent="Theme.MaterialComponents.DayNight.NoActionBar">
        
        <!-- Primary brand color -->
        <item name="colorPrimary">@color/colorPrimary</item>
        <item name="colorPrimaryVariant">@color/colorPrimaryDark</item>
        <item name="colorOnPrimary">@color/textPrimary</item>
        
        <!-- Secondary brand color -->
        <item name="colorSecondary">@color/colorAccent</item>
        <item name="colorSecondaryVariant">@color/colorAccent</item>
        <item name="colorOnSecondary">@color/backgroundColor</item>
        
        <!-- Background colors -->
        <item name="android:colorBackground">@color/backgroundColor</item>
        <item name="colorSurface">@color/backgroundCard</item>
        <item name="colorOnSurface">@color/textPrimary</item>
        
        <!-- Status bar -->
        <item name="android:statusBarColor">@color/backgroundColor</item>
        <item name="android:windowLightStatusBar">false</item>
        
        <!-- Navigation bar -->
        <item name="android:navigationBarColor">@color/backgroundColor</item>
        
        <!-- Text colors -->
        <item name="android:textColorPrimary">@color/textPrimary</item>
        <item name="android:textColorSecondary">@color/textSecondary</item>
        <item name="android:textColorHint">@color/textHint</item>
        
        <!-- Window -->
        <item name="android:windowBackground">@color/backgroundColor</item>
    </style>
    
    <!-- ============================================ -->
    <!-- SPLASH SCREEN THEME                          -->
    <!-- ============================================ -->
    <style name="Theme.Movie88.Splash" parent="Theme.Movie88">
        <item name="android:windowBackground">@drawable/bg_splash</item>
    </style>
    
    <!-- ============================================ -->
    <!-- BUTTON STYLES                                -->
    <!-- ============================================ -->
    
    <!-- Primary Button (Red background, white text) -->
    <style name="Button.Primary" parent="Widget.MaterialComponents.Button">
        <item name="backgroundTint">@color/buttonPrimary</item>
        <item name="android:textColor">@color/buttonTextPrimary</item>
        <item name="cornerRadius">8dp</item>
        <item name="android:textSize">16sp</item>
        <item name="android:textStyle">bold</item>
        <item name="android:paddingTop">14dp</item>
        <item name="android:paddingBottom">14dp</item>
        <item name="android:elevation">4dp</item>
    </style>
    
    <!-- Secondary Button (Gray background) -->
    <style name="Button.Secondary" parent="Widget.MaterialComponents.Button">
        <item name="backgroundTint">@color/buttonSecondary</item>
        <item name="android:textColor">@color/buttonTextPrimary</item>
        <item name="cornerRadius">8dp</item>
        <item name="android:textSize">16sp</item>
        <item name="android:paddingTop">14dp</item>
        <item name="android:paddingBottom">14dp</item>
    </style>
    
    <!-- Outlined Button -->
    <style name="Button.Outlined" parent="Widget.MaterialComponents.Button.OutlinedButton">
        <item name="strokeColor">@color/border</item>
        <item name="strokeWidth">1dp</item>
        <item name="android:textColor">@color/textPrimary</item>
        <item name="cornerRadius">8dp</item>
        <item name="android:textSize">16sp</item>
        <item name="android:paddingTop">14dp</item>
        <item name="android:paddingBottom">14dp</item>
    </style>
    
    <!-- Text Button -->
    <style name="Button.Text" parent="Widget.MaterialComponents.Button.TextButton">
        <item name="android:textColor">@color/colorPrimary</item>
        <item name="android:textSize">14sp</item>
        <item name="android:textStyle">bold</item>
    </style>
    
    <!-- ============================================ -->
    <!-- TEXT INPUT STYLES                            -->
    <!-- ============================================ -->
    
    <!-- Text Input Layout -->
    <style name="TextInputLayout" parent="Widget.MaterialComponents.TextInputLayout.OutlinedBox">
        <item name="boxStrokeColor">@color/borderFocused</item>
        <item name="boxStrokeWidth">1dp</item>
        <item name="hintTextColor">@color/textHint</item>
        <item name="android:textColorHint">@color/textHint</item>
        <item name="boxBackgroundColor">@color/backgroundInput</item>
    </style>
    
    <!-- Text Input EditText -->
    <style name="TextInputEditText" parent="Widget.MaterialComponents.TextInputEditText.OutlinedBox">
        <item name="android:textColor">@color/textPrimary</item>
        <item name="android:textSize">16sp</item>
        <item name="android:paddingTop">16dp</item>
        <item name="android:paddingBottom">16dp</item>
    </style>
    
    <!-- ============================================ -->
    <!-- CARD STYLES                                  -->
    <!-- ============================================ -->
    
    <!-- Card View -->
    <style name="CardView" parent="Widget.MaterialComponents.CardView">
        <item name="cardBackgroundColor">@color/backgroundCard</item>
        <item name="cardCornerRadius">8dp</item>
        <item name="cardElevation">4dp</item>
        <item name="contentPadding">0dp</item>
    </style>
    
    <!-- ============================================ -->
    <!-- TEXT STYLES                                  -->
    <!-- ============================================ -->
    
    <!-- Heading 1 -->
    <style name="Text.Heading1">
        <item name="android:textSize">28sp</item>
        <item name="android:textColor">@color/textPrimary</item>
        <item name="android:textStyle">bold</item>
    </style>
    
    <!-- Heading 2 -->
    <style name="Text.Heading2">
        <item name="android:textSize">24sp</item>
        <item name="android:textColor">@color/textPrimary</item>
        <item name="android:textStyle">bold</item>
    </style>
    
    <!-- Heading 3 -->
    <style name="Text.Heading3">
        <item name="android:textSize">20sp</item>
        <item name="android:textColor">@color/textPrimary</item>
        <item name="android:textStyle">bold</item>
    </style>
    
    <!-- Body Text -->
    <style name="Text.Body">
        <item name="android:textSize">16sp</item>
        <item name="android:textColor">@color/textPrimary</item>
    </style>
    
    <!-- Body Text Secondary -->
    <style name="Text.BodySecondary">
        <item name="android:textSize">14sp</item>
        <item name="android:textColor">@color/textSecondary</item>
    </style>
    
    <!-- Caption Text -->
    <style name="Text.Caption">
        <item name="android:textSize">12sp</item>
        <item name="android:textColor">@color/textSecondary</item>
    </style>
    
    <!-- ============================================ -->
    <!-- TOOLBAR STYLE                                -->
    <!-- ============================================ -->
    
    <style name="Toolbar" parent="Widget.MaterialComponents.Toolbar">
        <item name="android:background">@color/backgroundColor</item>
        <item name="titleTextColor">@color/textPrimary</item>
        <item name="titleTextAppearance">@style/Text.Heading3</item>
        <item name="android:elevation">4dp</item>
    </style>
    
    <!-- ============================================ -->
    <!-- BOTTOM NAVIGATION STYLE                      -->
    <!-- ============================================ -->
    
    <style name="BottomNavigation" parent="Widget.MaterialComponents.BottomNavigationView">
        <item name="android:background">@color/backgroundCard</item>
        <item name="itemIconTint">@drawable/bottom_nav_color</item>
        <item name="itemTextColor">@drawable/bottom_nav_color</item>
        <item name="labelVisibilityMode">labeled</item>
        <item name="android:elevation">8dp</item>
    </style>
    
    <!-- ============================================ -->
    <!-- TAB LAYOUT STYLE                             -->
    <!-- ============================================ -->
    
    <style name="TabLayout" parent="Widget.MaterialComponents.TabLayout">
        <item name="android:background">@color/backgroundColor</item>
        <item name="tabTextColor">@color/textSecondary</item>
        <item name="tabSelectedTextColor">@color/colorPrimary</item>
        <item name="tabIndicatorColor">@color/colorPrimary</item>
        <item name="tabIndicatorHeight">3dp</item>
    </style>
    
    <!-- ============================================ -->
    <!-- DIALOG STYLE                                 -->
    <!-- ============================================ -->
    
    <style name="Dialog" parent="Theme.MaterialComponents.Dialog">
        <item name="android:background">@color/backgroundDialog</item>
        <item name="android:windowBackground">@drawable/bg_dialog</item>
        <item name="colorPrimary">@color/colorPrimary</item>
    </style>
    
    <!-- ============================================ -->
    <!-- CHIP STYLE                                   -->
    <!-- ============================================ -->
    
    <style name="Chip" parent="Widget.MaterialComponents.Chip.Choice">
        <item name="chipBackgroundColor">@drawable/chip_background</item>
        <item name="android:textColor">@drawable/chip_text_color</item>
        <item name="chipStrokeColor">@color/border</item>
        <item name="chipStrokeWidth">1dp</item>
    </style>
    
</resources>
```

---

## 3️⃣ themes.xml (Night Theme)

### Path
`app/src/main/res/values-night/themes.xml`

### Implementation

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    
    <!-- Night theme (same as light theme for this app) -->
    <style name="Theme.Movie88" parent="Theme.MaterialComponents.DayNight.NoActionBar">
        
        <!-- Same colors as light theme since we use dark theme by default -->
        <item name="colorPrimary">@color/colorPrimary</item>
        <item name="colorPrimaryVariant">@color/colorPrimaryDark</item>
        <item name="colorOnPrimary">@color/textPrimary</item>
        
        <item name="colorSecondary">@color/colorAccent</item>
        <item name="colorSecondaryVariant">@color/colorAccent</item>
        <item name="colorOnSecondary">@color/backgroundColor</item>
        
        <item name="android:colorBackground">@color/backgroundColor</item>
        <item name="colorSurface">@color/backgroundCard</item>
        <item name="colorOnSurface">@color/textPrimary</item>
        
        <item name="android:statusBarColor">@color/backgroundColor</item>
        <item name="android:windowLightStatusBar">false</item>
        <item name="android:navigationBarColor">@color/backgroundColor</item>
        
        <item name="android:textColorPrimary">@color/textPrimary</item>
        <item name="android:textColorSecondary">@color/textSecondary</item>
        <item name="android:textColorHint">@color/textHint</item>
        
        <item name="android:windowBackground">@color/backgroundColor</item>
    </style>
    
</resources>
```

---

## 4️⃣ Drawable Resources

### bg_button_primary.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<selector xmlns:android="http://schemas.android.com/apk/res/android">
    <item android:state_enabled="false">
        <shape android:shape="rectangle">
            <solid android:color="@color/buttonDisabled"/>
            <corners android:radius="8dp"/>
        </shape>
    </item>
    <item android:state_pressed="true">
        <shape android:shape="rectangle">
            <solid android:color="@color/colorPrimaryDark"/>
            <corners android:radius="8dp"/>
        </shape>
    </item>
    <item>
        <shape android:shape="rectangle">
            <solid android:color="@color/buttonPrimary"/>
            <corners android:radius="8dp"/>
        </shape>
    </item>
</selector>
```

### bg_seat_available.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/seatAvailable"/>
    <corners android:radius="4dp"/>
</shape>
```

### bg_seat_selected.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/seatSelected"/>
    <corners android:radius="4dp"/>
</shape>
```

### bg_seat_occupied.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/seatOccupied"/>
    <corners android:radius="4dp"/>
</shape>
```

### bg_seat_vip.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/seatVIP"/>
    <corners android:radius="4dp"/>
</shape>
```

### bg_rating_badge.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/blackOverlay"/>
    <corners android:radius="4dp"/>
</shape>
```

### bg_age_rating.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/ageRatingC16"/>
    <corners android:radius="4dp"/>
</shape>
```

### bg_dialog.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<shape xmlns:android="http://schemas.android.com/apk/res/android"
    android:shape="rectangle">
    <solid android:color="@color/backgroundDialog"/>
    <corners android:radius="12dp"/>
</shape>
```

### bottom_nav_color.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<selector xmlns:android="http://schemas.android.com/apk/res/android">
    <item android:color="@color/colorPrimary" android:state_checked="true"/>
    <item android:color="@color/textSecondary"/>
</selector>
```

### chip_background.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<selector xmlns:android="http://schemas.android.com/apk/res/android">
    <item android:state_checked="true">
        <shape android:shape="rectangle">
            <solid android:color="@color/colorPrimary"/>
            <corners android:radius="16dp"/>
        </shape>
    </item>
    <item>
        <shape android:shape="rectangle">
            <solid android:color="@color/backgroundCard"/>
            <corners android:radius="16dp"/>
            <stroke android:width="1dp" android:color="@color/border"/>
        </shape>
    </item>
</selector>
```

### chip_text_color.xml
```xml
<?xml version="1.0" encoding="utf-8"?>
<selector xmlns:android="http://schemas.android.com/apk/res/android">
    <item android:color="@color/textPrimary" android:state_checked="true"/>
    <item android:color="@color/textSecondary"/>
</selector>
```

---

## 🎨 Usage in Layouts

### Apply theme to Activity
```xml
<style name="Theme.Movie88.NoActionBar" parent="Theme.Movie88">
    <item name="windowActionBar">false</item>
    <item name="windowNoTitle">true</item>
</style>
```

### Use color in layout
```xml
<TextView
    android:layout_width="wrap_content"
    android:layout_height="wrap_content"
    android:textColor="@color/textPrimary"
    android:background="@color/backgroundColor"/>
```

### Use style in layout
```xml
<com.google.android.material.button.MaterialButton
    style="@style/Button.Primary"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:text="Đăng nhập"/>
```

---

**Last Updated**: October 29, 2025
