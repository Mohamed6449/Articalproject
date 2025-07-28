using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Emailisrequired")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Passwordisrequired")]
        public string Password { get; set; }

        public bool RemberMe { get; set; } = false;

        public string? ReturnUrl { get; set; }


    }
}
