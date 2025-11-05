namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Seat information for staff booking verification
/// </summary>
public class SeatInfoDTO
{
    public int SeatId { get; set; }
    public string? Row { get; set; }
    public int? Number { get; set; }
    public string? Type { get; set; }
}
