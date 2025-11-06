namespace Movie88.Application.DTOs.Bookings;

using Movie88.Application.DTOs.Promotions;

public class CreateBookingRequestDTO
{
    public int Showtimeid { get; set; }
    public List<int> Seatids { get; set; } = new();
}

public class BookingResponseDTO
{
    public int Bookingid { get; set; }
    public string? Bookingcode { get; set; } // Null until payment confirmed
    public int Showtimeid { get; set; }
    public List<BookedSeatDTO> Seats { get; set; } = new();
    public decimal Totalamount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Createdat { get; set; }
    public List<AppliedPromotionDTO>? AppliedPromotions { get; set; }
}

public class BookedSeatDTO
{
    public int Seatid { get; set; }
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public decimal Seatprice { get; set; }
}
