using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class ResetUserPasswordModel {
        [DisplayName("New password")]
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm new password")]
        [Required(ErrorMessage = "Confirm new password is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirm new password must match")]
        public string ConfirmNewPassword { get; set; }
    }
}