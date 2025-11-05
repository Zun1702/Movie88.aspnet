namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Showtime information for staff booking verification
/// </summary>
public class ShowtimeInfoDTO
{
    public int ShowtimeId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public CinemaInfoDTO Cinema { get; set; } = new();
    public AuditoriumInfoDTO Auditorium { get; set; } = new();
    public string? Format { get; set; }
    public string? Language { get; set; }
}
