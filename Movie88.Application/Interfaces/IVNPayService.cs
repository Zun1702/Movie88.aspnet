namespace Movie88.Application.Interfaces;

/// <summary>
/// Service for VNPay integration helpers
/// </summary>
public interface IVNPayService
{
    /// <summary>
    /// Generate VNPay payment URL with secure hash
    /// </summary>
    string GeneratePaymentUrl(int bookingId, decimal amount, string transactionCode, string returnUrl, string ipAddress);
    
    /// <summary>
    /// Validate VNPay signature from callback/IPN
    /// </summary>
    bool ValidateSignature(Dictionary<string, string> parameters, string secureHash);
    
    /// <summary>
    /// Generate unique transaction code
    /// </summary>
    string GenerateTransactionCode(int bookingId);
}
