using Movie88.Application.DTOs.Vouchers;
using Movie88.Application.HandlerResponse;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Enums;

namespace Movie88.Application.Services;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IBookingRepository _bookingRepository;

    public VoucherService(
        IVoucherRepository voucherRepository,
        IBookingRepository bookingRepository)
    {
        _voucherRepository = voucherRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<Result<ValidateVoucherResponseDTO>> ValidateVoucherAsync(
        ValidateVoucherRequestDTO request,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        // 1. Find voucher by code
        var voucher = await _voucherRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (voucher == null)
        {
            return Result<ValidateVoucherResponseDTO>.Failure("Voucher code not found");
        }

        // 2. Get booking and verify ownership
        var booking = await _bookingRepository.GetByIdAsync(request.Bookingid, cancellationToken);
        if (booking == null)
        {
            return Result<ValidateVoucherResponseDTO>.Failure("Booking not found");
        }

        if (booking.Customerid != customerId)
        {
            return Result<ValidateVoucherResponseDTO>.Failure("This booking does not belong to you");
        }

        // 3. Verify booking status is Pending (using enum)
        if (booking.Status?.ToLower() != nameof(BookingStatus.Pending).ToLower())
        {
            return Result<ValidateVoucherResponseDTO>.Failure("Can only apply voucher to pending bookings");
        }

        // 4. Validate voucher is active
        if (voucher.Isactive != true)
        {
            return Result<ValidateVoucherResponseDTO>.Failure("Voucher is not active");
        }

        // 5. Check expiry date
        if (voucher.Expirydate.HasValue && voucher.Expirydate.Value < DateOnly.FromDateTime(DateTime.Now))
        {
            return Result<ValidateVoucherResponseDTO>.Failure("Voucher has expired");
        }

        // 6. Check usage limit
        if (voucher.Usagelimit.HasValue && voucher.Usedcount >= voucher.Usagelimit)
        {
            return Result<ValidateVoucherResponseDTO>.Failure("Voucher usage limit has been reached");
        }

        // 7. Check minimum purchase amount
        if (voucher.Minpurchaseamount.HasValue && booking.Totalamount < voucher.Minpurchaseamount)
        {
            return Result<ValidateVoucherResponseDTO>.Failure(
                $"Minimum purchase amount is {voucher.Minpurchaseamount:N0} VND");
        }

        // 8. Calculate applicable discount
        var applicableDiscount = await CalculateDiscountAsync(voucher.Voucherid, booking.Totalamount ?? 0, cancellationToken);

        // 9. Return validation result
        var response = new ValidateVoucherResponseDTO
        {
            Voucherid = voucher.Voucherid,
            Code = voucher.Code,
            Description = voucher.Description,
            Discounttype = voucher.Discounttype,
            Discountvalue = voucher.Discountvalue,
            Minpurchaseamount = voucher.Minpurchaseamount,
            Expirydate = voucher.Expirydate,
            Usagelimit = voucher.Usagelimit,
            Usedcount = voucher.Usedcount,
            Isactive = voucher.Isactive,
            ApplicableDiscount = applicableDiscount
        };

        return Result<ValidateVoucherResponseDTO>.Success(response);
    }

    public async Task<decimal> CalculateDiscountAsync(
        int voucherId,
        decimal totalAmount,
        CancellationToken cancellationToken = default)
    {
        var voucher = await _voucherRepository.GetByIdAsync(voucherId, cancellationToken);
        if (voucher == null || voucher.Discountvalue == null)
        {
            return 0;
        }

        decimal discount;

        if (voucher.Discounttype?.ToLower() == "percentage")
        {
            // Percentage discount
            discount = totalAmount * (voucher.Discountvalue.Value / 100);
        }
        else // "fixed"
        {
            // Fixed amount discount
            discount = voucher.Discountvalue.Value;
        }

        // Don't let discount exceed total amount
        return Math.Min(discount, totalAmount);
    }
}
