using System;

namespace AzurePlayground.Domain.Security {
    public class TemporaryPassword : Password {
        public static TemporaryPassword None => new TemporaryPassword();

        public DateTime? ExpiryDate { get; private set; }

        protected TemporaryPassword() : base() {
        }

        public TemporaryPassword(string password, DateTime expiryDate) : base(password) {
            ExpiryDate = expiryDate;
        }
    }
}