using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Admin
{
    public class BanUserCommand
    {
        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; } // false = ban, true = unban
    }
}
