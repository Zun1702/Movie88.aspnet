using AutoMapper;
using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Movies;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;

namespace Movie88.Application.Services;

public class AdminMovieService : IAdminMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly Movie88.Application.Interfaces.IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdminMovieService(
        IMovieRepository movieRepository,
        Movie88.Application.Interfaces.IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _movieRepository = movieRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MovieResponseDto>> CreateMovieAsync(CreateMovieDto request)
    {
        try
        {
            // Map DTO to Model
            var movieModel = new MovieModel
            {
                Title = request.Title,
                Durationminutes = request.DurationMinutes,
                Rating = request.Rating,
                Description = request.Description,
                Director = request.Director,
                Releasedate = string.IsNullOrWhiteSpace(request.ReleaseDate) 
                    ? null 
                    : DateOnly.Parse(request.ReleaseDate),
                Country = request.Country,
                Genre = request.Genre,
                Posterurl = request.PosterUrl,
                Trailerurl = request.TrailerUrl,
                Createdat = DateTime.UtcNow
            };

            var createdMovie = await _movieRepository.AddAsync(movieModel);
            await _unitOfWork.CommitAsync();

            var response = _mapper.Map<MovieResponseDto>(createdMovie);
            return Result<MovieResponseDto>.Success(response, "Movie created successfully");
        }
        catch (Exception ex)
        {
            return Result<MovieResponseDto>.Error($"Error creating movie: {ex.Message}", 500);
        }
    }

    public async Task<Result<MovieResponseDto>> UpdateMovieAsync(int movieId, UpdateMovieDto request)
    {
        try
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                return Result<MovieResponseDto>.Error("Movie not found", 404);
            }

            // Update only provided fields (partial update)
            if (!string.IsNullOrWhiteSpace(request.Title))
                movie.Title = request.Title;

            if (request.DurationMinutes.HasValue)
                movie.Durationminutes = request.DurationMinutes.Value;

            if (!string.IsNullOrWhiteSpace(request.Rating))
                movie.Rating = request.Rating;

            if (request.Description != null) // Allow empty string to clear
                movie.Description = request.Description;

            if (request.Director != null)
                movie.Director = request.Director;

            if (!string.IsNullOrWhiteSpace(request.ReleaseDate))
                movie.Releasedate = DateOnly.Parse(request.ReleaseDate);

            if (request.Country != null)
                movie.Country = request.Country;

            if (request.Genre != null)
                movie.Genre = request.Genre;

            if (request.PosterUrl != null)
                movie.Posterurl = request.PosterUrl;

            if (request.TrailerUrl != null)
                movie.Trailerurl = request.TrailerUrl;

            var updatedMovie = await _movieRepository.UpdateAsync(movie);
            await _unitOfWork.CommitAsync();

            var response = _mapper.Map<MovieResponseDto>(updatedMovie);
            return Result<MovieResponseDto>.Success(response, "Movie updated successfully");
        }
        catch (Exception ex)
        {
            return Result<MovieResponseDto>.Error($"Error updating movie: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> DeleteMovieAsync(int movieId)
    {
        try
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null)
            {
                return Result<bool>.Error("Movie not found", 404);
            }

            // Check if movie has any bookings
            var hasBookings = await _movieRepository.HasBookingsAsync(movieId);
            if (hasBookings)
            {
                return Result<bool>.Error("Cannot delete movie with existing bookings", 400);
            }

            var deleted = await _movieRepository.DeleteAsync(movieId);
            if (!deleted)
            {
                return Result<bool>.Error("Failed to delete movie", 500);
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, "Movie deleted successfully");
        }
        catch (Exception ex)
        {
            return Result<bool>.Error($"Error deleting movie: {ex.Message}", 500);
        }
    }

    public async Task<Result<PagedResultDTO<AdminMovieDto>>> GetMoviesForAdminAsync(int page, int pageSize)
    {
        try
        {
            // Get movies with aggregated data from repository
            var (movies, stats, totalCount) = await _movieRepository.GetMoviesForAdminAsync(page, pageSize);

            // Map to DTOs
            var adminMovies = movies.Select(m => new AdminMovieDto
            {
                MovieId = m.Movieid,
                Title = m.Title,
                DurationMinutes = m.Durationminutes,
                Rating = m.Rating,
                Description = m.Description,
                Director = m.Director,
                ReleaseDate = m.Releasedate?.ToString("yyyy-MM-dd"),
                Country = m.Country,
                Genre = m.Genre,
                PosterUrl = m.Posterurl,
                TrailerUrl = m.Trailerurl,
                TotalShowtimes = stats[m.Movieid].TotalShowtimes,
                TotalBookings = stats[m.Movieid].TotalBookings,
                Revenue = stats[m.Movieid].Revenue,
                AverageRating = stats[m.Movieid].AverageRating,
                TotalReviews = stats[m.Movieid].TotalReviews
            }).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedResult = new PagedResultDTO<AdminMovieDto>
            {
                Items = adminMovies,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalCount,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };

            return Result<PagedResultDTO<AdminMovieDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResultDTO<AdminMovieDto>>.Error($"Error retrieving movies: {ex.Message}", 500);
        }
    }
}
