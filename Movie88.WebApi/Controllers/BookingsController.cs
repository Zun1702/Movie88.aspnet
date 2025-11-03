using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;
using System.Security.Claims;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Get current user's bookings with optional status filter and pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <param name="status">Filter by status: pending, confirmed, cancelled, completed (optional)</param>
    /// <returns>Paginated list of user's bookings</returns>
    [HttpGet("my-bookings")]
    public async Task<IActionResult> GetMyBookings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        // Extract userId from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new
            {
                success = false,
                statusCode = 401,
                message = "Unauthorized - Invalid token"
            });
        }

        var result = await _bookingService.GetMyBookingsAsync(userId, page, pageSize, status);

        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                404 => NotFound(new
                {
                    success = false,
                    statusCode = 404,
                    message = result.Message
                }),
                _ => BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    message = result.Message
                })
            };
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = result.Message,
            data = result.Data
        });
    }
}
