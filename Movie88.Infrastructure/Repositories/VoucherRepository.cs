using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class VoucherRepository : IVoucherRepository
{
    private readonly AppDbContext _context;

    public VoucherRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VoucherModel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var voucher = await _context.Vouchers
            .FirstOrDefaultAsync(v => v.Code.ToLower() == code.ToLower(), cancellationToken);

        if (voucher == null)
            return null;

        return MapToModel(voucher);
    }

    public async Task<VoucherModel?> GetByIdAsync(int voucherId, CancellationToken cancellationToken = default)
    {
        var voucher = await _context.Vouchers
            .FindAsync(new object[] { voucherId }, cancellationToken);

        if (voucher == null)
            return null;

        return MapToModel(voucher);
    }

    public async Task IncrementUsageCountAsync(int voucherId, CancellationToken cancellationToken = default)
    {
        var voucher = await _context.Vouchers.FindAsync(new object[] { voucherId }, cancellationToken);
        
        if (voucher != null)
        {
            voucher.Usedcount = (voucher.Usedcount ?? 0) + 1;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static VoucherModel MapToModel(Voucher entity)
    {
        return new VoucherModel
        {
            Voucherid = entity.Voucherid,
            Code = entity.Code,
            Description = entity.Description,
            Discounttype = entity.Discounttype,
            Discountvalue = entity.Discountvalue,
            Minpurchaseamount = entity.Minpurchaseamount,
            Expirydate = entity.Expirydate,
            Usagelimit = entity.Usagelimit,
            Usedcount = entity.Usedcount,
            Isactive = entity.Isactive
        };
    }
}
