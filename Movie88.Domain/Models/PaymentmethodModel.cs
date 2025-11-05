namespace Movie88.Domain.Models;

/// <summary>
/// Domain model for Paymentmethod entity
/// </summary>
public class PaymentmethodModel
{
    public int Methodid { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
