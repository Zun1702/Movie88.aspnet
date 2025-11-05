using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;

namespace Movie88.Infrastructure.Repositories;

public class ComboRepository : IComboRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ComboRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ComboModel>> GetAllAsync()
    {
        var combos = await _context.Combos
            .OrderBy(c => c.Price)
            .ToListAsync();

        return combos.Select(c => new ComboModel
        {
            Comboid = c.Comboid,
            Name = c.Name,
            Description = c.Description,
            Price = c.Price,
            Imageurl = c.Imageurl
        }).ToList();
    }

    public async Task<List<ComboModel>> GetCombosByIdsAsync(List<int> comboIds, CancellationToken cancellationToken = default)
    {
        var combos = await _context.Combos
            .Where(c => comboIds.Contains(c.Comboid))
            .ToListAsync(cancellationToken);

        return combos.Select(c => new ComboModel
        {
            Comboid = c.Comboid,
            Name = c.Name,
            Description = c.Description,
            Price = c.Price,
            Imageurl = c.Imageurl
        }).ToList();
    }
}
