using Database.Constants;
using Database.Entities;
using Database.Interfaces;
using Logic.Interfaces;
using Logic.Models.Role;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class RoleService : IRoleService
    {
        public DbSet<Role> DbSet { get; set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }

        public RoleService(IAppDBContext appDBContext)
        {
            DbSet = appDBContext.Set<Role>();
            SaveChangesAsync = appDBContext.SaveChangesAsync;
        }

        public async Task AddAsync(Role valueToAdd, bool SaveChanges = true, CancellationToken cancellationToken = default)
        {
            if (await DbSet.AnyAsync(r => r.Name == valueToAdd.Name, cancellationToken)) throw new Exception($"Role name is taken {valueToAdd.Name}");
            await DbSet.AddAsync(valueToAdd, cancellationToken);
            if (SaveChanges)
                await SaveChangesAsync(cancellationToken);
        }

        public async Task<Role?> GetRoleAsync(int roleId)
        {
            if (roleId == IncludeModels.RolesNavigation.AdminRoleID)
            {
                return new Role { ID = roleId, Name = IncludeModels.RolesNavigation.AdminRoleName };
            }

            if (roleId == IncludeModels.RolesNavigation.OrdinaryUserRoleID)
            {
                return new Role { ID = roleId, Name = IncludeModels.RolesNavigation.OrdinaryUserRoleName };
            }

            if (roleId == IncludeModels.RolesNavigation.SuperAdminRoleID)
            {
                return new Role { ID = roleId, Name = IncludeModels.RolesNavigation.SuperAdminRoleName };
            }

            return await DbSet.FirstOrDefaultAsync(r => r.ID == roleId);
        }

        public bool IsAdminRoleID(int roleID)
        {
            return roleID == IncludeModels.RolesNavigation.AdminRoleID || roleID == IncludeModels.RolesNavigation.SuperAdminRoleID;
        }

        public bool IsAdminRoleName(string roleName)
        {
            return roleName == IncludeModels.RolesNavigation.AdminRoleName || roleName == IncludeModels.RolesNavigation.SuperAdminRoleName;
        }

        public async Task UpdateAsync(Role valuesToAply, CancellationToken token = default)
        {
            var roleToUpdate = await DbSet.FirstOrDefaultAsync(r => r.ID == valuesToAply.ID, token) ?? throw new DirectoryNotFoundException($"Role with ID = {valuesToAply.ID} not found");
            roleToUpdate.Name = valuesToAply.Name;
            await SaveChangesAsync(token);
        }

        public IQueryable<Role> GetViaSelectionObject(RoleSelectObject? selectionObject, IQueryable<Role> entities)
        {
            if (selectionObject == null)
            {
                return entities;
            }

            if (selectionObject.IDs != null)
            {
                entities = entities.Where(c => selectionObject.IDs.Contains(c.ID));
            }

            if (selectionObject.Name != null)
            {
                entities = entities.Where(c => c.Name.Contains(selectionObject.Name));
            }

            return entities;
        }

        public bool IsSuperAdminRoleID(int iD) => IncludeModels.RolesNavigation.SuperAdminRoleID == iD;

        public bool IsSuperAdminRoleName(string v)=>IncludeModels.RolesNavigation.SuperAdminRoleName == v;
    }
}
