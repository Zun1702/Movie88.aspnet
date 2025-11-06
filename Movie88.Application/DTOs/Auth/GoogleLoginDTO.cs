namespace Movie88.Application.DTOs.Auth;

/// <summary>
/// Google login request containing ID token from Google Sign-In
/// </summary>
public class GoogleLoginRequestDTO
{
    /// <summary>
    /// Google ID Token received from Google Sign-In SDK
    /// </summary>
    public string IdToken { get; set; } = string.Empty;
}

/// <summary>
/// Google user profile information extracted from ID token
/// </summary>
public class GoogleUserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Picture { get; set; }
    public string GoogleId { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    
    // Additional fields (may be available via Google People API in future)
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; } // "male", "female", "other"
    public DateTime? Birthdate { get; set; }
}
