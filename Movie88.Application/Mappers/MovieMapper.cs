using AutoMapper;
using Movie88.Application.DTOs.Movies;
using Movie88.Domain.Models;

namespace Movie88.Application.Mappers;

public class MovieMapper : Profile
{
    public MovieMapper()
    {
        // Model -> DTO
        CreateMap<MovieModel, MovieDTO>()
            .ForMember(dest => dest.Releasedate, 
                opt => opt.MapFrom(src => src.Releasedate.HasValue 
                    ? src.Releasedate.Value.ToString("yyyy-MM-dd") 
                    : null));
        
        // Model -> MovieDetailDTO
        CreateMap<MovieModel, MovieDetailDTO>()
            .ForMember(dest => dest.Releasedate, 
                opt => opt.MapFrom(src => src.Releasedate.HasValue 
                    ? src.Releasedate.Value.ToDateTime(TimeOnly.MinValue) 
                    : (DateTime?)null))
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.TotalReviews, opt => opt.Ignore())
            .ForMember(dest => dest.TotalShowtimes, opt => opt.Ignore());
    }
}
