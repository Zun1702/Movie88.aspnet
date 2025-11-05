using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerModel?> GetByIdAsync(int id);
    Task<List<CustomerModel>> GetAllAsync();
    Task<CustomerModel> AddAsync(CustomerModel model);
    Task<CustomerModel> UpdateAsync(CustomerModel model);
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Get customer profile by user ID
    /// </summary>
    Task<CustomerModel?> GetByUserIdAsync(int userId);
    
    /// <summary>
    /// Get customer by email
    /// </summary>
    Task<CustomerModel?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Check if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email);
    
    /// <summary>
    /// Get customer with user data by user ID
    /// </summary>
    Task<CustomerModel?> GetCustomerWithUserByUserIdAsync(int userId);
}
