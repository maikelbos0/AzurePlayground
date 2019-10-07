using System;
using System.Collections.Generic;

namespace AzurePlayground.Domain.Security {
    public class User {
        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public int PasswordHashIterations { get; set; }
        public int? ActivationCode { get; set; }
        public bool IsActive { get; set; }
        public Password PasswordResetToken { get; set; } = Password.None;
        public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}