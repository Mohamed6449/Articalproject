﻿using System.ComponentModel.DataAnnotations; 
namespace Articalproject.ViewModels.Identity
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Passwordisrequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPasswordisrequired")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The password and Confirmation password not match")]
        public string ConfirmPassword { get; set; }

    }

}


