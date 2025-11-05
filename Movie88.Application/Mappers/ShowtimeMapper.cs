using AutoMapper;
using Movie88.Application.DTOs.Showtimes;
using Movie88.Domain.Models;

namespace Movie88.Application.Mappers;

public class ShowtimeMapper : Profile
{
    public ShowtimeMapper()
    {
        // Model -> DTO
        CreateMap<ShowtimeModel, ShowtimeItemDTO>()
            .ForMember(dest => dest.Auditoriumid, opt => opt.MapFrom(src => src.Auditoriumid))
            .ForMember(dest => dest.AuditoriumName, opt => opt.MapFrom(src => 
                src.Auditorium != null ? src.Auditorium.Name : null))
            .ForMember(dest => dest.AvailableSeats, opt => opt.Ignore()); // Will be set manually
        
        CreateMap<CinemaModel, CinemaInfoDTO>();
    }
}
