using Microsoft.EntityFrameworkCore;

namespace Logic.Interfaces
{
    public interface IDbAccessServise<TEntity> where TEntity : class
    {
        DbSet<TEntity> DbSet { get; }
        public Task<int> SaveChangesAsync { get; }
    }
}
