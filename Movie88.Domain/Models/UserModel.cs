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
        public RoleModel? Role { get; set; }
    }
}
