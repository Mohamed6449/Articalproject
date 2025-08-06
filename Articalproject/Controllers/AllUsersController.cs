using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Articalproject.ViewModels.Author;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Controllers
{
    public class AllUsersController : Controller
    {
        private readonly IFileServiece _fileServiece;
        private readonly ILogger<AllUsersController> _logger;
        private readonly IAuthorServices _authorServices;
        private readonly IMapper _mapper;
        public readonly UserManager<User> _userManager;
        public readonly IUnitOfWork _unitOfWork;

        public AllUsersController(IUnitOfWork unitOfWork,IFileServiece fileServiece,ILogger<AllUsersController> logger,IAuthorServices authorServices ,IMapper mapper,UserManager<User> userManager )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorServices = authorServices;
            _mapper = mapper;
           _userManager = userManager;
            _fileServiece=fileServiece;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                    var trans=await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var user = await _userManager.FindByIdAsync(model.UserId);
                    if (user == null)
                    {
                        _logger.LogError($"User with ID {model.UserId} not found.in update ahthor");
                        return NotFound();
                    }
                    var author = await _authorServices.GetAuthorByIdWithOutInclude(model.AuthorId);
                    if (author == null)
                    {
                        _logger.LogError($"Author with ID {model.AuthorId} not found.update author");
                        return NotFound();
                    }
                    user.UserName = model.UserName;
                    user.NameAr = model.NameAr;
                    user.NameEn = model.NameEn;

                    var resultUpdateUser = await _userManager.UpdateAsync(user);
                    if (!resultUpdateUser.Succeeded)
                    {
                        _logger.LogError("Failed to update user information. in update Author");
                        foreach (var error in resultUpdateUser.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    author.UserId = model.UserId;
                    author.Bio = model.Bio;
                    author.FacebookUrl = model.FacebookUrl;
                    author.TwitterUrl = model.TwitterUrl;
                    author.Instagram = model.Instagram;
                    author.ProfilePictureUrl = await _fileServiece.Upload(model.File, "/img/");

                    var resultUpdateAuthor = await _authorServices.UpdateAuthorAsync(author);

                    if (resultUpdateAuthor != "success")
                    {
                        _logger.LogError("Failed to update Author information. in update Author");
                        return View(model);
                    }
                    await trans.CommitAsync();
                    _logger.LogInformation("Author updated successfully. in update Author");
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    _logger.LogError(ex, "An error occurred while updating the author.");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the author. Please try again later.");
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {

            if (Id == null)
            {
                return NotFound();
            }
            var Author = await _authorServices.GetAuthorByIdFullDataAsnyc((int)Id);
            if (Author == null) return NotFound();
            
            return View(Author);


        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? Id)
        {
            if (Id == null) return NotFound();


            var Author = await _authorServices.GetAuthorByIdWithOutInclude((int)Id);

            if (Author == null)
            {
                return NotFound();
            }
            var result = await _authorServices.DeleteAuthorAsync(Author);

            if (result != "success")
                return BadRequest(result);
          var deleteImg= _fileServiece.DeleteSource(Author.ProfilePictureUrl);
            if(!deleteImg)
            {
                _logger.LogWarning($"Failed to delete profile picture for author with ID {Id}.");
            }
            else
            {
                _logger.LogInformation($"Profile picture for author with ID {Id} deleted successfully.");
            }
            _logger.LogInformation($"Author with ID {Id} deleted successfully.");
            return RedirectToAction(nameof(Index));

        }
    }
}
