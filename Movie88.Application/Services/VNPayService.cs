using Microsoft.Extensions.Configuration;
using Movie88.Application.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Movie88.Application.Services;

public class VNPayService : IVNPayService
{
    private readonly IConfiguration _configuration;

    public VNPayService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GeneratePaymentUrl(int bookingId, decimal amount, string transactionCode, string returnUrl, string ipAddress)
    {
        var vnpayUrl = _configuration["VNPay:Url"];
        var tmnCode = _configuration["VNPay:TmnCode"];
        var hashSecret = _configuration["VNPay:HashSecret"];
        
        // Use default ReturnUrl from config if not provided
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = _configuration["VNPay:ReturnUrl"]!;
        }

        // VNPay uses smallest currency unit (cents for VND)
        var vnpayAmount = ((long)(amount * 100)).ToString();

        // Build VNPay parameters
        var vnpParams = new Dictionary<string, string>
        {
            {"vnp_Version", "2.1.0"},
            {"vnp_Command", "pay"},
            {"vnp_TmnCode", tmnCode!},
            {"vnp_Amount", vnpayAmount},
            {"vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
            {"vnp_CurrCode", "VND"},
            {"vnp_IpAddr", ipAddress},
            {"vnp_Locale", "vn"},
            {"vnp_OrderInfo", $"Thanh toan booking {bookingId}"},
            {"vnp_OrderType", "other"},
            {"vnp_ReturnUrl", returnUrl},
            {"vnp_TxnRef", transactionCode}
        };

        // Sort parameters by key
        var sortedParams = vnpParams.OrderBy(x => x.Key);

        // Create query string
        var queryString = string.Join("&", sortedParams.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}"));

        // Generate secure hash
        var secureHash = HmacSHA512(queryString, hashSecret!);

        // Build final URL
        return $"{vnpayUrl}?{queryString}&vnp_SecureHash={secureHash}";
    }

    public bool ValidateSignature(Dictionary<string, string> parameters, string secureHash)
    {
        var hashSecret = _configuration["VNPay:HashSecret"];

        // Sort parameters by key (exclude vnp_SecureHash)
        var sortedParams = parameters
            .Where(x => x.Key != "vnp_SecureHash")
            .OrderBy(x => x.Key);

        // Create hash data string
        var hashData = string.Join("&", sortedParams.Select(x => $"{x.Key}={x.Value}"));

        // Calculate checksum
        var checkSum = HmacSHA512(hashData, hashSecret!);

        return checkSum.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateTransactionCode(int bookingId)
    {
        return $"PAY_{DateTime.Now:yyyyMMddHHmmss}_{bookingId}";
    }

    /// <summary>
    /// Generate HMACSHA512 hash
    /// </summary>
    private string HmacSHA512(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
