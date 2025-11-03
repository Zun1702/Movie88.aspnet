namespace Movie88.Application.Services;

/// <summary>
/// Service to generate unique booking codes
/// </summary>
public interface IBookingCodeGenerator
{
    /// <summary>
    /// Generate unique booking code
    /// Format: BK-YYYYMMDD-XXXX (e.g., BK-20251103-0001)
    /// </summary>
    string GenerateBookingCode(DateTime bookingTime);
}

public class BookingCodeGenerator : IBookingCodeGenerator
{
    private static readonly object _lock = new object();
    private static int _counter = 0;
    private static string _lastDate = string.Empty;

    public string GenerateBookingCode(DateTime bookingTime)
    {
        lock (_lock)
        {
            var dateStr = bookingTime.ToString("yyyyMMdd");
            
            // Reset counter if date changed
            if (dateStr != _lastDate)
            {
                _counter = 0;
                _lastDate = dateStr;
            }
            
            _counter++;
            
            // Format: BK-20251103-0001
            return $"BK-{dateStr}-{_counter:D4}";
        }
    }
}
