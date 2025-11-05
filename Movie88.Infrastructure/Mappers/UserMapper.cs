using Movie88.Domain.Models;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Mappers
{
    public static class UserMapper
    {
        public static UserModel ToModel(this User entity)
        {
            return new UserModel
            {
                UserId = entity.Userid,
                Fullname = entity.Fullname,
                Email = entity.Email,
                Passwordhash = entity.Passwordhash,
                Phone = entity.Phone,
                Roleid = entity.Roleid,
                Createdat = entity.Createdat,
                Updatedat = entity.Updatedat,
                Role = entity.Role != null ? new RoleModel
                {
                    Roleid = entity.Role.Roleid,
                    Rolename = entity.Role.Rolename
                } : null
            };
        }

        public static User ToEntity(this UserModel model)
        {
            return new User
            {
                Userid = model.UserId,
                Fullname = model.Fullname,
                Email = model.Email,
                Passwordhash = model.Passwordhash,
                Phone = model.Phone,
                Roleid = model.Roleid,
                Createdat = model.Createdat,
                Updatedat = model.Updatedat,
                Isverified = model.IsVerified,
                Isactive = model.IsActive,
                Verifiedat = model.VerifiedAt
            };
        }
    }
}
