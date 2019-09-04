using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class UserRegistration {
        [Display(Description = "Email address")]
        [Required(ErrorMessage = "A valid email address is required")]
        public string Email { get; set; }

        [Display(Description = "Password")]
        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; }
    }
}
