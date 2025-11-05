using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Domain.Enums;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

public class AuditoriumRepository : IAuditoriumRepository
{
    private readonly AppDbContext _context;

    public AuditoriumRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuditoriumModel?> GetByIdAsync(int auditoriumId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auditoriums
            .Include(a => a.Cinema)
            .FirstOrDefaultAsync(a => a.Auditoriumid == auditoriumId, cancellationToken);

        if (entity == null)
            return null;

        return new AuditoriumModel
        {
            Auditoriumid = entity.Auditoriumid,
            Cinemaid = entity.Cinemaid,
            Name = entity.Name,
            Seatscount = entity.Seatscount,
            Cinema = entity.Cinema != null ? new CinemaModel
            {
                Cinemaid = entity.Cinema.Cinemaid,
                Name = entity.Cinema.Name,
                Address = entity.Cinema.Address,
                Phone = entity.Cinema.Phone,
                City = entity.Cinema.City,
                Createdat = entity.Cinema.Createdat
            } : null
        };
    }

    public async Task<List<SeatModel>> GetSeatsAsync(int auditoriumId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Seats
            .Where(s => s.Auditoriumid == auditoriumId)
            .OrderBy(s => s.Row)
            .ThenBy(s => s.Number)
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

    public async Task<List<int>> GetBookedSeatIdsAsync(int showtimeId, CancellationToken cancellationToken = default)
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
}
