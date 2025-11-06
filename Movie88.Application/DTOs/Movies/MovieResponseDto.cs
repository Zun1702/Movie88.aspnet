namespace Movie88.Application.DTOs.Movies;

/// <summary>
/// Response DTO for movie CRUD operations
/// </summary>
public class MovieResponseDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public string? Director { get; set; }
    public string? ReleaseDate { get; set; } // Format: yyyy-MM-dd
    public string? Country { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string? Genre { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Admin movie DTO with aggregated statistics
/// Used in GET /api/admin/movies
/// </summary>
public class AdminMovieDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Director { get; set; }
    public string? ReleaseDate { get; set; }
    public string? Country { get; set; }
    public string? Genre { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    
    // Aggregated statistics
    public int TotalShowtimes { get; set; }
    public int TotalBookings { get; set; }
    public decimal Revenue { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
}
