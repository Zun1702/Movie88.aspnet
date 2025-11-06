using Movie88.Application.DTOs.Promotions;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IPromotionService
{
    /// <summary>
    /// Get all active promotions (for banner display)
    /// </summary>
    Task<Result<List<PromotionDTO>>> GetActivePromotionsAsync();
    
    /// <summary>
    /// Auto-apply eligible promotions to a booking
    /// </summary>
    /// <param name="bookingId">Booking ID to apply promotions to</param>
    /// <param name="totalAmount">Original total amount before discounts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of applied promotions with discount amounts</returns>
    Task<List<AppliedPromotionDTO>> ApplyEligiblePromotionsAsync(
        int bookingId, 
        decimal totalAmount, 
        CancellationToken cancellationToken = default);
}
