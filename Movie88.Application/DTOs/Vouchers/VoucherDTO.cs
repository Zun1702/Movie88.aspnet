namespace Movie88.Application.DTOs.Vouchers;

// Request DTO for validating voucher
public class ValidateVoucherRequestDTO
{
    public string Code { get; set; } = null!;
    public int Bookingid { get; set; }
}

// Response DTO for voucher validation
public class ValidateVoucherResponseDTO
{
    public int Voucherid { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Discounttype { get; set; }
    public decimal? Discountvalue { get; set; }
    public decimal? Minpurchaseamount { get; set; }
    public DateOnly? Expirydate { get; set; }
    public int? Usagelimit { get; set; }
    public int? Usedcount { get; set; }
    public bool? Isactive { get; set; }
    public decimal ApplicableDiscount { get; set; } // Calculated discount for this booking
}

// Request DTO for applying voucher to booking
public class ApplyVoucherRequestDTO
{
    public string Code { get; set; } = null!;
}

// Response DTO for applying voucher
public class ApplyVoucherResponseDTO
{
    public int Bookingid { get; set; }
    public int Voucherid { get; set; }
    public string VoucherCode { get; set; } = null!;
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Totalamount { get; set; }
}
