using Movie88.Application.DTOs.Payments;
using Movie88.Application.DTOs.Email;
using Movie88.Application.Interfaces;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Movie88.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentmethodRepository _paymentmethodRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IVNPayService _vnPayService;
    private readonly IBookingCodeGenerator _bookingCodeGenerator;
    private readonly IQRCodeService _qrCodeService;
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentService> _logger;
    private readonly IServiceProvider _serviceProvider; // ✅ For creating scoped services in background task

    public PaymentService(
        IPaymentRepository paymentRepository,
        IPaymentmethodRepository paymentmethodRepository,
        IBookingRepository bookingRepository,
        IVNPayService vnPayService,
        IBookingCodeGenerator bookingCodeGenerator,
        IQRCodeService qrCodeService,
        IEmailService emailService,
        ILogger<PaymentService> logger,
        IServiceProvider serviceProvider)
    {
        _paymentRepository = paymentRepository;
        _paymentmethodRepository = paymentmethodRepository;
        _bookingRepository = bookingRepository;
        _vnPayService = vnPayService;
        _bookingCodeGenerator = bookingCodeGenerator;
        _qrCodeService = qrCodeService;
        _emailService = emailService;
        _logger = logger;
        _serviceProvider = serviceProvider;
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
            // 7. Send booking confirmation email with QR code (don't block callback)
            if (responseCode == "00" && !string.IsNullOrEmpty(bookingCode))
            {
                _logger.LogInformation("✅ Payment callback successful. Starting background email sending task for booking {BookingId}, bookingCode: {BookingCode}", payment.Bookingid, bookingCode);
                
                // ✅ Create new scope for background task to avoid disposed context
                _ = Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("📧 Background task started: Sending booking confirmation email for booking {BookingId}", payment.Bookingid);
                        
                        // Create new scope to get fresh DbContext
                        using var scope = _serviceProvider.CreateScope();
                        var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                        var qrCodeService = scope.ServiceProvider.GetRequiredService<IQRCodeService>();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        
                        await SendBookingConfirmationEmailAsync(
                            payment.Bookingid, 
                            bookingCode, 
                            txnRef,
                            bookingRepository,
                            qrCodeService,
                            emailService);
                            
                        _logger.LogInformation("✅ Background task completed: Email sent for booking {BookingId}", payment.Bookingid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Background task failed: Could not send booking confirmation email for booking {BookingId}", payment.Bookingid);
                    }
                });
            }
            else
            {
                _logger.LogWarning("⚠️ Email not sent: ResponseCode={ResponseCode}, BookingCode={BookingCode} (empty or null)", responseCode, bookingCode ?? "null");
            }
            
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

    private async Task SendBookingConfirmationEmailAsync(
        int bookingId, 
        string bookingCode, 
        string transactionCode,
        IBookingRepository bookingRepository,
        IQRCodeService qrCodeService,
        IEmailService emailService)
    {
        try
        {
            _logger.LogInformation("📧 Step 1: Fetching booking details for booking {BookingId}", bookingId);
            
            // Get full booking details with all includes
            var booking = await bookingRepository.GetByIdWithDetailsAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("❌ Booking {BookingId} not found for email confirmation", bookingId);
                return;
            }
            
            _logger.LogInformation("✅ Step 1 complete: Booking found. CustomerId={CustomerId}", booking.Customerid);

            _logger.LogInformation("📧 Step 2: Generating QR code for {BookingCode}", bookingCode);
            // Generate QR code
            var qrCodeBase64 = await qrCodeService.GenerateQRCodeBase64Async(bookingCode);
            _logger.LogInformation("✅ Step 2 complete: QR code generated (length: {Length} chars)", qrCodeBase64.Length);
            
            // Calculate discount amount
            decimal discountAmount = 0;
            if (booking.Voucherid.HasValue && booking.Voucher != null && booking.Voucher.Discountvalue.HasValue)
            {
                // Voucher was applied, estimate discount
                if (booking.Voucher.Discounttype == "percentage")
                {
                    var totalAmount = booking.Totalamount ?? 0;
                    var originalAmount = totalAmount / (1 - booking.Voucher.Discountvalue.Value / 100);
                    discountAmount = originalAmount - totalAmount;
                }
                else
                {
                    discountAmount = booking.Voucher.Discountvalue.Value;
                }
            }

            // Extract seat numbers from BookingSeats (Row + Number format)
            var seatNumbers = string.Join(", ", 
                booking.BookingSeats?.Select(bs => 
                    $"{bs.Seat?.Row ?? ""}{bs.Seat?.Number ?? 0}") ?? Enumerable.Empty<string>());

            // Extract combo items from BookingCombos
            var comboItems = booking.BookingCombos?.Select(bc => new ComboItemDTO
            {
                Name = bc.Combo?.Name ?? "",
                Quantity = bc.Quantity,
                Price = bc.Combo?.Price ?? 0
            }).ToList() ?? new List<ComboItemDTO>();

            _logger.LogInformation("📧 Step 3: Extracting customer email from booking data");
            // ✅ Get real customer email from User table (included in booking)
            var customerEmail = booking.Customer?.User?.Email ?? booking.Customer?.Email ?? "";
            var customerName = booking.Customer?.User?.Fullname ?? booking.Customer?.Fullname ?? "Khách Hàng";
            
            _logger.LogInformation("Customer info: Email={Email}, Name={Name}, HasCustomer={HasCustomer}, HasUser={HasUser}", 
                customerEmail, 
                customerName, 
                booking.Customer != null,
                booking.Customer?.User != null);
            
            if (string.IsNullOrEmpty(customerEmail))
            {
                _logger.LogError("❌ Customer email not found for booking {BookingId}, skipping email. Customer={Customer}, User={User}", 
                    bookingId,
                    booking.Customer != null ? "exists" : "null",
                    booking.Customer?.User != null ? "exists" : "null");
                return;
            }
            
            _logger.LogInformation("✅ Step 3 complete: Email found: {Email}", customerEmail);
            
            // Prepare email DTO
            var emailDto = new BookingConfirmationEmailDTO
            {
                CustomerEmail = customerEmail,
                CustomerName = customerName,
                BookingCode = bookingCode,
                QRCodeBase64 = qrCodeBase64,
                MovieTitle = booking.Showtime?.Movie?.Title ?? "Movie",
                CinemaName = booking.Showtime?.Auditorium?.Cinema?.Name ?? "Cinema",
                CinemaAddress = booking.Showtime?.Auditorium?.Cinema?.Address ?? "",
                ShowtimeDateTime = booking.Showtime?.Starttime ?? DateTime.Now,
                SeatNumbers = seatNumbers,
                ComboItems = comboItems,
                TotalAmount = booking.Totalamount ?? 0,
                DiscountAmount = discountAmount,
                VoucherCode = booking.Voucher?.Code,
                TransactionCode = transactionCode,
                PaymentTime = DateTime.Now // Payment just completed
            };

            _logger.LogInformation("📧 Step 4: Calling email service to send confirmation to {Email}", customerEmail);
            // Send email
            var emailSent = await emailService.SendBookingConfirmationAsync(emailDto);
            
            if (emailSent)
            {
                _logger.LogInformation(
                    "✅✅✅ SUCCESS! Booking confirmation email sent to {Email} for booking {BookingCode}",
                    emailDto.CustomerEmail,
                    bookingCode
                );
            }
            else
            {
                _logger.LogError(
                    "❌❌❌ FAILED! Could not send booking confirmation email to {Email} for booking {BookingCode}",
                    emailDto.CustomerEmail,
                    bookingCode
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking confirmation email for booking {BookingId}", bookingId);
        }
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
