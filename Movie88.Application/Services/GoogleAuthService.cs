using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Movie88.Application.DTOs.Auth;
using Movie88.Application.Interfaces;

namespace Movie88.Application.Services;

/// <summary>
/// Google OAuth authentication service implementation
/// </summary>
public class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;

    public GoogleAuthService(IConfiguration configuration)
    {
        _clientId = configuration["GoogleOAuth:ClientId"] 
            ?? throw new InvalidOperationException("Google OAuth ClientId is not configured");
    }

    /// <summary>
    /// Verify Google ID token and extract user information
    /// </summary>
    public async Task<GoogleUserInfo> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            // Verify the ID token with Google
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

            // Extract user information from the payload
            return new GoogleUserInfo
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Name = payload.Name,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                Picture = payload.Picture
            };
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException("Invalid Google token", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to verify Google token", ex);
        }
    }
}
