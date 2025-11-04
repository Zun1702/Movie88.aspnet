namespace Movie88.Application.DTOs.Admin
{
    public class UserListItemDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
