using Database.Entities;

namespace Logic.Interfaces
{
    public interface IRoleService : IDbAccessServise<Role>
    {
        public Task<Role?> GetRoleAsync(int roleId);
        public bool IsAdminRoleName(string roleName);
        public bool IsAdminRoleID(int roleID);
    }
}
