using Movie88.Application.DTOs.Showtimes;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service interface for Admin Showtime Management operations
/// </summary>
public interface IAdminShowtimeService
{
    /// <summary>
    /// Create a single showtime (Admin only)
    /// </summary>
    Task<Result<ShowtimeResponseDto>> CreateShowtimeAsync(CreateShowtimeDto request);

    /// <summary>
    /// Create multiple showtimes in bulk (weekly scheduling)
    /// </summary>
    Task<Result<BulkShowtimeResponseDto>> CreateBulkShowtimesAsync(BulkCreateShowtimeDto request);
}
