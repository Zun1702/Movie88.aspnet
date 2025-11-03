namespace Movie88.Application.DTOs.Movies;

public class MovieDetailDTO
{
    public int Movieid { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Releasedate { get; set; }
    public int? Durationminutes { get; set; } // Fixed: Changed from Duration to Durationminutes
    public string? Country { get; set; }
    public string? Director { get; set; }
    public string? Genre { get; set; }
    public string? Rating { get; set; }
    public string? Posterurl { get; set; }
    public string? Trailerurl { get; set; }
    
    // Computed fields
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalShowtimes { get; set; }
}
