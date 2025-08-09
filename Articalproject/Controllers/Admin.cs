using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Controllers
{
    [Authorize]

    public class Admin : Controller
    {
        private readonly ILogger<Admin> _logger;
        private readonly IAuthorPostServices _authorPostServices;
        private readonly UserManager<User> _userManager;

        public Admin(ILogger<Admin> logger, IAuthorPostServices authorPostServices, UserManager<User> userManager)

        {
            _userManager = userManager;
            _logger = logger;
            _authorPostServices = authorPostServices;
        }
        public async Task<IActionResult> Index()
        {
            var dateMonth = DateTime.Now.AddMonths(-1);
            var dateYear = DateTime.Now.AddYears(-1);
            var UserId = _userManager.GetUserId(User);
            var Posts =await _authorPostServices.GetAuthorPostsAsQueryble().Where(W => W.Author.user.Id == UserId).ToListAsync();
            var model = new SummaryViewModel
            {
                PostInMonth = Posts.Where(W=>W.PostDate>= dateMonth).Count(),
                PostInYear = Posts.Where(W => W.PostDate >= dateYear).Count(),
                AllPost = Posts.Count()
            };
            return View(model);
        }
    }
}
