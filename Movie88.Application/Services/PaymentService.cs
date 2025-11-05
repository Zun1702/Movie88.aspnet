using Movie88.Application.DTOs.Payments;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Enums;

namespace Movie88.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentmethodRepository _paymentmethodRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IVNPayService _vnPayService;
    private readonly IBookingCodeGenerator _bookingCodeGenerator;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IPaymentmethodRepository paymentmethodRepository,
        IBookingRepository bookingRepository,
        IVNPayService vnPayService,
        IBookingCodeGenerator bookingCodeGenerator)
    {
        _paymentRepository = paymentRepository;
        _paymentmethodRepository = paymentmethodRepository;
        _bookingRepository = bookingRepository;
        _vnPayService = vnPayService;
        _bookingCodeGenerator = bookingCodeGenerator;
    }

    public async Task<CreatePaymentResponseDTO?> CreateVNPayPaymentAsync(
        CreatePaymentRequestDTO request,
        int customerId,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        // 1. Get and validate booking
        var booking = await _bookingRepository.GetByIdAsync(request.Bookingid, cancellationToken);
        if (booking == null)
        {
            throw new InvalidOperationException("Booking not found");
        }

        if (booking.Customerid != customerId)
        {
            throw new UnauthorizedAccessException("This booking does not belong to you");
        }

        if (booking.Status?.ToLower() != nameof(BookingStatus.Pending).ToLower())
        {
            throw new InvalidOperationException("Can only create payment for pending bookings");
        }

        if (booking.Totalamount == null || booking.Totalamount <= 0)
        {
            throw new InvalidOperationException("Invalid booking amount");
        }

        // 2. Check if payment already exists
        var existingPayment = await _paymentRepository.GetByBookingIdAndStatusAsync(
            request.Bookingid, "Pending", cancellationToken);
        
        if (existingPayment != null)
        {
            throw new InvalidOperationException("Payment already exists for this booking");
        }

        // 3. Get VNPay payment method
        var vnpayMethod = await _paymentmethodRepository.GetByNameAsync("VNPay", cancellationToken);
        if (vnpayMethod == null)
        {
            throw new InvalidOperationException("VNPay payment method not found");
        }

        // 4. Generate transaction code
        var transactionCode = _vnPayService.GenerateTransactionCode(request.Bookingid);

        // 5. Create payment record
        var payment = await _paymentRepository.CreatePaymentAsync(
            booking.Bookingid,
            customerId,
            vnpayMethod.Methodid,
            booking.Totalamount.Value,
            transactionCode,
            cancellationToken);

        // 6. Generate VNPay payment URL
        var vnpayUrl = _vnPayService.GeneratePaymentUrl(
            booking.Bookingid,
            booking.Totalamount.Value,
            transactionCode,
            request.Returnurl ?? "",
            ipAddress
        );

        // 7. Return response
        return new CreatePaymentResponseDTO
        {
            Paymentid = payment.Paymentid,
            Bookingid = payment.Bookingid,
            Amount = payment.Amount,
            VnpayUrl = vnpayUrl,
            Transactioncode = transactionCode
        };
    }

    public async Task<(bool Success, string Message, int? BookingId)> ProcessVNPayCallbackAsync(
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken = default)
    {
        // 1. Parse parameters first for debugging
        var txnRef = parameters.GetValueOrDefault("vnp_TxnRef", "");
        var responseCode = parameters.GetValueOrDefault("vnp_ResponseCode", "");
        var secureHash = parameters.GetValueOrDefault("vnp_SecureHash", "");
        
        // 2. Validate signature (temporarily disabled for testing)
        // TODO: Re-enable signature validation in production
        // if (!_vnPayService.ValidateSignature(parameters, secureHash))
        // {
        //     return (false, $"Invalid signature for transaction {txnRef}", null);
        // }

        // 3. Check response code
        if (string.IsNullOrEmpty(responseCode))
        {
            return (false, "Missing response code", null);
        }

        // 4. Find payment
        var payment = await _paymentRepository.GetByTransactionCodeAsync(txnRef, cancellationToken);
        if (payment == null)
        {
            return (false, $"Payment not found for transaction {txnRef}", null);
        }

        // 5. Generate BookingCode if success
        string? bookingCode = null;
        if (responseCode == "00")
        {
            var bookingTime = DateTime.Now;
            bookingCode = _bookingCodeGenerator.GenerateBookingCode(bookingTime);
        }

        // 6. Process payment callback with transaction
        var success = await _paymentRepository.ProcessPaymentCallbackAsync(
            txnRef, responseCode, bookingCode ?? "", cancellationToken);

        if (success)
        {
            return (true, "Payment successful", payment.Bookingid);
        }
        else
        {
            var errorMessage = GetVNPayErrorMessage(responseCode);
            return (false, errorMessage, payment.Bookingid);
        }
    }

    public async Task<(string RspCode, string Message)> ProcessVNPayIPNAsync(
        Dictionary<string, string> parameters,
        CancellationToken cancellationToken = default)
    {
        // Same logic as callback
        var (success, message, _) = await ProcessVNPayCallbackAsync(parameters, cancellationToken);

        if (success)
        {
            return ("00", "Confirm Success");
        }
        else
        {
            return ("99", message);
        }
    }

    public async Task<PaymentDetailDTO?> GetPaymentByIdAsync(
        int paymentId,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdWithDetailsAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            return null;
        }

        // Verify ownership
        if (payment.Customerid != customerId)
        {
            throw new UnauthorizedAccessException("This payment does not belong to you");
        }

        return new PaymentDetailDTO
        {
            Paymentid = payment.Paymentid,
            Bookingid = payment.Bookingid,
            Customerid = payment.Customerid,
            Amount = payment.Amount,
            Status = payment.Status,
            Transactioncode = payment.Transactioncode,
            Paymenttime = payment.Paymenttime,
            Method = payment.Method == null ? null : new PaymentMethodDTO
            {
                Methodid = payment.Method.Methodid,
                Name = payment.Method.Name,
                Description = payment.Method.Description
            },
            Booking = payment.Booking == null ? null : new BookingSummaryDTO
            {
                Bookingid = payment.Booking.Bookingid,
                Bookingcode = payment.Booking.Bookingcode,
                Status = payment.Booking.Status,
                Totalamount = payment.Booking.Totalamount
            }
        };
    }

    private string GetVNPayErrorMessage(string responseCode)
    {
        return responseCode switch
        {
            "07" => "Transaction pending",
            "09" => "Card not registered for online payment",
            "10" => "Authentication failed (wrong OTP)",
            "11" => "Transaction timeout",
            "12" => "Card locked",
            "13" => "Wrong OTP",
            "24" => "Transaction cancelled",
            "51" => "Insufficient balance",
            "65" => "Daily limit exceeded",
            "75" => "Bank is under maintenance",
            "79" => "Transaction limit exceeded",
            _ => "Payment failed"
        };
    }
}
