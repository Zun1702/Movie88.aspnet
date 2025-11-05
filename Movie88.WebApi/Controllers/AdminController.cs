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
        private readonly IReportService _reportService;

        public AdminController(IAdminService adminService, IReportService reportService)
        {
            _adminService = adminService;
            _reportService = reportService;
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

        #region Reports & Analytics Endpoints

        /// <summary>
        /// GET /api/admin/dashboard/stats - Dashboard overview statistics
        /// </summary>
        /// <returns>Dashboard stats with today's revenue, active movies, popular movies, upcoming showtimes</returns>
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken = default)
        {
            var result = await _reportService.GetDashboardStatsAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }

        /// <summary>
        /// GET /api/admin/reports/revenue/daily - Daily revenue report
        /// </summary>
        /// <param name="query">Date to get revenue for</param>
        /// <returns>Revenue breakdown by movie, cinema, hour</returns>
        [HttpGet("reports/revenue/daily")]
        public async Task<IActionResult> GetDailyRevenue([FromQuery] DailyRevenueQuery query, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _reportService.GetDailyRevenueAsync(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }

        /// <summary>
        /// GET /api/admin/reports/revenue/monthly - Monthly revenue report
        /// </summary>
        /// <param name="query">Month and year to get revenue for</param>
        /// <returns>Revenue breakdown by movie, cinema</returns>
        [HttpGet("reports/revenue/monthly")]
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] MonthlyRevenueQuery query, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _reportService.GetMonthlyRevenueAsync(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }

        /// <summary>
        /// GET /api/admin/reports/bookings/statistics - Booking statistics
        /// </summary>
        /// <param name="query">Date range to get statistics for</param>
        /// <returns>Booking statistics with completion rate, check-in rate, peak hours/days</returns>
        [HttpGet("reports/bookings/statistics")]
        public async Task<IActionResult> GetBookingStatistics([FromQuery] BookingStatisticsQuery query, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _reportService.GetBookingStatisticsAsync(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }

        /// <summary>
        /// GET /api/admin/reports/popular-movies - Popular movies ranking
        /// </summary>
        /// <param name="query">Period (week/month/year) and limit</param>
        /// <returns>Popular movies ranked by revenue with occupancy rate and trend</returns>
        [HttpGet("reports/popular-movies")]
        public async Task<IActionResult> GetPopularMovies([FromQuery] PopularMoviesQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _reportService.GetPopularMoviesAsync(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }

        /// <summary>
        /// GET /api/admin/reports/customers/analytics - Customer analytics
        /// </summary>
        /// <param name="query">Period and top customers limit</param>
        /// <returns>Customer analytics with retention, churn, demographics, top customers</returns>
        [HttpGet("reports/customers/analytics")]
        public async Task<IActionResult> GetCustomerAnalytics([FromQuery] CustomerAnalyticsQuery query, CancellationToken cancellationToken = default)
        {
            var result = await _reportService.GetCustomerAnalyticsAsync(query, cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }

        #endregion
    }
}
