using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Reviews;
using Movie88.Application.Interfaces;
using System.Security.Claims;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get reviews by movie ID with pagination and sorting
    /// </summary>
    /// <param name="movieId">Movie ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="sort">Sort order: latest (default), oldest, highest, lowest</param>
    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetReviewsByMovieId(
        int movieId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sort = "latest")
    {
        var result = await _reviewService.GetReviewsByMovieIdAsync(movieId, page, pageSize, sort);

        return result.StatusCode switch
        {
            200 => Ok(result),
            404 => NotFound(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }

    /// <summary>
    /// Create a new review for a movie
    /// </summary>
    /// <param name="request">Review creation request</param>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDTO request)
    {
        // Get userId from JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        var result = await _reviewService.CreateReviewAsync(userId, request);

        return result.StatusCode switch
        {
            201 => CreatedAtAction(nameof(GetReviewsByMovieId), new { movieId = request.Movieid }, result),
            404 => NotFound(result),
            409 => Conflict(result),
            _ => StatusCode(result.StatusCode, result)
        };
    }
}
