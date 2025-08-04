//using Articalproject.Services.InterFaces;
//using Articalproject.ViewModels.Authors;
//using Microsoft.AspNetCore.Mvc;

//namespace Articalproject.Controllers
//{
//    public class AllUsers : Controller
//    {
//        private readonly IAuthorServices _authorServices;
//        public AllUsers(IAuthorServices authorServices) {
//            _authorServices = authorServices;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Index()
//        {
//            var Authors = await _authorServices.GetAuthors();
//            var result = _mapper.Map<List<GetAuthorsListViewModel>>(Authors);
//            return View(result);
//        }
//        [HttpGet]
//        public async Task<IActionResult> Update(int? Id)
//        {

//            if (Id == null)
//            {
//                return NotFound();
//            }
//            var Author = await _authorServices.GetAuthorByIdWithOutInclude((int)Id);
//            if (Author == null) return NotFound();

//            var result = _mapper.Map<UpdateAuthorViewModel>(Author);

//            return View(result);


//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Update(UpdateAuthorViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var OldAuthor = await _authorServices.GetAuthorByIdWithOutInclude(model.Id);

//                if (OldAuthor == null)
//                {
//                    return NotFound();
//                }

//                var Author = _mapper.Map(model, OldAuthor);

//                var result = await _authorServices.UpdateAuthorAsync(Author);

//                if (result != "success")
//                    return BadRequest(result);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(model);
//        }

//        [HttpGet]
//        public async Task<IActionResult> Delete(int? Id)
//        {

//            if (Id == null)
//            {
//                return NotFound();
//            }
//            var Author = await _authorServices.GetAuthorByIdWithOutInclude((int)Id);
//            if (Author == null) return NotFound();

//            var result = _mapper.Map<GetAuthorByIdViewModel>(Author);

//            return View(result);


//        }

//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirm(int? Id)
//        {
//            if (Id == null) return NotFound();


//            var Author = await _authorServices.GetAuthorByIdWithOutInclude((int)Id);

//            if (Author == null)
//            {
//                return NotFound();
//            }
//            var result = await _authorServices.DeleteAuthorAsync(Author);

//            if (result != "success")
//                return BadRequest(result);
//            return RedirectToAction(nameof(Index));

//        }
//    }
//}
