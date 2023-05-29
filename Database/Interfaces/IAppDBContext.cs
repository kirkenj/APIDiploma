using Microsoft.EntityFrameworkCore;

namespace Database.Interfaces
{
    public interface IAppDBContext
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
