namespace Movie88.Domain.Models;

public class AuditoriumModel
{
    public int Auditoriumid { get; set; }
    public int Cinemaid { get; set; }
    public string? Name { get; set; }
    public int? Seatscount { get; set; }
    
    /// <summary>
    /// Alias for Seatscount - used for consistency in DTOs
    /// </summary>
    public int? Totalseats => Seatscount;

    // Navigation properties
    public CinemaModel? Cinema { get; set; }
}
