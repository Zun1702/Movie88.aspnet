using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

/// <summary>
/// Repository interface for Movie entity
/// </summary>
public interface IMovieRepository
{
    Task<MovieModel?> GetByIdAsync(int id);
    Task<IEnumerable<MovieModel>> GetAllAsync();
    Task<MovieModel> AddAsync(MovieModel model);
    Task<MovieModel> UpdateAsync(MovieModel model);
    Task<bool> DeleteAsync(int id);

    Task<(List<MovieModel> Movies, int TotalCount)> GetMoviesAsync(
        int page, 
        int pageSize, 
        string? genre, 
        int? year, 
        string? rating, 
        string? sort);

    Task<(List<MovieModel> Movies, int TotalCount)> GetNowShowingMoviesAsync(int page, int pageSize);
    
    Task<(List<MovieModel> Movies, int TotalCount)> GetComingSoonMoviesAsync(int page, int pageSize);
    
    Task<(List<MovieModel> Movies, int TotalCount)> SearchMoviesAsync(string query, int page, int pageSize);
}
