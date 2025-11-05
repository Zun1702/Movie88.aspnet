namespace Movie88.Application.DTOs.Cinemas;

/// <summary>
/// Cinema data transfer object
/// </summary>
public class CinemaDTO
{
    public int Cinemaid { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public DateTime? Createdat { get; set; }
}
