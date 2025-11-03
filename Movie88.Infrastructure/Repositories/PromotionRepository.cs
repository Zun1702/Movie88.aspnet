using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PromotionRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PromotionModel>> GetActivePromotionsAsync()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var promotions = await _context.Promotions
            .Where(p => p.Startdate <= currentDate && p.Enddate >= currentDate)
            .OrderByDescending(p => p.Startdate)
            .ToListAsync();

        var models = _mapper.Map<List<PromotionModel>>(promotions);

        return models;
    }
}
