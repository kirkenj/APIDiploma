using Database.Entities;
using Logic.Interfaces.Common;

namespace Logic.Interfaces
{
    public interface IAccountService : IDbAccessServise<User>, IPeriodicValueServiceWithEdit<User,UserAcademicDegreeAssignament, int,  int, User>
    {
        public bool IsAdmin(User user);
        public Task<IEnumerable<User>> GetUsersAsync();
        public Task UpdatePasswordAsync(string userLogin, string password, CancellationToken token = default);
    }
}
