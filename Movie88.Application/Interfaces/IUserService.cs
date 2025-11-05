using Movie88.Application.DTOs.User;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserProfileGetDto>> GetCurrentUserAsync(int userId);
    Task<Result<UserProfileUpdateDto>> UpdateUserAsync(int id, int currentUserId, UpdateUserDto request);
}