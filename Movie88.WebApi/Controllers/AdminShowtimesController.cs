using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Showtimes;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminShowtimesController : ControllerBase
{
    private readonly IAdminShowtimeService _adminShowtimeService;

    public AdminShowtimesController(IAdminShowtimeService adminShowtimeService)
    {
        _adminShowtimeService = adminShowtimeService;
    }

    /// <summary>
    /// Create a single showtime (Admin only)
    /// POST /api/admin/showtimes
    /// </summary>
    [HttpPost("showtimes")]
    public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeDto request)
    {
        var result = await _adminShowtimeService.CreateShowtimeAsync(request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { 
                success = false,
                message = result.Message 
            });
        }

        return StatusCode(201, new
        {
            success = true,
            message = result.Message,
            data = new
            {
                showtimeId = result.Data!.ShowtimeId,
                movieTitle = result.Data.MovieTitle,
                startTime = result.Data.StartTime,
                endTime = result.Data.EndTime,
                availableSeats = result.Data.AvailableSeats
            }
        });
    }

    /// <summary>
    /// Create multiple showtimes in bulk (weekly scheduling)
    /// POST /api/admin/showtimes/bulk
    /// </summary>
    [HttpPost("showtimes/bulk")]
    public async Task<IActionResult> CreateBulkShowtimes([FromBody] BulkCreateShowtimeDto request)
    {
        var result = await _adminShowtimeService.CreateBulkShowtimesAsync(request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { 
                success = false,
                message = result.Message 
            });
        }

        return StatusCode(201, new
        {
            success = true,
            message = result.Message,
            data = new
            {
                created = result.Data!.Created,
                skipped = result.Data.Skipped,
                failed = result.Data.Failed,
                details = result.Data.Details
            }
        });
    }
}
