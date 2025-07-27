using Articalproject.SharedRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace Articalproject.UnitOfWorks
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;

        public Task<IDbContextTransaction> BeginTransactionAsync();

        public Task CommitTransactionAsync();
        public Task RollBackTransactionAsync();

        public Task<int> CompleteAsync();
    }
}
