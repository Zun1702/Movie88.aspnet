namespace Movie88.Domain.Models
{
    public class OtpTokenModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string OtpType { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Helper methods
        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
        public bool IsValid() => !IsUsed && !IsExpired();
    }

    public static class OtpTypeConstants
    {
        public const string EmailVerification = "EmailVerification";
        public const string PasswordReset = "PasswordReset";
        public const string Login = "Login";
    }
}
