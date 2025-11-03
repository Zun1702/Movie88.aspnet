using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IPromotionRepository
{
    Task<List<PromotionModel>> GetActivePromotionsAsync();
}
