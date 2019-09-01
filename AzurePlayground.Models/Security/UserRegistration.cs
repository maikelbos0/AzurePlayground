using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class UserRegistration {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
