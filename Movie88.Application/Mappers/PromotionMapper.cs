using AutoMapper;
using Movie88.Application.DTOs.Promotions;
using Movie88.Domain.Models;

namespace Movie88.Application.Mappers;

public class PromotionMapper : Profile
{
    public PromotionMapper()
    {
        CreateMap<PromotionModel, PromotionDTO>()
            .ForMember(dest => dest.Startdate, opt => opt.MapFrom(src => 
                src.Startdate.HasValue ? src.Startdate.Value.ToString("yyyy-MM-dd") : null))
            .ForMember(dest => dest.Enddate, opt => opt.MapFrom(src => 
                src.Enddate.HasValue ? src.Enddate.Value.ToString("yyyy-MM-dd") : null));
    }
}
