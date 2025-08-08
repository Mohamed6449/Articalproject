using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Post
{
    public class UpdatePostViewModel
    {
        public int Id { get; set; }
        public IFormFile PostImageUrl { get; set; }

        [Required(ErrorMessage = "PostTitleIsRequired")]

        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionIsRequired")]

        public string PostDescription { get; set; }
        public DateTime PostDate { get; set; } = DateTime.Now;

        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int CategoryId { get; set; }

    }
}
