using Movie88.Application.DTOs.Showtimes;

namespace Movie88.Application.Interfaces;

public interface IShowtimeService
{
    Task<ShowtimesByMovieResponseDTO?> GetShowtimesByMovieAsync(int movieId, CancellationToken cancellationToken = default);
    Task<ShowtimeDetailDTO?> GetShowtimeByIdAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task<List<ShowtimesByDateGroupDTO>> GetShowtimesByDateAsync(DateTime date, int? cinemaId = null, int? movieId = null, CancellationToken cancellationToken = default);
}
