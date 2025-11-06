using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Movies;

/// <summary>
/// DTO for updating an existing movie (Admin only)
/// All fields are optional for partial updates
/// </summary>
public class UpdateMovieDto
{
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
    public int? DurationMinutes { get; set; }

    [MaxLength(100, ErrorMessage = "Director name cannot exceed 100 characters")]
    public string? Director { get; set; }

    public string? ReleaseDate { get; set; } // Format: yyyy-MM-dd

    [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    [MaxLength(10, ErrorMessage = "Rating cannot exceed 10 characters")]
    public string? Rating { get; set; }

    [MaxLength(255, ErrorMessage = "Genre cannot exceed 255 characters")]
    public string? Genre { get; set; }

    [MaxLength(255, ErrorMessage = "Poster URL cannot exceed 255 characters")]
    public string? PosterUrl { get; set; }

    [MaxLength(255, ErrorMessage = "Trailer URL cannot exceed 255 characters")]
    public string? TrailerUrl { get; set; }
}
