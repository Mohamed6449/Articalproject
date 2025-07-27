using AutoMapper;
using Articalproject.Helper;
using Articalproject.Models.Identity;
using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Articalproject.Controllers
{
	public class ClaimController : Controller

	{
		private readonly IClaimService _claimService;
		private readonly IMapper _mapper;
		public ClaimController(IMapper mapper,IClaimService claimService)
		{
			_mapper = mapper;
			_claimService= claimService;
		}
		public async Task< IActionResult>Index()
		{
			var claims=await _claimService.GetClaimsAsync();

			var result = _mapper.Map<List<GetClaimsViewModel>>(claims);
			return View(result);
		}

		public IActionResult Create()
		{

			ViewBag.Chooses = new SelectList(Store.Chooses, "Id", CultureHelper.IsArabic() ? "NameAr" : "NameEn") ;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(AddClaimViewModel model)
		{
			if (ModelState.IsValid)
			{
				var claim =_mapper.Map<Claim>(model);

				var result =await _claimService.AddClaimAsync(claim);

				if(result == "Success")
				{
					return RedirectToAction(nameof(Index));
				}
				ViewBag.Chooses = new SelectList(Store.Chooses, "Id", CultureHelper.IsArabic() ? "NameAr" : "NameEn");
				ModelState.AddModelError("", result);
			}
			ViewBag.Chooses = new SelectList(Store.Chooses, "Id", CultureHelper.IsArabic() ? "NameAr" : "NameEn");
			return View(model);
		}


		public async Task<IActionResult> Delete(int Id)
		{

			var claim = await _claimService.GetClaimAsync(Id);
			if (claim == null) return NotFound();

			_claimService.DeleteClaimAsync(claim);
			return RedirectToAction(nameof(Index));


		}
	}
}
