using Movie88.Application.DTOs.Admin;
using Movie88.Application.DTOs.Common;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces
{
    public interface IAdminService
    {
        Task<Result<PagedResultDTO<UserListItemDTO>>> GetUsersAsync(GetUsersQuery query);
        Task<Result<UserDTO>> CreateUserAsync(CreateUserCommand command);
        Task<Result<UserDTO>> UpdateUserRoleAsync(int userId, UpdateUserRoleCommand command, int currentAdminId);
        Task<Result<UserDTO>> BanUserAsync(int userId, BanUserCommand command, int currentAdminId);
    }
}
