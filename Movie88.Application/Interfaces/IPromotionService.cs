using Movie88.Application.DTOs.Promotions;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IPromotionService
{
    Task<Result<List<PromotionDTO>>> GetActivePromotionsAsync();
}
