using AutoMapper;
using Movie88.Application.DTOs.Customers;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly Movie88.Application.Interfaces.IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(
        ICustomerRepository customerRepository,
        Movie88.Application.Interfaces.IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
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

    public async Task<Result<CustomerProfileResponseDto>> UpdateCustomerProfileAsync(int userId, UpdateCustomerProfileDto request)
    {
        var customer = await _customerRepository.GetCustomerWithUserByUserIdAsync(userId);

        if (customer == null)
        {
            return Result<CustomerProfileResponseDto>.NotFound("Customer profile not found");
        }

        customer.Address = request.Address;

        if (!string.IsNullOrEmpty(request.DateOfBirth))
        {
            try
            {
                customer.Dateofbirth = DateOnly.Parse(request.DateOfBirth);
            }
            catch (FormatException)
            {
                return Result<CustomerProfileResponseDto>.BadRequest("Invalid date format");
            }
        }

        customer.Gender = request.Gender;

        await _customerRepository.UpdateAsync(customer);

        var customerProfile = new CustomerProfileResponseDto
        {
            Customerid = customer.Customerid,
            Userid = customer.Userid,
            Fullname = customer.Fullname,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            DateOfBirth = customer.Dateofbirth?.ToString("yyyy-MM-dd"),
            Gender = customer.Gender
        };

        return Result<CustomerProfileResponseDto>.Success(
            customerProfile,
            "Customer profile updated successfully"
        );
    }
}
