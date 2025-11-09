using Movie88.Application.DTOs.Auth;
using Movie88.Application.DTOs.Authentication;

namespace Movie88.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, CancellationToken cancellationToken = default);
        Task<ForgotPasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordRequestDTO request, CancellationToken cancellationToken = default);
        Task<ResetPasswordResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO request, string? ipAddress = null, string? userAgent = null, CancellationToken cancellationToken = default);
        Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<LoginResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO request, CancellationToken cancellationToken = default);
    }
}
