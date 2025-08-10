using Articalproject.Helper;
using Articalproject.Models;
using Articalproject.Models.Identity;
using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Home;
using Articalproject.ViewModels.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Articalproject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryServices _categoryServices;
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthorPostServices _authorPostServices;
        private readonly IAuthorServices _authorServices;
        private readonly int _pageSize;

        public HomeController(IAuthorServices authorServices, ILogger<HomeController> logger, ICategoryServices categoryServices, IAuthorPostServices authorPostServices)
        {
            // Initialize the page size, you can set it to any value you want
            _pageSize = 6;
            _categoryServices = categoryServices;
            _logger = logger;
            _authorPostServices = authorPostServices;
            _authorServices = authorServices;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _authorPostServices.GetAuthorPostsAsQueryble().Take(_pageSize).ToListAsync();
            var HomeModel = new HomeViewModel
            {
                ListCategory = await _categoryServices.GetCategoriesAsListAsync(),
                ListAuthorPost = articles
            };

            ViewData["Title"] = "Home Page";

            return View(HomeModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }


        public async Task<IActionResult> GetArticles(int CategoryId)
        {
            var HomeModel = new HomeViewModel();

            if (CategoryId == -1)
            {
                HomeModel.ListAuthorPost = await _authorPostServices.GetAuthorPostsAsQueryble().ToListAsync();
                return PartialView("_PartialArticalData", HomeModel);

            }
            HomeModel.ListAuthorPost = await _authorPostServices.GetAuthorPostsAsQueryble().Where(W => W.CategoryId == CategoryId).ToListAsync();
            return PartialView("_PartialArticalData", HomeModel);
        }


        

        public IActionResult Article(int Id)

        {
            var article = _authorPostServices.GetAuthorPostsAsQuerybleWithInclude().
                Select(S => new ArticleViewModel()
                {
                    Id = S.Id,
                    PostImage = S.PostImage,
                    PostTitle = S.PostTitle,
                    PostDescription = S.PostDescription,
                    PostDate = S.PostDate,
                    AuthorName = CultureHelper.IsArabic() ? S.Author.user.NameAr : S.Author.user.NameEn
                }).FirstOrDefault(x => x.Id == Id);
            return View(article);
        }


        public async Task<IActionResult> GetAuthors()
        {
            try
            {

                var Authors = await _authorServices.GetAuthorsAsQerayableFullData().Select(S => new AuthorsViewModel()
                {
                    Bio = S.Bio,
                    ProfilePictureUrl = S.ProfilePictureUrl==null? "/img/portfolio/l..png" : S.ProfilePictureUrl,
                    FacebookUrl = S.FacebookUrl,
                    TwitterUrl = S.TwitterUrl,
                    Instagram = S.Instagram,
                    Name = CultureHelper.IsArabic() ? S.NameAr : S.NameEn
                }).ToListAsync();
                return View(Authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving authors.");
                return RedirectToAction(nameof(Index));
            }


        }
      
        public async Task<IActionResult> Search(string SearchItem)
        {
            try
            {

                var posts = await _authorPostServices.GetAuthorPostsAsQerayableSearch(SearchItem).ToListAsync();
                var HomeModel = new HomeViewModel()
                {
                    ListAuthorPost = posts
                };
                return PartialView("_PartialArticalData", HomeModel);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for posts.");
                ModelState.AddModelError("", $"An error occurred while searching for posts: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> SearchAuthor(string SearchItem)
        {
            try
            {

                var Authors =await _authorServices.GetAuthorsAsQerayableToSearch(SearchItem).Select(S => new AuthorsViewModel()
                {
                    Bio = S.Bio,
                    ProfilePictureUrl = S.ProfilePictureUrl == null ? "/img/portfolio/l..png" : S.ProfilePictureUrl,
                    FacebookUrl = S.FacebookUrl,
                    TwitterUrl = S.TwitterUrl,
                    Instagram = S.Instagram,
                    Name = CultureHelper.IsArabic() ? S.NameAr : S.NameEn
                }).ToListAsync();




                return PartialView("_PartialAuthorData", Authors);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for posts.");
                ModelState.AddModelError("", $"An error occurred while searching for posts: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }



        public IActionResult About()
        {
            return View();
        }


    }
}
