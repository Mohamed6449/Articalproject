using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="RequireEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Please enter your password")]
        public string Password { get; set; }

        public bool RemberMe { get; set; } = false;

        public string? ReturnUrl { get; set; }


    }
}
