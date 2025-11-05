namespace Movie88.Application.Interfaces;

/// <summary>
/// Service for generating QR codes from booking codes
/// </summary>
public interface IQRCodeService
{
    /// <summary>
    /// Generate QR code as Base64 string for email embedding
    /// </summary>
    /// <param name="bookingCode">The booking code to encode (e.g., "BK-20251105-0156")</param>
    /// <returns>Base64 encoded PNG image</returns>
    Task<string> GenerateQRCodeBase64Async(string bookingCode);
    
    /// <summary>
    /// Generate QR code as byte array for API response
    /// </summary>
    /// <param name="bookingCode">The booking code to encode</param>
    /// <returns>PNG image as byte array</returns>
    Task<byte[]> GenerateQRCodeBytesAsync(string bookingCode);
}
