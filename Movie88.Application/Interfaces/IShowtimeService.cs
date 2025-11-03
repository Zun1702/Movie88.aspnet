using Movie88.Application.DTOs.Showtimes;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IShowtimeService
{
    Task<Result<List<ShowtimesByDateDTO>>> GetShowtimesByMovieIdAsync(int movieId, DateTime? date, int? cinemaId);
}
