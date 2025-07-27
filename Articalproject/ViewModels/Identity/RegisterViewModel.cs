using System.ComponentModel.DataAnnotations;

namespace Articalproject.ViewModels.Identity
{
    public class RegisterViewModel
    {
        [Required]
        public string NameAr { get; set; }
        [Required]
        public string NameEn { get; set; }

        public string? Address { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="The password and Confirmation password not match")]
        public string ConfirmPassword { get; set;}
    
    }
}
