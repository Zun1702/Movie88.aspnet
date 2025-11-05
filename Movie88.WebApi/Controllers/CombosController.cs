using Microsoft.AspNetCore.Mvc;
using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CombosController : ControllerBase
{
    private readonly IComboService _comboService;

    public CombosController(IComboService comboService)
    {
        _comboService = comboService;
    }

    /// <summary>
    /// Get all available combos
    /// </summary>
    /// <returns>List of combos</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCombos()
    {
        var result = await _comboService.GetAllCombosAsync();

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                success = false,
                statusCode = 400,
                message = result.Message,
                data = (object?)null
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
}
