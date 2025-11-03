namespace Movie88.Application.DTOs.Customers;

public class CustomerProfileDTO
{
    public int Customerid { get; set; }
    public int Userid { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Dateofbirth { get; set; } // yyyy-MM-dd format
    public string? Gender { get; set; }
    public string Createdat { get; set; } = string.Empty; // ISO 8601 format
}
