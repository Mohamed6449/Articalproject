using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Articalproject.ViewModels.Author
{
    public class UpdateAuthorViewModel
    {
        [Display(Name = "معرف الناشر")]
        [Required(ErrorMessage = "الرجاء إدخال معرف الناشر")]
        public int AuthorId { get; set; }
        [Display(Name = "معرف المستخدم")]
        [Required(ErrorMessage = "الرجاء إدخال معرف المستخدم")]
        public string UserId { get; set; }

        [Display(Name = "اسم المستخدم")]
        [Required(ErrorMessage = "الرجاء إدخال اسم المستخدم")]
        public string UserName { get; set; }
        [Display(Name = "الاسم بالعربي")]
        [Required(ErrorMessage = "الرجاء إدخال الاسم بالعربي")]
        public string NameAr { get; set; }

        [Display(Name = "الاسم بالانجلش")]
        [Required(ErrorMessage = "الرجاء إدخال الاسم بالانجلش")]
        public string NameEn { get; set; }

        [Display(Name = "السيرة الذاتية")]
        [Required(ErrorMessage = "الرجاء إدخال السيرة الذاتية")]
        public string Bio { get; set; }

        [Display(Name = "تحميل الصورة")]
        public IFormFile? File { get; set; }

        [Display(Name = "FaceBook")]
        public string? FacebookUrl { get; set; }
        [Display(Name = "Twitter")]
        public string? TwitterUrl { get; set; }
        [Display(Name = "Instagram")]
        public string? Instagram { get; set; }


    }
}
