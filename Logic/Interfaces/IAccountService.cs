using Database.Entities;

namespace Logic.Interfaces
{
    public interface IAccountService : IDbAccessServise<User>
    {
        public Task<(bool succed, string explanation)> AddUser(User userToAdd);
        public Task<User?> GetUserAsync(string login, string password);
        public Task<User?> GetUserAsync(string Login);
        public bool IsAdmin(User user);
        public bool IsAdmin(int roleID);
        public bool IsAdmin(string roleName);
        public Task<User?> GetUserAsync(int userId);
        public Task<Role?> GetRoleAsync(int roleId);
        public Task SetRoleAsync(int userId, int roleId);
        public Task<IEnumerable<User>> GetUsersAsync();
        public Task UpdateUser(User user);
        public Task UpdatePasswordAsync(string userLogin, string password);
    }
}
