using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin
{
    public class UpdateUserRoleCommand
    {
        [Required(ErrorMessage = "NewRole is required")]
        public string NewRole { get; set; } = string.Empty; // "Customer", "Staff", or "Admin"
    }
}
