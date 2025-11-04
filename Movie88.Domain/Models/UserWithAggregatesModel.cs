namespace Movie88.Domain.Models
{
    public class UserWithAggregatesModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public int Roleid { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime? Createdat { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }

        // Aggregated data
        public int TotalBookings { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
