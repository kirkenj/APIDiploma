﻿using Data.Constants;
using Database.Entities;
using Database.Interfaces;
using Logic.Interfaces;
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

        public async Task AddAsync(Role valueToAdd, CancellationToken cancellationToken = default) 
        {
            if (await DbSet.AnyAsync(r => r.Name == valueToAdd.Name, cancellationToken)) throw new Exception($"Role name is taken {valueToAdd.Name}");
            DbSet.Add(valueToAdd);
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
    }
}