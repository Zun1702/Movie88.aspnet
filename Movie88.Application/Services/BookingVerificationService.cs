using Microsoft.Extensions.Logging;
using Movie88.Application.DTOs.Booking;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

/// <summary>
/// Service xử lý xác thực và check-in booking.
/// ⚠️ BẢO MẬT: Đảm bảo mỗi BookingCode chỉ được sử dụng 1 lần (Rule 5)
/// </summary>
public class BookingVerificationService : IBookingVerificationService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly Application.Interfaces.IUnitOfWork _unitOfWork;
    private readonly ILogger<BookingVerificationService> _logger;

    public BookingVerificationService(
        IBookingRepository bookingRepository,
        Application.Interfaces.IUnitOfWork unitOfWork,
        ILogger<BookingVerificationService> logger)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BookingVerifyDTO> VerifyBookingCodeAsync(
        string bookingCode, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying booking code: {BookingCode}", bookingCode);

        // 1. Tìm booking theo BookingCode
        var booking = await _bookingRepository.GetByBookingCodeWithDetailsAsync(bookingCode, cancellationToken);
        
        if (booking == null)
        {
            _logger.LogWarning("Booking code not found: {BookingCode}", bookingCode);
            throw new KeyNotFoundException($"Mã đặt vé '{bookingCode}' không tồn tại trong hệ thống.");
        }

        // 2. Lấy thông tin payment (kiểm tra thanh toán)
        var payment = booking.Payments?.FirstOrDefault();
        var isPaymentCompleted = payment?.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true;

        // 3. Kiểm tra trạng thái check-in
        var isCheckedIn = booking.Status?.Equals("CheckedIn", StringComparison.OrdinalIgnoreCase) == true;
        
        // 4. Xác định lý do không thể check-in
        string? blockedReason = null;
        bool canCheckIn = true;

        if (!isPaymentCompleted)
        {
            blockedReason = "Booking chưa được thanh toán. Vui lòng hoàn tất thanh toán trước.";
            canCheckIn = false;
        }
        else if (isCheckedIn)
        {
            // ⚠️ QUAN TRỌNG: Đã check-in rồi thì KHÔNG THỂ check-in lại
            blockedReason = $"Booking đã được check-in lúc {booking.Checkedintime:dd/MM/yyyy HH:mm:ss}. " +
                          $"Mỗi mã đặt vé chỉ được sử dụng 1 lần duy nhất.";
            canCheckIn = false;
            
            _logger.LogWarning(
                "Booking code {BookingCode} đã được check-in rồi. CheckedInTime: {CheckedInTime}, CheckedInBy: {CheckedInBy}",
                bookingCode,
                booking.Checkedintime,
                booking.Checkedinby
            );
        }
        else if (booking.Status?.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) == true)
        {
            blockedReason = "Booking đã bị hủy. Không thể check-in.";
            canCheckIn = false;
        }
        else if (booking.Showtime?.Endtime < DateTime.UtcNow)
        {
            blockedReason = "Suất chiếu đã kết thúc. Không thể check-in.";
            canCheckIn = false;
        }

        // 5. Lấy thông tin staff đã check-in (nếu có)
        string? checkedInByStaffName = booking.CheckedInByUser?.Fullname;

        // 6. Tạo DTO response
        var verifyDto = new BookingVerifyDTO
        {
            BookingId = booking.Bookingid,
            BookingCode = booking.Bookingcode ?? string.Empty,
            Status = booking.Status ?? "Unknown",
            
            // Customer
            CustomerName = booking.Customer?.User?.Fullname ?? "N/A",
            CustomerEmail = booking.Customer?.User?.Email ?? "N/A",
            CustomerPhone = booking.Customer?.User?.Phone ?? "N/A",
            
            // Movie
            MovieTitle = booking.Showtime?.Movie?.Title ?? "N/A",
            MoviePosterUrl = booking.Showtime?.Movie?.Posterurl ?? string.Empty,
            DurationMinutes = booking.Showtime?.Movie?.Durationminutes ?? 0,
            
            // Showtime
            ShowtimeStart = booking.Showtime?.Starttime ?? DateTime.MinValue,
            ShowtimeEnd = booking.Showtime?.Endtime ?? DateTime.MinValue,
            CinemaName = booking.Showtime?.Auditorium?.Cinema?.Name ?? "N/A",
            AuditoriumName = booking.Showtime?.Auditorium?.Name ?? "N/A",
            
            // Seats
            SeatNumbers = booking.BookingSeats?
                .Select(bs => $"{bs.Seat?.Row}{bs.Seat?.Number}")
                .ToList() ?? new List<string>(),
            
            // Payment
            TotalAmount = booking.Totalamount ?? 0,
            IsPaymentCompleted = isPaymentCompleted,
            PaymentMethod = payment?.Method?.Name ?? "N/A",
            PaymentTime = payment?.Paymenttime,
            
            // Check-in status
            IsCheckedIn = isCheckedIn,
            CheckedInTime = booking.Checkedintime,
            CheckedInByStaffName = checkedInByStaffName,
            
            // Validation
            CanCheckIn = canCheckIn,
            CheckInBlockedReason = blockedReason
        };

        _logger.LogInformation(
            "Booking verified: {BookingCode}, CanCheckIn: {CanCheckIn}, IsCheckedIn: {IsCheckedIn}",
            bookingCode,
            canCheckIn,
            isCheckedIn
        );

        return verifyDto;
    }

    public async Task<BookingCheckInResponseDTO> CheckInBookingAsync(
        int bookingId,
        int staffUserId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Check-in booking: BookingId={BookingId}, StaffUserId={StaffUserId}",
            bookingId,
            staffUserId
        );

        // 1. Lấy booking với details
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
        
        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking ID {bookingId} không tồn tại.");
        }

        // 2. Kiểm tra payment đã hoàn thành chưa
        var payment = booking.Payments?.FirstOrDefault();
        var isPaymentCompleted = payment?.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true;
        
        if (!isPaymentCompleted)
        {
            throw new InvalidOperationException(
                $"Không thể check-in. Booking {booking.Bookingcode} chưa được thanh toán."
            );
        }

        // 3. ⚠️ QUAN TRỌNG: Kiểm tra đã check-in chưa (KHÔNG CHO CHECK-IN LẦN 2)
        var isAlreadyCheckedIn = booking.Status?.Equals("CheckedIn", StringComparison.OrdinalIgnoreCase) == true;
        
        if (isAlreadyCheckedIn)
        {
            _logger.LogError(
                "❌ SECURITY VIOLATION: Attempt to check-in already checked-in booking. " +
                "BookingId: {BookingId}, BookingCode: {BookingCode}, " +
                "Previous CheckedInTime: {CheckedInTime}, Previous CheckedInBy: {CheckedInBy}, " +
                "Attempted by StaffUserId: {StaffUserId}",
                bookingId,
                booking.Bookingcode,
                booking.Checkedintime,
                booking.Checkedinby,
                staffUserId
            );

            throw new InvalidOperationException(
                $"Booking {booking.Bookingcode} đã được check-in lúc {booking.Checkedintime:dd/MM/yyyy HH:mm:ss}. " +
                $"Mỗi mã đặt vé chỉ được sử dụng 1 lần duy nhất. " +
                $"Không thể check-in lại."
            );
        }

        // 4. Kiểm tra booking đã bị hủy chưa
        if (booking.Status?.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) == true)
        {
            throw new InvalidOperationException(
                $"Không thể check-in. Booking {booking.Bookingcode} đã bị hủy."
            );
        }

        // 5. Kiểm tra suất chiếu đã kết thúc chưa
        if (booking.Showtime?.Endtime < DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                $"Không thể check-in. Suất chiếu đã kết thúc lúc {booking.Showtime.Endtime:dd/MM/yyyy HH:mm:ss}."
            );
        }

        // 6. ✅ Thực hiện check-in (chỉ 1 lần duy nhất)
        var checkInTime = DateTime.UtcNow;
        
        booking.Status = "CheckedIn";
        booking.Checkedintime = checkInTime;
        booking.Checkedinby = staffUserId;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "✅ Check-in successful: BookingId={BookingId}, BookingCode={BookingCode}, " +
            "CheckedInTime={CheckedInTime}, CheckedInBy StaffUserId={StaffUserId}",
            bookingId,
            booking.Bookingcode,
            checkInTime,
            staffUserId
        );

        // 7. Lấy lại booking để có CheckedInByUser details (sau khi save)
        var updatedBooking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);

        // 8. Tạo response
        var response = new BookingCheckInResponseDTO
        {
            BookingId = booking.Bookingid,
            BookingCode = booking.Bookingcode ?? string.Empty,
            Status = "CheckedIn",
            CheckedInAt = checkInTime,
            CheckedInBy = new StaffInfoDTO
            {
                StaffId = staffUserId,
                StaffName = updatedBooking?.CheckedInByUser?.Fullname ?? "Staff",
                StaffEmail = updatedBooking?.CheckedInByUser?.Email ?? "N/A"
            }
        };

        return response;
    }

    public async Task<bool> CanUseBookingCodeAsync(
        string bookingCode, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var verifyResult = await VerifyBookingCodeAsync(bookingCode, cancellationToken);
            
            // BookingCode có thể sử dụng khi:
            // 1. Payment đã hoàn thành
            // 2. Chưa check-in (isCheckedIn = false)
            // 3. Chưa bị hủy
            // 4. Suất chiếu chưa kết thúc
            return verifyResult.CanCheckIn && !verifyResult.IsCheckedIn;
        }
        catch (KeyNotFoundException)
        {
            // BookingCode không tồn tại
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if booking code {BookingCode} can be used", bookingCode);
            return false;
        }
    }
}
