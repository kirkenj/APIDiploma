using Data.Constants;
using Database.Entities;
using Database.Interfaces;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Role> GetRoleByIDAsync(int id)
    {
        return await _context.Roles.FirstAsync(u => u.ID == id);
    }

    public async Task<User> GetUserByIDAsync(int id)
    {
        return await _context.Users.Include(y => y.Contracts).ThenInclude(c => c.Department).FirstAsync(u => u.ID == id);
    }

    public async Task<User> GetUserByLoginAsync(string Login)
    {
        return await _context.Users.Include(y => y.Contracts).ThenInclude(c => c.Department).FirstAsync(u => u.Login == Login);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public bool IsAdmin(User user)
    {
        return user.RoleId == IncludeModels.RolesNavigation.AdminRoleID || user.RoleId == IncludeModels.RolesNavigation.SuperAdminRoleID;
    }

    public async Task SetRoleAsync(int userId, int roleId)
    {
        var roleExcists = await _context.Roles.AnyAsync(r => r.ID == roleId);
        var user = await _context.Users.FirstOrDefaultAsync(r => r.ID == userId);
        if (roleExcists && user != null)
        {
            user.RoleId = roleId;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> SignInAsync(string login, string password)
    {
        return await _context.Users.Include(u => u.Contracts)
            .FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == _hashProvider.GetHash(password));
    }
}
