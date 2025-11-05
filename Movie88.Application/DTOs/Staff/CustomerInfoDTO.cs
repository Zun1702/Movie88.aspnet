namespace Movie88.Application.DTOs.Staff;

/// <summary>
/// Customer information for staff booking verification
/// </summary>
public class CustomerInfoDTO
{
    public int CustomerId { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
