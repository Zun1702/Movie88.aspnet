using Movie88.Domain.Models;

namespace Movie88.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(UserModel user, CancellationToken cancellationToken = default);
        void Update(UserModel user);
        void Delete(UserModel user);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

        // Admin user management methods
        Task<(List<UserWithAggregatesModel> Users, int TotalCount)> GetUsersWithAggregatesAsync(
            string? role,
            string? status,
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int?> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken = default);
    }
}
