﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class UserRegistration {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "This email address is invalid")]
        public string Email { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [DisplayName("Confirm password")]
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }
    }
}
