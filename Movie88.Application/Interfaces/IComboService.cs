using Movie88.Application.DTOs.Combos;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IComboService
{
    Task<Result<List<ComboDTO>>> GetAllCombosAsync();
}
