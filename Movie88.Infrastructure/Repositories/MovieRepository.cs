using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    protected readonly AppDbContext _context;
    protected readonly IMapper _mapper;

    public MovieRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MovieModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Movies.FindAsync(id);
        return entity != null ? _mapper.Map<MovieModel>(entity) : null;
    }

    public async Task<IEnumerable<MovieModel>> GetAllAsync()
    {
        var entities = await _context.Movies.ToListAsync();
        return _mapper.Map<List<MovieModel>>(entities);
    }

    public async Task<MovieModel> AddAsync(MovieModel model)
    {
        var entity = _mapper.Map<Movie>(model);
        _context.Movies.Add(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<MovieModel>(entity);
    }

    public async Task<MovieModel> UpdateAsync(MovieModel model)
    {
        var existing = await _context.Movies.FindAsync(model.Movieid);
        if (existing == null)
            throw new InvalidOperationException("Movie not found");

        // Update only the fields that should be updated
        existing.Title = model.Title;
        existing.Description = model.Description;
        existing.Durationminutes = model.Durationminutes;
        existing.Director = model.Director;
        existing.Trailerurl = model.Trailerurl;
        existing.Releasedate = model.Releasedate;
        existing.Posterurl = model.Posterurl;
        existing.Country = model.Country;
        existing.Rating = model.Rating;
        existing.Genre = model.Genre;
        // Don't update Movieid or Createdat

        await _context.SaveChangesAsync();
        return _mapper.Map<MovieModel>(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Movies.FindAsync(id);
        if (entity == null) return false;
        
        _context.Movies.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<MovieModel> Movies, int TotalCount)> GetMoviesAsync(
        int page, 
        int pageSize, 
        string? genre, 
        int? year, 
        string? rating, 
        string? sort)
    {
        var query = _context.Movies.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(m => m.Genre != null && m.Genre.Contains(genre));
        }

        if (year.HasValue)
        {
            query = query.Where(m => m.Releasedate.HasValue && m.Releasedate.Value.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(rating))
        {
            query = query.Where(m => m.Rating == rating);
        }

        // Apply sorting
        query = sort?.ToLower() switch
        {
            "releasedate_desc" => query.OrderByDescending(m => m.Releasedate),
            "releasedate_asc" => query.OrderBy(m => m.Releasedate),
            "title_asc" => query.OrderBy(m => m.Title),
            "title_desc" => query.OrderByDescending(m => m.Title),
            _ => query.OrderByDescending(m => m.Releasedate) // Default
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var movies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var models = _mapper.Map<List<MovieModel>>(movies);

        return (models, totalCount);
    }

    public async Task<(List<MovieModel> Movies, int TotalCount)> GetNowShowingMoviesAsync(int page, int pageSize)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        // Remove timezone info for PostgreSQL timestamp without time zone
        var currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        // Movies that have been released and have active showtimes
        var query = _context.Movies
            .Where(m => m.Releasedate <= currentDate)
            .Where(m => _context.Showtimes.Any(s => s.Movieid == m.Movieid && s.Starttime >= currentDateTime))
            .OrderByDescending(m => m.Releasedate);

        var totalCount = await query.CountAsync();

        var movies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var models = _mapper.Map<List<MovieModel>>(movies);

        return (models, totalCount);
    }

    public async Task<(List<MovieModel> Movies, int TotalCount)> GetComingSoonMoviesAsync(int page, int pageSize)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        // Remove timezone info for PostgreSQL timestamp without time zone
        var currentDateTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        // Movies with future release date OR movies without any future showtimes
        var query = _context.Movies
            .Where(m => m.Releasedate > currentDate || 
                       !_context.Showtimes.Any(s => s.Movieid == m.Movieid && s.Starttime >= currentDateTime))
            .OrderBy(m => m.Releasedate);

        var totalCount = await query.CountAsync();

        var movies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var models = _mapper.Map<List<MovieModel>>(movies);

        return (models, totalCount);
    }

    public async Task<(List<MovieModel> Movies, int TotalCount)> SearchMoviesAsync(string query, int page, int pageSize)
    {
        var searchQuery = _context.Movies
            .Where(m => 
                (m.Title != null && m.Title.ToLower().Contains(query.ToLower())) ||
                (m.Director != null && m.Director.ToLower().Contains(query.ToLower())) ||
                (m.Genre != null && m.Genre.ToLower().Contains(query.ToLower())) ||
                (m.Description != null && m.Description.ToLower().Contains(query.ToLower())))
            .OrderByDescending(m => m.Releasedate);

        var totalCount = await searchQuery.CountAsync();

        var movies = await searchQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var models = _mapper.Map<List<MovieModel>>(movies);

        return (models, totalCount);
    }

    public async Task<bool> HasBookingsAsync(int movieId)
    {
        return await _context.Showtimes
            .Where(s => s.Movieid == movieId)
            .AnyAsync(s => _context.Bookings.Any(b => b.Showtimeid == s.Showtimeid));
    }

    public async Task<(List<MovieModel> Movies, Dictionary<int, MovieStatistics> Stats, int TotalCount)> GetMoviesForAdminAsync(int page, int pageSize)
    {
        // Get all movies with aggregated data
        var query = from movie in _context.Movies
                    select new
                    {
                        Movie = movie,
                        TotalShowtimes = _context.Showtimes.Count(s => s.Movieid == movie.Movieid),
                        TotalBookings = _context.Showtimes
                            .Where(s => s.Movieid == movie.Movieid)
                            .SelectMany(s => _context.Bookings.Where(b => b.Showtimeid == s.Showtimeid))
                            .Count(),
                        Revenue = _context.Showtimes
                            .Where(s => s.Movieid == movie.Movieid)
                            .SelectMany(s => _context.Bookings.Where(b => b.Showtimeid == s.Showtimeid))
                            .Where(b => _context.Payments.Any(p => p.Bookingid == b.Bookingid && p.Status == "Completed"))
                            .Sum(b => (decimal?)b.Totalamount) ?? 0,
                        TotalReviews = _context.Reviews.Count(r => r.Movieid == movie.Movieid),
                        AverageRating = _context.Reviews
                            .Where(r => r.Movieid == movie.Movieid)
                            .Average(r => (double?)r.Rating) ?? 0
                    };

        var totalCount = await query.CountAsync();

        var results = await query
            .OrderByDescending(x => x.Movie.Releasedate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Separate movies and statistics
        var movies = results.Select(x => _mapper.Map<MovieModel>(x.Movie)).ToList();
        
        var stats = results.ToDictionary(
            x => x.Movie.Movieid,
            x => new MovieStatistics
            {
                TotalShowtimes = x.TotalShowtimes,
                TotalBookings = x.TotalBookings,
                Revenue = x.Revenue,
                AverageRating = x.AverageRating,
                TotalReviews = x.TotalReviews
            });

        return (movies, stats, totalCount);
    }
}
