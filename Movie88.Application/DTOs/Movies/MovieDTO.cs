namespace Movie88.Application.DTOs.Movies;

/// <summary>
/// DTO for movie list item
/// Used in GET /api/movies, /api/movies/now-showing, /api/movies/coming-soon
/// </summary>
public class MovieDTO
{
    public int Movieid { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Durationminutes { get; set; }
    public string? Director { get; set; }
    public string? Trailerurl { get; set; }
    public string? Releasedate { get; set; } // yyyy-MM-dd format
    public string? Posterurl { get; set; }
    public string? Country { get; set; }
    public string Rating { get; set; } = null!; // Age rating: G, PG, PG-13, R
    public string? Genre { get; set; }
}
