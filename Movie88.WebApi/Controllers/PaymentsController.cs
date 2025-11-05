using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Payments;
using Movie88.Application.Interfaces;
using System.Security.Claims;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ICustomerService _customerService;

    public PaymentsController(
        IPaymentService paymentService,
        ICustomerService customerService)
    {
        _paymentService = paymentService;
        _customerService = customerService;
    }

    /// <summary>
    /// Create VNPay payment URL
    /// </summary>
    [Authorize]
    [HttpPost("vnpay/create")]
    public async Task<IActionResult> CreateVNPayPayment([FromBody] CreatePaymentRequestDTO request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var customerResult = await _customerService.GetProfileByUserIdAsync(userId);

            if (!customerResult.IsSuccess || customerResult.Data == null)
            {
                return BadRequest(new { message = "Customer not found" });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            var result = await _paymentService.CreateVNPayPaymentAsync(
                request,
                customerResult.Data.Customerid,
                ipAddress);

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Payment URL created successfully",
                data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating payment", error = ex.Message });
        }
    }

    /// <summary>
    /// VNPay callback handler (redirect after payment)
    /// </summary>
    [HttpGet("vnpay/callback")]
    public async Task<IActionResult> VNPayCallback()
    {
        try
        {
            var parameters = Request.Query.ToDictionary(
                x => x.Key,
                x => x.Value.ToString()
            );

            var (success, message, bookingId) = await _paymentService.ProcessVNPayCallbackAsync(parameters);

            // Return HTML that redirects to app deep link
            var status = success ? "success" : "failed";
            var deepLink = $"movieapp://payment/result?status={status}&bookingid={bookingId}&message={Uri.EscapeDataString(message)}";

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='refresh' content='0;url={deepLink}' />
    <title>Payment Result</title>
</head>
<body>
    <p>{(success ? "Payment successful" : "Payment failed")}. Redirecting to app...</p>
    <p>If you are not redirected automatically, <a href='{deepLink}'>click here</a>.</p>
</body>
</html>";

            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            var deepLink = $"movieapp://payment/result?status=error&message={Uri.EscapeDataString(ex.Message)}";
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='refresh' content='0;url={deepLink}' />
    <title>Error</title>
</head>
<body>
    <p>An error occurred. Redirecting to app...</p>
</body>
</html>";

            return Content(html, "text/html");
        }
    }

    /// <summary>
    /// VNPay IPN (Instant Payment Notification)
    /// </summary>
    [HttpPost("vnpay/ipn")]
    public async Task<IActionResult> VNPayIPN()
    {
        try
        {
            var parameters = Request.Form.ToDictionary(
                x => x.Key,
                x => x.Value.ToString()
            );

            var (rspCode, message) = await _paymentService.ProcessVNPayIPNAsync(parameters);

            return Ok(new { RspCode = rspCode, Message = message });
        }
        catch (Exception)
        {
            return Ok(new { RspCode = "99", Message = "Unknown error" });
        }
    }

    /// <summary>
    /// Get payment details
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentDetails(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var customerResult = await _customerService.GetProfileByUserIdAsync(userId);

            if (!customerResult.IsSuccess || customerResult.Data == null)
            {
                return BadRequest(new { message = "Customer not found" });
            }

            var payment = await _paymentService.GetPaymentByIdAsync(id, customerResult.Data.Customerid);

            if (payment == null)
            {
                return NotFound(new { message = "Payment not found" });
            }

            return Ok(new
            {
                success = true,
                statusCode = 200,
                message = "Payment details retrieved successfully",
                data = payment
            });
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, new { message = "This payment does not belong to you" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving payment", error = ex.Message });
        }
    }
}
