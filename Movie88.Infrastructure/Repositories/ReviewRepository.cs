using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    protected readonly AppDbContext _context;
    protected readonly IMapper _mapper;

    public ReviewRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewModel>> GetByMovieIdAsync(int movieId, int page, int pageSize, string sort)
    {
        var query = _context.Reviews
            .Include(r => r.Customer)
                .ThenInclude(c => c!.User)
            .Where(r => r.Movieid == movieId);

        // Apply sorting
        query = sort?.ToLower() switch
        {
            "oldest" => query.OrderBy(r => r.Createdat),
            "highest" => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.Createdat),
            "lowest" => query.OrderBy(r => r.Rating).ThenByDescending(r => r.Createdat),
            _ => query.OrderByDescending(r => r.Createdat) // "latest" is default
        };

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReviewModel>>(reviews);
    }

    public async Task<int> GetCountByMovieIdAsync(int movieId)
    {
        return await _context.Reviews
            .Where(r => r.Movieid == movieId)
            .CountAsync();
    }

    public async Task<decimal?> GetAverageRatingByMovieIdAsync(int movieId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.Movieid == movieId && r.Rating.HasValue)
            .ToListAsync();

        if (!reviews.Any())
            return null;

        return (decimal)reviews.Average(r => r.Rating!.Value);
    }

    public async Task<ReviewModel?> GetByCustomerAndMovieAsync(int customerId, int movieId)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Customerid == customerId && r.Movieid == movieId);

        return review == null ? null : _mapper.Map<ReviewModel>(review);
    }

    public async Task<ReviewModel> AddAsync(ReviewModel reviewModel)
    {
        var review = _mapper.Map<Review>(reviewModel);
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(review)
            .Reference(r => r.Customer)
            .Query()
            .Include(c => c!.User)
            .LoadAsync();

        return _mapper.Map<ReviewModel>(review);
    }

    public async Task<ReviewModel?> GetByIdAsync(int reviewId)
    {
        var review = await _context.Reviews
            .Include(r => r.Customer)
                .ThenInclude(c => c!.User)
            .FirstOrDefaultAsync(r => r.Reviewid == reviewId);

        return review == null ? null : _mapper.Map<ReviewModel>(review);
    }
}
