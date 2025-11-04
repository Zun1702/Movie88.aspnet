namespace Movie88.Application.DTOs.Admin
{
    public class GetUsersQuery
    {
        public string? Role { get; set; } // all, customer, staff, admin
        public string? Status { get; set; } // all, active, inactive
        public string? Search { get; set; } // Search by email or fullname
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
