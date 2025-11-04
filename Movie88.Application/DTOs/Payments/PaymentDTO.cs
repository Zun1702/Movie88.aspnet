using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Payments;

/// <summary>
/// Request to create VNPay payment
/// </summary>
public class CreatePaymentRequestDTO
{
    [Required(ErrorMessage = "Booking ID is required")]
    public int Bookingid { get; set; }
    
    /// <summary>
    /// Optional return URL. If not provided, will use default from config
    /// </summary>
    public string? Returnurl { get; set; }
}

/// <summary>
/// Response after creating VNPay payment
/// </summary>
public class CreatePaymentResponseDTO
{
    public int Paymentid { get; set; }
    public int Bookingid { get; set; }
    public decimal Amount { get; set; }
    public string VnpayUrl { get; set; } = null!;
    public string Transactioncode { get; set; } = null!;
}

/// <summary>
/// Detailed payment information
/// </summary>
public class PaymentDetailDTO
{
    public int Paymentid { get; set; }
    public int Bookingid { get; set; }
    public int Customerid { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public string? Transactioncode { get; set; }
    public DateTime? Paymenttime { get; set; }
    public PaymentMethodDTO? Method { get; set; }
    public BookingSummaryDTO? Booking { get; set; }
}

/// <summary>
/// Payment method information
/// </summary>
public class PaymentMethodDTO
{
    public int Methodid { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

/// <summary>
/// Booking summary in payment details
/// </summary>
public class BookingSummaryDTO
{
    public int Bookingid { get; set; }
    public string? Bookingcode { get; set; }
    public string? Status { get; set; }
    public decimal? Totalamount { get; set; }
}

/// <summary>
/// VNPay callback/IPN parameters
/// </summary>
public class VNPayCallbackParamsDTO
{
    public string vnp_TxnRef { get; set; } = null!;
    public string vnp_ResponseCode { get; set; } = null!;
    public string vnp_TransactionStatus { get; set; } = null!;
    public string vnp_Amount { get; set; } = null!;
    public string? vnp_BankCode { get; set; }
    public string? vnp_BankTranNo { get; set; }
    public string? vnp_CardType { get; set; }
    public string? vnp_OrderInfo { get; set; }
    public string? vnp_PayDate { get; set; }
    public string? vnp_TransactionNo { get; set; }
    public string vnp_SecureHash { get; set; } = null!;
}
