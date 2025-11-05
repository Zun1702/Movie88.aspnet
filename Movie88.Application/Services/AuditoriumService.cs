using Movie88.Application.DTOs.Seats;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class AuditoriumService : IAuditoriumService
{
    private readonly IAuditoriumRepository _auditoriumRepository;

    public AuditoriumService(IAuditoriumRepository auditoriumRepository)
    {
        _auditoriumRepository = auditoriumRepository;
    }

    public async Task<AuditoriumSeatsResponseDTO?> GetAuditoriumSeatsAsync(
        int auditoriumId, 
        int? showtimeId = null, 
        CancellationToken cancellationToken = default)
    {
        var auditorium = await _auditoriumRepository.GetByIdAsync(auditoriumId, cancellationToken);
        if (auditorium == null)
            return null;

        var seats = await _auditoriumRepository.GetSeatsAsync(auditoriumId, cancellationToken);

        var bookedSeatIds = new List<int>();
        if (showtimeId.HasValue)
        {
            bookedSeatIds = await _auditoriumRepository.GetBookedSeatIdsAsync(showtimeId.Value, cancellationToken);
        }

        return new AuditoriumSeatsResponseDTO
        {
            Auditoriumid = auditorium.Auditoriumid,
            Name = auditorium.Name ?? "",
            Seatscount = auditorium.Seatscount ?? 0,
            Seats = seats.Select(s => new SeatDTO
            {
                Seatid = s.Seatid,
                Auditoriumid = s.Auditoriumid,
                Row = s.Row ?? "",
                Number = s.Number ?? 0,
                Seattype = s.Type ?? "standard",
                // COMPUTED: Check if seat is booked for THIS specific showtime
                IsAvailableForShowtime = !bookedSeatIds.Contains(s.Seatid)
            }).ToList()
        };
    }
}
