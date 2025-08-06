using Articalproject.Models;

namespace Articalproject.Services.InterFaces
{
	public interface ICategoryServices
	{
		public Task< List<Category> >GetCategoriesAsListAsync();


		//public Task<Category?> GetCategoryById(int CategoryId);
		public Task<string> AddCategoryAsync(Category Category);
		public Task<string> UpdateCategoryAsync(Category Category);
		public Task<string> DeleteCategoryAsync(Category CategoryId);
		public Task<Category?> GetCategoryByIdWithOutInclude(int CategoryId);

		public Task<bool> IsCategoryNameArExist(string Name);
		public Task<bool> IsCategoryNameEnExist(string Name);

		public IQueryable<Category> GetCategorysAsQerayableSearch(string? search);
		public IQueryable<Category> GetCategoriesAsQueryble();

    }
}

