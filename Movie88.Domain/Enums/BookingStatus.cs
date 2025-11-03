namespace Movie88.Domain.Enums;

/// <summary>
/// Enum for booking status
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking created but payment not completed
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Payment completed, booking confirmed
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// Booking cancelled by user or system
    /// </summary>
    Cancelled = 2,
    
    /// <summary>
    /// User checked in at cinema (scanned QR/booking code)
    /// </summary>
    CheckedIn = 3,
    
    /// <summary>
    /// Showtime completed
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// Booking expired (not paid within time limit)
    /// </summary>
    Expired = 5
}
