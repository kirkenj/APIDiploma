using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IAppDBContext _appDBContext;
        public DepartmentService(IAppDBContext appDBContext)
        {
            _appDBContext= appDBContext;
        }

        public async Task Create(Department department)
        {
            if (_appDBContext.Departments.Any(d => d.Name== department.Name))
            {
                throw new ArgumentException("Name is taken");
            }

            _appDBContext.Departments.Add(department);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _appDBContext.Departments.FirstOrDefaultAsync(d => d.ID == id);
            if (item is null)
            {
                throw new ObjectNotFoundException($"Entity with this ID = {id} not found");
            }

            _appDBContext.Departments.Remove(item);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task<Department?> FindByID(int id)=>await _appDBContext.Departments.FirstOrDefaultAsync(d => d.ID==id);

        public async Task<Department?> FindByNameAsync(string name) => await _appDBContext.Departments.FirstOrDefaultAsync(d => d.Name == name);

        public async Task<List<Department>> GetAllAsync() => await _appDBContext.Departments.ToListAsync();

        public async Task<bool> TryEditAsync(int id, string newName)
        {
            var dep = await FindByID(id);
            if (dep is null)
            {
                return false;
            }

            dep.Name = newName;
            await _appDBContext.SaveChangesAsync();
            return true;
        }
    }
}
