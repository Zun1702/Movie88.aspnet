namespace Movie88.Domain.Models;

public class BookingModel
{
    public int Bookingid { get; set; }
    public int Customerid { get; set; }
    public int Showtimeid { get; set; }
    public int? Voucherid { get; set; }
    public string? Bookingcode { get; set; }
    public DateTime? Bookingtime { get; set; }
    public decimal? Totalamount { get; set; }
    public string? Status { get; set; }
    
    // Check-in tracking (staff verification at cinema)
    public DateTime? Checkedintime { get; set; }
    public int? Checkedinby { get; set; }

    // Navigation properties
    public CustomerModel? Customer { get; set; }
    public ShowtimeModel? Showtime { get; set; }
    public VoucherModel? Voucher { get; set; }
    public UserModel? CheckedInByUser { get; set; }
    public List<BookingSeatModel>? BookingSeats { get; set; }
    public List<BookingComboModel>? BookingCombos { get; set; }
    public List<PaymentModel>? Payments { get; set; }
}
