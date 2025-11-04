using Movie88.Application.DTOs.Showtimes;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class ShowtimeService : IShowtimeService
{
    private readonly IShowtimeRepository _showtimeRepository;

    public ShowtimeService(IShowtimeRepository showtimeRepository)
    {
        _showtimeRepository = showtimeRepository;
    }

    public async Task<ShowtimesByMovieResponseDTO?> GetShowtimesByMovieAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var showtimes = await _showtimeRepository.GetByMovieIdAsync(movieId, cancellationToken);

        if (showtimes == null || !showtimes.Any())
            return null;

        // Pre-calculate available seats for all showtimes (batch operation)
        var showtimeIds = showtimes.Select(s => s.Showtimeid).ToList();
        var availableSeatsDict = new Dictionary<int, int>();
        
        foreach (var showtimeId in showtimeIds)
        {
            var availableSeats = await _showtimeRepository.GetAvailableSeatsCountAsync(showtimeId, cancellationToken);
            availableSeatsDict[showtimeId] = availableSeats;
        }

        // Group by date first
        var dateGroups = showtimes
            .Where(s => s.Starttime.HasValue)
            .GroupBy(s => s.Starttime!.Value.Date)
            .OrderBy(g => g.Key)
            .Select(dateGroup => new ShowtimesByDateGroupDTO
            {
                Date = dateGroup.Key.ToString("yyyy-MM-dd"),
                Cinemas = dateGroup
                    .Where(s => s.Auditorium?.Cinema != null)
                    .GroupBy(s => new
                    {
                        s.Auditorium!.Cinema!.Cinemaid,
                        s.Auditorium.Cinema.Name,
                        s.Auditorium.Cinema.Address
                    })
                    .Select(cinemaGroup => new ShowtimesByCinemaGroupDTO
                    {
                        Cinemaid = cinemaGroup.Key.Cinemaid,
                        Name = cinemaGroup.Key.Name ?? "",
                        Address = cinemaGroup.Key.Address ?? "",
                        Showtimes = cinemaGroup.Select(s => new ShowtimeItemDTO
                        {
                            Showtimeid = s.Showtimeid,
                            Starttime = s.Starttime,
                            Price = s.Price,
                            Format = s.Format,
                            Languagetype = s.Languagetype,
                            AuditoriumName = s.Auditorium?.Name,
                            AvailableSeats = availableSeatsDict.GetValueOrDefault(s.Showtimeid, 0)
                        }).OrderBy(s => s.Starttime).ToList()
                    }).ToList()
            }).ToList();

        // Get movie info from first showtime
        var firstShowtime = showtimes.First();
        var movie = firstShowtime.Movie;

        return new ShowtimesByMovieResponseDTO
        {
            Movie = movie != null ? new MovieInfoDTO
            {
                Movieid = movie.Movieid,
                Title = movie.Title,
                Posterurl = movie.Posterurl,
                Durationminutes = movie.Durationminutes,
                Rating = movie.Rating
            } : null!,
            ShowtimesByDate = dateGroups
        };
    }

    public async Task<ShowtimeDetailDTO?> GetShowtimeByIdAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        var showtime = await _showtimeRepository.GetByIdAsync(showtimeId, cancellationToken);

        if (showtime == null)
            return null;

        var availableSeats = await _showtimeRepository.GetAvailableSeatsCountAsync(showtimeId, cancellationToken);

        return new ShowtimeDetailDTO
        {
            Showtimeid = showtime.Showtimeid,
            Movieid = showtime.Movieid,
            Auditoriumid = showtime.Auditoriumid,
            Starttime = showtime.Starttime!.Value,
            Endtime = showtime.Endtime,
            Price = showtime.Price ?? 0,
            Format = showtime.Format ?? "",
            Languagetype = showtime.Languagetype ?? "",
            AvailableSeats = availableSeats,
            Movie = showtime.Movie != null ? new MovieInfoDTO
            {
                Movieid = showtime.Movie.Movieid,
                Title = showtime.Movie.Title,
                Posterurl = showtime.Movie.Posterurl,
                Durationminutes = showtime.Movie.Durationminutes,
                Rating = showtime.Movie.Rating
            } : null,
            Cinema = showtime.Auditorium?.Cinema != null ? new CinemaInfoDTO
            {
                Cinemaid = showtime.Auditorium.Cinema.Cinemaid,
                Name = showtime.Auditorium.Cinema.Name,
                Address = showtime.Auditorium.Cinema.Address,
                City = showtime.Auditorium.Cinema.City
            } : null,
            Auditorium = showtime.Auditorium != null ? new AuditoriumInfoDTO
            {
                Auditoriumid = showtime.Auditorium.Auditoriumid,
                Name = showtime.Auditorium.Name,
                Seatscount = showtime.Auditorium.Seatscount ?? 0
            } : null
        };
    }

    public async Task<List<ShowtimesByDateGroupDTO>> GetShowtimesByDateAsync(DateTime date, int? cinemaId = null, int? movieId = null, CancellationToken cancellationToken = default)
    {
        var showtimes = await _showtimeRepository.GetByDateAsync(date, cinemaId, movieId, cancellationToken);

        if (showtimes == null || !showtimes.Any())
            return new List<ShowtimesByDateGroupDTO>();

        // Pre-calculate available seats for all showtimes (batch operation)
        var showtimeIds = showtimes.Select(s => s.Showtimeid).ToList();
        var availableSeatsDict = new Dictionary<int, int>();
        
        foreach (var showtimeId in showtimeIds)
        {
            var availableSeats = await _showtimeRepository.GetAvailableSeatsCountAsync(showtimeId, cancellationToken);
            availableSeatsDict[showtimeId] = availableSeats;
        }

        // Group by date (should be single date, but keeping structure consistent)
        var dateGroups = showtimes
            .Where(s => s.Starttime.HasValue && s.Auditorium?.Cinema != null)
            .GroupBy(s => s.Starttime!.Value.Date)
            .OrderBy(g => g.Key)
            .Select(dateGroup => new ShowtimesByDateGroupDTO
            {
                Date = dateGroup.Key.ToString("yyyy-MM-dd"),
                Cinemas = dateGroup
                    .GroupBy(s => new
                    {
                        s.Auditorium!.Cinema!.Cinemaid,
                        s.Auditorium.Cinema.Name,
                        s.Auditorium.Cinema.Address
                    })
                    .Select(cinemaGroup => new ShowtimesByCinemaGroupDTO
                    {
                        Cinemaid = cinemaGroup.Key.Cinemaid,
                        Name = cinemaGroup.Key.Name ?? "",
                        Address = cinemaGroup.Key.Address ?? "",
                        Showtimes = cinemaGroup.Select(s => new ShowtimeItemDTO
                        {
                            Showtimeid = s.Showtimeid,
                            Starttime = s.Starttime,
                            Price = s.Price,
                            Format = s.Format,
                            Languagetype = s.Languagetype,
                            AuditoriumName = s.Auditorium?.Name,
                            AvailableSeats = availableSeatsDict.GetValueOrDefault(s.Showtimeid, 0)
                        }).OrderBy(s => s.Starttime).ToList()
                    }).ToList()
            }).ToList();

        return dateGroups;
    }
}
