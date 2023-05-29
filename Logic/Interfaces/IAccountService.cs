using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.User;

namespace Logic.Interfaces
{
    public interface IAccountService : IDbAccessServise<User>, IPeriodicValueServiceWithEdit<User, UserAcademicDegreeAssignament, int, int, User>, IGetViaSelectObjectService<User, UserSelectObject>
    {
        public bool IsAdmin(User user);
        public Task UpdatePasswordAsync(string userLogin, string password, CancellationToken token = default);
    }
}
