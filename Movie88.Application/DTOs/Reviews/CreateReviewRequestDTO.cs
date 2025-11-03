using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Reviews;

public class CreateReviewRequestDTO
{
    [Required(ErrorMessage = "Movie ID is required")]
    public int Movieid { get; set; }
    
    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }
    
    [MaxLength(500, ErrorMessage = "Comment must not exceed 500 characters")]
    public string? Comment { get; set; }
}
