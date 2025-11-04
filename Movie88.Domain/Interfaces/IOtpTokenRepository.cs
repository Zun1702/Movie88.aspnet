using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces
{
    public interface IOtpTokenRepository
    {
        /// <summary>
        /// Create new OTP token
        /// </summary>
        Task<OtpTokenModel> CreateAsync(OtpTokenModel otpToken);

        /// <summary>
        /// Get OTP by code, type and email
        /// </summary>
        Task<OtpTokenModel?> GetByCodeAsync(string otpCode, string otpType, string email);

        /// <summary>
        /// Mark OTP as used
        /// </summary>
        Task<bool> MarkAsUsedAsync(int otpId, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Get active OTP for user (not used, not expired)
        /// </summary>
        Task<OtpTokenModel?> GetActiveOtpAsync(int userId, string otpType);

        /// <summary>
        /// Invalidate all OTPs for user of specific type
        /// </summary>
        Task<bool> InvalidateUserOtpsAsync(int userId, string otpType);

        /// <summary>
        /// Delete expired OTPs (cleanup)
        /// </summary>
        Task<int> DeleteExpiredOtpsAsync();

        /// <summary>
        /// Get OTP count for user in timespan (rate limiting)
        /// </summary>
        Task<int> GetOtpCountAsync(int userId, string otpType, TimeSpan timespan);
    }
}
