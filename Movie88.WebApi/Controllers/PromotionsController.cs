using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    /// <summary>
    /// Get all active promotions (promotions that are currently running)
    /// </summary>
    /// <returns>List of active promotions</returns>
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePromotions()
    {
        var result = await _promotionService.GetActivePromotionsAsync();
        return StatusCode(result.StatusCode, result);
    }
}
