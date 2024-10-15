using Ahmed_mart.DbContexts.v1;
using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1.GenericRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ahmed_mart.Repository.v1.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories;
        private readonly SqlDbContext _sqlDbContext;
        private IDbContextTransaction _dbContextTransaction;
        private bool _disposed;

        public UnitOfWork(SqlDbContext sqlDbContext)
        {
            _sqlDbContext = sqlDbContext;
            _repositories = new Dictionary<Type, object>();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class, IEntityBase
        {
            //return new GenericRepository<T>(_sqlDbContext);
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)_repositories[typeof(T)];
            }
            var repository = new GenericRepository<T>(_sqlDbContext);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        public void BeginTransaction()
        {
            _dbContextTransaction = _sqlDbContext.Database.BeginTransaction();
        }

        public async Task BeginTransactionAsync()
        {
            _dbContextTransaction = await _sqlDbContext.Database.BeginTransactionAsync();
        }

        public int SaveChanges()
        {
            return _sqlDbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _sqlDbContext.SaveChangesAsync();
        }

        public void Commit()
        {
            try
            {
                _dbContextTransaction.Commit();
            }
            catch (Exception)
            {
                _dbContextTransaction.Rollback();
                throw;
            }
            finally
            {
                _dbContextTransaction.Dispose();
                _dbContextTransaction = null!;
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _dbContextTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                await _dbContextTransaction.RollbackAsync();
                throw new Exception("Error committing changes.", ex);
            }
            finally
            {
                await _dbContextTransaction.DisposeAsync();
                _dbContextTransaction = null!;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _sqlDbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Rollback()
        {
            _dbContextTransaction.Rollback();
            _dbContextTransaction.Dispose();
            _dbContextTransaction = null!;
        }

        public async Task RollbackAsync()
        {
            await _dbContextTransaction.RollbackAsync();
            await _dbContextTransaction.DisposeAsync();
            _dbContextTransaction = null!;
        }
    }
}
