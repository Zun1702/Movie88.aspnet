namespace Movie88.Domain.Models;

public class PromotionModel
{
    public int Promotionid { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateOnly? Startdate { get; set; }
    public DateOnly? Enddate { get; set; }
    public string? Discounttype { get; set; }
    public decimal? Discountvalue { get; set; }
}
