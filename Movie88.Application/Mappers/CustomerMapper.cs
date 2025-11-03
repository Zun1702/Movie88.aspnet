using AutoMapper;
using Movie88.Application.DTOs.Customers;
using Movie88.Domain.Models;

namespace Movie88.Application.Mappers;

public class CustomerMapper : Profile
{
    public CustomerMapper()
    {
        CreateMap<CustomerModel, CustomerProfileDTO>()
            .ForMember(dest => dest.Dateofbirth, 
                opt => opt.MapFrom(src => src.Dateofbirth.HasValue 
                    ? src.Dateofbirth.Value.ToString("yyyy-MM-dd") 
                    : null))
            .ForMember(dest => dest.Createdat,
                opt => opt.MapFrom(src => src.Createdat.ToString("yyyy-MM-ddTHH:mm:ss")));
    }
}
