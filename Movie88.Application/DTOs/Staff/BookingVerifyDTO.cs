namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Response DTO for booking verification endpoint
/// GET /api/bookings/verify/{bookingCode}
/// </summary>
public class BookingVerifyDTO
{
    public int BookingId { get; set; }
    public string? BookingCode { get; set; }
    public string? Status { get; set; }
    public DateTime? BookingDate { get; set; }
    
    public CustomerInfoDTO Customer { get; set; } = new();
    public MovieInfoDTO Movie { get; set; } = new();
    public ShowtimeInfoDTO Showtime { get; set; } = new();
    public List<SeatInfoDTO> Seats { get; set; } = new();
    public PricingInfoDTO Pricing { get; set; } = new();
    
    /// <summary>
    /// Payment information retrieved from Payment entity via Booking.Payments collection
    /// </summary>
    public PaymentInfoDTO Payment { get; set; } = new();
    
    /// <summary>
    /// Check-in information using Booking.Checkedintime and Booking.Checkedinby
    /// </summary>
    public CheckInInfoDTO CheckIn { get; set; } = new();
    
    public string BookingStatus { get; set; } = string.Empty;
    
    /// <summary>
    /// Can check-in if: Payment.Status == "Completed" AND Booking.Checkedintime == null
    /// </summary>
    public bool CanCheckIn { get; set; }
}
