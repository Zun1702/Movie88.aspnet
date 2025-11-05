namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Response DTO for check-in endpoint
/// PUT /api/bookings/{id}/check-in
/// </summary>
public class CheckInResponseDTO
{
    public int BookingId { get; set; }
    public string? BookingCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CheckedInAt { get; set; }
    public StaffInfoDTO CheckedInBy { get; set; } = new();
}
