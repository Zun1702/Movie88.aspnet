namespace Movie88.Domain.Models;

/// <summary>
/// Domain model for Payment entity
/// </summary>
public class PaymentModel
{
    public int Paymentid { get; set; }
    public int Bookingid { get; set; }
    public int Customerid { get; set; }
    public int Methodid { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public string? Transactioncode { get; set; }
    public DateTime? Paymenttime { get; set; }
    
    // Navigation properties
    public PaymentmethodModel? Method { get; set; }
    public BookingModel? Booking { get; set; }
}
