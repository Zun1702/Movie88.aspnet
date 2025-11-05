using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IShowtimeService _showtimeService;

    public MoviesController(IMovieService movieService, IShowtimeService showtimeService)
    {
        _movieService = movieService;
        _showtimeService = showtimeService;
    }

    /// <summary>
    /// GET /api/movies - Get all movies with pagination and filters
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMovies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? genre = null,
        [FromQuery] int? year = null,
        [FromQuery] string? rating = null,
        [FromQuery] string? sort = null)
    {
        var result = await _movieService.GetMoviesAsync(page, pageSize, genre, year, rating, sort);
        
        if (result.IsSuccess)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/movies/now-showing - Get movies currently showing in cinemas
    /// </summary>
    [HttpGet("now-showing")]
    public async Task<IActionResult> GetNowShowingMovies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _movieService.GetNowShowingMoviesAsync(page, pageSize);
        
        if (result.IsSuccess)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/movies/coming-soon - Get upcoming movies
    /// </summary>
    [HttpGet("coming-soon")]
    public async Task<IActionResult> GetComingSoonMovies(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _movieService.GetComingSoonMoviesAsync(page, pageSize);
        
        if (result.IsSuccess)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/movies/search - Search movies by title, director, genre, or description
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new { success = false, message = "Search query is required" });
        }

        var result = await _movieService.SearchMoviesAsync(query, page, pageSize);
        
        if (result.IsSuccess)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/movies/{id} - Get movie details by ID with computed fields
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovieById(int id)
    {
        var result = await _movieService.GetMovieByIdAsync(id);
        
        if (result.IsSuccess)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>
    /// GET /api/movies/{id}/showtimes - Get showtimes for a movie, grouped by date and cinema
    /// </summary>
    /// <param name="id">Movie ID</param>
    [HttpGet("{id}/showtimes")]
    public async Task<IActionResult> GetMovieShowtimes(int id)
    {
        var showtimes = await _showtimeService.GetShowtimesByMovieAsync(id);
        
        if (showtimes == null)
        {
            return NotFound(new
            {
                success = false,
                statusCode = 404,
                message = $"No showtimes found for movie ID {id}",
                data = (object?)null
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = "Showtimes retrieved successfully",
            data = showtimes
        });
    }
}
