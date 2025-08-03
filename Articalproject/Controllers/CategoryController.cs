using AutoMapper;
using Articalproject.Models;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Articalproject.Controllers
{
  
    public class CategoryController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly ICategoryServices _categoryServices;
		private readonly IMapper _mapper;
		public CategoryController(ICategoryServices categoryServices , IMapper mapper,
                                    ILogger<HomeController> logger) { 
		
			_categoryServices = categoryServices;
			_mapper = mapper;
			_logger = logger;
        }

		[HttpGet]
		public async Task< IActionResult> Index()
		{
			var Categories = await _categoryServices.GetCategories();
			var result = _mapper.Map<List<GetCategoriesListViewModel>>(Categories);
			return View(result);
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
