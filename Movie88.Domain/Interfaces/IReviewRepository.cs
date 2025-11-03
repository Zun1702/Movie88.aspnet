using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<ReviewModel>> GetByMovieIdAsync(int movieId, int page, int pageSize, string sort);
    Task<int> GetCountByMovieIdAsync(int movieId);
    Task<decimal?> GetAverageRatingByMovieIdAsync(int movieId);
    Task<ReviewModel?> GetByCustomerAndMovieAsync(int customerId, int movieId);
    Task<ReviewModel> AddAsync(ReviewModel review);
    Task<ReviewModel?> GetByIdAsync(int reviewId);
}
