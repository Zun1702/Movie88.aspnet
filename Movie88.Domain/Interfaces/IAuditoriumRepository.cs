using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IAuditoriumRepository
{
    Task<AuditoriumModel?> GetByIdAsync(int auditoriumId, CancellationToken cancellationToken = default);
    Task<List<SeatModel>> GetSeatsAsync(int auditoriumId, CancellationToken cancellationToken = default);
    Task<List<int>> GetBookedSeatIdsAsync(int showtimeId, CancellationToken cancellationToken = default);
}
