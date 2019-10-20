using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzurePlayground.Models.Security {
    public sealed class ActivateUserModel {
        [DisplayName("Email address")]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "This email address is invalid")]
        public string Email { get; set; }

        [DisplayName("Activation code")]
        [Required(ErrorMessage = "Activation code is required")]
        public string ActivationCode { get; set; }
    }
}