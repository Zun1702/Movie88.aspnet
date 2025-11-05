using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Staff;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service for staff booking verification and check-in operations
/// </summary>
public interface IBookingVerificationService
{
    /// <summary>
    /// Verify booking code and return full booking details
    /// Checks payment status via Booking.Payments collection
    /// </summary>
    Task<Result<BookingVerifyDTO>> VerifyBookingCodeAsync(string bookingCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check-in customer at counter
    /// Updates Booking.Status, Checkedintime, Checkedinby
    /// </summary>
    Task<Result<CheckInResponseDTO>> CheckInAsync(int bookingId, CheckInCommand command, int staffUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get today's bookings with filters and pagination
    /// </summary>
    Task<Result<PagedResultDTO<TodayBookingDTO>>> GetTodayBookingsAsync(TodayBookingsQuery query, CancellationToken cancellationToken = default);
}
