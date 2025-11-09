using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Cinemas;

/// <summary>
/// DTO for creating a new cinema (Admin only)
/// Uses minimal fields available in current entity
/// </summary>
public class CreateCinemaDto
{
    [Required(ErrorMessage = "Cinema name is required")]
    [MaxLength(100, ErrorMessage = "Cinema name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal? Longitude { get; set; }
}

/// <summary>
/// DTO for updating an existing cinema (Admin only)
/// All fields are optional for partial updates
/// </summary>
public class UpdateCinemaDto
{
    [MaxLength(100, ErrorMessage = "Cinema name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    public string? Address { get; set; }

    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal? Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal? Longitude { get; set; }
}

/// <summary>
/// Response DTO for cinema operations
/// </summary>
public class CinemaResponseDto
{
    public int CinemaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Phone { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int NumberOfAuditoriums { get; set; }
    public DateTime? CreatedAt { get; set; }
}
