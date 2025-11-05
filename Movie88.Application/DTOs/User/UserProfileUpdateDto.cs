namespace Movie88.Application.DTOs.User;

public class UserProfileUpdateDto
{
    public int Userid { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Rolename { get; set; } = string.Empty;
    public DateTime? Updatedat { get; set; }
}