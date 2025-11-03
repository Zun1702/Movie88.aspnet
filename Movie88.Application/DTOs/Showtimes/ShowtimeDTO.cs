namespace Movie88.Application.DTOs.Showtimes;

public class ShowtimesByDateDTO
{
    public string Date { get; set; } = string.Empty;
    public List<ShowtimesByCinemaDTO> Cinemas { get; set; } = new();
}

public class ShowtimesByCinemaDTO
{
    public CinemaInfoDTO Cinema { get; set; } = new();
    public List<ShowtimeItemDTO> Showtimes { get; set; } = new();
}

public class CinemaInfoDTO
{
    public int Cinemaid { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}

public class ShowtimeItemDTO
{
    public int Showtimeid { get; set; }
    public DateTime? Starttime { get; set; }
    public decimal? Price { get; set; }
    public string? Format { get; set; }
    public string? Languagetype { get; set; }
    public string? AuditoriumName { get; set; }
    public int AvailableSeats { get; set; }
}
