using System;
using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Query parameters for booking statistics
/// Example: ?startDate=2025-11-01&endDate=2025-11-30
/// </summary>
public class BookingStatisticsQuery
{
    /// <summary>
    /// Start date in format yyyy-MM-dd (e.g., 2025-11-01)
    /// </summary>
    [Required]
    public string StartDate { get; set; } = string.Empty;

    /// <summary>
    /// End date in format yyyy-MM-dd (e.g., 2025-11-30)
    /// </summary>
    [Required]
    public string EndDate { get; set; } = string.Empty;
    
    /// <summary>
    /// Parse string start date to DateOnly
    /// </summary>
    public DateOnly GetStartDateOnly()
    {
        return DateOnly.Parse(StartDate);
    }
    
    /// <summary>
    /// Parse string end date to DateOnly
    /// </summary>
    public DateOnly GetEndDateOnly()
    {
        return DateOnly.Parse(EndDate);
    }
}
