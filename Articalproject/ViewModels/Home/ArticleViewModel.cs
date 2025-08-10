namespace Articalproject.ViewModels.Home
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string PostImage { get; set; }

        public string PostTitle { get; set; }

        public string PostDescription { get; set; }

        public DateTime PostDate { get; set; }
        public string AuthorName { get; set; }
    }
}
