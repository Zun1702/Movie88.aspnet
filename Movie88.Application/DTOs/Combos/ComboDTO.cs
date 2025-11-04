using Movie88.Application.DTOs.Bookings;

namespace Movie88.Application.DTOs.Combos;

public class ComboDTO
{
    public int Comboid { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Imageurl { get; set; }
}

public class AddComboRequestDTO
{
    public int Comboid { get; set; }
    public int Quantity { get; set; }
}

public class AddCombosRequestDTO
{
    public List<AddComboRequestDTO> Combos { get; set; } = new();
}

public class UpdatedBookingResponseDTO
{
    public int Bookingid { get; set; }
    public string? Bookingcode { get; set; }
    public int Showtimeid { get; set; }
    public List<ComboItemDTO> Combos { get; set; } = new();
    public decimal Totalamount { get; set; }
    public string Status { get; set; } = string.Empty;
}

// ComboItemDTO is already defined in DTOs.Bookings namespace
// Reusing that to avoid duplication
