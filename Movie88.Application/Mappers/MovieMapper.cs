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
        
        // Model -> MovieResponseDto (for Admin CRUD operations)
        CreateMap<MovieModel, MovieResponseDto>()
            .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.Movieid))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.Durationminutes))
            .ForMember(dest => dest.PosterUrl, opt => opt.MapFrom(src => src.Posterurl))
            .ForMember(dest => dest.TrailerUrl, opt => opt.MapFrom(src => src.Trailerurl))
            .ForMember(dest => dest.ReleaseDate, 
                opt => opt.MapFrom(src => src.Releasedate.HasValue 
                    ? src.Releasedate.Value.ToString("yyyy-MM-dd") 
                    : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Createdat));
    }
}
