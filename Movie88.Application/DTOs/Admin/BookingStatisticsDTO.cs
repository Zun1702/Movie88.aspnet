using System;
using System.Collections.Generic;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Booking statistics with completion and check-in rates
/// </summary>
public class BookingStatisticsDTO
{
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CanceledBookings { get; set; }
    public int CheckedInBookings { get; set; }
    public string CancellationRate { get; set; } = "0%"; // Percentage string e.g., "5.6%"
    public string CheckInRate { get; set; } = "0%"; // Percentage string e.g., "94.9%"
    public decimal AverageBookingValue { get; set; }
    public List<string> PeakHours { get; set; } = new();
    public List<string> PeakDays { get; set; } = new();
    public string ConversionRate { get; set; } = "0%"; // Percentage string e.g., "78%"
}
