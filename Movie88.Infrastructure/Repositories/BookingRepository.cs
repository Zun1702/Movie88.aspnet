using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
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
}
