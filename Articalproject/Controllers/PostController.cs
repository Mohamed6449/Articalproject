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
        public async Task< IActionResult> Index(int? Id, bool next = true)
        {
             var UserId = _userManager.GetUserId(User);
            ViewBag.Next = true;
            List<GetPostsViewModel> posts = new List<GetPostsViewModel>();
            if (_authorizationService.AuthorizeAsync(User, "AdminPolicy").Result.Succeeded)
            {
                 posts = await _authorPostServices.GetAuthorPostsAsQueryble().Include(i => i.user).Include(i => i.Category)
                            .Select(s => new GetPostsViewModel
                            {
                                Id = s.Id,
                                UserName = s.user.UserName,
                                NameAr = s.user.NameAr,
                                NameEn = s.user.NameEn,
                                PostImage = s.PostImage,
                                PostTitle = s.PostTitle,
                                PostDescription = s.PostDescription,
                                PostDate = s.PostDate,
                                CategoryNameAr = s.Category.NameAr,
                                CategoryNameEn = s.Category.NameEn
                            }).ToListAsync();

            }
            else
            {
              posts = await _authorPostServices.GetAuthorPostsAsQueryble().Where(W=>W.user.Id==UserId).Include(i => i.user).Include(i => i.Category)
           .Select(s => new GetPostsViewModel
           {
               Id = s.Id,
               UserName = s.user.UserName,
               NameAr = s.user.NameAr,
               NameEn = s.user.NameEn,
               PostImage = s.PostImage,
               PostTitle = s.PostTitle,
               PostDescription = s.PostDescription,
               PostDate = s.PostDate,
               CategoryNameAr = s.Category.NameAr,
               CategoryNameEn = s.Category.NameEn
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
        public async Task< IActionResult> Create()
        {
            ViewBag.Category = new SelectList(await _categoryServices.GetCategoriesAsListAsync(),"Id",
                CultureHelper.IsArabic()? "NameAr": "NameEn");
            var userId = _userManager.GetUserId(User);
            var model = new CreatePostViewModel
            {
                UserId = userId,
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Create(CreatePostViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var authorPost = _mapper.Map<AuthorPost>(model);

                    authorPost.PostImage= await _fileServiece.Upload(model.PostImage, "/img/");
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
            ViewBag.Category = new SelectList(await _categoryServices.GetCategoriesAsListAsync(),"Id",
                CultureHelper.IsArabic()? "NameAr": "NameEn");
            return View(model);
        }
        public IActionResult Details(int Id)
        {
            var post = _authorPostServices.GetAuthorPostsAsQueryble().Include(i => i.user).Include(i => i.Category)
                .Select(s => new GetPostsViewModel
                {
                    Id = s.Id,
                    UserName = s.user.UserName,
                    NameAr = s.user.NameAr,
                    NameEn = s.user.NameEn,
                    PostImage = s.PostImage,
                    PostTitle = s.PostTitle,
                    PostDescription = s.PostDescription,
                    PostDate = s.PostDate,
                    CategoryNameAr = s.Category.NameAr,
                    CategoryNameEn = s.Category.NameEn
                }).FirstOrDefault(f => f.Id == Id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);

        }

























        }
}
