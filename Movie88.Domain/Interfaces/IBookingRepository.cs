using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<BookingModel>> GetByCustomerIdAsync(int customerId, int page, int pageSize, string? status);
    Task<int> GetCountByCustomerIdAsync(int customerId, string? status);
    Task<BookingModel?> GetByIdWithDetailsAsync(int bookingId);
    
    // Booking creation methods
    Task<ShowtimeModel?> GetShowtimeWithAuditoriumAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task<List<SeatModel>> GetSeatsByIdsAsync(List<int> seatIds, CancellationToken cancellationToken = default);
    Task<List<int>> GetBookedSeatIdsForShowtimeAsync(int showtimeId, CancellationToken cancellationToken = default);
    Task<BookingModel> CreateBookingAsync(int customerid, int showtimeid, string bookingcode, decimal totalamount, List<(int seatid, decimal seatprice)> seats, CancellationToken cancellationToken = default);
}
