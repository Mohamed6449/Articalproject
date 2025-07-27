using Articalproject.Data;
using Articalproject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Articalproject.SharedRepository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly ApplicationDbContext _Context;

		private readonly DbSet<T> _Dbset;
		public GenericRepository(ApplicationDbContext Context)
		{
			_Context = Context;
			_Dbset = _Context.Set<T>();
		}
		public async Task AddAsync(T Entity)
		{
			await _Dbset.AddAsync(Entity);
			await _Context.SaveChangesAsync();
		}

		public async Task AddRangeAsync(IEnumerable<T> Entites) {
			await _Dbset.AddRangeAsync(Entites);
			await _Context.SaveChangesAsync();

		}

		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await _Context.Database.BeginTransactionAsync();
		}

		public async Task DeleteAsync(T Entity)
		{
			 _Dbset.Remove(Entity);
			await _Context.SaveChangesAsync();

		}

		public async Task DeleteRangeAsync(IEnumerable<T> Entitys)
		{

			_Dbset.RemoveRange(Entitys);
			await _Context.SaveChangesAsync();
		}

		public async Task<List<T>> GetAsListAsync()
		{
			return await _Dbset.ToListAsync();

		}

		public IQueryable<T> GetAsQueryble()
		{
			return _Dbset.AsQueryable();

		}

		public async Task<T> GetProductbyIdAsync(int Id)
		{
			return await _Dbset.FindAsync(Id);
		}

		public async Task UpdateAsync(T Entity)
		{
			_Dbset.Update(Entity);
			await _Context.SaveChangesAsync();
		}

		public async Task UpdateRangeAsync(IEnumerable<T> Entitys)
		{
			_Dbset.UpdateRange(Entitys);
			await _Context.SaveChangesAsync();
		}
	}
}
