using Movie88.Application.DTOs.Auth;
using Movie88.Application.Interfaces;
using Movie88.Domain.Models;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly Application.Interfaces.IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ICustomerRepository customerRepository,
            Application.Interfaces.IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IPasswordHashingService passwordHashingService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
        }

        private static DateTime GetCurrentTimestamp()
        {
            // For User table: timestamp without time zone
            return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        }

        private static DateTime GetCurrentTimestampUtc()
        {
            // For UserRefreshToken table (public schema): timestamp without time zone
            return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            if (!_passwordHashingService.VerifyPassword(request.Password, user.Passwordhash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(
                user.UserId, 
                user.Email, 
                user.Fullname, 
                user.Role?.Rolename ?? "Customer"
            );
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = _jwtService.GetTokenExpiration(accessToken);

            // Save refresh token to database
            var refreshTokenModel = new RefreshTokenModel
            {
                Token = refreshToken,
                UserId = user.UserId.ToString(),
                CreatedAt = GetCurrentTimestampUtc(),
                UpdatedAt = GetCurrentTimestampUtc(),
                Revoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshTokenModel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = new UserDTO
                {
                    UserId = user.UserId,
                    FullName = user.Fullname,
                    Email = user.Email,
                    PhoneNumber = user.Phone,
                    RoleId = user.Roleid,
                    RoleName = user.Role?.Rolename ?? "Customer"
                }
            };
        }

        public async Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request, CancellationToken cancellationToken = default)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Hash password
            var passwordHash = _passwordHashingService.HashPassword(request.Password);

            // Create new user (default role: Customer = 3)
            var user = new UserModel
            {
                Fullname = request.FullName,
                Email = request.Email,
                Passwordhash = passwordHash,
                Phone = request.PhoneNumber,
                Roleid = 3, // Customer role
                Createdat = GetCurrentTimestamp()
            };

            // Add user and save to get the generated UserId
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Query back the saved user to get the generated UserId
            var savedUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (savedUser == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }
            
            // Now create Customer profile with the generated UserId
            var customer = new CustomerModel
            {
                Userid = savedUser.UserId, // Use UserId from queried user
                Address = null,
                Dateofbirth = null,
                Gender = null,
                // User info will be populated via navigation property
                Fullname = savedUser.Fullname,
                Email = savedUser.Email,
                Phone = savedUser.Phone,
                Createdat = savedUser.Createdat ?? GetCurrentTimestamp()
            };

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Auto login after registration
            var loginRequest = new LoginRequestDTO
            {
                Email = request.Email,
                Password = request.Password
            };

            return await LoginAsync(loginRequest, cancellationToken);
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
        {
            // Validate refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Check if token is revoked
            if (refreshToken.Revoked == true)
            {
                throw new UnauthorizedAccessException("Refresh token has been revoked");
            }

            // Get user - validate UserId format
            if (string.IsNullOrEmpty(refreshToken.UserId) || !int.TryParse(refreshToken.UserId, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid refresh token format");
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            // Revoke old refresh token
            refreshToken.Revoked = true;
            refreshToken.UpdatedAt = GetCurrentTimestampUtc();
            _refreshTokenRepository.Update(refreshToken);

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(
                user.UserId,
                user.Email,
                user.Fullname,
                user.Role?.Rolename ?? "Customer"
            );
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = _jwtService.GetTokenExpiration(newAccessToken);

            // Save new refresh token
            var newRefreshTokenModel = new RefreshTokenModel
            {
                Token = newRefreshToken,
                UserId = user.UserId.ToString(),
                CreatedAt = GetCurrentTimestampUtc(),
                UpdatedAt = GetCurrentTimestampUtc(),
                Revoked = false
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenModel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
                User = new UserDTO
                {
                    UserId = user.UserId,
                    FullName = user.Fullname,
                    Email = user.Email,
                    PhoneNumber = user.Phone,
                    RoleId = user.Roleid,
                    RoleName = user.Role?.Rolename ?? "Customer"
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDTO request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Verify old password
            if (!_passwordHashingService.VerifyPassword(request.OldPassword, user.Passwordhash))
            {
                throw new UnauthorizedAccessException("Old password is incorrect");
            }

            // Hash new password
            user.Passwordhash = _passwordHashingService.HashPassword(request.NewPassword);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Revoke all refresh tokens
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId.ToString(), cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDTO request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return true;
            }

            // TODO: Generate password reset token and send email
            // For now, just return true
            // In production: Generate token, save to DB, send email with reset link

            return true;
        }

        public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
            if (token != null)
            {
                token.Revoked = true;
                token.UpdatedAt = GetCurrentTimestampUtc();
                _refreshTokenRepository.Update(token);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return true;
        }
    }
}
