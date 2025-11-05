using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Movie88.Application.DTOs.Admin;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IReportRepository reportRepository, ILogger<ReportService> logger)
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    public async Task<Result<DashboardStatsDTO>> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Today's revenue from completed payments
            var todayRevenue = await _reportRepository.GetTodayRevenueAsync(cancellationToken);

            // Today's bookings count
            var todayBookings = await _reportRepository.GetTodayBookingsCountAsync(cancellationToken);

            // Active movies (have upcoming showtimes)
            var activeMovies = await _reportRepository.GetActiveMoviesCountAsync(cancellationToken);

            // Active customers (booked in last 30 days)
            var activeCustomers = await _reportRepository.GetActiveCustomersCountAsync(cancellationToken);

            // Top 5 popular movies (by revenue in last 30 days)
            var popularMoviesData = await _reportRepository.GetPopularMoviesForDashboardAsync(cancellationToken);
            var popularMovies = popularMoviesData.Select(m => new PopularMovieItemDTO
            {
                MovieId = m.MovieId,
                Title = m.Title,
                PosterUrl = m.PosterUrl,
                TotalBookings = m.TotalBookings,
                Revenue = m.Revenue
            }).ToList();

            // Upcoming showtimes (next 24 hours)
            var upcomingShowtimesData = await _reportRepository.GetUpcomingShowtimesAsync(cancellationToken);
            var upcomingShowtimes = upcomingShowtimesData.Select(s => new UpcomingShowtimeDTO
            {
                ShowtimeId = s.ShowtimeId,
                MovieTitle = s.MovieTitle,
                CinemaName = s.CinemaName,
                AuditoriumName = s.AuditoriumName,
                StartTime = s.StartTime,
                Price = s.Price,
                Format = s.Format,
                TotalSeats = s.TotalSeats,
                AvailableSeats = s.TotalSeats - s.BookedSeats
            }).ToList();

            var result = new DashboardStatsDTO
            {
                TodayRevenue = todayRevenue,
                TodayBookings = todayBookings,
                ActiveMovies = activeMovies,
                ActiveCustomers = activeCustomers,
                PopularMovies = popularMovies,
                UpcomingShowtimes = upcomingShowtimes
            };

            return Result<DashboardStatsDTO>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return Result<DashboardStatsDTO>.Failure("Failed to retrieve dashboard statistics");
        }
    }

    public async Task<Result<RevenueReportDTO>> GetDailyRevenueAsync(DailyRevenueQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var dateOnly = query.GetDateOnly();
            var date = dateOnly.ToDateTime(TimeOnly.MinValue);
            var startDate = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            var endDate = startDate.AddDays(1);

            // Total revenue
            var totalRevenue = await _reportRepository.GetTotalRevenueAsync(startDate, endDate, cancellationToken);

            // Total bookings
            var totalBookings = await _reportRepository.GetTotalBookingsCountAsync(startDate, endDate, cancellationToken);

            // Average ticket price
            var averageTicketPrice = totalBookings > 0 ? totalRevenue / totalBookings : 0;

            // Breakdown: Ticket sales and Concessions
            var ticketSales = await _reportRepository.GetTicketSalesRevenueAsync(startDate, endDate, cancellationToken);
            var concessions = await _reportRepository.GetConcessionsRevenueAsync(startDate, endDate, cancellationToken);

            // Revenue by movie
            var byMovieData = await _reportRepository.GetRevenueByMovieAsync(startDate, endDate, cancellationToken);
            var byMovie = byMovieData.Select(m => new RevenueByMovieDTO
            {
                MovieId = m.MovieId,
                MovieTitle = m.MovieTitle,
                Revenue = m.Revenue,
                Bookings = m.Bookings
            }).ToList();

            // Revenue by cinema
            var byCinemaData = await _reportRepository.GetRevenueByCinemaAsync(startDate, endDate, cancellationToken);
            var byCinema = byCinemaData.Select(c => new RevenueByCinemaDTO
            {
                CinemaId = c.CinemaId,
                CinemaName = c.CinemaName,
                Revenue = c.Revenue,
                Bookings = c.Bookings
            }).ToList();

            // Revenue by hour (for daily report)
            var byHourData = await _reportRepository.GetRevenueByHourAsync(startDate, endDate, cancellationToken);
            var byHour = byHourData.Select(h => new RevenueByHourDTO
            {
                Hour = $"{h.Hour:D2}:00-{(h.Hour + 1):D2}:00",
                Revenue = h.Revenue,
                Bookings = h.Bookings
            }).ToList();

            var result = new RevenueReportDTO
            {
                Period = dateOnly.ToString("yyyy-MM-dd"),
                TotalRevenue = totalRevenue,
                TotalBookings = totalBookings,
                AverageTicketPrice = averageTicketPrice,
                Breakdown = new RevenueBreakdownDTO
                {
                    TicketSales = ticketSales,
                    Concessions = concessions
                },
                ByMovie = byMovie,
                ByCinema = byCinema,
                ByHour = byHour
            };

            return Result<RevenueReportDTO>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily revenue for date {Date}", query.Date);
            return Result<RevenueReportDTO>.Failure("Failed to retrieve daily revenue report. Invalid date format or query error.");
        }
    }

    public async Task<Result<RevenueReportDTO>> GetMonthlyRevenueAsync(MonthlyRevenueQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var startDate = DateTime.SpecifyKind(new DateTime(query.Year, query.Month, 1), DateTimeKind.Unspecified);
            var endDate = startDate.AddMonths(1);

            // Total revenue
            var totalRevenue = await _reportRepository.GetTotalRevenueAsync(startDate, endDate, cancellationToken);

            // Total bookings
            var totalBookings = await _reportRepository.GetTotalBookingsCountAsync(startDate, endDate, cancellationToken);

            // Average ticket price
            var averageTicketPrice = totalBookings > 0 ? totalRevenue / totalBookings : 0;

            // Breakdown: Ticket sales and Concessions
            var ticketSales = await _reportRepository.GetTicketSalesRevenueAsync(startDate, endDate, cancellationToken);
            var concessions = await _reportRepository.GetConcessionsRevenueAsync(startDate, endDate, cancellationToken);

            // Revenue by movie
            var byMovieData = await _reportRepository.GetRevenueByMovieAsync(startDate, endDate, cancellationToken);
            var byMovie = byMovieData.Select(m => new RevenueByMovieDTO
            {
                MovieId = m.MovieId,
                MovieTitle = m.MovieTitle,
                Revenue = m.Revenue,
                Bookings = m.Bookings
            }).ToList();

            // Revenue by cinema
            var byCinemaData = await _reportRepository.GetRevenueByCinemaAsync(startDate, endDate, cancellationToken);
            var byCinema = byCinemaData.Select(c => new RevenueByCinemaDTO
            {
                CinemaId = c.CinemaId,
                CinemaName = c.CinemaName,
                Revenue = c.Revenue,
                Bookings = c.Bookings
            }).ToList();

            var result = new RevenueReportDTO
            {
                Period = $"{query.Year}-{query.Month:D2}",
                TotalRevenue = totalRevenue,
                TotalBookings = totalBookings,
                AverageTicketPrice = averageTicketPrice,
                Breakdown = new RevenueBreakdownDTO
                {
                    TicketSales = ticketSales,
                    Concessions = concessions
                },
                ByMovie = byMovie,
                ByCinema = byCinema,
                ByHour = null // Not applicable for monthly report
            };

            return Result<RevenueReportDTO>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monthly revenue for {Year}-{Month}", query.Year, query.Month);
            return Result<RevenueReportDTO>.Failure("Failed to retrieve monthly revenue report");
        }
    }

    public async Task<Result<BookingStatisticsDTO>> GetBookingStatisticsAsync(BookingStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var startDateOnly = query.GetStartDateOnly();
            var endDateOnly = query.GetEndDateOnly();
            
            var startDate = DateTime.SpecifyKind(startDateOnly.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
            var endDate = DateTime.SpecifyKind(endDateOnly.ToDateTime(TimeOnly.MinValue).AddDays(1), DateTimeKind.Unspecified);

            // Total bookings in period
            var allBookings = await _reportRepository.GetBookingsInRangeAsync(startDate, endDate, cancellationToken);

            var totalBookings = allBookings.Count;

            // Completed bookings (Status = "Completed")
            var completedBookings = allBookings.Count(b => b.Status == "Completed");

            // Canceled bookings (Status = "Cancelled")
            var canceledBookings = allBookings.Count(b => b.Status == "Cancelled");

            // Checked-in bookings (Checkedintime is not null)
            var checkedInBookings = allBookings.Count(b => b.Checkedintime != null);

            // Cancellation rate
            var cancellationRate = totalBookings > 0 ? (decimal)canceledBookings / totalBookings * 100 : 0;

            // Check-in rate (of completed bookings)
            var checkInRate = completedBookings > 0 ? (decimal)checkedInBookings / completedBookings * 100 : 0;

            // Average booking value
            var averageBookingValue = allBookings.Any() ? allBookings.Average(b => b.Totalamount ?? 0) : 0;

            // Peak hours (top 2)
            var peakHoursData = await _reportRepository.GetBookingsByHourAsync(startDate, endDate, cancellationToken);
            var peakHours = peakHoursData.Select(h => $"{h.Hour:D2}:00-{(h.Hour + 1):D2}:00").ToList();

            // Peak days (day of week - top 2)
            var peakDays = allBookings
                .GroupBy(b => b.Bookingtime!.Value.DayOfWeek)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .OrderByDescending(d => d.Count)
                .Take(2)
                .Select(d => d.Day.ToString())
                .ToList();

            // Conversion rate (completed / total)
            var conversionRate = totalBookings > 0 ? (decimal)completedBookings / totalBookings * 100 : 0;

            var result = new BookingStatisticsDTO
            {
                TotalBookings = totalBookings,
                CompletedBookings = completedBookings,
                CanceledBookings = canceledBookings,
                CheckedInBookings = checkedInBookings,
                CancellationRate = $"{Math.Round(cancellationRate, 1)}%",
                CheckInRate = $"{Math.Round(checkInRate, 1)}%",
                AverageBookingValue = Math.Round(averageBookingValue, 0),
                PeakHours = peakHours,
                PeakDays = peakDays,
                ConversionRate = $"{Math.Round(conversionRate, 0)}%"
            };

            return Result<BookingStatisticsDTO>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking statistics for {StartDate} to {EndDate}", query.StartDate, query.EndDate);
            return Result<BookingStatisticsDTO>.Failure("Failed to retrieve booking statistics. Invalid date format or query error.");
        }
    }

    public async Task<Result<PopularMovieDTO[]>> GetPopularMoviesAsync(PopularMoviesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var startDate = query.Period.ToLower() switch
            {
                "week" => DateTime.SpecifyKind(now.AddDays(-7), DateTimeKind.Unspecified),
                "month" => DateTime.SpecifyKind(now.AddDays(-30), DateTimeKind.Unspecified),
                "year" => DateTime.SpecifyKind(now.AddDays(-365), DateTimeKind.Unspecified),
                _ => DateTime.SpecifyKind(now.AddDays(-30), DateTimeKind.Unspecified)
            };

            // Get previous period for trend calculation
            var previousStartDate = query.Period.ToLower() switch
            {
                "week" => DateTime.SpecifyKind(now.AddDays(-14), DateTimeKind.Unspecified),
                "month" => DateTime.SpecifyKind(now.AddDays(-60), DateTimeKind.Unspecified),
                "year" => DateTime.SpecifyKind(now.AddDays(-730), DateTimeKind.Unspecified),
                _ => DateTime.SpecifyKind(now.AddDays(-60), DateTimeKind.Unspecified)
            };

            // Get movie stats from repository
            var popularMoviesData = await _reportRepository.GetPopularMoviesAsync(startDate, query.Limit, cancellationToken);

            // Get previous period revenue for trend calculation
            var previousPeriodRevenue = await _reportRepository.GetPreviousPeriodRevenueAsync(previousStartDate, startDate, cancellationToken);

            var result = popularMoviesData.Select((m, index) =>
            {
                var previousRevenue = previousPeriodRevenue.FirstOrDefault(r => r.MovieId == m.MovieId).Revenue;
                var trend = m.Revenue > previousRevenue ? "up" : m.Revenue < previousRevenue ? "down" : "stable";
                var occupancy = m.TotalSeats > 0 ? (decimal)m.BookedSeats / m.TotalSeats * 100 : 0;

                return new PopularMovieDTO
                {
                    Rank = index + 1,
                    MovieId = m.MovieId,
                    Title = m.Title,
                    TotalBookings = m.TotalBookings,
                    Revenue = m.Revenue,
                    AverageOccupancy = $"{Math.Round(occupancy, 0)}%",
                    Rating = m.AverageRating.HasValue ? Math.Round(m.AverageRating.Value, 1) : null,
                    Trend = trend
                };
            }).ToArray();

            return Result<PopularMovieDTO[]>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular movies for period {Period}", query.Period);
            return Result<PopularMovieDTO[]>.Failure("Failed to retrieve popular movies");
        }
    }

    public async Task<Result<CustomerAnalyticsDTO>> GetCustomerAnalyticsAsync(CustomerAnalyticsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var startDate = query.Period.ToLower() switch
            {
                "week" => DateTime.SpecifyKind(now.AddDays(-7), DateTimeKind.Unspecified),
                "month" => DateTime.SpecifyKind(now.AddDays(-30), DateTimeKind.Unspecified),
                "year" => DateTime.SpecifyKind(now.AddDays(-365), DateTimeKind.Unspecified),
                _ => DateTime.MinValue // "all"
            };

            // Total customers
            var totalCustomers = await _reportRepository.GetTotalCustomersCountAsync(cancellationToken);

            // New customers (User.Createdat in period)
            var newCustomers = await _reportRepository.GetNewCustomersCountAsync(startDate, cancellationToken);

            // Active customers (with bookings in period)
            var activeCustomers = query.Period == "all" 
                ? await _reportRepository.GetActiveCustomersCountAsync(DateTime.MinValue, cancellationToken)
                : await _reportRepository.GetActiveCustomersCountAsync(startDate, cancellationToken);

            // Retention rate (customers with repeat bookings)
            var customersWithRepeatBookings = query.Period == "all"
                ? await _reportRepository.GetCustomersWithRepeatBookingsCountAsync(DateTime.MinValue, cancellationToken)
                : await _reportRepository.GetCustomersWithRepeatBookingsCountAsync(startDate, cancellationToken);

            var retentionRate = totalCustomers > 0 ? (decimal)customersWithRepeatBookings / totalCustomers * 100 : 0;

            // Churn rate (inactive customers in last 90 days)
            var last90Days = DateTime.SpecifyKind(now.AddDays(-90), DateTimeKind.Unspecified);
            var inactiveCustomers = await _reportRepository.GetInactiveCustomersCountAsync(last90Days, cancellationToken);

            var churnRate = totalCustomers > 0 ? (decimal)inactiveCustomers / totalCustomers * 100 : 0;

            // Average lifetime value
            var lifetimeValues = await _reportRepository.GetAllCustomerLifetimeValuesAsync(cancellationToken);
            var averageLifetimeValue = lifetimeValues.Any() ? lifetimeValues.Average() : 0;

            // Top customers
            var topCustomersData = await _reportRepository.GetTopCustomersAsync(query.TopCustomersLimit, cancellationToken);
            var topCustomersList = topCustomersData.Select(c => new TopCustomerDTO
            {
                CustomerId = c.CustomerId,
                Fullname = c.Fullname,
                Email = c.Email,
                TotalBookings = c.TotalBookings,
                TotalSpent = c.TotalSpent,
                MemberSince = c.MemberSince
            }).ToList();

            // Demographics - Age
            var customersWithAge = await _reportRepository.GetCustomersWithDateOfBirthAsync(cancellationToken);

            var ageGroups = customersWithAge
                .GroupBy(c =>
                {
                    var age = DateTime.Now.Year - c.Dateofbirth!.Value.Year;
                    return age switch
                    {
                        >= 18 and < 25 => "18-24",
                        >= 25 and < 35 => "25-34",
                        >= 35 and < 45 => "35-44",
                        >= 45 => "45+",
                        _ => "Under 18"
                    };
                })
                .ToDictionary(
                    g => g.Key,
                    g => customersWithAge.Count > 0 
                        ? $"{Math.Round((decimal)g.Count() / customersWithAge.Count * 100, 0)}%" 
                        : "0%"
                );

            // Demographics - Gender
            var customersWithGender = await _reportRepository.GetCustomersWithGenderAsync(cancellationToken);

            var genderGroups = customersWithGender
                .GroupBy(c => c.Gender!.ToLower())
                .ToDictionary(
                    g => g.Key,
                    g => customersWithGender.Count > 0 
                        ? $"{Math.Round((decimal)g.Count() / customersWithGender.Count * 100, 0)}%" 
                        : "0%"
                );

            var result = new CustomerAnalyticsDTO
            {
                TotalCustomers = totalCustomers,
                NewCustomers = newCustomers,
                ActiveCustomers = activeCustomers,
                RetentionRate = $"{Math.Round(retentionRate, 0)}%",
                ChurnRate = $"{Math.Round(churnRate, 0)}%",
                AverageLifetimeValue = Math.Round(averageLifetimeValue, 0),
                TopCustomers = topCustomersList,
                Demographics = new DemographicsDTO
                {
                    Age = ageGroups,
                    Gender = genderGroups
                }
            };

            return Result<CustomerAnalyticsDTO>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer analytics for period {Period}", query.Period);
            return Result<CustomerAnalyticsDTO>.Failure("Failed to retrieve customer analytics");
        }
    }
}
