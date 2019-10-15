using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class ChangeUserPasswordModel {
        public string Email { get; set; }

        [DisplayName("Current password")]
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [DisplayName("New password")]
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm new password")]
        [Required(ErrorMessage = "Confirm new password is required")]
        public string ConfirmNewPassword { get; set; }
    }
}