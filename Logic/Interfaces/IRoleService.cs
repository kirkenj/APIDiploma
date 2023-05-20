using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.Role;
using Logic.Models.User;

namespace Logic.Interfaces
{
    public interface IRoleService : IDbAccessServise<Role>, IGetViaSelectObjectService<Role, RoleSelectObject>
    {
        public Task<Role?> GetRoleAsync(int roleId);
        public bool IsAdminRoleName(string roleName);
        public bool IsAdminRoleID(int roleID);
    }
}
