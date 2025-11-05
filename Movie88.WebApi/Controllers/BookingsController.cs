using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Bookings;
using Movie88.Application.DTOs.Combos;
using Movie88.Application.DTOs.Vouchers;
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
        try
        {
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
                    message = "Failed to create booking",
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                statusCode = 400,
                message = ex.Message,
                data = (object?)null
            });
        }
    }

    /// <summary>
    /// Add combos to an existing booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">List of combos with quantities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated booking with combos</returns>
    [HttpPost("{id}/add-combos")]
    public async Task<IActionResult> AddCombos(
        int id,
        [FromBody] AddCombosRequestDTO request,
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

        try
        {
            var updatedBooking = await _bookingService.AddCombosToBookingAsync(
                id,
                customerResult.Data.Customerid,
                request,
                cancellationToken);

            if (updatedBooking == null)
            {
                return NotFound(new
                {
                    success = false,
                    statusCode = 404,
                    message = "Booking not found",
                    data = (object?)null
                });
            }

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Combos added to booking successfully",
                data = updatedBooking
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                success = false,
                statusCode = 403,
                message = ex.Message,
                data = (object?)null
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                statusCode = 400,
                message = ex.Message,
                data = (object?)null
            });
        }
    }

    /// <summary>
    /// Apply a voucher code to a booking
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="request">Request containing voucher code</param>
    /// <returns>Updated booking information with discount applied</returns>
    [HttpPost("{id}/apply-voucher")]
    public async Task<IActionResult> ApplyVoucher(int id, [FromBody] ApplyVoucherRequestDTO request)
    {
        try
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

            // Get customer from userId
            var customerResult = await _customerService.GetProfileByUserIdAsync(userId);
            if (!customerResult.IsSuccess || customerResult.Data == null)
            {
                return NotFound(new
                {
                    success = false,
                    statusCode = 404,
                    message = "Customer profile not found"
                });
            }

            var result = await _bookingService.ApplyVoucherToBookingAsync(id, customerResult.Data.Customerid, request);

            if (result == null)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    message = "Failed to apply voucher"
                });
            }

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Voucher applied successfully",
                data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                success = false,
                statusCode = 403,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                statusCode = 400,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                statusCode = 500,
                message = "An error occurred while applying voucher",
                error = ex.Message
            });
        }
    }
}
