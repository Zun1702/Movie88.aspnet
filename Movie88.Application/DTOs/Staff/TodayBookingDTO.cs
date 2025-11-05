namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// DTO for today's bookings list item
/// GET /api/bookings/today
/// </summary>
public class TodayBookingDTO
{
    public int BookingId { get; set; }
    public string? BookingCode { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime? ShowtimeStart { get; set; }
    public string CinemaName { get; set; } = string.Empty;
    public string AuditoriumName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Payment status from Payment.Status via Booking.Payments collection
    /// </summary>
    public string PaymentStatus { get; set; } = string.Empty;
    
    /// <summary>
    /// Check-in timestamp from Booking.Checkedintime
    /// </summary>
    public DateTime? CheckedInTime { get; set; }
    
    /// <summary>
    /// Staff name who performed check-in from Booking.CheckedInByUser
    /// </summary>
    public string? CheckedInByStaffName { get; set; }
    
    /// <summary>
    /// Can check-in if: Payment completed AND not yet checked in
    /// </summary>
    public bool CanCheckIn { get; set; }
}
