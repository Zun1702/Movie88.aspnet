using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<BookingModel>> GetByCustomerIdAsync(int customerId, int page, int pageSize, string? status);
    Task<int> GetCountByCustomerIdAsync(int customerId, string? status);
    Task<BookingModel?> GetByIdWithDetailsAsync(int bookingId);
}
