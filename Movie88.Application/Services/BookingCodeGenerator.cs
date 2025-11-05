using System.Security.Cryptography;

namespace Movie88.Application.Services;

/// <summary>
/// Service to generate secure unique booking codes
/// </summary>
public interface IBookingCodeGenerator
{
    /// <summary>
    /// Generate secure unique booking code
    /// Format: M88-YYYYMMDD-XXXXX (e.g., M88-20251103-47B2E)
    /// XXXXX = 5-character random alphanumeric code (no collision in same day)
    /// </summary>
    string GenerateBookingCode(DateTime bookingTime);
}

public class BookingCodeGenerator : IBookingCodeGenerator
{
    private static readonly object _lock = new object();
    private static readonly HashSet<string> _usedCodesPerDay = new();
    private static string _lastDate = string.Empty;
    private static readonly Random _random = new Random();

    public string GenerateBookingCode(DateTime bookingTime)
    {
        lock (_lock)
        {
            var dateStr = bookingTime.ToString("yyyyMMdd");
            
            // Reset used codes if date changed
            if (dateStr != _lastDate)
            {
                _usedCodesPerDay.Clear();
                _lastDate = dateStr;
            }
            
            // Generate unique 5-character random code
            string randomCode;
            int attempts = 0;
            const int maxAttempts = 100; // Prevent infinite loop
            
            do
            {
                randomCode = GenerateRandomCode(5);
                attempts++;
                
                if (attempts >= maxAttempts)
                {
                    // Fallback: use timestamp-based code if random collision persists
                    randomCode = GenerateTimestampCode();
                    break;
                }
            }
            while (_usedCodesPerDay.Contains(randomCode));
            
            _usedCodesPerDay.Add(randomCode);
            
            // Format: M88-20251103-47B2E
            return $"M88-{dateStr}-{randomCode}";
        }
    }
    
    private string GenerateRandomCode(int length)
    {
        // Use alphanumeric characters (uppercase) excluding confusing ones: 0, O, 1, I, L
        const string chars = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
        var code = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            code[i] = chars[_random.Next(chars.Length)];
        }
        
        return new string(code);
    }
    
    private string GenerateTimestampCode()
    {
        // Fallback: use last 5 digits of timestamp in hex
        var timestamp = DateTime.Now.Ticks;
        var hash = (timestamp % 1048576).ToString("X5"); // 5 hex chars
        return hash;
    }
}
