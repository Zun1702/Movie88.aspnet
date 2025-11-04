using Movie88.Application.DTOs.Bookings;
using Movie88.Application.DTOs.Combos;
using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Vouchers;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IBookingService
{
    Task<Result<PagedResultDTO<BookingListDTO>>> GetMyBookingsAsync(int userId, int page, int pageSize, string? status);
    Task<BookingResponseDTO?> CreateBookingAsync(int customerid, CreateBookingRequestDTO request, CancellationToken cancellationToken = default);
    Task<UpdatedBookingResponseDTO?> AddCombosToBookingAsync(int bookingId, int customerId, AddCombosRequestDTO request, CancellationToken cancellationToken = default);
    Task<ApplyVoucherResponseDTO?> ApplyVoucherToBookingAsync(int bookingId, int customerId, ApplyVoucherRequestDTO request, CancellationToken cancellationToken = default);
}
