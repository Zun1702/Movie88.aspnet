namespace Movie88.Domain.Models;

public class CustomerModel
{
    public int Customerid { get; set; }
    public int Userid { get; set; }
    public string? Address { get; set; }
    public DateOnly? Dateofbirth { get; set; }
    public string? Gender { get; set; }
    
    // Navigation property
    public UserModel? User { get; set; }
    
    // From User table (for flat queries)
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime Createdat { get; set; }
}
