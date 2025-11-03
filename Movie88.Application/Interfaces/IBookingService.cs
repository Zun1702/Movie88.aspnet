using Movie88.Application.DTOs.Bookings;
using Movie88.Application.DTOs.Common;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IBookingService
{
    Task<Result<PagedResultDTO<BookingListDTO>>> GetMyBookingsAsync(int userId, int page, int pageSize, string? status);
}
