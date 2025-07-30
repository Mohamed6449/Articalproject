using Articalproject.Models.Identity;
using Articalproject.Resources;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.Controllers
{

    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<SharedResources> _sharedResources;
        private readonly IMapper _mapper;
        public AccountController(UserManager<User> userManager ,
                                 SignInManager<User> signInManager,
                                 IStringLocalizer<SharedResources> sharedResources ,
                                 IMapper mapper, IEmailSender emailSender , IAccountService accountService,
                                  IMemoryCache cache,
                                  ILogger<HomeController> logger)
        {
            _cache = cache;
            _userManager = userManager;
            _sharedResources=sharedResources;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        
        public IActionResult Login(string? ReturnUrl)
        {
            
            var model = new LoginViewModel() {
                ReturnUrl = ReturnUrl
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
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
                if (!user.EmailConfirmed)
                {
                    return RedirectToAction("EmailNotConfirmed" , new { Id = user.Id });
                }

                var result=await _signInManager.PasswordSignInAsync(user, model.Password, model.RemberMe, false);
                 if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(model.ReturnUrl)&& Url.IsLocalUrl(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    _logger.LogInformation("دخل المستخدم على الصفحة الرئيسية");
                    return RedirectToAction("Index","Home");

                }
                return View(model);
                
            }

            return View(model);
        }


        public IActionResult Admin()
        {
            return View();
        }




        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
               var user=await _userManager.FindByEmailAsync(register.Email);

                if (user == null)
               
                {
                     user=await _userManager.FindByNameAsync(register.UserName);
                    if (user == null)
                    {
                        var NewUser = _mapper.Map<User>(register);
                        var result = await _userManager.CreateAsync(NewUser,register.Password);
                        if (result.Succeeded)
                        {
                            var token= await _userManager.GenerateEmailConfirmationTokenAsync(NewUser);
                            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = NewUser.Id, token = token }, Request.Scheme);
                           await _emailSender.SendEmailAsync(NewUser.Email, "Confirm your email", confirmationLink);
                      
                            TempData["ConfirmEmail"] =_sharedResources[SharedResourcesKeys.ConfirmEmailMessage].Value;
                           // await _signInManager.SignInAsync(NewUser, isPersistent:false);
                           return RedirectToAction(nameof(Login));
                        }
                        foreach(var error in result.Errors)
                        {

                             ModelState.AddModelError("",error.Description);
                        }
                        return View(register);
                    }
                }
                ModelState.AddModelError("","User is already Exist");

                return View(register);
                

            }
            return View(register);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index","Home");
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsUserNameAvailable(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user == null ? Json(true) : Json(_sharedResources["UserNameIsExist"].Value);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null ? Json(true) : Json(_sharedResources["EmailIsExist"].Value);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Success"] = _sharedResources[SharedResourcesKeys.EmailConfirmed].Value;
                _cache.Remove($"resend_{userId}");
                return RedirectToAction(nameof(Login));
            }
            return View("Error");
        }





        public async Task< IActionResult >EmailNotConfirmed(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var resendResult = _accountService.CanUserResend(Id);
                var time = resendResult.remainingTime;
                if(time!=null)
                    ViewBag.RemainingTime =$"{time.Value.Hours}:{time.Value.Minutes}:{time.Value.Seconds}" ;

                ViewBag.CanResend = resendResult.canResend;
                return View("EmailNotConfirmed",user.Email);
                
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                TempData["Failed"] =_sharedResources[SharedResourcesKeys.EmailProblem].Value;
                return RedirectToAction(nameof(Login));
            }

            if (user.EmailConfirmed)
            {
                TempData["Success"] = _sharedResources[SharedResourcesKeys.EmailConfirmed].Value;
                return RedirectToAction(nameof(Login));
            }

            // إنشاء التوكن والرابط
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

            // إرسال الإيميل
            await _emailSender.SendEmailAsync(email, "Confirm your email", confirmationLink);

            TempData["ConfirmEmail"] =_sharedResources[SharedResourcesKeys.ConfirmEmailMessage].Value;
            _accountService.RecordResend(user.Id);
             return RedirectToAction(nameof(Login));
        }
    }
}
