namespace Movie88.Application.DTOs.Bookings;

public class BookingListDTO
{
    public int Bookingid { get; set; }
    public int Customerid { get; set; }
    public int Showtimeid { get; set; }
    public MovieSummaryDTO? Movie { get; set; }
    public CinemaDTO? Cinema { get; set; }
    public ShowtimeDTO? Showtime { get; set; }
    public List<string>? Seats { get; set; }
    public List<ComboItemDTO>? Combos { get; set; }
    public string? VoucherCode { get; set; }
    public string? Bookingcode { get; set; }
    public decimal? Totalamount { get; set; }
    public string? Status { get; set; }
    public string? Bookingtime { get; set; }
}

public class MovieSummaryDTO
{
    public int Movieid { get; set; }
    public string? Title { get; set; }
    public string? Posterurl { get; set; }
}

public class CinemaDTO
{
    public int Cinemaid { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class ShowtimeDTO
{
    public string? Starttime { get; set; }
    public string? Format { get; set; }
    public string? Languagetype { get; set; }
    public string? Auditoriumname { get; set; }
}

public class ComboItemDTO
{
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public decimal? Price { get; set; }
}
