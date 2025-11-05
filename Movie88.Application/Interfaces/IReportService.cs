using System.Threading;
using System.Threading.Tasks;
using Movie88.Application.DTOs.Admin;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service for generating admin reports and analytics
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Get dashboard overview statistics
    /// </summary>
    Task<Result<DashboardStatsDTO>> GetDashboardStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get daily revenue report
    /// </summary>
    Task<Result<RevenueReportDTO>> GetDailyRevenueAsync(DailyRevenueQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get monthly revenue report
    /// </summary>
    Task<Result<RevenueReportDTO>> GetMonthlyRevenueAsync(MonthlyRevenueQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get booking statistics
    /// </summary>
    Task<Result<BookingStatisticsDTO>> GetBookingStatisticsAsync(BookingStatisticsQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get popular movies ranking
    /// </summary>
    Task<Result<PopularMovieDTO[]>> GetPopularMoviesAsync(PopularMoviesQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get customer analytics
    /// </summary>
    Task<Result<CustomerAnalyticsDTO>> GetCustomerAnalyticsAsync(CustomerAnalyticsQuery query, CancellationToken cancellationToken = default);
}
