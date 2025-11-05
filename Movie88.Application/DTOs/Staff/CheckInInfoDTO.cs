namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Check-in information for staff booking verification
/// NOTE: Uses Booking.Status = "CheckedIn" (NOT Booking.checkedinStatus - field doesn't exist)
/// Uses Booking.Checkedintime and Booking.Checkedinby (NEW fields added to DB)
/// </summary>
public class CheckInInfoDTO
{
    /// <summary>
    /// Whether booking is checked in (Booking.Status == "CheckedIn")
    /// </summary>
    public bool IsCheckedIn { get; set; }
    
    /// <summary>
    /// Check-in timestamp from Booking.Checkedintime (NEW field)
    /// </summary>
    public DateTime? CheckedInTime { get; set; }
    
    /// <summary>
    /// Staff user ID who performed check-in from Booking.Checkedinby (NEW field)
    /// </summary>
    public int? CheckedInBy { get; set; }
    
    /// <summary>
    /// Staff name from Booking.CheckedInByUser navigation property
    /// </summary>
    public string? CheckedInByStaffName { get; set; }
}
