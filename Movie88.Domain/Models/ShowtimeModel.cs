namespace Movie88.Domain.Models;

public class ShowtimeModel
{
    public int Showtimeid { get; set; }
    public int Movieid { get; set; }
    public int Auditoriumid { get; set; }
    public DateTime? Starttime { get; set; }
    public DateTime? Endtime { get; set; }
    public decimal? Price { get; set; }
    public string? Format { get; set; }
    public string? Languagetype { get; set; }

    // Navigation properties
    public MovieModel? Movie { get; set; }
    public AuditoriumModel? Auditorium { get; set; }
}
