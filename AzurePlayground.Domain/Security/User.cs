using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AzurePlayground.Domain.Security {
    public class User : Entity {
        public UserStatus Status { get; set; }
        public string Email { get; set; }
        public int? ActivationCode { get; set; }
        public Password Password { get; set; }
        public TemporaryPassword PasswordResetToken { get; set; } = TemporaryPassword.None;
        public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

        public void AddEvent(UserEventType userEventType) {
            UserEvents.Add(new UserEvent() {
                Date = DateTime.UtcNow,
                Type = userEventType
            });
        }

        public bool LogIn(string password) {
            var isValid = Status == UserStatus.Active && Password.Verify(password);

            if (isValid) {
                // If we log in, the password reset is not needed anymore and leaving it is a security risk
                PasswordResetToken = TemporaryPassword.None;
                AddEvent(UserEventType.LoggedIn);
            }
            else {
                AddEvent(UserEventType.FailedLogIn);
            }

            return isValid;
        }

        public bool LogOut() {
            var isValid = Status == UserStatus.Active;

            if (isValid) {
                AddEvent(UserEventType.LoggedOut);
            }

            return isValid;
        }

        public string ForgotPassword() {
            if (Status == UserStatus.Active) {
                var token = GetNewPasswordResetToken();

                PasswordResetToken = new TemporaryPassword(token);
                AddEvent(UserEventType.PasswordResetRequested);

                return token;
            }

            return null;
        }

        public bool Activate(int activationCode) {
            var isValid = Status == UserStatus.New && ActivationCode == activationCode;
                        
            if (isValid) {
                Status = UserStatus.Active;
                ActivationCode = null;
                AddEvent(UserEventType.Activated);
            }
            else {
                AddEvent(UserEventType.FailedActivation);
            }

            return isValid;
        }

        protected string GetNewPasswordResetToken() {
            using (var rng = new RNGCryptoServiceProvider()) {
                // Establish a maximum based on the amount of characters to prevent bias
                var tokenCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
                var maximumNumber = (byte.MaxValue / tokenCharacters.Length) * tokenCharacters.Length;
                var tokenBuilder = new StringBuilder();
                byte[] buffer = new byte[1];

                for (var i = 0; i < 20; i++) {
                    // Get a new number as long as we're at or over the maximum number
                    do {
                        rng.GetBytes(buffer);
                    }
                    while (buffer[0] >= maximumNumber);

                    tokenBuilder.Append(tokenCharacters[buffer[0] % tokenCharacters.Length]);
                }

                return tokenBuilder.ToString();
            }
        }
    }
}