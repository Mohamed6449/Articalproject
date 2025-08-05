using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.ViewModels.Author;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Controllers
{
    public class AllUsersController : Controller
    {
        private readonly ILogger<AllUsersController> _logger;
        private readonly IAuthorServices _authorServices;
        private readonly IMapper _mapper;
        public readonly UserManager<User> _userManager;
        public AllUsersController(ILogger<AllUsersController> logger,IAuthorServices authorServices ,IMapper mapper,UserManager<User> userManager )
        {
            _logger = logger;
            _authorServices = authorServices;
            _mapper = mapper;
           _userManager = userManager;
        }

        [HttpGet]
        public async Task< IActionResult >Index()
        {

            var Authors =await _authorServices.GetAuthorsAsQerayableFullData().ToListAsync();
            return View(Authors);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? Id)
        {

            if (Id == null)
            {
                return NotFound();
            }
            var Author = await _authorServices.GetAuthorByIdFullDataAsnyc((int)Id);
            if (Author == null) return NotFound();

            var result = _mapper.Map<UpdateAuthorViewModel>(Author);

            return View(result);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Update(UpdateAuthorViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByIdAsync(model.UserId);
        //        if (user == null)
        //        {
        //            _logger.LogError($"User with ID {model.UserId} not found.in update ahthor");
        //            return NotFound();
        //        }
        //       var author = await _authorServices.GetAuthorByIdWithOutInclude(model.AuthorId);
        //        if (author == null)
        //        {
        //            _logger.LogError($"Author with ID {model.AuthorId} not found.update author");
        //            return NotFound();
        //        }

        //        user.UserName = model.UserName;
        //        user.NameAr = model.NameAr;
        //        user.NameEn = model.NameEn;
        //         var resultUpdateUser=await _userManager.UpdateAsync(user);
        //        if (!resultUpdateUser.Succeeded)
        //        {
        //            _logger.LogError("Failed to update user information. in update Author");
        //            foreach (var error in resultUpdateUser.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //            return View(model);
        //        }

        //        author.UserId = model.UserId;
        //        author.Bio = model.Bio;
        //        author.FacebookUrl = model.FacebookUrl;
        //        author.TwitterUrl = model.TwitterUrl;
        //        author.Instagram = model.Instagram;
        //        //author.ProfilePictureUrl=




        //    //    var OldAuthor = await _authorServices.GetAuthorByIdWithOutInclude(model.Id);

        //    //    if (OldAuthor == null)
        //    //    {
        //    //        return NotFound();
        //    //    }

        //    //    var Author = _mapper.Map(model, OldAuthor);

        //    //    var result = await _authorServices.UpdateAuthorAsync(Author);

        //    //    if (result != "success")
        //    //        return BadRequest(result);
        //    //    return RedirectToAction(nameof(Index));
        //    //}
        //    return View(model);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Delete(int? Id)
        //{

        //    if (Id == null)
        //    {
        //        return NotFound();
        //    }
        //    var Author = await _authorServices.GetAuthorByIdWithOutInclude((int)Id);
        //    if (Author == null) return NotFound();

        //    var result = _mapper.Map<GetAuthorByIdViewModel>(Author);

        //    return View(result);


        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirm(int? Id)
        //{
        //    if (Id == null) return NotFound();


        //    var Author = await _authorServices.GetAuthorByIdWithOutInclude((int)Id);

        //    if (Author == null)
        //    {
        //        return NotFound();
        //    }
        //    var result = await _authorServices.DeleteAuthorAsync(Author);

        //    if (result != "success")
        //        return BadRequest(result);
        //    return RedirectToAction(nameof(Index));

        //}
    }
}
