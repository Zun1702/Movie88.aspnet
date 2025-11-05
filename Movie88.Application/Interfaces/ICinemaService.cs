using Movie88.Application.DTOs.Cinemas;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Cinema service interface
/// </summary>
public interface ICinemaService
{
    /// <summary>
    /// Get all cinemas, optionally filtered by city
    /// </summary>
    /// <param name="city">Optional city filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of cinemas</returns>
    Task<List<CinemaDTO>> GetCinemasAsync(string? city = null, CancellationToken cancellationToken = default);
}
