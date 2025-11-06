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
    
    // Admin operations
    Task<bool> HasBookingsAsync(int movieId);
    
    Task<(List<MovieModel> Movies, Dictionary<int, MovieStatistics> Stats, int TotalCount)> GetMoviesForAdminAsync(int page, int pageSize);
}

/// <summary>
/// Movie statistics for admin view
/// </summary>
public class MovieStatistics
{
    public int TotalShowtimes { get; set; }
    public int TotalBookings { get; set; }
    public decimal Revenue { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
