namespace Movie88.Application.DTOs.Authentication;

public class ForgotPasswordResponseDTO
{
    public string Email { get; set; } = string.Empty;
    public string OtpType { get; set; } = "PasswordReset";
    public DateTime? ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
}
