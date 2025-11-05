using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Query parameters for monthly revenue report
/// </summary>
public class MonthlyRevenueQuery
{
    [Required]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required]
    [Range(2020, 2100)]
    public int Year { get; set; }
}
