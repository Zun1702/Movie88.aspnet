using System.ComponentModel.DataAnnotations;

namespace Movie88.Application.DTOs.Customers;

public class UpdateCustomerProfileDto
{
    [MaxLength(255)]
    public string? Address { get; set; }

    public string? DateOfBirth { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }
}

public class CustomerProfileResponseDto
{
    public int Customerid { get; set; }
    public int Userid { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
}