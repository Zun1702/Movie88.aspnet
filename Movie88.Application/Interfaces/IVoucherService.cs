using Movie88.Application.DTOs.Vouchers;
using Movie88.Application.HandlerResponse;

namespace Movie88.Application.Interfaces;

public interface IVoucherService
{
    Task<Result<ValidateVoucherResponseDTO>> ValidateVoucherAsync(
        ValidateVoucherRequestDTO request,
        int customerId,
        CancellationToken cancellationToken = default);

    Task<decimal> CalculateDiscountAsync(
        int voucherId,
        decimal totalAmount,
        CancellationToken cancellationToken = default);
}
