using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing booking promotions
/// </summary>
public class BookingPromotionRepository : IBookingPromotionRepository
{
    private readonly AppDbContext _context;

    public BookingPromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<int> CreateAsync(int bookingId, int promotionId, decimal discountApplied, CancellationToken cancellationToken = default)
    {
        var entity = new Bookingpromotion
        {
            Bookingid = bookingId,
            Promotionid = promotionId,
            Discountapplied = discountApplied
        };

        _context.Bookingpromotions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Bookingpromotionid;
    }

    /// <inheritdoc/>
    public async Task<List<BookingPromotionModel>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Bookingpromotions
            .Include(bp => bp.Promotion)
            .Where(bp => bp.Bookingid == bookingId)
            .ToListAsync(cancellationToken);

        return entities.Select(e => new BookingPromotionModel
        {
            Bookingpromotionid = e.Bookingpromotionid,
            Bookingid = e.Bookingid,
            Promotionid = e.Promotionid,
            Discountapplied = e.Discountapplied,
            PromotionName = e.Promotion?.Name ?? "",
            PromotionDescription = e.Promotion?.Description
        }).ToList();
    }
}
