using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.DataServices
{
    public abstract class GenericRepository<C, T, TId> : IGenericRepository<T, TId>
         where TId : IEquatable<TId>
         where T : Entity<TId>
         where C : DbContext, new()
    {
        protected C _dbContext;

        protected GenericRepository(C dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T Get(object id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Document: https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{T}.</returns>
        public async Task<T> GetByIdAsync(TId id)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id.Equals(id));
        }

        public T GetByComposite(params object[] ids)
        {
            return _dbContext.Set<T>().Find(ids);
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> query = _dbContext.Set<T>();
            return query;
        }

        public async Task<List<T>> ListAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbContext.Set<T>().Where(predicate);
            return query;
        }

        public async Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddAndSaveChangesAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
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

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async Task UpdateAndSaveChangesAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(entity).State = EntityState.Unchanged;
        }

        public void Reload(T entity)
        {
            if (entity != null)
            {
                _dbContext.Entry(entity).Reload();
            }
        }

        public async Task ReloadAsync(T entity)
        {
            if (entity != null)
            {
                await _dbContext.Entry(entity).ReloadAsync();
            }
        }

        public void LoadCollection(T entity, string propertyName)
        {
            if (entity != null)
            {
                _dbContext.Entry(entity).Collection(propertyName).Load();
            }
        }

        public async Task LoadCollectionAsync(T entity, string propertyName)
        {
            if (entity != null)
            {
                await _dbContext.Entry(entity).Collection(propertyName).LoadAsync();
            }
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void CommitAndReload(T entity)
        {
            _dbContext.SaveChanges();
            if (entity != null)
            {
                _dbContext.Entry(entity).Reload();
            }
        }

        public async Task CommitAndReloadAsync(T entity)
        {
            await _dbContext.SaveChangesAsync();
            if (entity != null)
            {
                await _dbContext.Entry(entity).ReloadAsync();
            }
        }
    }
}
