using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movie88.Application.DTOs.Email;
using Movie88.Application.Interfaces;
using Movie88.Domain.Models;

namespace Movie88.Application.Services;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResendEmailService> _logger;
    private readonly string _apiKey;
    private readonly string _endpoint;

    public ResendEmailService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ResendEmailService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["Resend:ApiKey"] ?? throw new InvalidOperationException("Resend API Key not configured");
        _endpoint = configuration["Resend:Endpoint"] ?? "https://api.resend.com";
        
        // Configure HttpClient
        _httpClient.BaseAddress = new Uri(_endpoint);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<bool> SendOtpEmailAsync(string toEmail, string otpCode, string otpType)
    {
        try
        {
            var (subject, htmlBody) = GetOtpEmailContent(otpCode, otpType);
            
            var emailRequest = new ResendEmailRequest
            {
                From = "Movie88 <movie88@ezyfix.site>",
                To = new List<string> { toEmail },
                Subject = subject,
                Html = htmlBody
            };

            var json = JsonSerializer.Serialize(emailRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/emails", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Email sent successfully to {Email}. Response: {Response}", toEmail, responseBody);
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send email to {Email}. Status: {Status}, Error: {Error}", 
                    toEmail, response.StatusCode, errorBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string fullname)
    {
        try
        {
            var htmlBody = GetWelcomeEmailContent(fullname);
            
            var emailRequest = new ResendEmailRequest
            {
                From = "Movie88 <movie88@ezyfix.site>",
                To = new List<string> { toEmail },
                Subject = "🎬 Welcome to Movie88!",
                Html = htmlBody
            };

            var json = JsonSerializer.Serialize(emailRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/emails", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Welcome email sent successfully to {Email}", toEmail);
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send welcome email. Status: {Status}, Error: {Error}", 
                    response.StatusCode, errorBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending welcome email to {Email}", toEmail);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetConfirmationAsync(string toEmail, string fullname)
    {
        try
        {
            var htmlBody = GetPasswordResetConfirmationContent(fullname);
            
            var emailRequest = new ResendEmailRequest
            {
                From = "Movie88 <movie88@ezyfix.site>",
                To = new List<string> { toEmail },
                Subject = "🔒 Password Reset Successful - Movie88",
                Html = htmlBody
            };

            var json = JsonSerializer.Serialize(emailRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/emails", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password reset confirmation sent to {Email}", toEmail);
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send password reset confirmation. Status: {Status}, Error: {Error}", 
                    response.StatusCode, errorBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending password reset confirmation to {Email}", toEmail);
            return false;
        }
    }

    private (string subject, string htmlBody) GetOtpEmailContent(string otpCode, string otpType)
    {
        var subject = otpType switch
        {
            OtpTypeConstants.EmailVerification => "🔐 Verify Your Email - Movie88",
            OtpTypeConstants.PasswordReset => "🔑 Reset Your Password - Movie88",
            OtpTypeConstants.Login => "🔒 Login Verification - Movie88",
            _ => "🔐 OTP Code - Movie88"
        };

        var title = otpType switch
        {
            OtpTypeConstants.EmailVerification => "Verify Your Email Address",
            OtpTypeConstants.PasswordReset => "Reset Your Password",
            OtpTypeConstants.Login => "Login Verification",
            _ => "One-Time Password"
        };

        var description = otpType switch
        {
            OtpTypeConstants.EmailVerification => "Thank you for registering with Movie88! Please use the following code to verify your email address:",
            OtpTypeConstants.PasswordReset => "You requested to reset your password. Please use the following code to proceed:",
            OtpTypeConstants.Login => "A login attempt requires verification. Please use the following code:",
            _ => "Your one-time password code:"
        };

        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{subject}</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td align=""center"" style=""padding: 40px 0;"">
                <table role=""presentation"" style=""width: 600px; border-collapse: collapse; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""padding: 40px 30px; text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 8px 8px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">🎬 Movie88</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #333333; font-size: 24px;"">{title}</h2>
                            <p style=""margin: 0 0 20px 0; color: #666666; font-size: 16px; line-height: 1.5;"">{description}</p>
                            
                            <!-- OTP Code Box -->
                            <table role=""presentation"" style=""width: 100%; margin: 30px 0;"">
                                <tr>
                                    <td align=""center"" style=""padding: 30px; background-color: #f8f9fa; border-radius: 8px; border: 2px dashed #667eea;"">
                                        <div style=""font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 8px; font-family: 'Courier New', monospace;"">{otpCode}</div>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""margin: 20px 0; color: #666666; font-size: 14px; line-height: 1.5;"">
                                <strong>⏰ This code will expire in 10 minutes.</strong>
                            </p>
                            
                            <p style=""margin: 20px 0; color: #666666; font-size: 14px; line-height: 1.5;"">
                                If you didn't request this code, please ignore this email or contact our support team if you have concerns.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 8px 8px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                © 2025 Movie88. All rights reserved.<br>
                                This is an automated email, please do not reply.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        return (subject, htmlBody);
    }

    private string GetWelcomeEmailContent(string fullname)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to Movie88</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td align=""center"" style=""padding: 40px 0;"">
                <table role=""presentation"" style=""width: 600px; border-collapse: collapse; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <tr>
                        <td style=""padding: 40px 30px; text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 8px 8px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">🎬 Welcome to Movie88!</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #333333; font-size: 24px;"">Hi {fullname}! 👋</h2>
                            <p style=""margin: 0 0 20px 0; color: #666666; font-size: 16px; line-height: 1.5;"">
                                Thank you for verifying your email address! Your Movie88 account is now fully activated.
                            </p>
                            <p style=""margin: 0 0 20px 0; color: #666666; font-size: 16px; line-height: 1.5;"">
                                You can now enjoy:
                            </p>
                            <ul style=""color: #666666; font-size: 16px; line-height: 1.8; margin: 0 0 20px 20px;"">
                                <li>🎟️ Book movie tickets easily</li>
                                <li>⭐ Rate and review movies</li>
                                <li>🎁 Exclusive promotions and vouchers</li>
                                <li>📱 Manage your bookings</li>
                            </ul>
                            <p style=""margin: 20px 0; color: #666666; font-size: 16px; line-height: 1.5;"">
                                Start exploring and booking your favorite movies today!
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 8px 8px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                © 2025 Movie88. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private string GetPasswordResetConfirmationContent(string fullname)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Password Reset Successful</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse;"">
        <tr>
            <td align=""center"" style=""padding: 40px 0;"">
                <table role=""presentation"" style=""width: 600px; border-collapse: collapse; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <tr>
                        <td style=""padding: 40px 30px; text-align: center; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 8px 8px 0 0;"">
                            <h1 style=""margin: 0; color: #ffffff; font-size: 28px; font-weight: bold;"">🔒 Password Reset Successful</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""margin: 0 0 20px 0; color: #333333; font-size: 24px;"">Hi {fullname},</h2>
                            <p style=""margin: 0 0 20px 0; color: #666666; font-size: 16px; line-height: 1.5;"">
                                Your password has been successfully reset. You can now login with your new password.
                            </p>
                            <p style=""margin: 20px 0; color: #666666; font-size: 14px; line-height: 1.5;"">
                                If you didn't make this change, please contact our support team immediately.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 30px; text-align: center; background-color: #f8f9fa; border-radius: 0 0 8px 8px;"">
                            <p style=""margin: 0; color: #999999; font-size: 12px;"">
                                © 2025 Movie88. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
