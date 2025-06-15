using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Interfaces.Repository
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> CreateAsync(T model);
        Task<bool> DeleteAsync(T model);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(object id);
        Task<T> UpdateAsync(T model);
        Task UpdatePartialAsync(T model, params Expression<Func<T, object>>[] updatedProperties);
        Task<IEnumerable<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filter = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> filter);

        IQueryable<T> Query();
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
    }

}
