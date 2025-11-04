using Movie88.Application.DTOs.Auth;

namespace Movie88.Application.Interfaces
{
    public interface IOtpService
    {
        /// <summary>
        /// Send OTP to email (EmailVerification, PasswordReset, Login)
        /// </summary>
        Task<SendOtpResponseDTO> SendOtpAsync(SendOtpRequestDTO request, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Verify OTP code
        /// </summary>
        Task<VerifyOtpResponseDTO> VerifyOtpAsync(VerifyOtpRequestDTO request, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Resend OTP (rate limited)
        /// </summary>
        Task<SendOtpResponseDTO> ResendOtpAsync(ResendOtpRequestDTO request, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Generate 6-digit OTP code
        /// </summary>
        string GenerateOtpCode();

        /// <summary>
        /// Send OTP email (mock or real implementation)
        /// </summary>
        Task<bool> SendOtpEmailAsync(string email, string otpCode, string otpType);
    }
}
