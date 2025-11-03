using Movie88.Domain.Models;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Mappers
{
    public static class RefreshTokenMapper
    {
        public static RefreshTokenModel ToModel(this UserRefreshToken entity)
        {
            return new RefreshTokenModel
            {
                Id = (int)entity.Id,
                Token = entity.Token,
                UserId = entity.UserId,
                Revoked = entity.Revoked,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static UserRefreshToken ToEntity(this RefreshTokenModel model)
        {
            return new UserRefreshToken
            {
                Id = model.Id,
                Token = model.Token ?? string.Empty,
                UserId = model.UserId ?? string.Empty,
                Revoked = model.Revoked ?? false,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}
