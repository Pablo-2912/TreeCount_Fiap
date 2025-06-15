using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TreeCount.Domain.Interfaces.Repository;
using TreeCount.Repository.Context;

namespace TreeCount.Repository.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly AppDbContext _context;

        public RepositoryBase(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }
        public virtual async Task<T> CreateAsync(T model)
        {
            await _dbSet.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public virtual async Task<bool> DeleteAsync(T model)
        {
            _dbSet.Remove(model);
            return await _context.SaveChangesAsync() > 0;
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T model)
        {
            var entry = _context.Entry(model);

            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(model);
            }

            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return model;
        }


        public async Task<IEnumerable<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdatePartialAsync(T model, params Expression<Func<T, object>>[] updatedProperties)
        {
            _dbSet.Attach(model);
            var entry = _context.Entry(model);

            foreach (var property in updatedProperties)
            {
                entry.Property(property).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            return filter == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(filter);
        }
    }
}
