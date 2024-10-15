using Ahmed_mart.DbContexts.v1;
using Ahmed_mart.Models.v1;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ahmed_mart.Repository.v1.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntityBase
    {
        protected readonly DbSet<T> _dbSet;
        private readonly SqlDbContext _sqlDbContext;

        public GenericRepository(SqlDbContext sqlDbContext)
        {
            _dbSet = sqlDbContext.Set<T>();
            _sqlDbContext = sqlDbContext ?? throw new ArgumentNullException(nameof(sqlDbContext));
        }

        #region new repos

        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false)
        {
            IQueryable<T> query = includeDeleted ? _dbSet : _dbSet.Where(x => !x.IsDeleted);
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = includeProperties.Aggregate(_dbSet.AsQueryable(), (current, includeProperty) => current.Include(includeProperty));
            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = includeProperties.Aggregate(_dbSet.AsQueryable(), (current, includeProperty) => current.Include(includeProperty));
            return await query.Where(predicate).ToListAsync();
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public T Update(T entity)
        {
            _dbSet.Attach(entity);
            _sqlDbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _sqlDbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IEnumerable<T> UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _sqlDbContext.Entry(entity).State = EntityState.Modified;
            }
            return entities;
        }

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _sqlDbContext.Entry(entity).State = EntityState.Modified;
            }
            return entities;
        }
        #endregion

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet.Where(x => x.IsDeleted == false);
        }

        public virtual IQueryable<T> GetAllIncludingDeleted()
        {
            return _dbSet;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(x => x.IsDeleted == false).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllincludingDeleted()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual IQueryable<T> All
        {
            get { return GetAll(); }
        }

        public virtual async Task<IEnumerable<T>> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public T GetSingle(int id)
        {
            return GetAll().FirstOrDefault(x => x.ID == id);
        }

        public async Task<T> GetSingleAsync(int id)
        {
            return await GetAll().FirstOrDefaultAsync(x => x.ID == id);
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindByAsync(params Expression<Func<T, bool>>[] predicates)
        {
            IQueryable<T> query = _dbSet;
            foreach (var includePredicate in predicates)
            {
                query = query.Where(includePredicate);
            }
            return await query.ToListAsync();
        }

        public virtual async Task<bool> Exists(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            var result = await query.ToListAsync();
            return result.Count > 0 ? true : false;
        }

        public virtual async Task<IEnumerable<T>> Search(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindByAsyncWithInclude(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(x => x.ID == id);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public virtual void BulkInsert(IList<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void BulkUpdate(IList<T> entities)
        {
            try
            {
                _sqlDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                _sqlDbContext.ChangeTracker.LazyLoadingEnabled = false;
                foreach (var entity in entities)
                {
                    _dbSet.Attach(entity);
                    _sqlDbContext.Entry(entity).State = EntityState.Modified;
                }
            }
            finally
            {
                _sqlDbContext.ChangeTracker.AutoDetectChangesEnabled = true;
                _sqlDbContext.ChangeTracker.LazyLoadingEnabled = true;
            }
        }

        public virtual void Edit(T entity)
        {
            _dbSet.Attach(entity);
            _sqlDbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public virtual void AddOrUpdate(T entity)
        {
            var existingEntity = _dbSet.Find(entity.ID);

            if (existingEntity != null)
            {
                _sqlDbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                _sqlDbContext.Entry(existingEntity).State = EntityState.Modified;
            }
            else
            {
                _dbSet.Add(entity);
            }
        }

        public virtual async Task<IEnumerable<T>> GetPageAsync(int pageSize, int pageNo, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.OrderBy(x => x.ID).Skip((pageNo - 1) * pageSize).Take(pageSize).AsNoTracking();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetPageAsync(Expression<Func<T, bool>> predicate, int pageSize, int pageNo, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(predicate).OrderBy(x => x.ID).Skip((pageNo - 1) * pageSize).Take(pageSize).AsNoTracking();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }

        public virtual IEnumerable<T> FindByIncluding(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual void AddOrUpdateList(IList<T> entities)
        {
            try
            {
                _sqlDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                _sqlDbContext.ChangeTracker.LazyLoadingEnabled = false;

                foreach (var entity in entities)
                {
                    if (entity.ID == 0)
                    {
                        _dbSet.Add(entity);
                    }
                    else
                    {
                        _sqlDbContext.Entry(entity).State = EntityState.Modified;
                    }
                }
            }
            finally
            {
                _sqlDbContext.ChangeTracker.AutoDetectChangesEnabled = true;
                _sqlDbContext.ChangeTracker.LazyLoadingEnabled = true;
            }
        }
    }
}
