using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

/// <summary>
/// Repository for generating admin reports and analytics
/// Complex aggregation queries for dashboard and reports
/// </summary>
public interface IReportRepository
{
    // Dashboard Stats
    Task<decimal> GetTodayRevenueAsync(CancellationToken cancellationToken = default);
    Task<int> GetTodayBookingsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveMoviesCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetActiveCustomersCountAsync(CancellationToken cancellationToken = default);
    Task<List<(int MovieId, string Title, string? PosterUrl, int TotalBookings, decimal Revenue)>> GetPopularMoviesForDashboardAsync(CancellationToken cancellationToken = default);
    Task<List<(int ShowtimeId, string MovieTitle, string CinemaName, string AuditoriumName, DateTime StartTime, decimal Price, string Format, int TotalSeats, int BookedSeats)>> GetUpcomingShowtimesAsync(CancellationToken cancellationToken = default);
    
    // Daily/Monthly Revenue
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<int> GetTotalBookingsCountAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetTicketSalesRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<decimal> GetConcessionsRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<(int MovieId, string MovieTitle, decimal Revenue, int Bookings)>> GetRevenueByMovieAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<(int CinemaId, string CinemaName, decimal Revenue, int Bookings)>> GetRevenueByCinemaAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<(int Hour, decimal Revenue, int Bookings)>> GetRevenueByHourAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    
    // Booking Statistics
    Task<List<BookingModel>> GetBookingsInRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<(int Hour, int Count)>> GetBookingsByHourAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    
    // Popular Movies
    Task<List<(int MovieId, string Title, int TotalBookings, decimal Revenue, int TotalSeats, int BookedSeats, decimal? AverageRating)>> GetPopularMoviesAsync(DateTime startDate, int limit, CancellationToken cancellationToken = default);
    Task<List<(int MovieId, decimal Revenue)>> GetPreviousPeriodRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    
    // Customer Analytics
    Task<int> GetTotalCustomersCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetNewCustomersCountAsync(DateTime startDate, CancellationToken cancellationToken = default);
    Task<int> GetActiveCustomersCountAsync(DateTime startDate, CancellationToken cancellationToken = default);
    Task<int> GetCustomersWithRepeatBookingsCountAsync(DateTime startDate, CancellationToken cancellationToken = default);
    Task<int> GetInactiveCustomersCountAsync(DateTime last90Days, CancellationToken cancellationToken = default);
    Task<List<decimal>> GetAllCustomerLifetimeValuesAsync(CancellationToken cancellationToken = default);
    Task<List<(int CustomerId, string Fullname, string Email, int TotalBookings, decimal TotalSpent, DateTime MemberSince)>> GetTopCustomersAsync(int limit, CancellationToken cancellationToken = default);
    Task<List<CustomerModel>> GetCustomersWithDateOfBirthAsync(CancellationToken cancellationToken = default);
    Task<List<CustomerModel>> GetCustomersWithGenderAsync(CancellationToken cancellationToken = default);
}
