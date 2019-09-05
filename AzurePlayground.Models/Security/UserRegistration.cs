using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class UserRegistration {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "A valid email address is required")]
        public string Email { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "A password is required")]
        public string Password { get; set; }
    }
}
