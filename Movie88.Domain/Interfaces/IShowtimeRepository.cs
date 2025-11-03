using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IShowtimeRepository
{
    Task<IEnumerable<ShowtimeModel>> GetByMovieIdAsync(int movieId, DateTime? date, int? cinemaId);
    Task<int> GetActiveShowtimesCountByMovieIdAsync(int movieId);
    Task<int> GetAvailableSeatsAsync(int showtimeId);
}
