namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Pricing information for staff booking verification
/// </summary>
public class PricingInfoDTO
{
    public decimal TicketPrice { get; set; }
    public int NumberOfTickets { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
}
