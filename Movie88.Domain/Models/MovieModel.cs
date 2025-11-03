namespace Movie88.Domain.Models;

/// <summary>
/// Domain model for Movie entity
/// Represents a movie in the cinema system
/// </summary>
public class MovieModel
{
    public int Movieid { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Durationminutes { get; set; }
    public string? Director { get; set; }
    public string? Trailerurl { get; set; }
    public DateOnly? Releasedate { get; set; }
    public string? Posterurl { get; set; }
    public string? Country { get; set; }
    public string Rating { get; set; } = null!; // Age rating: G, PG, PG-13, R
    public string? Genre { get; set; }
    public DateTime? Createdat { get; set; }
}
