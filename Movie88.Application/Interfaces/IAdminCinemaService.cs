using Movie88.Application.DTOs.Cinemas;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service interface for Admin Cinema Management operations
/// </summary>
public interface IAdminCinemaService
{
    /// <summary>
    /// Create a new cinema (Admin only)
    /// </summary>
    Task<Result<CinemaResponseDto>> CreateCinemaAsync(CreateCinemaDto request);

    /// <summary>
    /// Update an existing cinema (Admin only)
    /// </summary>
    Task<Result<CinemaResponseDto>> UpdateCinemaAsync(int cinemaId, UpdateCinemaDto request);

    /// <summary>
    /// Delete a cinema (Admin only) - Hard delete, checks for active showtimes first
    /// </summary>
    Task<Result<bool>> DeleteCinemaAsync(int cinemaId);

    /// <summary>
    /// Get cinema by ID with auditorium count
    /// </summary>
    Task<Result<CinemaResponseDto>> GetCinemaByIdAsync(int cinemaId);
}
