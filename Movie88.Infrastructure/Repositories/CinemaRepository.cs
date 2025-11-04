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
}
