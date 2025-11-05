using System;
using System.Collections.Generic;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Popular movies ranking
/// </summary>
public class PopularMovieDTO
{
    public int Rank { get; set; }
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal Revenue { get; set; }
    public string AverageOccupancy { get; set; } = "0%"; // Percentage string e.g., "34%"
    public decimal? Rating { get; set; } // Average rating from reviews
    public string Trend { get; set; } = string.Empty; // "up", "down", "stable"
}
