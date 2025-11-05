using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class PaymentmethodRepository : IPaymentmethodRepository
{
    protected readonly AppDbContext _context;

    public PaymentmethodRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentmethodModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var method = await _context.Paymentmethods
            .FirstOrDefaultAsync(pm => pm.Name == name, cancellationToken);

        return method == null ? null : MapToModel(method);
    }

    private PaymentmethodModel MapToModel(Paymentmethod method)
    {
        return new PaymentmethodModel
        {
            Methodid = method.Methodid,
            Name = method.Name,
            Description = method.Description
        };
    }
}
