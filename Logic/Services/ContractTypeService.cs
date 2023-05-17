using Database;
using Database.Entities;
using Database.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class ContractTypeService : IContractTypeService
    {
        public DbSet<ContractTypePriceAssignment> AssignmentsDBSet { get; set; }
        public ContractTypeService(IAppDBContext appDBContext)
        {
            DbSet = appDBContext.Set<ContractType>();
            SaveChangesAsync = appDBContext.SaveChangesAsync;
            AssignmentsDBSet = appDBContext.Set<ContractTypePriceAssignment>();
        }

        public DbSet<ContractType> DbSet { get; private set; }
        public Func<CancellationToken, Task<int>> SaveChangesAsync { get; private set; }

        public async Task AddAsync(ContractType objectToAdd, bool SaveChanges = true, CancellationToken token = default)
        {
            if (await DbSet.AnyAsync(d => d.Name== objectToAdd.Name, token))
            {
                throw new ArgumentException("Name is taken");
            }

            DbSet.Add(objectToAdd);
            if (SaveChanges)
                await SaveChangesAsync(token);
        }

        public async Task UpdateAsync(ContractType valueToAply, CancellationToken token = default)
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
