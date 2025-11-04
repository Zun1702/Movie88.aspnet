using Movie88.Application.DTOs.Admin;
using Movie88.Application.DTOs.Common;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Models;
using System.Text.RegularExpressions;

namespace Movie88.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHashingService = passwordHashingService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResultDTO<UserListItemDTO>>> GetUsersAsync(GetUsersQuery query)
        {
            var (users, totalCount) = await _userRepository.GetUsersWithAggregatesAsync(
                query.Role,
                query.Status,
                query.Search,
                query.Page,
                query.PageSize);

            // Map to DTO (aggregation already done in Repository)
            var userListItems = users.Select(user => new UserListItemDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Fullname = user.Fullname,
                Role = user.RoleName,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                RegisteredAt = user.Createdat,
                TotalBookings = user.TotalBookings,
                TotalSpent = user.TotalSpent,
                LastLogin = null
            }).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var pagedResult = new PagedResultDTO<UserListItemDTO>
            {
                Items = userListItems,
                CurrentPage = query.Page,
                PageSize = query.PageSize,
                TotalPages = totalPages,
                TotalItems = totalCount,
                HasNextPage = query.Page < totalPages,
                HasPreviousPage = query.Page > 1
            };

            return Result<PagedResultDTO<UserListItemDTO>>.Success(pagedResult, "Users retrieved successfully");
        }

        public async Task<Result<UserDTO>> CreateUserAsync(CreateUserCommand command)
        {
            // Validate role (only Staff or Admin allowed)
            if (command.Role.ToLower() != "staff" && command.Role.ToLower() != "admin")
            {
                return Result<UserDTO>.BadRequest("Can only create Staff or Admin users");
            }

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(command.Email))
            {
                return Result<UserDTO>.BadRequest("Email already exists");
            }

            // Validate password strength
            if (!IsPasswordStrong(command.Password))
            {
                return Result<UserDTO>.BadRequest(
                    "Password must contain at least 8 characters, one uppercase letter, one number, and one special character");
            }

            // Get role id from repository
            var roleId = await _userRepository.GetRoleIdByNameAsync(command.Role);

            if (roleId == null)
            {
                return Result<UserDTO>.BadRequest($"Role '{command.Role}' not found");
            }

            // Hash password
            var hashedPassword = _passwordHashingService.HashPassword(command.Password);

            // Create user model
            var user = new UserModel
            {
                Email = command.Email,
                Passwordhash = hashedPassword,
                Fullname = command.Fullname,
                Phone = command.Phone,
                Roleid = roleId.Value,
                Createdat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                IsActive = true,
                IsVerified = false
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Get the created user with role info
            var createdUser = await _userRepository.GetByEmailAsync(command.Email);

            if (createdUser == null)
            {
                return Result<UserDTO>.Error("Failed to retrieve created user", 500);
            }

            var userDTO = new UserDTO
            {
                UserId = createdUser.UserId,
                Email = createdUser.Email,
                Fullname = createdUser.Fullname,
                Role = createdUser.Role?.Rolename ?? command.Role,
                IsActive = createdUser.IsActive,
                IsVerified = createdUser.IsVerified,
                Phone = createdUser.Phone,
                CreatedAt = createdUser.Createdat
            };

            return Result<UserDTO>.Created(userDTO, "User created successfully");
        }

        public async Task<Result<UserDTO>> UpdateUserRoleAsync(int userId, UpdateUserRoleCommand command, int currentAdminId)
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result<UserDTO>.NotFound("User not found");
            }

            // Check if admin is trying to demote themselves
            if (userId == currentAdminId && command.NewRole.ToLower() != "admin")
            {
                return Result<UserDTO>.BadRequest("Cannot demote yourself");
            }

            // Get new role id from repository
            var newRoleId = await _userRepository.GetRoleIdByNameAsync(command.NewRole);

            if (newRoleId == null)
            {
                return Result<UserDTO>.BadRequest($"Role '{command.NewRole}' not found");
            }

            // Update user role
            user.Roleid = newRoleId.Value;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Get updated user with new role info
            var updatedUser = await _userRepository.GetByIdAsync(userId);

            var userDTO = new UserDTO
            {
                UserId = updatedUser!.UserId,
                Email = updatedUser.Email,
                Fullname = updatedUser.Fullname,
                Role = updatedUser.Role?.Rolename ?? command.NewRole,
                IsActive = updatedUser.IsActive,
                IsVerified = updatedUser.IsVerified,
                Phone = updatedUser.Phone,
                UpdatedAt = DateTime.UtcNow
            };

            return Result<UserDTO>.Success(userDTO, "User role updated successfully");
        }

        public async Task<Result<UserDTO>> BanUserAsync(int userId, BanUserCommand command, int currentAdminId)
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return Result<UserDTO>.NotFound("User not found");
            }

            // Check if admin is trying to ban themselves
            if (userId == currentAdminId)
            {
                return Result<UserDTO>.BadRequest("Cannot ban yourself");
            }

            // Update isActive status
            user.IsActive = command.IsActive;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var message = command.IsActive ? "User unbanned successfully" : "User banned successfully";

            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                Email = user.Email,
                Fullname = user.Fullname,
                Role = user.Role?.Rolename ?? "Unknown",
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                Phone = user.Phone,
                UpdatedAt = DateTime.UtcNow
            };

            return Result<UserDTO>.Success(userDTO, message);
        }

        private bool IsPasswordStrong(string password)
        {
            // At least 8 characters, one uppercase, one number, one special character
            if (password.Length < 8) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false; // Uppercase
            if (!Regex.IsMatch(password, @"[0-9]")) return false; // Number
            if (!Regex.IsMatch(password, @"[\W_]")) return false; // Special character

            return true;
        }
    }
}
