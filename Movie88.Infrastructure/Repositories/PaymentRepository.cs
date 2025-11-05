using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Domain.Enums;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    protected readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentModel?> GetByIdWithDetailsAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .Include(p => p.Method)
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.Paymentid == paymentId, cancellationToken);

        return payment == null ? null : MapToModel(payment);
    }

    public async Task<PaymentModel?> GetByTransactionCodeAsync(string transactionCode, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .Include(p => p.Method)
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.Transactioncode == transactionCode, cancellationToken);

        return payment == null ? null : MapToModel(payment);
    }

    public async Task<PaymentModel?> GetByBookingIdAndStatusAsync(int bookingId, string status, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Bookingid == bookingId && p.Status == status, cancellationToken);

        return payment == null ? null : MapToModel(payment);
    }

    public async Task<PaymentModel> CreatePaymentAsync(
        int bookingId,
        int customerId,
        int methodId,
        decimal amount,
        string transactionCode,
        CancellationToken cancellationToken = default)
    {
        var payment = new Payment
        {
            Bookingid = bookingId,
            Customerid = customerId,
            Methodid = methodId,
            Amount = amount,
            Status = "Pending",
            Transactioncode = transactionCode,
            Paymenttime = null
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToModel(payment);
    }

    public async Task<bool> ProcessPaymentCallbackAsync(
        string transactionCode,
        string responseCode,
        string bookingCode,
        CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.Transactioncode == transactionCode, cancellationToken);

        if (payment == null)
        {
            return false;
        }

        if (responseCode == "00") // Success
        {
            // Use execution strategy for transaction
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Update payment
                    payment.Status = "Completed";
                    payment.Paymenttime = DateTime.Now;

                    // Update booking (using enum)
                    payment.Booking.Status = nameof(BookingStatus.Confirmed);
                    payment.Booking.Bookingcode = bookingCode;

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });

            return true;
        }
        else // Failed
        {
            payment.Status = "Failed";
            await _context.SaveChangesAsync(cancellationToken);
            return false;
        }
    }

    private PaymentModel MapToModel(Payment payment)
    {
        return new PaymentModel
        {
            Paymentid = payment.Paymentid,
            Bookingid = payment.Bookingid,
            Customerid = payment.Customerid,
            Methodid = payment.Methodid,
            Amount = payment.Amount,
            Status = payment.Status,
            Transactioncode = payment.Transactioncode,
            Paymenttime = payment.Paymenttime,
            Method = payment.Method == null ? null : new PaymentmethodModel
            {
                Methodid = payment.Method.Methodid,
                Name = payment.Method.Name,
                Description = payment.Method.Description
            },
            Booking = payment.Booking == null ? null : new BookingModel
            {
                Bookingid = payment.Booking.Bookingid,
                Bookingcode = payment.Booking.Bookingcode,
                Status = payment.Booking.Status,
                Totalamount = payment.Booking.Totalamount
            }
        };
    }
}
