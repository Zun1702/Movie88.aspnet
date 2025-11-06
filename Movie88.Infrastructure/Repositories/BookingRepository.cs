using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Domain.Enums;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    protected readonly AppDbContext _context;
    protected readonly IMapper _mapper;

    public BookingRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingModel>> GetByCustomerIdAsync(int customerId, int page, int pageSize, string? status)
    {
        var query = _context.Bookings
            .Include(b => b.Showtime)
                .ThenInclude(s => s!.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s!.Auditorium)
                    .ThenInclude(a => a!.Cinema)
            .Include(b => b.Bookingseats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.Bookingcombos)
                .ThenInclude(bc => bc.Combo)
            .Include(b => b.Voucher)
            .Where(b => b.Customerid == customerId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(b => b.Status != null && b.Status.ToLower() == status.ToLower());
        }

        var bookings = await query
            .OrderByDescending(b => b.Bookingtime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<BookingModel>>(bookings);
    }

    public async Task<int> GetCountByCustomerIdAsync(int customerId, string? status)
    {
        var query = _context.Bookings
            .Where(b => b.Customerid == customerId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(b => b.Status != null && b.Status.ToLower() == status.ToLower());
        }

        return await query.CountAsync();
    }

    public async Task<BookingModel?> GetByIdWithDetailsAsync(int bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
                .ThenInclude(c => c!.User) // ✅ Include User to get email
            .Include(b => b.Showtime)
                .ThenInclude(s => s!.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s!.Auditorium)
                    .ThenInclude(a => a!.Cinema)
            .Include(b => b.Bookingseats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.Bookingcombos)
                .ThenInclude(bc => bc.Combo)
            .Include(b => b.Voucher)
            .FirstOrDefaultAsync(b => b.Bookingid == bookingId);

        return booking == null ? null : _mapper.Map<BookingModel>(booking);
    }

    public async Task<ShowtimeModel?> GetShowtimeWithAuditoriumAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Showtimes
            .Include(s => s.Auditorium)
            .FirstOrDefaultAsync(s => s.Showtimeid == showtimeId, cancellationToken);

        if (entity == null)
            return null;

        return new ShowtimeModel
        {
            Showtimeid = entity.Showtimeid,
            Movieid = entity.Movieid,
            Auditoriumid = entity.Auditoriumid,
            Starttime = entity.Starttime,
            Price = entity.Price
        };
    }

    public async Task<List<SeatModel>> GetSeatsByIdsAsync(List<int> seatIds, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Seats
            .Where(s => seatIds.Contains(s.Seatid))
            .ToListAsync(cancellationToken);

        return entities.Select(e => new SeatModel
        {
            Seatid = e.Seatid,
            Auditoriumid = e.Auditoriumid,
            Row = e.Row,
            Number = e.Number,
            Type = e.Type
        }).ToList();
    }

    public async Task<List<int>> GetBookedSeatIdsForShowtimeAsync(int showtimeId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookingseats
            .Include(bs => bs.Booking)
            .Where(bs => bs.Showtimeid == showtimeId 
                && bs.Booking != null 
                && bs.Booking.Status != null
                && bs.Booking.Status.ToLower() != nameof(BookingStatus.Cancelled).ToLower())
            .Select(bs => bs.Seatid)
            .ToListAsync(cancellationToken);
    }

    public async Task<BookingModel> CreateBookingAsync(
        int customerid, 
        int showtimeid, 
        string? bookingcode, 
        decimal totalamount, 
        List<(int seatid, decimal seatprice)> seats, 
        CancellationToken cancellationToken = default)
    {
        // Use execution strategy for retry-compatible transactions
        var strategy = _context.Database.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Create booking entity
                var bookingEntity = new Booking
                {
                    Customerid = customerid,
                    Showtimeid = showtimeid,
                    Bookingcode = bookingcode,
                    Bookingtime = DateTime.Now,
                    Totalamount = totalamount,
                    Status = nameof(BookingStatus.Pending)
                };

                _context.Bookings.Add(bookingEntity);
                await _context.SaveChangesAsync(cancellationToken);

                // Create bookingseat entities
                foreach (var (seatid, seatprice) in seats)
                {
                    var bookingSeat = new Bookingseat
                    {
                        Bookingid = bookingEntity.Bookingid,
                        Seatid = seatid,
                        Showtimeid = showtimeid,
                        Seatprice = seatprice
                    };
                    _context.Bookingseats.Add(bookingSeat);
                    
                    // ✅ KHÔNG CẦN update seat.Isavailable
                    // Lý do:
                    // - seat.Isavailable CHỈ dùng để đánh dấu ghế HƯ/BẢO TRÌ (vĩnh viễn)
                    // - Trạng thái "ghế đã đặt" được quản lý qua bảng bookingseats + showtimeid
                    // - API sẽ tính toán isAvailableForShowtime = !bookedSeatIds.Contains(seatid)
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // Return booking model
                return new BookingModel
                {
                    Bookingid = bookingEntity.Bookingid,
                    Customerid = bookingEntity.Customerid,
                    Showtimeid = bookingEntity.Showtimeid,
                    Bookingcode = bookingEntity.Bookingcode,
                    Bookingtime = bookingEntity.Bookingtime,
                    Totalamount = bookingEntity.Totalamount,
                    Status = bookingEntity.Status
                };
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task AddCombosAsync(
        int bookingId, 
        List<(int comboid, int quantity, decimal price)> combos, 
        decimal newTotalAmount, 
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Add bookingcombo records
                foreach (var (comboid, quantity, price) in combos)
                {
                    var bookingCombo = new Bookingcombo
                    {
                        Bookingid = bookingId,
                        Comboid = comboid,
                        Quantity = quantity,
                        Comboprice = price
                    };
                    _context.Bookingcombos.Add(bookingCombo);
                }

                // Update booking total amount
                var booking = await _context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken);
                if (booking != null)
                {
                    booking.Totalamount = newTotalAmount;
                    _context.Bookings.Update(booking);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<BookingModel?> GetByIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            .Include(b => b.Showtime)
            .FirstOrDefaultAsync(b => b.Bookingid == bookingId, cancellationToken);

        return booking != null ? _mapper.Map<BookingModel>(booking) : null;
    }

    public async Task ApplyVoucherAsync(int bookingId, int voucherId, decimal newTotalAmount, CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var booking = await _context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken);
                if (booking == null)
                {
                    throw new InvalidOperationException($"Booking with ID {bookingId} not found");
                }

                booking.Voucherid = voucherId;
                booking.Totalamount = newTotalAmount;
                _context.Bookings.Update(booking);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task UpdateBookingTotalAmountAsync(int bookingId, decimal newTotalAmount, CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { bookingId }, cancellationToken);
        if (booking == null)
        {
            throw new InvalidOperationException($"Booking with ID {bookingId} not found");
        }

        booking.Totalamount = newTotalAmount;
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<BookingModel?> GetByBookingCodeWithDetailsAsync(
        string bookingCode, 
        CancellationToken cancellationToken = default)
    {
        var booking = await _context.Bookings
            // Customer & User
            .Include(b => b.Customer)
                .ThenInclude(c => c!.User)
            
            // Showtime details
            .Include(b => b.Showtime)
                .ThenInclude(s => s!.Movie)
            .Include(b => b.Showtime)
                .ThenInclude(s => s!.Auditorium)
                    .ThenInclude(a => a!.Cinema)
            
            // Seats
            .Include(b => b.Bookingseats)
                .ThenInclude(bs => bs.Seat)
            
            // Combos
            .Include(b => b.Bookingcombos)
                .ThenInclude(bc => bc.Combo)
            
            // Voucher
            .Include(b => b.Voucher)
            
            // Payments
            .Include(b => b.Payments)
            
            // Staff who checked in (if any)
            .Include(b => b.CheckedInByUser)
            
            .FirstOrDefaultAsync(
                b => b.Bookingcode != null && b.Bookingcode == bookingCode, 
                cancellationToken
            );

        return booking == null ? null : _mapper.Map<BookingModel>(booking);
    }

    public async Task<(BookingModel booking, List<int> seatIds)> CancelBookingAndReleaseSeatsAsync(
        int bookingId, 
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get booking with seats and combos
                var booking = await _context.Bookings
                    .Include(b => b.Bookingseats)
                    .ThenInclude(bs => bs.Seat)
                    .Include(b => b.Bookingcombos)
                    .FirstOrDefaultAsync(b => b.Bookingid == bookingId, cancellationToken);

                if (booking == null)
                    throw new InvalidOperationException("Booking not found");

                // Update booking status to Cancelled
                booking.Status = nameof(BookingStatus.Cancelled);
                _context.Bookings.Update(booking);

                // Collect seat IDs for response
                var seatIds = new List<int>();
                foreach (var bookingSeat in booking.Bookingseats)
                {
                    seatIds.Add(bookingSeat.Seatid);
                }

                // ✅ KHÔNG CẦN update seat.Isavailable = true
                // Lý do:
                // - seat.Isavailable CHỈ dùng để đánh dấu ghế HƯ/BẢO TRÌ (vĩnh viễn)
                // - Khi cancel booking, chỉ cần XÓA records trong bookingseats
                // - API tự động tính lại isAvailableForShowtime khi query

                // 🔥 CRITICAL: Hard delete bookingseat and bookingcombo records
                // This allows the same seats to be rebooked without constraint violations
                // The booking record is preserved with Status='Cancelled' for audit purposes
                _context.Bookingseats.RemoveRange(booking.Bookingseats);
                _context.Bookingcombos.RemoveRange(booking.Bookingcombos);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var bookingModel = _mapper.Map<BookingModel>(booking);
                return (bookingModel, seatIds);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<List<BookingModel>> GetPendingBookingsOlderThanAsync(
        int minutes, 
        CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTime.Now.AddMinutes(-minutes);
        
        var bookings = await _context.Bookings
            .Include(b => b.Bookingseats)
            .ThenInclude(bs => bs.Seat)
            .Where(b => b.Status == nameof(BookingStatus.Pending) 
                     && b.Bookingtime.HasValue 
                     && b.Bookingtime.Value <= cutoffTime)
            .ToListAsync(cancellationToken);

        return bookings.Select(b => _mapper.Map<BookingModel>(b)).ToList();
    }
}
