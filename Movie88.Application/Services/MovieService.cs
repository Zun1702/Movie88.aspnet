using AutoMapper;
using Movie88.Application.DTOs.Common;
using Movie88.Application.DTOs.Movies;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;

namespace Movie88.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IMapper _mapper;

    public MovieService(
        IMovieRepository movieRepository,
        IReviewRepository reviewRepository,
        IShowtimeRepository showtimeRepository,
        IMapper mapper)
    {
        _movieRepository = movieRepository;
        _reviewRepository = reviewRepository;
        _showtimeRepository = showtimeRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResultDTO<MovieDTO>>> GetMoviesAsync(
        int page, 
        int pageSize, 
        string? genre, 
        int? year, 
        string? rating, 
        string? sort)
    {
        var (movies, totalCount) = await _movieRepository.GetMoviesAsync(page, pageSize, genre, year, rating, sort);

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedResult = new PagedResultDTO<MovieDTO>
        {
            Items = movieDTOs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalCount,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };

        return Result<PagedResultDTO<MovieDTO>>.Success(pagedResult, "Movies retrieved successfully");
    }

    public async Task<Result<PagedResultDTO<MovieDTO>>> GetNowShowingMoviesAsync(int page, int pageSize)
    {
        var (movies, totalCount) = await _movieRepository.GetNowShowingMoviesAsync(page, pageSize);

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedResult = new PagedResultDTO<MovieDTO>
        {
            Items = movieDTOs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalCount,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };

        return Result<PagedResultDTO<MovieDTO>>.Success(pagedResult, "Now showing movies retrieved successfully");
    }

    public async Task<Result<PagedResultDTO<MovieDTO>>> GetComingSoonMoviesAsync(int page, int pageSize)
    {
        var (movies, totalCount) = await _movieRepository.GetComingSoonMoviesAsync(page, pageSize);

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedResult = new PagedResultDTO<MovieDTO>
        {
            Items = movieDTOs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalCount,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };

        return Result<PagedResultDTO<MovieDTO>>.Success(pagedResult, "Coming soon movies retrieved successfully");
    }

    public async Task<Result<PagedResultDTO<MovieDTO>>> SearchMoviesAsync(string query, int page, int pageSize)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Result<PagedResultDTO<MovieDTO>>.BadRequest("Search query is required");
        }

        var (movies, totalCount) = await _movieRepository.SearchMoviesAsync(query, page, pageSize);

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var pagedResult = new PagedResultDTO<MovieDTO>
        {
            Items = movieDTOs,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalItems = totalCount,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };

        return Result<PagedResultDTO<MovieDTO>>.Success(pagedResult, "Search results retrieved successfully");
    }

    public async Task<Result<MovieDetailDTO>> GetMovieByIdAsync(int movieId)
    {
        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
        {
            return Result<MovieDetailDTO>.NotFound("Movie not found");
        }

        // Map basic movie info
        var movieDetailDTO = _mapper.Map<MovieDetailDTO>(movie);

        // Get computed fields
        movieDetailDTO.AverageRating = await _reviewRepository.GetAverageRatingByMovieIdAsync(movieId);
        movieDetailDTO.TotalReviews = await _reviewRepository.GetCountByMovieIdAsync(movieId);
        movieDetailDTO.TotalShowtimes = await _showtimeRepository.GetActiveShowtimesCountByMovieIdAsync(movieId);

        return Result<MovieDetailDTO>.Success(movieDetailDTO, "Movie retrieved successfully");
    }
}
