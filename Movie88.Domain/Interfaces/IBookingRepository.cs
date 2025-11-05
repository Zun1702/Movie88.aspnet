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
    Task<BookingModel> CreateBookingAsync(int customerid, int showtimeid, string? bookingcode, decimal totalamount, List<(int seatid, decimal seatprice)> seats, CancellationToken cancellationToken = default);
    
    // Add combos methods
    Task AddCombosAsync(int bookingId, List<(int comboid, int quantity, decimal price)> combos, decimal newTotalAmount, CancellationToken cancellationToken = default);
    
    // Voucher methods
    Task<BookingModel?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task ApplyVoucherAsync(int bookingId, int voucherId, decimal newTotalAmount, CancellationToken cancellationToken = default);
    
    // Staff booking verification methods
    /// <summary>
    /// Get booking by booking code with full details including Payments, CheckedInByUser
    /// </summary>
    Task<BookingModel?> GetByBookingCodeAsync(string bookingCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get today's bookings with filters and pagination
    /// </summary>
    Task<(IEnumerable<BookingModel> bookings, int totalCount)> GetTodayBookingsAsync(
        int page, 
        int pageSize, 
        int? cinemaId = null, 
        string? status = null, 
        bool? hasPayment = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update check-in status: Status, Checkedintime, Checkedinby
    /// </summary>
    Task<bool> UpdateCheckInStatusAsync(int bookingId, DateTime checkinTime, int staffUserId, CancellationToken cancellationToken = default);
}
