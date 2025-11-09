using Movie88.Application.DTOs.Cinemas;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;

namespace Movie88.Application.Services;

public class AdminCinemaService : IAdminCinemaService
{
    private readonly ICinemaRepository _cinemaRepository;
    private readonly Movie88.Application.Interfaces.IUnitOfWork _unitOfWork;

    public AdminCinemaService(
        ICinemaRepository cinemaRepository,
        Movie88.Application.Interfaces.IUnitOfWork unitOfWork)
    {
        _cinemaRepository = cinemaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CinemaResponseDto>> CreateCinemaAsync(CreateCinemaDto request)
    {
        try
        {
            var cinemaModel = new CinemaModel
            {
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                Phone = request.Phone,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            var createdCinema = await _cinemaRepository.AddAsync(cinemaModel);
            await _unitOfWork.CommitAsync();

            var response = new CinemaResponseDto
            {
                CinemaId = createdCinema.Cinemaid,
                Name = createdCinema.Name,
                Address = createdCinema.Address,
                City = createdCinema.City,
                Phone = createdCinema.Phone,
                Latitude = createdCinema.Latitude,
                Longitude = createdCinema.Longitude,
                NumberOfAuditoriums = 0,
                CreatedAt = createdCinema.Createdat
            };

            return Result<CinemaResponseDto>.Success(response, "Cinema created successfully");
        }
        catch (Exception ex)
        {
            return Result<CinemaResponseDto>.Error($"Error creating cinema: {ex.Message}", 500);
        }
    }

    public async Task<Result<CinemaResponseDto>> UpdateCinemaAsync(int cinemaId, UpdateCinemaDto request)
    {
        try
        {
            var cinema = await _cinemaRepository.GetByIdAsync(cinemaId);
            if (cinema == null)
            {
                return Result<CinemaResponseDto>.Error("Cinema not found", 404);
            }

            // Update only provided fields (partial update)
            if (!string.IsNullOrWhiteSpace(request.Name))
                cinema.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Address))
                cinema.Address = request.Address;

            if (request.City != null)
                cinema.City = request.City;

            if (request.Phone != null)
                cinema.Phone = request.Phone;

            if (request.Latitude.HasValue)
                cinema.Latitude = request.Latitude;

            if (request.Longitude.HasValue)
                cinema.Longitude = request.Longitude;

            var updatedCinema = await _cinemaRepository.UpdateAsync(cinema);
            await _unitOfWork.CommitAsync();

            var auditoriumCount = await _cinemaRepository.GetAuditoriumCountAsync(cinemaId);

            var response = new CinemaResponseDto
            {
                CinemaId = updatedCinema.Cinemaid,
                Name = updatedCinema.Name,
                Address = updatedCinema.Address,
                City = updatedCinema.City,
                Phone = updatedCinema.Phone,
                Latitude = updatedCinema.Latitude,
                Longitude = updatedCinema.Longitude,
                NumberOfAuditoriums = auditoriumCount,
                CreatedAt = updatedCinema.Createdat
            };

            return Result<CinemaResponseDto>.Success(response, "Cinema updated successfully");
        }
        catch (Exception ex)
        {
            return Result<CinemaResponseDto>.Error($"Error updating cinema: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> DeleteCinemaAsync(int cinemaId)
    {
        try
        {
            var cinema = await _cinemaRepository.GetByIdAsync(cinemaId);
            if (cinema == null)
            {
                return Result<bool>.Error("Cinema not found", 404);
            }

            // Check if cinema has active showtimes
            var hasActiveShowtimes = await _cinemaRepository.HasActiveShowtimesAsync(cinemaId);
            if (hasActiveShowtimes)
            {
                return Result<bool>.Error("Cannot delete cinema with active showtimes", 400);
            }

            var deleted = await _cinemaRepository.DeleteAsync(cinemaId);
            if (!deleted)
            {
                return Result<bool>.Error("Failed to delete cinema", 500);
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, "Cinema deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<bool>.Error($"Error deleting cinema: {ex.Message}", 500);
        }
    }

    public async Task<Result<CinemaResponseDto>> GetCinemaByIdAsync(int cinemaId)
    {
        try
        {
            var cinema = await _cinemaRepository.GetByIdAsync(cinemaId);
            if (cinema == null)
            {
                return Result<CinemaResponseDto>.Error("Cinema not found", 404);
            }

            var auditoriumCount = await _cinemaRepository.GetAuditoriumCountAsync(cinemaId);

            var response = new CinemaResponseDto
            {
                CinemaId = cinema.Cinemaid,
                Name = cinema.Name,
                Address = cinema.Address,
                City = cinema.City,
                Phone = cinema.Phone,
                Latitude = cinema.Latitude,
                Longitude = cinema.Longitude,
                NumberOfAuditoriums = auditoriumCount,
                CreatedAt = cinema.Createdat
            };

            return Result<CinemaResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CinemaResponseDto>.Error($"Error retrieving cinema: {ex.Message}", 500);
        }
    }
}
