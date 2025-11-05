using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.User;
using Movie88.Application.Interfaces;
using System.Security.Claims;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token" });
        }

        var result = await _userService.GetCurrentUserAsync(userId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized(new { message = "Invalid or missing user ID in token" });
        }

        var result = await _userService.UpdateUserAsync(id, userId, request);

        if (!result.IsSuccess)
        {
            if (result.StatusCode == 403)
            {
                return Forbid();
            }

            return StatusCode(result.StatusCode, result);
        }

        return Ok(result);
    }
}