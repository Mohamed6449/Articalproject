using Articalproject.Models;
using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Categories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;

namespace Articalproject.Controllers
{

	public class CategoryController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ICategoryServices _categoryServices;
		private readonly IMapper _mapper;
		private readonly int _PageItem;
        public CategoryController(ICategoryServices categoryServices, IMapper mapper,
									ILogger<HomeController> logger) {

			_categoryServices = categoryServices;
			_mapper = mapper;
			_logger = logger;
			_PageItem = 10; // Set the number of items per page
        }

		[HttpGet]
		public async Task<IActionResult> Index(int? Id, bool next = true)
		{
				ViewBag.Next = true;
			if (Id == null || Id == 0)
			{

				var Categories =  _categoryServices.GetCategoriesAsQueryble();
                if (Categories.Count() <= _PageItem)
                    ViewBag.Next = false;
				var newCategories = await Categories.Take(_PageItem).ToListAsync();
                var result = _mapper.Map<List<GetCategoriesListViewModel>>(newCategories);
				return View(result);

			}

			if (next)
			{
				var CategoriesId = await _categoryServices.GetCategoriesAsQueryble().Where(W => W.Id > Id).Take(_PageItem).ToListAsync();
				var result = _mapper.Map<List<GetCategoriesListViewModel>>(CategoriesId);
				ViewBag.Previous =true;
				var newId = CategoriesId.LastOrDefault().Id;
                if ( _categoryServices.GetCategoriesAsQueryble().Where(W => W.Id > newId).Count()>1)
					return View(result);
				ViewBag.Next = false;
				return View(result);
            }
			else
			{
				var CategoriesId = await _categoryServices.GetCategoriesAsQueryble().Where(W => W.Id < Id).OrderDescending().Take(_PageItem).OrderBy(O=>O.Id).ToListAsync();
				var result = _mapper.Map<List<GetCategoriesListViewModel>>(CategoriesId);
                var newId = CategoriesId.FirstOrDefault().Id;
                if (_categoryServices.GetCategoriesAsQueryble().Where(W => W.Id < newId).Count() > 1)
					ViewBag.Previous =true;

                    return View(result);


			}
		}

        public async Task<IActionResult> Search(string SearchItem)
        {
            if (string.IsNullOrEmpty(SearchItem))
            {
                return RedirectToAction(nameof(Index));
            }
            var Categories = await _categoryServices.GetCategorysAsQerayableSearch(SearchItem).ToListAsync();
			var result = _mapper.Map<List<GetCategoriesListViewModel>>(Categories);
            return View("Index", result);
        }








        [HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AddCategoryViewModel addCategoryViewModel)

		{
			if (ModelState.IsValid)
			{
				try
				{
					var category = _mapper.Map<Category>(addCategoryViewModel);

					var result = await _categoryServices.AddCategoryAsync(category);

					if (result != "success")
						return BadRequest(result);


					return RedirectToAction(nameof(Index));

				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while creating the category.");
                    ModelState.AddModelError("", $"An error occurred while creating the category: {ex.Message}");
				}
			}
				return View(addCategoryViewModel);
		}
		[HttpGet]
		public async Task<IActionResult> Update(int? Id)
		{

			if (Id == null)
			{
				return NotFound();
			}
			var category = await _categoryServices.GetCategoryByIdWithOutInclude((int)Id);
			if (category == null) return NotFound();

			var result = _mapper.Map<UpdateCategoryViewModel>(category);

			return View(result);


		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(UpdateCategoryViewModel model)
		{
			if (ModelState.IsValid)
			{
				var OldCategory = await _categoryServices.GetCategoryByIdWithOutInclude(model.Id);

				if (OldCategory == null)
				{
					return NotFound();
				}

				var category = _mapper.Map(model,OldCategory);

				var result = await _categoryServices.UpdateCategoryAsync(category);

				if (result != "success")
					return BadRequest(result);
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Delete(int? Id)
		{

			if (Id == null)
			{
				return NotFound();
			}
			var category = await _categoryServices.GetCategoryByIdWithOutInclude((int)Id);
			if (category == null) return NotFound();

			var result = _mapper.Map<GetCategoryByIdViewModel>(category);

			return View(result);


		}

        [HttpPost,ActionName("Delete")]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? Id)
        {
			if (Id == null) return NotFound();
            

            var Category = await _categoryServices.GetCategoryByIdWithOutInclude((int)Id);

            if (Category == null)
            {
                return NotFound();
            }
            var result = await _categoryServices.DeleteCategoryAsync(Category);

            if (result != "success")
                return BadRequest(result);
            return RedirectToAction(nameof(Index));

        }
    }
}
