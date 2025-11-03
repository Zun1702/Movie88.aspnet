using AutoMapper;
using Movie88.Application.DTOs.Showtimes;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class ShowtimeService : IShowtimeService
{
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;

    public ShowtimeService(
        IShowtimeRepository showtimeRepository,
        IMovieRepository movieRepository,
        IMapper mapper)
    {
        _showtimeRepository = showtimeRepository;
        _movieRepository = movieRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ShowtimesByDateDTO>>> GetShowtimesByMovieIdAsync(int movieId, DateTime? date, int? cinemaId)
    {
        // Check if movie exists
        var movie = await _movieRepository.GetByIdAsync(movieId);
        if (movie == null)
        {
            return Result<List<ShowtimesByDateDTO>>.NotFound("Movie not found");
        }

        // Get showtimes
        var showtimes = await _showtimeRepository.GetByMovieIdAsync(movieId, date, cinemaId);
        var showtimesList = showtimes.ToList();

        if (!showtimesList.Any())
        {
            return Result<List<ShowtimesByDateDTO>>.Success(new List<ShowtimesByDateDTO>());
        }

        // Group by date, then by cinema
        var groupedByDate = showtimesList
            .GroupBy(s => s.Starttime!.Value.Date)
            .OrderBy(g => g.Key)
            .Select(dateGroup => new ShowtimesByDateDTO
            {
                Date = dateGroup.Key.ToString("yyyy-MM-dd"),
                Cinemas = dateGroup
                    .GroupBy(s => new 
                    { 
                        Cinemaid = s.Auditorium!.Cinema!.Cinemaid,
                        Name = s.Auditorium.Cinema.Name,
                        Address = s.Auditorium.Cinema.Address,
                        City = s.Auditorium.Cinema.City
                    })
                    .OrderBy(cinemaGroup => cinemaGroup.Key.Name)
                    .Select(cinemaGroup => new ShowtimesByCinemaDTO
                    {
                        Cinema = new CinemaInfoDTO
                        {
                            Cinemaid = cinemaGroup.Key.Cinemaid,
                            Name = cinemaGroup.Key.Name,
                            Address = cinemaGroup.Key.Address,
                            City = cinemaGroup.Key.City
                        },
                        Showtimes = cinemaGroup
                            .OrderBy(s => s.Starttime)
                            .Select(s => 
                            {
                                var showtimeDto = _mapper.Map<ShowtimeItemDTO>(s);
                                // Get available seats synchronously (blocking - not ideal but acceptable for this scenario)
                                showtimeDto.AvailableSeats = _showtimeRepository.GetAvailableSeatsAsync(s.Showtimeid).Result;
                                return showtimeDto;
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

        return Result<List<ShowtimesByDateDTO>>.Success(groupedByDate);
    }
}
