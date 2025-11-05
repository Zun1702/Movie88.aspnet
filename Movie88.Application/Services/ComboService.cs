using Movie88.Application.DTOs.Combos;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class ComboService : IComboService
{
    private readonly IComboRepository _comboRepository;

    public ComboService(IComboRepository comboRepository)
    {
        _comboRepository = comboRepository;
    }

    public async Task<Result<List<ComboDTO>>> GetAllCombosAsync()
    {
        var combos = await _comboRepository.GetAllAsync();

        var comboDTOs = combos.Select(c => new ComboDTO
        {
            Comboid = c.Comboid,
            Name = c.Name ?? string.Empty,
            Description = c.Description,
            Price = c.Price ?? 0,
            Imageurl = c.Imageurl
        }).ToList();

        return Result<List<ComboDTO>>.Success(comboDTOs, "Combos retrieved successfully");
    }
}
