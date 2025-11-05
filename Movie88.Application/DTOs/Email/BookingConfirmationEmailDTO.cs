namespace Movie88.Application.DTOs.Email;

/// <summary>
/// DTO for booking confirmation email with QR code
/// Contains all information needed to generate professional invoice email
/// </summary>
public class BookingConfirmationEmailDTO
{
    // Customer Information
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    
    // Booking Information
    public string BookingCode { get; set; } = string.Empty;
    public string QRCodeBase64 { get; set; } = string.Empty; // Base64 PNG for inline attachment
    
    // Movie & Cinema Details
    public string MovieTitle { get; set; } = string.Empty;
    public string CinemaName { get; set; } = string.Empty;
    public string CinemaAddress { get; set; } = string.Empty;
    public DateTime ShowtimeDateTime { get; set; }
    public string SeatNumbers { get; set; } = string.Empty; // e.g., "A5, A6, A7"
    
    // Combo Items
    public List<ComboItemDTO> ComboItems { get; set; } = new();
    
    // Payment Details
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; } // 0 if no voucher
    public string? VoucherCode { get; set; }
    public string TransactionCode { get; set; } = string.Empty;
    public DateTime? PaymentTime { get; set; }
}

/// <summary>
/// DTO for combo item in booking confirmation email
/// </summary>
public class ComboItemDTO
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
