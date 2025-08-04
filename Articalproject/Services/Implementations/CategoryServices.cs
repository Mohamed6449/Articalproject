using Articalproject.Models;

using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Services.Implementations
{
	public class CategoryServices : ICategoryServices
	{
		#region Fields

		private readonly IUnitOfWork _unitOfWork;

		#endregion

		#region Constructor

		public CategoryServices(IUnitOfWork unitOfWork)
		{

			_unitOfWork = unitOfWork;
		}

	
		#endregion

		#region Hundle Function

		public async Task<List<Category>> GetCategories()
		{
			return await _unitOfWork.Repository<Category>().GetAsListAsync();
		}

		//public async Task<Category?> GetCategoryById(int CategoryId)
		//{
		//	return await _unitOfWork.Repository<Category>().GetAsQueryble().Include(I => I.product).AsNoTracking().FirstOrDefaultAsync(F=>F.Id==CategoryId);
		//}

		public async Task<Category?> GetCategoryByIdWithOutInclude(int CategoryId)
		{
			return await _unitOfWork.Repository<Category>().GetProductbyIdAsync(CategoryId);
		}

		public async Task<bool> IsCategoryNameArExist(string Name)
		{
			return await _unitOfWork.Repository<Category>().GetAsQueryble().AnyAsync(A => A.NameAr == Name);
		}

		public async Task<bool> IsCategoryNameEnExist(string Name)
		{
			return await _unitOfWork.Repository<Category>().GetAsQueryble().AnyAsync(A => A.NameEn == Name);
		}

		public async Task<string> UpdateCategoryAsync(Category Category)
		{

			try
			{

				await _unitOfWork.Repository<Category>().UpdateAsync(Category);
				return "success";


			}
			catch (Exception ex)
			{
				return ex.Message + "--" + ex.InnerException;
			}

		}
		public async Task<string>AddCategoryAsync(Category Category)
		{
			try
			{

				await _unitOfWork.Repository<Category>().AddAsync(Category);
				return "success";


			}
			catch (Exception ex)
			{
				return ex.Message + "--" + ex.InnerException;
			}



		}

		public async Task<string> DeleteCategoryAsync(Category Category)
		{
			try
			{

				await _unitOfWork.Repository<Category>().DeleteAsync(Category);
				return "success";


			}
			catch (Exception ex)
			{
				return ex.Message + "--" + ex.InnerException;
			}


		}

		public IQueryable<Category> GetCategorysAsQerayable(string? search)
		{
			var Category = _unitOfWork.Repository<Category>().GetAsQueryble();

			if (search != null)
			{
				Category = Category.Where(W => W.NameAr.Contains(search) || W.NameEn.Contains(search));
			}
			return Category;


		}
		#endregion





	}
}
