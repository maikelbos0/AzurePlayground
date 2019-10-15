using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class DeactivateUserModel {

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}