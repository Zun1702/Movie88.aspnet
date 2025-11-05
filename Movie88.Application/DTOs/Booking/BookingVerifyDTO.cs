namespace Movie88.Application.DTOs.Booking;

/// <summary>
/// Response DTO khi xác thực BookingCode/QR Code tại rạp
/// </summary>
public class BookingVerifyDTO
{
    public int BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Confirmed", "CheckedIn", "Cancelled"
    
    // Customer Info
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    
    // Movie Info
    public string MovieTitle { get; set; } = string.Empty;
    public string MoviePosterUrl { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    
    // Showtime Info
    public DateTime ShowtimeStart { get; set; }
    public DateTime ShowtimeEnd { get; set; }
    public string CinemaName { get; set; } = string.Empty;
    public string AuditoriumName { get; set; } = string.Empty;
    
    // Seats
    public List<string> SeatNumbers { get; set; } = new();
    
    // Payment Info
    public decimal TotalAmount { get; set; }
    public bool IsPaymentCompleted { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // "VNPay", "MOMO"
    public DateTime? PaymentTime { get; set; }
    
    // Check-in Info
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckedInTime { get; set; }
    public string? CheckedInByStaffName { get; set; }
    
    // Validation
    public bool CanCheckIn { get; set; }
    public string? CheckInBlockedReason { get; set; } // Lý do không thể check-in
}

/// <summary>
/// Response DTO sau khi check-in thành công
/// </summary>
public class BookingCheckInResponseDTO
{
    public int BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public string Status { get; set; } = "CheckedIn";
    public DateTime CheckedInAt { get; set; }
    public StaffInfoDTO CheckedInBy { get; set; } = new();
}

/// <summary>
/// Thông tin staff thực hiện check-in
/// </summary>
public class StaffInfoDTO
{
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string StaffEmail { get; set; } = string.Empty;
}
