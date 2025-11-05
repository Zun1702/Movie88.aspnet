namespace Movie88.Domain.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Passwordhash { get; set; } = null!;
        public string? Phone { get; set; }
        public int Roleid { get; set; }
        public DateTime? Createdat { get; set; }
        public DateTime? Updatedat { get; set; }
        
        // OTP Verification fields
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? VerifiedAt { get; set; }
        
        // Navigation properties
        public RoleModel? Role { get; set; }
    }
}
