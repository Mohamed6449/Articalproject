using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Post
{
    public class GetPostsViewModel
    {
        [Required]
        public int Id { get; set; }
        [Display(Name = "UserName", ResourceType = typeof(Resources.SharedResources))]
        public string UserName { get; set; }

        [Display(Name = "NameAr", ResourceType = typeof(Resources.SharedResources))] 
        public string NameAr { get; set; }

        [Display(Name = "NameEn", ResourceType = typeof(Resources.SharedResources))]
        public string NameEn { get; set; }

        [Display(Name = "Img", ResourceType = typeof(Resources.SharedResources))]
        public string PostImage { get; set; }

        [Required(ErrorMessage = "PostTitleIsRequired")]

        [Display(Name = "PostTitle", ResourceType = typeof(Resources.SharedResources))]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "PostDescriptionIsRequired")]

        [Display(Name = "PostDescription", ResourceType = typeof(Resources.SharedResources))]
        public string PostDescription { get; set; }

        [Display(Name = "PostDate", ResourceType = typeof(Resources.SharedResources))]
        public DateTime PostDate { get; set; }

        [Display(Name = "CategoryNameAr", ResourceType = typeof(Resources.SharedResources))]
        public string CategoryNameAr { get; set; }

        [Display(Name = "CategoryNameEn", ResourceType = typeof(Resources.SharedResources))]
        public string CategoryNameEn { get; set; }





    }
}
