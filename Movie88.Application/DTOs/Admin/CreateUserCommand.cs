using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin
{
    public class CreateUserCommand
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fullname is required")]
        [StringLength(100, ErrorMessage = "Fullname cannot exceed 100 characters")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty; // "Staff" or "Admin"

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }
    }
}
