namespace Movie88.Domain.Models;

public class SeatModel
{
    public int Seatid { get; set; }
    public int Auditoriumid { get; set; }
    public string? Row { get; set; }
    public int? Number { get; set; }
    public string? Type { get; set; }
}
