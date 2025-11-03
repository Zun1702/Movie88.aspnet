namespace Movie88.Domain.Models;

public class AuditoriumModel
{
    public int Auditoriumid { get; set; }
    public int Cinemaid { get; set; }
    public string? Name { get; set; }
    public int? Seatscount { get; set; }

    // Navigation properties
    public CinemaModel? Cinema { get; set; }
}
