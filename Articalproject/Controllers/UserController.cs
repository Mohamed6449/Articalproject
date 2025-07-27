using AutoMapper;
using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Articalproject.ViewModels.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using secClaims=System.Security.Claims;

namespace Articalproject.Controllers
{
    //[Authorize(Roles = "Admin")]

    public class UserController : Controller
	{
		private readonly IClaimService _claimService;
		private readonly UserManager<User> _userManager;

		private readonly RoleManager<IdentityRole> _roleManager;

		private readonly IMapper _mapper;

		private readonly IUnitOfWork _unitOfWork;
		public UserController(IUnitOfWork unitOfWork
			,RoleManager<IdentityRole> roleManager
			,UserManager<User> userManager , IMapper mapper,
			IClaimService claimService)
		{
			_unitOfWork = unitOfWork;
			_roleManager = roleManager;
			_mapper = mapper;
			_userManager = userManager;
			_claimService= claimService;
		}
		public async Task< IActionResult> Index()
		{
			var users =await _userManager.Users.ToListAsync();

			var result = _mapper.Map<List<GetUsersListViewModel>>(users);
			
			
			
			return View(result);
		}

		public async Task<IActionResult> Update(string Id)
		{
			var User = await _userManager.FindByIdAsync(Id);
			if (User == null) return NotFound();

			var newUser = _mapper.Map<UpdateUserViewModel>(User);


			return View(newUser);

		}

		[HttpPost]
		public async Task<IActionResult> Update(UpdateUserViewModel model)
		{

			if (ModelState.IsValid)
			{

				var User = await _userManager.FindByIdAsync(model.Id);
				if (User == null) return NotFound();

				var newUser = _mapper.Map(model,User);

				var result = await _userManager.UpdateAsync(newUser);

				if(result.Succeeded) return RedirectToAction(nameof(Index));



			}

				return View(model);


		}
		public async Task<IActionResult> Delete(string Id)
		{
			var User = await _userManager.FindByIdAsync(Id);
			if (User == null) return NotFound();

			var newUser = _mapper.Map<GetUserByIdViewModel>(User);

			return View(newUser);

		}
		[HttpPost,ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirm(string Id)
		{
			var User = await _userManager.FindByIdAsync(Id);
			if (User == null) return NotFound();

			var result=await _userManager.DeleteAsync(User);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(Index));
			}
			foreach(var error in result.Errors.ToList())
			{
				ModelState.AddModelError("",error.Description);
			}

			return View();

		}

		public async Task<IActionResult> ManageRoleInUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return NotFound();
			var roles = await _roleManager.Roles.ToListAsync();

			var roleManagers=new List<ManageRolesViewModel>();

			foreach(var item in roles)
			{
				var rolemanager = _mapper.Map<ManageRolesViewModel>(item);
				rolemanager.IsSelected= await _userManager.IsInRoleAsync(user, item.Name);
				roleManagers.Add(rolemanager);
			}
			ViewBag.userId = userId;
			return View(roleManagers);
		}
		[HttpPost]

		public async Task<IActionResult> ManageRoleInUser(List<ManageRolesViewModel> manageRoles,string userId)
		{

			var trans = await _unitOfWork.BeginTransactionAsync();
			try
			{

				var user =await _userManager.FindByIdAsync(userId);
				if(user == null) return NotFound();
			
				foreach(var item in manageRoles)
				{
					var role = await _roleManager.FindByIdAsync(item.RoleId);
					if (role == null) return NotFound();

					if (item.IsSelected&&!(await _userManager.IsInRoleAsync(user, item.RoleName)))
					{
						await _userManager.AddToRoleAsync(user, item.RoleName);
					}
					else if(!item.IsSelected&&(await _userManager.IsInRoleAsync(user, item.RoleName)))
					{
						await _userManager.RemoveFromRoleAsync(user, item.RoleName);
					}
					else
					{
						continue;
					}
				}
				await trans.CommitAsync(); 
				return RedirectToAction(nameof(Index));

			}
			catch(Exception ex)
			{
				await trans.RollbackAsync();
				ModelState.AddModelError("", ex.Message);
				return View(manageRoles);
			}
		}

		public async Task<IActionResult> ManageUsereClaims(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return NotFound();

			var Allclaims = await _claimService.GetClaimsAsync();
			var claims=Allclaims.Where(W => W.IsUserClaim == true);

			var userClaims = await _userManager.GetClaimsAsync(user);

			var model = new ManageUserClaimViewModel()
			{
				UserId = userId
			};
			foreach (var item in claims)
			{
				var userclaim = new UserClaim();
				if(userClaims.Any(A => A.Type == item.NameEn))
				{
					userclaim.IsSelected = true;
				}
				else
				{
					userclaim.IsSelected = false;
				}
				userclaim.ClaimType = item.Localize(item.NameAr,item.NameEn);


				model.Claims.Add(userclaim);
			}

		
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ManageUsereClaims(ManageUserClaimViewModel Model)
		{
			var trans= await _unitOfWork.BeginTransactionAsync();

			try
			{

			var user = await _userManager.FindByIdAsync(Model.UserId);
			if (user == null) return NotFound();

			var userClaims = await _userManager.GetClaimsAsync(user);

			var result = await _userManager.RemoveClaimsAsync(user, userClaims);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "try again there is an error ");
				return View(Model);
			}
			var selectedUserClaims = Model.Claims.Where(W => W.IsSelected == true).
				Select(src => new secClaims.Claim(src.ClaimType, src.IsSelected.ToString())).ToList();

			var addClaims = await _userManager.AddClaimsAsync(user, selectedUserClaims);

			if (!addClaims.Succeeded)
			{
				ModelState.AddModelError("", "try again there is an error ");
				return View(Model);
			}

			await	trans.CommitAsync();
			return RedirectToAction(nameof(Index));
			}
			catch(Exception ex)
			{
				await trans.RollbackAsync();
				ModelState.AddModelError("", ex.Message);
				return View(Model);
			}
		}
	}
}
