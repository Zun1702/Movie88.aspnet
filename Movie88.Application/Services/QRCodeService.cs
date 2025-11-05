using QRCoder;
using Movie88.Application.Interfaces;

namespace Movie88.Application.Services;

/// <summary>
/// Service for generating QR codes from booking codes
/// Uses QRCoder library with high error correction level
/// </summary>
public class QRCodeService : IQRCodeService
{
    /// <summary>
    /// Generate QR code as Base64 string for email embedding
    /// </summary>
    public async Task<string> GenerateQRCodeBase64Async(string bookingCode)
    {
        return await Task.Run(() =>
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(
                bookingCode, 
                QRCodeGenerator.ECCLevel.Q // 25% error correction
            );
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            // 20 pixels per module = 300x300px final size (15x15 modules)
            var qrCodeBytes = qrCode.GetGraphic(20);
            return Convert.ToBase64String(qrCodeBytes);
        });
    }
    
    /// <summary>
    /// Generate QR code as byte array for API response
    /// </summary>
    public async Task<byte[]> GenerateQRCodeBytesAsync(string bookingCode)
    {
        return await Task.Run(() =>
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(
                bookingCode, 
                QRCodeGenerator.ECCLevel.Q
            );
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            return qrCode.GetGraphic(20);
        });
    }
}
