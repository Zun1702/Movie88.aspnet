using Movie88.Application.DTOs.Seats;

namespace Movie88.Application.Interfaces;

public interface IAuditoriumService
{
    Task<AuditoriumSeatsResponseDTO?> GetAuditoriumSeatsAsync(int auditoriumId, int? showtimeId = null, CancellationToken cancellationToken = default);
}
