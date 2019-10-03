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
        public byte[] PasswordResetTokenSalt { get; set; }
        public byte[] PasswordResetTokenHash { get; set; }
        public int? PasswordResetTokenHashIterations { get; set; }
        public DateTime? PasswordResetTokenExpiryDate { get; set; }
        public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}