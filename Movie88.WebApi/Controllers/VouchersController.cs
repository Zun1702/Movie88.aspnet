using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Vouchers;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _voucherService;
    private readonly ICustomerRepository _customerRepository;

    public VouchersController(
        IVoucherService voucherService,
        ICustomerRepository customerRepository)
    {
        _voucherService = voucherService;
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Validate a voucher code for a specific booking
    /// </summary>
    /// <param name="request">Validation request containing voucher code and booking ID</param>
    /// <returns>Voucher details with applicable discount</returns>
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateVoucher([FromBody] ValidateVoucherRequestDTO request)
    {
        try
        {
            // Get customer ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid user credentials" });
            }

            var customer = await _customerRepository.GetByUserIdAsync(userId);
            if (customer == null)
            {
                return Unauthorized(new { message = "Customer not found" });
            }

            var result = await _voucherService.ValidateVoucherAsync(request, customer.Customerid);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while validating the voucher",
                error = ex.Message
            });
        }
    }
}
