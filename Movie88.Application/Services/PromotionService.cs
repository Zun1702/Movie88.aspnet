using AutoMapper;
using Microsoft.Extensions.Logging;
using Movie88.Application.DTOs.Promotions;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IBookingPromotionRepository _bookingPromotionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PromotionService> _logger;

    public PromotionService(
        IPromotionRepository promotionRepository,
        IBookingPromotionRepository bookingPromotionRepository,
        IMapper mapper,
        ILogger<PromotionService> logger)
    {
        _promotionRepository = promotionRepository;
        _bookingPromotionRepository = bookingPromotionRepository;
        _mapper = mapper;
        _logger = logger;
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

    /// <inheritdoc/>
    public async Task<List<AppliedPromotionDTO>> ApplyEligiblePromotionsAsync(
        int bookingId, 
        decimal totalAmount, 
        CancellationToken cancellationToken = default)
    {
        var appliedPromotions = new List<AppliedPromotionDTO>();
        
        try
        {
            // 1. Get all active promotions
            var activePromotions = await _promotionRepository.GetActivePromotionsAsync();
            
            if (activePromotions == null || !activePromotions.Any())
            {
                _logger.LogInformation("No active promotions found for booking {BookingId}", bookingId);
                return appliedPromotions;
            }
            
            _logger.LogInformation("Found {Count} active promotions for booking {BookingId}", 
                activePromotions.Count, bookingId);
            
            // 2. Apply each eligible promotion
            foreach (var promotion in activePromotions)
            {
                try
                {
                    // Calculate discount based on type
                    decimal discount = CalculateDiscount(promotion, totalAmount);
                    
                    if (discount <= 0)
                    {
                        _logger.LogWarning("Calculated discount is zero or negative for promotion {PromotionId}", 
                            promotion.Promotionid);
                        continue;
                    }
                    
                    // 3. Insert into bookingpromotions table
                    await _bookingPromotionRepository.CreateAsync(
                        bookingId, 
                        promotion.Promotionid, 
                        discount, 
                        cancellationToken);
                    
                    // 4. Add to result list
                    appliedPromotions.Add(new AppliedPromotionDTO
                    {
                        Promotionid = promotion.Promotionid,
                        Name = promotion.Name,
                        Description = promotion.Description,
                        Discountapplied = discount
                    });
                    
                    _logger.LogInformation(
                        "Applied promotion {PromotionId} ({PromotionName}) to booking {BookingId}. Discount: {Discount:C}",
                        promotion.Promotionid, promotion.Name, bookingId, discount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Error applying promotion {PromotionId} to booking {BookingId}", 
                        promotion.Promotionid, bookingId);
                    // Continue with next promotion - don't fail entire booking
                }
            }
            
            if (appliedPromotions.Any())
            {
                var totalDiscount = appliedPromotions.Sum(p => p.Discountapplied);
                _logger.LogInformation(
                    "Successfully applied {Count} promotions to booking {BookingId}. Total discount: {TotalDiscount:C}",
                    appliedPromotions.Count, bookingId, totalDiscount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ApplyEligiblePromotionsAsync for booking {BookingId}", bookingId);
            // Don't throw - promotions are optional, booking should succeed even if promotion application fails
        }
        
        return appliedPromotions;
    }
    
    /// <summary>
    /// Calculate discount amount based on promotion type
    /// </summary>
    private decimal CalculateDiscount(Movie88.Domain.Models.PromotionModel promotion, decimal totalAmount)
    {
        // Database uses "Percent" and "Fixed" (case-sensitive from PostgreSQL)
        if (string.Equals(promotion.Discounttype, "Percent", StringComparison.OrdinalIgnoreCase))
        {
            // Percentage discount: totalAmount * (discountValue / 100)
            var discount = totalAmount * ((promotion.Discountvalue ?? 0) / 100);
            _logger.LogDebug("Calculated percentage discount: {Total} * {Percent}% = {Discount}", 
                totalAmount, promotion.Discountvalue ?? 0, discount);
            return discount;
        }
        else if (string.Equals(promotion.Discounttype, "Fixed", StringComparison.OrdinalIgnoreCase))
        {
            // Fixed discount: just return the discount value
            var discount = promotion.Discountvalue ?? 0;
            _logger.LogDebug("Calculated fixed discount: {Discount}", discount);
            return discount;
        }
        else
        {
            _logger.LogWarning("Unknown discount type: {DiscountType} for promotion {PromotionId}", 
                promotion.Discounttype, promotion.Promotionid);
            return 0;
        }
    }
}
