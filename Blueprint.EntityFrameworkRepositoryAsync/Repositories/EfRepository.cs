using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blueprint.EntityFrameworkRepositoryAsync
{
    public abstract class EfRepository<C, T, TId> : IRepository<T, TId>
        where TId : IEquatable<TId>
        where T : Entity<TId>, IAggregateRoot
        where C : DbContext, new()
    {
        protected C _dbContext;

        protected EfRepository(C dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Document: https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{T}.</returns>
        public Task<T> GetByIdAsync(TId id)
        {
            return _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id.Equals(id));
        }

        public Task<List<T>> ListAsync()
        {
            return _dbContext.Set<T>().ToListAsync();
        }

        public Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public Task ReloadAsync(T entity)
        {
            if (entity != null)
            {
                _dbContext.Entry(entity).ReloadAsync();
            }

            return Task.CompletedTask;
        }

        public Task LoadCollectionAsync(T entity, string propertyName)
        {
            if (entity != null)
            {
                _dbContext.Entry(entity).Collection(propertyName).LoadAsync();
            }

            return Task.CompletedTask;
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task CommitAndReloadAsync(T entity)
        {
            await _dbContext.SaveChangesAsync();
            if (entity != null)
            {
                _ = _dbContext.Entry(entity).ReloadAsync();
            }
        }
    }
}
