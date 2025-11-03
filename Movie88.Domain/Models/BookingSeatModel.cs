namespace Movie88.Domain.Models;

public class BookingSeatModel
{
    public int Bookingseatid { get; set; }
    public int Bookingid { get; set; }
    public int Showtimeid { get; set; }
    public int Seatid { get; set; }
    public decimal? Seatprice { get; set; }

    // Navigation properties
    public SeatModel? Seat { get; set; }
}
