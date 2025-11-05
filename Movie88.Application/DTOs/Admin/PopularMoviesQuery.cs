using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Query parameters for popular movies report
/// </summary>
public class PopularMoviesQuery
{
    public string Period { get; set; } = "month"; // "week", "month", "year"

    [Range(1, 50)]
    public int Limit { get; set; } = 10;
}
