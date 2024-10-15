using Ahmed_mart.Models.v1;
using System.Linq.Expressions;

namespace Ahmed_mart.Repository.v1.GenericRepository
{
    public interface IGenericRepository<T> where T : class, IEntityBase
    {
        #region new repos
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
        Task<T> GetByIdAsync(int id);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        Task<T> UpdateAsync(T entity);
        IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
        #endregion
        Task<IEnumerable<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> All { get; }
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIncludingDeleted();
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllincludingDeleted();
        T GetSingle(int id);
        Task<T> GetSingleAsync(int id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindByIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> Search(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindByAsync(params Expression<Func<T, bool>>[] predicates);
        Task<IEnumerable<T>> FindByAsyncWithInclude(int id, params Expression<Func<T, object>>[] includeProperties);
        void AddOrUpdate(T entity);
        void BulkInsert(IList<T> entities);
        void BulkUpdate(IList<T> entities);
        void Delete(T entity);
        void Edit(T entity);
        Task<bool> Exists(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetPageAsync(int pageSize, int pageNo, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetPageAsync(Expression<Func<T, bool>> predicate, int pageSize, int pageNo, params Expression<Func<T, object>>[] includeProperties);
        void AddOrUpdateList(IList<T> entities);
    }
}
