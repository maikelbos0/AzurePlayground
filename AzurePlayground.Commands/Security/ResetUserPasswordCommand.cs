using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Utilities.Configuration;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class ResetUserPasswordCommand : BaseUserCommand, IResetUserPasswordCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;

        public ResetUserPasswordCommand(IPlaygroundContextFactory playgroundContextFactory, IMailClient mailClient, IAppSettings appSettings) : base(mailClient, appSettings) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserPasswordReset> Execute(UserPasswordReset parameter) {
            var result = new CommandResult<UserPasswordReset>();

            using (var context = _playgroundContextFactory.GetContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

                if (user == null || !user.IsActive) {
                    // Since there is no user input for email, the user is not responsible for these errors and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to reset password for {(user == null ? "non-existent" : "inactive")} user '{parameter.Email}'");
                }

                if (parameter.PasswordResetToken == null) {
                    // Since the token comes in a url, the user normally is not responsible for the error and we should not use the command result for feedback
                    throw new InvalidOperationException($"Attempted to reset password with missing token for user '{parameter.Email}'");
                }

                if (user.PasswordResetToken == Password.None) {
                    result.AddError(p => p.PasswordResetToken, "The password reset link has expired; please request a new one");
                }

                if (user.PasswordResetToken.ExpiryDate < DateTime.UtcNow) {
                    result.AddError(p => p.PasswordResetToken, "The password reset link has expired; please request a new one");
                }

                if (!parameter.NewPassword.Equals(parameter.ConfirmNewPassword, StringComparison.Ordinal)) {
                    result.AddError(p => p.ConfirmNewPassword, "New password and confirm new password must match");
                }

                if (result.Success) {
                    // TODO: move password validation to Password class
                    if (!user.PasswordResetToken.Hash.SequenceEqual(GetPasswordHash(parameter.PasswordResetToken, user.PasswordResetToken.Salt, user.PasswordResetToken.HashIterations))) {
                        // Since the token comes in a url, the user normally is not responsible for the error and we should not use the command result for feedback
                        throw new InvalidOperationException($"Attempted to reset password with incorrect token for user '{parameter.Email}'");
                    }

                    user.PasswordResetToken = Password.None;

                    user.PasswordSalt = GetNewPasswordSalt();
                    user.PasswordHashIterations = _passwordHashIterations;
                    user.PasswordHash = GetPasswordHash(parameter.NewPassword, user.PasswordSalt, user.PasswordHashIterations);

                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType_Id = UserEventType.PasswordReset.Id
                    });
                }
                else {
                    user.UserEvents.Add(new UserEvent() {
                        Date = DateTime.UtcNow,
                        UserEventType_Id = UserEventType.FailedPasswordReset.Id
                    });
                }

                context.SaveChanges();
            }

            return result;
        }
    }
}