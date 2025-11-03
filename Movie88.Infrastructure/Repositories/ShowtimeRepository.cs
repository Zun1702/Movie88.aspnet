using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

public class ShowtimeRepository : IShowtimeRepository
{
    protected readonly AppDbContext _context;
    protected readonly IMapper _mapper;

    public ShowtimeRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShowtimeModel>> GetByMovieIdAsync(int movieId, DateTime? date, int? cinemaId)
    {
        var currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        var query = _context.Showtimes
            .Include(s => s.Auditorium)
                .ThenInclude(a => a!.Cinema)
            .Where(s => s.Movieid == movieId && s.Starttime >= currentDateTime);

        // Filter by date if provided
        if (date.HasValue)
        {
            var startOfDay = date.Value.Date;
            var endOfDay = startOfDay.AddDays(1);
            query = query.Where(s => s.Starttime >= startOfDay && s.Starttime < endOfDay);
        }

        // Filter by cinema if provided
        if (cinemaId.HasValue)
        {
            query = query.Where(s => s.Auditorium!.Cinemaid == cinemaId.Value);
        }

        var showtimes = await query
            .OrderBy(s => s.Starttime)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ShowtimeModel>>(showtimes);
    }

    public async Task<int> GetActiveShowtimesCountByMovieIdAsync(int movieId)
    {
        var currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        return await _context.Showtimes
            .Where(s => s.Movieid == movieId && s.Starttime >= currentDateTime)
            .CountAsync();
    }

    public async Task<int> GetAvailableSeatsAsync(int showtimeId)
    {
        var showtime = await _context.Showtimes
            .Include(s => s.Auditorium)
            .FirstOrDefaultAsync(s => s.Showtimeid == showtimeId);

        if (showtime?.Auditorium == null)
            return 0;

        var bookedSeatsCount = await _context.Bookingseats
            .Include(bs => bs.Booking)
            .Where(bs => bs.Showtimeid == showtimeId 
                && bs.Booking != null 
                && bs.Booking.Status != null
                && bs.Booking.Status.ToLower() != "cancelled")
            .CountAsync();

        return showtime.Auditorium.Seatscount - bookedSeatsCount;
    }
}
