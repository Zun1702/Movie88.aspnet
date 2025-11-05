namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Cinema information for staff booking verification
/// </summary>
public class CinemaInfoDTO
{
    public int CinemaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}
