namespace Movie88.Application.DTOs.Auth
{
    public class ResetPasswordResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
