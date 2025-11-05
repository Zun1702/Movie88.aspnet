using System;
using System.Collections.Generic;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Daily or Monthly revenue report
/// </summary>
public class RevenueReportDTO
{
    public string Period { get; set; } = string.Empty; // "2025-11-04" for daily, "2025-11" for monthly
    public decimal TotalRevenue { get; set; }
    public int TotalBookings { get; set; }
    public decimal AverageTicketPrice { get; set; }
    public RevenueBreakdownDTO Breakdown { get; set; } = new();
    public List<RevenueByMovieDTO> ByMovie { get; set; } = new();
    public List<RevenueByCinemaDTO> ByCinema { get; set; } = new();
    public List<RevenueByHourDTO>? ByHour { get; set; } // Only for daily reports
}

public class RevenueBreakdownDTO
{
    public decimal TicketSales { get; set; }
    public decimal Concessions { get; set; }
}

public class RevenueByMovieDTO
{
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}

public class RevenueByCinemaDTO
{
    public int CinemaId { get; set; }
    public string CinemaName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}

public class RevenueByHourDTO
{
    public string Hour { get; set; } = string.Empty; // "10:00-11:00"
    public decimal Revenue { get; set; }
    public int Bookings { get; set; }
}
