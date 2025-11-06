using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Domain.Enums;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

public class ShowtimeRepository : IShowtimeRepository
{
    private readonly AppDbContext _context;

    public ShowtimeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShowtimeModel>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        var entities = await _context.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Auditorium)
                .ThenInclude(a => a!.Cinema)
            .Where(s => s.Movieid == movieId && s.Starttime >= currentDateTime)
            .OrderBy(s => s.Starttime)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new ShowtimeModel
        {
            Showtimeid = e.Showtimeid,
            Movieid = e.Movieid,
            Auditoriumid = e.Auditoriumid,
            Starttime = e.Starttime,
            Endtime = e.Endtime,
            Price = e.Price,
            Format = e.Format,
            Languagetype = e.Languagetype,
            Movie = e.Movie != null ? new MovieModel
            {
                Movieid = e.Movie.Movieid,
                Title = e.Movie.Title,
                Description = e.Movie.Description,
                Durationminutes = e.Movie.Durationminutes,
                Director = e.Movie.Director,
                Trailerurl = e.Movie.Trailerurl,
                Releasedate = e.Movie.Releasedate,
                Posterurl = e.Movie.Posterurl,
                Country = e.Movie.Country,
                Rating = e.Movie.Rating,
                Genre = e.Movie.Genre,
                Createdat = e.Movie.Createdat
            } : null,
            Auditorium = e.Auditorium != null ? new AuditoriumModel
            {
                Auditoriumid = e.Auditorium.Auditoriumid,
                Cinemaid = e.Auditorium.Cinemaid,
                Name = e.Auditorium.Name,
                Seatscount = e.Auditorium.Seatscount,
                Cinema = e.Auditorium.Cinema != null ? new CinemaModel
                {
                    Cinemaid = e.Auditorium.Cinema.Cinemaid,
                    Name = e.Auditorium.Cinema.Name,
                    Address = e.Auditorium.Cinema.Address,
                    Phone = e.Auditorium.Cinema.Phone,
                    City = e.Auditorium.Cinema.City,
                    Createdat = e.Auditorium.Cinema.Createdat
                } : null
            } : null
        }).ToList();
    }

    public async Task<ShowtimeModel?> GetByIdAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Auditorium)
                .ThenInclude(a => a!.Cinema)
            .FirstOrDefaultAsync(s => s.Showtimeid == showtimeId, cancellationToken);

        if (entity == null)
            return null;

        return new ShowtimeModel
        {
            Showtimeid = entity.Showtimeid,
            Movieid = entity.Movieid,
            Auditoriumid = entity.Auditoriumid,
            Starttime = entity.Starttime,
            Endtime = entity.Endtime,
            Price = entity.Price,
            Format = entity.Format,
            Languagetype = entity.Languagetype,
            Movie = entity.Movie != null ? new MovieModel
            {
                Movieid = entity.Movie.Movieid,
                Title = entity.Movie.Title,
                Description = entity.Movie.Description,
                Durationminutes = entity.Movie.Durationminutes,
                Director = entity.Movie.Director,
                Trailerurl = entity.Movie.Trailerurl,
                Releasedate = entity.Movie.Releasedate,
                Posterurl = entity.Movie.Posterurl,
                Country = entity.Movie.Country,
                Rating = entity.Movie.Rating,
                Genre = entity.Movie.Genre,
                Createdat = entity.Movie.Createdat
            } : null,
            Auditorium = entity.Auditorium != null ? new AuditoriumModel
            {
                Auditoriumid = entity.Auditorium.Auditoriumid,
                Cinemaid = entity.Auditorium.Cinemaid,
                Name = entity.Auditorium.Name,
                Seatscount = entity.Auditorium.Seatscount,
                Cinema = entity.Auditorium.Cinema != null ? new CinemaModel
                {
                    Cinemaid = entity.Auditorium.Cinema.Cinemaid,
                    Name = entity.Auditorium.Cinema.Name,
                    Address = entity.Auditorium.Cinema.Address,
                    Phone = entity.Auditorium.Cinema.Phone,
                    City = entity.Auditorium.Cinema.City,
                    Createdat = entity.Auditorium.Cinema.Createdat
                } : null
            } : null
        };
    }

    public async Task<List<ShowtimeModel>> GetByDateAsync(DateTime date, int? cinemaId = null, int? movieId = null, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var query = _context.Showtimes
            .Include(s => s.Movie)
            .Include(s => s.Auditorium)
                .ThenInclude(a => a!.Cinema)
            .Where(s => s.Starttime >= startOfDay && s.Starttime < endOfDay);

        if (cinemaId.HasValue)
        {
            query = query.Where(s => s.Auditorium!.Cinemaid == cinemaId.Value);
        }

        if (movieId.HasValue)
        {
            query = query.Where(s => s.Movieid == movieId.Value);
        }

        var entities = await query
            .OrderBy(s => s.Starttime)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new ShowtimeModel
        {
            Showtimeid = e.Showtimeid,
            Movieid = e.Movieid,
            Auditoriumid = e.Auditoriumid,
            Starttime = e.Starttime,
            Endtime = e.Endtime,
            Price = e.Price,
            Format = e.Format,
            Languagetype = e.Languagetype,
            Movie = e.Movie != null ? new MovieModel
            {
                Movieid = e.Movie.Movieid,
                Title = e.Movie.Title,
                Description = e.Movie.Description,
                Durationminutes = e.Movie.Durationminutes,
                Director = e.Movie.Director,
                Trailerurl = e.Movie.Trailerurl,
                Releasedate = e.Movie.Releasedate,
                Posterurl = e.Movie.Posterurl,
                Country = e.Movie.Country,
                Rating = e.Movie.Rating,
                Genre = e.Movie.Genre,
                Createdat = e.Movie.Createdat
            } : null,
            Auditorium = e.Auditorium != null ? new AuditoriumModel
            {
                Auditoriumid = e.Auditorium.Auditoriumid,
                Cinemaid = e.Auditorium.Cinemaid,
                Name = e.Auditorium.Name,
                Seatscount = e.Auditorium.Seatscount,
                Cinema = e.Auditorium.Cinema != null ? new CinemaModel
                {
                    Cinemaid = e.Auditorium.Cinema.Cinemaid,
                    Name = e.Auditorium.Cinema.Name,
                    Address = e.Auditorium.Cinema.Address,
                    Phone = e.Auditorium.Cinema.Phone,
                    City = e.Auditorium.Cinema.City,
                    Createdat = e.Auditorium.Cinema.Createdat
                } : null
            } : null
        }).ToList();
    }

    public async Task<int> GetAvailableSeatsCountAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        var showtime = await _context.Showtimes
            .Include(s => s.Auditorium)
            .FirstOrDefaultAsync(s => s.Showtimeid == showtimeId, cancellationToken);

        if (showtime?.Auditorium == null)
            return 0;

        var bookedSeatsCount = await _context.Bookingseats
            .Include(bs => bs.Booking)
            .Where(bs => bs.Showtimeid == showtimeId 
                && bs.Booking != null 
                && bs.Booking.Status != null
                && bs.Booking.Status.ToLower() != nameof(BookingStatus.Cancelled).ToLower())
            .CountAsync(cancellationToken);

        return showtime.Auditorium.Seatscount - bookedSeatsCount;
    }

    public async Task<int> GetActiveShowtimesCountByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
    {
        var currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        return await _context.Showtimes
            .Where(s => s.Movieid == movieId && s.Starttime >= currentDateTime)
            .CountAsync(cancellationToken);
    }

    public async Task<ShowtimeModel> AddAsync(ShowtimeModel model)
    {
        var showtime = new Entities.Showtime
        {
            Movieid = model.Movieid,
            Auditoriumid = model.Auditoriumid,
            Starttime = model.Starttime.HasValue 
                ? DateTime.SpecifyKind(model.Starttime.Value, DateTimeKind.Unspecified) 
                : DateTime.UtcNow,
            Endtime = model.Endtime.HasValue 
                ? DateTime.SpecifyKind(model.Endtime.Value, DateTimeKind.Unspecified) 
                : null,
            Price = model.Price ?? 0m,
            Format = model.Format,
            Languagetype = model.Languagetype
        };

        _context.Showtimes.Add(showtime);
        await _context.SaveChangesAsync();

        model.Showtimeid = showtime.Showtimeid;
        return model;
    }

    public async Task<List<ShowtimeModel>> AddRangeAsync(List<ShowtimeModel> models)
    {
        var showtimes = models.Select(m => new Entities.Showtime
        {
            Movieid = m.Movieid,
            Auditoriumid = m.Auditoriumid,
            Starttime = m.Starttime.HasValue 
                ? DateTime.SpecifyKind(m.Starttime.Value, DateTimeKind.Unspecified) 
                : DateTime.UtcNow,
            Endtime = m.Endtime.HasValue 
                ? DateTime.SpecifyKind(m.Endtime.Value, DateTimeKind.Unspecified) 
                : null,
            Price = m.Price ?? 0m,
            Format = m.Format,
            Languagetype = m.Languagetype
        }).ToList();

        await _context.Showtimes.AddRangeAsync(showtimes);
        await _context.SaveChangesAsync();

        // Update model IDs
        for (int i = 0; i < models.Count; i++)
        {
            models[i].Showtimeid = showtimes[i].Showtimeid;
        }

        return models;
    }
}
