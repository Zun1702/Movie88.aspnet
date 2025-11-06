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
    
    // Admin operations
    Task<CinemaModel?> GetByIdAsync(int id);
    Task<CinemaModel> AddAsync(CinemaModel model);
    Task<CinemaModel> UpdateAsync(CinemaModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> HasActiveShowtimesAsync(int cinemaId);
    Task<int> GetAuditoriumCountAsync(int cinemaId);
}
