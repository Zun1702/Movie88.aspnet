using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

/// <summary>
/// Repository interface for Paymentmethod operations
/// </summary>
public interface IPaymentmethodRepository
{
    /// <summary>
    /// Get payment method by name (e.g., "VNPay", "MoMo")
    /// </summary>
    Task<PaymentmethodModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
