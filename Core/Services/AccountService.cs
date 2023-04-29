using Data.Constants;
using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Logic.Services;

public class AccountService : IAccountService
{
    private readonly IAppDBContext _context;
    private readonly IHashProvider _hashProvider;


    public AccountService(IAppDBContext context, IHashProvider hashProvider)
    {
        _context = context;
        _hashProvider = hashProvider;
    }

    public async Task<(bool, string)> AddUser(User userToAdd)
    {
        if (userToAdd is null)
        {
            return (false, $"{nameof(userToAdd)} is null");
        }

        if (await _context.Users.AnyAsync(u => u.Login == userToAdd.Login))
        {
            return (false, "This login is taken");
        }

        var userEntity = new User 
        { 
            Login = userToAdd.Login, 
            Name = userToAdd.Name, 
            Surname = userToAdd.Surname, 
            Patronymic = userToAdd.Patronymic, 
            PasswordHash = _hashProvider.GetHash(userToAdd.PasswordHash)
        };
        
        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
        return (true, $"User {userEntity.Login} created");
    }

    public async Task<Role?> GetRoleAsync(int id)
    {
        if (IncludeModels.RolesNavigation.SuperAdminRoleID == id) return new Role { ID = id, Name = IncludeModels.RolesNavigation.SuperAdminRoleName };
        if (IncludeModels.RolesNavigation.AdminRoleID == id) return new Role { ID = id, Name = IncludeModels.RolesNavigation.AdminRoleName };
        if (IncludeModels.RolesNavigation.OrdinaryUserRoleID == id) return new Role { ID = id, Name = IncludeModels.RolesNavigation.OrdinaryUserRoleName };
        return await _context.Roles.FirstAsync(u => u.ID == id);
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await _context.Users.Include(u=>u.Role).FirstOrDefaultAsync(u => u.ID == id);
    }

    public async Task<User?> GetUserAsync(string login)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.Include(u => u.Role).ToListAsync();
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
        var roleExists = await _context.Roles.AnyAsync(r => r.ID == roleId);
        var user = await _context.Users.FirstOrDefaultAsync(r => r.ID == userId);
        if (roleExists && user != null)
        {
            user.RoleId = roleId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetUserAsync(string login, string password)
    {
        return await _context.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == _hashProvider.GetHash(password));
    }

    public async Task UpdateUser(User userUpdaateData)
    {
        var userToUpdate = await GetUserAsync(userUpdaateData.Login) ?? throw new ObjectNotFoundException($"User with login '{userUpdaateData.Login}' not found");
        userToUpdate.Name = userUpdaateData.Name;
        userToUpdate.Surname = userUpdaateData.Surname;
        userToUpdate.Patronymic = userUpdaateData.Patronymic;
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(string userLogin, string password)
    {
        var userToUpdate = await GetUserAsync(userLogin) ?? throw new ObjectNotFoundException($"User with login '{userLogin}' not found");
        userToUpdate.PasswordHash = _hashProvider.GetHash(password);
        await _context.SaveChangesAsync();
    }
}
