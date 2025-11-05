using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

/// <summary>
/// Cinemas management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CinemasController : ControllerBase
{
    private readonly ICinemaService _cinemaService;

    public CinemasController(ICinemaService cinemaService)
    {
        _cinemaService = cinemaService;
    }

    /// <summary>
    /// Get all cinemas, optionally filtered by city
    /// </summary>
    /// <param name="city">Optional city filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of cinemas</returns>
    [HttpGet]
    public async Task<IActionResult> GetCinemas([FromQuery] string? city = null, CancellationToken cancellationToken = default)
    {
        var cinemas = await _cinemaService.GetCinemasAsync(city, cancellationToken);

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Cinemas retrieved successfully",
            data = cinemas
        });
    }
}
