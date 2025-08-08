using Articalproject.Models;
using Articalproject.Models.Identity;
using Articalproject.Services.InterFaces;
using Articalproject.UnitOfWorks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Services.Implementations
{

    public class AuthorPostServices: IAuthorPostServices
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;


        #endregion

        #region Constructor

        public AuthorPostServices(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }


        #endregion

        #region Hundle Function

        public async Task<List<AuthorPost>> GetAuthorPostsAsListAsync()
        {
            return await _unitOfWork.Repository<AuthorPost>().GetAsListAsync();
        }
        public IQueryable<AuthorPost> GetAuthorPostsAsQueryble()
        {
            return _unitOfWork.Repository<AuthorPost>().GetAsQueryble();
        }

        public async Task<AuthorPost?> GetAuthorPostByIdWithOutIncludeAsync(int AuthorPostId)
        {
            return await _unitOfWork.Repository<AuthorPost>().GetProductbyIdAsync(AuthorPostId);
        }
        public async Task<AuthorPost?> GetAuthorPostByIdWithIncludeAsync(int AuthorPostId)
        {
            return await GetAuthorPostsAsQueryble()
                .Where(W => W.Id == AuthorPostId).
                Include(I => I.Author).ThenInclude(T=>T.user).Include(I => I.Category).FirstOrDefaultAsync();
        }


        public async Task<string> UpdateAuthorPostAsync(AuthorPost AuthorPost)
        {

            try
            {

                await _unitOfWork.Repository<AuthorPost>().UpdateAsync(AuthorPost);
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }

        }
        public async Task<string> AddAuthorPostAsync(AuthorPost AuthorPost)
        {
            try
            {

                await _unitOfWork.Repository<AuthorPost>().AddAsync(AuthorPost);
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }



        }

        public async Task<string> DeleteAuthorPostAsync(AuthorPost AuthorPost)
        {
            try
            {

                await _unitOfWork.Repository<AuthorPost>().DeleteAsync(AuthorPost);
                return "success";


            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }


        }

        public IQueryable <AuthorPost> GetAuthorPostsAsQerayableSearch(string? search)
        {
            var AuthorPost = _unitOfWork.Repository<AuthorPost>().GetAsQueryble()
                .Include(I => I.Author).ThenInclude(T=>T.user).Include(I => I.Category);

            if (search != null)
            {

                var NewAuthorPost = AuthorPost.Where(W => W.Author.user.NameAr.Contains(search) ||
                                        W.Author.user.NameEn.Contains(search) ||
                                        W.AuthorId.ToString().Contains(search) ||
                                        W.PostTitle.Contains(search) ||
                                        W.Author.user.UserName.Contains(search) ||
                                        W.PostDescription.Contains(search) ||
                                        W.Author.Id.ToString().Contains(search) ||
                                        W.Category.NameAr.Contains(search) ||
                                        W.Category.NameEn.Contains(search) ||
                                        W.PostDate.ToString().Contains(search) ||
                                        W.CategoryId.ToString().Contains(search));

                return NewAuthorPost;
            }
            return AuthorPost;

        }

        #endregion
    }

}

