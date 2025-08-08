using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Post
{
    public class GetPostsViewModel
    {
        [Required]
        public int Id { get; set; }
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string PostImage { get; set; }

        [Required(ErrorMessage = "PostTitleIsRequired")]

        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionIsRequired")]

        public string PostDescription { get; set; }

        public DateTime PostDate { get; set; }

        public string CategoryName { get; set; }





    }
}
