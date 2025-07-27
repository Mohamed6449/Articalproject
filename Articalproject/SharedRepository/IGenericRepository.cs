using Articalproject.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Articalproject.SharedRepository
{
	public interface IGenericRepository  <T> where T:class 
	{
		public IQueryable<T> GetAsQueryble();

		public Task<List<T>> GetAsListAsync();

		public Task<T> GetProductbyIdAsync(int Id);

		public Task AddAsync(T Entity);
		public Task UpdateAsync(T Entity);
		public Task DeleteAsync(T Entity);

		public Task AddRangeAsync(IEnumerable<T> Entitys);
		public Task DeleteRangeAsync(IEnumerable<T> Entitys);
		public Task UpdateRangeAsync(IEnumerable<T> Entitys);
		public Task<IDbContextTransaction> BeginTransactionAsync();


	}
}
