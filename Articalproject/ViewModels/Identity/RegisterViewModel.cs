using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "NameArIsRequired")]
        public string NameAr { get; set; }
        [Required(ErrorMessage = "NameEnIsRequired")]
        public string NameEn { get; set; }

        public string? Address { get; set; }

        [Required]
        [Remote("IsUserNameAvailable", "Account", ErrorMessage = "UserNameIsExist")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsEmailAvailable", "Account", ErrorMessage = "EmailIsExist")]

        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="The password and Confirmation password not match")]
        public string ConfirmPassword { get; set;}
    
    }
}
