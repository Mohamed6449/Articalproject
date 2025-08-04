using Articalproject.Models;

using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
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

        public IQueryable<Author> GetAuthorsAsQerayable(string? search)
        {
            throw new NotImplementedException();
        }

        //public IQueryable<Author> GetAuthorsAsQerayable(string? search)
        //{
        //    var Author = _unitOfWork.Repository<Author>().GetAsQueryble();

        //    if (search != null)
        //    {
        //        Author = Author.Where(W => W.FullName.Contains(search)||
        //                                W.Id.ToString().Contains(search)||
        //                                W.UserName.ToString().Contains(search)||
        //                                W.FacebookUrl.ToString().Contains(search)||
        //                                W.TwitterUrl.ToString().Contains(search)||
        //                                W.Instagram.ToString().Contains(search) );
        //    }
        //    return Author;


        //}
        #endregion





    }
}
