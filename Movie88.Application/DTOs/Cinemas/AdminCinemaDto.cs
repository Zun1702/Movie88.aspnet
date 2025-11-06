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
    public int NumberOfAuditoriums { get; set; }
    public DateTime? CreatedAt { get; set; }
}
