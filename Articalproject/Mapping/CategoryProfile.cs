using AutoMapper;
using Articalproject.Models;
using Articalproject.ViewModels.Categories;

namespace Articalproject.Mapping
{
	public class CategoryProfile:Profile
	{
		public CategoryProfile() {

			CreateMap<Category, GetCategoriesListViewModel>().ForMember(F => F.Name, Option => Option.
	MapFrom(M => M.Localize(M.NameAr, M.NameEn)));

			CreateMap<Category, GetCategoryByIdViewModel>();
			CreateMap<Category, UpdateCategoryViewModel>();
			CreateMap<AddCategoryViewModel,Category>();
		}
	}
}
