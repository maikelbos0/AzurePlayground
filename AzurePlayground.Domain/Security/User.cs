using System.Collections.Generic;

namespace AzurePlayground.Domain.Security {
    public class User {
        public int Id { get; set; }
        public string Email { get; set; }
        public Password Password { get; set; }
        public int? ActivationCode { get; set; }
        public bool IsActive { get; set; }
        public TemporaryPassword PasswordResetToken { get; set; } = TemporaryPassword.None;
        public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}