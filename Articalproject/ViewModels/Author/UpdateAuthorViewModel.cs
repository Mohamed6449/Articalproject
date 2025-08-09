using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Articalproject.ViewModels.Author
{
    public class UpdateAuthorViewModel
    {
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage = "UserNameisrequired")]
        [Remote("IsUserNameAvailable", "Account", ErrorMessage = "UserNameIsExist")]
        public string UserName { get; set; }
   
        [Required(ErrorMessage = "NameArIsRequired")]
        public string NameAr { get; set; }


        [Required(ErrorMessage = "NameEnIsRequired")]
        public string NameEn { get; set; }


        [Required(ErrorMessage = "Bio")]
        public string Bio { get; set; }

        public IFormFile? File { get; set; }

        [Display(Name = "FaceBook")]
        public string? FacebookUrl { get; set; }
        [Display(Name = "Twitter")]
        public string? TwitterUrl { get; set; }
        [Display(Name = "Instagram")]
        public string? Instagram { get; set; }


    }
}
