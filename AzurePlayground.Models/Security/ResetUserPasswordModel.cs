using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public class ResetUserPasswordModel {
        // This property is used only for showing validation
        // TODO figure out a cleaner way of having error messages without corresponding properties
        // TODO probably use that way for "invalid username or password"
        public string PasswordResetToken { get; set; }

        [DisplayName("New password")]
        [Required(ErrorMessage = "New password is required")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm new password")]
        [Required(ErrorMessage = "Confirm new password is required")]
        public string ConfirmNewPassword { get; set; }
    }
}
