using Articalproject.Helper;
using Articalproject.Resources;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Author
{
    public class ff: LocalizableEntity
    {

        [Required]
        [Display(Name = "Id", ResourceType = typeof(SharedResources))]
        public int AuthorId { get; set; }
        public string UserId { get; set; }

        [Required]
        [Display(Name = "UserName", ResourceType = typeof(SharedResources))]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Name", ResourceType = typeof(SharedResources))]
        public string FullName { get; set; }

        [MinLength(200, ErrorMessage = "اقصي عدد احرف مسموح به 200")]
        [Display(Name = "Bio", ResourceType = typeof(SharedResources))]
        public string Bio { get; set; }

        [Display(Name = "Img", ResourceType = typeof(SharedResources))]
        public string ProfilePictureUrl { get; set; }

        [Display(Name = "FaceBook")]
        public string? FacebookUrl { get; set; }
        [Display(Name = "Twitter")]
        public string? TwitterUrl { get; set; }
        [Display(Name = "Instagram")]
        public string? Instagram { get; set; }

    }
}
