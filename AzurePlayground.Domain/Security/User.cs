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

        public User() {
        }

        public User(string email, string password) : this() {
            Email = email;
            Password = new Password(password);
            Status = UserStatus.New;
            ActivationCode = GetNewActivationCode();
            AddEvent(UserEventType.Registered);
        }

        public void AddEvent(UserEventType userEventType) {
            UserEvents.Add(new UserEvent() {
                Date = DateTime.UtcNow,
                Type = userEventType
            });
        }

        public void LogIn() {
            // If we log in, the password reset is not needed anymore and leaving it is a security risk
            PasswordResetToken = TemporaryPassword.None;
            AddEvent(UserEventType.LoggedIn);
        }

        public void LogInFailed() {
            AddEvent(UserEventType.FailedLogIn);
        }

        public void LogOut() {
            AddEvent(UserEventType.LoggedOut);
        }

        public string GeneratePasswordResetToken() {
            var token = GetNewPasswordResetToken();

            PasswordResetToken = new TemporaryPassword(token);
            AddEvent(UserEventType.PasswordResetRequested);

            return token;
        }

        public void Activate() {
            Status = UserStatus.Active;
            ActivationCode = null;
            AddEvent(UserEventType.Activated);
        }

        public void ActivationFailed() {
            AddEvent(UserEventType.FailedActivation);
        }

        public void ChangePassword(string password) {
            Password = new Password(password);
            AddEvent(UserEventType.PasswordChanged);
        }

        public void ChangePasswordFailed() {
            AddEvent(UserEventType.FailedPasswordChange);
        }

        public void ResetPassword(string password) {
            PasswordResetToken = TemporaryPassword.None;
            Password = new Password(password);
            AddEvent(UserEventType.PasswordReset);
        }

        public void ResetPasswordFailed() {
            AddEvent(UserEventType.FailedPasswordReset);
        }

        public void GenerateActivationCode() {
            ActivationCode = GetNewActivationCode();
            AddEvent(UserEventType.ActivationCodeSent);
        }

        public void Deactivate() {
            Status = UserStatus.Inactive;
            AddEvent(UserEventType.Deactivated);
        }

        public void DeactivationFailed() {
            AddEvent(UserEventType.FailedDeactivation);
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

        protected int GetNewActivationCode() {
            using (var rng = new RNGCryptoServiceProvider()) {
                byte[] buffer = new byte[4];
                int activationCode = 0;

                while (activationCode < 10000) {
                    rng.GetBytes(buffer);

                    activationCode = BitConverter.ToInt32(buffer, 0);
                }

                return activationCode;
            }
        }
    }
}