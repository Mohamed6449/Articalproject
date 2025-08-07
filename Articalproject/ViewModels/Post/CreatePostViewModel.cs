using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Post
{
    public class CreatePostViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Img", ResourceType = typeof(Resources.SharedResources))]
        public IFormFile PostImage { get; set; }

        [Required(ErrorMessage = "PostTitleIsRequired")]

        [Display(Name = "PostTitle", ResourceType = typeof(Resources.SharedResources))]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionIsRequired")]

        [Display(Name = "PostDescription", ResourceType = typeof(Resources.SharedResources))]
        public string PostDescription { get; set; }
        public DateTime PostDate { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; }
        [Required]
        public int CategoryId { get; set; }


    }
}
