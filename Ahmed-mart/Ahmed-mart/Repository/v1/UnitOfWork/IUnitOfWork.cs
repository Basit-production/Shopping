using Ahmed_mart.Models.v1;
using Ahmed_mart.Repository.v1.GenericRepository;

namespace Ahmed_mart.Repository.v1.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class, IEntityBase;
        void BeginTransaction();
        Task BeginTransactionAsync();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
