using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services;

public class AccountService : IAccountService
{
    private readonly IHashProvider _hashProvider;
    private readonly IRoleService _roleService;
    private readonly IAcademicDegreeService _academicDegreeService;
    public IAccountService iAccountService => this;

    public DbSet<User> DbSet { get; private set; }
    public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }
    public DbSet<UserAcademicDegreeAssignament> AssignmentsDBSet { get; set; }

    public AccountService(IAppDBContext context, IHashProvider hashProvider, IRoleService roleService, IAcademicDegreeService academicDegreeService)
    {
        _hashProvider = hashProvider;
        _roleService = roleService;
        DbSet = context.Set<User>();
        AssignmentsDBSet = context.Set<UserAcademicDegreeAssignament>();
        SaveChangesAsync = context.SaveChangesAsync;
        _academicDegreeService = academicDegreeService;
    }

    public virtual async Task AddAsync(User userToAdd, bool SaveChanges = true, CancellationToken token = default)
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
        if (SaveChanges)
            await SaveChangesAsync.Invoke(CancellationToken.None);
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
            userToUpdate.RoleId = role.ID;
        }

        await SaveChangesAsync.Invoke(CancellationToken.None);
    }

    public virtual async Task AddAssignmentAsync(UserAcademicDegreeAssignament assignation, CancellationToken token = default)
    {
        if (!await DbSet.AnyAsync(e => e.ID.Equals(assignation.ObjectIdentifier), token))
        {
            throw new ObjectNotFoundException($"{assignation.GetType().Name} with ID = {assignation.ObjectIdentifier} not found");
        }

        if (await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == assignation.Value, token) is null)
        {
            throw new ObjectNotFoundException($"{typeof(AcademicDegree).Name} not found with ID = {assignation.Value}");
        }

        assignation.AssignmentDate = new DateTime(assignation.AssignmentDate.Year, assignation.AssignmentDate.Month, 1, 0, 0, 0);
        if (await AssignmentsDBSet.AnyAsync(a => a.AssignmentDate == assignation.AssignmentDate && assignation.ObjectIdentifier.Equals(a.ObjectIdentifier), token))
        {
            throw new ArgumentException($"{assignation.GetType().Name} with key [AssignationDate = {assignation.AssignmentDate}, ObjectIdentifier = {assignation.ObjectIdentifier}] already exists");
        }
        await AssignmentsDBSet.AddAsync(assignation, token);
        await SaveChangesAsync(token);
    }

    public virtual async Task EditAssignmentAsync(int id, DateTime assignationActiveDate, int newValue, DateTime? newAssignationDate, CancellationToken token = default)
    {
        var assignation = await GetAssignmentOnDate(assignationActiveDate, id, token) ?? throw new ObjectNotFoundException($"{typeof(UserAcademicDegreeAssignament).Name} not found with key [activeDate = {assignationActiveDate}, ObjectID = {id}]");
        if (await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == assignation.Value, token) is null)
        {
            throw new ObjectNotFoundException($"{typeof(AcademicDegree).Name} not found with ID = {assignation.Value}");
        }

        assignation.Value = newValue;
        if (newAssignationDate.HasValue)
        {
            assignation.AssignmentDate = new DateTime(newAssignationDate.Value.Year, newAssignationDate.Value.Month, 1, 0, 0, 0);
        }

        await SaveChangesAsync(token);
    }

    public async Task<UserAcademicDegreeAssignament?> GetAssignmentOnDate(DateTime date, int objectIDToFindPerVal, CancellationToken token = default)
    {
        return await AssignmentsDBSet.OrderByDescending(a => a.AssignmentDate).FirstOrDefaultAsync(a => a.AssignmentDate <= date && a.ObjectIdentifier.Equals(objectIDToFindPerVal), token);
    }

    public IQueryable<User> GetViaSelectionObject(UserSelectObject? selectionObject, IQueryable<User> entities)
    {
        if (selectionObject == null)
        {
            return entities;
        }

        if (selectionObject.IDs != null)
        {
            entities = entities.Where(c => selectionObject.IDs.Contains(c.ID));
        }

        if (selectionObject.NSP != null)
        {
            entities = entities.Where(c => c.NSP.Contains(selectionObject.NSP));
        }

        if (selectionObject.Login != null)
        {
            entities = entities.Where(c => c.Login.Contains(selectionObject.Login));
        }

        if (selectionObject.RoleIds != null)
        {
            entities = entities.Where(c => selectionObject.RoleIds.Contains(c.RoleId));
        }

        return entities;
    }
}
