using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Movies;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service interface for Admin Movie Management operations
/// </summary>
public interface IAdminMovieService
{
    /// <summary>
    /// Create a new movie (Admin only)
    /// </summary>
    Task<Result<MovieResponseDto>> CreateMovieAsync(CreateMovieDto request);

    /// <summary>
    /// Update an existing movie (Admin only)
    /// </summary>
    Task<Result<MovieResponseDto>> UpdateMovieAsync(int movieId, UpdateMovieDto request);

    /// <summary>
    /// Delete a movie (Admin only) - Hard delete, checks for bookings first
    /// </summary>
    Task<Result<bool>> DeleteMovieAsync(int movieId);

    /// <summary>
    /// Get all movies with aggregated data for admin (Admin only)
    /// </summary>
    Task<Result<PagedResultDTO<AdminMovieDto>>> GetMoviesForAdminAsync(int page, int pageSize);
}
