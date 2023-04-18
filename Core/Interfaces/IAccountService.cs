using Database.Entities;

namespace Logic.Interfaces
{
    public interface IAccountService
    {
        public Task<(bool succed, string explanation)> AddUser(User userToAdd);
        public Task<User?> SignInAsync(string login, string password);
        public Task<User> GetUserByLoginAsync(string Login);
        public bool IsAdmin(User user);
        public Task<User> GetUserByIDAsync(int id);
        public Task<Role> GetRoleByIDAsync(int id);
        public Task SetRoleAsync(int userId, int roleId);
        public Task<IEnumerable<User>> GetUsers();
    }
}
