using Data.Constants;
using Database.Entities;
using Database.Interfaces;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services;

public class AccountService : IAccountService
{
    private readonly IHashProvider _hashProvider;
    private readonly IRoleService _roleService;
    public IAccountService iAccountService => this;

    public DbSet<User> DbSet { get; private set; }

    public Func<CancellationToken, Task<int>> SaveChangesAsync {get; private set;}

    public AccountService(IAppDBContext context, IHashProvider hashProvider, IRoleService roleService)
    {
        _hashProvider = hashProvider;
        _roleService = roleService;
        DbSet = context.Set<User>();
        SaveChangesAsync = context.SaveChangesAsync;
    }

    public virtual async Task AddAsync(User userToAdd, CancellationToken token = default)
    {
        if (await DbSet.AnyAsync(u => u.Login == userToAdd.Login))
        {
            throw new ArgumentException("This login is taken");
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
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await DbSet.Include(u => u.Role).ToListAsync();
    }

    public bool IsAdmin(User user) => _roleService.IsAdminRoleID(user.RoleId);

    public async Task UpdatePasswordAsync(string userLogin, string password, CancellationToken token = default)
    {
        var userToUpdate = await DbSet.FirstOrDefaultAsync(e => e.Login == userLogin) ?? throw new ObjectNotFoundException($"User with login '{userLogin}' not found");
        userToUpdate.PasswordHash = _hashProvider.GetHash(password);
        await SaveChangesAsync.Invoke(CancellationToken.None);
    }

    public async Task UpdateAsync(User valuesToAply, CancellationToken token = default)
    {
        var userToUpdate = await DbSet.FirstOrDefaultAsync(e => e.Login == valuesToAply.Login, cancellationToken: token) ?? throw new ObjectNotFoundException($"User with login '{valuesToAply.Login}' not found");
        userToUpdate.Name = valuesToAply.Name;
        userToUpdate.Surname = valuesToAply.Surname;
        userToUpdate.Patronymic = valuesToAply.Patronymic;
        if (valuesToAply.RoleId != -1)
        {
            var role = await _roleService.GetRoleAsync(valuesToAply.RoleId) ?? throw new ObjectNotFoundException($"Role with ID = {valuesToAply.RoleId} not found");
        }

        await SaveChangesAsync.Invoke(CancellationToken.None);
    }

    public Task OnObjectConfirmedAsync(User entity, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }
}
