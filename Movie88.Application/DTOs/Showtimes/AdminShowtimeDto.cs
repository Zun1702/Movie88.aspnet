using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Showtimes;

/// <summary>
/// DTO for creating a single showtime (Admin only)
/// </summary>
public class CreateShowtimeDto
{
    [Required(ErrorMessage = "Movie ID is required")]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Auditorium ID is required")]
    public int AuditoriumId { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Format is required")]
    [MaxLength(20, ErrorMessage = "Format cannot exceed 20 characters")]
    public string Format { get; set; } = string.Empty; // 2D, 3D, IMAX

    [MaxLength(50, ErrorMessage = "Language cannot exceed 50 characters")]
    public string? Language { get; set; } // Audio language

    [MaxLength(50, ErrorMessage = "Subtitle cannot exceed 50 characters")]
    public string? Subtitle { get; set; } // Subtitle language

    [Required(ErrorMessage = "Base price is required")]
    [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000")]
    public decimal BasePrice { get; set; }
}

/// <summary>
/// DTO for bulk creating showtimes (weekly scheduling)
/// </summary>
public class BulkCreateShowtimeDto
{
    [Required(ErrorMessage = "Movie ID is required")]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Auditorium ID is required")]
    public int AuditoriumId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateOnly EndDate { get; set; }

    [Required(ErrorMessage = "At least one timeslot is required")]
    [MinLength(1, ErrorMessage = "At least one timeslot is required")]
    public List<TimeOnly> Timeslots { get; set; } = new();

    public List<DayOfWeek> SkipDays { get; set; } = new(); // Days to skip

    [Required(ErrorMessage = "Format is required")]
    [MaxLength(20, ErrorMessage = "Format cannot exceed 20 characters")]
    public string Format { get; set; } = string.Empty;

    [Required(ErrorMessage = "Language type is required")]
    [MaxLength(50, ErrorMessage = "Language type cannot exceed 50 characters")]
    public string LanguageType { get; set; } = string.Empty; // Combined: "English - Phụ đề Việt"

    [Required(ErrorMessage = "Pricing is required")]
    public ShowtimePricingDto Pricing { get; set; } = new();
}

public class ShowtimePricingDto
{
    [Required(ErrorMessage = "Weekday price is required")]
    [Range(0, 1000000, ErrorMessage = "Weekday price must be between 0 and 1,000,000")]
    public decimal Weekday { get; set; }

    [Required(ErrorMessage = "Weekend price is required")]
    [Range(0, 1000000, ErrorMessage = "Weekend price must be between 0 and 1,000,000")]
    public decimal Weekend { get; set; }
}

/// <summary>
/// Response DTO for showtime operations
/// </summary>
public class ShowtimeResponseDto
{
    public int ShowtimeId { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public int AuditoriumId { get; set; }
    public string AuditoriumName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal Price { get; set; }
    public string Format { get; set; } = string.Empty;
    public string LanguageType { get; set; } = string.Empty;
    public int AvailableSeats { get; set; }
}

/// <summary>
/// Response DTO for bulk showtime creation
/// </summary>
public class BulkShowtimeResponseDto
{
    public int Created { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public List<ShowtimeCreationDetailDto> Details { get; set; } = new();
}

public class ShowtimeCreationDetailDto
{
    public string Date { get; set; } = string.Empty;
    public List<string> Timeslots { get; set; } = new();
    public decimal Price { get; set; }
    public int Created { get; set; }
}
