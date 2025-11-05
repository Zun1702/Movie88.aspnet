using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReportRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region Dashboard Stats

    public async Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
        var tomorrow = today.AddDays(1);

        return await _context.Payments
            .Where(p => p.Paymenttime >= today && p.Paymenttime < tomorrow)
            .Where(p => p.Status == "Completed")
            .SumAsync(p => p.Amount, cancellationToken);
    }

    public async Task<int> GetTodayBookingsCountAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
        var tomorrow = today.AddDays(1);

        return await _context.Bookings
            .CountAsync(b => b.Bookingtime >= today && b.Bookingtime < tomorrow, cancellationToken);
    }

    public async Task<int> GetActiveMoviesCountAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        return await _context.Showtimes
            .Where(s => s.Starttime >= now)
            .Select(s => s.Movieid)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetActiveCustomersCountAsync(CancellationToken cancellationToken = default)
    {
        var last30Days = DateTime.SpecifyKind(DateTime.Today.AddDays(-30), DateTimeKind.Unspecified);

        return await _context.Bookings
            .Where(b => b.Bookingtime >= last30Days)
            .Select(b => b.Customerid)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<List<(int MovieId, string Title, string? PosterUrl, int TotalBookings, decimal Revenue)>> GetPopularMoviesForDashboardAsync(CancellationToken cancellationToken = default)
    {
        var last30Days = DateTime.SpecifyKind(DateTime.Today.AddDays(-30), DateTimeKind.Unspecified);

        var result = await _context.Payments
            .Where(p => p.Paymenttime >= last30Days)
            .Where(p => p.Status == "Completed")
            .Include(p => p.Booking)
                .ThenInclude(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .GroupBy(p => new
            {
                MovieId = p.Booking.Showtime.Movie.Movieid,
                Title = p.Booking.Showtime.Movie.Title,
                PosterUrl = p.Booking.Showtime.Movie.Posterurl
            })
            .Select(g => new
            {
                g.Key.MovieId,
                g.Key.Title,
                g.Key.PosterUrl,
                TotalBookings = g.Select(p => p.Bookingid).Distinct().Count(),
                Revenue = g.Sum(p => p.Amount)
            })
            .OrderByDescending(m => m.Revenue)
            .Take(5)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.MovieId, r.Title, r.PosterUrl, r.TotalBookings, r.Revenue)).ToList();
    }

    public async Task<List<(int ShowtimeId, string MovieTitle, string CinemaName, string AuditoriumName, DateTime StartTime, decimal Price, string Format, int TotalSeats, int BookedSeats)>> GetUpcomingShowtimesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var next24Hours = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(24), DateTimeKind.Unspecified);

        var result = await _context.Showtimes
            .Where(s => s.Starttime >= now && s.Starttime <= next24Hours)
            .Include(s => s.Movie)
            .Include(s => s.Auditorium)
                .ThenInclude(a => a.Cinema)
            .Include(s => s.Bookingseats)
            .OrderBy(s => s.Starttime)
            .Take(10)
            .Select(s => new
            {
                s.Showtimeid,
                MovieTitle = s.Movie.Title,
                CinemaName = s.Auditorium.Cinema.Name,
                AuditoriumName = s.Auditorium.Name,
                s.Starttime,
                s.Price,
                s.Format,
                TotalSeats = s.Auditorium.Seatscount,
                BookedSeats = s.Bookingseats.Count
            })
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.Showtimeid, r.MovieTitle, r.CinemaName, r.AuditoriumName, r.Starttime, r.Price, r.Format, r.TotalSeats, r.BookedSeats)).ToList();
    }

    #endregion

    #region Daily/Monthly Revenue

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.Paymenttime >= startDate && p.Paymenttime < endDate)
            .Where(p => p.Status == "Completed")
            .SumAsync(p => p.Amount, cancellationToken);
    }

    public async Task<int> GetTotalBookingsCountAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .CountAsync(b => b.Bookingtime >= startDate && b.Bookingtime < endDate, cancellationToken);
    }

    public async Task<decimal> GetTicketSalesRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.Paymenttime >= startDate && p.Paymenttime < endDate)
            .Where(p => p.Status == "Completed")
            .Where(p => p.Booking.Bookingseats.Any())
            .SumAsync(p => p.Amount, cancellationToken);
    }

    public async Task<decimal> GetConcessionsRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .Where(p => p.Paymenttime >= startDate && p.Paymenttime < endDate)
            .Where(p => p.Status == "Completed")
            .Where(p => p.Booking.Bookingcombos.Any())
            .SumAsync(p => p.Amount, cancellationToken);
    }

    public async Task<List<(int MovieId, string MovieTitle, decimal Revenue, int Bookings)>> GetRevenueByMovieAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var result = await _context.Payments
            .Where(p => p.Paymenttime >= startDate && p.Paymenttime < endDate)
            .Where(p => p.Status == "Completed")
            .Include(p => p.Booking)
                .ThenInclude(b => b.Showtime)
                .ThenInclude(s => s.Movie)
            .GroupBy(p => new
            {
                MovieId = p.Booking.Showtime.Movie.Movieid,
                MovieTitle = p.Booking.Showtime.Movie.Title
            })
            .Select(g => new
            {
                g.Key.MovieId,
                g.Key.MovieTitle,
                Revenue = g.Sum(p => p.Amount),
                Bookings = g.Select(p => p.Bookingid).Distinct().Count()
            })
            .OrderByDescending(m => m.Revenue)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.MovieId, r.MovieTitle, r.Revenue, r.Bookings)).ToList();
    }

    public async Task<List<(int CinemaId, string CinemaName, decimal Revenue, int Bookings)>> GetRevenueByCinemaAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var result = await _context.Payments
            .Where(p => p.Paymenttime >= startDate && p.Paymenttime < endDate)
            .Where(p => p.Status == "Completed")
            .Include(p => p.Booking)
                .ThenInclude(b => b.Showtime)
                .ThenInclude(s => s.Auditorium)
                .ThenInclude(a => a.Cinema)
            .GroupBy(p => new
            {
                CinemaId = p.Booking.Showtime.Auditorium.Cinema.Cinemaid,
                CinemaName = p.Booking.Showtime.Auditorium.Cinema.Name
            })
            .Select(g => new
            {
                g.Key.CinemaId,
                g.Key.CinemaName,
                Revenue = g.Sum(p => p.Amount),
                Bookings = g.Select(p => p.Bookingid).Distinct().Count()
            })
            .OrderByDescending(c => c.Revenue)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.CinemaId, r.CinemaName, r.Revenue, r.Bookings)).ToList();
    }

    public async Task<List<(int Hour, decimal Revenue, int Bookings)>> GetRevenueByHourAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var result = await _context.Bookings
            .Where(b => b.Bookingtime >= startDate && b.Bookingtime < endDate)
            .Include(b => b.Showtime)
            .Include(b => b.Payments)
            .GroupBy(b => b.Showtime.Starttime.Hour)
            .Select(g => new
            {
                Hour = g.Key,
                Revenue = g.SelectMany(b => b.Payments)
                    .Where(p => p.Status == "Completed")
                    .Sum(p => p.Amount),
                Bookings = g.Count()
            })
            .OrderBy(h => h.Hour)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.Hour, r.Revenue, r.Bookings)).ToList();
    }

    #endregion

    #region Booking Statistics

    public async Task<List<BookingModel>> GetBookingsInRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var bookings = await _context.Bookings
            .Where(b => b.Bookingtime >= startDate && b.Bookingtime < endDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<BookingModel>>(bookings);
    }

    public async Task<List<(int Hour, int Count)>> GetBookingsByHourAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var result = await _context.Bookings
            .Where(b => b.Bookingtime >= startDate && b.Bookingtime < endDate)
            .Include(b => b.Showtime)
            .GroupBy(b => b.Showtime.Starttime.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderByDescending(h => h.Count)
            .Take(2)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.Hour, r.Count)).ToList();
    }

    #endregion

    #region Popular Movies

    public async Task<List<(int MovieId, string Title, int TotalBookings, decimal Revenue, int TotalSeats, int BookedSeats, decimal? AverageRating)>> GetPopularMoviesAsync(DateTime startDate, int limit, CancellationToken cancellationToken = default)
    {
        var movieStats = await _context.Movies
            .Select(m => new
            {
                Movie = m,
                TotalBookings = _context.Bookings
                    .Where(b => b.Showtime.Movieid == m.Movieid)
                    .Where(b => b.Bookingtime >= startDate)
                    .Count(),
                Revenue = _context.Payments
                    .Where(p => p.Booking.Showtime.Movieid == m.Movieid)
                    .Where(p => p.Paymenttime >= startDate)
                    .Where(p => p.Status == "Completed")
                    .Sum(p => (decimal?)p.Amount) ?? 0,
                AverageRating = m.Reviews.Any() ? m.Reviews.Average(r => (decimal?)r.Rating) : null,
                TotalSeats = _context.Showtimes
                    .Where(s => s.Movieid == m.Movieid)
                    .Where(s => s.Starttime >= startDate)
                    .Sum(s => s.Auditorium.Seatscount),
                BookedSeats = _context.Bookingseats
                    .Where(bs => bs.Showtime.Movieid == m.Movieid)
                    .Where(bs => bs.Booking.Bookingtime >= startDate)
                    .Count()
            })
            .Where(m => m.Revenue > 0)
            .OrderByDescending(m => m.Revenue)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return movieStats.Select(m => (
            m.Movie.Movieid,
            m.Movie.Title,
            m.TotalBookings,
            m.Revenue,
            m.TotalSeats,
            m.BookedSeats,
            m.AverageRating
        )).ToList();
    }

    public async Task<List<(int MovieId, decimal Revenue)>> GetPreviousPeriodRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var result = await _context.Payments
            .Where(p => p.Paymenttime >= startDate && p.Paymenttime < endDate)
            .Where(p => p.Status == "Completed")
            .Include(p => p.Booking)
                .ThenInclude(b => b.Showtime)
            .GroupBy(p => p.Booking.Showtime.Movieid)
            .Select(g => new { MovieId = g.Key, Revenue = g.Sum(p => p.Amount) })
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.MovieId, r.Revenue)).ToList();
    }

    #endregion

    #region Customer Analytics

    public async Task<int> GetTotalCustomersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.CountAsync(cancellationToken);
    }

    public async Task<int> GetNewCustomersCountAsync(DateTime startDate, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.User)
            .CountAsync(c => c.User.Createdat >= startDate, cancellationToken);
    }

    public async Task<int> GetActiveCustomersCountAsync(DateTime startDate, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.Bookingtime >= startDate)
            .Select(b => b.Customerid)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetCustomersWithRepeatBookingsCountAsync(DateTime startDate, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.Bookingtime >= startDate)
            .GroupBy(b => b.Customerid)
            .Where(g => g.Count() > 1)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetInactiveCustomersCountAsync(DateTime last90Days, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => !_context.Bookings.Any(b => b.Customerid == c.Customerid && b.Bookingtime >= last90Days))
            .CountAsync(cancellationToken);
    }

    public async Task<List<decimal>> GetAllCustomerLifetimeValuesAsync(CancellationToken cancellationToken = default)
    {
        var lifetimeValues = await _context.Customers
            .Select(c => new
            {
                TotalSpent = _context.Payments
                    .Where(p => p.Customerid == c.Customerid)
                    .Where(p => p.Status == "Completed")
                    .Sum(p => (decimal?)p.Amount) ?? 0
            })
            .Where(c => c.TotalSpent > 0)
            .Select(c => c.TotalSpent)
            .ToListAsync(cancellationToken);

        return lifetimeValues;
    }

    public async Task<List<(int CustomerId, string Fullname, string Email, int TotalBookings, decimal TotalSpent, DateTime MemberSince)>> GetTopCustomersAsync(int limit, CancellationToken cancellationToken = default)
    {
        var result = await _context.Customers
            .Include(c => c.User)
            .Select(c => new
            {
                c.Customerid,
                Fullname = c.User.Fullname,
                Email = c.User.Email,
                TotalBookings = _context.Bookings.Where(b => b.Customerid == c.Customerid).Count(),
                TotalSpent = _context.Payments
                    .Where(p => p.Customerid == c.Customerid)
                    .Where(p => p.Status == "Completed")
                    .Sum(p => (decimal?)p.Amount) ?? 0,
                MemberSince = c.User.Createdat ?? DateTime.MinValue
            })
            .Where(c => c.TotalSpent > 0)
            .OrderByDescending(c => c.TotalSpent)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.Customerid, r.Fullname, r.Email, r.TotalBookings, r.TotalSpent, r.MemberSince)).ToList();
    }

    public async Task<List<CustomerModel>> GetCustomersWithDateOfBirthAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _context.Customers
            .Include(c => c.User)
            .Where(c => c.Dateofbirth != null)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CustomerModel>>(customers);
    }

    public async Task<List<CustomerModel>> GetCustomersWithGenderAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _context.Customers
            .Include(c => c.User)
            .Where(c => c.Gender != null)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<CustomerModel>>(customers);
    }

    #endregion
}
