using Articalproject.Data;
using Articalproject.SharedRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Articalproject.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _Context;

        private readonly Dictionary<Type,object>_Repositories=new Dictionary<Type,object>();

        //private IDbContextTransaction _Transaction;

        private bool IsDispose = false;
        public UnitOfWork(ApplicationDbContext Context)
        {
            _Context=Context;
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _Context.Database.BeginTransactionAsync();
            
        }

        public async Task CommitTransactionAsync()
        {
            await _Context.Database.CommitTransactionAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _Context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDispose)
            {
                if (disposing)
                {
                    //_Transaction.Dispose();
                    //_Context.Database.BeginTransaction().Dispose();
                    _Context.Dispose();
                }
                IsDispose = true;
            }

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_Repositories.ContainsKey(typeof(T)))
            {
                return _Repositories[typeof(T)] as IGenericRepository<T>;
            }

            var Repository = new GenericRepository<T>(_Context);
            _Repositories.Add(typeof(T), Repository);
            return Repository;
        }

        public async Task RollBackTransactionAsync()
        {
            await _Context.Database.RollbackTransactionAsync();
        }
    }
}
