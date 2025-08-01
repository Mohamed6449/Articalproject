using AutoMapper;
using Articalproject.Data;
using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Articalproject.ViewModels.Identity.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using cls = System.Security.Claims;

namespace Articalproject.Controllers
{
	//[Authorize(Roles = "Admin")]

	public class RoleController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<User> _userManager;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IClaimService _claimService;

		public RoleController(IClaimService claimService, IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_mapper = mapper;
			_roleManager = roleManager;
			_claimService = claimService;
		}
		public IActionResult Index()
		{
			var roles = _roleManager.Roles.ToList();
			var result = _mapper.Map<List<GetRolesViewModel>>(roles);
			return View(result);
		}

		public IActionResult Create()
		{
			return View();
		}
		public async Task  Creato()
		{
            var role = new IdentityRole()
            {
                Name = "User",
            };
            var role2 = new IdentityRole()
            {
                Name = "Admin",
            };
            await _roleManager.CreateAsync(role);
            await _roleManager.CreateAsync(role2);

        }

        [HttpPost]
		public async Task<IActionResult> Create(CreateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				var role = new IdentityRole()
				{
					Name = model.Name
				};
				var result = await _roleManager.CreateAsync(role);

				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Index));

				}

				return View(model);
			}

			return View(model);
		}


		public async Task<IActionResult> Update(string Id)
		{

			var role = await _roleManager.FindByIdAsync(Id);

			if (role == null) return NotFound();

			var result = _mapper.Map<UpdateRoleViewModel>(role);

			return View(result);
		}
		[HttpPost]

		public async Task<IActionResult> Update(UpdateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				var role = await _roleManager.FindByIdAsync(model.Id);

				if (role == null) return NotFound();

				var newRole = _mapper.Map(model, role);

				var result = await _roleManager.UpdateAsync(newRole);


				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Index));
				}
				return View(model);

			}
			return View(model);
		}


		public async Task<IActionResult> Details(string Id)
		{

			var role = await _roleManager.FindByIdAsync(Id);

			if (role == null) return NotFound();

			var result = _mapper.Map<GetRoleByIdViewModel>(role);

			return View(result);
		}

		public async Task<IActionResult> Delete(string Id)
		{

			var role = await _roleManager.FindByIdAsync(Id);

			if (role == null) return NotFound();

			var result = _mapper.Map<GetRoleByIdViewModel>(role);

			return View(result);
		}
		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirm(string Id)
		{

			var role = await _roleManager.FindByIdAsync(Id);

			if (role == null) return NotFound();

			var result = await _roleManager.DeleteAsync(role);

			if (result.Succeeded) return RedirectToAction(nameof(Index));
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> ManageUsersInRole(string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null) return NotFound();

			var users = await _userManager.Users.ToListAsync();

			var manageUsersList = new List<ManageUsersInRoleViewModel>();
			foreach (var user in users)
			{
				var managedUser = _mapper.Map<ManageUsersInRoleViewModel>(user);
				managedUser.IsSelected = await _userManager.IsInRoleAsync(user, role.Name);
				manageUsersList.Add(managedUser);
			}
			ViewBag.roleId = roleId;

			return View(manageUsersList);
		}

		public async Task<IActionResult> ManageUsersInRole(List<ManageUsersInRoleViewModel> manageUser, string roleId)
		{
			var trans = await _unitOfWork.BeginTransactionAsync();
			try
			{

				var role = await _roleManager.FindByIdAsync(roleId);
				if (role == null) return NotFound();

				foreach (var item in manageUser)
				{
					var user = await _userManager.FindByIdAsync(item.UserId);
					if (user == null) return NotFound();

					if (item.IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
					{
						await _userManager.AddToRoleAsync(user, role.Name);
					}
					else if (!item.IsSelected && (await _userManager.IsInRoleAsync(user, role.Name)))
					{
						await _userManager.RemoveFromRoleAsync(user, role.Name);
					}
					else
					{
						continue;
					}
				}
				await trans.CommitAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{

				await trans.RollbackAsync();
				ModelState.AddModelError("", ex.Message);
				return View(manageUser);
			}

		}
		public async Task<IActionResult> ManageClaimsInRole(string RoleId)
		{
			var Role = await _roleManager.FindByIdAsync(RoleId);

			if (Role == null) return NotFound();

			var AllClaims = await _claimService.GetClaimsAsync();
			var Claims = AllClaims.Where(W => W.IsRoleClaim == true);


			var rolesClaims = await _roleManager.GetClaimsAsync(Role);

			var ClaimsManger = new ManageRoleClaimViewModel();
			ClaimsManger.RoleId = RoleId;

			foreach (var item in Claims)
			{
				var claimManger = new RoleClaims()
				{
					ClaimType = item.Localize(item.NameAr, item.NameEn)
				};
				if (rolesClaims.Any(A => A.Type == item.NameEn))
				{
					claimManger.IsSelected = true;
				}
				else
				{

					claimManger.IsSelected = false;
				}
				ClaimsManger.Claims.Add(claimManger);
			}

			return View(ClaimsManger);

		}

		[HttpPost]
		public async Task<IActionResult> ManageClaimsInRole(ManageRoleClaimViewModel Model)
		{
			var trans = await _unitOfWork.BeginTransactionAsync();
			try
			{

				var Role = await _roleManager.FindByIdAsync(Model.RoleId);

				if (Role == null) return NotFound();

				var rolesClaims = await _roleManager.GetClaimsAsync(Role);

				foreach (var item in Model.Claims)
				{
					var claim = new cls.Claim(item.ClaimType, item.IsSelected.ToString());
					if (item.IsSelected && !rolesClaims.Any(A => A.Type == item.ClaimType))
					{
						await _roleManager.AddClaimAsync(Role, claim);
					}
					else if(!item.IsSelected && rolesClaims.Any(A => A.Type == item.ClaimType))
					{
						claim=rolesClaims.FirstOrDefault(F => F.Type == item.ClaimType);
						await _roleManager.RemoveClaimAsync(Role, claim);

					}
					else
					{
						continue;
					}
				}


				await trans.CommitAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				await trans.RollbackAsync();
				ModelState.AddModelError("", ex.Message);
				return View(Model);
			}
		}
	}


}






