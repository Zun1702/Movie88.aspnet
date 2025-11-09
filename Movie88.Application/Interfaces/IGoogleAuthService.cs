using Google.Apis.Auth;
using Movie88.Application.DTOs.Auth;

namespace Movie88.Application.Interfaces;

/// <summary>
/// Service for Google OAuth authentication
/// </summary>
public interface IGoogleAuthService
{
    /// <summary>
    /// Verify Google ID token and extract user information
    /// </summary>
    /// <param name="idToken">ID token from Google Sign-In</param>
    /// <returns>Google user information</returns>
    Task<GoogleUserInfo> VerifyGoogleTokenAsync(string idToken);
}
