using Movie88.Application.DTOs.Customers;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface ICustomerService
{
    /// <summary>
    /// Get customer profile by user ID (from JWT token)
    /// </summary>
    Task<Result<CustomerProfileDTO>> GetProfileByUserIdAsync(int userId);
    
    /// <summary>
    /// Update customer profile information
    /// </summary>
    Task<Result<CustomerProfileResponseDto>> UpdateCustomerProfileAsync(int userId, UpdateCustomerProfileDto request);
}
