using Database.Entities;
using Database.Interfaces;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace Logic.Services
{
    public class AcademicDegreeService : IAcademicDegreeService
    {
        public DbSet<AcademicDegreePriceAssignation> AssignationsDBSet { get; private set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }
        public DbSet<AcademicDegree> DbSet { get; private set;}

        public async Task AddAsync(AcademicDegree entity, CancellationToken token = default)
        {
            if (await DbSet.AnyAsync(a => a.Name ==entity.Name))
            {
                throw new ArgumentException($"Name '{entity.Name}' is taken");
            }

            await DbSet.AddAsync(entity, token);
            await SaveChangesAsync(token);
        }

        public AcademicDegreeService(IAppDBContext appDBContext)
        {
            AssignationsDBSet = appDBContext.Set<AcademicDegreePriceAssignation>();
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
    }
}
