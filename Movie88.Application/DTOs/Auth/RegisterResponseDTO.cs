namespace Movie88.Application.DTOs.Auth;

public class RegisterResponseDTO
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool OtpSent { get; set; }
    public DateTime? OtpExpiresAt { get; set; }
}
