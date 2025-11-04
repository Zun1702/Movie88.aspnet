using Movie88.Application.DTOs.Bookings;
using Movie88.Application.DTOs.Combos;
using Movie88.Application.DTOs.Common;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBookingCodeGenerator _bookingCodeGenerator;
    private readonly IComboRepository _comboRepository;

    public BookingService(
        IBookingRepository bookingRepository, 
        ICustomerRepository customerRepository,
        IBookingCodeGenerator bookingCodeGenerator,
        IComboRepository comboRepository)
    {
        _bookingRepository = bookingRepository;
        _customerRepository = customerRepository;
        _bookingCodeGenerator = bookingCodeGenerator;
        _comboRepository = comboRepository;
    }

    public async Task<Result<PagedResultDTO<BookingListDTO>>> GetMyBookingsAsync(int userId, int page, int pageSize, string? status)
    {
        // Get customer by userId
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null)
        {
            return Result<PagedResultDTO<BookingListDTO>>.NotFound("Customer profile not found");
        }

        // Get bookings
        var bookings = await _bookingRepository.GetByCustomerIdAsync(customer.Customerid, page, pageSize, status);
        var totalItems = await _bookingRepository.GetCountByCustomerIdAsync(customer.Customerid, status);

        // Map to DTOs
        var bookingDTOs = bookings.Select(b => new BookingListDTO
        {
            Bookingid = b.Bookingid,
            Customerid = b.Customerid,
            Showtimeid = b.Showtimeid,
            Movie = b.Showtime?.Movie != null ? new MovieSummaryDTO
            {
                Movieid = b.Showtime.Movie.Movieid,
                Title = b.Showtime.Movie.Title,
                Posterurl = b.Showtime.Movie.Posterurl
            } : null,
            Cinema = b.Showtime?.Auditorium?.Cinema != null ? new CinemaDTO
            {
                Cinemaid = b.Showtime.Auditorium.Cinema.Cinemaid,
                Name = b.Showtime.Auditorium.Cinema.Name,
                Address = b.Showtime.Auditorium.Cinema.Address,
                City = b.Showtime.Auditorium.Cinema.City
            } : null,
            Showtime = b.Showtime != null ? new ShowtimeDTO
            {
                Starttime = b.Showtime.Starttime?.ToString("yyyy-MM-ddTHH:mm:ss"),
                Format = b.Showtime.Format,
                Languagetype = b.Showtime.Languagetype
            } : null,
            Seats = b.BookingSeats?
                .Select(bs => $"{bs.Seat?.Row}{bs.Seat?.Number}")
                .ToList() ?? new List<string>(),
            Combos = b.BookingCombos?
                .Select(bc => new ComboItemDTO
                {
                    Name = bc.Combo?.Name,
                    Quantity = bc.Quantity,
                    Price = bc.Comboprice
                })
                .ToList() ?? new List<ComboItemDTO>(),
            VoucherCode = b.Voucher?.Code,
            Bookingcode = b.Bookingcode,
            Totalamount = b.Totalamount,
            Status = b.Status,
            Bookingtime = b.Bookingtime?.ToString("yyyy-MM-ddTHH:mm:ss")
        }).ToList();

        var pagedResult = new PagedResultDTO<BookingListDTO>
        {
            Items = bookingDTOs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            TotalItems = totalItems,
            HasNextPage = page < (int)Math.Ceiling(totalItems / (double)pageSize),
            HasPreviousPage = page > 1
        };

        return Result<PagedResultDTO<BookingListDTO>>.Success(pagedResult, "Bookings retrieved successfully");
    }

    public async Task<BookingResponseDTO?> CreateBookingAsync(
        int customerid, 
        CreateBookingRequestDTO request, 
        CancellationToken cancellationToken = default)
    {
        // Validate showtime exists and not started
        var showtime = await _bookingRepository.GetShowtimeWithAuditoriumAsync(request.Showtimeid, cancellationToken);
        if (showtime == null)
            throw new InvalidOperationException("Showtime not found");

        if (showtime.Starttime.HasValue && showtime.Starttime.Value <= DateTime.Now)
            throw new InvalidOperationException("Showtime has already started");

        // Validate seats exist in auditorium
        var seats = await _bookingRepository.GetSeatsByIdsAsync(request.Seatids, cancellationToken);
        if (seats.Count != request.Seatids.Count)
            throw new InvalidOperationException("One or more seats not found");

        // Check all seats belong to correct auditorium
        if (seats.Any(s => s.Auditoriumid != showtime.Auditoriumid))
            throw new InvalidOperationException("One or more seats do not belong to the showtime's auditorium");

        // Check seats not already booked
        var bookedSeatIds = await _bookingRepository.GetBookedSeatIdsForShowtimeAsync(request.Showtimeid, cancellationToken);
        var alreadyBookedSeats = request.Seatids.Where(seatId => bookedSeatIds.Contains(seatId)).ToList();
        if (alreadyBookedSeats.Any())
        {
            var bookedSeatNames = seats
                .Where(s => alreadyBookedSeats.Contains(s.Seatid))
                .Select(s => $"{s.Row}{s.Number}")
                .ToList();
            throw new InvalidOperationException($"The following seats are already booked: {string.Join(", ", bookedSeatNames)}");
        }

        // Note: BookingCode will be generated only after payment confirmation
        // Initial booking has null BookingCode

        // Calculate total amount (seat price based on showtime price)
        var seatPrice = showtime.Price ?? 0;
        var totalAmount = seatPrice * request.Seatids.Count;
        var seatsWithPrices = request.Seatids.Select(seatId => (seatId, seatPrice)).ToList();

        // Create booking with null BookingCode (will be generated after payment)
        var booking = await _bookingRepository.CreateBookingAsync(
            customerid, 
            request.Showtimeid, 
            null, // BookingCode is null until payment confirmed
            totalAmount, 
            seatsWithPrices, 
            cancellationToken);

        // Map to response DTO
        return new BookingResponseDTO
        {
            Bookingid = booking.Bookingid,
            Bookingcode = booking.Bookingcode, // Will be null for pending bookings
            Showtimeid = booking.Showtimeid,
            Seats = seats.Select(s => new BookedSeatDTO
            {
                Seatid = s.Seatid,
                Row = s.Row ?? "",
                Number = s.Number ?? 0,
                Seatprice = seatPrice
            }).ToList(),
            Totalamount = totalAmount,
            Status = booking.Status ?? "pending",
            Createdat = booking.Bookingtime ?? DateTime.Now
        };
    }

    public async Task<UpdatedBookingResponseDTO?> AddCombosToBookingAsync(
        int bookingId, 
        int customerId, 
        AddCombosRequestDTO request, 
        CancellationToken cancellationToken = default)
    {
        // Validate booking exists and belongs to customer
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
        if (booking == null)
            throw new InvalidOperationException("Booking not found");

        if (booking.Customerid != customerId)
            throw new UnauthorizedAccessException("This booking does not belong to you");

        if (booking.Status?.ToLower() != "pending")
            throw new InvalidOperationException("Can only add combos to pending bookings");

        // Validate all combos
        if (request.Combos.Any(c => c.Quantity <= 0))
            throw new InvalidOperationException("Combo quantity must be greater than 0");

        var comboIds = request.Combos.Select(c => c.Comboid).ToList();
        var combos = await _comboRepository.GetCombosByIdsAsync(comboIds, cancellationToken);

        if (combos.Count != comboIds.Count)
            throw new InvalidOperationException("One or more combos not found");

        // Calculate new total amount
        var existingTotal = booking.Totalamount ?? 0;
        var comboTotal = request.Combos.Sum(c =>
        {
            var combo = combos.First(co => co.Comboid == c.Comboid);
            return (combo.Price ?? 0) * c.Quantity;
        });
        var newTotal = existingTotal + comboTotal;

        // Add combos to booking
        var comboItems = request.Combos.Select(c =>
        {
            var combo = combos.First(co => co.Comboid == c.Comboid);
            return (c.Comboid, c.Quantity, combo.Price ?? 0);
        }).ToList();

        await _bookingRepository.AddCombosAsync(bookingId, comboItems, newTotal, cancellationToken);

        // Return updated booking response
        return new UpdatedBookingResponseDTO
        {
            Bookingid = bookingId,
            Bookingcode = booking.Bookingcode,
            Showtimeid = booking.Showtimeid,
            Combos = request.Combos.Select(c =>
            {
                var combo = combos.First(co => co.Comboid == c.Comboid);
                return new ComboItemDTO
                {
                    Name = combo.Name,
                    Quantity = c.Quantity,
                    Price = combo.Price ?? 0
                };
            }).ToList(),
            Totalamount = newTotal,
            Status = booking.Status ?? "pending"
        };
    }
}
