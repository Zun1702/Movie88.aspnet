using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

/// <summary>
/// Repository interface for managing booking promotions
/// </summary>
public interface IBookingPromotionRepository
{
    /// <summary>
    /// Create a new booking promotion record
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="promotionId">Promotion ID</param>
    /// <param name="discountApplied">Discount amount applied</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created booking promotion ID</returns>
    Task<int> CreateAsync(int bookingId, int promotionId, decimal discountApplied, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get all promotions applied to a specific booking
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of applied promotions</returns>
    Task<List<BookingPromotionModel>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
}
