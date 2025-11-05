using Movie88.Application.Interfaces;

namespace Movie88.WebApi.Services;

/// <summary>
/// Background service to automatically cancel pending bookings that are older than 15 minutes
/// Runs every 5 minutes to check for expired bookings
/// </summary>
public class BookingExpirationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingExpirationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public BookingExpirationService(
        IServiceProvider serviceProvider,
        ILogger<BookingExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🕒 Booking Expiration Service started. Checking every {Minutes} minutes.", _checkInterval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndCancelExpiredBookingsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while checking for expired bookings");
            }

            // Wait before next check
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("🛑 Booking Expiration Service stopped.");
    }

    private async Task CheckAndCancelExpiredBookingsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Checking for expired pending bookings (older than 15 minutes)...");

        // Create a new scope to get scoped services
        using var scope = _serviceProvider.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

        try
        {
            await bookingService.AutoCancelExpiredBookingsAsync(cancellationToken);
            _logger.LogInformation("✅ Expired bookings check completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to auto-cancel expired bookings");
        }
    }
}
