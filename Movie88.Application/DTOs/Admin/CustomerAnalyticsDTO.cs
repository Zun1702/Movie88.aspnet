using System;
using System.Collections.Generic;

namespace Movie88.Application.DTOs.Admin;

/// <summary>
/// Customer analytics with retention and demographics
/// </summary>
public class CustomerAnalyticsDTO
{
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public string RetentionRate { get; set; } = "0%"; // Percentage string e.g., "75%"
    public string ChurnRate { get; set; } = "0%"; // Percentage string e.g., "25%"
    public decimal AverageLifetimeValue { get; set; }
    public List<TopCustomerDTO> TopCustomers { get; set; } = new();
    public DemographicsDTO Demographics { get; set; } = new();
}

public class TopCustomerDTO
{
    public int CustomerId { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime MemberSince { get; set; }
}

public class DemographicsDTO
{
    public Dictionary<string, string> Age { get; set; } = new(); // "18-24": "35%"
    public Dictionary<string, string> Gender { get; set; } = new(); // "male": "55%"
}
