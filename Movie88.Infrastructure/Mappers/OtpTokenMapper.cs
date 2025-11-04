using Movie88.Domain.Models;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Mappers;

public static class OtpTokenMapper
{
    public static OtpTokenModel ToModel(OtpToken entity)
    {
        return new OtpTokenModel
        {
            Id = entity.Id,
            UserId = entity.Userid,
            OtpCode = entity.Otpcode,
            OtpType = entity.Otptype,
            Email = entity.Email,
            CreatedAt = DateTime.SpecifyKind(entity.Createdat, DateTimeKind.Utc),
            ExpiresAt = DateTime.SpecifyKind(entity.Expiresat, DateTimeKind.Utc),
            IsUsed = entity.Isused,
            UsedAt = entity.Usedat.HasValue 
                ? DateTime.SpecifyKind(entity.Usedat.Value, DateTimeKind.Utc) 
                : null,
            IpAddress = entity.Ipaddress,
            UserAgent = entity.Useragent
        };
    }

    public static OtpToken ToEntity(OtpTokenModel model)
    {
        return new OtpToken
        {
            Id = model.Id,
            Userid = model.UserId,
            Otpcode = model.OtpCode,
            Otptype = model.OtpType,
            Email = model.Email,
            Createdat = DateTime.SpecifyKind(model.CreatedAt, DateTimeKind.Unspecified),
            Expiresat = DateTime.SpecifyKind(model.ExpiresAt, DateTimeKind.Unspecified),
            Isused = model.IsUsed,
            Usedat = model.UsedAt.HasValue 
                ? DateTime.SpecifyKind(model.UsedAt.Value, DateTimeKind.Unspecified) 
                : null,
            Ipaddress = model.IpAddress,
            Useragent = model.UserAgent
        };
    }
}
