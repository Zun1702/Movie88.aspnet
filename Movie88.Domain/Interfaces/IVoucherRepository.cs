using Movie88.Domain.Models;

namespace Movie88.Domain.Interfaces;

public interface IVoucherRepository
{
    Task<VoucherModel?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<VoucherModel?> GetByIdAsync(int voucherId, CancellationToken cancellationToken = default);
    Task IncrementUsageCountAsync(int voucherId, CancellationToken cancellationToken = default);
}
