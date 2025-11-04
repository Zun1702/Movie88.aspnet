using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Auth;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using System.Security.Claims;
using HandlerResponse = Movie88.Application.HandlerResponse.Response<object>;

namespace Movie88.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, IOtpService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Register a new user (does not return tokens - email verification required)
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(Response<RegisterResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RegisterAsync(request, cancellationToken);
                return Ok(new Response<RegisterResponseDTO>(
                    message: response.Message,
                    status: 200,
                    data: response
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new Response(
                    message: ex.Message,
                    status: 400
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(
                    message: ex.Message,
                    status: 400
                ));
            }
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
                return Ok(new { success = result, message = "Password changed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset OTP (sends OTP to email)
        /// </summary>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request, CancellationToken cancellationToken)
        {
            await _authService.ForgotPasswordAsync(request, cancellationToken);
            return Ok(new Response(
                message: "If the email exists, an OTP has been sent. Please check your email.",
                status: 200
            ));
        }

        /// <summary>
        /// Reset password with OTP verification
        /// </summary>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(Response<ResetPasswordResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request, CancellationToken cancellationToken)
        {
            try
            {
                // Extract IP address and User-Agent for audit trail
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();
                
                var response = await _authService.ResetPasswordAsync(request, ipAddress, userAgent, cancellationToken);
                
                if (!response.Success)
                {
                    return BadRequest(new Response(
                        message: response.Message,
                        status: 400
                    ));
                }

                return Ok(new Response<ResetPasswordResponseDTO>(
                    message: response.Message,
                    status: 200,
                    data: response
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(
                    message: ex.Message,
                    status: 400
                ));
            }
        }

        /// <summary>
        /// Logout and revoke refresh token
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDTO request, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(request.RefreshToken, cancellationToken);
            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Send OTP to email for verification
        /// </summary>
        [HttpPost("send-otp")]
        [ProducesResponseType(typeof(SendOtpResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDTO request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                
                var result = await _otpService.SendOtpAsync(request, ipAddress, userAgent);
                
                return Ok(new
                {
                    success = true,
                    statusCode = 200,
                    message = "OTP sent successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Verify OTP code
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(VerifyOtpResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDTO request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                
                var result = await _otpService.VerifyOtpAsync(request, ipAddress, userAgent);
                
                return Ok(new
                {
                    success = true,
                    statusCode = 200,
                    message = "OTP verified successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Resend OTP code
        /// </summary>
        [HttpPost("resend-otp")]
        [ProducesResponseType(typeof(SendOtpResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDTO request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                
                var result = await _otpService.ResendOtpAsync(request, ipAddress, userAgent);
                
                return Ok(new
                {
                    success = true,
                    statusCode = 200,
                    message = "OTP resent successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    message = ex.Message
                });
            }
        }
    }
}
