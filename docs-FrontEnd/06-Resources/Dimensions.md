# 📐 Dimensions & Spacing

## Tổng quan

Hệ thống spacing và dimensions để đảm bảo consistency trong toàn bộ app.

---

## 📂 dimens.xml

### Purpose
Định nghĩa tất cả spacing, sizes, margins, padding values.

### Implementation

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    
    <!-- ============================================ -->
    <!-- SPACING SYSTEM (8dp grid)                   -->
    <!-- ============================================ -->
    <dimen name="spacing_xxs">2dp</dimen>         <!-- Extra Extra Small -->
    <dimen name="spacing_xs">4dp</dimen>          <!-- Extra Small -->
    <dimen name="spacing_sm">8dp</dimen>          <!-- Small -->
    <dimen name="spacing_md">12dp</dimen>         <!-- Medium -->
    <dimen name="spacing_base">16dp</dimen>       <!-- Base spacing -->
    <dimen name="spacing_lg">24dp</dimen>         <!-- Large -->
    <dimen name="spacing_xl">32dp</dimen>         <!-- Extra Large -->
    <dimen name="spacing_xxl">48dp</dimen>        <!-- Extra Extra Large -->
    <dimen name="spacing_xxxl">64dp</dimen>       <!-- Extra Extra Extra Large -->
    
    <!-- ============================================ -->
    <!-- TEXT SIZES                                   -->
    <!-- ============================================ -->
    <dimen name="text_size_caption">12sp</dimen>       <!-- Caption, labels -->
    <dimen name="text_size_body_small">14sp</dimen>    <!-- Small body text -->
    <dimen name="text_size_body">16sp</dimen>          <!-- Body text -->
    <dimen name="text_size_subtitle">18sp</dimen>      <!-- Subtitle -->
    <dimen name="text_size_heading3">20sp</dimen>      <!-- H3 -->
    <dimen name="text_size_heading2">24sp</dimen>      <!-- H2 -->
    <dimen name="text_size_heading1">28sp</dimen>      <!-- H1 -->
    <dimen name="text_size_display">32sp</dimen>       <!-- Display -->
    
    <!-- ============================================ -->
    <!-- BUTTON DIMENSIONS                            -->
    <!-- ============================================ -->
    <dimen name="button_height">48dp</dimen>           <!-- Standard button height -->
    <dimen name="button_height_small">36dp</dimen>     <!-- Small button -->
    <dimen name="button_height_large">56dp</dimen>     <!-- Large button -->
    <dimen name="button_corner_radius">8dp</dimen>     <!-- Button corner radius -->
    <dimen name="button_padding_horizontal">24dp</dimen>
    <dimen name="button_padding_vertical">12dp</dimen>
    
    <!-- ============================================ -->
    <!-- CARD DIMENSIONS                              -->
    <!-- ============================================ -->
    <dimen name="card_corner_radius">8dp</dimen>       <!-- Card corner radius -->
    <dimen name="card_elevation">4dp</dimen>           <!-- Card elevation -->
    <dimen name="card_padding">16dp</dimen>            <!-- Card padding -->
    <dimen name="card_margin">8dp</dimen>              <!-- Card margin -->
    
    <!-- ============================================ -->
    <!-- MOVIE POSTER DIMENSIONS                      -->
    <!-- ============================================ -->
    <dimen name="poster_width_small">100dp</dimen>     <!-- Small poster (list) -->
    <dimen name="poster_height_small">140dp</dimen>
    
    <dimen name="poster_width_medium">140dp</dimen>    <!-- Medium poster (grid) -->
    <dimen name="poster_height_medium">200dp</dimen>
    
    <dimen name="poster_width_large">180dp</dimen>     <!-- Large poster (featured) -->
    <dimen name="poster_height_large">260dp</dimen>
    
    <dimen name="poster_corner_radius">8dp</dimen>     <!-- Poster corner radius -->
    
    <!-- ============================================ -->
    <!-- SEAT DIMENSIONS                              -->
    <!-- ============================================ -->
    <dimen name="seat_size">32dp</dimen>               <!-- Seat width/height -->
    <dimen name="seat_spacing">4dp</dimen>             <!-- Space between seats -->
    <dimen name="seat_corner_radius">4dp</dimen>       <!-- Seat corner radius -->
    <dimen name="seat_text_size">10sp</dimen>          <!-- Seat number text size -->
    
    <!-- ============================================ -->
    <!-- INPUT FIELD DIMENSIONS                       -->
    <!-- ============================================ -->
    <dimen name="input_height">56dp</dimen>            <!-- Input field height -->
    <dimen name="input_corner_radius">8dp</dimen>      <!-- Input corner radius -->
    <dimen name="input_padding_horizontal">16dp</dimen>
    <dimen name="input_padding_vertical">16dp</dimen>
    
    <!-- ============================================ -->
    <!-- ICON SIZES                                   -->
    <!-- ============================================ -->
    <dimen name="icon_size_xs">16dp</dimen>            <!-- Extra small icon -->
    <dimen name="icon_size_sm">20dp</dimen>            <!-- Small icon -->
    <dimen name="icon_size_md">24dp</dimen>            <!-- Medium icon (default) -->
    <dimen name="icon_size_lg">32dp</dimen>            <!-- Large icon -->
    <dimen name="icon_size_xl">48dp</dimen>            <!-- Extra large icon -->
    <dimen name="icon_size_xxl">64dp</dimen>           <!-- Extra extra large icon -->
    
    <!-- ============================================ -->
    <!-- BADGE DIMENSIONS                             -->
    <!-- ============================================ -->
    <dimen name="badge_size">20dp</dimen>              <!-- Notification badge -->
    <dimen name="badge_text_size">10sp</dimen>         <!-- Badge text size -->
    
    <dimen name="rating_badge_width">48dp</dimen>      <!-- Rating badge width -->
    <dimen name="rating_badge_height">24dp</dimen>     <!-- Rating badge height -->
    
    <dimen name="age_rating_size">36dp</dimen>         <!-- Age rating badge size -->
    <dimen name="age_rating_text_size">14sp</dimen>    <!-- Age rating text size -->
    
    <!-- ============================================ -->
    <!-- BANNER DIMENSIONS                            -->
    <!-- ============================================ -->
    <dimen name="banner_height">200dp</dimen>          <!-- Banner height -->
    <dimen name="banner_indicator_size">8dp</dimen>    <!-- Banner indicator dot size -->
    <dimen name="banner_indicator_margin">4dp</dimen>  <!-- Space between dots -->
    
    <!-- ============================================ -->
    <!-- TOOLBAR DIMENSIONS                           -->
    <!-- ============================================ -->
    <dimen name="toolbar_height">56dp</dimen>          <!-- Toolbar height -->
    <dimen name="toolbar_elevation">4dp</dimen>        <!-- Toolbar elevation -->
    
    <!-- ============================================ -->
    <!-- BOTTOM NAVIGATION DIMENSIONS                 -->
    <!-- ============================================ -->
    <dimen name="bottom_nav_height">56dp</dimen>       <!-- Bottom nav height -->
    <dimen name="bottom_nav_elevation">8dp</dimen>     <!-- Bottom nav elevation -->
    <dimen name="bottom_nav_icon_size">24dp</dimen>    <!-- Bottom nav icon size -->
    
    <!-- ============================================ -->
    <!-- TAB DIMENSIONS                               -->
    <!-- ============================================ -->
    <dimen name="tab_height">48dp</dimen>              <!-- Tab height -->
    <dimen name="tab_indicator_height">3dp</dimen>     <!-- Tab indicator height -->
    
    <!-- ============================================ -->
    <!-- DIVIDER DIMENSIONS                           -->
    <!-- ============================================ -->
    <dimen name="divider_height">1dp</dimen>           <!-- Divider height -->
    <dimen name="divider_margin">16dp</dimen>          <!-- Divider margin -->
    
    <!-- ============================================ -->
    <!-- DIALOG DIMENSIONS                            -->
    <!-- ============================================ -->
    <dimen name="dialog_corner_radius">12dp</dimen>    <!-- Dialog corner radius -->
    <dimen name="dialog_padding">24dp</dimen>          <!-- Dialog padding -->
    <dimen name="dialog_title_size">20sp</dimen>       <!-- Dialog title size -->
    <dimen name="dialog_message_size">16sp</dimen>     <!-- Dialog message size -->
    
    <!-- ============================================ -->
    <!-- COMBO ITEM DIMENSIONS                        -->
    <!-- ============================================ -->
    <dimen name="combo_image_size">80dp</dimen>        <!-- Combo item image size -->
    <dimen name="combo_card_height">100dp</dimen>      <!-- Combo card height -->
    
    <!-- ============================================ -->
    <!-- AVATAR DIMENSIONS                            -->
    <!-- ============================================ -->
    <dimen name="avatar_size_small">32dp</dimen>       <!-- Small avatar -->
    <dimen name="avatar_size_medium">48dp</dimen>      <!-- Medium avatar -->
    <dimen name="avatar_size_large">64dp</dimen>       <!-- Large avatar -->
    <dimen name="avatar_size_xlarge">96dp</dimen>      <!-- Extra large avatar -->
    
    <!-- ============================================ -->
    <!-- LOADING INDICATOR DIMENSIONS                 -->
    <!-- ============================================ -->
    <dimen name="loading_size_small">24dp</dimen>      <!-- Small loading -->
    <dimen name="loading_size_medium">36dp</dimen>     <!-- Medium loading -->
    <dimen name="loading_size_large">48dp</dimen>      <!-- Large loading -->
    
    <!-- ============================================ -->
    <!-- CHIP DIMENSIONS                              -->
    <!-- ============================================ -->
    <dimen name="chip_height">32dp</dimen>             <!-- Chip height -->
    <dimen name="chip_corner_radius">16dp</dimen>      <!-- Chip corner radius -->
    <dimen name="chip_padding_horizontal">12dp</dimen> <!-- Chip horizontal padding -->
    
    <!-- ============================================ -->
    <!-- QR CODE DIMENSIONS                           -->
    <!-- ============================================ -->
    <dimen name="qr_code_size_small">120dp</dimen>     <!-- Small QR code -->
    <dimen name="qr_code_size_medium">200dp</dimen>    <!-- Medium QR code -->
    <dimen name="qr_code_size_large">300dp</dimen>     <!-- Large QR code -->
    
    <!-- ============================================ -->
    <!-- SCREEN PADDING                               -->
    <!-- ============================================ -->
    <dimen name="screen_padding_horizontal">16dp</dimen>  <!-- Screen horizontal padding -->
    <dimen name="screen_padding_vertical">16dp</dimen>    <!-- Screen vertical padding -->
    
    <!-- ============================================ -->
    <!-- LIST ITEM DIMENSIONS                         -->
    <!-- ============================================ -->
    <dimen name="list_item_height">72dp</dimen>        <!-- Standard list item height -->
    <dimen name="list_item_height_small">56dp</dimen>  <!-- Small list item -->
    <dimen name="list_item_height_large">88dp</dimen>  <!-- Large list item -->
    
    <!-- ============================================ -->
    <!-- BORDER & STROKE                              -->
    <!-- ============================================ -->
    <dimen name="border_width">1dp</dimen>             <!-- Border width -->
    <dimen name="border_width_thick">2dp</dimen>       <!-- Thick border -->
    
    <!-- ============================================ -->
    <!-- RIPPLE EFFECT                                -->
    <!-- ============================================ -->
    <dimen name="ripple_radius">20dp</dimen>           <!-- Ripple effect radius -->
    
</resources>
```

---

## 🎯 Usage Examples

### Spacing in Layouts

```xml
<!-- Consistent spacing using defined dimensions -->
<LinearLayout
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:padding="@dimen/screen_padding_horizontal"
    android:orientation="vertical">
    
    <!-- Title with margin bottom -->
    <TextView
        style="@style/Text.Heading2"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_marginBottom="@dimen/spacing_base"
        android:text="Chọn ghế"/>
    
    <!-- Divider -->
    <View
        android:layout_width="match_parent"
        android:layout_height="@dimen/divider_height"
        android:layout_marginTop="@dimen/spacing_sm"
        android:layout_marginBottom="@dimen/spacing_sm"
        android:background="@color/divider"/>
    
    <!-- Button with standard height -->
    <com.google.android.material.button.MaterialButton
        style="@style/Button.Primary"
        android:layout_width="match_parent"
        android:layout_height="@dimen/button_height"
        android:text="Tiếp tục"/>
    
</LinearLayout>
```

### Movie Poster Card

```xml
<com.google.android.material.card.MaterialCardView
    android:layout_width="@dimen/poster_width_medium"
    android:layout_height="@dimen/poster_height_medium"
    android:layout_margin="@dimen/card_margin"
    app:cardCornerRadius="@dimen/poster_corner_radius"
    app:cardElevation="@dimen/card_elevation">
    
    <ImageView
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:scaleType="centerCrop"/>
    
</com.google.android.material.card.MaterialCardView>
```

### Seat Grid

```xml
<FrameLayout
    android:layout_width="@dimen/seat_size"
    android:layout_height="@dimen/seat_size"
    android:layout_margin="@dimen/seat_spacing"
    android:background="@drawable/bg_seat_available">
    
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:layout_gravity="center"
        android:textSize="@dimen/seat_text_size"
        android:textColor="@color/textPrimary"
        android:text="A1"/>
    
</FrameLayout>
```

### Input Field

```xml
<com.google.android.material.textfield.TextInputLayout
    style="@style/TextInputLayout"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_marginBottom="@dimen/spacing_base"
    android:hint="Email">
    
    <com.google.android.material.textfield.TextInputEditText
        style="@style/TextInputEditText"
        android:layout_width="match_parent"
        android:layout_height="@dimen/input_height"
        android:inputType="textEmailAddress"/>
    
</com.google.android.material.textfield.TextInputLayout>
```

---

## 📱 Responsive Sizing

### Different Screen Sizes

Create `values-sw600dp/dimens.xml` for tablets:

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <!-- Larger spacing for tablets -->
    <dimen name="spacing_base">20dp</dimen>
    <dimen name="spacing_lg">28dp</dimen>
    <dimen name="spacing_xl">36dp</dimen>
    
    <!-- Larger text sizes -->
    <dimen name="text_size_body">18sp</dimen>
    <dimen name="text_size_heading3">24sp</dimen>
    <dimen name="text_size_heading2">28sp</dimen>
    <dimen name="text_size_heading1">32sp</dimen>
    
    <!-- Larger poster sizes -->
    <dimen name="poster_width_medium">180dp</dimen>
    <dimen name="poster_height_medium">260dp</dimen>
    
    <!-- Larger screen padding -->
    <dimen name="screen_padding_horizontal">24dp</dimen>
</resources>
```

---

## 🎨 Design System Grid

### 8dp Grid System

Tất cả spacing values follow 8dp grid:
- **2dp**: Micro spacing (spacing_xxs)
- **4dp**: Extra small spacing (spacing_xs)
- **8dp**: Small spacing (spacing_sm)
- **16dp**: Base spacing (spacing_base)
- **24dp**: Large spacing (spacing_lg)
- **32dp**: Extra large spacing (spacing_xl)
- **48dp**: Extra extra large spacing (spacing_xxl)
- **64dp**: Extra extra extra large spacing (spacing_xxxl)

### Typography Scale

Text sizes follow Material Design scale:
- **12sp**: Caption text
- **14sp**: Small body text
- **16sp**: Body text (base)
- **18sp**: Subtitle
- **20sp**: Heading 3
- **24sp**: Heading 2
- **28sp**: Heading 1
- **32sp**: Display text

---

**Last Updated**: October 29, 2025
