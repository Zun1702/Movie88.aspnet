namespace Movie88.Domain.Models;

public class VoucherModel
{
    public int Voucherid { get; set; }
    public string? Code { get; set; }
    public string? Discounttype { get; set; }
    public decimal? Discountvalue { get; set; }
    public DateOnly? Startdate { get; set; }
    public DateOnly? Enddate { get; set; }
    public int? Usagelimit { get; set; }
    public int? Usedcount { get; set; }
}
