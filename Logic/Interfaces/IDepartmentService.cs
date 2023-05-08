using Database.Entities;

namespace Logic.Interfaces
{
    public interface IDepartmentService : IDbAccessServise<Department>
    {
        public Task<Department?> FindByNameAsync(string name);
        public Task<Department?> FindByIDAsync(int id);
        public Task<bool> TryEditAsync(int id, string newName);
        public Task Create(Department department);
        public Task DeleteAsync(int id);
        public Task<List<Department>> GetAllAsync();
    }
}
