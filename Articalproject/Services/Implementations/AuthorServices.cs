using Articalproject.Helper;
using Articalproject.Models;
using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using Articalproject.ViewModels.Author;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Services.Implementations
{
    public class AuthorServices : IAuthorServices
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        public AuthorServices(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }


        #endregion

        #region Hundle Function

        public async Task<List<Author>> GetAuthors()
        {
            return await _unitOfWork.Repository<Author>().GetAsListAsync();
        }

        //public async Task<Author?> GetAuthorById(int AuthorId)
        //{
        //	return await _unitOfWork.Repository<Author>().GetAsQueryble().Include(I => I.product).AsNoTracking().FirstOrDefaultAsync(F=>F.Id==AuthorId);
        //}

        public async Task<Author?> GetAuthorByIdWithOutInclude(int AuthorId)
        {
            try
            {
                return await _unitOfWork.Repository<Author>().GetProductbyIdAsync(AuthorId);

            }
            catch (Exception ex)
            {

                return null;
            }
            }

        public async Task<string> UpdateAuthorAsync(Author Author)
        {

            try
            {

                await _unitOfWork.Repository<Author>().UpdateAsync(Author);
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }

        }
        public async Task<string> AddAuthorAsync(Author Author)
        {
            try
            {

                await _unitOfWork.Repository<Author>().AddAsync(Author);
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }



        }

        public async Task<string> DeleteAuthorAsync(Author Author)
        {
            try
            {

                await _unitOfWork.Repository<Author>().DeleteAsync(Author);
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }


        }


        public IQueryable<GetAuthorsViewModel> GetAuthorsAsQerayableFullData ()
        {
            var Author = _unitOfWork.Repository<Author>().GetAsQueryble().Join(_unitOfWork.Repository<User>().GetAsQueryble()
               , Au => Au.UserId, Us => Us.Id, (Aut, user) =>
               new GetAuthorsViewModel
               {
                   FacebookUrl = Aut.FacebookUrl,
                   Instagram = Aut.Instagram,
                   TwitterUrl = Aut.TwitterUrl,
                   Bio = Aut.Bio,
                   UserId = Aut.UserId,
                   FullName = CultureHelper.IsArabic() ? user.NameAr : user.NameEn,
                   UserName = user.UserName,
                   AuthorId = Aut.Id,
                   ProfilePictureUrl = Aut.ProfilePictureUrl
               });
          return  Author;

        }
        public IQueryable<GetAuthorsViewModel> GetAuthorsAsQerayable(string? search)
        {
           var Author = GetAuthorsAsQerayableFullData();

            if (search != null)
            {
                Author = Author.Where(W => W.FullName.Contains(search) ||
                                        W.UserId.ToString().Contains(search) ||
                                        W.Bio.Contains(search) ||
                                        W.UserName.Contains(search) ||
                                        W.FacebookUrl.Contains(search) ||
                                        W.TwitterUrl.Contains(search) ||
                                        W.Instagram.Contains(search));
            }
            return Author;
        }
        #endregion





    }
}
