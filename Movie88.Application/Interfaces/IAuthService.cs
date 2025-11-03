using Movie88.Application.DTOs.Auth;

namespace Movie88.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
        Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, CancellationToken cancellationToken = default);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request, CancellationToken cancellationToken = default);
        Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
