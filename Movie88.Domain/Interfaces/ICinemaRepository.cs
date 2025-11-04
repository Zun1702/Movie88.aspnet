using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

/// <summary>
/// Cinema repository interface
/// </summary>
public interface ICinemaRepository
{
    /// <summary>
    /// Get all cinemas, optionally filtered by city
    /// </summary>
    /// <param name="city">Optional city filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of cinema models</returns>
    Task<List<CinemaModel>> GetCinemasAsync(string? city = null, CancellationToken cancellationToken = default);
}
