namespace Movie88.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(int userId, string email, string fullName, string roleName);
        string GenerateRefreshToken();
        int? ValidateToken(string token);
        DateTime GetTokenExpiration(string token);
    }
}
