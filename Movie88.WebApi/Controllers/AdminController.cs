using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie88.Application.DTOs.Admin;
using Movie88.Application.Interfaces;
using System.Security.Claims;

namespace Movie88.WebApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// GET /api/admin/users - Get all users with pagination, filtering, and aggregated data
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of users with totalBookings and totalSpent</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var result = await _adminService.GetUsersAsync(query);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// POST /api/admin/users - Create a new Staff or Admin user
        /// </summary>
        /// <param name="command">User creation data</param>
        /// <returns>Created user information</returns>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _adminService.CreateUserAsync(command);

            if (result.IsSuccess)
                return StatusCode(201, result);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// PUT /api/admin/users/{id}/role - Update user role
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="command">New role data</param>
        /// <returns>Updated user information</returns>
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleCommand command)
        {
            // Get current admin ID from JWT token
            var currentAdminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentAdminIdClaim, out int currentAdminId))
            {
                return Unauthorized(new { success = false, message = "Invalid user token" });
            }

            var result = await _adminService.UpdateUserRoleAsync(id, command, currentAdminId);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// PUT /api/admin/users/{id}/ban - Ban or unban a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="command">Ban/unban data (isActive: false = ban, true = unban)</param>
        /// <returns>Updated user information</returns>
        [HttpPut("users/{id}/ban")]
        public async Task<IActionResult> BanUser(int id, [FromBody] BanUserCommand command)
        {
            // Get current admin ID from JWT token
            var currentAdminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentAdminIdClaim, out int currentAdminId))
            {
                return Unauthorized(new { success = false, message = "Invalid user token" });
            }

            var result = await _adminService.BanUserAsync(id, command, currentAdminId);

            if (result.IsSuccess)
                return Ok(result);

            return StatusCode(result.StatusCode, result);
        }
    }
}
