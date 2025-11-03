using AutoMapper;
using Movie88.Application.DTOs.Promotions;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IMapper _mapper;

    public PromotionService(IPromotionRepository promotionRepository, IMapper mapper)
    {
        _promotionRepository = promotionRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PromotionDTO>>> GetActivePromotionsAsync()
    {
        try
        {
            var promotions = await _promotionRepository.GetActivePromotionsAsync();
            
            if (promotions == null || promotions.Count == 0)
            {
                return Result<List<PromotionDTO>>.Success(
                    new List<PromotionDTO>(), 
                    "No active promotions found");
            }

            var promotionDtos = _mapper.Map<List<PromotionDTO>>(promotions);
            
            return Result<List<PromotionDTO>>.Success(
                promotionDtos, 
                "Active promotions retrieved successfully");
        }
        catch (Exception ex)
        {
            return Result<List<PromotionDTO>>.Error(
                $"Error retrieving active promotions: {ex.Message}");
        }
    }
}
