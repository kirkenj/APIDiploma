using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.AcademicDegree;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace Logic.Services
{
    public class AcademicDegreeService : IAcademicDegreeService
    {
        public DbSet<AcademicDegreePriceAssignation> AssignmentsDBSet { get; private set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }
        public DbSet<AcademicDegree> DbSet { get; private set;}

        public async Task AddAsync(AcademicDegree entity, bool SaveChanges = true, CancellationToken token = default)
        {
            if (await DbSet.AnyAsync(a => a.Name ==entity.Name))
            {
                throw new ArgumentException($"Name '{entity.Name}' is taken");
            }

            await DbSet.AddAsync(entity, token);
            if (SaveChanges)
                await SaveChangesAsync(token);
        }

        public AcademicDegreeService(IAppDBContext appDBContext)
        {
            AssignmentsDBSet = appDBContext.Set<AcademicDegreePriceAssignation>();
            DbSet = appDBContext.Set<AcademicDegree>();
            SaveChangesAsync = appDBContext.SaveChangesAsync;
        }


        public async Task UpdateAsync(AcademicDegree valueToAply, CancellationToken token = default)
        {
            var valueToModify = await DbSet.FirstOrDefaultAsync(a => a.ID == valueToAply.ID, token) ?? throw new ObjectNotFoundException($"Academic degree with ID = {valueToAply.ID} not found");
            if (valueToModify.Name == valueToAply.Name)
            {
                return;
            }

            if (await DbSet.AnyAsync(a => a.ID != valueToModify.ID && a.Name == valueToAply.Name, token) ) 
            { 
                throw new ArgumentException($"Name '{valueToAply.Name}' is taken");
            }

            valueToModify.Name = valueToAply.Name;
            await SaveChangesAsync(token);
        }

        public IQueryable<AcademicDegree> GetViaSelectionObject(AcademicDegreeSelectObject? selectionObject, IQueryable<AcademicDegree> entities)
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
    }
}
