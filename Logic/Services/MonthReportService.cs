using Database;
using Database.Entities;
using Logic.Exceptions;
using Logic.Interfaces;
using Logic.Models.MonthReports;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class MonthReportService : IMonthReportService
    {
        public DbSet<MonthReport> DbSet { get; private set; }

        public Func<CancellationToken, Task<int>> SaveChangesAsync {get; private set;}

        public MonthReportService(AppDbContext dbContext)
        {
            DbSet = dbContext.Set<MonthReport>();
            SaveChangesAsync = dbContext.SaveChangesAsync;
        }

        public async Task UpdateAsync(MonthReport valueToAply, CancellationToken token = default)
        {
            var valueToUpdate = await DbSet.FirstOrDefaultAsync(m => m.LinkingPartID == valueToAply.LinkingPartID && m.Year == valueToAply.Year && m.Month == valueToAply.Month, cancellationToken: token)
                ?? throw new ObjectNotFoundException($"Month report not found by key [m.LinkingPartID == {valueToAply.LinkingPartID}, Year == {valueToAply.Month}, m.Month = {valueToAply.Month}]");
            DbSet.Remove(valueToUpdate);
            await SaveChangesAsync(token);
            DbSet.Add(valueToAply);
            await SaveChangesAsync(token);
        }

        public async Task AddAsync(MonthReport entity, bool SaveChanges = true, CancellationToken token = default)
        {
            await DbSet.AddAsync(entity, token);
            if (SaveChanges)
                await SaveChangesAsync(token);
        }

        public IQueryable<MonthReport> GetViaSelectionObject(MonthReportPartSelectObject? selectionObject, IQueryable<MonthReport> entities)
        {
            throw new NotImplementedException();
        }
    }
}
