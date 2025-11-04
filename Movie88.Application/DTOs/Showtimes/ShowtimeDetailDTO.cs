using Movie88.Application.DTOs.Cinemas;

namespace Movie88.Application.DTOs.Showtimes;

/// <summary>
/// Showtime details DTO with full information
/// </summary>
public class ShowtimeDetailDTO
{
    public int Showtimeid { get; set; }
    public int Movieid { get; set; }
    public int Auditoriumid { get; set; }
    public DateTime Starttime { get; set; }
    public DateTime? Endtime { get; set; }
    public decimal Price { get; set; }
    public string Format { get; set; } = null!;
    public string Languagetype { get; set; } = null!;
    
    // Related entities
    public MovieInfoDTO? Movie { get; set; }
    public CinemaInfoDTO? Cinema { get; set; }
    public AuditoriumInfoDTO? Auditorium { get; set; }
    public int AvailableSeats { get; set; }
}

/// <summary>
/// Movie information in showtime context
/// </summary>
public class MovieInfoDTO
{
    public int Movieid { get; set; }
    public string? Title { get; set; }
    public string? Posterurl { get; set; }
    public int Durationminutes { get; set; }
    public string? Rating { get; set; }
}

/// <summary>
/// Auditorium information in showtime context
/// </summary>
public class AuditoriumInfoDTO
{
    public int Auditoriumid { get; set; }
    public string? Name { get; set; }
    public int Seatscount { get; set; }
}

/// <summary>
/// Cinema information in showtime context (if not using from Cinemas folder)
/// </summary>
public class CinemaInfoDTO
{
    public int Cinemaid { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}
