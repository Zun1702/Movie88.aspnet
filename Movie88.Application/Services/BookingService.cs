using Movie88.Application.DTOs.Bookings;
using Movie88.Application.DTOs.Common;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ICustomerRepository _customerRepository;

    public BookingService(IBookingRepository bookingRepository, ICustomerRepository customerRepository)
    {
        _bookingRepository = bookingRepository;
        _customerRepository = customerRepository;
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
}
