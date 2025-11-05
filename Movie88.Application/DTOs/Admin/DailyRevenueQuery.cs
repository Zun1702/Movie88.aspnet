using System;
using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Query parameters for daily revenue report
/// Example: ?date=2025-11-04
/// </summary>
public class DailyRevenueQuery
{
    /// <summary>
    /// Date in format yyyy-MM-dd (e.g., 2025-11-04)
    /// </summary>
    [Required]
    public string Date { get; set; } = string.Empty;
    
    /// <summary>
    /// Parse string date to DateOnly
    /// </summary>
    public DateOnly GetDateOnly()
    {
        return DateOnly.Parse(Date);
    }
}
