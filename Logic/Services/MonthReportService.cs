using Database;
using Database.Entities;
using Logic.Exceptions;
using Logic.Interfaces;
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
            var valueToUpdate = await DbSet.FirstOrDefaultAsync(m => m.LinkingPartID == valueToAply.LinkingPartID && m.Year == valueToAply.Month && m.Month == valueToAply.Month, cancellationToken: token)
                ?? throw new ObjectNotFoundException($"Month report not found by key [m.LinkingPartID == {valueToAply.LinkingPartID}, Year == {valueToAply.Month}, m.Month = {valueToAply.Month}]");
            DbSet.Remove(valueToUpdate);
            DbSet.Add(valueToAply);
            await SaveChangesAsync(token);
        }

        public async Task AddAsync(MonthReport entity, CancellationToken token = default)
        {
            await DbSet.AddAsync(entity, token);
            await SaveChangesAsync(token);
        }
    }
}
