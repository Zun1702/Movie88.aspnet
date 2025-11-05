namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Query parameters for today's bookings endpoint
/// GET /api/bookings/today
/// </summary>
public class TodayBookingsQuery
{
    /// <summary>
    /// Filter by cinema ID (optional)
    /// </summary>
    public int? CinemaId { get; set; }
    
    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Items per page (default: 50)
    /// </summary>
    public int PageSize { get; set; } = 50;
    
    /// <summary>
    /// Filter by status: all, pending, confirmed, checkedin, cancelled, completed
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Filter: only bookings with completed payment (check via Payments collection)
    /// </summary>
    public bool? HasPayment { get; set; }
}
