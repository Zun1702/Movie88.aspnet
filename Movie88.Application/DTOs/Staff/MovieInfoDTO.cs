namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Movie information for staff booking verification
/// </summary>
public class MovieInfoDTO
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Rating { get; set; }
}
