using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Request command for check-in endpoint
/// PUT /api/bookings/{id}/check-in
/// </summary>
public class CheckInCommand
{
    [Required(ErrorMessage = "Check-in time is required")]
    public DateTime CheckinTime { get; set; }
    
    /// <summary>
    /// Optional notes (e.g., "Late arrival - 15 minutes after showtime")
    /// </summary>
    public string? Notes { get; set; }
}
