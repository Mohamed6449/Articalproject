using Articalproject.Helper;
using Articalproject.Models;
using Articalproject.Models.Identity;
using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Articalproject.ViewModels.Categories;
using Articalproject.ViewModels.Post;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class PostController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICategoryServices _categoryServices;
        private readonly IFileServiece _fileServiece;
        private readonly ILogger<AllUsersController> _logger;
        private readonly IAuthorPostServices _authorPostServices;
        public readonly UserManager<User> _userManager;
        public readonly IUnitOfWork _unitOfWork;
        private readonly int _PageItem;
        private readonly IAuthorizationService _authorizationService;


        public PostController(IUnitOfWork unitOfWork,
            IFileServiece fileServiece, ILogger<AllUsersController> logger,
            IAuthorPostServices authorPostServices, IMapper mapper,
            UserManager<User> userManager, IAuthorizationService authorizationService, ICategoryServices categoryServices
            )
        {
            _authorizationService = authorizationService;
            _categoryServices = categoryServices;
            _PageItem = 10;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorPostServices = authorPostServices;
            _userManager = userManager;
            _fileServiece = fileServiece;

            _mapper = mapper;

        }
        public async Task<IActionResult> Index(int? Id, bool next = true)
        {
            var isAdmin = _authorizationService.AuthorizeAsync(User, "AdminPolicy").Result.Succeeded;
            var UserId = _userManager.GetUserId(User);
            var authorId = await _unitOfWork.Repository<Author>().GetAsQueryble().Where(W => W.UserId == UserId).Select(s => s.Id).FirstOrDefaultAsync();

            if (authorId == 0 && !isAdmin)
            {
                _logger.LogWarning("Author not found for user ID: {UserId}", UserId);
                return View("~/Views/Shared/NotAllowCreatePost.cshtml");
            }
            ViewBag.Next = true;
            List<GetPostsViewModel> posts = new List<GetPostsViewModel>();

            if (isAdmin)
            {
                posts = await _authorPostServices.GetAuthorPostsAsQueryble().Include(i => i.Author).ThenInclude(T => T.user).Include(i => i.Category)
                           .Select(s => new GetPostsViewModel
                           {
                               Id = s.Id,
                               UserName = s.Author.user.UserName,
                               FullName = CultureHelper.IsArabic() ? s.Author.user.NameAr : s.Author.user.NameEn,
                               PostImage = s.PostImage,
                               PostTitle = s.PostTitle,
                               PostDescription = s.PostDescription,
                               PostDate = s.PostDate,
                               CategoryName = CultureHelper.IsArabic() ? s.Category.NameAr : s.Category.NameEn
                           }).ToListAsync();

            }
            else
            {
                posts = await _authorPostServices.GetAuthorPostsAsQueryble().Include(i => i.Author).ThenInclude(T => T.user).Where(W => W.Author.user.Id == UserId).Include(i => i.Category)
             .Select(s => new GetPostsViewModel
             {
                 Id = s.Id,
                 UserName = s.Author.user.UserName,
                 FullName = CultureHelper.IsArabic() ? s.Author.user.NameAr : s.Author.user.NameEn,
                 PostImage = s.PostImage,
                 PostTitle = s.PostTitle,
                 PostDescription = s.PostDescription,
                 PostDate = s.PostDate,
                 CategoryName = CultureHelper.IsArabic() ? s.Category.NameAr : s.Category.NameEn
             }).ToListAsync();
            }
            if (Id == null || Id == 0)
            {

                if (posts.Count() <= _PageItem)
                    ViewBag.Next = false;
                var newCategories = posts.Take(_PageItem);
                return View(newCategories);

            }

            if (next)
            {
                var CategoriesId = posts.Where(W => W.Id > Id).Take(_PageItem);
                ViewBag.Previous = true;
                var newId = CategoriesId.LastOrDefault().Id;
                if (posts.Where(W => W.Id > newId).Count() > 1)
                    return View(CategoriesId);
                ViewBag.Next = false;
                return View(CategoriesId);
            }
            else
            {
                var CategoriesId = posts.Where(W => W.Id < Id).OrderDescending().Take(_PageItem).OrderBy(O => O.Id);
                var newId = CategoriesId.FirstOrDefault().Id;
                if (posts.Where(W => W.Id < newId).Count() > 1)
                    ViewBag.Previous = true;

                return View(CategoriesId);
            }

        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Category = new SelectList(await _categoryServices.GetCategoriesAsListAsync(), "Id",
                CultureHelper.IsArabic() ? "NameAr" : "NameEn");
            var userId = _userManager.GetUserId(User);
            var authorId = await _unitOfWork.Repository<Author>().GetAsQueryble().Where(W => W.UserId == userId).Select(s => s.Id).FirstOrDefaultAsync();
            if (authorId == 0)
            {
                return BadRequest("انت لست ناشر ");
            }
            var model = new CreatePostViewModel
            {
                AuthorId = authorId,
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var authorPost = _mapper.Map<AuthorPost>(model);

                    authorPost.PostImage = await _fileServiece.Upload(model.PostImage, "/img/");
                    var result = await _authorPostServices.AddAuthorPostAsync(authorPost);
                    if (result != "success")
                    {
                        _logger.LogError("Error creating post: {Error}", result);
                        return BadRequest(result);
                    }
                    _logger.LogInformation("Post created successfully with ID: {Id}", authorPost.Id);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the post.");
                    ModelState.AddModelError("", $"An error occurred while creating the post: {ex.Message}");
                }
            }
            ViewBag.Category = new SelectList(await _categoryServices.GetCategoriesAsListAsync(), "Id",
                CultureHelper.IsArabic() ? "NameAr" : "NameEn");
            return View(model);
        }






        public async Task<IActionResult> Update(int Id)
        {
            try
            {

                var post = await _authorPostServices.GetAuthorPostByIdWithOutIncludeAsync(Id);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {Id} not found for update.", Id);
                    return NotFound();
                }
                var model = _mapper.Map<UpdatePostViewModel>(post);

                ViewBag.Category = new SelectList(await _categoryServices.GetCategoriesAsListAsync(), "Id",
                CultureHelper.IsArabic() ? "NameAr" : "NameEn");
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the post for update.");
                ModelState.AddModelError("", $"An error occurred while retrieving the post: {ex.Message}");
                return View();
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdatePostViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var post = await _authorPostServices.GetAuthorPostByIdWithOutIncludeAsync(model.Id);
                    if (post == null)
                    {
                        _logger.LogWarning("Post with ID {Id} not found for update.", model.Id);
                        return NotFound();
                    }
                    if (model.PostImageUrl != null)
                    {
                        _fileServiece.DeleteSource(post.PostImage);
                        post.PostImage = await _fileServiece.Upload(model.PostImageUrl, "/img/");
                    }
                    post = _mapper.Map(model, post);

                    var result = await _authorPostServices.UpdateAuthorPostAsync(post);

                    if (result != "success")
                    {
                        _logger.LogError("Error Update post: {Error}", result);
                        return BadRequest(result);
                    }
                    _logger.LogInformation("Post Update successfully with ID: {Id}", post.Id);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error Update while creating the post.");
                    ModelState.AddModelError("", $"An error Update while creating the post: {ex.Message}");
                }
            }
            ViewBag.Category = new SelectList(await _categoryServices.GetCategoriesAsListAsync(), "Id",
                CultureHelper.IsArabic() ? "NameAr" : "NameEn");
            return View(model);
        }






        public IActionResult Details(int Id)
        {
            var post = _authorPostServices.GetAuthorPostsAsQueryble().Include(i => i.Author).ThenInclude(T => T.user).Include(i => i.Category)
                .Select(s => new GetPostsViewModel
                {
                    Id = s.Id,
                    UserName = s.Author.user.UserName,
                    FullName = CultureHelper.IsArabic() ? s.Author.user.NameAr : s.Author.user.NameEn,
                    PostImage = s.PostImage,
                    PostTitle = s.PostTitle,
                    PostDescription = s.PostDescription,
                    PostDate = s.PostDate,
                    CategoryName = CultureHelper.IsArabic() ? s.Category.NameAr : s.Category.NameEn
                }).FirstOrDefault(f => f.Id == Id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);

        }

        public async Task<IActionResult> Delete(int Id)
        {
            try
            {

                var post = _authorPostServices.GetAuthorPostsAsQueryble().Include(i => i.Author).ThenInclude(T => T.user).Include(i => i.Category)
                    .Select(s => new GetPostsViewModel
                    {
                        Id = s.Id,
                        UserName = s.Author.user.UserName,
                        FullName = CultureHelper.IsArabic() ? s.Author.user.NameAr : s.Author.user.NameEn,
                        PostImage = s.PostImage,
                        PostTitle = s.PostTitle,
                        PostDescription = s.PostDescription,
                        PostDate = s.PostDate,
                        CategoryName = CultureHelper.IsArabic() ? s.Category.NameAr : s.Category.NameEn
                    }).FirstOrDefault(f => f.Id == Id);
                if (post == null)
                {
                    return NotFound();
                }
                return View(post);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the post for deletion.");
                ModelState.AddModelError("", $"An error occurred while retrieving the post: {ex.Message}");
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            try
            {
                var post = await _authorPostServices.GetAuthorPostByIdWithOutIncludeAsync(Id);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {Id} not found for deletion.", Id);
                    return NotFound();
                }
                _fileServiece.DeleteSource(post.PostImage);
                var result = await _authorPostServices.DeleteAuthorPostAsync(post);
                if (result != "success")
                {
                    _logger.LogError("Error deleting post: {Error}", result);
                    return BadRequest(result);
                }
                _logger.LogInformation("Post deleted successfully with ID: {Id}", Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the post.");
                ModelState.AddModelError("", $"An error occurred while deleting the post: {ex.Message}");
                return View();
            }
        }





        public async Task<IActionResult> Search(string SearchItem)
        {
            try
            {

                var Posts = await _authorPostServices.GetAuthorPostsAsQerayableSearch(SearchItem)
                    .Select(s => new GetPostsViewModel
                    {
                        Id = s.Id,
                        UserName = s.Author.user.UserName,
                        FullName = CultureHelper.IsArabic() ? s.Author.user.NameAr : s.Author.user.NameEn,
                        PostImage = s.PostImage,
                        PostTitle = s.PostTitle,
                        PostDescription = s.PostDescription,
                        PostDate = s.PostDate,
                        CategoryName = CultureHelper.IsArabic() ? s.Category.NameAr : s.Category.NameEn
                    }).ToListAsync();

                return PartialView("_PartialPostData", Posts);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for posts.");
                ModelState.AddModelError("", $"An error occurred while searching for posts: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
