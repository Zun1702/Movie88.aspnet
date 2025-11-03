using AutoMapper;
using Movie88.Application.DTOs.Customers;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerProfileDTO>> GetProfileByUserIdAsync(int userId)
    {
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        
        if (customer == null)
        {
            return Result<CustomerProfileDTO>.NotFound("Customer profile not found");
        }

        var customerDto = _mapper.Map<CustomerProfileDTO>(customer);
        
        return Result<CustomerProfileDTO>.Success(
            customerDto, 
            "Customer profile retrieved successfully");
    }
}
