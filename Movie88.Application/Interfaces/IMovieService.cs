using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Movies;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service interface for Movie operations
/// </summary>
public interface IMovieService
{
    Task<Result<PagedResultDTO<MovieDTO>>> GetMoviesAsync(
        int page, 
        int pageSize, 
        string? genre, 
        int? year, 
        string? rating, 
        string? sort);

    Task<Result<PagedResultDTO<MovieDTO>>> GetNowShowingMoviesAsync(int page, int pageSize);
    
    Task<Result<PagedResultDTO<MovieDTO>>> GetComingSoonMoviesAsync(int page, int pageSize);
    
    Task<Result<PagedResultDTO<MovieDTO>>> SearchMoviesAsync(string query, int page, int pageSize);
    
    Task<Result<MovieDetailDTO>> GetMovieByIdAsync(int movieId);
}
