namespace Movie88.Application.DTOs.Promotions;

/// <summary>
/// DTO representing a promotion that has been applied to a booking
/// </summary>
public class AppliedPromotionDTO
{
    /// <summary>
    /// Promotion ID
    /// </summary>
    public int Promotionid { get; set; }
    
    /// <summary>
    /// Promotion name (e.g., "Khuyến Mãi Tháng 11")
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Promotion description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Actual discount amount applied to this booking (in VND)
    /// </summary>
    public decimal Discountapplied { get; set; }
}
