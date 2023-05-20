using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces.Common
{
    public interface IGetViaSelectObjectService<TEntity, TSelectObject> : IDbAccessServise<TEntity> where TEntity : class
    {
        public IQueryable<TEntity> GetViaSelectionObject(TSelectObject? selectionObject, IQueryable<TEntity> entities);
        public async Task<IEnumerable<TEntity>> GetListViaSelectionObjectAsync(TSelectObject? selectionObject, int? page = default, int? pageSize = default) 
            => await GetPageContent(GetViaSelectionObject(selectionObject, DbSet), page, pageSize).ToListAsync();

        public IQueryable<T> GetPageContent<T>(IQueryable<T> query, int? page = default, int? pageSize = default)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }
    }
}
