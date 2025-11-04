using Movie88.Application.DTOs.Auth;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using IUnitOfWork = Movie88.Application.Interfaces.IUnitOfWork;

namespace Movie88.Application.Services;

public class OtpService : IOtpService
{
    private readonly IOtpTokenRepository _otpRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OtpService> _logger;

    public OtpService(
        IOtpTokenRepository otpRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<OtpService> logger)
    {
        _otpRepository = otpRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SendOtpResponseDTO> SendOtpAsync(
        SendOtpRequestDTO request, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        // 1. Validate email exists
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Email not found");
        }

        // 2. Check if user already verified (only for EmailVerification type)
        if (request.OtpType == OtpTypeConstants.EmailVerification && user.IsVerified)
        {
            throw new Exception("Email is already verified");
        }

        // 3. Check rate limit (max 3 OTPs per 10 minutes)
        var otpCount = await _otpRepository.GetOtpCountAsync(
            user.UserId, 
            request.OtpType, 
            TimeSpan.FromMinutes(10)
        );

        if (otpCount >= 3)
        {
            throw new Exception("Too many OTP requests. Please try again after 10 minutes.");
        }

        // 4. Check if active OTP exists
        var activeOtp = await _otpRepository.GetActiveOtpAsync(user.UserId, request.OtpType);
        if (activeOtp != null)
        {
            var remainingMinutes = (activeOtp.ExpiresAt - DateTime.UtcNow).TotalMinutes;
            throw new Exception($"An OTP is already active. Please use the existing OTP or wait {Math.Ceiling(remainingMinutes)} minutes.");
        }

        // 5. Generate OTP code
        var otpCode = GenerateOtpCode();

        // 6. Create OTP token
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var otpToken = new OtpTokenModel
        {
            UserId = user.UserId,
            OtpCode = otpCode,
            OtpType = request.OtpType,
            Email = request.Email,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(10),
            IsUsed = false,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        await _otpRepository.CreateAsync(otpToken);

        // 7. Send email via Resend
        var emailSent = await SendOtpEmailAsync(request.Email, otpCode, request.OtpType);
        
        if (!emailSent)
        {
            _logger.LogWarning("Failed to send OTP email to {Email}, but OTP was created in database", request.Email);
        }

        // 8. Return response
        return new SendOtpResponseDTO
        {
            Email = request.Email,
            OtpType = request.OtpType,
            ExpiresAt = otpToken.ExpiresAt,
            ExpiresInMinutes = 10,
            Message = "OTP has been sent to your email. Please check your inbox."
        };
    }

    public async Task<VerifyOtpResponseDTO> VerifyOtpAsync(
        VerifyOtpRequestDTO request, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        // 1. Get OTP token
        var otpToken = await _otpRepository.GetByCodeAsync(
            request.OtpCode, 
            request.OtpType, 
            request.Email
        );

        if (otpToken == null)
        {
            throw new Exception("Invalid OTP code");
        }

        // 2. Check expired
        if (otpToken.IsExpired())
        {
            throw new Exception("OTP code has expired");
        }

        // 3. Check used
        if (otpToken.IsUsed)
        {
            throw new Exception("OTP code has already been used");
        }

        // 4. Mark OTP as used FIRST
        await _otpRepository.MarkAsUsedAsync(otpToken.Id, ipAddress, userAgent);
        
        // 5. Save OTP changes immediately to avoid tracking conflicts
        await _unitOfWork.SaveChangesAsync();

        // 6. Get user (after OTP is saved)
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        // 7. Handle based on OTP type
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var response = new VerifyOtpResponseDTO
        {
            Email = request.Email,
            IsVerified = true,
            VerifiedAt = now
        };

        if (request.OtpType == OtpTypeConstants.EmailVerification)
        {
            // Update user verification status (only once)
            if (!user.IsVerified)
            {
                user.IsVerified = true;
                user.VerifiedAt = now;
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
                
                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.Fullname);
            }
            
            response.Message = "Your email has been verified successfully. You can now login.";
        }
        else if (request.OtpType == OtpTypeConstants.PasswordReset)
        {
            // Save OTP marked as used
            await _unitOfWork.SaveChangesAsync();
            response.Message = "OTP verified successfully. You can now reset your password.";
        }
        else if (request.OtpType == OtpTypeConstants.Login)
        {
            // Save OTP marked as used
            await _unitOfWork.SaveChangesAsync();
            response.Message = "Login verification successful.";
        }

        return response;
    }

    public async Task<SendOtpResponseDTO> ResendOtpAsync(
        ResendOtpRequestDTO request, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        // 1. Get user
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Email not found");
        }

        // 2. Check if user already verified (only for EmailVerification type)
        if (request.OtpType == OtpTypeConstants.EmailVerification && user.IsVerified)
        {
            throw new Exception("Email is already verified. No need to resend OTP.");
        }

        // 3. Invalidate old OTPs
        await _otpRepository.InvalidateUserOtpsAsync(user.UserId, request.OtpType);

        // 4. Send new OTP
        return await SendOtpAsync(new SendOtpRequestDTO
        {
            Email = request.Email,
            OtpType = request.OtpType
        }, ipAddress, userAgent);
    }

    public string GenerateOtpCode()
    {
        // Generate cryptographically secure 6-digit code
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var randomNumber = BitConverter.ToUInt32(bytes, 0);
        var otpCode = (randomNumber % 1000000).ToString("D6");
        return otpCode;
    }

    public async Task<bool> SendOtpEmailAsync(string email, string otpCode, string otpType)
    {
        return await _emailService.SendOtpEmailAsync(email, otpCode, otpType);
    }
}
