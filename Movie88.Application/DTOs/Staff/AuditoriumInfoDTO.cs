namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Auditorium information for staff booking verification
/// </summary>
public class AuditoriumInfoDTO
{
    public int AuditoriumId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TotalSeats { get; set; }
}
