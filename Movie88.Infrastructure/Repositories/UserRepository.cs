using Microsoft.EntityFrameworkCore;
using Movie88.Application.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Mappers;

namespace Movie88.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Userid == userId, cancellationToken);
            
            return entity?.ToModel();
        }

        public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            
            return entity?.ToModel();
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Users
                .Include(u => u.Role)
                .ToListAsync(cancellationToken);
            
            return entities.Select(e => e.ToModel());
        }

        public async Task AddAsync(UserModel user, CancellationToken cancellationToken = default)
        {
            var entity = user.ToEntity();
            await _context.Users.AddAsync(entity, cancellationToken);
            // Note: entity.Userid will be populated after SaveChanges
            // But we need to sync it back to the model, so we'll do it in AuthService
        }

        public void Update(UserModel user)
        {
            // Detach any existing tracked entity with the same key to avoid conflicts
            var existingEntity = _context.Users.Local.FirstOrDefault(u => u.Userid == user.UserId);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            var entity = user.ToEntity();
            _context.Users.Update(entity);
        }

        public void Delete(UserModel user)
        {
            var entity = user.ToEntity();
            _context.Users.Remove(entity);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<UserModel?> GetUserWithRoleByIdAsync(int userId)
        {
            var entity = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Userid == userId);
            
            return entity?.ToModel();
        }
    }
}
