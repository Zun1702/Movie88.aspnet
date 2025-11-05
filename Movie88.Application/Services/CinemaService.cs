using Movie88.Application.DTOs.Cinemas;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

/// <summary>
/// Cinema service implementation
/// </summary>
public class CinemaService : ICinemaService
{
    private readonly ICinemaRepository _cinemaRepository;

    public CinemaService(ICinemaRepository cinemaRepository)
    {
        _cinemaRepository = cinemaRepository;
    }

    /// <summary>
    /// Get all cinemas, optionally filtered by city
    /// </summary>
    public async Task<List<CinemaDTO>> GetCinemasAsync(string? city = null, CancellationToken cancellationToken = default)
    {
        var cinemas = await _cinemaRepository.GetCinemasAsync(city, cancellationToken);

        return cinemas.Select(c => new CinemaDTO
        {
            Cinemaid = c.Cinemaid,
            Name = c.Name,
            Address = c.Address,
            Phone = c.Phone,
            City = c.City,
            Createdat = c.Createdat
        }).ToList();
    }
}
