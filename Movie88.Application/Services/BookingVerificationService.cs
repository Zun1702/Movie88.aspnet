using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Staff;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Enums;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

/// <summary>
/// Service for staff booking verification and check-in operations
/// Handles online booking verification flow for staff at cinema counter
/// </summary>
public class BookingVerificationService : IBookingVerificationService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingVerificationService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Result<BookingVerifyDTO>> VerifyBookingCodeAsync(
        string bookingCode, 
        CancellationToken cancellationToken = default)
    {
        // Get booking with all details including Payments and CheckedInByUser
        var booking = await _bookingRepository.GetByBookingCodeAsync(bookingCode, cancellationToken);

        if (booking == null)
        {
            return Result<BookingVerifyDTO>.NotFound($"Booking code '{bookingCode}' not found");
        }

        // Check if booking is cancelled
        if (booking.Status?.ToLower() == nameof(BookingStatus.Cancelled).ToLower())
        {
            return Result<BookingVerifyDTO>.BadRequest("This booking has been cancelled");
        }

        // IMPORTANT: Check payment status via Payments collection (NOT Booking.paymentStatus)
        // Accept both "Success" and "Completed" as valid payment status
        var hasCompletedPayment = booking.Payments?.Any(p => 
            p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true || 
            p.Status?.Equals("Success", StringComparison.OrdinalIgnoreCase) == true) ?? false;
        var completedPayment = booking.Payments?.FirstOrDefault(p => 
            p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true || 
            p.Status?.Equals("Success", StringComparison.OrdinalIgnoreCase) == true);

        // Calculate if can check-in
        var canCheckIn = hasCompletedPayment && booking.Checkedintime == null;

        // Map to DTO
        var dto = new BookingVerifyDTO
        {
            BookingId = booking.Bookingid,
            BookingCode = booking.Bookingcode,
            Status = booking.Status,
            BookingDate = booking.Bookingtime,
            Customer = new CustomerInfoDTO
            {
                CustomerId = booking.Customer?.Customerid ?? 0,
                Fullname = booking.Customer?.Fullname ?? string.Empty,
                Email = booking.Customer?.Email ?? string.Empty,
                Phone = booking.Customer?.Phone
            },
            Movie = new MovieInfoDTO
            {
                MovieId = booking.Showtime?.Movie?.Movieid ?? 0,
                Title = booking.Showtime?.Movie?.Title ?? string.Empty,
                PosterUrl = booking.Showtime?.Movie?.Posterurl,
                DurationMinutes = booking.Showtime?.Movie?.Durationminutes,
                Rating = booking.Showtime?.Movie?.Rating
            },
            Showtime = new ShowtimeInfoDTO
            {
                ShowtimeId = booking.Showtime?.Showtimeid ?? 0,
                StartTime = booking.Showtime?.Starttime,
                EndTime = booking.Showtime?.Endtime,
                Cinema = new CinemaInfoDTO
                {
                    CinemaId = booking.Showtime?.Auditorium?.Cinema?.Cinemaid ?? 0,
                    Name = booking.Showtime?.Auditorium?.Cinema?.Name ?? string.Empty,
                    Address = booking.Showtime?.Auditorium?.Cinema?.Address
                },
                Auditorium = new AuditoriumInfoDTO
                {
                    AuditoriumId = booking.Showtime?.Auditorium?.Auditoriumid ?? 0,
                    Name = booking.Showtime?.Auditorium?.Name ?? string.Empty,
                    TotalSeats = booking.Showtime?.Auditorium?.Totalseats
                },
                Format = booking.Showtime?.Format,
                Language = booking.Showtime?.Languagetype
            },
            Seats = booking.BookingSeats?.Select(bs => new SeatInfoDTO
            {
                SeatId = bs.Seat?.Seatid ?? 0,
                Row = bs.Seat?.Row,
                Number = bs.Seat?.Number,
                Type = bs.Seat?.Type
            }).ToList() ?? new List<SeatInfoDTO>(),
            Pricing = new PricingInfoDTO
            {
                TicketPrice = booking.Showtime?.Price ?? 0,
                NumberOfTickets = booking.BookingSeats?.Count ?? 0,
                Subtotal = booking.Totalamount ?? 0,
                Discount = 0, // Calculate from voucher if needed
                TotalAmount = booking.Totalamount ?? 0
            },
            Payment = new PaymentInfoDTO
            {
                Status = completedPayment?.Status ?? (booking.Payments?.Any() == true ? booking.Payments.First().Status ?? "Pending" : "Pending"),
                PaymentMethod = completedPayment?.Method?.Methodname,
                TransactionCode = completedPayment?.Transactioncode,
                PaidAt = completedPayment?.Paymenttime
            },
            CheckIn = new CheckInInfoDTO
            {
                IsCheckedIn = booking.Status?.ToLower() == nameof(BookingStatus.CheckedIn).ToLower(),
                CheckedInTime = booking.Checkedintime,
                CheckedInBy = booking.Checkedinby,
                CheckedInByStaffName = booking.CheckedInByUser?.Fullname
            },
            BookingStatus = booking.Status ?? string.Empty,
            CanCheckIn = canCheckIn
        };

        return Result<BookingVerifyDTO>.Success(dto, "Booking verified successfully");
    }

    public async Task<Result<CheckInResponseDTO>> CheckInAsync(
        int bookingId, 
        CheckInCommand command, 
        int staffUserId, 
        CancellationToken cancellationToken = default)
    {
        // First, get basic booking info to get booking code
        var bookingBasic = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        
        if (bookingBasic == null)
        {
            return Result<CheckInResponseDTO>.NotFound($"Booking with ID {bookingId} not found");
        }

        // Get booking with FULL details including Payments collection
        // Use GetByBookingCodeAsync which includes Payments, CheckedInByUser, etc.
        var booking = await _bookingRepository.GetByBookingCodeAsync(bookingBasic.Bookingcode!, cancellationToken);

        if (booking == null)
        {
            return Result<CheckInResponseDTO>.NotFound($"Booking with ID {bookingId} not found");
        }

        // Validate: Only "Confirmed" status can check-in
        // - Pending: not paid yet → cannot check-in
        // - Confirmed: paid, not checked-in yet → CAN check-in
        // - Completed: already checked-in → cannot check-in again
        var bookingStatus = booking.Status?.ToLower();
        
        if (bookingStatus == nameof(BookingStatus.Pending).ToLower())
        {
            return Result<CheckInResponseDTO>.BadRequest(
                "Cannot check-in. Booking is still Pending. Please complete payment first.");
        }
        
        if (bookingStatus == nameof(BookingStatus.Completed).ToLower())
        {
            return Result<CheckInResponseDTO>.BadRequest(
                "Cannot check-in. Booking is already Completed (checked-in previously).");
        }
        
        if (bookingStatus != nameof(BookingStatus.Confirmed).ToLower())
        {
            return Result<CheckInResponseDTO>.BadRequest(
                $"Booking must be in 'Confirmed' status to check-in. Current status: {booking.Status}");
        }

        // IMPORTANT: Validate payment completed via Payments collection
        // Accept both "Success" and "Completed" as valid payment status
        var hasCompletedPayment = booking.Payments?.Any(p => 
            p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true || 
            p.Status?.Equals("Success", StringComparison.OrdinalIgnoreCase) == true) ?? false;
        
        if (!hasCompletedPayment)
        {
            return Result<CheckInResponseDTO>.BadRequest(
                "Payment not completed. No completed payment found in Payments collection.");
        }

        // Validate: Not already checked in (double check via Checkedintime)
        if (booking.Checkedintime != null)
        {
            return Result<CheckInResponseDTO>.BadRequest(
                $"This booking was already checked in at {booking.Checkedintime:yyyy-MM-dd HH:mm:ss}");
        }

        // Update check-in status
        var updated = await _bookingRepository.UpdateCheckInStatusAsync(
            bookingId, 
            command.CheckinTime, 
            staffUserId, 
            cancellationToken);

        if (!updated)
        {
            return Result<CheckInResponseDTO>.InternalServerError("Failed to update check-in status");
        }

        // Get updated booking to retrieve staff name
        var updatedBooking = await _bookingRepository.GetByBookingCodeAsync(booking.Bookingcode!, cancellationToken);

        var response = new CheckInResponseDTO
        {
            BookingId = bookingId,
            BookingCode = booking.Bookingcode,
            Status = nameof(BookingStatus.CheckedIn),
            CheckedInAt = command.CheckinTime,
            CheckedInBy = new StaffInfoDTO
            {
                StaffId = staffUserId,
                StaffName = updatedBooking?.CheckedInByUser?.Fullname ?? "Staff"
            }
        };

        return Result<CheckInResponseDTO>.Success(response, "Check-in successful");
    }

    public async Task<Result<PagedResultDTO<TodayBookingDTO>>> GetTodayBookingsAsync(
        TodayBookingsQuery query, 
        CancellationToken cancellationToken = default)
    {
        // Get today's bookings with filters
        var (bookings, totalCount) = await _bookingRepository.GetTodayBookingsAsync(
            query.Page,
            query.PageSize,
            query.CinemaId,
            query.Status,
            query.HasPayment,
            cancellationToken);

        // Map to DTOs
        var items = bookings.Select(b =>
        {
            var hasCompletedPayment = b.Payments?.Any(p => 
                p.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true || 
                p.Status?.Equals("Success", StringComparison.OrdinalIgnoreCase) == true) ?? false;
            
            // Can check-in only if: Status = "Confirmed" AND has completed payment AND not checked-in yet
            var canCheckIn = b.Status?.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) == true 
                && hasCompletedPayment 
                && b.Checkedintime == null;

            return new TodayBookingDTO
            {
                BookingId = b.Bookingid,
                BookingCode = b.Bookingcode,
                CustomerName = b.Customer?.Fullname ?? string.Empty,
                MovieTitle = b.Showtime?.Movie?.Title ?? string.Empty,
                ShowtimeStart = b.Showtime?.Starttime,
                CinemaName = b.Showtime?.Auditorium?.Cinema?.Name ?? string.Empty,
                AuditoriumName = b.Showtime?.Auditorium?.Name ?? string.Empty,
                Status = b.Status ?? string.Empty,
                PaymentStatus = hasCompletedPayment ? "Completed" : (b.Payments?.Any() == true ? b.Payments.First().Status ?? "Pending" : "Pending"),
                CheckedInTime = b.Checkedintime,
                CheckedInByStaffName = b.CheckedInByUser?.Fullname,
                CanCheckIn = canCheckIn
            };
        }).ToList();

        var pagedResult = new PagedResultDTO<TodayBookingDTO>
        {
            Items = items,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
            TotalItems = totalCount,
            HasNextPage = query.Page < (int)Math.Ceiling(totalCount / (double)query.PageSize),
            HasPreviousPage = query.Page > 1
        };

        return Result<PagedResultDTO<TodayBookingDTO>>.Success(
            pagedResult, 
            $"Retrieved {items.Count} bookings for today");
    }
}
