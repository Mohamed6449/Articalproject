using AutoMapper;
using Articalproject.Models.Identity;
using Articalproject.Resources;
using Articalproject.ViewModels.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<SharedResources> _sharedResources;
        private readonly IMapper _mapper;
        public AccountController(UserManager<User> userManager ,
                                 SignInManager<User> signInManager,
                                 IStringLocalizer<SharedResources> sharedResources ,
                                 IMapper mapper)
        {
            _userManager = userManager;
            _sharedResources=sharedResources;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet]
        
        public IActionResult Login(string ReturnUrl)
        {
            var model = new LoginViewModel() {
                ReturnUrl = ReturnUrl
            };
            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var IsEmailValid = new EmailAddressAttribute().IsValid(model.Email);
                var user = new User();
                if (IsEmailValid)
                {

                    user = await _userManager.FindByEmailAsync(model.Email);
                }
                else
                {
                    user = await _userManager.FindByNameAsync(model.Email);

                }
                if (user == null)
                {
                    ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.UserNameOrPassIsWrong]);
                    return View(model);
                }
                var check =await _userManager.CheckPasswordAsync(user,model.Password);
                if (!check)
                {
                    ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.UserNameOrPassIsWrong]);
                    return View(model);
                }

                var result=await _signInManager.PasswordSignInAsync(user, model.Password, model.RemberMe, false);
                 if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(model.ReturnUrl)&& Url.IsLocalUrl(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);
                    return RedirectToAction("Index","Home");

                }
                return View(model);
                
            }

            return View(model);
        }







        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
               var user=await _userManager.FindByEmailAsync(register.Email);

                if (user == null)
                {
                    var NewUser = _mapper.Map<User>(register);
                    var result = await _userManager.CreateAsync(NewUser,register.Password);
                    if (result.Succeeded)
                    {
                       await _signInManager.SignInAsync(NewUser, isPersistent:false);
                       return RedirectToAction("Index", "Home");
                    }
                    foreach(var error in result.Errors)
                    {

                         ModelState.AddModelError("",error.Description);
                    }
                    return View(register);
                }
                ModelState.AddModelError("","User is already Exist");

                return View(register);
                

            }
            return View(register);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index","Home");
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }
    }
}
