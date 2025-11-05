namespace Movie88.Application.DTOs.Seats;

/// <summary>
/// Seat information with booking status
/// </summary>
public class SeatDTO
{
    public int Seatid { get; set; }
    public int Auditoriumid { get; set; }
    public string Row { get; set; } = null!;
    public int Number { get; set; }
    public string? Seattype { get; set; }
    public bool IsAvailable { get; set; } // True if not booked for specific showtime
}

/// <summary>
/// Response for GET /api/auditoriums/{id}/seats
/// </summary>
public class AuditoriumSeatsResponseDTO
{
    public int Auditoriumid { get; set; }
    public string? Name { get; set; }
    public int Seatscount { get; set; }
    public List<SeatDTO> Seats { get; set; } = new();
}
