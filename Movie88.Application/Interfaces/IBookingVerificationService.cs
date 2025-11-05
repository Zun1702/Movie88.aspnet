using Movie88.Application.DTOs.Booking;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service xử lý xác thực và check-in booking tại rạp chiếu phim.
/// Đảm bảo mỗi BookingCode chỉ được sử dụng 1 lần duy nhất (Rule 5).
/// </summary>
public interface IBookingVerificationService
{
    /// <summary>
    /// Xác thực BookingCode/QR Code trước khi check-in.
    /// Kiểm tra: Payment hoàn thành, chưa check-in, chưa hết hạn.
    /// </summary>
    /// <param name="bookingCode">Mã booking (format: M88-00000123)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// BookingVerifyDTO với thông tin booking và canCheckIn = true/false
    /// </returns>
    /// <exception cref="KeyNotFoundException">BookingCode không tồn tại</exception>
    Task<BookingVerifyDTO> VerifyBookingCodeAsync(
        string bookingCode, 
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check-in khách hàng tại rạp. 
    /// ⚠️ QUAN TRỌNG: Sau khi check-in, BookingCode KHÔNG THỂ được sử dụng lại.
    /// </summary>
    /// <param name="bookingId">ID của booking</param>
    /// <param name="staffUserId">ID của staff thực hiện check-in</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>BookingCheckInResponseDTO với thông tin check-in</returns>
    /// <exception cref="KeyNotFoundException">Booking không tồn tại</exception>
    /// <exception cref="InvalidOperationException">
    /// - Booking chưa thanh toán
    /// - Booking đã check-in rồi (KHÔNG THỂ CHECK-IN LẦN 2)
    /// - Booking đã bị hủy
    /// </exception>
    Task<BookingCheckInResponseDTO> CheckInBookingAsync(
        int bookingId,
        int staffUserId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Kiểm tra xem BookingCode có thể được sử dụng hay không.
    /// </summary>
    /// <param name="bookingCode">Mã booking</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// True: BookingCode hợp lệ và chưa check-in
    /// False: BookingCode đã được sử dụng hoặc không hợp lệ
    /// </returns>
    Task<bool> CanUseBookingCodeAsync(
        string bookingCode, 
        CancellationToken cancellationToken = default
    );
}
