using Movie88.Application.DTOs.Showtimes;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;

namespace Movie88.Application.Services;

public class AdminShowtimeService : IAdminShowtimeService
{
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly Movie88.Application.Interfaces.IUnitOfWork _unitOfWork;

    public AdminShowtimeService(
        IShowtimeRepository showtimeRepository,
        IMovieRepository movieRepository,
        Movie88.Application.Interfaces.IUnitOfWork unitOfWork)
    {
        _showtimeRepository = showtimeRepository;
        _movieRepository = movieRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ShowtimeResponseDto>> CreateShowtimeAsync(CreateShowtimeDto request)
    {
        try
        {
            // Get movie to calculate endtime
            var movie = await _movieRepository.GetByIdAsync(request.MovieId);
            if (movie == null)
            {
                return Result<ShowtimeResponseDto>.Error("Movie not found", 404);
            }

            // Combine language and subtitle into languageType
            var languageType = !string.IsNullOrWhiteSpace(request.Language) && !string.IsNullOrWhiteSpace(request.Subtitle)
                ? $"{request.Language} - {request.Subtitle}"
                : request.Language ?? request.Subtitle ?? "Not specified";

            var showtimeModel = new ShowtimeModel
            {
                Movieid = request.MovieId,
                Auditoriumid = request.AuditoriumId,
                Starttime = request.StartTime,
                Endtime = request.StartTime.AddMinutes(movie.Durationminutes), // Auto-calculate
                Price = request.BasePrice,
                Format = request.Format,
                Languagetype = languageType
            };

            var createdShowtime = await _showtimeRepository.AddAsync(showtimeModel);
            await _unitOfWork.CommitAsync();

            // Get available seats count
            var availableSeats = await _showtimeRepository.GetAvailableSeatsCountAsync(createdShowtime.Showtimeid);

            var response = new ShowtimeResponseDto
            {
                ShowtimeId = createdShowtime.Showtimeid,
                MovieId = createdShowtime.Movieid,
                MovieTitle = movie.Title ?? "Unknown",
                AuditoriumId = createdShowtime.Auditoriumid,
                AuditoriumName = $"Auditorium {createdShowtime.Auditoriumid}", // Simplified
                StartTime = createdShowtime.Starttime!.Value,
                EndTime = createdShowtime.Endtime, // Nullable is OK
                Price = createdShowtime.Price!.Value,
                Format = createdShowtime.Format ?? string.Empty,
                LanguageType = createdShowtime.Languagetype ?? string.Empty,
                AvailableSeats = availableSeats
            };

            return Result<ShowtimeResponseDto>.Success(response, "Showtime created successfully");
        }
        catch (Exception ex)
        {
            return Result<ShowtimeResponseDto>.Error($"Error creating showtime: {ex.Message}", 500);
        }
    }

    public async Task<Result<BulkShowtimeResponseDto>> CreateBulkShowtimesAsync(BulkCreateShowtimeDto request)
    {
        try
        {
            // Get movie to calculate endtime
            var movie = await _movieRepository.GetByIdAsync(request.MovieId);
            if (movie == null)
            {
                return Result<BulkShowtimeResponseDto>.Error("Movie not found", 404);
            }

            var showtimesToCreate = new List<ShowtimeModel>();
            var details = new List<ShowtimeCreationDetailDto>();
            int created = 0;
            int skipped = 0;

            // Loop through each date in the range
            for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
            {
                var dayOfWeek = date.ToDateTime(TimeOnly.MinValue).DayOfWeek;

                // Skip if day is in skipDays list
                if (request.SkipDays.Contains(dayOfWeek))
                {
                    skipped++;
                    continue;
                }

                // Determine price based on weekend/weekday
                var isWeekend = dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
                var price = isWeekend ? request.Pricing.Weekend : request.Pricing.Weekday;

                var timeslotsForDay = new List<string>();
                int createdForDay = 0;

                // Create showtime for each timeslot
                foreach (var timeslot in request.Timeslots)
                {
                    var startTime = date.ToDateTime(timeslot);
                    var endTime = startTime.AddMinutes(movie.Durationminutes);

                    var showtimeModel = new ShowtimeModel
                    {
                        Movieid = request.MovieId,
                        Auditoriumid = request.AuditoriumId,
                        Starttime = startTime,
                        Endtime = endTime,
                        Price = price,
                        Format = request.Format,
                        Languagetype = request.LanguageType
                    };

                    showtimesToCreate.Add(showtimeModel);
                    timeslotsForDay.Add(timeslot.ToString("HH:mm"));
                    createdForDay++;
                    created++;
                }

                // Add detail for this date
                if (createdForDay > 0)
                {
                    details.Add(new ShowtimeCreationDetailDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        Timeslots = timeslotsForDay,
                        Price = price,
                        Created = createdForDay
                    });
                }
            }

            // Bulk insert all showtimes
            if (showtimesToCreate.Any())
            {
                await _showtimeRepository.AddRangeAsync(showtimesToCreate);
                await _unitOfWork.CommitAsync();
            }

            var response = new BulkShowtimeResponseDto
            {
                Created = created,
                Skipped = skipped,
                Failed = 0,
                Details = details
            };

            return Result<BulkShowtimeResponseDto>.Success(
                response,
                $"{created} showtimes created successfully");
        }
        catch (Exception ex)
        {
            return Result<BulkShowtimeResponseDto>.Error($"Error creating bulk showtimes: {ex.Message}", 500);
        }
    }
}
