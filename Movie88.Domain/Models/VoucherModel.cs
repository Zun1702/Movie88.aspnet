namespace Movie88.Domain.Models;

public class VoucherModel
{
    public int Voucherid { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Discounttype { get; set; } // "percentage" or "fixed"
    public decimal? Discountvalue { get; set; }
    public decimal? Minpurchaseamount { get; set; }
    public DateOnly? Expirydate { get; set; }
    public int? Usagelimit { get; set; }
    public int? Usedcount { get; set; }
    public bool? Isactive { get; set; }
}
