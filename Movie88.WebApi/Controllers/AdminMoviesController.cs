using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Movies;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Admin")]
public class AdminMoviesController : ControllerBase
{
    private readonly IAdminMovieService _adminMovieService;

    public AdminMoviesController(IAdminMovieService adminMovieService)
    {
        _adminMovieService = adminMovieService;
    }

    /// <summary>
    /// Create a new movie (Admin only)
    /// POST /api/movies
    /// </summary>
    [HttpPost("movies")]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto request)
    {
        var result = await _adminMovieService.CreateMovieAsync(request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Message });
        }

        return StatusCode(201, new
        {
            success = true,
            statusCode = 201,
            message = result.Message,
            data = result.Data
        });
    }

    /// <summary>
    /// Update an existing movie (Admin only)
    /// PUT /api/movies/{id}
    /// </summary>
    [HttpPut("movies/{id}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieDto request)
    {
        var result = await _adminMovieService.UpdateMovieAsync(id, request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { 
                success = false,
                message = result.Message 
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = result.Message,
            data = result.Data
        });
    }

    /// <summary>
    /// Delete a movie (Admin only)
    /// DELETE /api/movies/{id}
    /// </summary>
    [HttpDelete("movies/{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var result = await _adminMovieService.DeleteMovieAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { 
                success = false,
                message = result.Message,
                errors = new[] { result.Message }
            });
        }

        return Ok(new
        {
            success = true,
            statusCode = 200,
            message = result.Message
        });
    }

    /// <summary>
    /// Get all movies with aggregated data for admin (Admin only)
    /// GET /api/admin/movies
    /// </summary>
    [HttpGet("admin/movies")]
    public async Task<IActionResult> GetMoviesForAdmin(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest(new { 
                success = false,
                message = "Page and pageSize must be greater than 0" 
            });
        }

        var result = await _adminMovieService.GetMoviesForAdminAsync(page, pageSize);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { 
                success = false,
                message = result.Message 
            });
        }

        return Ok(new
        {
            success = true,
            data = new
            {
                movies = result.Data!.Items,
                pagination = new
                {
                    currentPage = result.Data.CurrentPage,
                    pageSize = result.Data.PageSize,
                    totalPages = result.Data.TotalPages,
                    totalRecords = result.Data.TotalItems
                }
            }
        });
    }
}
