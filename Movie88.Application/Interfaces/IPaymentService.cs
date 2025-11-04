using Movie88.Application.DTOs.Payments;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service for Payment operations
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Create VNPay payment and return payment URL
    /// </summary>
    Task<CreatePaymentResponseDTO?> CreateVNPayPaymentAsync(
        CreatePaymentRequestDTO request,
        int customerId,
        string ipAddress,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process VNPay callback from user redirect
    /// </summary>
    Task<(bool Success, string Message, int? BookingId)> ProcessVNPayCallbackAsync(
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Process VNPay IPN (Instant Payment Notification)
    /// </summary>
    Task<(string RspCode, string Message)> ProcessVNPayIPNAsync(
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get payment details by ID
    /// </summary>
    Task<PaymentDetailDTO?> GetPaymentByIdAsync(
        int paymentId,
        int customerId,
        CancellationToken cancellationToken = default);
}
