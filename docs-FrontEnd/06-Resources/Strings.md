# 📝 String Resources (Vietnamese)

## Tổng quan

Tất cả text strings của ứng dụng bằng tiếng Việt để dễ maintain và localize.

---

## 📂 strings.xml

### Purpose
Centralized location cho tất cả text strings trong app.

### Implementation

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    
    <!-- ============================================ -->
    <!-- APP NAME                                     -->
    <!-- ============================================ -->
    <string name="app_name">Movie88</string>
    
    <!-- ============================================ -->
    <!-- AUTHENTICATION                               -->
    <!-- ============================================ -->
    
    <!-- Login Screen -->
    <string name="login_title">Đăng nhập</string>
    <string name="login_subtitle">Chào mừng bạn trở lại!</string>
    <string name="login_email_hint">Email</string>
    <string name="login_password_hint">Mật khẩu</string>
    <string name="login_button">Đăng nhập</string>
    <string name="login_forgot_password">Quên mật khẩu?</string>
    <string name="login_no_account">Chưa có tài khoản?</string>
    <string name="login_sign_up">Đăng ký ngay</string>
    <string name="login_success">Đăng nhập thành công!</string>
    <string name="login_error">Sai email hoặc mật khẩu</string>
    
    <!-- Register Screen -->
    <string name="register_title">Đăng ký</string>
    <string name="register_subtitle">Tạo tài khoản mới</string>
    <string name="register_full_name_hint">Họ và tên</string>
    <string name="register_email_hint">Email</string>
    <string name="register_phone_hint">Số điện thoại</string>
    <string name="register_password_hint">Mật khẩu</string>
    <string name="register_confirm_password_hint">Xác nhận mật khẩu</string>
    <string name="register_button">Đăng ký</string>
    <string name="register_have_account">Đã có tài khoản?</string>
    <string name="register_login">Đăng nhập</string>
    <string name="register_success">Đăng ký thành công!</string>
    <string name="register_error">Email đã được sử dụng</string>
    <string name="register_terms">Bằng việc đăng ký, bạn đồng ý với Điều khoản sử dụng và Chính sách bảo mật</string>
    
    <!-- Change Password -->
    <string name="change_password_title">Đổi mật khẩu</string>
    <string name="change_password_old">Mật khẩu cũ</string>
    <string name="change_password_new">Mật khẩu mới</string>
    <string name="change_password_confirm">Xác nhận mật khẩu mới</string>
    <string name="change_password_button">Đổi mật khẩu</string>
    <string name="change_password_success">Đổi mật khẩu thành công!</string>
    
    <!-- ============================================ -->
    <!-- BOTTOM NAVIGATION                            -->
    <!-- ============================================ -->
    <string name="nav_home">Trang chủ</string>
    <string name="nav_movies">Phim</string>
    <string name="nav_bookings">Vé của tôi</string>
    <string name="nav_profile">Tài khoản</string>
    
    <!-- ============================================ -->
    <!-- HOME SCREEN                                  -->
    <!-- ============================================ -->
    <string name="home_now_showing">Đang chiếu</string>
    <string name="home_coming_soon">Sắp chiếu</string>
    <string name="home_search_hint">Tìm kiếm phim...</string>
    <string name="home_view_all">Xem tất cả</string>
    
    <!-- ============================================ -->
    <!-- MOVIE DETAIL                                 -->
    <!-- ============================================ -->
    <string name="movie_detail_overview">Tóm tắt</string>
    <string name="movie_detail_cast">Diễn viên</string>
    <string name="movie_detail_director">Đạo diễn</string>
    <string name="movie_detail_genre">Thể loại</string>
    <string name="movie_detail_duration">Thời lượng</string>
    <string name="movie_detail_release_date">Khởi chiếu</string>
    <string name="movie_detail_language">Ngôn ngữ</string>
    <string name="movie_detail_rating">Đánh giá</string>
    <string name="movie_detail_book_now">Đặt vé ngay</string>
    <string name="movie_detail_watch_trailer">Xem trailer</string>
    <string name="movie_detail_reviews">Đánh giá của khán giả</string>
    <string name="movie_detail_write_review">Viết đánh giá</string>
    
    <!-- ============================================ -->
    <!-- BOOKING FLOW                                 -->
    <!-- ============================================ -->
    
    <!-- Select Cinema -->
    <string name="booking_select_cinema_title">Chọn rạp</string>
    <string name="booking_select_location">Chọn khu vực</string>
    <string name="booking_all_locations">Tất cả khu vực</string>
    <string name="booking_cinema_distance">Cách %s km</string>
    
    <!-- Select Showtime -->
    <string name="booking_select_showtime_title">Chọn suất chiếu</string>
    <string name="booking_select_date">Chọn ngày</string>
    <string name="booking_today">Hôm nay</string>
    <string name="booking_tomorrow">Ngày mai</string>
    <string name="booking_auditorium">Phòng %s</string>
    <string name="booking_seats_available">%d ghế trống</string>
    
    <!-- Seat Selection -->
    <string name="booking_seat_selection_title">Chọn ghế</string>
    <string name="booking_screen">Màn hình</string>
    <string name="booking_seat_available">Ghế trống</string>
    <string name="booking_seat_selected">Ghế đã chọn</string>
    <string name="booking_seat_occupied">Ghế đã có người</string>
    <string name="booking_seat_vip">Ghế VIP</string>
    <string name="booking_seat_couple">Ghế đôi</string>
    <string name="booking_seat_count">Đã chọn %d ghế</string>
    <string name="booking_seat_warning">Vui lòng chọn ít nhất 1 ghế</string>
    <string name="booking_time_remaining">Thời gian còn lại: %s</string>
    <string name="booking_continue">Tiếp tục</string>
    
    <!-- Combo Selection -->
    <string name="booking_combo_title">Chọn combo</string>
    <string name="booking_combo_subtitle">Đồ ăn và nước uống</string>
    <string name="booking_combo_skip">Bỏ qua</string>
    <string name="booking_combo_add">Thêm</string>
    <string name="booking_combo_quantity">Số lượng: %d</string>
    
    <!-- Booking Summary -->
    <string name="booking_summary_title">Xác nhận đặt vé</string>
    <string name="booking_summary_movie">Phim</string>
    <string name="booking_summary_cinema">Rạp</string>
    <string name="booking_summary_showtime">Suất chiếu</string>
    <string name="booking_summary_seats">Ghế</string>
    <string name="booking_summary_combos">Combo</string>
    <string name="booking_summary_voucher">Mã giảm giá</string>
    <string name="booking_summary_apply_voucher">Áp dụng</string>
    <string name="booking_summary_ticket_price">Tiền vé</string>
    <string name="booking_summary_combo_price">Tiền combo</string>
    <string name="booking_summary_discount">Giảm giá</string>
    <string name="booking_summary_total">Tổng cộng</string>
    <string name="booking_summary_payment">Thanh toán</string>
    
    <!-- ============================================ -->
    <!-- PAYMENT                                      -->
    <!-- ============================================ -->
    <string name="payment_title">Thanh toán</string>
    <string name="payment_method">Phương thức thanh toán</string>
    <string name="payment_vnpay">VNPay</string>
    <string name="payment_processing">Đang xử lý thanh toán...</string>
    <string name="payment_success">Thanh toán thành công!</string>
    <string name="payment_failed">Thanh toán thất bại</string>
    <string name="payment_cancelled">Đã hủy thanh toán</string>
    <string name="payment_result_title">Kết quả thanh toán</string>
    <string name="payment_booking_code">Mã đặt vé</string>
    <string name="payment_qr_code">Mã QR</string>
    <string name="payment_save_qr">Lưu mã QR</string>
    <string name="payment_view_ticket">Xem vé</string>
    <string name="payment_back_home">Về trang chủ</string>
    
    <!-- ============================================ -->
    <!-- MY BOOKINGS                                  -->
    <!-- ============================================ -->
    <string name="bookings_title">Vé của tôi</string>
    <string name="bookings_upcoming">Sắp tới</string>
    <string name="bookings_past">Đã xem</string>
    <string name="bookings_cancelled">Đã hủy</string>
    <string name="bookings_empty">Bạn chưa có vé nào</string>
    <string name="bookings_view_detail">Xem chi tiết</string>
    <string name="bookings_cancel">Hủy vé</string>
    <string name="bookings_cancel_confirm">Bạn có chắc muốn hủy vé này?</string>
    <string name="bookings_cancel_success">Hủy vé thành công</string>
    <string name="bookings_status_pending">Chờ thanh toán</string>
    <string name="bookings_status_completed">Đã thanh toán</string>
    <string name="bookings_status_cancelled">Đã hủy</string>
    
    <!-- ============================================ -->
    <!-- PROFILE                                      -->
    <!-- ============================================ -->
    <string name="profile_title">Tài khoản</string>
    <string name="profile_edit">Chỉnh sửa</string>
    <string name="profile_full_name">Họ và tên</string>
    <string name="profile_email">Email</string>
    <string name="profile_phone">Số điện thoại</string>
    <string name="profile_birthday">Ngày sinh</string>
    <string name="profile_gender">Giới tính</string>
    <string name="profile_male">Nam</string>
    <string name="profile_female">Nữ</string>
    <string name="profile_other">Khác</string>
    <string name="profile_change_password">Đổi mật khẩu</string>
    <string name="profile_change_avatar">Đổi ảnh đại diện</string>
    <string name="profile_settings">Cài đặt</string>
    <string name="profile_notifications">Thông báo</string>
    <string name="profile_language">Ngôn ngữ</string>
    <string name="profile_help">Trợ giúp</string>
    <string name="profile_about">Về chúng tôi</string>
    <string name="profile_logout">Đăng xuất</string>
    <string name="profile_logout_confirm">Bạn có chắc muốn đăng xuất?</string>
    <string name="profile_update_success">Cập nhật thông tin thành công</string>
    
    <!-- ============================================ -->
    <!-- REVIEWS                                      -->
    <!-- ============================================ -->
    <string name="review_title">Đánh giá</string>
    <string name="review_write">Viết đánh giá</string>
    <string name="review_rating">Đánh giá của bạn</string>
    <string name="review_comment">Nhận xét</string>
    <string name="review_submit">Gửi đánh giá</string>
    <string name="review_success">Gửi đánh giá thành công!</string>
    <string name="review_empty">Chưa có đánh giá nào</string>
    <string name="review_edit">Sửa đánh giá</string>
    <string name="review_delete">Xóa đánh giá</string>
    <string name="review_delete_confirm">Bạn có chắc muốn xóa đánh giá này?</string>
    
    <!-- ============================================ -->
    <!-- SEARCH                                       -->
    <!-- ============================================ -->
    <string name="search_title">Tìm kiếm</string>
    <string name="search_hint">Tìm phim, diễn viên, đạo diễn...</string>
    <string name="search_recent">Tìm kiếm gần đây</string>
    <string name="search_popular">Tìm kiếm phổ biến</string>
    <string name="search_results">Kết quả tìm kiếm</string>
    <string name="search_no_results">Không tìm thấy kết quả</string>
    <string name="search_filter">Lọc</string>
    <string name="search_sort">Sắp xếp</string>
    
    <!-- ============================================ -->
    <!-- FILTERS                                      -->
    <!-- ============================================ -->
    <string name="filter_title">Bộ lọc</string>
    <string name="filter_genre">Thể loại</string>
    <string name="filter_year">Năm</string>
    <string name="filter_rating">Đánh giá</string>
    <string name="filter_language">Ngôn ngữ</string>
    <string name="filter_apply">Áp dụng</string>
    <string name="filter_reset">Đặt lại</string>
    
    <!-- Sort -->
    <string name="sort_title">Sắp xếp</string>
    <string name="sort_popular">Phổ biến</string>
    <string name="sort_rating">Đánh giá cao</string>
    <string name="sort_newest">Mới nhất</string>
    <string name="sort_name_az">Tên A-Z</string>
    <string name="sort_name_za">Tên Z-A</string>
    
    <!-- ============================================ -->
    <!-- GENRES                                       -->
    <!-- ============================================ -->
    <string name="genre_action">Hành động</string>
    <string name="genre_comedy">Hài</string>
    <string name="genre_drama">Chính kịch</string>
    <string name="genre_horror">Kinh dị</string>
    <string name="genre_romance">Lãng mạn</string>
    <string name="genre_thriller">Giật gân</string>
    <string name="genre_sci_fi">Khoa học viễn tưởng</string>
    <string name="genre_animation">Hoạt hình</string>
    <string name="genre_documentary">Tài liệu</string>
    <string name="genre_adventure">Phiêu lưu</string>
    
    <!-- ============================================ -->
    <!-- AGE RATINGS                                  -->
    <!-- ============================================ -->
    <string name="age_rating_p">P - Mọi lứa tuổi</string>
    <string name="age_rating_c13">C13 - Dưới 13 tuổi cần người lớn</string>
    <string name="age_rating_c16">C16 - Dưới 16 tuổi cấm xem</string>
    <string name="age_rating_c18">C18 - Dưới 18 tuổi cấm xem</string>
    
    <!-- ============================================ -->
    <!-- COMMON ACTIONS                               -->
    <!-- ============================================ -->
    <string name="action_ok">OK</string>
    <string name="action_cancel">Hủy</string>
    <string name="action_yes">Có</string>
    <string name="action_no">Không</string>
    <string name="action_save">Lưu</string>
    <string name="action_delete">Xóa</string>
    <string name="action_edit">Sửa</string>
    <string name="action_share">Chia sẻ</string>
    <string name="action_close">Đóng</string>
    <string name="action_back">Quay lại</string>
    <string name="action_next">Tiếp theo</string>
    <string name="action_done">Xong</string>
    <string name="action_retry">Thử lại</string>
    <string name="action_refresh">Làm mới</string>
    <string name="action_view_more">Xem thêm</string>
    
    <!-- ============================================ -->
    <!-- ERROR MESSAGES                               -->
    <!-- ============================================ -->
    <string name="error_network">Lỗi kết nối. Vui lòng kiểm tra internet.</string>
    <string name="error_unknown">Đã có lỗi xảy ra. Vui lòng thử lại.</string>
    <string name="error_server">Lỗi máy chủ. Vui lòng thử lại sau.</string>
    <string name="error_token_expired">Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.</string>
    <string name="error_empty_field">Trường này không được để trống</string>
    <string name="error_invalid_email">Email không hợp lệ</string>
    <string name="error_invalid_phone">Số điện thoại không hợp lệ</string>
    <string name="error_invalid_password">Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ và số</string>
    <string name="error_password_mismatch">Mật khẩu xác nhận không khớp</string>
    <string name="error_seats_unavailable">Một số ghế đã được đặt. Vui lòng chọn ghế khác.</string>
    <string name="error_booking_expired">Hết thời gian giữ ghế. Vui lòng đặt lại.</string>
    
    <!-- ============================================ -->
    <!-- LOADING MESSAGES                             -->
    <!-- ============================================ -->
    <string name="loading">Đang tải...</string>
    <string name="loading_movies">Đang tải phim...</string>
    <string name="loading_cinemas">Đang tải rạp...</string>
    <string name="loading_seats">Đang tải sơ đồ ghế...</string>
    <string name="loading_payment">Đang xử lý thanh toán...</string>
    
    <!-- ============================================ -->
    <!-- EMPTY STATES                                 -->
    <!-- ============================================ -->
    <string name="empty_movies">Không có phim nào</string>
    <string name="empty_bookings">Bạn chưa có vé nào</string>
    <string name="empty_reviews">Chưa có đánh giá nào</string>
    <string name="empty_search">Không tìm thấy kết quả</string>
    <string name="empty_notifications">Không có thông báo mới</string>
    
    <!-- ============================================ -->
    <!-- NOTIFICATIONS                                -->
    <!-- ============================================ -->
    <string name="notification_booking_reminder">Nhắc nhở: Phim sắp bắt đầu trong 30 phút!</string>
    <string name="notification_new_movie">Phim mới đã được thêm: %s</string>
    <string name="notification_promotion">Ưu đãi đặc biệt: Giảm 20%% cho vé đầu tiên!</string>
    
    <!-- ============================================ -->
    <!-- TIME & DATE                                  -->
    <!-- ============================================ -->
    <string name="time_just_now">Vừa xong</string>
    <string name="time_minutes_ago">%d phút trước</string>
    <string name="time_hours_ago">%d giờ trước</string>
    <string name="time_days_ago">%d ngày trước</string>
    
    <!-- ============================================ -->
    <!-- VOUCHER                                      -->
    <!-- ============================================ -->
    <string name="voucher_title">Mã giảm giá</string>
    <string name="voucher_enter_code">Nhập mã giảm giá</string>
    <string name="voucher_apply">Áp dụng</string>
    <string name="voucher_applied">Đã áp dụng mã giảm giá</string>
    <string name="voucher_invalid">Mã giảm giá không hợp lệ</string>
    <string name="voucher_expired">Mã giảm giá đã hết hạn</string>
    <string name="voucher_minimum_not_met">Chưa đạt giá trị tối thiểu</string>
    
    <!-- ============================================ -->
    <!-- PERMISSIONS                                  -->
    <!-- ============================================ -->
    <string name="permission_camera">Cần quyền truy cập camera để chụp ảnh</string>
    <string name="permission_storage">Cần quyền truy cập bộ nhớ để lưu ảnh</string>
    <string name="permission_location">Cần quyền truy cập vị trí để tìm rạp gần bạn</string>
    
</resources>
```

---

## 🎯 Usage in Code

### Access strings in Activity/Fragment
```java
String title = getString(R.string.booking_select_cinema_title);
tvTitle.setText(R.string.booking_select_cinema_title);
```

### Format strings with parameters
```xml
<string name="booking_seats_available">%d ghế trống</string>
```

```java
String message = getString(R.string.booking_seats_available, 25);
// Result: "25 ghế trống"
```

### Multiple parameters
```xml
<string name="booking_cinema_distance">Cách %1$s km • %2$d ghế trống</string>
```

```java
String message = getString(R.string.booking_cinema_distance, "2.5", 30);
// Result: "Cách 2.5 km • 30 ghế trống"
```

---

## 🌍 Localization (English)

Create `values-en/strings.xml` for English:

```xml
<?xml version="1.0" encoding="utf-8"?>
<resources>
    <string name="app_name">Movie88</string>
    
    <!-- Authentication -->
    <string name="login_title">Login</string>
    <string name="login_subtitle">Welcome back!</string>
    <string name="login_email_hint">Email</string>
    <string name="login_password_hint">Password</string>
    <string name="login_button">Login</string>
    <!-- ... more English translations -->
</resources>
```

---

**Last Updated**: October 29, 2025
