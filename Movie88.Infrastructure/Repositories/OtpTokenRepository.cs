using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Mappers;

namespace Movie88.Infrastructure.Repositories;

public class OtpTokenRepository : IOtpTokenRepository
{
    private readonly AppDbContext _context;

    public OtpTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OtpTokenModel> CreateAsync(OtpTokenModel otpToken)
    {
        var entity = OtpTokenMapper.ToEntity(otpToken);
        _context.OtpTokens.Add(entity);
        await _context.SaveChangesAsync();
        return OtpTokenMapper.ToModel(entity);
    }

    public async Task<OtpTokenModel?> GetByCodeAsync(string otpCode, string otpType, string email)
    {
        var entity = await _context.OtpTokens
            .Where(o => o.Otpcode == otpCode 
                     && o.Otptype == otpType 
                     && o.Email == email)
            .OrderByDescending(o => o.Createdat)
            .FirstOrDefaultAsync();

        return entity != null ? OtpTokenMapper.ToModel(entity) : null;
    }

    public async Task<bool> MarkAsUsedAsync(int otpId, string? ipAddress = null, string? userAgent = null)
    {
        var entity = await _context.OtpTokens.FindAsync(otpId);
        if (entity == null) return false;

        entity.Isused = true;
        entity.Usedat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        entity.Ipaddress = ipAddress;
        entity.Useragent = userAgent;

        // Don't call SaveChangesAsync here - let UnitOfWork handle it
        return true;
    }

    public async Task<OtpTokenModel?> GetActiveOtpAsync(int userId, string otpType)
    {
        var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var entity = await _context.OtpTokens
            .Where(o => o.Userid == userId 
                     && o.Otptype == otpType
                     && !o.Isused
                     && o.Expiresat > now)
            .OrderByDescending(o => o.Createdat)
            .FirstOrDefaultAsync();

        return entity != null ? OtpTokenMapper.ToModel(entity) : null;
    }

    public async Task<bool> InvalidateUserOtpsAsync(int userId, string otpType)
    {
        var tokens = await _context.OtpTokens
            .Where(o => o.Userid == userId 
                     && o.Otptype == otpType
                     && !o.Isused)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Isused = true;
            token.Usedat = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> DeleteExpiredOtpsAsync()
    {
        var cutoffDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-7), DateTimeKind.Unspecified);
        var expiredTokens = await _context.OtpTokens
            .Where(o => o.Expiresat < cutoffDate)
            .ToListAsync();

        _context.OtpTokens.RemoveRange(expiredTokens);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> GetOtpCountAsync(int userId, string otpType, TimeSpan timespan)
    {
        var since = DateTime.SpecifyKind(DateTime.UtcNow.Subtract(timespan), DateTimeKind.Unspecified);
        return await _context.OtpTokens
            .Where(o => o.Userid == userId 
                     && o.Otptype == otpType
                     && o.Createdat > since)
            .CountAsync();
    }
}
