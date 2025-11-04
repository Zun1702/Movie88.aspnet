using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/auditoriums")]
public class AuditoriumsController : ControllerBase
{
    private readonly IAuditoriumService _auditoriumService;

    public AuditoriumsController(IAuditoriumService auditoriumService)
    {
        _auditoriumService = auditoriumService;
    }

    [HttpGet("{id}/seats")]
    public async Task<IActionResult> GetAuditoriumSeats(
        [FromRoute] int id, 
        [FromQuery] int? showtimeId,
        CancellationToken cancellationToken)
    {
        var result = await _auditoriumService.GetAuditoriumSeatsAsync(id, showtimeId, cancellationToken);

        if (result == null)
        {
            return NotFound(new
            {
                success = false,
                statusCode = 404,
                message = "Auditorium not found",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Auditorium seats retrieved successfully",
            data = result
        });
    }
}
