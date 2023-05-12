using Database;
using Database.Entities;
using Database.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class DepartmentService : IDepartmentService
    {
        
        public DepartmentService(IAppDBContext appDBContext)
        {
            DbSet = appDBContext.Set<Department>();
            SaveChangesAsync = appDBContext.SaveChangesAsync;
        }

        public DbSet<Department> DbSet { get; private set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }

        public async Task AddAsync(Department department, CancellationToken token)
        {
            if (await DbSet.AnyAsync(d => d.Name== department.Name, token))
            {
                throw new ArgumentException("Name is taken");
            }

            DbSet.Add(department);
            await SaveChangesAsync(token);
        }

        public async Task UpdateAsync(Department valueToAply, CancellationToken token = default)
        {
            var valueToModify = await DbSet.FirstOrDefaultAsync(d => d.ID == valueToAply.ID, cancellationToken: token) ?? throw new ObjectNotFoundException($"Department with ID = {valueToAply.ID} not found");
            if (valueToModify.Name == valueToAply.Name)
            {
                return;
            }
            
            if (await DbSet.AnyAsync(d => d.Name == valueToAply.Name, token))
            {
                throw new Exception($"Department name '{valueToAply.Name}' is taken");
            }

            valueToModify.Name = valueToAply.Name;
            await SaveChangesAsync(token);
        }
    }
}
