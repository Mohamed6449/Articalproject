using Articalproject.Models;
using Articalproject.Models.Identity;
using Articalproject.Resources;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Articalproject.ViewModels.Identity;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Data;
using secClaims = System.Security.Claims;

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
        private readonly IUnitOfWork  _unitOfWork;
        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IStringLocalizer<SharedResources> sharedResources,
                                 IMapper mapper, IEmailSender emailSender, IAccountService accountService,
                                  IMemoryCache cache,
                                  ILogger<HomeController> logger,
                                  IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _userManager = userManager;
            _sharedResources = sharedResources;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]

        public IActionResult Login(string? ReturnUrl)
        {

            var model = new LoginViewModel()
            {
                ReturnUrl = ReturnUrl
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var IsEmailValid = new EmailAddressAttribute().IsValid(model.Email);
                var user = new User();
                user = null;
                if (IsEmailValid)
                {
                    user = await _userManager.FindByEmailAsync(model.Email);
                }
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Email);
                }

                if (user == null)
                {
                    Log.Warning("Login failed. User with email {Email} not found", model.Email);
                    ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.EmailProblem]);
                    return View(model);
                }



                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning($"User {user.UserName} has been temporarily locked out due to failed login attempts.");
                    ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.AccountIsLocked]);
                    return View(model);
                }
                var check = await _userManager.CheckPasswordAsync(user, model.Password);

                if (!check)
                {
                    Log.Warning("Login failed. Incorrect password for user {Email}", user.Email);

                    await _userManager.AccessFailedAsync(user);

                    ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.UserNameOrPassIsWrong]);
                    return View(model);
                }
                if (!user.EmailConfirmed)
                {
                    Log.Warning("Login failed. Email not confirmed for user {Email}", user.Email);
                    return RedirectToAction("EmailNotConfirmed", new { Id = user.Id });
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RemberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    Log.Information($"User {user.Email} logged in successfully", user.Email);
                    _cache.Remove($"resend_{user.Id}password");
                    return RedirectToAction("Index", "Home");

                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during login for user {Email}", model.Email);
                ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.ErrorOccurred]);
                return View(model);
            }
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
                var user = await _userManager.FindByEmailAsync(register.Email);

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(register.UserName);
                    if (user == null)
                    {
                        var transations = await _unitOfWork.BeginTransactionAsync();
                        try
                        {

                            var NewUser = _mapper.Map<User>(register);
                            var result = await _userManager.CreateAsync(NewUser, register.Password);

                            if (result.Succeeded)
                            {
                                // add Author

                                var author = new Author
                                {
                                    UserId = NewUser.Id
                                };
                                await _unitOfWork.Repository<Author>().AddAsync(author);


                                _logger.LogInformation("User created successfully: {Email}", NewUser.Email);

                                try
                                {
                                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(NewUser);
                                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = NewUser.Id, token = token }, Request.Scheme);
                                    await _emailSender.SendEmailAsync(NewUser.Email, "Confirm your email", confirmationLink, 1);
                                    _logger.LogInformation("Confirmation email sent to {Email}", NewUser.Email);
                                    TempData["ConfirmEmail"] = _sharedResources[SharedResourcesKeys.ConfirmEmailMessage].Value;
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error sending confirmation email for user {Email}", NewUser.Email);
                                    ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.EmailProblem]);
                                    TempData["Failed"] = _sharedResources[SharedResourcesKeys.EmailNotSend].Value;
                                }
                                var addRole = await _userManager.AddToRoleAsync(NewUser, "User");

                                if (!addRole.Succeeded)
                                {
                                    _logger.LogWarning("Failed to add claim to user {Email}", NewUser.Email);
                                }
                                await transations.CommitAsync();
                                return RedirectToAction(nameof(Login));
                            }
                            foreach (var error in result.Errors)
                            {
                                _logger.LogWarning("User creation error: {Error}", error.Description);

                                ModelState.AddModelError("", error.Description);
                            }

                            return View(register);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred while creating user {Email}", register.Email);
                            await transations.RollbackAsync();
                            ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.ErrorOccurred].Value);
                            return View(register);
                        }
                    }
                }
                ModelState.AddModelError("", "User is already Exist");

                return View(register);

            }
                
                return View(register);
  }
        

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Logout()
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown User";
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User {UserName} has logged out.", userName);


                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during logout.");
                return RedirectToAction("Index", "Home");
            }
        }



        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsUserNameAvailable(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                return user == null ? Json(true) : Json(_sharedResources["UserNameIsExist"].Value);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username availability for {UserName}", username);
                return Json("");
            }
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user == null ? Json(true) : Json(_sharedResources["EmailIsExist"].Value);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability for {Email}", email);
                return Json("");
            }


        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailNotAvailable(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null ? Json(true) : Json(_sharedResources["EmailNotExist"].Value);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability for {Email}", email);
                return Json("");
            }


        }






        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    return NotFound();
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found during email confirmation.", userId);
                    return View("Error");
                }
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Email confirmed successfully for user {UserId}.", userId);
                    TempData["Success"] = _sharedResources[SharedResourcesKeys.EmailConfirmed].Value;
                    _cache.Remove($"resend_{userId}");
                    return RedirectToAction(nameof(Login));
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for user {UserId}.", userId);
                return View("Error");
            }

        }



        public async Task<IActionResult> EmailNotConfirmed(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    var resendResult = _accountService.CanUserResend(Id, "Email");
                    var time = resendResult.remainingTime;
                    if (time != null)
                        ViewBag.RemainingTime = $"{time.Value.Hours}:{time.Value.Minutes}:{time.Value.Seconds}";

                    ViewBag.CanResend = resendResult.canResend;
                    return View("EmailNotConfirmed", user.Email);

                }
                _logger.LogWarning("Email not confirmed page accessed for non-existent user with ID {UserId}.", Id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing EmailNotConfirmed page for user {UserId}.", Id);
                return View("Error");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    Log.Warning("Resend confirmation failed. User with email {Email} not found", email);
                    TempData["Failed"] = _sharedResources[SharedResourcesKeys.EmailProblem].Value;
                    return RedirectToAction(nameof(Login));
                }

                if (user.EmailConfirmed)
                {
                    Log.Information("Resend confirmation skipped. Email already confirmed for user {Email}", email);
                    TempData["Success"] = _sharedResources[SharedResourcesKeys.EmailConfirmed].Value;
                    return RedirectToAction(nameof(Login));
                }

                // إنشاء التوكن والرابط
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                // إرسال الإيميل
                await _emailSender.SendEmailAsync(email, "Confirm your email", confirmationLink, 1);

                TempData["ConfirmEmail"] = _sharedResources[SharedResourcesKeys.ConfirmEmailMessage].Value;
                _accountService.RecordResend(user.Id, "Email");
                Log.Information("Confirmation email resent to {Email}", email);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error resending confirmation email for user {Email}", email);
                return RedirectToAction(nameof(Login));
            }
        }
        public IActionResult ForgotPassword()
        {
            return View(new ResetPasswordRequestViewModel());
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ResetPasswordRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.EmailNotExist].Value);
                return View(model);
            }
            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Forgot password request for unconfirmed email {Email}", model.Email);
                ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.EmailNotConfirmed].Value);
                return View(model);
            }

            var resendResult = _accountService.CanUserResend(user.Id, "password");
            if (!resendResult.canResend)
            {
                var time = resendResult.remainingTime;
                if (time != null)
                    ViewBag.RemainingTime = $"{time.Value.Hours}:{time.Value.Minutes}:{time.Value.Seconds}";
                return View("CanRestPass");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, Request.Scheme);


            try
            {
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", resetLink, 2);
                _accountService.RecordResend(user.Id, "password");
                TempData["Success"] = _sharedResources[SharedResourcesKeys.SendRestPassword].Value;
                Log.Information("Reset password email sent to {Email}", model.Email);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending reset password email to {Email}", model.Email);
                ModelState.AddModelError("", _sharedResources[SharedResourcesKeys.ErrorOccurred].Value);
                return View(model);
            }
        }









        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid reset password link.");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    TempData["Failed"] = _sharedResources[SharedResourcesKeys.EmailNotExist].Value;
                    return RedirectToAction(nameof(Login));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    _cache.Remove($"resend_{user.Id}password");
                    TempData["Success"] = _sharedResources[SharedResourcesKeys.PasswordResetSuccess].Value;
                    Log.Information("Password reset successfully for user {UserId}", user.Id);

                    return RedirectToAction(nameof(Login));
                }
                Log.Warning("Password reset failed for user {UserId}. Errors: {Errors}", user.Id, result.Errors);
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error resetting password for user {UserId}", model.UserId);
                return View(model);
            }

        }

    }

}
