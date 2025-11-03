namespace Movie88.Application.DTOs.Promotions;

public class PromotionDTO
{
    public int Promotionid { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Startdate { get; set; } // Format: yyyy-MM-dd
    public string? Enddate { get; set; }   // Format: yyyy-MM-dd
    public string? Discounttype { get; set; }
    public decimal? Discountvalue { get; set; }
}
