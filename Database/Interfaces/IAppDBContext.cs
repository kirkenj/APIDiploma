using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Interfaces
{
    public interface IAppDBContext
    {
        DbSet<Contract> Contracts { get; }
        DbSet<Role> Roles { get; }
        DbSet<User> Users { get; }
        DbSet<MonthReport> MonthReports { get; }
        DbSet<Department> Departments { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
