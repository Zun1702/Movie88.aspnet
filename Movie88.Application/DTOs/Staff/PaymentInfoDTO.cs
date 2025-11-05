namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Payment information for staff booking verification
/// NOTE: Payment status is retrieved from Payment entity (separate table), NOT from Booking.paymentStatus
/// </summary>
public class PaymentInfoDTO
{
    /// <summary>
    /// Payment status from Payment.Status (Pending, Completed, Failed)
    /// Retrieved via Booking.Payments collection
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    public string? PaymentMethod { get; set; }
    public string? TransactionCode { get; set; }
    public DateTime? PaidAt { get; set; }
}
