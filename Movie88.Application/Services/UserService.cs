using AutoMapper;
using Movie88.Application.DTOs.User;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Models;

namespace Movie88.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly Movie88.Application.Interfaces.IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        Movie88.Application.Interfaces.IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserProfileGetDto>> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetUserWithRoleByIdAsync(userId);

        if (user == null)
        {
            return Result<UserProfileGetDto>.NotFound("User not found");
        }

        var userProfile = new UserProfileGetDto
        {
            Userid = user.UserId,
            Fullname = user.Fullname,
            Email = user.Email,
            Phone = user.Phone,
            Roleid = user.Roleid,
            Rolename = user.Role?.Rolename ?? string.Empty,
            Createdat = user.Createdat,
            Updatedat = user.Updatedat
        };

        return Result<UserProfileGetDto>.Success(
            userProfile, 
            "User information retrieved successfully"
        );
    }

    public async Task<Result<UserProfileUpdateDto>> UpdateUserAsync(int id, int currentUserId, UpdateUserDto request)
    {
        if (id != currentUserId)
        {
            return Result<UserProfileUpdateDto>.Error("Forbidden", 403);
        }

        var user = await _userRepository.GetUserWithRoleByIdAsync(id);

        if (user == null)
        {
            return Result<UserProfileUpdateDto>.NotFound("User not found");
        }

        user.Fullname = request.Fullname;
        user.Phone = request.Phone;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var userProfile = new UserProfileUpdateDto
        {
            Userid = user.UserId,
            Fullname = user.Fullname,
            Email = user.Email,
            Phone = user.Phone,
            Rolename = user.Role?.Rolename ?? string.Empty,
            Updatedat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        return Result<UserProfileUpdateDto>.Success(
            userProfile,
            "User information updated successfully"
        );
    }
}