using AutoMapper;
using Movie88.Application.DTOs.Reviews;
using Movie88.Domain.Models;

namespace Movie88.Application.Mappers;

public class ReviewMapper : Profile
{
    public ReviewMapper()
    {
        // Model -> DTO
        CreateMap<ReviewModel, ReviewDTO>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new CustomerInfoDTO
            {
                Customerid = src.Customerid,
                Fullname = src.Customer != null ? src.Customer.Fullname : null,
                Gender = src.Customer != null ? src.Customer.Gender : null
            }));
        
        // CreateReviewRequestDTO -> ReviewModel
        CreateMap<CreateReviewRequestDTO, ReviewModel>()
            .ForMember(dest => dest.Reviewid, opt => opt.Ignore())
            .ForMember(dest => dest.Customerid, opt => opt.Ignore())
            .ForMember(dest => dest.Createdat, opt => opt.MapFrom(src => 
                DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)))
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Movie, opt => opt.Ignore());
    }
}
