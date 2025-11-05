namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Query parameters for customer analytics
/// </summary>
public class CustomerAnalyticsQuery
{
    public string Period { get; set; } = "month"; // "week", "month", "year", "all"
    public int TopCustomersLimit { get; set; } = 10;
}
