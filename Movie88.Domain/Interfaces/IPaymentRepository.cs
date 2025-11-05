using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

/// <summary>
/// Repository interface for Payment operations
/// </summary>
public interface IPaymentRepository
{
    /// <summary>
    /// Get payment by ID with related details (Method, Booking)
    /// </summary>
    Task<PaymentModel?> GetByIdWithDetailsAsync(int paymentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get payment by transaction code with related details
    /// </summary>
    Task<PaymentModel?> GetByTransactionCodeAsync(string transactionCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get payment by booking ID with specific status
    /// </summary>
    Task<PaymentModel?> GetByBookingIdAndStatusAsync(int bookingId, string status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create new payment
    /// </summary>
    Task<PaymentModel> CreatePaymentAsync(
        int bookingId,
        int customerId,
        int methodId,
        decimal amount,
        string transactionCode,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process payment callback with booking code generation
    /// </summary>
    Task<bool> ProcessPaymentCallbackAsync(
        string transactionCode,
        string responseCode,
        string bookingCode,
        CancellationToken cancellationToken = default);
}
