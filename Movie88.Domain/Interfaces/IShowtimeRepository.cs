using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IShowtimeRepository
{
    Task<List<ShowtimeModel>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
    Task<ShowtimeModel?> GetByIdAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task<List<ShowtimeModel>> GetByDateAsync(DateTime date, int? cinemaId = null, int? movieId = null, CancellationToken cancellationToken = default);
    Task<int> GetAvailableSeatsCountAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task<int> GetActiveShowtimesCountByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
    
    // Admin operations
    Task<ShowtimeModel> AddAsync(ShowtimeModel model);
    Task<List<ShowtimeModel>> AddRangeAsync(List<ShowtimeModel> models);
}
