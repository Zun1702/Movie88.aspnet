using AutoMapper;
using Movie88.Application.DTOs.Reviews;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public ReviewService(
        IReviewRepository reviewRepository,
        IMovieRepository movieRepository,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<ReviewsPagedResultDTO>> GetReviewsByMovieIdAsync(int movieId, int page, int pageSize, string? sort)
    {
        // Validate page and pageSize
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        // Check if movie exists
        var movie = await _movieRepository.GetByIdAsync(movieId);
        if (movie == null)
        {
            return Result<ReviewsPagedResultDTO>.NotFound("Movie not found");
        }

        // Get reviews with pagination
        var reviews = await _reviewRepository.GetByMovieIdAsync(movieId, page, pageSize, sort ?? "latest");
        var totalCount = await _reviewRepository.GetCountByMovieIdAsync(movieId);
        var averageRating = await _reviewRepository.GetAverageRatingByMovieIdAsync(movieId);

        // Map to DTOs
        var reviewDtos = _mapper.Map<List<ReviewDTO>>(reviews);

        // Calculate pagination metadata
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var result = new ReviewsPagedResultDTO
        {
            Reviews = reviewDtos,
            CurrentPage = page,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasPrevious = page > 1,
            HasNext = page < totalPages,
            AverageRating = averageRating
        };

        return Result<ReviewsPagedResultDTO>.Success(result);
    }

    public async Task<Result<ReviewDTO>> CreateReviewAsync(int userId, CreateReviewRequestDTO request)
    {
        // Get customer by userId
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null)
        {
            return Result<ReviewDTO>.NotFound("Customer profile not found");
        }

        // Check if movie exists
        var movie = await _movieRepository.GetByIdAsync(request.Movieid);
        if (movie == null)
        {
            return Result<ReviewDTO>.NotFound("Movie not found");
        }

        // Check for duplicate review (optional - user can only review once)
        var existingReview = await _reviewRepository.GetByCustomerAndMovieAsync(customer.Customerid, request.Movieid);
        if (existingReview != null)
        {
            return Result<ReviewDTO>.Error("You have already reviewed this movie", 409);
        }

        // Map DTO to Model
        var review = _mapper.Map<Domain.Models.ReviewModel>(request);
        review.Customerid = customer.Customerid;
        review.Createdat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        // Save review
        var createdReview = await _reviewRepository.AddAsync(review);

        // Map to DTO
        var reviewDto = _mapper.Map<ReviewDTO>(createdReview);

        return Result<ReviewDTO>.Created(reviewDto);
    }
}
