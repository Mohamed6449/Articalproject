using Articalproject.Models;

namespace Articalproject.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<Category> ListCategory { get; set; } = new List<Category>();

        public List<AuthorPost> ListAuthorPost { get; set; } = new List<AuthorPost>();

    }
}
