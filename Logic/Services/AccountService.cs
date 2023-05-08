using Data.Constants;
using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services;

public class AccountService : IAccountService
{
    private readonly IHashProvider _hashProvider;

    public DbSet<User> DbSet { get; private set; }

    public Func<CancellationToken, Task<int>> SaveChangesAsync {get; private set;}

    public AccountService(IAppDBContext context, IHashProvider hashProvider)
    {
        DbSet = context.Set<User>();
        _hashProvider = hashProvider;
        SaveChangesAsync = context.SaveChangesAsync;
    }

    public async Task<(bool, string)> AddUser(User userToAdd)
    {
        if (userToAdd is null)
        {
            return (false, $"{nameof(userToAdd)} is null");
        }

        if (await DbSet.AnyAsync(u => u.Login == userToAdd.Login))
        {
            return (false, "This login is taken");
        }

        var userToSave = new User 
        { 
            Login = userToAdd.Login, 
            Name = userToAdd.Name, 
            Surname = userToAdd.Surname, 
            Patronymic = userToAdd.Patronymic, 
            PasswordHash = _hashProvider.GetHash(userToAdd.PasswordHash)
        };
        
        DbSet.Add(userToSave);
        await SaveChangesAsync.Invoke(CancellationToken.None);
        return (true, $"User {userToSave.Login} created");
    }

    public async Task<Role?> GetRoleAsync(int roleID)
    {
        if (IncludeModels.RolesNavigation.SuperAdminRoleID == roleID) return new Role { ID = roleID, Name = IncludeModels.RolesNavigation.SuperAdminRoleName };
        if (IncludeModels.RolesNavigation.AdminRoleID == roleID) return new Role { ID = roleID, Name = IncludeModels.RolesNavigation.AdminRoleName };
        if (IncludeModels.RolesNavigation.OrdinaryUserRoleID == roleID) return new Role { ID = roleID, Name = IncludeModels.RolesNavigation.OrdinaryUserRoleName };
        return await _context.Roles.FirstAsync(u => u.ID == roleID);
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await DbSet.Include(u=>u.Role).FirstOrDefaultAsync(u => u.ID == id);
    }

    public async Task<User?> GetUserAsync(string login)
    {
        return await DbSet.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await DbSet.Include(u => u.Role).ToListAsync();
    }

    public bool IsAdmin(int roleID)
    {
        return roleID == IncludeModels.RolesNavigation.AdminRoleID || roleID == IncludeModels.RolesNavigation.SuperAdminRoleID;
    }

    public bool IsAdmin(User user) => IsAdmin(user.RoleId);

    public bool IsAdmin(string roleName)
    {
        return roleName == IncludeModels.RolesNavigation.AdminRoleName || roleName == IncludeModels.RolesNavigation.SuperAdminRoleName;
    }

    public async Task SetRoleAsync(int userId, int roleId)
    {
        var roleExists = await DbSet.AnyAsync(r => r.ID == roleId);
        var user = await DbSet.FirstOrDefaultAsync(r => r.ID == userId);
        if (roleExists && user != null)
        {
            user.RoleId = roleId; 
            await SaveChangesAsync.Invoke(CancellationToken.None);
        }
    }

    public async Task<User?> GetUserAsync(string login, string password)
    {
        return await DbSet.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == _hashProvider.GetHash(password));
    }

    public async Task UpdateUser(User userUpdaateData)
    {
        var userToUpdate = await GetUserAsync(userUpdaateData.Login) ?? throw new ObjectNotFoundException($"User with login '{userUpdaateData.Login}' not found");
        userToUpdate.Name = userUpdaateData.Name;
        userToUpdate.Surname = userUpdaateData.Surname;
        userToUpdate.Patronymic = userUpdaateData.Patronymic;
        await SaveChangesAsync.Invoke(CancellationToken.None);
    }

    public async Task UpdatePasswordAsync(string userLogin, string password)
    {
        var userToUpdate = await GetUserAsync(userLogin) ?? throw new ObjectNotFoundException($"User with login '{userLogin}' not found");
        userToUpdate.PasswordHash = _hashProvider.GetHash(password);
        await SaveChangesAsync.Invoke(CancellationToken.None);
    }
}
