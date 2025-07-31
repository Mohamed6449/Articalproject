using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity
{
    public class ResetPasswordRequestViewModel
    {
        [Required(ErrorMessage = "Emailisrequired")]
        [EmailAddress]
        [Remote("IsEmailNotAvailable", "Account", ErrorMessage = "EmailNotExist")]
        public string Email { get; set; }
    }
}
