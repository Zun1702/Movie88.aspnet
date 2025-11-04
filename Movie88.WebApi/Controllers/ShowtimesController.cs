using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/showtimes")]
public class ShowtimesController : ControllerBase
{
    private readonly IShowtimeService _showtimeService;

    public ShowtimesController(IShowtimeService showtimeService)
    {
        _showtimeService = showtimeService;
    }

    /// <summary>
    /// Get showtimes grouped by date and cinema for a specific movie
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Showtimes grouped by date and cinema</returns>
    [HttpGet("by-movie/{movieId}")]
    public async Task<IActionResult> GetShowtimesByMovie(
        [FromRoute] int movieId,
        CancellationToken cancellationToken = default)
    {
        var showtimes = await _showtimeService.GetShowtimesByMovieAsync(movieId, cancellationToken);

        if (showtimes == null)
        {
            return NotFound(new
            {
                success = false,
                statusCode = 404,
                message = $"No showtimes found for movie ID {movieId}",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Showtimes retrieved successfully",
            data = showtimes
        });
    }

    /// <summary>
    /// Get detailed information for a specific showtime
    /// </summary>
    /// <param name="id">Showtime ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Showtime details</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetShowtimeById(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var showtime = await _showtimeService.GetShowtimeByIdAsync(id, cancellationToken);

        if (showtime == null)
        {
            return NotFound(new
            {
                success = false,
                statusCode = 404,
                message = $"Showtime with ID {id} not found",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Showtime retrieved successfully",
            data = showtime
        });
    }

    /// <summary>
    /// Get showtimes by date with optional cinema and movie filters
    /// </summary>
    /// <param name="date">Date in yyyy-MM-dd format</param>
    /// <param name="cinemaid">Optional cinema ID filter</param>
    /// <param name="movieid">Optional movie ID filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Showtimes grouped by date and cinema</returns>
    [HttpGet("by-date")]
    public async Task<IActionResult> GetShowtimesByDate(
        [FromQuery] DateTime date,
        [FromQuery] int? cinemaid = null,
        [FromQuery] int? movieid = null,
        CancellationToken cancellationToken = default)
    {
        var showtimes = await _showtimeService.GetShowtimesByDateAsync(date, cinemaid, movieid, cancellationToken);

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Showtimes retrieved successfully",
            data = showtimes
        });
    }

    /// <summary>
    /// Get available seats count for a specific showtime
    /// </summary>
    /// <param name="id">Showtime ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Available seats count</returns>
    [HttpGet("{id}/available-seats")]
    public async Task<IActionResult> GetAvailableSeats(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var availableSeats = await _showtimeService.GetAvailableSeatsAsync(id, cancellationToken);

        if (availableSeats == -1)
        {
            return NotFound(new
            {
                success = false,
                statusCode = 404,
                message = $"Showtime with ID {id} not found",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Available seats retrieved successfully",
            data = new
            {
                showtimeid = id,
                availableSeats = availableSeats
            }
        });
    }
}
