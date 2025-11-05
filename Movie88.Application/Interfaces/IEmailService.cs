using Movie88.Application.DTOs.Email;

namespace Movie88.Application.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Send OTP email for verification
    /// </summary>
    Task<bool> SendOtpEmailAsync(string toEmail, string otpCode, string otpType);
    
    /// <summary>
    /// Send welcome email after registration
    /// </summary>
    Task<bool> SendWelcomeEmailAsync(string toEmail, string fullname);
    
    /// <summary>
    /// Send password reset success confirmation
    /// </summary>
    Task<bool> SendPasswordResetConfirmationAsync(string toEmail, string fullname);
    
    /// <summary>
    /// Send booking confirmation email with QR code after successful payment
    /// </summary>
    Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmailDTO dto);
}
