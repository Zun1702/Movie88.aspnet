namespace Movie88.Application.DTOs.Showtimes;

/// <summary>
/// Response for GET /api/showtimes/by-movie/{movieId}
/// </summary>
public class ShowtimesByMovieResponseDTO
{
    public MovieInfoDTO Movie { get; set; } = null!;
    public List<ShowtimesByDateGroupDTO> ShowtimesByDate { get; set; } = new();
}

/// <summary>
/// Showtimes grouped by date
/// </summary>
public class ShowtimesByDateGroupDTO
{
    public string Date { get; set; } = string.Empty; // yyyy-MM-dd format
    public List<ShowtimesByCinemaGroupDTO> Cinemas { get; set; } = new();
}

/// <summary>
/// Showtimes grouped by cinema within a date
/// </summary>
public class ShowtimesByCinemaGroupDTO
{
    public int Cinemaid { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public List<ShowtimeItemDTO> Showtimes { get; set; } = new();
}

/// <summary>
/// Individual showtime item
/// </summary>
public class ShowtimeItemDTO
{
    public int Showtimeid { get; set; }
    public DateTime Starttime { get; set; }
    public DateTime? Endtime { get; set; }
    public decimal Price { get; set; }
    public string Format { get; set; } = null!;
    public string Languagetype { get; set; } = null!;
    public int Auditoriumid { get; set; }
    public string Auditoriumname { get; set; } = null!;
    public int AvailableSeats { get; set; }
}
