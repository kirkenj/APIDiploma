using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces.Common
{
    public interface IDbAccessServise<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> DbSet { get; }
        protected Func<CancellationToken, Task<int>> SaveChangesAsync { get; }
        public Task UpdateAsync(TEntity valueToAply, CancellationToken token = default);
        public Task AddAsync(TEntity entity, bool SaveChanges = true, CancellationToken token = default);
        public virtual async Task<TEntity?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) => await DbSet.FirstOrDefaultAsync(predicate, token);
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken token = default)
        {
            DbSet.Remove(entity);
            await SaveChangesAsync(token);
        }
    }
}
