namespace Movie88.Domain.Models;

public class BookingComboModel
{
    public int Bookingcomboid { get; set; }
    public int Bookingid { get; set; }
    public int Comboid { get; set; }
    public int Quantity { get; set; }
    public decimal? Comboprice { get; set; }

    // Navigation properties
    public ComboModel? Combo { get; set; }
}
