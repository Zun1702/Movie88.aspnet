using Movie88.Domain.Models;

namespace Movie88.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenModel?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<RefreshTokenModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<RefreshTokenModel>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshTokenModel refreshToken, CancellationToken cancellationToken = default);
        void Update(RefreshTokenModel refreshToken);
        void Delete(RefreshTokenModel refreshToken);
        Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default);
    }
}
