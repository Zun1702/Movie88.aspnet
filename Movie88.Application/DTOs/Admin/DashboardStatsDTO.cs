using System;
using System.Collections.Generic;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Dashboard overview statistics
/// </summary>
public class DashboardStatsDTO
{
    public decimal TodayRevenue { get; set; }
    public int TodayBookings { get; set; }
    public int ActiveMovies { get; set; }
    public int ActiveCustomers { get; set; }
    public List<PopularMovieItemDTO> PopularMovies { get; set; } = new();
    public List<UpcomingShowtimeDTO> UpcomingShowtimes { get; set; } = new();
}

public class PopularMovieItemDTO
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int TotalBookings { get; set; }
    public decimal Revenue { get; set; }
}

public class UpcomingShowtimeDTO
{
    public int ShowtimeId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string CinemaName { get; set; } = string.Empty;
    public string AuditoriumName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public decimal Price { get; set; }
    public string Format { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
}
