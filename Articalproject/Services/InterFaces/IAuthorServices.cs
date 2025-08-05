using Articalproject.Models;
using Articalproject.ViewModels.Author;

namespace Articalproject.Services.InterFaces
{
    public interface IAuthorServices
    {
        public Task<List<Author>> GetAuthors();


        //public Task<Author?> GetAuthorById(int AuthorId);
        public Task<string> AddAuthorAsync(Author Author);
        public Task<string> UpdateAuthorAsync(Author Author);
        public Task<string> DeleteAuthorAsync(Author AuthorId);
        public Task<Author?> GetAuthorByIdWithOutInclude(int AuthorId);
        public IQueryable<GetAuthorsViewModel> GetAuthorsAsQerayableFullData();
        public IQueryable<GetAuthorsViewModel> GetAuthorsAsQerayable(string? search);
    }
}

