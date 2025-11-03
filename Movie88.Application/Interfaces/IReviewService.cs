using Movie88.Application.DTOs.Reviews;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IReviewService
{
    Task<Result<ReviewsPagedResultDTO>> GetReviewsByMovieIdAsync(int movieId, int page, int pageSize, string? sort);
    Task<Result<ReviewDTO>> CreateReviewAsync(int userId, CreateReviewRequestDTO request);
}
