using Articalproject.Models;

namespace Articalproject.Services.InterFaces
{
    public interface IAuthorPostServices
    {
        public Task<AuthorPost?> GetAuthorPostByIdWithOutIncludeAsync(int AuthorPostId);
        public IQueryable<AuthorPost> GetAuthorPostsAsQueryble();

        public  Task<List<AuthorPost>> GetAuthorPostsAsListAsync();

        public  Task<AuthorPost?> GetAuthorPostByIdWithIncludeAsync(int AuthorPostId);

        public Task<string> UpdateAuthorPostAsync(AuthorPost AuthorPost);

        public Task<string> AddAuthorPostAsync(AuthorPost AuthorPost);

        public Task<string> DeleteAuthorPostAsync(AuthorPost AuthorPost);

        public IQueryable<AuthorPost> GetAuthorPostsAsQerayableSearch(string? search);

    }
}