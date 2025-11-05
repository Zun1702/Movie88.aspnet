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

    public async Task<bool> SendBookingConfirmationAsync(BookingConfirmationEmailDTO dto)
    {
        try
        {
            var emailHtml = GenerateBookingConfirmationHtml(dto);
            
            var emailRequest = new
            {
                from = "Movie88 <movie88@ezyfix.site>",
                to = new[] { dto.CustomerEmail },
                subject = $"Xác Nhận Đặt Vé - {dto.MovieTitle} - Movie88",
                html = emailHtml,
                attachments = new[]
                {
                    new
                    {
                        content = dto.QRCodeBase64,
                        filename = $"booking-qr-{dto.BookingCode}.png",
                        content_id = "qrcode" // For inline embedding with <img src="cid:qrcode">
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(emailRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("emails", httpContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Booking confirmation email sent successfully to {Email} for booking {BookingCode}",
                    dto.CustomerEmail,
                    dto.BookingCode
                );
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "Failed to send booking confirmation email: {StatusCode} - {Error}",
                response.StatusCode,
                errorContent
            );
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking confirmation email to {Email}", dto.CustomerEmail);
            return false;
        }
    }

    private string GenerateBookingConfirmationHtml(BookingConfirmationEmailDTO dto)
    {
        // Convert to Vietnam timezone (UTC+7)
        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var showtimeVietnam = TimeZoneInfo.ConvertTimeFromUtc(dto.ShowtimeDateTime.ToUniversalTime(), vietnamTimeZone);
        var paymentTimeVietnam = dto.PaymentTime.HasValue 
            ? TimeZoneInfo.ConvertTimeFromUtc(dto.PaymentTime.Value.ToUniversalTime(), vietnamTimeZone)
            : (DateTime?)null;
        
        // Format showtime
        var showtimeText = showtimeVietnam.ToString("dddd, dd MMMM yyyy - HH:mm");
        
        // Calculate prices
        var comboPrice = dto.ComboItems.Sum(c => c.Price * c.Quantity);
        var ticketPrice = dto.TotalAmount + dto.DiscountAmount - comboPrice;
        
        // Format combo items
        var comboItemsHtml = string.Empty;
        if (dto.ComboItems.Any())
        {
            var comboText = string.Join("<br>", dto.ComboItems.Select(c => $"<span style=\"color: #FFFFFF;\">{c.Name}</span> <span style=\"color: #FFB800;\">x{c.Quantity}</span>"));
            comboItemsHtml = $@"
                <tr>
                    <td style=""padding: 12px 0; color: #B3B3B3; font-size: 14px;"">Combos</td>
                    <td style=""padding: 12px 0; font-size: 14px;"">{comboText}</td>
                </tr>";
        }
        
        // Discount section
        var discountHtml = string.Empty;
        if (dto.DiscountAmount > 0 && !string.IsNullOrEmpty(dto.VoucherCode))
        {
            discountHtml = $@"
                <tr>
                    <td style=""padding: 10px 0; color: #FFB800; font-size: 14px;"">Giảm Giá ({dto.VoucherCode})</td>
                    <td style=""padding: 10px 0; color: #FFB800; text-align: right; font-size: 15px; font-weight: 600;"">-{dto.DiscountAmount:N0} VND</td>
                </tr>";
        }
        
        // Combo price row
        var comboPriceHtml = string.Empty;
        if (comboPrice > 0)
        {
            comboPriceHtml = $@"
                <tr>
                    <td style=""padding: 10px 0; color: #B3B3B3; font-size: 14px;"">Tổng Combo</td>
                    <td style=""padding: 10px 0; color: #FFFFFF; text-align: right; font-size: 15px;"">{comboPrice:N0} VND</td>
                </tr>";
        }

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Booking Confirmation - Movie88</title>
</head>
<body style=""margin: 0; padding: 0; font-family: 'Segoe UI', Arial, sans-serif; background-color: #141414;"">
    
    <!-- Header with Netflix Red -->
    <div style=""background: linear-gradient(135deg, #E50914 0%, #B20710 100%); padding: 40px 20px; text-align: center; box-shadow: 0 4px 12px rgba(229,9,20,0.3);"">
        <h1 style=""color: #FFFFFF; margin: 0; font-size: 36px; font-weight: bold; letter-spacing: 1px;"">Movie88</h1>
        <p style=""color: rgba(255,255,255,0.95); margin: 10px 0 0 0; font-size: 18px; font-weight: 500;"">Xác Nhận Đặt Vé</p>
    </div>
    
    <!-- Main Content -->
    <div style=""max-width: 600px; margin: 0 auto; background-color: #1F1F1F; border-radius: 12px; margin-top: -20px; box-shadow: 0 8px 24px rgba(0,0,0,0.4); overflow: hidden;"">
        
        <!-- BookingCode Section -->
        <div style=""padding: 30px 20px; text-align: center; border-bottom: 2px dashed #2B2B2B;"">
            <h2 style=""color: #B3B3B3; margin: 0 0 15px 0; font-size: 16px; text-transform: uppercase; letter-spacing: 1px;"">Mã Đặt Vé</h2>
            <div style=""background: linear-gradient(135deg, #E50914 0%, #B20710 100%); color: #FFFFFF; padding: 20px; border-radius: 12px; font-size: 32px; font-weight: bold; letter-spacing: 3px; box-shadow: 0 4px 12px rgba(229,9,20,0.4);"">
                {dto.BookingCode}
            </div>
        </div>
        
        <!-- QR Code Section -->
        <div style=""padding: 30px 20px; text-align: center; background-color: #141414;"">
            <h3 style=""color: #B3B3B3; margin: 0 0 25px 0; font-size: 18px; text-transform: uppercase; letter-spacing: 1px;"">Mã QR Của Bạn</h3>
            <div style=""display: inline-block; padding: 15px; background: #FFFFFF; border-radius: 12px; box-shadow: 0 4px 16px rgba(229,9,20,0.3);"">
                <img src=""cid:qrcode"" alt=""Mã QR Đặt Vé"" style=""width: 280px; height: 280px; display: block; border-radius: 8px;"">
            </div>
            <p style=""color: #B3B3B3; margin: 25px 0 0 0; font-size: 14px; line-height: 1.6;"">
                 Xuất trình mã QR này tại cửa rạp<br>
                 Chụp màn hình để sử dụng khi offline
            </p>
        </div>
        
        <!-- Booking Details -->
        <div style=""padding: 30px 20px; background-color: #1F1F1F;"">
            <h3 style=""color: #FFFFFF; margin: 0 0 25px 0; border-bottom: 2px solid #E50914; padding-bottom: 12px; font-size: 20px; text-transform: uppercase; letter-spacing: 1px;"">Thông Tin Đặt Vé</h3>
            
            <table style=""width: 100%; border-collapse: collapse;"">
                <tr>
                    <td style=""padding: 12px 0; color: #B3B3B3; width: 35%; font-size: 14px;""> Phim</td>
                    <td style=""padding: 12px 0; color: #FFFFFF; font-weight: bold; font-size: 15px;"">{dto.MovieTitle}</td>
                </tr>
                <tr>
                    <td style=""padding: 12px 0; color: #B3B3B3; font-size: 14px;"">Rạp Chiếu</td>
                    <td style=""padding: 12px 0; color: #FFFFFF; font-size: 15px;"">{dto.CinemaName}</td>
                </tr>
                <tr>
                    <td style=""padding: 12px 0; color: #B3B3B3; font-size: 14px;"">Địa Chỉ</td>
                    <td style=""padding: 12px 0; color: #B3B3B3; font-size: 14px;"">{dto.CinemaAddress}</td>
                </tr>
                <tr>
                    <td style=""padding: 12px 0; color: #B3B3B3; font-size: 14px;"">Ngày & Giờ Chiếu</td>
                    <td style=""padding: 12px 0; color: #FFB800; font-weight: bold; font-size: 15px;"">{showtimeText}</td>
                </tr>
                <tr>
                    <td style=""padding: 12px 0; color: #B3B3B3; font-size: 14px;"">Ghế Ngồi</td>
                    <td style=""padding: 12px 0; color: #FFFFFF; font-weight: bold; font-size: 16px;"">{dto.SeatNumbers}</td>
                </tr>
                {comboItemsHtml}
            </table>
        </div>
        
        <!-- Payment Summary -->
        <div style=""padding: 30px 20px; background-color: #141414; border-radius: 12px; border: 1px solid #2B2B2B;"">
            <h3 style=""color: #FFFFFF; margin: 0 0 25px 0; border-bottom: 2px solid #E50914; padding-bottom: 12px; font-size: 20px; text-transform: uppercase; letter-spacing: 1px;"">Thông Tin Thanh Toán</h3>
            
            <table style=""width: 100%; border-collapse: collapse;"">
                <tr>
                    <td style=""padding: 10px 0; color: #B3B3B3; font-size: 14px;"">Giá Vé</td>
                    <td style=""padding: 10px 0; color: #FFFFFF; text-align: right; font-size: 15px;"">{ticketPrice:N0} VND</td>
                </tr>
                {comboPriceHtml}
                {discountHtml}
                <tr style=""border-top: 2px solid #2B2B2B;"">
                    <td style=""padding: 18px 0 5px 0; color: #FFFFFF; font-size: 20px; font-weight: bold;"">Tổng Thanh Toán</td>
                    <td style=""padding: 18px 0 5px 0; color: #E50914; font-size: 24px; font-weight: bold; text-align: right;"">{dto.TotalAmount:N0} VND</td>
                </tr>
                <tr>
                    <td style=""padding: 10px 0; color: #B3B3B3; font-size: 13px; padding-top: 20px;"">Phương Thức</td>
                    <td style=""padding: 10px 0; color: #FFB800; font-size: 14px; text-align: right; padding-top: 20px; font-weight: 600;"">VNPay - {dto.TransactionCode}</td>
                </tr>
                <tr>
                    <td style=""padding: 8px 0; color: #808080; font-size: 12px;"">Thời Gian TT</td>
                    <td style=""padding: 8px 0; color: #808080; font-size: 12px; text-align: right;"">{paymentTimeVietnam?.ToString("dd/MM/yyyy HH:mm:ss")} (GMT+7)</td>
                </tr>
            </table>
        </div>
        
        <!-- Important Information -->
        <div style=""padding: 30px 20px; border-top: 2px solid #2B2B2B; background-color: #1F1F1F;"">
            <h3 style=""color: #FFB800; margin: 0 0 20px 0; font-size: 18px;"">⚠️ Lưu Ý Quan Trọng</h3>
            <ul style=""color: #B3B3B3; line-height: 2; padding-left: 20px; margin: 0; font-size: 14px;"">
                <li>Vui lòng có mặt <strong style=""color: #FFFFFF;"">trước 15 phút</strong> so với giờ chiếu</li>
                <li>Xuất trình <strong style=""color: #FFFFFF;"">Mã QR hoặc Mã Đặt Vé</strong> tại cửa rạp</li>
                <li>Không hoàn tiền sau <strong style=""color: #FFFFFF;"">24 giờ trước giờ chiếu</strong></li>
                <li>Liên hệ hỗ trợ: <a href=""mailto:support@movie88.com"" style=""color: #E50914; text-decoration: none;"">support@movie88.com</a></li>
            </ul>
        </div>
        
        <!-- Call to Action -->
        <div style=""padding: 40px 20px; text-align: center; background: linear-gradient(135deg, #E50914 0%, #B20710 100%); box-shadow: 0 -2px 12px rgba(229,9,20,0.2);"">
            <p style=""color: #FFFFFF; margin: 0 0 15px 0; font-size: 20px; font-weight: bold;"">Chúc Bạn Xem Phim Vui Vẻ!🍿</p>
            <p style=""color: rgba(255,255,255,0.9); margin: 0; font-size: 16px;"">Cảm ơn bạn đã chọn Movie88</p>
        </div>
    </div>
    
    <!-- Footer -->
    <div style=""max-width: 600px; margin: 20px auto; padding: 25px 20px; text-align: center; color: #808080; font-size: 12px; background-color: #141414; border-radius: 8px; border: 1px solid #2B2B2B;"">
        <p style=""margin: 0 0 10px 0; color: #B3B3B3;"">Đây là email tự động từ Movie88. Vui lòng không trả lời email này.</p>
        <p style=""margin: 0; color: #808080;"">© 2025 Movie88. Bản quyền thuộc về Movie88.</p>
    </div>
    
</body>
</html>";
    }
}
