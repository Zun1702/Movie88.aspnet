using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Cinemas;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminCinemasController : ControllerBase
{
    private readonly IAdminCinemaService _adminCinemaService;

    public AdminCinemasController(IAdminCinemaService adminCinemaService)
    {
        _adminCinemaService = adminCinemaService;
    }

    /// <summary>
    /// Create a new cinema (Admin only)
    /// POST /api/admin/cinemas
    /// </summary>
    [HttpPost("cinemas")]
    public async Task<IActionResult> CreateCinema([FromBody] CreateCinemaDto request)
    {
        var result = await _adminCinemaService.CreateCinemaAsync(request);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { 
                success = false,
                message = result.Message 
            });
        }

        return StatusCode(201, new
        {
            success = true,
            message = result.Message,
            data = new
            {
                cinemaId = result.Data!.CinemaId,
                name = result.Data.Name
            }
        });
    }

    /// <summary>
    /// Update an existing cinema (Admin only)
    /// PUT /api/admin/cinemas/{id}
    /// </summary>
    [HttpPut("cinemas/{id}")]
    public async Task<IActionResult> UpdateCinema(int id, [FromBody] UpdateCinemaDto request)
    {
        var result = await _adminCinemaService.UpdateCinemaAsync(id, request);

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
            message = result.Message,
            data = result.Data
        });
    }

    /// <summary>
    /// Delete a cinema (Admin only)
    /// DELETE /api/admin/cinemas/{id}
    /// </summary>
    [HttpDelete("cinemas/{id}")]
    public async Task<IActionResult> DeleteCinema(int id)
    {
        var result = await _adminCinemaService.DeleteCinemaAsync(id);

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
            message = result.Message
        });
    }

    /// <summary>
    /// Get cinema by ID (Admin only)
    /// GET /api/admin/cinemas/{id}
    /// </summary>
    [HttpGet("cinemas/{id}")]
    public async Task<IActionResult> GetCinemaById(int id)
    {
        var result = await _adminCinemaService.GetCinemaByIdAsync(id);

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
            data = result.Data
        });
    }
}
