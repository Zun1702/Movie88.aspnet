using Microsoft.EntityFrameworkCore;
using Movie88.Application.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Mappers;

namespace Movie88.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshTokenModel?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var entity = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.Revoked == false, cancellationToken);
            
            return entity?.ToModel();
        }

        public async Task<RefreshTokenModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
            
            return entity?.ToModel();
        }

        public async Task<IEnumerable<RefreshTokenModel>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var entities = await _context.UserRefreshTokens
                .Where(rt => rt.UserId == userId && rt.Revoked == false)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync(cancellationToken);
            
            return entities.Select(e => e.ToModel());
        }

        public async Task AddAsync(RefreshTokenModel refreshToken, CancellationToken cancellationToken = default)
        {
            var entity = refreshToken.ToEntity();
            await _context.UserRefreshTokens.AddAsync(entity, cancellationToken);
            refreshToken.Id = (int)entity.Id; // Update ID after insert
        }

        public void Update(RefreshTokenModel refreshToken)
        {
            var entity = refreshToken.ToEntity();
            _context.UserRefreshTokens.Update(entity);
        }

        public void Delete(RefreshTokenModel refreshToken)
        {
            var entity = refreshToken.ToEntity();
            _context.UserRefreshTokens.Remove(entity);
        }

        public async Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken = default)
        {
            var tokens = await _context.UserRefreshTokens
                .Where(rt => rt.UserId == userId && rt.Revoked == false)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.Revoked = true;
                token.UpdatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            }

            _context.UserRefreshTokens.UpdateRange(tokens);
        }
    }
}
