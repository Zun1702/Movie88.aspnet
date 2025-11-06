using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

/// <summary>
/// Cinema repository implementation
/// </summary>
public class CinemaRepository : ICinemaRepository
{
    private readonly AppDbContext _context;

    public CinemaRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all cinemas, optionally filtered by city
    /// </summary>
    public async Task<List<CinemaModel>> GetCinemasAsync(string? city = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Cinemas.AsQueryable();

        // Filter by city if provided
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(c => c.City != null && c.City.ToLower() == city.ToLower());
        }

        // Sort by name
        var cinemas = await query
            .OrderBy(c => c.Name)
            .Select(c => new CinemaModel
            {
                Cinemaid = c.Cinemaid,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                City = c.City,
                Createdat = c.Createdat
            })
            .ToListAsync(cancellationToken);

        return cinemas;
    }

    public async Task<CinemaModel?> GetByIdAsync(int id)
    {
        var cinema = await _context.Cinemas.FindAsync(id);
        if (cinema == null) return null;

        return new CinemaModel
        {
            Cinemaid = cinema.Cinemaid,
            Name = cinema.Name,
            Address = cinema.Address,
            Phone = cinema.Phone,
            City = cinema.City,
            Createdat = cinema.Createdat
        };
    }

    public async Task<CinemaModel> AddAsync(CinemaModel model)
    {
        var cinema = new Entities.Cinema
        {
            Name = model.Name,
            Address = model.Address,
            Phone = model.Phone,
            City = model.City,
            Createdat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.Cinemas.Add(cinema);
        await _context.SaveChangesAsync();

        model.Cinemaid = cinema.Cinemaid;
        model.Createdat = cinema.Createdat;
        return model;
    }

    public async Task<CinemaModel> UpdateAsync(CinemaModel model)
    {
        var cinema = await _context.Cinemas.FindAsync(model.Cinemaid);
        if (cinema == null)
            throw new InvalidOperationException("Cinema not found");

        cinema.Name = model.Name;
        cinema.Address = model.Address;
        cinema.Phone = model.Phone;
        cinema.City = model.City;

        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cinema = await _context.Cinemas.FindAsync(id);
        if (cinema == null) return false;

        _context.Cinemas.Remove(cinema);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasActiveShowtimesAsync(int cinemaId)
    {
        var currentTime = DateTime.UtcNow;
        
        return await _context.Auditoriums
            .Where(a => a.Cinemaid == cinemaId)
            .AnyAsync(a => _context.Showtimes
                .Any(s => s.Auditoriumid == a.Auditoriumid && s.Starttime > currentTime));
    }

    public async Task<int> GetAuditoriumCountAsync(int cinemaId)
    {
        return await _context.Auditoriums
            .CountAsync(a => a.Cinemaid == cinemaId);
    }
}
