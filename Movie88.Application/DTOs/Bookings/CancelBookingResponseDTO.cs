namespace Movie88.Application.DTOs.Bookings;

public class CancelBookingResponseDTO
{
    public int Bookingid { get; set; }
    public string? Bookingcode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> ReleasedSeats { get; set; } = new();
    public DateTime CancelledAt { get; set; }
}
