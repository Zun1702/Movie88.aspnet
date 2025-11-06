namespace Movie88.Domain.Models;

/// <summary>
/// Model representing the relationship between a booking and applied promotions
/// </summary>
public class BookingPromotionModel
{
    public int Bookingpromotionid { get; set; }
    public int Bookingid { get; set; }
    public int Promotionid { get; set; }
    public decimal Discountapplied { get; set; }
    
    // Navigation properties
    public string PromotionName { get; set; } = string.Empty;
    public string? PromotionDescription { get; set; }
}
