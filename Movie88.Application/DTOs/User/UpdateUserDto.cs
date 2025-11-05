using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.User;

public class UpdateUserDto
{
    [Required]
    [MaxLength(100)]
    public string Fullname { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }
}