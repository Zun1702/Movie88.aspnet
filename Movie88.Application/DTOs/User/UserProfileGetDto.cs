namespace Movie88.Application.DTOs.User;

public class UserProfileGetDto
{
    public int Userid { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int Roleid { get; set; }
    public string Rolename { get; set; } = string.Empty;
    public DateTime? Createdat { get; set; }
}