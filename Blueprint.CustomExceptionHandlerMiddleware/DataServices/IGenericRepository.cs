using Blueprint.CustomExceptionHandlerMiddleware.Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.DataServices
{
    public interface IGenericRepository<T, TId> where T : Entity<TId>
    {
        T Get(object id);
        Task<T> GetAsync(object id);
        Task<T> GetByIdAsync(TId id);
        T GetByComposite(params object[] ids);
        IQueryable<T> GetAll();
        Task<List<T>> ListAsync();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        Task AddAsync(T entity);
        Task AddAndSaveChangesAsync(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        Task UpdateAndSaveChangesAsync(T entity);
        void Reload(T entity);
        Task ReloadAsync(T entity);
        void LoadCollection(T entity, string propertyName);
        Task LoadCollectionAsync(T entity, string propertyName);
        void Commit();
        Task CommitAsync();
        void CommitAndReload(T entity);
        Task CommitAndReloadAsync(T entity);
    }
}
