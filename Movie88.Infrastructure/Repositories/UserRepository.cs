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

        public async Task<(List<UserWithAggregatesModel> Users, int TotalCount)> GetUsersWithAggregatesAsync(
            string? role,
            string? status,
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            // Filter by role
            if (!string.IsNullOrWhiteSpace(role) && role.ToLower() != "all")
            {
                query = query.Where(u => u.Role.Rolename.ToLower() == role.ToLower());
            }

            // Filter by status (active/inactive)
            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
            {
                bool isActive = status.ToLower() == "active";
                query = query.Where(u => u.Isactive == isActive);
            }

            // Search by email or fullname
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Email.ToLower().Contains(search.ToLower()) ||
                    u.Fullname.ToLower().Contains(search.ToLower()));
            }

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and join with aggregated data
            var usersWithAggregates = await query
                .OrderByDescending(u => u.Createdat)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .GroupJoin(
                    _context.Customers,
                    user => user.Userid,
                    customer => customer.Userid,
                    (user, customers) => new { user, customer = customers.FirstOrDefault() }
                )
                .Select(x => new UserWithAggregatesModel
                {
                    UserId = x.user.Userid,
                    Fullname = x.user.Fullname,
                    Email = x.user.Email,
                    Phone = x.user.Phone,
                    Roleid = x.user.Roleid,
                    RoleName = x.user.Role.Rolename,
                    Createdat = x.user.Createdat,
                    IsVerified = x.user.Isverified,
                    IsActive = x.user.Isactive,
                    TotalBookings = x.customer != null
                        ? _context.Bookings.Count(b => b.Customerid == x.customer.Customerid)
                        : 0,
                    TotalSpent = x.customer != null
                        ? _context.Bookings
                            .Where(b => b.Customerid == x.customer.Customerid && b.Status == "Confirmed")
                            .Sum(b => (decimal?)b.Totalamount) ?? 0
                        : 0
                })
                .ToListAsync(cancellationToken);

            return (usersWithAggregates, totalCount);
        }

        public async Task<int?> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Rolename.ToLower() == roleName.ToLower(), cancellationToken);

            return role?.Roleid;
        }
    }
}
