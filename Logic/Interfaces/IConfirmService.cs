using Database.Interfaces;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces
{
    public interface IConfirmService<TEntity> : IDbAccessServise<TEntity> where TEntity : class, IConfirmableByAdminObject
    {
        internal IAccountService _accountService { get; }

        public virtual async Task ConfirmAsync(int itemID, string adminLogin, CancellationToken token = default)
        {

            var contract = await DbSet.FirstOrDefaultAsync(c => c.ID == itemID, token) ?? throw new ObjectNotFoundException($"Contract wasn't found by ID = {itemID}");
            if (contract.IsConfirmed)
            {
                throw new ArgumentException("This contract is already confirmed");
            }

            var user = await _accountService.FirstOrDefaultAsync(u => u.Login == adminLogin, token) ?? throw new ObjectNotFoundException($"User with login = '{adminLogin}' not found");
            if (!_accountService.IsAdmin(user))
            {
                throw new NoAccessException();
            }

            contract.ConfirmedByUserID = user.ID;
            await OnObjectConfirmedAsync(contract, token);
            await SaveChangesAsync(token);
        }

        public Task OnObjectConfirmedAsync(TEntity entity, CancellationToken token = default);
    }
}
