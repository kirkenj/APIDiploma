﻿using Database.Entities;

namespace Logic.Interfaces
{
    public interface IAccountService : IDbAccessServise<User>, IConfirmService<User>
    {
        public bool IsAdmin(User user);
        public Task<IEnumerable<User>> GetUsersAsync();
        public Task UpdatePasswordAsync(string userLogin, string password, CancellationToken token = default);
    }
}