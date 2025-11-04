using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Bookings;
using Movie88.Application.Interfaces;
using System.Security.Claims;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ICustomerService _customerService;

    public BookingsController(IBookingService bookingService, ICustomerService customerService)
    {
        _bookingService = bookingService;
        _customerService = customerService;
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

    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingRequestDTO request,
        CancellationToken cancellationToken)
    {
        // Extract userid from JWT token
        var useridClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(useridClaim) || !int.TryParse(useridClaim, out int userid))
        {
            return Unauthorized(new
            {
                success = false,
                statusCode = 401,
                message = "User not authenticated",
                data = (object?)null
            });
        }

        // Get customerid from userid
        var customerResult = await _customerService.GetProfileByUserIdAsync(userid);
        if (!customerResult.IsSuccess || customerResult.Data == null)
        {
            return NotFound(new
            {
                success = false,
                statusCode = 404,
                message = "Customer profile not found",
                data = (object?)null
            });
        }

        // Create booking
        var booking = await _bookingService.CreateBookingAsync(
            customerResult.Data.Customerid, 
            request, 
            cancellationToken);

        if (booking == null)
        {
            return BadRequest(new
            {
                success = false,
                statusCode = 400,
                message = "Failed to create booking. Showtime not found, already started, or seats unavailable",
                data = (object?)null
            });
        }

        return CreatedAtAction(
            nameof(CreateBooking),
            new { id = booking.Bookingid },
            new
            {
                success = true,
                statusCode = 201,
                message = "Booking created successfully",
                data = booking
            });
    }
}
