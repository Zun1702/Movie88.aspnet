using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Auth
{
    /// <summary>
    /// Request để gửi OTP qua email
    /// </summary>
    public class SendOtpRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP type is required")]
        public string OtpType { get; set; } = string.Empty; // EmailVerification, PasswordReset, Login
    }

    /// <summary>
    /// Request để verify OTP
    /// </summary>
    public class VerifyOtpRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP code must be 6 digits")]
        public string OtpCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP type is required")]
        public string OtpType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response sau khi gửi OTP
    /// </summary>
    public class SendOtpResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string OtpType { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int ExpiresInMinutes { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response sau khi verify OTP thành công
    /// </summary>
    public class VerifyOtpResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public DateTime VerifiedAt { get; set; }
        public string Message { get; set; } = string.Empty;
        
        // Optional: Return token if this is login OTP
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }

    /// <summary>
    /// Request để resend OTP
    /// </summary>
    public class ResendOtpRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP type is required")]
        public string OtpType { get; set; } = string.Empty;
    }
}
