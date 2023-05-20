using Database;
using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.Department;
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

        public async Task AddAsync(Department department, bool SaveChanges = true, CancellationToken token = default)
        {
            if (await DbSet.AnyAsync(d => d.Name== department.Name, token))
            {
                throw new ArgumentException("Name is taken");
            }

            DbSet.Add(department);
            if (SaveChanges)
                await SaveChangesAsync(token);
        }

        public IQueryable<Department> GetViaSelectionObject(DepartmentSelectObject? selectionObject, IQueryable<Department> entities)
        {
            if (selectionObject == null)
            {
                return entities;
            }

            if (selectionObject.IDs != null)
            {
                entities = entities.Where(c => selectionObject.IDs.Contains(c.ID));
            }

            if (selectionObject.Name != null)
            {
                entities = entities.Where(c => c.Name.Contains(selectionObject.Name));
            }

            return entities;
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
